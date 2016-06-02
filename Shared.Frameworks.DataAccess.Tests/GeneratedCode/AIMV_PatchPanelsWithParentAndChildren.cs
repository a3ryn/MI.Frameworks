using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_PatchPanelsWithParentAndChildren
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeName", 2)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ResourceTypeName", 3)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("Description", 4)]
		public System.String Description { get; set; }
		
		[DataMapping("Position", 5)]
		public System.Int32? Position { get; set; }
		
		[DataMapping("Orientation", 6)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("PanelPortType", 7)]
		public System.Int32? PanelPortType { get; set; }
		
		[DataMapping("UCapacity", 8)]
		public System.Int32? UCapacity { get; set; }
		
		[DataMapping("LocationInRack", 9)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("TotalPorts", 10)]
		public System.Int32? TotalPorts { get; set; }
		
		[DataMapping("MaximumPorts", 11)]
		public System.Int32? MaximumPorts { get; set; }
		
		[DataMapping("RowsInRack", 12)]
		public System.Int32? RowsInRack { get; set; }
		
		[DataMapping("ParentId", 13)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentName", 14)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 15)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 16)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("ParentDescription", 17)]
		public System.String ParentDescription { get; set; }
		
		[DataMapping("ChildId", 18)]
		public System.Int32? ChildId { get; set; }
		
		[DataMapping("ChildName", 19)]
		public System.String ChildName { get; set; }
		
		[DataMapping("ChildObjectTypeName", 20)]
		public System.String ChildObjectTypeName { get; set; }
		
		[DataMapping("ChildResourceTypeName", 21)]
		public System.String ChildResourceTypeName { get; set; }
		
		[DataMapping("ChildDescription", 22)]
		public System.String ChildDescription { get; set; }
		
		[DataMapping("ChildModuleType", 23)]
		public System.Int32? ChildModuleType { get; set; }
		
		[DataMapping("ChildOrientation", 24)]
		public System.Int32? ChildOrientation { get; set; }
		
		[DataMapping("ChildTotalPorts", 25)]
		public System.Int32? ChildTotalPorts { get; set; }
		
		[DataMapping("PortType", 26)]
		public System.Int32? PortType { get; set; }
		
		[DataMapping("ChildPosition", 27)]
		public System.Int32? ChildPosition { get; set; }
		
		[DataMapping("PortStatus", 28)]
		public System.Int32? PortStatus { get; set; }
		
		[DataMapping("PortStatusName", 29)]
		public System.String PortStatusName { get; set; }
		
		[DataMapping("IsPending", 30)]
		public System.Boolean? IsPending { get; set; }
	}
}