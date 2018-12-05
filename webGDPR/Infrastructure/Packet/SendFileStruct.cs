using System;
using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
    /* Instructions to send a file. To be used with FileDataStruct
     * Fill FileLengh, FileType
     * Calculate FilePayloadPackets = ( FileLengh + 13) / 14 (file size / efective file data payload rounded up)
     * Set Operation = Open
     * Set the Version parameters. set =0 if not provided. If local version is >= this, it will answer NACK 
     * Calculate CRC16 of the file (ushort CRC16.ComputeChecksum(byte[] bytes, int length))
     * Send SendFileStruct, expect an ACK
     * Prepare and send Each FileDataStruct (total FilePayloadPackets). Expect an ACK for each packet, Retry in NACK or timeout
     * Send SendFileStruct with operation = Close, expect an ACK
     * Send SendFileStruct with operation = Check, expect an ACK
     * When corresponds, Send SendFileStruct with operation = DoUpdate, expect an ACK
     * IF too many NACKs, timeouts, a failed operation = Close/Check, send a operation = Abort
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SendFileStruct
    {
        public uint FileLengh;
        public UInt16 FilePayloadPackets;       //Expected file packets  = file size / efective file data payload (14 bytes)
        public FileDescriptorType FileType;   //Gps ephemeris, Gps update, Collar/base Lora update, Ble update, collar/base config               
        public FileOperationType Operation;  //Opening, Closing, Checking, DoUpdate file
        public byte VersionMayor;               //version Mayor
        public byte VersionMinor;               //version Minor
        public byte VersionBuild;               //version Build
        public byte VersionRevision;            //version Revision
        public UInt16 CRC16;    //file CRC16 (valid on Closing, Checking, DoUpdate)
    }
}
