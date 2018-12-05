using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace webGDPR.Infrastructure.Packet
{

public static class DataConverter
{
    public static UInt32 UIntToHost(UInt32 source)
    {
        return (UInt32)(source >> 24) |
            ((source << 8) & 0x00ff0000) |
            ((source >> 8) & 0x0000ff00) |
            (source << 24);
    }

    private static T ByteOrderSwapStruct<T>(T inStruct) where T : struct
    {
        // Modify the structure in-place to convert between host
        // and network byte order. We only support a few basic types.
        // Inspired by http://stackoverflow.com/questions/2480116/

        Type t = inStruct.GetType();
        FieldInfo[] fieldInfo = t.GetFields();
        foreach (FieldInfo fi in fieldInfo)
        {
            if (fi.FieldType == typeof(System.Int16))
            {
                Int16 i16 = (Int16)fi.GetValue(inStruct);
                byte[] b16 = BitConverter.GetBytes(i16);
                byte[] b16r = b16.Reverse().ToArray();
                fi.SetValueDirect(__makeref(inStruct), BitConverter.ToInt16(b16r, 0));
            }
            else if (fi.FieldType == typeof(System.UInt16))
            {
                UInt16 i16 = (UInt16)fi.GetValue(inStruct);
                byte[] b16 = BitConverter.GetBytes(i16);
                byte[] b16r = b16.Reverse().ToArray();
                fi.SetValueDirect(__makeref(inStruct), BitConverter.ToUInt16(b16r, 0));
            }
            else if (fi.FieldType == typeof(System.Int32))
            {
                Int32 i32 = (Int32)fi.GetValue(inStruct);
                byte[] b32 = BitConverter.GetBytes(i32);
                byte[] b32r = b32.Reverse().ToArray();
                fi.SetValueDirect(__makeref(inStruct), BitConverter.ToInt32(b32r, 0));
            }
            else if (fi.FieldType == typeof(System.UInt32))
            {
                UInt32 i32 = (UInt32)fi.GetValue(inStruct);
                byte[] b32 = BitConverter.GetBytes(i32);
                byte[] b32r = b32.Reverse().ToArray();
                fi.SetValueDirect(__makeref(inStruct), BitConverter.ToUInt32(b32r, 0));
            }
            else
            {
                // Unrecognized type, we presume that it's OK to leave field AS-IS.
            }
        }
        return inStruct;
    }

    public static byte[] Serialize<T>(T inStruct) where T : struct
    {
        int rawSize = Marshal.SizeOf(typeof(T));
        byte[] rawData = new byte[rawSize];

        // Convert from host to network byte order
        //inStruct = ByteOrderSwapStruct<T>(inStruct);

        IntPtr buffer = Marshal.AllocHGlobal(rawSize);
        Marshal.StructureToPtr(inStruct, buffer, false);
        Marshal.Copy(buffer, rawData, 0, rawSize);
        Marshal.FreeHGlobal(buffer);
        return rawData;
    }

    public static T Deserialize<T>(byte[] rawData) where T : struct
    {
        int rawSize = Marshal.SizeOf(typeof(T));
        T outStruct;

        if (rawData == null)
        {
            throw new Exception("Deserialize: null rawData");
        }

        if (rawData.Length < rawSize)
            throw new Exception("Deserialize: bad data size");

        IntPtr buffer = Marshal.AllocHGlobal(rawSize);
        Marshal.Copy(rawData, 0, buffer, rawSize);
        outStruct = (T)Marshal.PtrToStructure(buffer, typeof(T));
        Marshal.FreeHGlobal(buffer);

        // Convert from network to host byte order
        //return ByteOrderSwapStruct<T>(outStruct);
        return outStruct;
    }

    public static FieldInfo[] GetFieldInfo<T>()
    {
        FieldInfo[] fi;
        fi = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
        return fi;
    }

    private static int PadLength(int length)
    {
        if ((length % 4) != 0)
            return (length + 4 - (length % 4));
        else
            return length;
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }

    public static bool Split(byte[] data, int firstLen, out byte[] first, out byte[] second)
    {
        first = new byte[0];
        second = new byte[0];
        if (firstLen > data.Length) return false;
        first = new byte[firstLen];
        second = new byte[data.Length - firstLen];
        Buffer.BlockCopy(data, 0, first, 0, firstLen);
        Buffer.BlockCopy(data, firstLen, second, 0, second.Length);
        return true;
    }
    public static byte ComputeChecksum(byte[] bytes, int length)
    {
        byte checksum = 0xCC;
        if (bytes != null && length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                checksum += bytes[i];
            }
        }
        return checksum;

    }

    public static int SignedByte2Int(byte data)
    {
        if (data >= 128)
        {
            return -256 + data;
        }
        return data;
    }

    public static DateTime UnixTimeStampToDateTime(UInt32 unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}

public static class Crc8
{
    static readonly byte[] table = new byte[256];
    // x8 + x7 + x6 + x4 + x2 + 1
    const byte poly = 0xd5;
    public const int Size = 1;

    public static byte ComputeCrc8(byte[] bytes, int length)
    {
        byte crc = 0;
        if (bytes != null && length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                crc = table[crc ^ bytes[i]];
            }
        }
        return crc;
    }

    static Crc8()
    {
        for (int i = 0; i < 256; ++i)
        {
            int temp = i;
            for (int j = 0; j < 8; ++j)
            {
                if ((temp & 0x80) != 0)
                {
                    temp = (temp << 1) ^ poly;
                }
                else
                {
                    temp <<= 1;
                }
            }
            table[i] = (byte)temp;
        }
    }
}

public class Crc16
{
    const ushort Polynomial = 0xA001;
    readonly ushort[] m_table = new ushort[256];

    public static short Size
    {
        get
        {
            return 2;
        }
    }

    public ushort ComputeChecksum(byte[] bytes, int length)
    {
        ushort crc = 0;

        if (length > bytes.Length)
            return 0;

        for (int i = 0; i < length; ++i)
        {
            var index = (byte)(crc ^ bytes[i]);
            crc = (ushort)((crc >> 8) ^ m_table[index]);
        }
        return crc;
    }

    public byte[] ComputeChecksumBytes(byte[] bytes, int length)
    {
        ushort crc = ComputeChecksum(bytes, length);
        return BitConverter.GetBytes(crc);
    }

    public Crc16()
    {
        for (ushort i = 0; i < m_table.Length; ++i)
        {
            ushort value = 0;
            ushort temp = i;
            for (byte j = 0; j < 8; ++j)
            {
                if (((value ^ temp) & 0x0001) != 0)
                {
                    value = (ushort)((value >> 1) ^ Polynomial);
                }
                else
                {
                    value >>= 1;
                }
                temp >>= 1;
            }
            m_table[i] = value;
        }
    }
}
}




