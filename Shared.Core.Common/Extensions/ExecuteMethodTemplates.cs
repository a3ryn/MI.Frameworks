/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using Shared.Core.Common.Logging;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Shared.Core.Common.Extensions
{
    /// <summary>
    /// Wrapper for action and function execution, to capture and log the processing times.
    /// </summary>
    public static class ExecuteMethodTemplates
    {
        static ExecuteMethodTemplates()
        {
            try
            {
                Logger =
                    LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Logger could not be resolved. Will continue without logging. INitialization exception: {e.Message}");
            }
        }

        private static readonly ILogger Logger = null;

        /// <summary>
        /// Executes some action and logs the execution time
        /// </summary>
        /// <param name="a"></param>
        public static void Execute(this Action a)
        {
            var f = new StackFrame(2);
            var m = f.GetMethod();

            BuildLogEntry(m).Log();
            var s = new Stopwatch();
            s.Start();

            a(); //execute some code

            s.Stop();
            BuildLogEntry(m, s.ElapsedMilliseconds).Log();
        }

        /// <summary>
        /// Execcutes some function, logs the execution time, and returns the result
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result</typeparam>
        /// <param name="func">The function to be executed</param>
        /// <returns>The result instance returned by the function</returns>
        public static TResult ExecuteFunc<TResult>(this Func<TResult> func)
        {
            var result = default(TResult);

            Execute(() => result = func()); //call the func and save the result

            return result;
        }

        private static void Log(this string m, bool toLog = false)
        {
            if (toLog)
            {
                Logger?.Debug(m);
            }
            else
            {
                Debug.WriteLine(m);
            }
        }

        private static string BuildLogEntry(MethodBase m, long? duration = null)
        {
            var startOrEnd = duration.HasValue ? "END" : "START";
            var durationStr = duration.HasValue ? $" Duration = {duration.Value} ms." : "";
            return $"\t\t[{m.DeclaringType?.Name}.{m.Name} {startOrEnd}] {durationStr}";
        }


    }
}
