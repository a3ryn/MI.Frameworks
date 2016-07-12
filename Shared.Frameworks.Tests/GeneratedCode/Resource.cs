using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class Resource
	{
		[DataMapping("DBID", 0)]
		public int Id { get; set; }
		
		[DataMapping("Name", 1)]
		public string Name { get; set; }
		
		[DataMapping("Desc", 2)]
		public string Description { get; set; }
		
		[DataMapping("CreatedDate", 3)]
		public DateTime CreatedDate { get; set; }
		
		[DataMapping("IsVisible", 4)]
		public bool IsVisible { get; set; }
	}
}