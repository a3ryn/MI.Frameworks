/*
This source file is under MIT License (MIT)
Copyright (c) 2014 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Shared.Core.Common.DataAccess;
using Shared.Core.Common.Logging;
using Shared.Core.Common.Extensions;
using static Shared.Core.Common.auxfunc;

namespace Shared.Frameworks.DataAccess
{
    internal static class DataAdapterHelper
    {
        static DataAdapterHelper()
        {
            try
            {
                Log = LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch(Exception e)
            {
                Debug.WriteLine($"No logger found. Will execute without logging. {e.Message}");
            }
            
            PrintDataBeforeInsert = appSetting(DataAdapter.DA_AppSettings_LogDataBeforeInsertKey, false);
            Log?.Debug($"Setting {DataAdapter.DA_AppSettings_LogDataBeforeInsertKey} = {PrintDataBeforeInsert}");
        }

        private static readonly bool PrintDataBeforeInsert;

        private static readonly ILogger Log;

        #region Helpers
        internal static void Execute(Action a, string query, string connStr)
        {
            Log?.Debug($"START Executing query/stproc {query}. ConnStr = {connStr}.");
            var s = new Stopwatch();
            s.Start();
            try
            {
                a();
            }
            catch (Exception e)
            {
                Log?.Error($"Exception Executing Query/StProc {query}", e);
                throw;
            }
            finally
            {
                s.Stop();
                Log?.Debug($"END  Executing Query/StProc {query}. Time ellapsed: {s.ElapsedMilliseconds} [ms].");
            }
        }

        internal static IEnumerable<T> OpenConnectionAndReadData<T>(this IDbConnection conn, Func<SqlDataReader, T> entityAdapter, 
            SqlCommand cmd, Dictionary<string,SqlDbType> output = null)
            where T : new()
        {
            var result = new List<T>();
            try
            {
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var p = entityAdapter != null
                            ? entityAdapter(reader)
                            : reader.MapObject<T>();
                        if (p != null)
                        {
                            result.Add(p);
                        }
                    }

                    ProcessOutputValues(cmd, output, result);
                }
            }
            catch (Exception dae)
            {
                throw new ApplicationException("Data Access Exception: " + dae.Message, dae);
            }
            return result;
        }

        private static void ProcessOutputValues<T>(SqlCommand cmd, Dictionary<string, SqlDbType> output, List<T> result) where T : new()
        {
            if (output != null)
            {
                result.AddRange(
                    cmd.Parameters.Cast<SqlParameter>()
                        .Where(x => x.Direction == ParameterDirection.Output)
                        .Select(o => (T) Convert.ChangeType(o.Value, typeof (T)))
                        .Where(p => p != null));
            }
        }

        internal static List<SqlParameter> CreateSqlParameters(
            string stProcName,
            IEnumerable<Tuple<string, object, string>> argNamesWithValuesAndType, 
            IDictionary<string, SqlDbType> outputParams = null)
        {
            if (argNamesWithValuesAndType == null)
                return null;

            var sb = new StringBuilder(stProcName + " ");
            var sparams = new List<SqlParameter>();
            var paramAdded = false;

            argNamesWithValuesAndType.ToList().ForEach(t => ProcessParam(t.Item1, t.Item2, ref sb, ref paramAdded, sparams, pStructuredTypeName:t.Item3));
            if (outputParams == null) return sparams;

            foreach (var oparam in outputParams)
            {
                ProcessParam(oparam.Key, null, ref sb, ref paramAdded, sparams, true, dbType:oparam.Value);
            }
            return sparams;
        }

        private static void ProcessParam(
            string pName, object pValue, 
            ref StringBuilder sb,
            ref bool paramAdded,
            ICollection<SqlParameter> sqlParams,
            bool isOutput = false,
            string pStructuredTypeName = null,
            SqlDbType dbType = SqlDbType.Int)
        {
            if (paramAdded)
                sb.Append(" , ");
            sb.Append("@");
            sb.Append(pName);

            var isStructured = !string.IsNullOrEmpty(pStructuredTypeName);

            var sqlParam = isOutput
                ? new SqlParameter(pName, dbType) {Direction = ParameterDirection.Output}
                : new SqlParameter(pName, pValue ?? DBNull.Value);
            if (isStructured)
            {
                sqlParam.Value = CreateDataTableForStructuredType(pValue, pStructuredTypeName);
                sqlParam.TypeName = pStructuredTypeName;
                sqlParam.SqlDbType = SqlDbType.Structured;
            }
            sqlParams.Add(sqlParam);
            paramAdded = true;
        }

        private static DataTable CreateDataTableForStructuredType(object data, string typeName) 
        {
            //const bool captureSerializedData = true; //set to true only when you want to get the rows to test in SSMS

            
            if (!(data is IEnumerable))
            {
                Log?.Warn("Data is not IEnumerable!");
                return null;
            }
          
            var type = data.GetType().GetGenericArguments().FirstOrDefault();
            if (type == null) return null;
            var props =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(x => Reflection.GetCustomAttributes<DataMappingAttribute>(x).FirstOrDefault()?.Order ?? 0)
                    .ToList();

            var result = CreateDataTable(typeName, props);
            var sdata = PopulateDataTable(data, props, result);

            if (!PrintDataBeforeInsert) return result;

            LogUdttData(sdata);
            return result;
        }

        private static DataTable CreateDataTable(string typeName, List<PropertyInfo> props)
        {
            var result = new DataTable(typeName);
            foreach (var pi in props)
            {
                var pt = pi.PropertyType;
                var ut = Nullable.GetUnderlyingType(pt);
                if (pt.IsGenericType && ut != null) //nullable
                {
                    pt = ut;
                }
                result.Columns.Add(pi.Name, pt);
            }
            return result;
        }

        private static List<string> PopulateDataTable(object data, List<PropertyInfo> props, DataTable result)
        {
            var dataEnum = (IEnumerable)data;
            var sdata = new List<string>();
            foreach (var d in dataEnum)
            {
                object[] vals;
                try
                {
                    vals = props.Select(x => x.GetValue(d, null) ?? DBNull.Value).ToArray();
                    result.Rows.Add(vals);
                }
                catch (Exception e)
                {
                    Log?.Error("Exception building DataTable: " + e.Message);
                    throw;
                }
                if (PrintDataBeforeInsert)
                {
                    sdata.Add(string.Join(",", vals.Select(x => x is string ? $"'{x.ToString()}'" : x?.ToString() ?? "NULL")));
                }
            }
            return sdata;
        }

        private static void LogUdttData(ICollection<string> sdata)
        {
            try
            {
                if (sdata.Count == 0)
                {
                    throw new ApplicationException("Data to save to the DB is empty");
                }
                var s = $"( {string.Join(" ),\n( ", sdata)} )";
                Log?.Debug($"Data to be inserted: \n{s}");
            }
            catch (Exception ex) //we could get out of memory exception for very large data sets
            {
                Log?.Warn(
                    $"EXCEPTION: Could not log data (UDTT) before calling DB st proc (insert/update). Item count: {sdata.Count}. Exception Reason = {ex.Message}");
            }
        }

        #endregion
    }
}
