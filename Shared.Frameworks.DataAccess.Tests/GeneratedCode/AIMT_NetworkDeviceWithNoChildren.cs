using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMT_NetworkDeviceWithNoChildren
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
		
		[DataMapping("Position", 5)]
		public System.Int32 Position { get; set; }
		
		[DataMapping("Units", 6)]
		public System.Int32? Units { get; set; }
		
		[DataMapping("LocationInRack", 7)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("IpAddress", 8)]
		public System.String IpAddress { get; set; }
		
		[DataMapping("IpAddress6", 9)]
		public System.String IpAddress6 { get; set; }
		
		[DataMapping("MacAddress", 10)]
		public System.String MacAddress { get; set; }
		
		[DataMapping("PortType", 11)]
		public System.Int32? PortType { get; set; }
		
		[DataMapping("DeviceType", 12)]
		public System.Int32? DeviceType { get; set; }
		
		[DataMapping("TotalPorts", 13)]
		public System.Int32? TotalPorts { get; set; }
	}
}