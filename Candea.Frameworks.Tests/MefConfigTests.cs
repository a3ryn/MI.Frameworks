using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DI;
using Shared.Core.Common.Serialization;
using System;
using System.IO;

namespace Candea.Frameworks.Tests
{
    [TestClass]
    public class MefConfigTests : TestBase
    {
        [TestMethod]
        public void LoadMefFromDedicatedMefSettingsJsonFile()
        {
            //Arrange
            var assembliesPath = "/abc1";
            var csvSearchPatterns = "pat11.*,pat12.*";

            var mefSettingsText = $@"{{""mef"": {{ ""assembliesPath"": ""{assembliesPath}"", ""csvSearchPatterns"": ""{csvSearchPatterns}"" }}  }}";

            File.WriteAllText(Mef.MEF_ConfigFileName, mefSettingsText);

            //Act
            new Mef(); //only for testing; not the usual way of using this; this is to force Init() and reading configuration settings from wherever provided

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        [TestMethod]
        public void LoadFromAppSettingsJsonSection()
        {
            var assembliesPath = "/abc2";
            var csvSearchPatterns = "pat21.*,pat22.*";

            var mefSettingsText = $@"""mef"": {{ ""assembliesPath"": ""{assembliesPath}"", ""csvSearchPatterns"": ""{csvSearchPatterns}"" }}";

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
            var assembliesPath = "/abc3";
            var csvSearchPatterns = "pat31.*,pat32.*,pat33.*";

            //Act
            new Mef(assembliesPath, csvSearchPatterns); //init with params; (will reset static data if any was already initialized via the static CTOR)

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        [TestMethod]
        public void InitMefWithInstanceCtorWithParametersAfterStaticCtorInitFromConfigFile()
        {
            //Arrange
            var assembliesPath = "/abc4";
            var csvSearchPatterns = "pat4.*";

            //Act - try resolve some contract, but only to invoke static CTOR to read from config file values other than what is above
            try
            {
                Mef.Resolve<IComparable>(); //will fail
            }
            catch { }
            new Mef(assembliesPath, csvSearchPatterns); //init with params that override what the static ctor set above

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }

        [TestMethod]
        public void InitMefFromXMLAppSettings()
        {
            //Arrange: delete all custom JSON config files
            if (File.Exists(Mef.MEF_ConfigFileName))
                File.Delete(Mef.MEF_ConfigFileName);
            if (File.Exists(settingsFile))
                File.Delete(settingsFile);
            new Mef();//reset configuration (from previous use) - force read from app.config (xml)

            //Act
            try
            {
                Mef.Resolve<IComparable>(); //will fail
            }
            catch { }

            //Assert
            ValidateMefConfiguration(".", "Shared.*");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExportNotFoundExpectingException()
        {
            Mef.Resolve<IComparable>();
        }



        [TestMethod]
        public void LoadFromAppSettingsJsonSection2()
        {
            var assembliesPath = "bin/Debug/netcore3.0";
            var csvSearchPatterns = "Shared.*";

            var mefSettingsText = $@"""mef"": {{ ""assembliesPath"": ""{assembliesPath}"", ""csvSearchPatterns"": ""{csvSearchPatterns}"" }}";

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
            try
            {
                Mef.Resolve<IComparable>();
            }
            catch { }

            //Assert
            ValidateMefConfiguration(assembliesPath, csvSearchPatterns);
        }


        private static void ValidateMefConfiguration(string assembliesPath, string csvSearchPatterns)
        {
            //check the two properties (private static readonly) via reflection:

            //assemblies path (simple string)
            ValidateNonPublicStaticFieldValue(typeof(Mef), "DefaultPath", assembliesPath);

            //csvSearchPatterns
            ValidateNonPublicStaticFieldValue(typeof(Mef), "SearchPatterns", csvSearchPatterns,
                v => string.Join(",", v as string[]));
        }       
    }
}
