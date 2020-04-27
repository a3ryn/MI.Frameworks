using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Core.Common.CustomTypes;
using Shared.Core.Common.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Shared.Core.Common.Config
{
    using static corefunc;

    public interface IAppSetting
    {
        string Name { get; }
        string Value { get; }
    }

    public interface IAppSetting<T> : IAppSetting 
        //where T : new()
    {
        new T Value { get; }

        //string IAppSetting.Value => Value?.ToString() ?? string.Empty;
    }

    public readonly struct AppSetting : IAppSetting
    {
        public AppSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }

    public readonly struct AppSetting<T> : IAppSetting<T> //where T : new()
    {
        public AppSetting(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public T Value { get; }

        string IAppSetting.Value => Value?.ToString();

        public AppSetting NonGeneric => new AppSetting(Name, Value?.ToString());
    }

    public interface IAppSettings : IEnumerable<IAppSetting>
    {
        string this[string name] { get; }

        string Setting(string name);
        T Setting<T>(string name);

        IEnumerable<IAppSetting> All { get; }
    }

    public class AppSettings : IAppSettings
    {
        public const string DefaultConfigFile = "config.json";

        protected IEnumerable<KeyValuePair<string, string>> kvpSettings;

        public AppSettings(IEnumerable<IAppSetting> settings = null)
        {    
            kvpSettings =
                settings == null
                ? new KeyValuePair<string, string>[] { }
                : settings.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
        }

        public static IAppSettings FromFile(string filePath = DefaultConfigFile)
        {
            //this one does KVPs only
            var settings = new Dictionary<string, string>();
            if (File.Exists(filePath))
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

            return new AppSettings(settings.Select(kvp => new AppSetting(kvp.Key, kvp.Value) as IAppSetting));
        }

        public static T FromFile<T>(string filePath = DefaultConfigFile)
        {
            if (!File.Exists(filePath))
                return default;
            var settings =  File.ReadAllText(filePath).FromJson<T>();
            return settings;
        }

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

        public static IAppSettings Empty => new AppSettings();

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

        public static T FromFile<T>(string filePath = DefaultConfigFile, string section = null)
        {
            if (string.IsNullOrWhiteSpace(section))
                return FromFile<T>(filePath);

            if (File.Exists(filePath))
            {
                var str = File.ReadAllText(filePath);
                var o = JsonConvert.DeserializeObject(str);
                if (o is JObject jo)
                {
                    var sj = jo.GetValue(section, StringComparison.OrdinalIgnoreCase);
                    var sec = sj.ToObject<T>();
                    return sec;
                }
            }
            return default;
        }
    }

    public abstract class AppSettings<T> : AppSettings
        where T : IAppSettings, new()
    {

        public static new T FromFile(string filePath = DefaultConfigFile)
        {
            if (!File.Exists(filePath))
                return default;
            var settings = File.ReadAllText(filePath).FromJson<T>();
            return settings;
        }


    }

    public static class JsonUtil
    {
        static Dictionary<Type, IEnumerable<PropertyInfo>> propsMap =
            new Dictionary<Type, IEnumerable<PropertyInfo>>();



        public static T FromJsonX<T>(string text) where T : new()
        {
            return text.FromJson<T>(new Newtonsoft.Json.JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto
            });
        }

        private static T FromJsonSingleItem<T>(string val)
            => 
            (T)Convert.ChangeType(val, typeof(T));
        private static IEnumerable<T> FromJsonItems<T>(string val)
            => val.Split(',').Select(x => x.Replace("\"", "")).Select(x => FromJsonSingleItem<T>(x));

    }
}
