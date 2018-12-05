using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace webGDPR.Infrastructure.Packet
{
	public class PacketHelper
	{
		//public static byte[] BuildCollar(Collar collar)
		//{
		//	object data = new InitCollarStruct
		//	{
		//		CollarNumber = collar.CollarNumber,
		//		UnitId = collar.HWId,
		//		Hash = 0
		//	};
		//	byte[] bytes = BuildPacket(PacketType.InitCollar, data);
		//	return bytes;
		//}

		//public static byte[] BuildQuery(PacketType type, byte collarNumber, byte param1, byte param2)
		//{
		//	object data = new BLEModels.Packet.QueryStruct
		//	{
		//		CollarNumber = collarNumber,
		//		Command = (byte)type,
		//		Param1 = param1,
		//		Param2 = param2
		//	};

		//	byte[] bytes = BuildPacket(PacketType.Query, data);
		//	return bytes;

		//}

		//public static List<MessageToBase> BuildFile(string path, FileDescriptorType fileDescriptorType)
		//{
		//	string filename = System.IO.Path.GetFileNameWithoutExtension(path);
		//	int index = filename.IndexOf("-") + 1;
		//	int length = filename.Length - index;
		//	string version = filename.Substring(index, length);
		//	version = version.Replace('_', '.');
		//	if (!(version.Contains(".")))
		//	{
		//		version = version + ".0";
		//	}
		//	Version fileVersion = new Version(version);
		//	byte[] filebytes = File.ReadAllBytes(path);
		//	uint fileLengh = (uint)filebytes.Length;
		//	uint numberpackets = (fileLengh + 13) / 14;
		//	Crc16 crc16 = new Crc16();
		//	List<MessageToBase> msgs = new List<MessageToBase>
		//	{
		//		new MessageToBase()
		//		{
		//			Command = (byte)PacketType.SendFile,
		//			Index = 0, // 1,
		//			CollarNumber = 0,
		//			ExtendedData = (byte)FileOperationType.Open,
		//			Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
		//			{
		//				FileLengh = fileLengh,
		//				FileType = fileDescriptorType,
		//				FilePayloadPackets = (ushort)numberpackets,
		//				Operation = FileOperationType.Open,
		//				VersionMayor = (byte)fileVersion.Major,
		//				VersionMinor = (byte)fileVersion.Minor,
		//				VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
		//				VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
		//				CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
		//			})
		//		}
		//	};

		//	for (int i = 0; i < numberpackets; i++)
		//	{
		//		msgs.Add(new MessageToBase()
		//		{
		//			Command = (byte)PacketType.FileData,
		//			Index = (uint)(i + 1), //2),
		//			CollarNumber = 0,
		//			Bytes = BuildPacket(PacketType.FileData, new FileDataStruct
		//			{
		//				FilePayloadPacketNo = (ushort)(i + 1),
		//				SequencialData = filebytes.Skip(14 * i).Take(14).ToArray()
		//			})
		//		});
		//	}
		//	msgs.Add(new MessageToBase()
		//	{
		//		Command = (byte)PacketType.SendFile,
		//		Index = 0, //numberpackets + 2,
		//		CollarNumber = 0,
		//		ExtendedData = (byte)FileOperationType.Close,
		//		Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
		//		{
		//			FileLengh = fileLengh,
		//			FileType = fileDescriptorType,
		//			FilePayloadPackets = (ushort)numberpackets,
		//			Operation = FileOperationType.Close,
		//			VersionMayor = (byte)fileVersion.Major,
		//			VersionMinor = (byte)fileVersion.Minor,
		//			VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
		//			VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
		//			CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
		//		})
		//	});
		//	msgs.Add(new MessageToBase()
		//	{
		//		Command = (byte)PacketType.SendFile,
		//		Index = 0, //numberpackets + 3,
		//		CollarNumber = 0,
		//		ExtendedData = (byte)FileOperationType.Check,
		//		Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
		//		{
		//			FileLengh = fileLengh,
		//			FileType = fileDescriptorType,
		//			FilePayloadPackets = (ushort)numberpackets,
		//			Operation = FileOperationType.Check,
		//			VersionMayor = (byte)fileVersion.Major,
		//			VersionMinor = (byte)fileVersion.Minor,
		//			VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
		//			VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
		//			CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
		//		})
		//	});
		//	msgs.Add(new MessageToBase()
		//	{
		//		Command = (byte)PacketType.SendFile,
		//		Index = 0, //numberpackets + 4,
		//		CollarNumber = 0,
		//		ExtendedData = (byte)FileOperationType.DoUpdate,
		//		Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
		//		{
		//			FileLengh = fileLengh,
		//			FileType = fileDescriptorType,
		//			FilePayloadPackets = (ushort)numberpackets,
		//			Operation = FileOperationType.DoUpdate,
		//			VersionMayor = (byte)fileVersion.Major,
		//			VersionMinor = (byte)fileVersion.Minor,
		//			VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
		//			VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
		//			CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
		//		})
		//	});
		//	return msgs;

		//}

		//private static List<byte[]> BuildListPacket(PacketType type, List<object> data)
		//{
		//	List<byte[]> bytes = new List<byte[]>();
		//	foreach (var o in data)
		//	{
		//		Packet p = new Packet(type)
		//		{
		//			Data = o
		//		};
		//		bytes.Add(p.Serialize());
		//	}
		//	return bytes;
		//}

		//private static byte[] BuildPacket(PacketType type, object data)
		//{
		//	Packet p = new Packet(type)
		//	{
		//		Data = data
		//	};
		//	byte[] bytes = p.Serialize();
		//	return bytes;
		//}
	}
}
