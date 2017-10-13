/*
This source file is under MIT License (MIT)
Copyright (c) 2017 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.Logging
{
    using DI;

    /// <summary>
    /// A static helper class that resolves the implementation fo the ILogManager which in turn is responsible for providing instances of the 
    /// concrete logger implementation.
    /// </summary>
    public static class LogResolver
    {
        private static readonly Lazy<ILogManager> _logManager = new Lazy<ILogManager>
            (() => Mef.Resolve<ILogManager>());

        /// <summary>
        /// Returns an instance of the ILogManager (if needed for management of the logger implementation)
        /// </summary>
        public static ILogManager LogManager => _logManager.Value;

        /// <summary>
        /// Returns an instance of the logger implementation, associated with a given type
        /// </summary>
        /// <typeparam name="T">A type that is associated with the logger instances (and that will show up in the log message)</typeparam>
        /// <returns>The instance of the ILogger implementation class</returns>
        public static ILogger GetLogger<T>() where T : class => LogManager.GetLogger<T>();

        /// <summary>
        /// Returns an instance of the logger implementation, associated with a given name
        /// </summary>
        /// <param name="name">A string/identifier that is associated with the logger instances (and that will show up in the log message)</param>
        /// <returns>The instance of the ILogger implementation class</returns>
        public static ILogger GetLogger(string name) => LogManager.GetLogger(name);

        /// <summary>
        /// Returns an instance of the logger implementation, associated with a given type
        /// </summary>
        /// <param name="t">A type that is associated with the logger instances (and that will show up in the log message)</param>
        /// <returns>The instance of the ILogger implementation class</returns>
        public static ILogger GetLogger(Type t) => LogManager.GetLogger(t);

        /// <summary>
        /// Releases the logging resource (if applicable, depending on the concrete implementation)
        /// </summary>
        public static void CloseLogger() => LogManager.Shutdown();

    }
}
