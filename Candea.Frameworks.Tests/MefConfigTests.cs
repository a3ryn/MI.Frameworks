using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DI;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Candea.Frameworks.Tests
{
    using static ConfigFileNames;

    [TestClass]
    public class MefConfigTests
    {
        //appsettings.json file of this project does not have mef settings in it; these unit tests will add/remove such settings
        //but it assumes that the appsettings.json was not modified manually to add these mef settings.
        static readonly string ExistingAppSettingsJsonText = 
            File.Exists(settingsFile)
                ? File.ReadAllText(settingsFile)
                : null;

        static readonly string ExistingMefSettingsJsonText =
            File.Exists(Mef.MEF_ConfigFileName)
                ? File.ReadAllText(Mef.MEF_ConfigFileName)
                : null;

        [TestCleanup]
        public void Cleanup() =>
            RestoreConfigFile(
                    new[]
                    {
                        (settingsFile, ExistingAppSettingsJsonText),
                        (Mef.MEF_ConfigFileName, ExistingMefSettingsJsonText)
                });
        

        [TestMethod]
        public void LoadMefFromDedicatedMefSettingsJsonFile()
        {
            //Arrange
            var assembliesPath = (val: "/abc", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "abc.*,def.*", fieldName: "SearchPatterns");

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
            var assembliesPath = (val: "/abc", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "abc.*,def.*", fieldName: "SearchPatterns");

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

        private static void ValidateMefConfiguration((string val, string fieldName) assembliesPath, (string val, string fieldName) csvSearchPatterns)
        {
            //check the two properties (private static readonly) via reflection:

            //assemblies path (simple string)
            var f1 = typeof(Mef).GetField(assembliesPath.fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(f1, "Field name or accessibility was changed!");
            Assert.AreEqual(assembliesPath.val, string.Join(",", f1.GetValue(null)));

            //csvSearchPatterns
            var f2 = typeof(Mef).GetField(csvSearchPatterns.fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(f2, "Field name or accessibility was changed!");
            Assert.AreEqual(csvSearchPatterns.val, string.Join(",", f2.GetValue(null) as string[]));
        }

        private static void RestoreConfigFile(IEnumerable<(string filePath, string text)> filesWithContentToRestore)
        {
            foreach((string filePath, string text) in filesWithContentToRestore)
            {
                if (text != null) //only when it is truly not null do we restore, even it is an empty string
                    File.WriteAllText(filePath, text);
            }
        }
    }
}
