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
    {
        new T Value { get; }
    }

}
