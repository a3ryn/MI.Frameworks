using System.Collections.Generic;

namespace Shared.Core.Common.Config
{
    public interface IAppSettings : IEnumerable<IAppSetting>
    {
        string this[string name] { get; }

        string Setting(string name);
        T Setting<T>(string name);

        IEnumerable<IAppSetting> All { get; }
    }
}
