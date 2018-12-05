using System;
using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BaseStatusStruct
	{
		public byte LoraRxPower;
		public byte BleRxPower;
		public byte FailedLoraPackets;
		public byte FailedBlePackets;
		public byte LoraPackets;
		public byte BlePackets;
		public byte ChargingStatus;
		public byte reserved;
		public UInt16 BatVoltage;
		public UInt16 UsbVoltage;
	}
}
