using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ModulesWithDetails
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
		
		[DataMapping("ModuleType", 7)]
		public System.Int32? ModuleType { get; set; }
		
		[DataMapping("IsCard", 8)]
		public System.Boolean? IsCard { get; set; }
		
		[DataMapping("PortType", 9)]
		public System.Int32? PortType { get; set; }
		
		[DataMapping("TotalPorts", 10)]
		public System.Int32? TotalPorts { get; set; }
		
		[DataMapping("Orientation", 11)]
		public System.Int32? Orientation { get; set; }
		
		[DataMapping("ParentId", 12)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentName", 13)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 14)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 15)]
		public System.String ParentResourceTypeName { get; set; }
	}
}