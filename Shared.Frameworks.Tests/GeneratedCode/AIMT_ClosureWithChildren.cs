using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMT_ClosureWithChildren
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
		
		[DataMapping("ModuleCapacityType", 8)]
		public System.Int32? ModuleCapacityType { get; set; }
		
		[DataMapping("RowsInRack", 9)]
		public System.Int32? RowsInRack { get; set; }
		
		[DataMapping("ModuleId", 10)]
		public System.Int32 ModuleId { get; set; }
		
		[DataMapping("ModuleObjectTypeClassId", 11)]
		public System.Int32? ModuleObjectTypeClassId { get; set; }
		
		[DataMapping("ModuleType", 12)]
		public System.Int32? ModuleType { get; set; }
		
		[DataMapping("ModuleName", 13)]
		public System.String ModuleName { get; set; }
		
		[DataMapping("ModulePosition", 14)]
		public System.Int32 ModulePosition { get; set; }
		
		[DataMapping("Orientation", 15)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("PortId", 16)]
		public System.Int32 PortId { get; set; }
		
		[DataMapping("PortObjectTypeClassId", 17)]
		public System.Int32 PortObjectTypeClassId { get; set; }
		
		[DataMapping("PortType", 18)]
		public System.Int32 PortType { get; set; }
		
		[DataMapping("PortName", 19)]
		public System.String PortName { get; set; }
		
		[DataMapping("MaximumPorts", 20)]
		public System.Int32? MaximumPorts { get; set; }
		
		[DataMapping("ChildPortPosition", 21)]
		public System.Int32 ChildPortPosition { get; set; }
	}
}