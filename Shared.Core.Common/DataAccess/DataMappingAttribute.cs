using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Core.Common.DataAccess
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    //class to map to table, property to map to column
    public class DataMappingAttribute : Attribute
    {
        public DataMappingAttribute(string name)
        {
            Name = name;
        }

        public DataMappingAttribute(int order)
        {
            Order = order;
        }

        public DataMappingAttribute(string name, int order) : this(name)
        {
            Order = order;
        }

        public string Name { get; }

        public int Order { get; }
    }
}
