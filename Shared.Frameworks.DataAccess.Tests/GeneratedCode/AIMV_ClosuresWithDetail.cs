using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ClosuresWithDetail
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeName", 2)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ObjectTypeId", 3)]
		public System.Int32 ObjectTypeId { get; set; }
		
		[DataMapping("ResourceTypeName", 4)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("Description", 5)]
		public System.String Description { get; set; }
		
		[DataMapping("Position", 6)]
		public System.Int32? Position { get; set; }
		
		[DataMapping("UCapacity", 7)]
		public System.Int32? UCapacity { get; set; }
		
		[DataMapping("ClosurePortType", 8)]
		public System.Int32? ClosurePortType { get; set; }
		
		[DataMapping("ParentId", 9)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentName", 10)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 11)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 12)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("RowsInRack", 13)]
		public System.Int32? RowsInRack { get; set; }
		
		[DataMapping("Orientation", 14)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("LocationInRack", 15)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("ClosureCustomType", 16)]
		public System.Int32? ClosureCustomType { get; set; }
		
		[DataMapping("MaximumPorts", 17)]
		public System.Int32? MaximumPorts { get; set; }
	}
}