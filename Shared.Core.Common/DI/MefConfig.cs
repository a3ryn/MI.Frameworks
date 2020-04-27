using Shared.Core.Common.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Common.DI
{
    using Config;

    public class MefConfig : AppSettings<MefConfig>
    {
        public string AssebliesPath { get; set; }

        public string CsvSearchPatterns { get; set; }
    }
}
