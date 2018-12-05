using System;
using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
    /* Instructions to send a file. To be used with SendFileStruct
     * Read SendFileStruct first
     * Always send FileDataStruct in order 
     * Always wait for ACK from the previous packet
     * If NACK or timeout, resend the same packet up to x times
     * If failed x times the same packet, send a SendFileStruct with operation = Abort, expect an ACK
     * Close the file according to SendFileStruct instructions
     */
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct FileDataStruct
	{
        public UInt16 FilePayloadPacketNo;       //file packet number  < file size / efective file data payload (12 bytes)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
		public byte[] SequencialData;     //14 bytes. If shorter, pad with "0x00". Will be trimmed using fileLengh
	}
}
