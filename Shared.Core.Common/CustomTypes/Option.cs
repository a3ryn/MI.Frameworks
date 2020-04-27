/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.CustomTypes
{

    /// <summary>
    /// A generic type wrapping around any kind of payload type
    /// to indicate if the payload exists or not, instead of 
    /// resorting to nullable type. 
    /// (Null can be a valid value in some cases; this helps
    /// distinguish between NULL as an actual value and no value at all.
    /// </summary>
    /// <remarks>Reference source: https://github.com/0xCM/z0/blob/master/src/monadic/src/optional/Option.Type.cs
    /// </remarks>
    /// <typeparam name="T">The payload type</typeparam>
    public readonly struct Option<T>
    {
        /// <summary>
        /// Returns an emtpy (no payload) option of a given generic type
        /// </summary>
        public static Option<T> None => default;

        /// <summary>
        /// Constructs an option from a given typed payload
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Option<T> Some(T val) => new Option<T>(val, true);

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="val"></param>
        Option(T val, bool exists)
        {
            Exists = exists;
            Value = val;
        }

        /// <summary>
        /// Returns the payload value
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Returns true if a payload value is set/defined
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// Returns true is a payload is defined
        /// </summary>
        public bool IsSome => Exists;

        /// <summary>
        /// Returns true if NO payload has been set/defined
        /// </summary>
        public bool IsNone => !Exists;

        /// <summary>
        /// If a payload is defined, execute some action on it
        /// and return back this option.
        /// </summary>
        /// <param name="ifSome"></param>
        /// <returns></returns>
        public Option<T> OnSome(Action<T> ifSome)
        {
            if (Exists)
                ifSome(Value);
            return this;
        }

        /// <summary>
        /// If no payload is defined, execute some action
        /// and return back this option.
        /// </summary>
        /// <param name="ifNone"></param>
        /// <returns></returns>
        public Option<T> OnNone(Action ifNone)
        {
            if (!Exists)
                ifNone();
            return this;
        }
    }
}
