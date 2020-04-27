using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.Config;
using Shared.Core.Common.DI;
using System;
using System.Linq;

namespace Candea.Frameworks.Tests
{
    using static ConfigFileNames;

    internal static class ConfigFileNames
    {
        internal const string settingsFile = "appsettings.json";
        internal const string simpleKvpFile = "simpleKvpConfig.json";
    }

    [TestClass]
    public class GeneralConfigTests
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
            var sec = AppSettings.FromFile<TestConfigSection>(settingsFile, section: "key4");
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
}
