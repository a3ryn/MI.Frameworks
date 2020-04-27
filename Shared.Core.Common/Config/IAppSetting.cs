/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

namespace Shared.Core.Common.Config
{
    /// <summary>
    /// A custom Key-Value pair of strings used
    /// to identify simple individual configuration settings
    /// </summary>
    public interface IAppSetting
    {
        string Name { get; }
        string Value { get; }
    }

    /// <summary>
    /// A generic version of a strongly typed KVP setting
    /// </summary>
    /// <typeparam name="T">The type of the setting payload (value)</typeparam>
    public interface IAppSetting<T> : IAppSetting 
    {
        new T Value { get; }
    }
}
