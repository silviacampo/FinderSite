using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GpsCountStruct
	{
		public uint ToReadGpsPoints;
		public uint Index;
		public uint Epoch;      //not signed, seconds since 1970 (ends 2100)
	}
}
