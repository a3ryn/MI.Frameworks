using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Core.Common.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shared.Core.Common.Config
{
    public class AppSettings : IAppSettings
    {
        public const string DefaultConfigFile = "appsettings.json";

        protected IEnumerable<KeyValuePair<string, string>> kvpSettings;

        public AppSettings(IEnumerable<IAppSetting> settings = null)
        {    
            kvpSettings =
                settings == null
                ? new KeyValuePair<string, string>[] { }
                : settings.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
        }

        /// <summary>
        /// Imports KVPs of strings from a configuration file: JSON (works with netcore) or, if not found, all appSettings section of a regular xml config file (old style)
        /// </summary>
        /// <param name="filePath">Optional configuration file with path; if not provided, 
        /// either appsettings.json is used or, if that one is not found, a regular XML config file is probed.</param>
        /// <returns>A collection of keyed settings</returns>
        public static IAppSettings FromFile(string filePath = DefaultConfigFile)
        {
            //this one does KVPs only
            var settings = new Dictionary<string, string>();
            if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".json"))
            {
                var kvps = File.ReadAllLines(filePath)
                    .Select(line => line.Trim().Replace("\"", "").Replace(",", ""))
                    .Select(line => line.Split(':'))
                    .Where(pair => pair.Length == 2)
                    .Select(p => new KeyValuePair<string, string>(p[0].Trim(), p[1].Trim()));

                foreach (var kvp in kvps)
                {
                    if (!settings.ContainsKey(kvp.Key))
                        settings.Add(kvp.Key, kvp.Value);
                }
            }
            else
            {
                var appSettings = System.Configuration.ConfigurationManager.AppSettings; //net framework style
                foreach (var appSettingKey in appSettings.AllKeys)
                    settings.Add(appSettingKey, appSettings[appSettingKey]);
            }

            return new AppSettings(settings.Select(kvp => new AppSetting(kvp.Key, kvp.Value) as IAppSetting));
        }


        /// <summary>
        /// Reads from a JSON (configuration) file and populates an instance of the provided POCO type with the matching type properties.
        /// This uses the default Newtonsoft JSON serializer. Settings may be overridden as needed.
        /// If 'section' input string is provided, from the JSON file, only the string value corresponding to that key (section name)
        /// will be deserialized into T (i.e., type T represents a Child/section of the overall configuration file.
        /// </summary>
        /// <typeparam name="T">The type of the POCO configuration object to be populated (deserialized)</typeparam>
        /// <param name="filePath">The path to the JSON file; Optional. If not provided the default appsettings.json file will be used, if present.</param>
        /// <param name="section">Optional: the key name of the section/fragment of the JSON configuration file 
        /// to be parsed and deserialized into  and instance of T</param>
        /// <param name="jsonSettings">Optional: Newtowsoft JSON serializer settings</param>
        /// <returns>An instance of the configuration POCO with properties populated from the deserialized JSON file</returns>
        public static T FromFile<T>(string filePath = DefaultConfigFile, string section = null, JsonSerializerSettings jsonSettings = null)
        {
            filePath = filePath ?? DefaultConfigFile;

            if (File.Exists(filePath))
            {
                var str = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(section)) //no section, the whole file maps to T
                    return str.FromJson<T>(jsonSettings);

                //section is not null: get the string value for that section (key) and deserialize into T
                var o = JsonConvert.DeserializeObject(str, jsonSettings);
                if (o is JObject jo)
                {
                    var sj = jo.GetValue(section, StringComparison.OrdinalIgnoreCase);
                    var sec = sj.ToObject<T>();
                    return sec;
                }
            }
            return default;
        }

        /// <summary>
        /// Given a POCO configuration instance, this parses that object via Reflection and creates a collection of keyed settings.
        /// </summary>
        /// <typeparam name="T">The type of the configuration model to be used as the source of the App Settings</typeparam>
        /// <param name="model">The POCO configuration instance, as the source of the App Settings</param>
        /// <returns>A collection of keyed settings</returns>
        public static IEnumerable<IAppSetting> From<T>(T model)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.CanWrite && p.CanRead);


            foreach (var prop in props)
            {
                var pt = prop.PropertyType;
                var gt = typeof(AppSetting<>).MakeGenericType(pt);
                var ctor = gt.GetConstructor(new[] { typeof(string), pt });
                var settingVal = ctor.Invoke(new[] { prop.Name, prop.GetValue(model) });
                yield return settingVal as IAppSetting;
            }
        }

        /// <summary>
        /// Returns an empty collection of AppSettings
        /// </summary>
        public static IAppSettings Empty => new AppSettings();

        /// <summary>
        /// Returns the string value of a setting identified by its key
        /// </summary>
        /// <param name="name">The key/name of the KVP setting</param>
        /// <returns></returns>
        public string Setting(string name)
            => kvpSettings.FirstOrDefault(x => x.Key == name).Value;

        public T Setting<T>(string name)
        {
            try
            {
                var v = Setting(name);
                return (T)Convert.ChangeType(v, typeof(T));
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Conversion error: {e.Message}");
            }
            return default;
        }

        public IEnumerable<IAppSetting> All =>
            kvpSettings.Select(x => new AppSetting(x.Key, x.Value) as IAppSetting);


        public string this[string name]
            => Setting(name);

        public IEnumerator<IAppSetting> GetEnumerator()
            => All.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => All.GetEnumerator();

        public static T FromSettings<T>(IAppSettings src) where T : new()
        {
            var dst = new T();
            foreach (var s in src)
            {
                var val = s.Value;
                var pi = typeof(T).GetProperty(s.Name);
                if (pi != null)
                    pi.SetValue(dst, Convert.ChangeType(val, pi.PropertyType));
            }
            return dst;
        }


    }
}
