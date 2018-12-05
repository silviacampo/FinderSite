using System;
using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{
    /*used by the base to report the locally stored files with their versions
     * can be queried by server using QueryStruct:
     * with param1 as FileType*/
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileVersionStruct
    {
        public uint FileLengh;
        public FileDescriptorType FileType;     //Gps ephemeris, Gps update, Collar/base Lora update, Ble update, collar/base config    
        public byte reserved;
        public byte VersionMayor;               //version Mayor
        public byte VersionMinor;               //version Minor
        public byte VersionBuild;               //version Build
        public byte VersionRevision;            //version Revision
        public UInt16 CRC16;                    //file CRC16
    }
}
