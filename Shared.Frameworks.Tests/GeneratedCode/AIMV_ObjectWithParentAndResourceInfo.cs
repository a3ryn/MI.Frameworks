using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ObjectWithParentAndResourceInfo
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeName", 2)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ObjectTypeClassId", 3)]
		public System.Int32 ObjectTypeClassId { get; set; }
		
		[DataMapping("ObjectTypeId", 4)]
		public System.Int32 ObjectTypeId { get; set; }
		
		[DataMapping("ResourceTypeName", 5)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("ResourceTypeId", 6)]
		public System.Int32 ResourceTypeId { get; set; }
		
		[DataMapping("ParentId", 7)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentName", 8)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 9)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentObjectTypeId", 10)]
		public System.Int32? ParentObjectTypeId { get; set; }
		
		[DataMapping("ParentResourceTypeName", 11)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeId", 12)]
		public System.Int32? ParentResourceTypeId { get; set; }
	}
}