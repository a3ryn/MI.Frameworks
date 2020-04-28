using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Shared.Frameworks.DataAccess;

namespace Candea.Frameworks.Tests
{
    using static ConfigFileNames;

    public abstract class TestBase
    {
        //appsettings.json file of this project does not have mef settings in it; these unit tests will add/remove such settings
        //but it assumes that the appsettings.json was not modified manually to add these mef settings.
        protected static readonly string ExistingAppSettingsJsonText =
            File.Exists(settingsFile)
                ? File.ReadAllText(settingsFile)
                : null;

        protected static readonly string ExistingMefSettingsJsonText =
            File.Exists(Mef.MEF_ConfigFileName)
                ? File.ReadAllText(Mef.MEF_ConfigFileName)
                : null;

        protected static readonly string ExistingDataAccessSettingsJsonText =
            File.Exists(DataAdapter.DA_ConfigFileName)
                ? File.ReadAllText(DataAdapter.DA_ConfigFileName)
                : null;


        [TestCleanup]
        public void Cleanup() =>
            RestoreConfigFile(
                    new[]
                    {
                        (settingsFile, ExistingAppSettingsJsonText),
                        (Mef.MEF_ConfigFileName, ExistingMefSettingsJsonText)
                });

        private static void RestoreConfigFile(IEnumerable<(string filePath, string text)> filesWithContentToRestore)
        {
            foreach ((string filePath, string text) in filesWithContentToRestore)
            {
                if (text != null) //only when it is truly not null do we restore, even it is an empty string
                    File.WriteAllText(filePath, text);
            }
        }

        protected static void ValidateNonPublicStaticFieldValue(Type t, string fieldName, object expectedFieldValue, Func<object,object> fieldValueAdapter = null)
        {
            var f1 = t.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(f1, "Field name or accessibility was changed!");

            var actualValue = fieldValueAdapter == null 
                ? f1.GetValue(null) 
                : fieldValueAdapter(f1.GetValue(null));
            Assert.AreEqual(expectedFieldValue, actualValue);
        }
    }

    [TestClass]
    public class MefConfigTests : TestBase
    {
        [TestMethod]
        public void LoadMefFromDedicatedMefSettingsJsonFile()
        {
            //Arrange
            var assembliesPath = (val: "/abc1", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "pat11.*,pat12.*", fieldName: "SearchPatterns");

            var mefSettingsText = $@"{{""mef"": {{ ""assebliesPath"": ""{assembliesPath.val}"", ""csvSearchPatterns"": ""{csvSearchPatterns.val}"" }}  }}";

            File.WriteAllText(Mef.MEF_ConfigFileName, mefSettingsText);

            //Act
            new Mef(); //only for testing; not the usual way of using this; this is to force Init() and reading configuration settings from wherever provided

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        [TestMethod]
        public void LoadFromAppSettingsJsonSection()
        {
            var assembliesPath = (val: "/abc2", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "pat21.*,pat22.*", fieldName: "SearchPatterns");

            var mefSettingsText = $@"""mef"": {{ ""assebliesPath"": ""{assembliesPath.val}"", ""csvSearchPatterns"": ""{csvSearchPatterns.val}"" }}";

            var appSettingsJsonNewText = //create a new appsettings.json file that has mef settings as a section inside, to use for deserialization
                $@"{{
                      ""key1"": ""val1"",
                      ""key2"": 200,
                      ""key3"":  true,
                      {mefSettingsText},
                      ""key4"": {{ ""k41"": ""v41"", ""k42"": ""v42""  }}
                    }}";


            File.WriteAllText(settingsFile, appSettingsJsonNewText); //overwrite if exists
            File.Delete(Mef.MEF_ConfigFileName); //delete mef-dedicated json config file

            //Act
            new Mef(); //only for testing; not the usual way of using this; this is to force Init() and reading configuration settings from wherever provided

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        [TestMethod]
        public void InitMefWithInstanceCtorWithParameters()
        {
            //Arrange
            var assembliesPath = (val: "/abc3", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "pat31.*,pat32.*,pat33.*", fieldName: "SearchPatterns");

            //Act
            new Mef(assembliesPath.val, csvSearchPatterns.val); //init with params; static CTOR was not yet invoked

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        [TestMethod]
        public void InitMefWithInstanceCtorWithParametersAfterStaticCtorInitFromConfigFile()
        {
            //Arrange
            var assembliesPath = (val: "/abc4", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "pat4.*", fieldName: "SearchPatterns");

            //Act - try resolve some assembly only to invoke static CTOR to read from config file values other than what is above
            try
            {
                Mef.Resolve<IComparable>(); //will fail
            }
            catch { }
            new Mef(assembliesPath.val, csvSearchPatterns.val); //init with params that override what the static ctor set above

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        private static void ValidateMefConfiguration((string val, string fieldName) assembliesPath, (string val, string fieldName) csvSearchPatterns)
        {
            //check the two properties (private static readonly) via reflection:

            //assemblies path (simple string)
            ValidateNonPublicStaticFieldValue(typeof(Mef), assembliesPath.fieldName, assembliesPath.val);

            //csvSearchPatterns
            ValidateNonPublicStaticFieldValue(typeof(Mef), csvSearchPatterns.fieldName, csvSearchPatterns.val,
                v => string.Join(",", v as string[]));
        }

        
    }
}
