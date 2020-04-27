using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.Config;
using Shared.Core.Common.DI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Candea.Frameworks.Tests
{
    using static ConfigFileNames;

    public class TestConfig
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public int Key5 { get; set; }

        public string[] Key3 { get; set; }

        public TestConfigSection Key4 { get; set; }
    }

    public class TestConfigSection
    {
        public string K41 { get; set; }
        public string K42 { get; set; }
    }

    internal static class ConfigFileNames
    {
        internal const string settingsFile = "appsettings.json";
        internal const string simpleKvpFile = "simpleKvpConfig.json";
    }

    [TestClass]
    public class ConfigTests
    {                
        [TestMethod]
        public void ReadStructureJsonConfigAsSimpleKvp()
        {
            var tc1 = AppSettings.FromFile(settingsFile);
            Assert.IsNotNull(tc1);
            Assert.IsTrue(tc1.All.Any());
            //top level settings:
            var (key1, val1) = ("key1", "val1");
            //key1: get via non generic Setting method
            var val1a = tc1.Setting(key1);
            Assert.AreEqual(val1a, val1);
            //key1: get via indexer
            var val1b = tc1[key1];
            Assert.AreEqual(val1b, val1);
            //key1: get via generic Setting method
            var val1c = tc1.Setting<string>(key1);
            Assert.AreEqual(val1c, val1);
        }

        [TestMethod]
        public void ReadNamedSectionFromJsonConfigIntoPoco()
        {
            var sec = AppSettings.FromFile<TestConfigSection>(settingsFile, section:"key4");
            Assert.IsNotNull(sec);
            Assert.AreEqual(sec.K41, "v41");
            Assert.AreEqual(sec.K42, "v42");

        }

        [TestMethod]
        public void ReadSimpleKvpJsonConfig()
        {
            var settings = AppSettings.FromFile(simpleKvpFile);
            Assert.IsNotNull(settings);
            Assert.AreEqual(settings.Count(), 3);
            var val1 = settings["key1"];
            Assert.AreEqual(val1, "val1");
            var val2 = settings.Setting<int>("key2");
            Assert.AreEqual(val2, 200);
            var val3 = settings.Setting<bool>("key3");
            Assert.IsTrue(val3);
        }
        
        [TestMethod]
        public void JsonFileNotFoundReadConfigFromXmlAppSettings()
        {
            var settings = AppSettings.FromFile("invalid.json");
            Assert.IsNotNull(settings);

            //find mef settings in AppSettings section of app.config of this project
            //NOTE: invidiual settings are accessed the same way regardless where they were loaded from (JSON, XML-AppSettings)
            var mef_assembliesPath = settings[Mef.MEF_AppSettings_AssembliesPathKey];
            Assert.IsFalse(string.IsNullOrWhiteSpace(mef_assembliesPath));
            var mef_csvSearchPattern = settings[Mef.MEF_AppSettings_CsvSearchPatternsKey];
            Assert.IsFalse(string.IsNullOrWhiteSpace(mef_csvSearchPattern));
        }

        [TestMethod]
        public void ReadEntireJsonConfigFileIntoPocoThenLoadSettingsFromPoco()
        {
            var tc = AppSettings.FromFile<TestConfig>(settingsFile);
            Assert.IsNotNull(tc);
            Assert.AreEqual(tc.Key1, "val1");
            Assert.IsNotNull(tc.Key4);
            Assert.AreEqual(tc.Key4.K42, "v42");
            Assert.AreEqual(tc.Key5, 22);

            //read same config file as KVPs (non-generic)
            var s = AppSettings.From(tc);
            Assert.IsNotNull(s);
            Assert.IsTrue(s.Count() == 5);
            var val4 = s.FirstOrDefault(x => x.Name == "Key4");
            Assert.IsNotNull(val4);
            var val5 = s.FirstOrDefault(x => x.Name.Equals("key5", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(val5);
            Assert.IsInstanceOfType(val5, typeof(AppSetting<int>));
            Assert.AreEqual(((AppSetting<int>)val5).Value, 22);
        }

    }

    [TestClass]
    public class MefConfigTests
    {
        //appsettings.json file of this project does not have mef settings in it; these unit tests will add/remove such settings
        //but it assumes that the appsettings.json was not modified manually to add these mef settings.

        [TestMethod]
        public void LoadMefFromDedicatedMefSettingsJsonFile()
        {
            //Arrange
            var assembliesPath = (val: "/abc", fieldName: "DefaultPath");
            var csvSearchPatterns = (val: "abc.*,def.*", fieldName: "SearchPatterns");

            var mefSettingsExistingText =
                File.Exists(Mef.MEF_ConfigFileName)
                ? File.ReadAllText(Mef.MEF_ConfigFileName)
                : null;
            
            var mefSettingsText = $@"{{""mef"": {{ ""assebliesPath"": ""{assembliesPath.val}"", ""csvSearchPatterns"": ""{csvSearchPatterns.val}"" }}  }}";

            try
            {
                File.WriteAllText(Mef.MEF_ConfigFileName, mefSettingsText);

                //Act

                //Assert
                ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
            }
            finally
            {
                //restore existing mef file
                if (mefSettingsExistingText != null)
                    File.WriteAllText(Mef.MEF_ConfigFileName, mefSettingsExistingText);

            }
        }

        [TestMethod]
        public void LoadFromAppSettingsJsonSection()
        {
            //Arrange
            var appSettingsJsonExistingText =
                File.Exists(settingsFile)
                ? File.ReadAllText(settingsFile)
                : null;

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

            var mefSettingsExistingText =
                File.Exists(Mef.MEF_ConfigFileName) //it already exists, we must remove it before testing with appsettings.json
                ? File.ReadAllText(Mef.MEF_ConfigFileName)
                : null;

            try
            {
                File.WriteAllText(settingsFile, appSettingsJsonNewText); //overwrite if exists
                File.Delete(Mef.MEF_ConfigFileName); //delete mef-dedicated json config file

                //Act

                //Assert
                ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
            }
            finally
            {
                //restore existing mef file
                if (appSettingsJsonExistingText != null)
                    File.WriteAllText(settingsFile, appSettingsJsonExistingText);

                if (mefSettingsExistingText != null)
                    File.WriteAllText(Mef.MEF_ConfigFileName, mefSettingsExistingText);
            }

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
    }
}
