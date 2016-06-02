using System.Collections.Generic;
using System.Linq;
using Shared.Frameworks.DataAccess.CodeGen.TemplateTypes;

namespace Shared.Frameworks.DataAccess.CodeGen.Helpers
{
    public static class CodeGenerator
    {
        public static void GenerateProxiesFromDb(string path, string @namespace, string connStr, IEnumerable<string> tableNames = null, IEnumerable<string> udttNames = null)
        {
            var sqlConnStr = new SqlConnectionString(connStr);

            sqlConnStr.GenerateProxiesFromDb(
                tableNames ?? sqlConnStr.GetDbObjectNames(TableType.TableOrView),
                udttNames ?? sqlConnStr.GetDbObjectNames(TableType.Udtt),
                path, @namespace
                );
        }

        public static void GenerateProxiesFromDb(string path, string @namespace, string connStr, string prefix = null, bool includeTablesAndViews = true, bool includeUdtts = true)
        {
            var sqlConnStr = new SqlConnectionString(connStr);

            sqlConnStr.GenerateProxiesFromDb(
                includeTablesAndViews ? sqlConnStr.GetDbObjectNames(TableType.TableOrView, prefix) : null,
                includeUdtts ? sqlConnStr.GetDbObjectNames(TableType.Udtt, prefix) : null,
                path, @namespace
                );
        }

        public class StringWrapper
        {
            public string Value { get; set; }
        }

        private static void GenerateProxiesFromDb(this SqlConnectionString sqlConnStr, IEnumerable<string> tableNames, IEnumerable<string> udttNames,
            string path, string @namespace)
        {
            sqlConnStr.GenerateProxiesFromDb(tableNames, TableType.TableOrView, path, @namespace);
            sqlConnStr.GenerateProxiesFromDb(udttNames, TableType.Udtt, path, @namespace);
        }

        private static void GenerateProxiesFromDb(this SqlConnectionString sqlConnStr, IEnumerable<string> dbObjectNames,
            TableType type, string path, string @namespace)
        {
            if (dbObjectNames == null) return;
            foreach (var name in dbObjectNames)
            {
                var script =
                    sqlConnStr
                    .GetResultSchema(string.Format(GetEmptyDataCommands[type], name))?
                    .ToTypeTemplate(name, @namespace)?
                    .Expand();

                script?.SaveToFile(path ?? @namespace.Replace('.', '\\'), name + ".cs");
            }
        }

        internal enum TableType
        {
            TableOrView,
            Udtt
        }

        /// <summary>
        /// Main queries to get names of Db objects for which to generate proxies. 
        /// The placeholder {0} is for extra clause in case of name filtering.
        /// </summary>
        private static readonly Dictionary<TableType, string> GetNamesCommands = new Dictionary<TableType, string>
        {
            [TableType.TableOrView] = "SELECT table_name AS Value FROM INFORMATION_SCHEMA.COLUMNS {0} GROUP BY TABLE_CATALOG, TABLE_NAME",
            [TableType.Udtt] = "SELECT name AS Value FROM SYS.TYPES WHERE IS_USER_DEFINED = 1 {0}"
        };

        /// <summary>
        /// The extra clause to be inserted in the main Db object name query, in case of filtering by name based on a prefix.
        /// The placeholder {0} is for the prefix to be used for filtering, if any.
        /// </summary>
        private static readonly Dictionary<TableType, string> GetNamesClauses = new Dictionary<TableType, string>
        {
            [TableType.TableOrView] = " WHERE table_name LIKE '{0}%'",
            [TableType.Udtt] = " AND name LIKE '{0}%'"
        };

        /// <summary>
        /// Queries to get an empty result set of the shape of the proxy to be generated.
        /// The placeholder {0} is for the Db object name to be queries (Table, View, or UDTT)
        /// </summary>
        private static readonly Dictionary<TableType, string> GetEmptyDataCommands = new Dictionary<TableType, string>
        {
            [TableType.TableOrView] = "SELECT TOP 0 * FROM {0}",
            [TableType.Udtt] = "DECLARE @data {0}; SELECT * FROM @data "
        };


        internal static IEnumerable<string> GetDbObjectNames(this SqlConnectionString sqlConnStr, TableType type, string prefix = null)
        {
            var clause = "";
            if (!string.IsNullOrEmpty(prefix))
            {
                clause = string.Format(GetNamesClauses[type], prefix);
            }
            var cmd = string.Format(GetNamesCommands[type], clause);
            return sqlConnStr.GetDbObjectNames(cmd);
        }

        private static IEnumerable<string> GetDbObjectNames(this SqlConnectionString sqlConnStr, string cmd)
        {
            var result = sqlConnStr.Get<StringWrapper>(cmd).Select(x => x.Value);

            return result;
        }

        private static TypeTemplate ToTypeTemplate(this IEnumerable<SchemaTableRecord> columnData, string typeName, string @namespace)
        {
            var propData = columnData.Select(x =>
                new PropertyTemplate
                {
                    Name = x.ColumnName,
                    Type = x.DataType.FullName + (x.AllowDBNull && x.DataType.IsValueType ? "?" : ""),
                    ColumnName = x.ColumnName,
                    ColumnIndex = x.ColumnOrdinal
                });
            var templateData = 
                new TypeTemplate
                {
                    Namespace = @namespace,
                    Typename = typeName,
                    Properties = propData
                };
            return templateData;
        }

        public static void SaveToFile(this string source, string path, string fileName)
        {
            if (string.IsNullOrEmpty(source)) return;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            System.IO.File.WriteAllText(System.IO.Path.Combine(path, fileName), source);
        }
    }
}
