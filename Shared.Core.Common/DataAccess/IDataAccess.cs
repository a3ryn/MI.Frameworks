/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Shared.Core.Common.DataAccess
{
    public interface IDataAccess
    {
        /// <summary>
        /// Used to run a query - against a view, table, or TVF, returnining an enumeration of object instances of type T
        /// </summary>
        /// <typeparam name="T">Type of the result item</typeparam>
        /// <param name="query">The SQL query to be executed</param>
        /// <param name="entityAdapter">The custom translation delegate, to build the instances of type T of the result set</param>
        /// <param name="connStr">The DB connection string (if other than the one provided in the configuration file of the running application)</param>
        /// <returns></returns>
        IEnumerable<T> Get<T>(string query,  //query can be a SQL query, including a call to a TVF or a st proc
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null);// where T : new();

        /// <summary>
        /// Adapter for calling a stored procedure - that returns (or not) some result (possibly a data set) as well as output parameters (optionally)
        /// </summary>
        /// <typeparam name="T">The type of result item</typeparam>
        /// <param name="stProcName">The name of the stored procedure</param>
        /// <param name="input">Input arguments given as key-value pairs, with the keys being the expected names of the arguments</param>
        /// <param name="output">Optional output arguments, keyed by their expected names</param>
        /// <param name="entityAdapter">Optional translation delegate</param>
        /// <param name="connStr">The DB connection string (if other than the one provided in the configuration file of the running application)</param>
        /// <returns></returns>
        IEnumerable<T> ExecStProc<T>(string stProcName,
            Dictionary<string, object> input,
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null);//where T : new();

        /// <summary>
        /// Adapter for calling a stored procedure that takes as input a data set in the form of a UDTT - given as an enumeration of tuples.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stProcName"></param>
        /// <param name="input">A collection of inputs: simple or structured readonly UDTT inputs, one per input parameter.
        /// Each tuple contains the name of the input parameter (in the st proc), the actual parameter value or 
        /// collection of C# proxy instances/rows (if UDTT), and finally the 2-part name of the parameter type name (simple data type or UDTT type name.</param>
        /// <param name="output"></param>
        /// <param name="entityAdapter"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        IEnumerable<T> ExecStProcWithStructuredType<T>(string stProcName,
            IEnumerable<Tuple<string, object, string>> input,
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null);
            //where T : new();

        IEnumerable<T> ExecStProcWithMixedTypes<T>(string stProcName,
            IEnumerable<(string paramName, object udttItems, string udttTypeName)> udttInputs = null,
            Dictionary<string, object> simpleInputs = null,
            Dictionary<string, SqlDbType> output = null,
            Func<SqlDataReader, T> entityAdapter = null,
            string connStr = null);
            //where T : new();
    }
}
