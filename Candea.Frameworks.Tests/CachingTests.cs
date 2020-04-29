using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DI;
using System;
using System.IO;
using Shared.Frameworks.Caching;
using Shared.Core.Common.Caching;
using System.Threading;

namespace Candea.Frameworks.Tests
{
    [TestClass]
    public class CachingTests : TestBase
    {
        [ClassInitialize]
        public static void SetupClass(TestContext context)
        {
            new Mef();
        }

        private static string BuildCachingJsonConfigString(TimeSpan expiration)
            => $@"""caching"": {{
                    ""expiration"": {{
                        ""Days"": {expiration.Days},
                        ""Hours"": {expiration.Hours},
                        ""Minutes"": {expiration.Minutes},
                        ""Seconds"": {expiration.Seconds}
                    }}
                }}";

        private static void SetupJsonConfigFile(TimeSpan? expiration = null)
        {
            var cachingConfigJsonText = BuildCachingJsonConfigString(
                expiration ?? new TimeSpan(0, 0, 3));

            File.WriteAllText(CacheManager.CACHE_ConfigFileName, $"{{ {cachingConfigJsonText} }}");
        }

        private static void SetupAppSettingsJsonConfigFile(TimeSpan? expiration = null)
        {
            var cachingConfigJsonText = BuildCachingJsonConfigString(
                expiration ?? new TimeSpan(0, 0, 3));

            var appSettingsJsonNewText = //create a new appsettings.json file that has mef settings as a section inside, to use for deserialization
                $@"{{
                      ""key1"": ""val1"",
                      ""key2"": 200,
                      ""key3"":  true,
                      {cachingConfigJsonText},
                      ""key4"": {{ ""k41"": ""v41"", ""k42"": ""v42""  }}
                    }}";
            File.WriteAllText(settingsFile, appSettingsJsonNewText);
        }

        [TestMethod]
        public void InitCacheWithMefAndJsonConfigFile()
        {
            //Arrange
            var expiration = new TimeSpan(0, 0, 5);
            //setup dedicated JSON configuration
            SetupJsonConfigFile(expiration);

            //Act: resolve ICache with configuration loading from dedicated JSON config file
            var cacheMgr = Mef.Resolve<ICache>();

            //Assert
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
        }

        [TestMethod]
        public void InitCacheWithDefaultCtorAndJsonConfigFile()
        {
            //Arrange
            var expiration = new TimeSpan(0, 0, 5);
            SetupJsonConfigFile(expiration); //with default of 3 seconds

            //Act
            var cacheMgr = new CacheManager();

            //Assert
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
        }

        [TestMethod]
        public void InitCacheWitMefAndhAppSettingsJsonConfigFile()
        {
            //Arrange
            var expiration = new TimeSpan(0, 12, 0);
            SetupAppSettingsJsonConfigFile(expiration);
            //delete any existing dedicated JSON config file (it would try to read from there first)
            if (File.Exists(CacheManager.CACHE_ConfigFileName))
                File.Delete(CacheManager.CACHE_ConfigFileName);

            //Act
            var cacheMgr = Mef.Resolve<ICache>();

            //Assert
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
        }

        [TestMethod]
        public void InitCacheWithMefAndXmlAppSettingsConfigFile()
        {
            //Arrange: delete all JSON config files
            if (File.Exists(CacheManager.CACHE_ConfigFileName))
                File.Delete(CacheManager.CACHE_ConfigFileName);
            if (File.Exists(settingsFile))
                File.Delete(settingsFile); //will be restored back in the Test Cleanup (base class)

            //in app.config we pre-set the expiration to be 1 day, 3 hours, 0 minutes and 0 seconds (1:3:0:0)
            //Act
            var cacheMgr = Mef.Resolve<ICache>();

            //Assert
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", new TimeSpan(1, 3, 0, 0)); //this is what is set in app.config
        }

        [TestMethod]
        public void InitCacheWithParamCtorAndNoJsonConfigFiles()
        {
            //Arrange: delete all JSON config files
            var expiration = new TimeSpan(1, 1, 1);
            if (File.Exists(CacheManager.CACHE_ConfigFileName))
                File.Delete(CacheManager.CACHE_ConfigFileName);
            if (File.Exists(settingsFile))
                File.Delete(settingsFile); //will be restored back in the Test Cleanup (base class)

            //Act
            var cacheMgr = new CacheManager(expiration);

            //Assert
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
        }

        [TestMethod]
        public void InitCacheWithParamCtorAndConfigIgnored()
        {
            //Arrange
            var expirationInConfigFile = new TimeSpan(5, 5, 5);
            SetupJsonConfigFile(expirationInConfigFile);
            var expiration = new TimeSpan(2, 2, 2);

            //Act
            var cacheMgr = new CacheManager(expiration);

            //Assert
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
        }

        [TestMethod]
        public void InitCacheWithMefAndJsonConfigAndTestExpirationOfCachedValue()
        {
            //Arrange
            var expiration = new TimeSpan(0, 0, 2); //2 seconds for cache to hold a value
            SetupAppSettingsJsonConfigFile(expiration);
            //delete any existing dedicated JSON config file (it would try to read from there first)
            if (File.Exists(CacheManager.CACHE_ConfigFileName))
                File.Delete(CacheManager.CACHE_ConfigFileName);

            var valueToCache = 23;
            var key = "MyIntVal";

            //Act: init cache and validate setting
            var cacheMgr = Mef.Resolve<ICache>();
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
            //Act: store value
            cacheMgr.Put(valueToCache, key);

            //Assert: value is immediately available, but not after 2 seconds
            var cachedValue = cacheMgr.Get<int>(key);
            Assert.AreEqual(valueToCache, cachedValue);

            //wait 2 seconds, value should be gone from cache
            Thread.Sleep(expiration.Seconds * 1000);

            cachedValue = cacheMgr.Get<int>(key);
            Assert.AreEqual(0, cachedValue); //0 is default for int
        }

        [TestMethod]
        public void InitCacheWithMefAndJsonConfigAndTestExpirationOverrideInPutCall()
        {
            //Arrange
            var expiration = new TimeSpan(0, 0, 10); //2 seconds for cache to hold a value
            SetupAppSettingsJsonConfigFile(expiration);
            //delete any existing dedicated JSON config file (it would try to read from there first)
            if (File.Exists(CacheManager.CACHE_ConfigFileName))
                File.Delete(CacheManager.CACHE_ConfigFileName);

            var valueToCache = 23;
            var key = "MyIntVal";
            var expirationOverride = new TimeSpan(0, 0, 2);

            //Act: init cache and validate setting
            var cacheMgr = Mef.Resolve<ICache>();
            Assert.IsNotNull(cacheMgr);
            ValidateNonPublicStaticFieldValue(typeof(CacheManager), "Expiration", expiration);
            //Act: store value
            cacheMgr.Put(valueToCache, key, expirationOverride);

            //Assert: value is immediately available, but not after 2 seconds
            var cachedValue = cacheMgr.Get<int>(key);
            Assert.AreEqual(valueToCache, cachedValue);

            //wait 2 seconds, value should be gone from cache
            Thread.Sleep(expirationOverride.Seconds*1000);

            cachedValue = cacheMgr.Get<int>(key);
            Assert.AreEqual(0, cachedValue); //0 is default for int

        }
    }
}
