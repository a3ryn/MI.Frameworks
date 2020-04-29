using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Shared.Frameworks.DataAccess;
using Shared.Frameworks.Caching;
using Shared.Frameworks.Logging;

namespace Candea.Frameworks.Tests
{
    /// <summary>
    /// Base class for some of the unit test classes.
    /// It deals with saving existing content of some of the configuration files included with the project
    /// and restoring that content as part of a test cleanup.
    /// </summary>
    public abstract class TestBase
    {
        internal const string settingsFile = "appsettings.json";
        internal const string Log4netConfig = "log4net.config";

        //appsettings.json file of this project does not have mef settings in it; these unit tests will add/remove such settings
        //but it assumes that the appsettings.json was not modified manually to add these mef settings.
        protected static readonly string ExistingAppSettingsJsonText =
            ReadFileContent(settingsFile);

        protected static readonly string ExistingMefSettingsJsonText =
            ReadFileContent(Mef.MEF_ConfigFileName);

        protected static readonly string ExistingDataAccessSettingsJsonText =
            ReadFileContent(DataAdapter.DA_ConfigFileName);

        protected static readonly string ExistingCacheSettingsJsonText =
            ReadFileContent(CacheManager.CACHE_ConfigFileName);

        protected static readonly string ExistingLogSettingsJsonText =
            ReadFileContent(LoggingManager.Logging_ConfigFileName);

        protected static readonly string ExistingLog4NetConfigText =
            ReadFileContent("log4net.config");

        private static string ReadFileContent(string file)
            => File.Exists(file)
                ? File.ReadAllText(file)
                : null;


        [TestCleanup]
        public virtual void Cleanup() =>
            RestoreConfigFile(
                    new[]
                    {
                        (settingsFile, ExistingAppSettingsJsonText),
                        (Mef.MEF_ConfigFileName, ExistingMefSettingsJsonText),
                        (DataAdapter.DA_ConfigFileName, ExistingDataAccessSettingsJsonText),
                        (CacheManager.CACHE_ConfigFileName, ExistingCacheSettingsJsonText),
                        (LoggingManager.Logging_ConfigFileName, ExistingLogSettingsJsonText),
                        ("log4net.config", ExistingLog4NetConfigText)
                    });

        protected static void RestoreConfigFile(IEnumerable<(string filePath, string text)> filesWithContentToRestore)
        {
            foreach ((string filePath, string text) in filesWithContentToRestore)
            {
                if (text != null) //only when it is truly not null do we restore, even it is an empty string
                    File.WriteAllText(filePath, text);
            }
        }

        /// <summary>
        /// Validates some static non-public field on some type, checking its value (or some processed value of that) 
        /// against some expected value provided in the input of this method call. Using <see cref="System.Reflection"/>.
        /// </summary>
        /// <param name="t">The type containing the static non-public field to be validated</param>
        /// <param name="fieldName">The name of the static non-public field</param>
        /// <param name="expectedFieldValue">The expected value to assert the reflected value against.</param>
        /// <param name="fieldValueAdapter">An optional delegate that will transform the retrieved field value and prepare it for comparison with expected value.</param>
        protected static void ValidateNonPublicStaticFieldValue(Type t, string fieldName, 
            object expectedFieldValue, Func<object,object> fieldValueAdapter = null)
        {
            var f1 = t.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(f1, "Field name or accessibility was changed!");

            var actualValue = fieldValueAdapter == null 
                ? f1.GetValue(null) 
                : fieldValueAdapter(f1.GetValue(null));
            Assert.AreEqual(expectedFieldValue, actualValue);
        }
    }
}
