/*
This source file is under MIT License (MIT)
Copyright (c) 2015 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.CustomTypes
{
    /// <summary>
    /// Defines an reference type KVP of same generic type for each of the two items in the pair
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pair<T>
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="first">First pair item instance</param>
        /// <param name="second">Second pair item instance</param>
        public Pair(T first, T second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Implements pair instantiation from an old-fashion tuple instance
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Pair<T>(Tuple<T, T> t) => new Pair<T>(corefunc.first(t), corefunc.second(t));

        /// <summary>
        /// Implements pair instantiation from a tuple instance
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Pair<T>((T first, T second) t) => new Pair<T>(t.first, t.second);

        public T First { get; }
        public T Second { get; }
        public override string ToString()
        {
            return $"{First}-{Second}";
        }
    }
}
