/*
This source file is under MIT License (MIT)
Copyright (c) Mihaela Iridon, 2020
https://opensource.org/licenses/MIT
*/

namespace Shared.Core.Common.DI
{
    /// <summary>
    /// A data type used to encapsulate DI/MEF configuration settings
    /// </summary>
    public class MefConfig 
    {
        /// <summary>
        /// The path where implementation assemblies are to be searched.
        /// Default is current executing folder.
        /// </summary>
        public string AssembliesPath { get; set; } = ".";

        /// <summary>
        /// Configuration setting Used for filtering implementation assemblies
        /// to search. This is a comma-separate collection of strings that represent
        /// search patterns for assembly names to search for.
        /// Example: "MyNamespace.*,Shared.Frameworks.*"
        /// </summary>
        public string CsvSearchPatterns { get; set; } = "*";
    }
}
