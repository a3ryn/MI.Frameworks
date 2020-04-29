using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Shared.Core.Common.DI;
using Shared.Frameworks.Logging;
using Shared.Core.Common.Logging;

namespace Candea.Frameworks.Tests
{
    [TestClass]
    public class LoggingTests : TestBase
    {
        const string LogStr = "log";

        [ClassInitialize]
        public static void SetupClass(TestContext context)
        {
            new Mef();

            //delete any log files created by the test and shut down logger
            //for consistency, we assume log file location is always in the log/ folder from the current executing folder
            if (Directory.Exists(LogStr))
                Directory.Delete(LogStr, true);
        }


        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();

            LogResolver.CloseLogger(); //shut down logger before deleting the log directory

            //delete any log files created by the test and shut down logger
            //for consistency, we assume log file location is always in the log/ folder from the current executing folder
            if (Directory.Exists(LogStr))
                Directory.Delete(LogStr, true);           
        }

        private static readonly string log4netConfiguredLogFilePath = "log/Candea.Frameworks.Tests.log"; //from the project's log4net.config file


        private static string BuildLogJsonConfigString(string header, string log4netpath = Log4netConfig, bool runOnSeparateThread = true)
            => $@"""log"": {{
                ""header"": ""{header}"",
                ""log4netConfigPath"": ""{log4netpath}"",
                ""runOnSeparateThread"": {runOnSeparateThread.ToString().ToLower()}
            }}";

        private static void SetupJsonConfigFile(string header, string log4netpath = Log4netConfig, bool runOnSeparateThread = true)
        {
            var logConfigJsonText = BuildLogJsonConfigString(header, log4netpath, runOnSeparateThread);

            File.WriteAllText(LoggingManager.Logging_ConfigFileName, $"{{ {logConfigJsonText} }}");
        }

        private static void SetupAppSettingsJsonConfigFile(string header, string log4netpath = Log4netConfig, bool runOnSeparateThread = true)
        {
            var logConfigJsonText = BuildLogJsonConfigString(header, log4netpath, runOnSeparateThread);

            var appSettingsJsonNewText = //create a new appsettings.json file that has mef settings as a section inside, to use for deserialization
                $@"{{
                      ""key1"": ""val1"",
                      ""key2"": 200,
                      ""key3"":  true,
                      {logConfigJsonText},
                      ""key4"": {{ ""k41"": ""v41"", ""k42"": ""v42""  }}
                    }}";
            File.WriteAllText(settingsFile, appSettingsJsonNewText);
        }

        [TestMethod]
        public void InitLoggingUsingMefAndJsonConfig()
        {
            //Arrange
            var header = "ABC";
            var log4netConfigPath = "log4netTmp.config"; //we will create this file as a copy of the existing log4net.config file, which we delete and retore at the end
            var logFilePath = "log/Tmp.log";
            var runOnSeparateThread = false;
            File.WriteAllText(log4netConfigPath, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath));
            SetupJsonConfigFile(header, log4netConfigPath, runOnSeparateThread);

            //Act
            var logger = Mef.Resolve<ILogManager>();

            //Assert
            ExecuteAssertions(logger, header, runOnSeparateThread, log4netConfigPath, logFilePath);

            //Cleanup for this test only
            DeleteLog4netConfig(log4netConfigPath);
        }

        [TestMethod]
        public void InitLoggingUsingMefAndAppSettingsJsonConfig()
        {
            //Arrange
            var header = "*****START*****";
            var log4netConfigPath = "myLog4net.config"; //we will create this file as a copy of the existing log4net.config file, which we delete and retore at the end
            var logFilePath = "log/MyLog.log";
            var runOnSeparateThread = false;
            File.WriteAllText(log4netConfigPath, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath));
            SetupAppSettingsJsonConfigFile(header, log4netConfigPath, runOnSeparateThread);
            if (File.Exists(LoggingManager.Logging_ConfigFileName)) //delete dedicated json config file, if any
                File.Delete(LoggingManager.Logging_ConfigFileName);

            //Act
            var logger = Mef.Resolve<ILogManager>();

            //Assert
            ExecuteAssertions(logger, header, runOnSeparateThread, log4netConfigPath, logFilePath);

            //Cleanup for this test only
            DeleteLog4netConfig(log4netConfigPath);
        }

        [TestMethod]
        public void InitLoggingUsingMefAndXmlAppSettingsConfig()
        {
            //Arrange
            var logFilePath = "log/Some.LogFile.log";
            File.WriteAllText(Log4netConfig, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath)); //will be restored in test cleanup
            
            if (File.Exists(LoggingManager.Logging_ConfigFileName)) //delete dedicated json config file, if any
                File.Delete(LoggingManager.Logging_ConfigFileName);
            if (File.Exists(settingsFile)) //delete the JSON appsettings file as well, to force fallback on XML app.config
                File.Delete(settingsFile);

            //Act
            var logger = Mef.Resolve<ILogManager>();

            //Assert
            ExecuteAssertions(logger, "*** START Unit tests (Samples) ***", true, Log4netConfig, logFilePath); //as specified in app.config of this project
        }

        [TestMethod]
        public void InitLoggingUsingDefaultCtorAndJsonConfig()
        {
            //Arrange
            var header = "ABC";
            var log4netConfigPath = "log4netTmp.config"; //we will create this file as a copy of the existing log4net.config file, which we delete and retore at the end
            var logFilePath = "log/Tmp.log";
            var runOnSeparateThread = false;
            File.WriteAllText(log4netConfigPath, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath));
            SetupJsonConfigFile(header, log4netConfigPath, runOnSeparateThread);

            //Act
            var logger = new LoggingManager();

            //Assert
            ExecuteAssertions(logger, header, runOnSeparateThread, log4netConfigPath, logFilePath);

            //Cleanup for this test only
            DeleteLog4netConfig(log4netConfigPath);
        }

        [TestMethod]
        public void InitLoggingUsingDefaultCtorAndAppSettingsJsonConfig()
        {
            //Arrange
            var header = "*****START*****";
            var log4netConfigPath = "myLog4net.config"; //we will create this file as a copy of the existing log4net.config file, which we delete and retore at the end
            var logFilePath = "log/MyLog.log";
            var runOnSeparateThread = false;
            File.WriteAllText(log4netConfigPath, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath));
            SetupAppSettingsJsonConfigFile(header, log4netConfigPath, runOnSeparateThread);
            if (File.Exists(LoggingManager.Logging_ConfigFileName)) //delete dedicated json config file, if any
                File.Delete(LoggingManager.Logging_ConfigFileName);

            //Act
            var logger = new LoggingManager();

            //Assert
            ExecuteAssertions(logger, header, runOnSeparateThread, log4netConfigPath, logFilePath);

            //Cleanup for this test only
            DeleteLog4netConfig(log4netConfigPath);
        }

        [TestMethod]
        public void InitLoggingUsingDefaultCtorAndXmlAppSettingsConfig()
        {
            //Arrange
            var logFilePath = "log/Some.LogFile.log";
            File.WriteAllText(Log4netConfig, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath)); //will be restored in test cleanup

            if (File.Exists(LoggingManager.Logging_ConfigFileName)) //delete dedicated json config file, if any
                File.Delete(LoggingManager.Logging_ConfigFileName);
            if (File.Exists(settingsFile)) //delete the JSON appsettings file as well, to force fallback on XML app.config
                File.Delete(settingsFile);

            //Act
            var logger = new LoggingManager();

            //Assert
            ExecuteAssertions(logger, "*** START Unit tests (Samples) ***", true, Log4netConfig, logFilePath); //as specified in app.config of this project
        }

        [TestMethod]
        public void InitLoggingUsingParamCtorAndIgnoringAllOtherConfig()
        {
            //Arrange
            (string header, string cfgHeader) = ("Hello Logger", "Not Used Header");
            (string log4netConfigPath, string cfgLog4netConfigPath) = ("helloLog4net.config", "uselessLog4Net.config"); 
            (string logFilePath, string cfgLogFilePath) = ("log/HelloLog.log", "log/NoLog.log");
            (bool runOnSeparateThread, bool cfgRunOnSeparateThread) = (false, true);
            File.WriteAllText(log4netConfigPath, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath)); //setting up the used log4net config file
            File.WriteAllText(cfgLog4netConfigPath, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, cfgLogFilePath)); //setting up the configured (not used) log4net config file

            SetupAppSettingsJsonConfigFile(cfgHeader, cfgLog4netConfigPath, cfgRunOnSeparateThread); //setting up the not-used config file
            SetupJsonConfigFile(header, log4netConfigPath, runOnSeparateThread); //setting up the not-used config file

            //Act
            var logger = new LoggingManager(header, log4netConfigPath, runOnSeparateThread); //input params will override all configured params

            //Assert
            ExecuteAssertions(logger, header, runOnSeparateThread, log4netConfigPath, logFilePath);

            //Cleanup for this test only
            DeleteLog4netConfig(log4netConfigPath);
        }

        private static void ExecuteAssertions(ILogManager logger, string header, bool runOnSeparateThread, string log4netConfigPath, string logFilePath)
        {
            //Assert
            Assert.IsNotNull(logger);
            Assert.IsTrue(File.Exists(logFilePath));
            ValidateHeader(logFilePath, header);
            ValidateNonPublicStaticFieldValue(typeof(LoggingManager), "Header", header);
            ValidateNonPublicStaticFieldValue(typeof(LoggingManager), "RunOnSeparateThread", runOnSeparateThread);
            ValidateNonPublicStaticFieldValue(typeof(LoggingManager), "Log4netConfigFilePath", log4netConfigPath);
        }

        private static void DeleteLog4netConfig(string log4netConfigPath)
        {
            //Cleanup for this test only
            if (File.Exists(log4netConfigPath))
                File.Delete(log4netConfigPath);
        }

        private static void ValidateHeader(string file, string header)
        {
            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var line = reader.ReadLine();
                    Assert.IsTrue(line.Contains(header));
                }
            }
        }
    }
}
