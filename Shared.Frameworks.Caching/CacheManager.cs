/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.Caching;
using Shared.Core.Common.Caching;
using Shared.Core.Common.Logging;
using System.Collections.Generic;
using static Shared.Core.Common.auxfunc;

namespace Shared.Frameworks.Caching
{
    /// <summary>
	/// Using System.Runtime.Caching to cache agencies with brokers and carriers;
	/// Will use provided cahing with expiration functionality provided with .NET
	/// </summary>
	[Export(typeof(ICache))]
    [PartCreationPolicy(CreationPolicy.Shared)] //singleton instance (only one Cache allowed)
    public class CacheManager : ICache
    {
        private static readonly ILogger Log = LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static TimeSpan _defaultExpiration;
        static ObjectCache _cache;

        public CacheManager()
        {
            _cache = MemoryCache.Default;
            _defaultExpiration = new TimeSpan(24, 0, 0);
        }

        public T Get<T>(string key)
        {
            if (_cache == null)
                throw new ApplicationException("No cache");

            var obj = _cache.GetCacheItem(key);
            if (!(obj?.Value is T)) return default(T);
            //Log.Debug("Found item in cache. Retrieving from cache item with key " + key);
            return (T)obj.Value;
        }

        public void Put<T>(T item, string key, TimeSpan? expiration = null) =>
            SaveToCache(item, key, expiration);

        private static void SaveToCache(object item, string key, TimeSpan? expiration = null)
        {
            //Log.Debug("Adding to cache item with key " + key);
            if (_cache == null)
                throw new ApplicationException("No cache");

            expiration = expiration ?? _defaultExpiration; //if not passed as input, use default expiration (of 1 day)

            var policy = CreateCacheItemPolicy(expiration.Value);
            var cacheItem = new CacheItem(key, item, null);

            _cache.Set(cacheItem, policy); //cache item with its policy

            //Log.Debug($"Cached item with key {key} with expiration {expiration.Value}sec");
        }

        public T Remove<T>(string key)
        {
            var obj = Remove(key);
            if (obj is T)
                return (T)obj;
            return default(T); //todo is this right? or do we throw an ex?
        }

        public object Remove(string key)
        {
            Log.Debug("Removing from cache item with key " + key);
            if (_cache == null)
                throw new ApplicationException("No cache");

            return _cache.Remove(key);
        }

        private static CacheItemPolicy CreateCacheItemPolicy(TimeSpan expiration)
        {
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = expiration,
                RemovedCallback = RemoveFromCache ,
                //UpdateCallback = UpdateCache
            };
            return policy;
        }

        private static void UpdateCache(CacheEntryUpdateArguments arguments) =>
            Log.Debug($"{logMsg(CacheAction.Update)} Key={arguments.Key}, Val={arguments.UpdatedCacheItem}");

        private static void RemoveFromCache(CacheEntryRemovedArguments arguments) =>
            Log.Debug($"{logMsg(CacheAction.Remove)} Reason={arguments.RemovedReason}, ItemKey={arguments.CacheItem.Key}");

        public T Execute<T>(CacheAction a, string key, T item)
        => (T)del[a](this, key, item);

        public enum CacheAction
        {
            Get,
            Add,
            Update,
            Remove
        }

        private static readonly Dictionary<CacheAction, Func<ICache, string, object, object>> del = new Dictionary<CacheAction, Func<ICache, string, object, object>>
        {
            [CacheAction.Get] = (c, k, item) => c.Get<object>(k),
            [CacheAction.Remove] = (c, k, item) => c.Remove(k),
            [CacheAction.Add] = (c, k, item) => { c.Put(item, k); return item; },
            [CacheAction.Update] = (c, k, item) => { c.Put(item, k); return item; }
        };

        private static Func<CacheAction, string> logMsg = a => $"Cache {a} event received. CacheEntry{a}Arguments: ";
        private static Func<CacheAction, IDictionary<string, string>, string> logMsgWithArgs = (a, args) => $"Cache {a} event received. CacheEntry{a}Arguments: {csv(args)}";
    }
}
