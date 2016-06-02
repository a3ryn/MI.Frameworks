using System;
using System.Configuration;
using System.Threading.Tasks;
using log4net;

namespace Shared.Core.Common.Logging
{
    /// <summary>
    /// The logging implementation class which only redirects the log calls to the log4net logger.
    /// This was added in order to separate all projects (Except this one) from the logging technology used (log4net).
    /// Note: all logging takes place on separate threads, to improve performance (configuration-based option)
    /// </summary>
    internal class Logger : ILogger
    {
        private ILog Log = null;

        private static readonly bool RunOnSeparateThread = false;
        static Logger()
        {
            //read configuration about running logging on a separate thread.
            var runOnSeparateThreadStr = ConfigurationManager.AppSettings["Logging.RunOnSeparateThread"];
            bool.TryParse(runOnSeparateThreadStr, out RunOnSeparateThread);
        }

        internal Logger(Type t)
        {
            Log = LogManager.GetLogger(t);
        }

        public void Debug(object message)
        {
            Execute(() => Log.Debug(message));
        }

        public void Debug(object message, Exception exception)
        {
            Execute(() => Log.Debug(message, exception));
        }

        public void Info(object message)
        {
            Execute(() => Log.Info(message));
        }

        public void Info(object message, Exception exception)
        {
            Execute(() => Log.Info(message, exception));
        }

        public void Warn(object message)
        {
            Execute(() => Log.Warn(message));
        }

        public void Warn(object message, Exception exception)
        {
            Log.Warn(message, exception);
        }

        public void Error(object message)
        {
            Execute(() => Log.Error(message));
        }

        public void Error(object message, Exception exception)
        {
            Execute(() => Log.Error(message, exception));
        }

        public void Fatal(object message)
        {
            Execute(() => Log.Fatal(message));
        }

        public void Fatal(object message, Exception exception)
        {
            Execute(() => Log.Fatal(message, exception));
        }

        private static void Execute(Action a)
        {
            if (RunOnSeparateThread)
            { 
                Task.Factory.StartNew(a);
            }
            else
            {
                a();
            }
        }
    }
}
