using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct QueryStruct
	{
		public byte Command;
		public byte CollarNumber;   //CollarNumber, "0" is base, "127" means all + 128 (wait for next query i.e. don't go to sleep)
		public byte Param1;         //Optional
		public byte Param2;         //Optional
	}
}
