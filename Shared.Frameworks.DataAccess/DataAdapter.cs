/*
This source file is under MIT License (MIT)
Copyright (c) 2014 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Shared.Core.Common.DataAccess;
using Shared.Core.Common.Logging;
using static Shared.Core.Common.auxfunc;

namespace Shared.Frameworks.DataAccess
{
    using static DataAdapterHelper;
    using Core.Common.Config;

    [Export(typeof (IDataAccess))]
    public class DataAdapter : IDataAccess
    {
        #region Exposing appSettings key and other data usedby this framework, making them discoverable via the API
        /// <summary>
        /// XML AppSettings KEY name for Data Access configuration. Set the value to the NAME of the connection string
        /// to be used as default, to set up the adapter. 
        /// The connection string can be overridden for individual Data Access calls as needed, by passing it in to the 
        /// method call, but if not provided, the Data Adapter (IDataAccess implementation) is going to use 
        /// the connection string identified by the name specified under this app setting key.
        /// EF-configured connection strings are not supported. This framework only supports basic connection strings 
        /// found under  the &lt; connectionStrings &gt; section in the configuration file of the startup project.
        /// </summary>
        public const string DA_XmlAppSettings_ConnStrNameKey = "DataAccess.ConnStrName";

        /// <summary>
        /// XML AppSettings KEY name for the Data Access configuration. Set the value to the desired SQL timeout for connecting 
        /// to a given SQL Server instance. If not provided, the default value used is 120 seconds.
        /// </summary>
        public const string DA_XmlAppSettings_SqlTimeoutKey = "DataAccess.SQLCommandTimeoutInSeconds";

        /// <summary>
        /// XML AppSettings KEY name for the Data Access configuration. Set the value to TRUE to print to log the input data 
        /// (in case of structured/UDTT) to stored procedures. If not configured, default value used is false.
        /// </summary>
        public const string DA_XmlAppSettings_LogDataBeforeInsertKey = "DataAccess.LogDataBeforeInsert";

        /// <summary>
        /// Default name of the connection string to be used by this framework globally, when not overridden in specific method calls.
        /// If using XML application settings, to use a different identifier for the connection string, set the ConnStrName configuration value accordingly.
        /// For JSOn configuration (appsettings.json or the dedicated dataAccessSettings.json, the actual connection string is configured under 'defaultConnStr'.
        /// </summary>
        public const string DA_XMLAppSettings_DefaultConnStringName = "Default";

        /// <summary>
        /// Name of the dedicated JSON configuration file used by the framework, if this file exists. It will be searched for first, then
        /// the global appsettings.json, and finally the XML app.config file.
        /// </summary>
        public const string DA_ConfigFileName = "dataAccessSettings.json";
        #endregion


        private static string DefaultConnString;
        private static int SqlCommandTimeout = 120;
        internal static bool LogDataBeforeInsert;

        internal static readonly ILogger Log = null;

        static DataAdapter()
        {
            Log = ResolveLogger();
        }

        public DataAdapter()
        {
            Init();
            LogConfig();
        }

        public DataAdapter(string connectionString, int commandTimeout = 120, bool logDateBeforeInsert = false)
        {
            DefaultConnString = connectionString;
            SqlCommandTimeout = commandTimeout;
            LogDataBeforeInsert = logDateBeforeInsert;
            LogConfig();
        }

        private static void Init()
        {
            var settings = AppSettings.FromFile<DataAccessConfig>(DA_ConfigFileName, "dataAccess");
            if (settings != null) //this means deserialization of JSON content or section succeeded and the POCO is populated
            {
                DefaultConnString = settings.DefaultConnStr;
                SqlCommandTimeout = settings.SqlCommandTimeout;
                LogDataBeforeInsert = settings.LogDataBeforeInsert;
            }
            else //no json file with MEF config was found; trying to retrieve from XML AppSettings section, if any
            {
                DefaultConnString = connString(appSetting(DA_XmlAppSettings_ConnStrNameKey, DA_XMLAppSettings_DefaultConnStringName));
                SqlCommandTimeout = appSetting(DA_XmlAppSettings_SqlTimeoutKey, 120);
                LogDataBeforeInsert = appSetting(DA_XmlAppSettings_LogDataBeforeInsertKey, false);               
            }
        }

        private void LogConfig()
            => Log?.Debug($"Default Connection String = {DefaultConnString ?? "None configured. Must be provided with every query as input."}. " +
                $"SQL timeout = {SqlCommandTimeout} sec. Log input data (for insert) = {LogDataBeforeInsert}.");

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
