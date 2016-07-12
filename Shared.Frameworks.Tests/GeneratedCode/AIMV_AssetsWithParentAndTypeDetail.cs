using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_AssetsWithParentAndTypeDetail
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("ObjectTypeId", 1)]
		public System.Int32 ObjectTypeId { get; set; }
		
		[DataMapping("Name", 2)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeName", 3)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ObjectTypeClassId", 4)]
		public System.Int32 ObjectTypeClassId { get; set; }
		
		[DataMapping("ResourceTypeName", 5)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("ParentId", 6)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentObjectTypeId", 7)]
		public System.Int32? ParentObjectTypeId { get; set; }
		
		[DataMapping("ParentName", 8)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 9)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 10)]
		public System.String ParentResourceTypeName { get; set; }
	}
}