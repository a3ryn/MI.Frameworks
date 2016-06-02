using System;
using System.Configuration;

namespace Shared.Core.Common.Logging
{
    public class LoggingManager
    {
        static LoggingManager()
        {
            var header = ConfigurationManager.AppSettings["Logger.Header"];
            if (string.IsNullOrEmpty(header))
                header = "_____________ START _____________";
            log4net.Config.XmlConfigurator.Configure();
            var log = log4net.LogManager.GetLogger(typeof(LoggingManager));
            log.Debug(header);
        }

        public static ILogger GetLogger(Type t) => new Logger(t);


        public static void CloseLogger() => log4net.LogManager.Shutdown();

    }
}
