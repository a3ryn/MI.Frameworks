using System;

namespace Shared.Core.Common.CustomTypes
{
    public class Pair<T>
    {
        public Pair(T first, T second)
        {
            First = first;
            Second = second;
        }

        public static implicit operator Pair<T>(Tuple<T, T> t) => new Pair<T>(corefunc.first(t), corefunc.second(t));

        public T First { get; }
        public T Second { get; }
        public override string ToString()
        {
            return $"{First}-{Second}";
        }
    }
}
