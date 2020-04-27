/*
This source file is under MIT License (MIT)
Copyright (c) 2017 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Threading.Tasks;
using log4net;
using Shared.Core.Common.Logging;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;

namespace Shared.Frameworks.Logging
{

    /// <summary>
    /// The logging implementation class which only redirects the log calls to the log4net logger.
    /// This was added in order to separate all projects (Except this one) from the logging technology used (log4net).
    /// Note: all logging takes place on separate threads, to improve performance (configuration-based option)
    /// </summary>
    public class Logger : ILogger
    {
        protected ILog Logr = null;

        //public Logger(string name)
        //{
        //    Logr = LogManager.GetLogger(name);
        //}

        public Logger(Type t)
        {
            Logr = LogManager.GetLogger(t);
        }

        public void Trace(object message, Exception exception = null) =>
            Execute(() => Logr.Debug(message, exception));
  
        public void Debug(object message, Exception exception = null) =>
            Execute(() => Logr.Debug(message, exception));

        public void Info(object message, Exception exception = null) =>
            Execute(() => Logr.Info(message, exception));

        public void Warn(object message, Exception exception = null) =>
            Execute(() => Logr.Warn(message, exception));

        public void Error(object message, Exception exception = null) =>
            Execute(() => Logr.Error(message, exception));

        public void Fatal(object message, Exception exception = null) =>
            Execute(() => Logr.Fatal(message, exception));

        public void Log(object message, Exception exception = null, LogLevel level = LogLevel.Trace) =>
            Execute(() => _delegates[level](Logr, message, exception));


        private static void Execute(Action a)
        {
            if (LoggingManager.RunOnSeparateThread)
            { 
                Task.Factory.StartNew(a);
            }
            else
            {
                a();
            }
        }

        private static readonly IDictionary<LogLevel, Action<ILog, object, Exception>> _delegates = 
            new ReadOnlyDictionary<LogLevel, Action<ILog, object, Exception>>(
                new Dictionary<LogLevel, Action<ILog, object, Exception>>
                {
                    [LogLevel.Trace] = (l,m,e) => l.Debug(m,e),
                    [LogLevel.Debug] = (l, m, e) => l.Debug(m, e),
                    [LogLevel.Info] = (l, m, e) => l.Info(m, e),
                    [LogLevel.Warn] = (l, m, e) => l.Warn(m, e),
                    [LogLevel.Error] = (l, m, e) => l.Error(m, e),
                    [LogLevel.Fatal] = (l, m, e) => l.Fatal(m, e)
                });
    }
}
