using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DataAccess;
using Shared.Frameworks.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using static Shared.Core.Common.DI.Mef;
using static Shared.Core.Common.auxfunc;
using Shared.Core.Common.Logging;
using System.Reflection;
using Shared.Core.Common.DI;
using System.IO;

namespace Candea.Frameworks.Tests
{

    [TestClass]
    public class DataAccessTests : TestBase
    {
        [ClassInitialize]
        public static void SetupClass(TestContext context)
        {
            //publish database first by running the batch file; comment this out if already published and no further updates
            var p = new Process();
            var startInfo = new ProcessStartInfo
            {       
                WorkingDirectory = ".\\..\\..\\..\\",
                FileName = "PublishSampleDb.bat",
                CreateNoWindow = false,
            };
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();

            //reset mef (settings) in case the other test classes used it and initialized its static data in a specific way
            new Mef();

            //setup logger (resolve)
            Log = LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            if (Log == null)
                Assert.Fail("Logger not found. Check configuration.");

            Log.Debug("Unit Test Class SETUP OK");
        }

        [ClassCleanup]
        public static void CleanupClass()
        {
            Log.Debug("Unit Test Class CLEANUP in progress.");

            //stop logger if exists
            LogResolver.CloseLogger();

            if (Directory.Exists("log"))
                Directory.Delete("log", true);
        }

        [TestInitialize]
        public void Setup()
        {
            var dbName = "Candea.SampleDb";
            var sqlCmdTimeout = 125; //overriding default to validate loading of config
            var logBeforeInsert = true; //same

            var dataAccessConfigJsonText =
                $@"{{
                ""dataAccess"": {{
                    ""defaultConnStr"": ""Data Source=localhost; Database = {dbName}; Integrated Security = true"",
                    ""sqlCommandTimeout"": {sqlCmdTimeout},
                    ""logDataBeforeInsert"":  {logBeforeInsert.ToString().ToLower()}
                    }}
                }}";

            File.WriteAllText(DataAdapter.DA_ConfigFileName, dataAccessConfigJsonText);

            var connStrName = appSetting<string>(DataAdapter.DA_XmlAppSettings_ConnStrNameKey);
            Assert.IsNotNull(connStrName);

            //resolve Data Access implementation
            adapter = Resolve<IDataAccess>(pattern: "Shared.*"); //pattern is optional but recommended, to avoid loading/reflecting over 3rd party DLLs
            if (adapter == null)
                Assert.Fail("Could not initialize adapter. No implementation found!");

