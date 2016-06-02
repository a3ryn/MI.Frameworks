using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.Caching;
using Shared.Core.Common.Caching;
using Shared.Core.Common.Logging;

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
        //private static readonly Lazy<CacheManager> This = new Lazy<CacheManager>(() => new CacheManager());
        private static readonly ILogger Log = LoggingManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static TimeSpan _defaultExpiration;
        static ObjectCache _cache;

        public CacheManager()
        {
            _cache = MemoryCache.Default;
            _defaultExpiration = new TimeSpan(24, 0, 0);
        }

        //public static CacheManager Instance => This.Value;


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
            Log.Debug(
                $"Cache update event received. CacheEntryUpdateArguments: Key={arguments.Key}, Val={arguments.UpdatedCacheItem}");

        private static void RemoveFromCache(CacheEntryRemovedArguments arguments) =>
            Log.Debug(
                $"Cache remove event received. CacheEntryRemovedArguments: Reason={arguments.RemovedReason}, ItemKey={arguments.CacheItem.Key}");
    }
}
