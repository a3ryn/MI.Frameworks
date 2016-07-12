using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_PortsWithDetail
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
		
		[DataMapping("PortType", 6)]
		public System.Int32 PortType { get; set; }
		
		[DataMapping("Position", 7)]
		public System.Int32? Position { get; set; }
		
		[DataMapping("VirtualParentID", 8)]
		public System.Int32? VirtualParentID { get; set; }
		
		[DataMapping("ParentId", 9)]
		public System.Int32 ParentId { get; set; }
		
		[DataMapping("ParentName", 10)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 11)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 12)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("PortStatus", 13)]
		public System.Int32? PortStatus { get; set; }
		
		[DataMapping("PortStatusName", 14)]
		public System.String PortStatusName { get; set; }
		
		[DataMapping("IsPending", 15)]
		public System.Boolean? IsPending { get; set; }
	}
}