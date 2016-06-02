using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Frameworks.DataAccess.CodeGen;
using Shared.Frameworks.DataAccess.CodeGen.Helpers;
using Shared.Frameworks.DataAccess.CodeGen.TemplateTypes;
using static Shared.Core.Common.corefunc;

namespace Shared.Frameworks.DataAccess.Tests
{
    [TestClass]
    public class CodeGenTests
    {
        private readonly Lazy<IEnumerable<Tuple<string, int>>> _tablesWithColumnCounts = 
            new Lazy<IEnumerable<Tuple<string, int>>>(GetAllTablesAndViewsWithColumnCounts);

        private const string ConnStr = @"Data Source=TXRH-3FPXX52\SQLEXPRESS;initial catalog=SystemManager7A;integrated security=True;MultipleActiveResultSets=True;App=AIMAPI";

        public class Pair<T1, T2>
        {
            public T1 First { get; set; }
            public T2 Second { get; set; }
        }

        [TestMethod]
        public void ReadSchema_Test()
        {
            var sqlConnStr = new SqlConnectionString(ConnStr);
            iter(_tablesWithColumnCounts.Value, t =>
            {
                Debug.WriteLine($"Processing metadata for table {t.Item1}");
                var result = sqlConnStr.GetResultSchema($"select top 0 * from {t.Item1}");
                Assert.IsNotNull(result);
                Assert.AreEqual(t.Item2, result.Count());
            });
        }

        [TestMethod]
        public void ReadSchema_Uddt_Test()
        {
            var sqlConnStr = new SqlConnectionString(ConnStr);
            var query = @"DECLARE @data dbo.AIMT_ClosureWithChildren;
                        select* from @data; ";
            var result = sqlConnStr.GetResultSchema(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(22,result.Count());
        }

        [TestMethod]
        public void PropertyTemplateReplace_Test()
        {
            var templateData = new PropertyTemplate
            { 
                Name = "Id",
                Type = "System.Int32",
                ColumnName = "DBID",
                ColumnIndex = 0
            };
            var template = templateData.Expand();
            Assert.IsNotNull(template);
            Assert.IsFalse(string.IsNullOrEmpty(template));
            Assert.IsFalse(template.Contains("$"));
        }

        [TestMethod]
        public void TypeTemplateReplace_Test()
        {
            var templateData = ConstructResourceTypeTemplate("imVision.API.Models");
            var code = templateData.Expand();
            Assert.IsNotNull(code);

            Assert.IsFalse(string.IsNullOrEmpty(code));
            Assert.IsFalse(code.Contains("$"));
            Assert.IsFalse(code.Contains("#"));
        }

        [TestMethod]
        public void TypeTemplate_Test_CustomLocation()
        {
            const string loc = @"C:\DEV\MIGit\SharedLibs\Shared.Frameworks.DataAccess.Tests\GeneratedCode";
            var templateData = ConstructResourceTypeTemplate("Shared.Frameworks.DataAccess.Tests.GeneratedCode");
            var fileName = $"{templateData.Typename}.cs";
            var code = templateData.Expand();
            code.SaveToFile(loc,fileName);
            Assert.IsNotNull(code);
            Assert.IsFalse(string.IsNullOrEmpty(code));
            Assert.IsFalse(code.Contains("$"));
            Assert.IsFalse(code.Contains("#"));
            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(loc, fileName)));
        }

        [TestMethod]
        public void GenerateCodeForAim()
        {
            const string loc = @"C:\DEV\MIGit\SharedLibs\Shared.Frameworks.DataAccess.Tests\GeneratedCode";
            const string ns = "Shared.Frameworks.DataAccess.Tests.GeneratedCode";
            CodeGenerator.GenerateProxiesFromDb(loc, ns, ConnStr, "AIM");

            foreach (var e in Enum.GetValues(typeof (CodeGenerator.TableType)))
            {
                var tableNames = new SqlConnectionString(ConnStr)
                    .GetDbObjectNames((CodeGenerator.TableType)e, "AIM");
                foreach (var tn in tableNames)
                    Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(loc, $"{tn}.cs")));
            }
        }


        private static IEnumerable<Tuple<string, int>> GetAllTablesAndViewsWithColumnCounts()
        {
            var sqlConnStr = new SqlConnectionString(ConnStr);
            var cmd = $@"select table_name as First, count(*) as Second
                        from {sqlConnStr.DatabaseName}.information_schema.columns
                        group by TABLE_CATALOG, TABLE_NAME";
            var result = sqlConnStr.Get<Pair<string, int>>(cmd)
                .Select(x => tuple(x.First, x.Second)) ;
            return result;
        }

        private static TypeTemplate ConstructResourceTypeTemplate(string @namespace)
        {
            var templateData = new TypeTemplate
            {
                Namespace = @namespace,
                Typename = "Resource",
                Properties = new List<ScriptTemplate<PropertyTemplate>>
                {
                    new PropertyTemplate
                    {
                        Name = "Id",
                        Type = "int",
                        ColumnName = "DBID",
                        ColumnIndex = 0
                    },
                    new PropertyTemplate
                    {
                        Name = "Name",
                        Type = "string",
                        ColumnName = "Name",
                        ColumnIndex = 1
                    },
                    new PropertyTemplate
                    {
                        Name = "Description",
                        Type = "string",
                        ColumnName = "Desc",
                        ColumnIndex = 2
                    },
                    new PropertyTemplate
                    {
                        Name = "CreatedDate",
                        Type = "DateTime",
                        ColumnName = "CreatedDate",
                        ColumnIndex = 3
                    },
                    new PropertyTemplate
                    {
                        Name = "IsVisible",
                        Type = "bool",
                        ColumnName = "IsVisible",
                        ColumnIndex = 4
                    }
                }
            };
            return templateData;
        }
    }
}
