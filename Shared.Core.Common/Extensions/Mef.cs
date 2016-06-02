using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Reflection;

namespace Shared.Core.Common.Extensions
{
    public static class Mef
    {
        private static readonly string DefaultPath;
        static Mef()
        {
            var configuredPathToScan = System.Configuration.ConfigurationManager.AppSettings["MEF.AssembliesPath"];
            DefaultPath = configuredPathToScan ?? ".\\bin"; //use bin only for web apps running in debug mode; but for console apps use Environment.CurrentDirectory
        }

        public static T Resolve<T>(string exportName = null, string path = null)
        {
            var result = default(T);
            var sw = new Stopwatch();
            sw.Start();

            Debug.WriteLine($"MEF: ExportName={exportName}");

            Debug.WriteLine($"Current Directory: {Environment.CurrentDirectory}");

            try
            {
                using (var catalog = new DirectoryCatalog(path?? DefaultPath))
                {
                    result = GetValueFromCatalog<T>(catalog, exportName);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                Debug.WriteLine($"Bad DLLs. Reflection Type Load exception: {e.Message}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"MEF exception: {e.Message}\n{e.StackTrace}");
            }

            sw.Stop();
            Debug.WriteLine($"MEF resolve execution time for loading type {typeof(T).Name} = {sw.ElapsedMilliseconds}ms");
            return result;
        }

        private static T GetValueFromCatalog<T>(ComposablePartCatalog catalog, string exportName = null)
        {
            using (var container = new CompositionContainer(catalog))
            {
                var export = exportName == null ? container.GetExport<T>() : container.GetExport<T>(exportName);
                if (export != null)
                {
                    return export.Value;
                }
            }
            throw new ApplicationException("MEF: Implementation type (i.e., [Export]) not found");
        }
    }
}
