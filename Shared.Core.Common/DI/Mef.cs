/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Reflection;
using Shared.Core.Common.Config;
using System.IO;

namespace Shared.Core.Common.DI
{
    using static Extensions.Reflection;
    using static auxfunc;

    public class Mef
    {
        #region Exposing appSettings key usedby this framework, making them discoverable via the API
        /// <summary>
        /// XML AppSetting key to use with MEF framework; set the value to the folder location where 
        /// the framework should search for implementation assemblies for resolution.
        /// </summary>
        public const string MEF_XmlAppSettings_AssembliesPathKey = "MEF.AssembliesPath";

        /// <summary>
        /// XML AppSetting key to use with MEF framework; set the value to a comma-separated string
        /// of patterns representing assembly names to search for resolution (filter criteria).
        /// Example: "MyNamespace.*,Shared.Frameworks.*"
        /// </summary>
        public const string MEF_XmlAppSettings_CsvSearchPatternsKey = "MEF.CsvSearchPatterns";

        /// <summary>
        /// If provided, this is the name of the JSON configuration file that is used to initialize
        /// the MEF-based DI framework provided by this package. Otherwise, the JSON settings for MEF
        /// can also be included in the project's appsettings.json file, as a section keyed as "mef".
        /// </summary>
        public const string MEF_ConfigFileName = "mefSettings.json";
        #endregion

        private static string DefaultPath;
        private static string[] SearchPatterns;

        static Mef() => Init();

        /// <summary>
        /// Instance default CTOR provided only as a mechanism to "reset" the Mef utility, or to
        /// force it to re-initialize (re-read) its configuration settings. 
        /// Usually, for a given project, these settings will all be the same for the lifetime of
        /// the application and do not need to change. This CTOR was primarily used for testing.
        /// </summary>
        public Mef() => Init();

        /// <summary>
        /// If needed, this Mef-based DI framework can be initialized using the instance CTOR
        /// with the settings parameters provided as input. It will override the settings read
        /// in the static CTOR from configuration file(s). As with the default instance CTOR,
        /// this is not the normal usage of this framework.
        /// </summary>
        /// <param name="assembliesPath"></param>
        /// <param name="csvSearchPatterns"></param>
        public Mef(string assembliesPath, string csvSearchPatterns )
        {
            DefaultPath = assembliesPath ?? executingAssemblyDir();
            SearchPatterns = csvSearchPatterns?.Split(',');
        }

        private static void Init()
        {
            var settings = AppSettings.FromFile<MefConfig>(MEF_ConfigFileName, "mef");
            if (settings != null) //this means deserialization of JSON content or section succeeded and the POCO is populated
            {
                DefaultPath = settings.AssebliesPath ?? executingAssemblyDir();
                SearchPatterns = settings.CsvSearchPatterns?.Split(',');
            }
            else //no json file with MEF config was found; trying to retrieve from XML AppSettings section, if any
            {
                var kvpSettings = AppSettings.FromFile(); //reads either the default JSON config file (KVPs) or the XML AppSettings
                if (kvpSettings != null)
                {
                    DefaultPath = kvpSettings[MEF_XmlAppSettings_AssembliesPathKey] ?? executingAssemblyDir();
                    SearchPatterns = kvpSettings[MEF_XmlAppSettings_CsvSearchPatternsKey]?.Split(',');
                }
            }
        }

        /// <summary>
        /// Returns an instance of type T. 
        /// IMPORANT: if type T or the realization of T implements IDisposable, use the Resolve method that returns the container, instead of this method.
        /// </summary>
        /// <typeparam name="T">The interface type. T should not be implementing IDisposable.</typeparam>
        /// <param name="exportName">The export name (optional)</param>
        /// <param name="path">[Optiona] The path to the realization assembly - in case a general path 
        /// is not defined in the configuration's appSettings section or to override that path.</param>
        /// <param name="pattern">[Optional] CSV search pattern for assembly name(s) - in case it is not 
        /// defined in the configuration's appSettings section or to override that path.</param>
        /// <returns>An instance of a non-disposable type.</returns>
        public static T Resolve<T>(string exportName = null, string path = null, string pattern = null) =>
            Resolve<T>(out _, exportName, path, pattern);



