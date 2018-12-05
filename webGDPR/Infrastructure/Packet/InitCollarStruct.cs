using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct InitCollarStruct
	{
		public ulong Hash;
		public uint UnitId;
		public byte CollarNumber;       //"0" is base, "255" means all
		public byte reserved1;
		public byte reserved2;
		public byte reserved3;
	}
}
