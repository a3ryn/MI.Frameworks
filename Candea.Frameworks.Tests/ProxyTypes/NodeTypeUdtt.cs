using System;

namespace Candea.Frameworks.Tests
{
    /// <summary>
    /// Proxy type for UDTT - input to a st proc call
    /// </summary>
    public class NodeTypeUdtt
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
