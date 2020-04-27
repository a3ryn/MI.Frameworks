/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System.Collections.Generic;

namespace Shared.Core.Common.Config
{
    /// <summary>
    /// The spec defining a collection of app settings
    /// </summary>
    public interface IAppSettings : IEnumerable<IAppSetting>
    {
        /// <summary>
        /// Indexer by setting name
        /// </summary>
        /// <param name="name">The key identifying the setting in this collection</param>
        /// <returns>The string value of the named setting item</returns>
        string this[string name] { get; }

        /// <summary>
        /// Retrieves the string setting value of a setting item identified by its name
        /// </summary>
        /// <param name="name">The name (key) of the setting item whose value is to be retrieved</param>
        /// <returns></returns>
        string Setting(string name);

        /// <summary>
        /// The generic version of the setting value accessor
        /// </summary>
        /// <typeparam name="T">The expected type of the setting payload/value</typeparam>
        /// <param name="name">The name/key of the setting whose values is to be retrieved</param>
        /// <returns>The strongly typed setting value</returns>
        T Setting<T>(string name);

        /// <summary>
        /// Returns the entire collection of settings
        /// </summary>
        IEnumerable<IAppSetting> All { get; }
    }
}
