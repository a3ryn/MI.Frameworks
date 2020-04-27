namespace Shared.Core.Common.Config
{
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

    public readonly struct AppSetting<T> : IAppSetting<T> 
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
}
