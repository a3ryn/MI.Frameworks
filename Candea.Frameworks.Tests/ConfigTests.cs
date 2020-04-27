using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shared.Core.Common.Config;
using Shared.Core.Common.Serialization;

namespace Candea.Frameworks.Tests
{
    public class TestConfig
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public int key5 { get; set; }

        public string[] Key3 { get; set; }

        public TestConfigSection Key4 { get; set; }
    }

    public class TestConfigSection
    {
        public string k41 { get; set; }
        public string k42 { get; set; }
    }

    [TestClass]
    public class ConfigTests
    {
        const string settingsFile = "appsettings.json";
                
        [TestMethod]
        public void Test1()
        {
            var tc = new TestConfig();
            Assert.IsNotNull(tc);

            var tc1 = AppSettings.FromFile(settingsFile);
            Assert.IsNotNull(tc1);
        }

        [TestMethod]
        public void Test2()
        {
            var sec = AppSettings.FromFile<TestConfigSection>(settingsFile, "key4");
            Assert.IsNotNull(sec);
        }

        [TestMethod]
        public void Test3()
        {
            var tc = AppSettings.FromFile<TestConfig>(settingsFile);
            Assert.IsNotNull(tc);
            Assert.AreEqual(tc.Key1, "val1");
            Assert.IsNotNull(tc.Key4);
            Assert.AreEqual(tc.Key4.k42, "v42");

            var s = AppSettings.From(tc).ToList();
            Assert.IsNotNull(s);
            Assert.IsTrue(s.Count() == 5);
            var sc = s.FirstOrDefault(x => x.Name == "Key4");
            Assert.IsNotNull(sc);

        }


        [TestMethod]
        public void LoadConfigFromDefaultAppSettingsJson()
        {
            var configText = File.ReadAllText(settingsFile);


            //var config = configText.FromJson<TestConfig>();
            var config = JsonConvert.DeserializeObject<TestConfig>(configText,
            new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                //Converters = new List<JsonConverter> { new JsonInstanceConverter() }
            });
            //TestConfig.FromFile();

            var o = JsonConvert.DeserializeObject(configText);


            Assert.IsNotNull(config);
            //Assert.IsNotNull(config.Key3);
            //Assert.IsTrue(config.Key3.Any());
        }

    }
}
