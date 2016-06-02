using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ModulesWithParentAndChildrenDetails
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("Description", 2)]
		public System.String Description { get; set; }
		
		[DataMapping("ObjectTypeName", 3)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ResourceTypeName", 4)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("ModuleType", 5)]
		public System.Int32? ModuleType { get; set; }
		
		[DataMapping("IsCard", 6)]
		public System.Boolean? IsCard { get; set; }
		
		[DataMapping("ParentId", 7)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentName", 8)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 9)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 10)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("ChildId", 11)]
		public System.Int32? ChildId { get; set; }
		
		[DataMapping("ChildName", 12)]
		public System.String ChildName { get; set; }
		
		[DataMapping("ChildObjectTypeName", 13)]
		public System.String ChildObjectTypeName { get; set; }
		
		[DataMapping("ChildResourceTypeName", 14)]
		public System.String ChildResourceTypeName { get; set; }
		
		[DataMapping("PortType", 15)]
		public System.Int32? PortType { get; set; }
		
		[DataMapping("ConnectorType", 16)]
		public System.Int32? ConnectorType { get; set; }
		
		[DataMapping("IsMpoPort", 17)]
		public System.Boolean? IsMpoPort { get; set; }
		
		[DataMapping("Position", 18)]
		public System.Int32? Position { get; set; }
		
		[DataMapping("TotalPorts", 19)]
		public System.Int32? TotalPorts { get; set; }
		
		[DataMapping("Orientation", 20)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("PortStatus", 21)]
		public System.Int32? PortStatus { get; set; }
		
		[DataMapping("PortStatusName", 22)]
		public System.String PortStatusName { get; set; }
		
		[DataMapping("IsPending", 23)]
		public System.Boolean? IsPending { get; set; }
		
		[DataMapping("ChildPosition", 24)]
		public System.Int32? ChildPosition { get; set; }
	}
}