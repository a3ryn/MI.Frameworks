/*
This source file is under MIT License (MIT)
Copyright (c) 2015 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

namespace Shared.Core.Common.CustomTypes
{
    public class DirectedPair
    {
        public static DirectedPair<T> Create<T>(T source, T sink)
        {
            return new DirectedPair<T>(source, sink);
        }
    }
    public class DirectedPair<T> : DirectedPair
    {
        public DirectedPair(T source, T sink)
        {
            Source = source;
            Sink = sink;
        }

        public T Source { get; }
        public T Sink { get; }

        public override string ToString()
        {
            return $"[{Source}-{Sink}]";
        }
    }
}
