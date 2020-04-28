/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Caching;
using Shared.Core.Common.Caching;
using Shared.Core.Common.Logging;
using System.Collections.Generic;
using static Shared.Core.Common.auxfunc;
using Shared.Core.Common.Config;
using System.IO;
using System.Linq;

namespace Shared.Frameworks.Caching
{
    public class CachingConfig
    {
        public CacheExpirationConfig Expiration { get; set; } = new CacheExpirationConfig(); //default

        public TimeSpan ExpirationTimeSpan => CacheExpirationConfig.GetTimeSpan(Expiration);

        public static TimeSpan DefaultExpiration => new TimeSpan(24, 0, 0);
    }

    public class CacheExpirationConfig
    {
        public int Days { get; set; }
        public int Hours { get; set; } = 24;
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public TimeSpan GetTimeSpan()
            => GetTimeSpan(this);

        public static TimeSpan GetTimeSpan(CacheExpirationConfig e)
            => new TimeSpan(e.Days, e.Hours, e.Minutes, e.Seconds);
    }

    /// <summary>
	/// Using System.Runtime.Caching to cache agencies with brokers and carriers;
	/// Will use provided cahing with expiration functionality provided with .NET
    /// Expiration is configurable at the framework level (with a default of 24 hours) or
    /// can be overridden when calling the method to cache some data object (applicable only for that item)
	/// </summary>
	[Export(typeof(ICache))]
    [PartCreationPolicy(CreationPolicy.Shared)] //singleton instance (only one Cache allowed)
    public class CacheManager : ICache
    {
        #region Exposing appSettings key and other data usedby this framework, making them discoverable via the API
        /// <summary>
        /// If using XML App Settings, set the global cache expiration value associated with this key name in the appSettings section.
        /// XML appSettings are probed only when the dedicated JSON config file is not found and neither the appsettings.json file
        /// (as per new .netcore configuration) contains the expected section.
        /// </summary>
        public const string CACHE_XmlAppSettings_ExpirationTimespanKey = "Caching.ExpirationTimespan";

        /// <summary>
        /// The name of the dedicated JSON config file for this framework. A sample is included with the NuGet package.
        /// This file is optional. It's content can be included in the general appsettings.json (core), or configuration
        /// can be specified inside the XML appSettings section, using specific appSetting keys (exposed via this API).
        /// </summary>
        public const string CACHE_ConfigFileName = "cachingSettings.json";
        #endregion

        private static readonly ILogger Log = null;
        static TimeSpan Expiration;

        static CacheManager()
        {
            try
            {
                Log = LogResolver.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"No logger implementation found. Will not log details. {ex.Message}");
            }
        }
        
        static ObjectCache cache;

        /// <summary>
        /// Default CTOR. Cache expiration setting will be probed in JSON or XML configuration files. 
        /// Otherwise, the default value (24 hrs) will be used.
        /// </summary>
        public CacheManager()
        {
            Init();
        }

        /// <summary>
        /// CTOR that also sets the global cache expiratio timespan. No configuration setting is looked up.
        /// </summary>
        /// <param name="expiration"></param>
        public CacheManager(TimeSpan expiration)
        {
            cache = MemoryCache.Default;
            Expiration = expiration;
        }

        private static void Init()
        {
            cache = MemoryCache.Default;
            Expiration = CachingConfig.DefaultExpiration;

            var settings = AppSettings.FromFile<CachingConfig>(CACHE_ConfigFileName, "caching");
            if (settings != null)
            {
                Expiration = settings.ExpirationTimeSpan;
            }
            else //try reading from XMl app settings
            {
                var expirationStr = appSetting(CACHE_XmlAppSettings_ExpirationTimespanKey, "24:0:0");

                Expiration = 
                    TimeSpan.TryParse(expirationStr, out TimeSpan ets) 
                    ? ets 
                    : CachingConfig.DefaultExpiration;
            }
        }

        public T Get<T>(string key)
        {
            if (cache == null)
                throw new ApplicationException("No cache");

            var obj = cache.GetCacheItem(key);
            if (!(obj?.Value is T)) return default(T);
            //Log?.Debug("Found item in cache. Retrieving from cache item with key " + key);
            return (T)obj.Value;
        }

        public void Put<T>(T item, string key, TimeSpan? expiration = null) =>
            SaveToCache(item, key, expiration);

        private static void SaveToCache(object item, string key, TimeSpan? expiration = null)
        {
            //Log.Debug("Adding to cache item with key " + key);
            if (cache == null)
                throw new ApplicationException("No cache");

            var policy = CreateCacheItemPolicy(expiration ?? Expiration);//if not passed as input, use default expiration (of 1 day)
            var cacheItem = new CacheItem(key, item, null);

            cache.Set(cacheItem, policy); //cache item with its policy

            //Log?.Debug($"Cached item with key {key} with expiration {expiration.Value}sec");
        }

        public T Remove<T>(string key)
        {
            var obj = Remove(key);
            if (obj is T)
                return (T)obj;
            return default; //todo is this right? or do we throw an ex? (make configurable?)
        }

        public object Remove(string key)
        {
            Log.Debug("Removing from cache item with key " + key);
            if (cache == null)
                throw new ApplicationException("No cache");

            return cache.Remove(key);
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
            Log?.Debug($"{logMsg(CacheAction.Update)} Key={arguments.Key}, Val={arguments.UpdatedCacheItem}");

        private static void RemoveFromCache(CacheEntryRemovedArguments arguments) =>
            Log?.Debug($"{logMsg(CacheAction.Remove)} Reason={arguments.RemovedReason}, ItemKey={arguments.CacheItem.Key}");

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

        private static readonly Func<CacheAction, string> logMsg = a => $"Cache {a} event received. CacheEntry{a}Arguments: ";
        private static readonly Func<CacheAction, IDictionary<string, string>, string> logMsgWithArgs = (a, args) => $"Cache {a} event received. CacheEntry{a}Arguments: {csv(args)}";
    }
}
