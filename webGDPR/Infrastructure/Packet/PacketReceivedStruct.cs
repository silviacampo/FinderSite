using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PacketReceivedStruct
	{
		public byte Command;
		public byte Rssi;
		public byte Flags;          //Conencted (b0), Disconnected (b1), Indication timeout (b2), 
		public byte Connection;
	}
}
