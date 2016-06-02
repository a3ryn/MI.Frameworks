using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Shared.Core.Common.DataAccess;
using Shared.Core.Common.Logging;
using Shared.Core.Common.Extensions;

namespace Shared.Frameworks.DataAccess
{
    using static DataAdapterHelper;

    [Export(typeof (IDataAccess))]
    public class DataAdapter : IDataAccess
    {
        private static class ConfigConstants
        {
            //connection strings can always be passed in with each individual request to the DataAccess framework; these are here in case we consistently go against the same DB
            static ConfigConstants()
            {
                BasicConnStrId = ConfigurationManager.AppSettings["DataAccess.BasicConnStrName"];
                EfConnStrId = ConfigurationManager.AppSettings["DataAccess.EfConnStrName"];
            }

            internal static string BasicConnStrId { get; }
            internal static string EfConnStrId { get; }
            internal const string SqlConnectionTimeoutId = "SQLCommandTimeoutInSeconds";
        }

        private static readonly string DefaultConnectionStr;
        private static readonly int SqlCommandTimeout;

        private static readonly ILogger Log = LoggingManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static DataAdapter()
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConfigConstants.BasicConnStrId]?
                .ConnectionString; //basic

            //see if there isn't an EF-defined connection string:
            var entityConnectionString = ConfigurationManager.ConnectionStrings[ConfigConstants.EfConnStrId]?
                .ConnectionString;
            if (string.IsNullOrEmpty(entityConnectionString))
            {
                if (string.IsNullOrEmpty(connStr))
                    throw new ApplicationException(
                        "Missing connection string. No basic connection string with ID 'SystemManager' was found" +
                        "and not EntityFramework connection string with ID 'Entities' was found either.");

                DefaultConnectionStr = connStr;
            }
            else
            {
                DefaultConnectionStr = new EntityConnectionStringBuilder(entityConnectionString)
                    .ProviderConnectionString;
            }

            Log.Debug($"Default Connection String = {DefaultConnectionStr}");

            var cmdTimeoutStr = ConfigurationManager.AppSettings[ConfigConstants.SqlConnectionTimeoutId];
            if (!int.TryParse(cmdTimeoutStr, out SqlCommandTimeout))
                SqlCommandTimeout = 120;
        }

        public IEnumerable<T> Get<T>(string query,
            Func<SqlDataReader, T> entityAdapter = null, string connStr = null)
            where T : new()
        {
            connStr = connStr ?? DefaultConnectionStr;
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
                        Log.Debug($"ExecuteReader() returned {result.Count()} rows of data of type {typeof(T).Name}");
                    }
                }
            }, query, connStr);           
            return result;
        }

        public IEnumerable<T> ExecStProc<T>(string stProcName,
            Dictionary<string, object> input,
            List<string> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null)
            where T : new()
        {
            return ExecStProcWithStructuredType(stProcName,
                input.Select(kv => new Tuple<string, object, string>(kv.Key, kv.Value, null)),
                output, entityAdapter, connStr);
        }

        public IEnumerable<T> ExecStProcWithStructuredType<T>(string stProcName,
            IEnumerable<Tuple<string, object, string>> input,
            List<string> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null)
            where T : new()
        {
            connStr = connStr ?? DefaultConnectionStr;
            IEnumerable<T> result = null;

            Execute(() =>
            {
                var sqlParams = CreateSqlParameters(stProcName, input);

                using (var conn = new SqlConnection(connStr))
                {
                    using (var spcmd = new SqlCommand(stProcName, conn))
                    {

                        spcmd.CommandType = CommandType.StoredProcedure;
                        if (sqlParams != null)
                            spcmd.Parameters.AddRange(sqlParams.ToArray());
                        spcmd.CommandTimeout = SqlCommandTimeout;

                        result = conn.OpenConnectionAndReadData(entityAdapter, spcmd);
                    }
                }
            }, stProcName, connStr);
            return result;
        }
    }

    internal static class DataAdapterHelper
    {
        private static readonly ILogger Log = LoggingManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Helpers
        internal static void Execute(Action a, string query, string connStr)
        {
            Log.Debug($"START Executing query/stproc {query}. ConnStr = {connStr}.");
            var s = new Stopwatch();
            s.Start();
            try
            {
                a();
            }
            catch (Exception e)
            {
                Log.Error($"Exception Executing Query/StProc {query}", e);
                throw;
            }
            finally
            {
                s.Stop();
                Log.Debug($"END  Executing Query/StProc {query}. Time ellapsed: {s.ElapsedMilliseconds} [ms].");
            }
        }

        internal static IEnumerable<T> OpenConnectionAndReadData<T>(this IDbConnection conn, Func<SqlDataReader, T> entityAdapter, SqlCommand cmd)
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
                }
            }
            catch (Exception dae)
            {
                throw new ApplicationException("Data Access Exception: " + dae.Message, dae);
            }
            return result;
        }

        internal static List<SqlParameter> CreateSqlParameters(
            string stProcName,
            IEnumerable<Tuple<string, object, string>> argNamesWithValuesAndType, 
            IEnumerable<string> outputParams = null)
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
                ProcessParam(oparam, null, ref sb, ref paramAdded, sparams, true);
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

            var result = new DataTable(typeName);
            if (!(data is IEnumerable))
                return null;

            var type = data.GetType().GetGenericArguments().FirstOrDefault();
            if (type == null) return null;
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => Reflection.GetCustomAttributes<DataMappingAttribute>(x).FirstOrDefault()?.Order ?? 0).ToList();
            foreach (var pi in props)
            {
                result.Columns.Add(pi.Name, pi.PropertyType);
            }
            
            var sdata = new List<string>();
            foreach (var d in (IEnumerable)data)
            {
                var vals = props.Select(x => x.GetValue(d, null)).ToArray();
                result.Rows.Add(vals);

                //if (captureSerializedData)
                //{
                    sdata.Add(string.Join(",", vals.Select(x => x is string ? $"'{x.ToString()}'" : x?.ToString() ?? "NULL")));
                //}
            }

            //if (captureSerializedData)
            //{
                if (sdata.Count == 0)
                {
                    throw new ApplicationException("Data to save to the DB is empty");
                }
                var s = $"( {string.Join(" ),\n( ", sdata)} )";                
                Log.Debug($"Data to be inserted: \n{s}");
            //}
            return result;
        }
        #endregion
    }
}
