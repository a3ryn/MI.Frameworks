using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace Shared.Core.Common.Extensions
{
    public static class ExecuteMethodTemplates
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                Logger.Debug(m);
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
