using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Shared.Core.Common.DI;
using Shared.Frameworks.Logging;
using Shared.Core.Common.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Shared.Core.Common.Extensions;

namespace Candea.Frameworks.Tests
{
    [TestClass]
    public class LoggingTests : TestBase
    {
        const string LogFolder = "log1"; //because of DataAccess tests logging to the same "log" folder, as the cleanup attempts to delete it, this test class will use a different log file location

        [ClassInitialize]
        public static void SetupClass(TestContext context)
        {
            new Mef();

            //delete any log files there might be left over
            if (Directory.Exists(LogFolder))
                Directory.Delete(LogFolder, true);
        }

        [ClassCleanup]
        public static void CleanupClass()
        {
            if (Directory.Exists(LogFolder))
                Directory.Delete(LogFolder, true);
        }


        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();

            LogResolver.CloseLogger(); //shut down logger before deleting the log directory

            //delete any log files created by the test
            if (Directory.Exists(LogFolder))
                Directory.GetFiles(LogFolder).ForEach(f => File.Delete(f));
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
            var logFilePath = $"{LogFolder}/Tmp.log";
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
            var logFilePath = $"{LogFolder}/MyLog.log";
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
            var logFilePath = $"{LogFolder}/Some.LogFile.log";
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
            var logFilePath = $"{LogFolder}/Tmp.log";
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
            var logFilePath = $"{LogFolder}/MyLog.log";
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
            var logFilePath = $"{LogFolder}/Some.LogFile.log";
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
            (string logFilePath, string cfgLogFilePath) = ($"{LogFolder}/HelloLog.log", $"{LogFolder}/NoLog.log");
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

        [TestMethod]
        public void TestLogLevels()
        {
            //Arrange - use custom JSON config file for logging
            var header = "TestLogLevels";
            var logFilePath = $"{LogFolder}/Some.LogFile.log";
            var log4netConfigFile = "myOtherLog4net.config";
            var runOnSeparateThread = false; //to avoid delay in logging
            File.WriteAllText(log4netConfigFile, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath)); //will be restored in test cleanup
            SetupJsonConfigFile(header, log4netConfigFile, runOnSeparateThread);

            if (File.Exists(settingsFile)) //delete the JSON appsettings file as well, to force fallback on XML app.config
                File.Delete(settingsFile);

            var ex = new Exception("Test");
            var msg = "Some-123 Message";
            var pat = $".*{msg.Replace(" ", "\\s")}";

            //Act
            var logMgr = Mef.Resolve<ILogManager>();
            Console.WriteLine($"LoggingManager hash {logMgr.GetHashCode()}");
            ExecuteAssertions(logMgr, header, runOnSeparateThread, log4netConfigFile, logFilePath); //as specified in app.config of this project

            var logger = logMgr.GetLogger<LoggingTests>();
            foreach (var lev in LogLevels)
            {
                logger.Log(msg, ex, lev);
            }

            //Assert
            ValidateLogEntry(logFilePath, pat);

            if (File.Exists(log4netConfigFile))
                File.Delete(log4netConfigFile);
        }


        [TestMethod]
        public void TestLogLevels2()
        {
            //Arrange (use the XML AppSettings to init the logging framework
            var header = "TestLogLevels2";
            var logFilePath = $"{LogFolder}/Some.LogFile2.log";
            var log4netConfigFile = "myLog4net.config";
            var runOnSeparateThread = false; //to avoid delay in logging
            File.WriteAllText(log4netConfigFile, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath)); //will be restored in test cleanup

            if (File.Exists(LoggingManager.Logging_ConfigFileName)) //delete dedicated json config file, if any
                File.Delete(LoggingManager.Logging_ConfigFileName);
            if (File.Exists(settingsFile)) //delete the JSON appsettings file as well, to force fallback on XML app.config
                File.Delete(settingsFile);

            var msg = "Some-456 Text";
            var pattern = $".*{msg.Replace(" ", "\\s")}";


            //Act
            var logMgr = new LoggingManager(header, log4netConfigFile, runOnSeparateThread);
            Console.WriteLine($"LoggingManager hash {logMgr.GetHashCode()}");
            ExecuteAssertions(header, runOnSeparateThread, log4netConfigFile, logFilePath); //as specified in app.config of this project

