using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ObjectTypeToParentResourceTypeMapping
	{
		[DataMapping("ResourceTypeName", 0)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("ObjectTypeName", 1)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 2)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 3)]
		public System.String ParentObjectTypeName { get; set; }
	}
}