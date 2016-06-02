using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ClosuresWithParentAndChildren
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeName", 2)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("Description", 3)]
		public System.String Description { get; set; }
		
		[DataMapping("ResourceTypeName", 4)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("Position", 5)]
		public System.Int32? Position { get; set; }
		
		[DataMapping("UCapacity", 6)]
		public System.Int32? UCapacity { get; set; }
		
		[DataMapping("ParentID", 7)]
		public System.Int32? ParentID { get; set; }
		
		[DataMapping("ParentName", 8)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentDescription", 9)]
		public System.Int32? ParentDescription { get; set; }
		
		[DataMapping("ParentResourceTypeName", 10)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 11)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ChildId", 12)]
		public System.Int32? ChildId { get; set; }
		
		[DataMapping("ChildName", 13)]
		public System.String ChildName { get; set; }
		
		[DataMapping("ChildDescription", 14)]
		public System.String ChildDescription { get; set; }
		
		[DataMapping("ChildObjectTypeName", 15)]
		public System.String ChildObjectTypeName { get; set; }
		
		[DataMapping("ChildResourceTypeName", 16)]
		public System.String ChildResourceTypeName { get; set; }
		
		[DataMapping("ChildModuleType", 17)]
		public System.Int32? ChildModuleType { get; set; }
		
		[DataMapping("ChildOrientation", 18)]
		public System.Int32? ChildOrientation { get; set; }
		
		[DataMapping("ChildPosition", 19)]
		public System.Int32? ChildPosition { get; set; }
		
		[DataMapping("ChildTotalPorts", 20)]
		public System.Int32? ChildTotalPorts { get; set; }
		
		[DataMapping("PortType", 21)]
		public System.Int32? PortType { get; set; }
		
		[DataMapping("PortStatus", 22)]
		public System.Int32? PortStatus { get; set; }
		
		[DataMapping("PortStatusName", 23)]
		public System.String PortStatusName { get; set; }
		
		[DataMapping("IsPending", 24)]
		public System.Boolean? IsPending { get; set; }
		
		[DataMapping("RowsInRack", 25)]
		public System.Int32? RowsInRack { get; set; }
		
		[DataMapping("Orientation", 26)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("LocationInRack", 27)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("ClosureCustomType", 28)]
		public System.Int32? ClosureCustomType { get; set; }
		
		[DataMapping("MaximumPorts", 29)]
		public System.Int32? MaximumPorts { get; set; }
	}
}