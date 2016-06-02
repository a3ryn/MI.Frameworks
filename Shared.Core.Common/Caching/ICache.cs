using System;
using System.Collections.Generic;

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
