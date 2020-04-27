/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

namespace Shared.Core.Common.Config
{
    /// <summary>
    /// Immutable struct encapsulating a string-string KVP setting
    /// </summary>
    public readonly struct AppSetting : IAppSetting
    {
        public AppSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The name/key of the setting
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The string value of the setting
        /// </summary>
        public string Value { get; }
    }

    /// <summary>
    /// A generic version of an immutable struct that encapsulates a KVP setting
    /// </summary>
    /// <typeparam name="T">The generic type of the setting value</typeparam>
    public readonly struct AppSetting<T> : IAppSetting<T> 
    {
        public AppSetting(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public T Value { get; }

        string IAppSetting.Value => Value?.ToString();

        public AppSetting NonGeneric => new AppSetting(Name, Value?.ToString());
    }
}
