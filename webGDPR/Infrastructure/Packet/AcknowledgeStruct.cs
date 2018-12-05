using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AcknowledgeStruct
	{
		public byte Command;
		public byte Return;             //Must (0 = no error, other = an error (Command dependant)
		public byte CollarNumber;       //CollarNumber, "0" is base + 128 (wait for next query i.e. don't go to sleep)
		public byte ExtendedData;       //Ack additionnal data (optional)  
        public uint Index;              //#GpsPoint or FilePayloadPacketNo (FileDataStruct) or FileOperationType (SendFileStruct)
    }
}
