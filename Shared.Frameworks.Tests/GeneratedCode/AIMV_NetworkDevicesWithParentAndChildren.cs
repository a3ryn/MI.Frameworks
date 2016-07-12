using System;
using Shared.Core.Common.DataAccess;

/* Auto-generated code [Date generated: 2016-06-02 13:56:02]. Do not change manually. */

namespace Shared.Frameworks.DataAccess.Tests.GeneratedCode
{
	public class AIMV_NetworkDevicesWithParentAndChildren
	{
		[DataMapping("ID", 0)]
		public System.Int32 ID { get; set; }
		
		[DataMapping("Name", 1)]
		public System.String Name { get; set; }
		
		[DataMapping("ObjectTypeName", 2)]
		public System.String ObjectTypeName { get; set; }
		
		[DataMapping("ResourceTypeName", 3)]
		public System.String ResourceTypeName { get; set; }
		
		[DataMapping("Description", 4)]
		public System.String Description { get; set; }
		
		[DataMapping("Position", 5)]
		public System.Int32? Position { get; set; }
		
		[DataMapping("IPAddress", 6)]
		public System.String IPAddress { get; set; }
		
		[DataMapping("IPAddress6", 7)]
		public System.String IPAddress6 { get; set; }
		
		[DataMapping("MACAddress", 8)]
		public System.String MACAddress { get; set; }
		
		[DataMapping("UCapacity", 9)]
		public System.Int32? UCapacity { get; set; }
		
		[DataMapping("LocationInRack", 10)]
		public System.Int32? LocationInRack { get; set; }
		
		[DataMapping("ParentId", 11)]
		public System.Int32? ParentId { get; set; }
		
		[DataMapping("ParentName", 12)]
		public System.String ParentName { get; set; }
		
		[DataMapping("ParentObjectTypeName", 13)]
		public System.String ParentObjectTypeName { get; set; }
		
		[DataMapping("ParentResourceTypeName", 14)]
		public System.String ParentResourceTypeName { get; set; }
		
		[DataMapping("ChildId", 15)]
		public System.Int32? ChildId { get; set; }
		
		[DataMapping("ChildName", 16)]
		public System.String ChildName { get; set; }
		
		[DataMapping("ChildDescription", 17)]
		public System.String ChildDescription { get; set; }
		
		[DataMapping("ChildObjectTypeName", 18)]
		public System.String ChildObjectTypeName { get; set; }
		
		[DataMapping("ChildResourceTypeName", 19)]
		public System.String ChildResourceTypeName { get; set; }
		
		[DataMapping("ChildPosition", 20)]
		public System.Int32? ChildPosition { get; set; }
		
		[DataMapping("PortType", 21)]
		public System.Int32? PortType { get; set; }
		
		[DataMapping("PortStatus", 22)]
		public System.Int32? PortStatus { get; set; }
		
		[DataMapping("PortStatusName", 23)]
		public System.String PortStatusName { get; set; }
		
		[DataMapping("IsPending", 24)]
		public System.Boolean? IsPending { get; set; }
		
		[DataMapping("DeviceType", 25)]
		public System.Int32? DeviceType { get; set; }
	}
}