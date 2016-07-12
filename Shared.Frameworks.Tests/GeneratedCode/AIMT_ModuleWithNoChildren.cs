using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMT_ModuleWithNoChildren
	{
		[DataMapping("Id", 0)]
		public System.Int32 Id { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeClassId", 2)]
		public System.Int32 ObjectTypeClassId { get; set; }
		
		[DataMapping("Description", 3)]
		public System.String Description { get; set; }
		
		[DataMapping("ParentId", 4)]
		public System.Int32 ParentId { get; set; }
		
		[DataMapping("Position", 5)]
		public System.Int32 Position { get; set; }
		
		[DataMapping("Units", 6)]
		public System.Int32? Units { get; set; }
		
		[DataMapping("LocationInRack", 7)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("PortType", 8)]
		public System.Int32 PortType { get; set; }
		
		[DataMapping("ModuleType", 9)]
		public System.Int32? ModuleType { get; set; }
		
		[DataMapping("TotalPorts", 10)]
		public System.Int32? TotalPorts { get; set; }
		
		[DataMapping("Orientation", 11)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("ConnectorType", 12)]
		public System.Int32? ConnectorType { get; set; }
	}
}