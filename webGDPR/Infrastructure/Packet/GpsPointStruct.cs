using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GpsPointStruct
	{
		public int Latitude;    //signed, in deg * 10^7 (- is S, + is N) 
		public int Longitude;   //signed, in deg * 10^7 (- is W, + is E)
		public uint Epoch;      //not signed, seconds since 1970 (ends 2100)
		public byte Flags;      //Valid (b7), Interference(b6), encription(b5), activity level (b3~0, 4 bit avg accelerometer rms)
		public byte ErrorSats;      //1,2,3,4,5,6,7,8,10,12,15,18,22,28,36,big mts,  # of sats
		public byte reserved;
		public byte CollarNumber;
	}
}
