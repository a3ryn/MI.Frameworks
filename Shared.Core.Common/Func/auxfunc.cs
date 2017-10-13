/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Shared.Core.Common.CustomTypes;
using System.Reflection;

namespace Shared.Core.Common
{
    public static class auxfunc
    {
        [DebuggerStepThrough]
        public static Fraction fraction(int numerator, int denomiator = 1) => Fraction.Create(numerator, denomiator);

        public static readonly bool[] BoolValues = { false, true };

        [DebuggerStepThrough]
        public static IEnumerable<Tuple<T1, T2>> crossJoin<T1, T2>(IEnumerable<T1> set1, IEnumerable<T2> set2) =>
                from t1 in set1
                from t2 in set2
                select Tuple.Create(t1, t2);

        [DebuggerStepThrough]
        public static IEnumerable<Tuple<T1, T2, T3>> crossJoin<T1, T2, T3>(IEnumerable<T1> set1, IEnumerable<T2> set2, IEnumerable<T3> set3) =>
                from t1 in set1
                from t2 in set2
                from t3 in set3
                select Tuple.Create(t1, t2, t3);

        [DebuggerStepThrough]
        public static IEnumerable<Tuple<bool, bool>> boolCrossJoin() =>
            crossJoin(BoolValues, BoolValues);

        [DebuggerStepThrough]
        public static IEnumerable<Tuple<bool, bool,bool>> bool3CrossJoin() =>
            crossJoin(BoolValues, BoolValues,BoolValues);

        [DebuggerStepThrough]
        public static Pair<T> pair<T>(T first, T second) => new Pair<T>(first, second);

        [DebuggerStepThrough]
        public static T reverseCond<T>(bool rev, T obj1, T obj2) => rev ? obj2 : obj1;

        [DebuggerStepThrough]
        public static long elapsedMs(Action a)
        {
            var s = new Stopwatch();
            s.Start();
            a();
            s.Stop();
            return s.ElapsedMilliseconds;
        }

        [DebuggerStepThrough]
        public static bool isNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        public static Func<string, string> appSettingStr =
            k => ConfigurationManager.AppSettings[k];

        [DebuggerStepThrough]
        public static T appSetting<T>(string key) =>
            readSetting(key, () => default(T));

        [DebuggerStepThrough]
        public static T appSetting<T>(string key, T defaultValue) =>
            readSetting(key, () => defaultValue);

        public static Func<string, string> connString =
            k => ConfigurationManager.ConnectionStrings[k]?.ConnectionString;


        public static string executingAssemblyDir() =>
            Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        public static string csv<T>(IEnumerable<T> items, string sep = ",") => string.Join(sep, items);
        public static string csv<T>(IDictionary<string, T> items, string sep = ",") => string.Join(sep, items.Select(i => $"[{i.Key}]={i.Value}"));

        private static T readSetting<T>(string key, Func<T> defaultFunc)
        {
            var s = appSettingStr(key);
            if (string.IsNullOrEmpty(s))
                return defaultFunc();

            var o = (T)Convert.ChangeType(s, typeof(T));
            return o;
        }

    }
}
