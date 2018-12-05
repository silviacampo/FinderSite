

namespace webGDPR.Infrastructure.Packet
{
        public enum FileDescriptorType : byte
        {
            None = 0,
            GpsEphemeris = 1,
            GpsUpdate = 2,
            BaseLoraUpdate = 3,
            BaseBleUpdate = 4,
            CollarLoraUpdate = 5,
            BaseConfig = 6,
            CollarConfig = 7,
            LastFile,      //This should always be the last file. Do not use. Only for validation / for loops
        }
        public enum FileOperationType : byte
        {
            Idle = 0,
            Open = 1,
            Close = 2,
            Check = 3,
            DoUpdate = 4,
            Abort = 5,
            Erase = 6,
            LastOperation,      //This should always be the last file operation. Do not use. Only for validation / for loops
        }
}