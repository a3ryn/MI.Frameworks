using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_ObjectTypeToResourceModel
	{
		[DataMapping("SMObjectTypeName", 0)]
		public System.String SMObjectTypeName { get; set; }
		
		[DataMapping("ResourceModelId", 1)]
		public System.Int32 ResourceModelId { get; set; }
		
		[DataMapping("ResourceModelName", 2)]
		public System.String ResourceModelName { get; set; }
		
		[DataMapping("SMSystemTypeName", 3)]
		public System.String SMSystemTypeName { get; set; }
		
		[DataMapping("CategoryID", 4)]
		public System.Int32 CategoryID { get; set; }
		
		[DataMapping("CategoryName", 5)]
		public System.String CategoryName { get; set; }
		
		[DataMapping("PropertyTable", 6)]
		public System.String PropertyTable { get; set; }
		
		[DataMapping("SMObjectIDs", 7)]
		public System.String SMObjectIDs { get; set; }
	}
}