using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UnitNameStruct
        {
            public byte CollarNumber;       //CollarNumber, "0" is base, 127 all, "OR" 128 (wait for next query i.e. don't go to sleep)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
            public string Name;             //15 chars. If shorter, pad with "0x00"
        }
}