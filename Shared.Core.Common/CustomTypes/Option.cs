using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Common.CustomTypes
{


    public readonly struct Option<T>
    {
        public static Option<T> None => new Option<T>();

        public static Option<T> Some(T val) => new Option<T>(val);


        public Option(T val)
        {
            Exists = (val != null);
            Value = val;
        }

        public T Value { get; }

        public bool Exists { get; }

        public bool IsSome => Exists;
        public bool IsNone => !Exists;

        public Option<T> OnSome(Action<T> ifSome)
        {
            if (Exists)
                ifSome(Value);
            return this;
        }

        public Option<T> OnNone(Action ifNone)
        {
            if (!Exists)
                ifNone();
            return this;
        }
    }
}
