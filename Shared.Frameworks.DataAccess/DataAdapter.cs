/*
This source file is under MIT License (MIT)
Copyright (c) 2014 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    using static DataAdapterHelper;

    [Export(typeof (IDataAccess))]
    public class DataAdapter : IDataAccess
    {
        #region Exposing appSettings key usedby this framework, making them discoverable via the API
        /// <summary>
        /// AppSettings KEY name for Data Access configuration. Set the value to the NAME of the connection string
        /// to be used as default, to set up the adapter. 
        /// The connection string can be overridden for individual Data Access calls as needed, by passing it in to the 
        /// method call, but if not provided, the Data Adapter (IDataAccess implementation) is going to use 
        /// the connection string identified by the name specified under this app setting key.
        /// EF-configured connection strings are not supported. This framework only supports basic connection strings 
        /// found under  the &lt; connectionStrings &gt; section in the configuration file of the startup project.
        /// </summary>
        public const string DA_AppSettings_ConnStrNameKey = "DataAccess.ConnStrName";

        /// <summary>
        /// AppSettings KEY name for the Data Access configuration. Set the value to the desired SQL timeout for connecting 
        /// to a given SQL Server instance. If not provided, the default value used is 120 seconds.
        /// </summary>
        public const string DA_AppSettings_SqlTimeoutKey = "DataAccess.SQLCommandTimeoutInSeconds";


        public const string DA_AppSettings_LogDataBeforeInsertKey = "DataAccess.LogDataBeforeInsert";
        #endregion

        private static class ConfigConstants
        {
            //connection strings can always be passed in with each individual request to the DataAccess framework; these are here in case we consistently go against the same DB
            internal static string BasicConnStrId = appSetting<string>(DA_AppSettings_ConnStrNameKey);
            internal const string SqlConnectionTimeoutId = DA_AppSettings_SqlTimeoutKey;
        }

        private string DefaultConnString { get; }
        private int SqlCommandTimeout { get; }

        private static readonly ILogger Log = null;

        static DataAdapter() => ResolveLogger();

        public DataAdapter() : this(
            connString(ConfigConstants.BasicConnStrId),
            appSetting(ConfigConstants.SqlConnectionTimeoutId, 120))
        {
        }

        public DataAdapter(string connectionString, int commandTimeout = 120)
        {
            DefaultConnString = connectionString;
            SqlCommandTimeout = commandTimeout;
            LogConfig();
        }

        private void LogConfig()
            => Log?.Debug($"Default Connection String = {DefaultConnString ?? "None configured. Must be provided with every query as input."}. " +
                $"SQL timeout = {SqlCommandTimeout}s.");

        private static ILogger ResolveLogger()
        {
            try
            {
                return LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Logger could not be resolved. Will continue without logging. INitialization exception: {e.Message}");
            }
            return default;
        }

        public IEnumerable<T> Get<T>(string query,
            Func<SqlDataReader, T> entityAdapter = null, string connStr = null)
            where T : new()
        {
            connStr = connStr ?? DefaultConnString;
            IEnumerable<T> result = null;

            Execute(() =>
            {
                using (var conn = new SqlConnection(connStr))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.CommandText = query;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        cmd.CommandTimeout = SqlCommandTimeout;

                        result = conn.OpenConnectionAndReadData(entityAdapter, cmd);
                        Log?.Debug($"ExecuteReader() returned {result.Count()} rows of data of type {typeof(T).Name}");
                    }
                }
            }, query, connStr);           
            return result;
        }

        public IEnumerable<T> ExecStProc<T>(string stProcName,
            Dictionary<string, object> input,
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null)
            where T : new() =>
        
            ExecStProcWithStructuredType(stProcName,
                input.Select(kv => new Tuple<string, object, string>(kv.Key, kv.Value, null)),
                output, entityAdapter, connStr);
        

        public IEnumerable<T> ExecStProcWithStructuredType<T>(string stProcName,
            IEnumerable<Tuple<string, object, string>> input,
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null)
            where T : new()
        {
            connStr = connStr ?? DefaultConnString;
            IEnumerable<T> result = null;

            Execute(() =>
            {
                var sqlParams = CreateSqlParameters(stProcName, input, output);

                using (var conn = new SqlConnection(connStr))
                {
                    using (var spcmd = new SqlCommand(stProcName, conn))
                    {

                        spcmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParams != null)
                            spcmd.Parameters.AddRange(sqlParams.ToArray());
                        spcmd.CommandTimeout = SqlCommandTimeout;

                        result = conn.OpenConnectionAndReadData(entityAdapter, spcmd, output);
                    }
                }
            }, stProcName, connStr);
            return result;
        }
    }
}
