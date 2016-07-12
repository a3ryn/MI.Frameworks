using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AimApiLog
	{
		[DataMapping("Id", 0)]
		public System.Int32 Id { get; set; }
		
		[DataMapping("Date", 1)]
		public System.DateTime Date { get; set; }
		
		[DataMapping("Thread", 2)]
		public System.String Thread { get; set; }
		
		[DataMapping("Level", 3)]
		public System.String Level { get; set; }
		
		[DataMapping("Logger", 4)]
		public System.String Logger { get; set; }
		
		[DataMapping("Message", 5)]
		public System.String Message { get; set; }
		
		[DataMapping("Exception", 6)]
		public System.String Exception { get; set; }
	}
}