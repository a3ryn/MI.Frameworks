﻿/*
This source file is under MIT License (MIT)
Copyright (c) 2014 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using static System.Diagnostics.Debug;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Shared.Core.Common.DataAccess;
using Shared.Core.Common.Logging;
using static Shared.Core.Common.corefunc;

namespace Shared.Frameworks.DataAccess
{
    /// <summary>
    /// Utility class for object translations.
    /// A: from rows and columns, as retrieved from the DB (using <see cref="SqlDataReader"/>) to a (generic) strongly typed instance.
    /// B: from an array of strongly typed instances to a UDTT proxy, for stored proc calls that use UDTTs as input
    /// </summary>
    internal static class EntityMapper
    {
        static EntityMapper()
        {
            try
            {
                Log = LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }catch(Exception e)
            {
                WriteLine($"Logger could not be resolved. Will continue without logging. INitialization exception: {e.Message}");
            }
        }

        private static readonly ILogger Log = null;

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>> PropertyInfosByType 
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>>(); //lazy hydration

        internal static object Map(Type t, SqlDataReader reader, Dictionary<Guid, List<Tuple<object, string>>> navProps)
        {
            return InvokeGenericMethod(t, nameof(MapObject), new object[] { reader, navProps, null });
        }

        private const string TranslationError =
            "[{0} translation level]: Could not translate property associated with column {1}.";

        internal static T MapObject<T>(this SqlDataReader reader, 
            //Dictionary<Guid, List<Tuple<object, string>>> navProps = null, 
            Dictionary<string, SqlDbType> output = null) 
            //where T : new()
        {
            var entity = CreateInstance<T>();

            Execute(() =>
            {
                var propertyInfos = PropertyInfosByType[typeof (T)]; //already populated

                var outputColumnName = (output != null && output.Any()) ? output.First().Key : null;
                var numberOfColumns = reader.FieldCount;

                T tmpEntity;
                if (CheckAndProcessScalarOutput(reader, output, outputColumnName, numberOfColumns, out tmpEntity))
                {
                    entity = tmpEntity;
                    return;
                }

                for (var columnIndex = 0; columnIndex < numberOfColumns; columnIndex++)
                {
                   var kvp = GetColumnNameAndValue(reader, columnIndex);
                    var columnName = kvp.Key;
                    var val = kvp.Value;

                    if (!propertyInfos.ContainsKey(columnName)) continue;

                    try
                    {
                        propertyInfos[columnName].SetValue(entity, val, null);
                    }
                    catch (Exception)
                    {
                        Log?.Warn(string.Format(TranslationError, "1st",columnName));
                        TrySetPropertyUsingConvert(propertyInfos, columnName, val, entity);
                    }
                }
            }, "MapObject<T>");

            return entity;
        }

        internal static T MapObject<T>(this DataRow dataRow) //where T : new()
        {
            var entity = CreateInstance<T>();

            Execute(() =>
            {
                var propertyInfos = PropertyInfosByType[typeof(T)]; //already populated

                var numberOfColumns = dataRow.Table.Columns.Count;

                for (var columnIndex = 0; columnIndex < numberOfColumns; columnIndex++)
                {
                    var kvp = 
                        new KeyValuePair<string, object>(
                            dataRow.Table.Columns[columnIndex].ColumnName, 
                            dataRow[columnIndex]);
                    var columnName = kvp.Key;
                    var val = kvp.Value;

                    if (!propertyInfos.ContainsKey(columnName)) continue;

                    TrySetPropertyUsingConvert(propertyInfos, columnName, val, entity);
                }
            }, "MapObject<T>");

            return entity;
        }

        #region Helpers
        private static object CreateInstance(Type t)
        {
            return InvokeGenericMethod(t, nameof(CreateInstance), new object[] { });
        }

        private static T CreateInstance<T>() //where T : new()
        {
            var entity = default(T);
            var t = typeof (T);

            Execute(() =>
            {
                entity = Activator.CreateInstance<T>();

                if (PropertyInfosByType.ContainsKey(t)) return;// entity;

                SavePropertyInfosToMap<T>(t);
            });
            return entity;
        }

        private static void SavePropertyInfosToMap<T>(Type t) //where T : new()
        {
            var propertyInfoMap = new ConcurrentDictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            iter(t.GetProperties(), pi =>
            {
                var propertyName = pi.Name;
                var attrib = pi.GetCustomAttributes<DataMappingAttribute>(false).FirstOrDefault();
                var columnName = attrib?.Name ?? propertyName;

                propertyInfoMap.TryAdd(columnName, pi);
            }, true);
            PropertyInfosByType.GetOrAdd(t, new ReadOnlyDictionary<string, PropertyInfo>(propertyInfoMap));
        }

        private static object InvokeGenericMethod(Type t, string methodName, object[] objects)
        {
            var method = typeof(EntityMapper)
                            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            var genericMethod = method.MakeGenericMethod(t);
            return genericMethod.Invoke(null, objects);
        }

        private static bool CheckAndProcessScalarOutput<T>(SqlDataReader reader, 
            Dictionary<string, SqlDbType> output, 
            string outputColumnName,
            int numberOfColumns, 
            out T entity) 
            //where T : new()
        {
            entity = default;
            var kvp = GetColumnNameAndValue(reader, 0);
            if ((string.IsNullOrEmpty(outputColumnName) ||
                 !kvp.Key.Equals(outputColumnName, StringComparison.CurrentCultureIgnoreCase)) &&
                (output != null || numberOfColumns != 1))
                return false;
            var val = kvp.Value;
            entity = (val is T) ? (T)val : default;
            return true;
        }

        private static KeyValuePair<string, object> GetColumnNameAndValue(IDataRecord reader, int columnIndex)
        {
            return new KeyValuePair<string, object>(
                reader.GetName(columnIndex),
                !reader.IsDBNull(columnIndex)
                    ? reader.GetValue(columnIndex)
                    : null);
        }

        private static void TrySetPropertyUsingConvert<T>(
            IReadOnlyDictionary<string, PropertyInfo> propertyInfos, 
            string columnName, object val, T entity)
            //where T : new()
        {
            try
            {
                //try converting to specific type
                var propertyInfo = propertyInfos[columnName];
                var targetType = propertyInfo.PropertyType;
                if (val == DBNull.Value)
                {
                    return;
                }

                var sourceType = val.GetType();

                if (targetType.IsGenericType)
                {
                    targetType = targetType.GetGenericArguments().First();
                }
                var propertyVal = val;
                if (sourceType.Name.Equals("RuntimeType", StringComparison.InvariantCultureIgnoreCase))
                    propertyVal = (Type)sourceType.GetProperty("UnderlyingSystemType").GetValue(val, null);
                else if (targetType != sourceType)
                {
                    WriteLine($"Conversion needed for column {columnName}");
                    propertyVal = Convert.ChangeType(val, targetType);
                }
                propertyInfos[columnName].SetValue(entity, propertyVal, null);
            }
            catch (Exception e)
            {
                //ignore
                var msg = string.Format(TranslationError, "2nd", columnName);
                Log?.Warn($"{msg} Exception: {e.Message}.");
            }
        }
        #endregion


        private static void Execute(Action a, string mtd = null)
        {
            //var s = new Stopwatch();
            //s.Start();
            a();
            //s.Stop();
            //Log?.Debug($"Execute{mtd}: {s.ElapsedMilliseconds} [ms]");
        }
    }
}
