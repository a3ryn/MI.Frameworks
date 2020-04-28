/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

namespace Shared.Frameworks.DataAccess
{
    public class DataAccessConfig
    {
        /// <summary>
        /// Connection string used at the global level (when not passed as input to the various queries or SQL statements.
        /// The various methods exposed by the framework will allow overriding this default connection string for that
        /// particular call.
        /// </summary>
        public string DefaultConnStr { get; set; }

        /// <summary>
        /// SQL command execution timeout, in seconds. Default is 120s.
        /// </summary>
        public int SqlCommandTimeout { get; set; } = 120;

        /// <summary>
        /// When true, input data in the form of UDTT proxy will be printed to the log. Default is false.
        /// </summary>
        public bool LogDataBeforeInsert { get; set; }
    }
}
