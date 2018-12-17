using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SwitchModeStruct
	{
		public byte CollarNumber;       //CollarNumber, "0" is base, 127 all, "OR" 128 (wait for next query i.e. don't go to sleep)
		public byte GpsPeriod;          //in seconds, "0"= tracking mode
		public byte GpsDuration;        //in seconds, Gps active time per period
		public byte NewBandwidth;
		public byte NewSpreadFactor;
		public byte BaseTimeout;     //in 10s of seconds, base will rollback if not all the collars have communicated back
		public byte reserved1;         //Optional
		public byte reserver2;
	}
}
