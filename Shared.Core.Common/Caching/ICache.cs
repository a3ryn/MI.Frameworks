/*
This source file is under MIT License (MIT)
Copyright (c) 2014 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.Caching
{
    public interface ICache
    {
        T Get<T>(string key);
        void Put<T>(T item, string key, TimeSpan? expiration = null);

        T Remove<T>(string key);
        object Remove(string key);
    }
}
