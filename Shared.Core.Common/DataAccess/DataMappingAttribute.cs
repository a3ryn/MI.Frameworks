/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.DataAccess
{
    /// <summary>
    /// Attribute for mapping a type to a data table and a property to a data table column.
    /// Also used when defining UDTTs or results from calling TVFs.
    /// Ordering of columns is relevant when populating UDTTs that are passed as input to stored procedures.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DataMappingAttribute : Attribute
    {
        public DataMappingAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// CTOR taking the order of column as input - relevant for UDTTs mappings (used as input args to st procs)
        /// </summary>
        /// <param name="order">Column order</param>
        public DataMappingAttribute(int order)
        {
            Order = order;
        }

        public DataMappingAttribute(string name, int order) : this(name)
        {
            Order = order;
        }

        /// <summary>
        /// Name of table, or name of column - depending on what this attribute is used to decorate (type or property)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Order of column - relevant for UDTTs mappings (used as input args to st procs)
        /// </summary>
        public int Order { get; }
    }
}
