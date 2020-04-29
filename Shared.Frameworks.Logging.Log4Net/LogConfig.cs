/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

namespace Shared.Frameworks.Logging
{
    public class LogConfig
    {

        internal const string DefaultHeader = "_____________ START _____________";
        internal const string DefaultLog4netConfigPath = "log4net.config";

        public string Header { get; set; } = DefaultHeader;
        public string Log4netConfigPath { get; set; } = DefaultLog4netConfigPath;
        public bool RunOnSeparateThread { get; set; } = true;
    }
}
