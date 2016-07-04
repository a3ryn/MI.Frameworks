using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shared.Core.Common.CustomTypes;

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

        public static Pair<T> pair<T>(T first, T second) => new Pair<T>(first, second);

        public static T reverseCond<T>(bool rev, T obj1, T obj2) => rev ? obj2 : obj1;

        public static long elapsedMs(Action a)
        {
            var s = new Stopwatch();
            s.Start();
            a();
            s.Stop();
            return s.ElapsedMilliseconds;
        }

        public static bool isNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

    }
}