            var logger = logMgr.GetLogger<MefConfigTests>();
            logger.Info($"log file: {logFilePath}");
            foreach (var lev in LogLevels)
            {
                logger.Log(msg, level:lev);
            }

            //Assert
            ValidateLogEntry(logFilePath, pattern);

            if (File.Exists(log4netConfigFile))
                File.Delete(log4netConfigFile);
        }

        [TestMethod]
        public void TestLogLevels3()
        {
            //Arrange (use the XML AppSettings to init the logging framework
            var header = "TestLogLevels3";
            var logFilePath = $"{LogFolder}/Some.LogFile3.log";
            var log4netConfigFile = "xyzLog4net.config";
            var runOnSeparateThread = false; //to avoid delay in logging
            File.WriteAllText(log4netConfigFile, ExistingLog4NetConfigText.Replace(log4netConfiguredLogFilePath, logFilePath)); //will be restored in test cleanup
            SetupJsonConfigFile(header, log4netConfigFile, runOnSeparateThread);

            if (File.Exists(settingsFile)) //delete the JSON appsettings file as well, to force fallback on XML app.config
                File.Delete(settingsFile);

            string pattern(string txt) => $".*{txt.Replace(" ", "\\s")}";

            //Act - 1
            var logMgr = Mef.Resolve<ILogManager>();
            
            //Assert - 1
            ExecuteAssertions(header, runOnSeparateThread, log4netConfigFile, logFilePath); //as specified in app.config of this project

            //Act - 2
            var logger = logMgr.GetLogger<MefConfigTests>();            
            logger.Info($"log file: {logFilePath}");
            
            //Assert = 2
            ValidateLogEntry(logFilePath, pattern(logFilePath), new[] { LogLevel.Info });

            //Act - 3
            logger = logMgr.GetLogger(typeof(GeneralConfigTests));
            logger.Warn("abc");

            //Assert - 3
            ValidateLogEntry(logFilePath, pattern("abc"), new[] { LogLevel.Warn });

            if (File.Exists(log4netConfigFile))
                File.Delete(log4netConfigFile);
        }

        static readonly IEnumerable<LogLevel> LogLevels = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>();

        private static void ExecuteAssertions(string header, bool runOnSeparateThread, string log4netConfigPath, string logFilePath)
        {
            //Assert
            Assert.IsTrue(File.Exists(logFilePath), $"Log file {logFilePath} not found!");
            ValidateHeader(logFilePath, header);
            ValidateNonPublicStaticFieldValue(typeof(LoggingManager), "Header", header);
            ValidateNonPublicStaticFieldValue(typeof(LoggingManager), "RunOnSeparateThread", runOnSeparateThread);
            ValidateNonPublicStaticFieldValue(typeof(LoggingManager), "Log4netConfigFilePath", log4netConfigPath);
        }

        private static void ExecuteAssertions(ILogManager logger, string header, bool runOnSeparateThread, string log4netConfigPath, string logFilePath)
        {
            //Assert
            Assert.IsNotNull(logger);
            ExecuteAssertions(header, runOnSeparateThread, log4netConfigPath, logFilePath);
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

        private static void ValidateLogEntry(string file, string pat, IEnumerable<LogLevel> levels = null)
        {
            var levelsFound = (levels ?? LogLevels)
                .ToDictionary(x => x, x => false);

            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        Console.WriteLine(line);
                        //if (line.Contains(fragment))
                        //    return; //found, done
                        var levelsNotFound = levelsFound.Where(x => !x.Value).Select(x => x.Key).ToList();
                        foreach (var lev in levelsNotFound)
                        {
                            var levStr = (lev == LogLevel.Trace //Trace is using Debug level in log4net as there is not Trace level in log4net, but is supported by the framework
                                ? LogLevel.Debug
                                : lev).ToString().ToUpper();

                            if (Regex.IsMatch(line, $"{levStr}{pat}"))
                            {
                                levelsFound[lev] = true;
                                break;
                            }
                        }
                        if (levelsFound.All(x => x.Value))
                        {
                            return; //all found
                        }
                    }

                    var notFound = string.Join(";", levelsFound.Where(x => !x.Value)
                        .Select(x => $"{x.Key.ToString().ToUpper()}{pat}"));
                    Assert.Fail($"Expected log content (regex pattern) {notFound} not found.");
                }
            }
        }
    }
}