            ValidateNonPublicStaticFieldValue(typeof(DataAdapter), "SqlCommandTimeout", sqlCmdTimeout);
            ValidateNonPublicStaticFieldValue(typeof(DataAdapter), "LogDataBeforeInsert", logBeforeInsert);
        }

        IDataAccess adapter = null;
        static ILogger Log;

        [TestMethod]
        public void AddAndRetrieveOneNodeWithExplicitQuery()
        {
            //Arrange
            var nodeName = "alpha";
            var nodeVal = Guid.NewGuid().ToString(); //to ensure uniqueness for retrieval
            var createdBy = "test";
            var insertQuery = "insert into [cfg].[Node] ([Name], [Value], [CreatedBy]) " +
                $"values ('{nodeName}', '{nodeVal}', '{createdBy}')";
            var retrieveQuery = $"select * from [cfg].[Node] where [Name]='{nodeName}' and [Value]='{nodeVal}'";
            IEnumerable<object> result = null;

            using (var ts = new TransactionScope()) //we will not commit this
            {
                //Act
                adapter.Get<object>(insertQuery); //using configured connection string (whose name is configured under appSettings)
                //we don't expect any data back;

                //Assert
                result = adapter.Get<object>(retrieveQuery);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count() == 1); //the newly inserted record 
            }

            //once transaction rolled back, the retrieval will not return any record
            result = adapter.Get<object>(retrieveQuery);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void AddAndRetrieveOneNodeWithStProcCallWithInputParamsAndAutoProxyTranslation()
        {
            //Arrange
            var nodeName = "alpha";
            var nodeVal = Guid.NewGuid().ToString(); //to ensure uniqueness for retrieval
            var createdBy = "test";

            var retrieveQueryWithTVF = $"select * from cfg.GetNodes() where [Name]='{nodeName}' and [Value]='{nodeVal}'";

            using (var ts = new TransactionScope())
            {
                //Act
                adapter.ExecStProc<object>("cfg.AddNode", new Dictionary<string, object>
                {
                    ["nodeName"] = nodeName,
                    ["nodeVal"] = nodeVal,
                    ["user"] = createdBy
                });

                //Assert
                var result = adapter.Get<NodeProxy>(retrieveQueryWithTVF); //automatic translation to NodeProxy elements
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count() == 1); //the newly inserted record 
                var record = result.Single();
                Assert.AreEqual(record.CreatedBy, createdBy);
                Assert.IsNull(record.UpdatedBy);
                Assert.AreEqual(record.NodeVal, nodeVal);
            }
        }

        [TestMethod]
        public void AddAndUpdateNodesWithStProcCallAndUdtt()
        {
            //Arrange
            var simpleImput = new List<(string name, string val)>
            {
                ("Alpha", "val1"),
                ("Beta", "val2"),
                ("Gamma", "val3")
            };
            var createdby = "test";

            var input = simpleImput.Select(x => new NodeTypeUdtt
            {
                Name = x.name,
                Value = x.val,
                CreatedBy = createdby,
            }).ToList();


            var retrieveQueryWithTVF = $"select * from cfg.GetNodes() where [Name] in ('{string.Join("', '", simpleImput.Select(x => x.name))}')";

            using (var ts = new TransactionScope())
            {
                //Act
                adapter.ExecStProcWithStructuredType<object>("cfg.AddOrUpdateNodes", new List<Tuple<string, object, string>>
                {
                    new Tuple<string, object, string>("nodes", input, "cfg.NodeType")
                });

                //Assert
                var result = adapter.Get<NodeProxy>(retrieveQueryWithTVF); //automatic translation to NodeProxy elements
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count() == 3); //the newly inserted record 

                Assert.IsTrue(result.All(x => x.CreatedBy == createdby));
                Assert.IsFalse(result.Select(x => x.Name).Except(simpleImput.Select(x => x.name)).Any());
                Assert.IsFalse(input.Select(x => x.Name).Except(simpleImput.Select(x => x.name)).Any());
            }
        }

        [TestMethod]
        [DataRow("alpha")]
        [DataRow(null)]
        public void AddAndDeleteNodesWithStProcCallAndUdttInput(string userName)
        {
            //Arrange
            var simpleImput = new List<(string name, string val)>
            {
                ("Alpha", "val1"),
                ("Beta", "val2"),
                ("Gamma", "val3")
            };
            var createdby = "test";

            var inputForAdd = simpleImput.Select(x => new NodeTypeUdtt
            {
                Name = x.name,
                Value = x.val,
                CreatedBy = createdby,
            }).ToList();


            var retrieveQueryWithTVFAfterAdd = 
                $"select * from cfg.GetNodes() where [Name] in " +
                $"('{string.Join("', '", simpleImput.Select(x => x.name))}')";
            string retrieveQueryWithTVFAfterDelete(IEnumerable<int> ids) => 
                $"select * from cfg.GetNodes() where [Id] in ({string.Join(", ", ids)})";

            using (var ts = new TransactionScope())
            {
                //Act - ADD
                adapter.ExecStProcWithStructuredType<object>("cfg.AddOrUpdateNodes", new List<Tuple<string, object, string>>
                {
                    //a single UDTT input param to the st proc with param name: @nodes and of type cfg.NodeType
                    new Tuple<string, object, string>("nodes", inputForAdd, "cfg.NodeType")
                });

                //Assert - ADD
                var result = adapter.Get<NodeProxy>(retrieveQueryWithTVFAfterAdd); //automatic translation to NodeProxy elements
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count() == 3); //the newly inserted record 

                Assert.IsTrue(result.All(x => x.CreatedBy == createdby));
                Assert.IsFalse(result.Select(x => x.Name).Except(simpleImput.Select(x => x.name)).Any());
                Assert.IsFalse(inputForAdd.Select(x => x.Name).Except(simpleImput.Select(x => x.name)).Any());

                //Act - DELETE
                //prepare IDs in UDDT of type dbo.IntVal (expected as input to the delete st proc)
                var inputForDelete = result.Select(x => new { x.Id }).ToList(); //UDTT is a simple list of int values (using anonymous type with inferred property name "Id")
                adapter.ExecStProcWithMixedTypes<object>("cfg.DeleteNodes", 
                    new List<(string, object, string)>
                    {
                        //a single UDTT input param to the st proc with param name: @nodeIds and of type dbo.IntVal
                        ("nodeIds", inputForDelete, "dbo.IntVal")
                    },
                    new Dictionary<string, object>
                    {
                        ["user"] = userName
                    });

                //Assert - DELETE
                result = adapter.Get<NodeProxy>(retrieveQueryWithTVFAfterDelete(inputForDelete.Select(x => x.Id))); //automatic translation to NodeProxy elements
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count() == (userName == null ? 3 : 0)); //nothing is deleted when user name input is null
            }
        }
    }
}
