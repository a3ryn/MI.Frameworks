using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMT_PatchPanelWithChildren
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
		
		[DataMapping("RowsInRack", 7)]
		public System.Int32? RowsInRack { get; set; }
		
		[DataMapping("LocationInRack", 8)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("PortType", 9)]
		public System.Int32 PortType { get; set; }
		
		[DataMapping("ModuleType", 10)]
		public System.Int32? ModuleType { get; set; }
		
		[DataMapping("TotalPorts", 11)]
		public System.Int32? TotalPorts { get; set; }
		
		[DataMapping("Orientation", 12)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("ConnectorType", 13)]
		public System.Int32? ConnectorType { get; set; }
		
		[DataMapping("MaximumPorts", 14)]
		public System.Int32? MaximumPorts { get; set; }
		
		[DataMapping("ChildModuleId", 15)]
		public System.Int32 ChildModuleId { get; set; }
		
		[DataMapping("ChildModuleObjectTypeClassId", 16)]
		public System.Int32? ChildModuleObjectTypeClassId { get; set; }
		
		[DataMapping("ChildModuleType", 17)]
		public System.Int32? ChildModuleType { get; set; }
		
		[DataMapping("ChildModuleName", 18)]
		public System.String ChildModuleName { get; set; }
		
		[DataMapping("ChildModulePosition", 19)]
		public System.Int32 ChildModulePosition { get; set; }
		
		[DataMapping("ChildOrientation", 20)]
		public System.Int32? ChildOrientation { get; set; }
		
		[DataMapping("ChildPortId", 21)]
		public System.Int32 ChildPortId { get; set; }
		
		[DataMapping("ChildPortObjectTypeClassId", 22)]
		public System.Int32 ChildPortObjectTypeClassId { get; set; }
		
		[DataMapping("ChildPortType", 23)]
		public System.Int32 ChildPortType { get; set; }
		
		[DataMapping("ChildPortName", 24)]
		public System.String ChildPortName { get; set; }
		
		[DataMapping("ChildMaximumPorts", 25)]
		public System.Int32? ChildMaximumPorts { get; set; }
		
		[DataMapping("ChildPortPosition", 26)]
		public System.Int32 ChildPortPosition { get; set; }
	}
}