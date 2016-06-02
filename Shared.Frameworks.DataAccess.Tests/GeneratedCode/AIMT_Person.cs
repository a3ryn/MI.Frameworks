using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMT_Person
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
		
		[DataMapping("LastName", 5)]
		public System.String LastName { get; set; }
		
		[DataMapping("FirstName", 6)]
		public System.String FirstName { get; set; }
		
		[DataMapping("Role", 7)]
		public System.String Role { get; set; }
		
		[DataMapping("AlternateIdentifier", 8)]
		public System.String AlternateIdentifier { get; set; }
	}
}