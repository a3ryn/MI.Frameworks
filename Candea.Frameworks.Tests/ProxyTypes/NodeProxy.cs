using Shared.Core.Common.DataAccess;
using System;

namespace Candea.Frameworks.Tests
{
    /// <summary>
    /// A Class the matches the table structure to use with automatic mapping table-to-type
    /// One property does not match the column name (Value) so we can use the DataMappingAttribute to
    /// specify the actual column name that the property maps to.
    /// </summary>
    public class NodeProxy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataMapping("Value")]
        public string NodeVal { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
