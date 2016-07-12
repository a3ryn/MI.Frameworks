using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMT_PortDetail
	{
		[DataMapping("PortType", 0)]
		public System.Int32 PortType { get; set; }
		
		[DataMapping("ObjectType", 1)]
		public System.Int32 ObjectType { get; set; }
		
		[DataMapping("ParentId", 2)]
		public System.Int32 ParentId { get; set; }
		
		[DataMapping("Name", 3)]
		public System.String Name { get; set; }
		
		[DataMapping("Description", 4)]
		public System.String Description { get; set; }
		
		[DataMapping("Position", 5)]
		public System.Int32 Position { get; set; }
		
		[DataMapping("SiteId", 6)]
		public System.Int32 SiteId { get; set; }
	}
}