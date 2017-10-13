/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.Logging
{
    public enum LogLevel
    {
        Trace = 0, //default/unspecified
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public interface ILogger
    {
        void Trace(object message, Exception exception = null);

        void Debug(object message, Exception exception = null);

        void Info(object message, Exception exception = null);

        void Warn(object message, Exception exception = null);

        void Error(object message, Exception exception = null);

        void Fatal(object message, Exception exception = null);

        void Log(object message, Exception exception = null, LogLevel level = LogLevel.Trace);
    }
}
