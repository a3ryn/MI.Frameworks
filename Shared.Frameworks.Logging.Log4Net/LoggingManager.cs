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

namespace Shared.Frameworks.Logging
{
    /// <summary>
    /// An implementation of the ILogManager to deal with the log4net logging framework; specifically the initialization and shutting down of the logger
    /// </summary>
    [Export(typeof(ILogManager))]
    public class LoggingManager : ILogManager, IDisposable
    {
        internal static readonly bool RunOnSeparateThread = false;

        static LoggingManager()
        {
            var header = appSetting("Logging.Header", "_____________ START _____________");
            var configFileName = appSetting("Logging.ConfigFilePath", "log4net.config");

            RunOnSeparateThread = appSetting("Logging.RunOnSeparateThread", true);
            try
            {
                var configFile = new FileInfo(Path.Combine(Environment.CurrentDirectory,configFileName));
                if (configFile.Exists)
                    Configure(configFile);
                else
                    Configure();
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
       

        public static ILogger Logger(string name) => new Logger(name) as ILogger;
        public ILogger GetLogger(string name) => Logger(name);
        
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
