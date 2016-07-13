using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Shared.Core.Common.DataAccess
{
    public interface IDataAccess
    {
        IEnumerable<T> Get<T>(string query,  //query can be a SQL query, including a call to a TVF or a st proc
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null) where T : new();

        IEnumerable<T> ExecStProc<T>(string stProcName, 
            Dictionary<string, object> input, 
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null, 
            string connStr = null) where T : new();

        IEnumerable<T> ExecStProcWithStructuredType<T>(string stProcName,
            IEnumerable<Tuple<string, object, string>> input,
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null)
            where T : new();
    }
}
