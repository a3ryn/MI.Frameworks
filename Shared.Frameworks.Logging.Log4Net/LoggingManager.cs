/*
This source file is under MIT License (MIT)
Copyright (c) 2017 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using Shared.Core.Common.Logging;
using System;
using System.IO;
using static System.Diagnostics.Debug;
using static Shared.Core.Common.auxfunc;
using static log4net.Config.XmlConfigurator;
using System.ComponentModel.Composition;
using System.Reflection;

namespace Shared.Frameworks.Logging
{
    /// <summary>
    /// An implementation of the ILogManager to deal with the log4net logging framework; specifically the initialization and shutting down of the logger
    /// </summary>
    [Export(typeof(ILogManager))]
    public class LoggingManager : ILogManager, IDisposable
    {
        internal static readonly bool RunOnSeparateThread = false;

        /// <summary>
        /// AppSetting key name to use for configuring this logging framework and to override defaults.
        /// Set the value for this key to be TRUE if you want the logging activity to be executed on a separate thread.
        /// If not configured, the default value used will be True.
        /// </summary>
        public const string Logging_AppSettings_RunOnSeparateThreadKey = "Logging.RunOnSeparateThread";

        /// <summary>
        /// AppSetting key name to use to configure the path to the log4net config file, including the file name.
        /// If not configured, the default location will "log4net.config", which means the configuration file
        /// is "log4net.config" and it will be read from the current directory (for executing assembly).
        /// </summary>
        public const string Logging_AppSetting_ConfigFilePathKey = "Logging.ConfigFilePath";

        /// <summary>
        /// AppSetting key name to use to configure the first line (header) to be logged at startup of the application.
        /// If a value for this key is not specified, the default header string will be "_____________ START _____________".
        /// </summary>
        public const string Logging_AppSetting_HeaderStringKey = "Logging.Header";

        static LoggingManager()
        {
            var header = appSetting(Logging_AppSetting_HeaderStringKey, "_____________ START _____________");
            var configFileName = appSetting(Logging_AppSetting_ConfigFilePathKey, "log4net.config");

            RunOnSeparateThread = appSetting(Logging_AppSettings_RunOnSeparateThreadKey, true);
            try
            {
                var configFile = new FileInfo(Path.Combine(Environment.CurrentDirectory,configFileName));
                var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
                if (configFile.Exists)
                {
                    Configure(logRepository, configFile);
                }
                else
                    Configure(logRepository);
            }
            catch(Exception e)
            {
                WriteLine($"ERROR: Could not initialize logging: {e.Message}", e);
                return;
            }

            _localLogger = log4net.LogManager.GetLogger(typeof(LoggingManager));
            _localLogger.Info(header);
            _localLogger.Debug($"Logging messages on separate thread={RunOnSeparateThread}");
        }
       

        //public static ILogger Logger(string name) => new Logger(name) as ILogger;
        //public ILogger GetLogger(string name) => Logger(name);
        
        public static ILogger Logger<T>() where T : class => new Logger(typeof(T)) as ILogger;
        public ILogger GetLogger<T>() where T : class => Logger<T>();

        public static ILogger Logger(Type type) => new Logger(type) as ILogger;
        public ILogger GetLogger(Type type) => Logger(type);

        public static void ShutdownLogger() => LogAndShutdownLogger();
        public void Shutdown() => ShutdownLogger();
        public void Dispose() => ShutdownLogger();


        private static readonly log4net.ILog _localLogger;

        private static void LogAndShutdownLogger()
        {
            _localLogger.Info(">>>>> Shutting down log4net Logger. BYE.");
            log4net.LogManager.Shutdown();
        }
    }

}
