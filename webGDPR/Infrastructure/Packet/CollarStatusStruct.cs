using System;
using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CollarStatusStruct
	{
		public byte LoraRxPower;
		public byte Flags;              //(1) Lora error, (0) Gps error
		public byte FailedLoraPackets;
		public byte FailedGpsLocks;
		public byte LoraPackets;
		public byte GpsLocks;
		public UInt16 BatVoltage;
		public byte CollarNumber;
		public byte GpsSeconds;
		public byte LoraSeconds;
		public byte reserved3;
	}
}