        /// <summary>
        /// Returns an instance of type T - which could be disposable. If it is disposable, instead of calling Dispose on T, Dispose on the output variable container should be called instead.
        /// If T is not disposable, the container will be disposed of before this method returns.
        /// </summary>
        /// <typeparam name="T">The interface type. T may be disposable.</typeparam>
        /// <param name="container">The disposable container that controls the lifecycle of the instance of T. When done, this container should be disposed, instead of disposing T.</param>
        /// <param name="exportName">The export name (optional)</param>
        /// <param name="path">[Optiona] The path to the realization assembly - in case a general path 
        /// is not defined in the configuration's appSettings section or to override that path.</param>
        /// <param name="pattern">[Optional] CSV search pattern for assembly name(s) - in case it is not 
        /// defined in the configuration's appSettings section or to override that path.</param>
        /// <returns>An instance of a potentially disposable type.</returns>
        public static T Resolve<T>(out IDisposable container, string exportName = null, string path = null, string pattern = null)
        {
            CompositionContainer c = null;
            var r = Resolve(catalog =>
            {
                var result = GetValueFromCatalog<T>(catalog, out c, exportName);
                if (!result.GetType().ImplementsInterface(typeof(IDisposable)))
                    c.Dispose();
                return result;
            }, exportName, path, pattern);
            container = c;
            return r;
        }

        private static T Resolve<T>(Func<AggregateCatalog, T> f, string exportName = null, string path = null, string pattern = null)
        {
            Debug.WriteLine($". . . . . . . Calling Resolve for type: {typeof(T).Name}");
            var result = default(T);
            var sw = new Stopwatch();
            sw.Start();

            Debug.WriteLine($"MEF (input) : ExportName={exportName}. Path={path}. Pattern={pattern}");
            Debug.WriteLine($"MEF (actual): ExportName={exportName}. Path={path ?? DefaultPath}. Pattern={pattern ?? string.Join(",", SearchPatterns ?? new string[] { })}");

            Debug.WriteLine($"Current Directory: {Environment.CurrentDirectory}");

            try
            {
                using (var catalog = new AggregateCatalog())
                {
                    var patterns = pattern?.Split(',') ?? SearchPatterns;
                    if (patterns != null && patterns.Length > 0)
                    {
                        patterns.ToList().ForEach(x => catalog.Catalogs.Add(new DirectoryCatalog(path ?? DefaultPath, x.EndsWith("dll") ? x : $"{x}dll")));
                    }
                    else
                    {
                        catalog.Catalogs.Add(new DirectoryCatalog(path ?? DefaultPath));
                    }

                    result = f(catalog);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Debug.WriteLine($"Bad DLLs. Reflection Type Load exception: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MEF exception: {e.Message}\n{e.StackTrace}");
                throw new ApplicationException($"MEF exception: {e.Message}\n{e.StackTrace}");
            }

            sw.Stop();
            Debug.WriteLine($"MEF resolve execution time for loading type {typeof(T).Name} = {sw.ElapsedMilliseconds}ms");
            return result;
        }


        private static T GetValueFromCatalog<T>(AggregateCatalog catalog, out CompositionContainer container, string exportName = null)
        {
            //using (var container = new CompositionContainer(catalog))
            container = new CompositionContainer(catalog);
            //{
            var export = exportName == null ? container.GetExport<T>() : container.GetExport<T>(exportName);
            if (export == null)
            {
                var path = string.Join(",", catalog.Catalogs.Where(x => x is DirectoryCatalog).Cast<DirectoryCatalog>().Select(x => x.FullPath));
                throw new ApplicationException($"MEF: Implementation for type {typeof(T).Name} not found. " +
                                               $"Full Path(s)={path}; exportName={exportName}");
            }
            return export.Value;
            //}
            //the container will be disposed if only if T or the realization of T does not implement IDisposable
        }

    }
}
