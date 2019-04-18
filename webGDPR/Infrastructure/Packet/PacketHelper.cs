using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webGDPR.Models;

namespace webGDPR.Infrastructure.Packet
{
	public class PacketHelper
	{
		public static uint StripNonNumeric(string str)
		{
			try
			{
				return uint.Parse(new string(str.Where(c => (Char.IsDigit(c))).ToArray()));
			}
			catch
			{
				return 0;
			}
		}

		public static byte[] BuildCollar(Collar collar)
		{
			object data = new InitCollarStruct
			{
				CollarNumber = collar.CollarNumber,
				UnitId = StripNonNumeric(collar.HWId),
				Hash = 0
			};
			byte[] bytes = BuildPacket(PacketType.InitCollar, data);
			return bytes;
		}

		public static CustomWebSockets.Messages.ConfigMode BuildMode(byte collarNumber, ConfigModeTypes type, byte[] customConfig = null) {
			byte[] config;
			switch (type) {
				case ConfigModeTypes.Emergency:
					config = BuildEmergencyMode(collarNumber);
					break;
				case ConfigModeTypes.None:
					config = BuildNormalMode(collarNumber);
					break;
				case ConfigModeTypes.Young:
					config = BuildVeryActiveMode(collarNumber);
					break;
				case ConfigModeTypes.Active:
					config = BuildActiveMode(collarNumber);
					break;
				case ConfigModeTypes.Regular:
					config = BuildVeryLazyMode(collarNumber);
					break;
				case ConfigModeTypes.Lazy:
					config = BuildLazyMode(collarNumber);
					break;				
				case ConfigModeTypes.Mature:
					config = BuildVeryLazyMode(collarNumber);
					break;
				case ConfigModeTypes.Custom:
					config = BuildCustomMode(collarNumber, customConfig);
					break;
				default:
					config = new byte[0];
					break;
			}
			return new CustomWebSockets.Messages.ConfigMode()
			{
				Type = type,
				Config = config,
				CollarNumber = collarNumber
			};
		}

		private static byte[] BuildCustomMode(byte collarNumber, byte[] config)
		{
			return BuildModePacket(collarNumber, config[0], config[1], config[2], config[3], config[4], config[5], config[6]);
		}

		private static byte[] BuildVeryLazyMode(byte collarNumber)
		{
			return BuildModePacket(collarNumber, Constants.SwitchModeEmergencyGpsPeriod, Constants.SwitchModeEmergencyGpsDuration, Constants.SwitchModeEmergencyBandwidth, Constants.SwitchModeEmergencySpreadFactor, Constants.SwitchModeEmergencyBaseTimeout, 0, 0);
		}

		private static byte[] BuildVeryActiveMode(byte collarNumber)
		{
			return BuildModePacket(collarNumber, Constants.SwitchModeEmergencyGpsPeriod, Constants.SwitchModeEmergencyGpsDuration, Constants.SwitchModeEmergencyBandwidth, Constants.SwitchModeEmergencySpreadFactor, Constants.SwitchModeEmergencyBaseTimeout, 0, 0);
		}

		private static byte[] BuildLazyMode(byte collarNumber)
		{
			return BuildModePacket(collarNumber, Constants.SwitchModeEmergencyGpsPeriod, Constants.SwitchModeEmergencyGpsDuration, Constants.SwitchModeEmergencyBandwidth, Constants.SwitchModeEmergencySpreadFactor, Constants.SwitchModeEmergencyBaseTimeout, 0, 0);
		}

		private static byte[] BuildActiveMode(byte collarNumber)
		{
			return BuildModePacket(collarNumber, Constants.SwitchModeEmergencyGpsPeriod, Constants.SwitchModeEmergencyGpsDuration, Constants.SwitchModeEmergencyBandwidth, Constants.SwitchModeEmergencySpreadFactor, Constants.SwitchModeEmergencyBaseTimeout, 0, 0);
		}

		private static byte[] BuildNormalMode(byte collarNumber)
		{
			return BuildModePacket(collarNumber, Constants.SwitchModeNormalGpsPeriod, Constants.SwitchModeNormalGpsDuration, Constants.SwitchModeNormalBandwidth, Constants.SwitchModeNormalSpreadFactor, Constants.SwitchModeNormalBaseTimeout, 0, 0);
		}

		public static byte[] BuildEmergencyMode(byte collarNumber)
		{
			return BuildModePacket(collarNumber, Constants.SwitchModeEmergencyGpsPeriod, Constants.SwitchModeEmergencyGpsDuration, Constants.SwitchModeEmergencyBandwidth, Constants.SwitchModeEmergencySpreadFactor, Constants.SwitchModeEmergencyBaseTimeout, 0,0);
		}

		public static byte[] BuildModePacket(byte CollarNumber, byte GpsPeriod, byte GpsDuration, byte NewBandwidth, byte NewSpreadFactor, byte BaseTimeout, byte reserved1, byte reserver2)
		{
			object data = new SwitchModeStruct
			{
				CollarNumber = CollarNumber,
				GpsPeriod = GpsPeriod,
				GpsDuration = GpsDuration,
				NewBandwidth = NewBandwidth,
				NewSpreadFactor = NewSpreadFactor,
				BaseTimeout = BaseTimeout,
				reserved1 = reserved1,
				reserver2 = reserver2
			};
			byte[] bytes = BuildPacket(PacketType.SwitchMode, data);
			return bytes;
		}

		public static byte[] BuildQuery(PacketType type, byte collarNumber, byte param1, byte param2)
		{
			object data = new QueryStruct
			{
				CollarNumber = collarNumber,
				Command = (byte)type,
				Param1 = param1,
				Param2 = param2
			};

			byte[] bytes = BuildPacket(PacketType.Query, data);
			return bytes;

		}

		public static List<MessageToBase> BuildFile(string path, FileDescriptorType fileDescriptorType)
		{
			string filename = System.IO.Path.GetFileNameWithoutExtension(path);
			int index = filename.IndexOf("-") + 1;
			int length = filename.Length - index;
			string version = filename.Substring(index, length);
			version = version.Replace('_', '.');
			if (!(version.Contains(".")))
			{
				version = version + ".0";
			}
			Version fileVersion = new Version(version);
			byte[] filebytes = File.ReadAllBytes(path);
			uint fileLengh = (uint)filebytes.Length;
			uint numberpackets = (fileLengh + 13) / 14;
			Crc16 crc16 = new Crc16();
			List<MessageToBase> msgs = new List<MessageToBase>
			{
				new MessageToBase()
				{
					Command = (byte)PacketType.SendFile,
					Index = 0,
					CollarNumber = 0,
					ExtendedData = (byte)FileOperationType.Open,
					Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
					{
						FileLengh = fileLengh,
						FileType = fileDescriptorType,
						FilePayloadPackets = (ushort)numberpackets,
						Operation = FileOperationType.Open,
						VersionMayor = (byte)fileVersion.Major,
						VersionMinor = (byte)fileVersion.Minor,
						VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
						VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
						CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
					})
				}
			};

			for (int i = 0; i < numberpackets; i++)
			{
				int len = filebytes.Length;
				int rem = (len - 14 * i) > 14 ? 14 : len - 14 * i;
				byte[] payload = new byte[14];
				byte[] data = filebytes.Skip(14 * i).Take(rem).ToArray();
				Array.Copy(data, payload, rem);
				msgs.Add(new MessageToBase()
				{
					Command = (byte)PacketType.FileData,
					Index = (uint)(i + 1),
					CollarNumber = 0,
					ExtendedData = 0,
					Bytes = BuildPacket(PacketType.FileData, new FileDataStruct
					{
						FilePayloadPacketNo = (ushort)(i + 1),
						SequencialData = payload
					})
				});
			}
			msgs.Add(new MessageToBase()
			{
				Command = (byte)PacketType.SendFile,
				Index = 0, //numberpackets + 2,
				CollarNumber = 0,
				ExtendedData = (byte)FileOperationType.Close,
				Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
				{
					FileLengh = fileLengh,
					FileType = fileDescriptorType,
					FilePayloadPackets = (ushort)numberpackets,
					Operation = FileOperationType.Close,
					VersionMayor = (byte)fileVersion.Major,
					VersionMinor = (byte)fileVersion.Minor,
					VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
					VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
					CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
				})
			});
			msgs.Add(new MessageToBase()
			{
				Command = (byte)PacketType.SendFile,
				Index = 0, //numberpackets + 3,
				CollarNumber = 0,
				ExtendedData = (byte)FileOperationType.Check,
				Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
				{
					FileLengh = fileLengh,
					FileType = fileDescriptorType,
					FilePayloadPackets = (ushort)numberpackets,
					Operation = FileOperationType.Check,
					VersionMayor = (byte)fileVersion.Major,
					VersionMinor = (byte)fileVersion.Minor,
					VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
					VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
					CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
				})
			});
			msgs.Add(new MessageToBase()
			{
				Command = (byte)PacketType.SendFile,
				Index = 0, //numberpackets + 4,
				CollarNumber = 0,
				ExtendedData = (byte)FileOperationType.DoUpdate,
				Bytes = BuildPacket(PacketType.SendFile, new SendFileStruct
				{
					FileLengh = fileLengh,
					FileType = fileDescriptorType,
					FilePayloadPackets = (ushort)numberpackets,
					Operation = FileOperationType.DoUpdate,
					VersionMayor = (byte)fileVersion.Major,
					VersionMinor = (byte)fileVersion.Minor,
					VersionRevision = (byte)(fileVersion.Revision < 0 ? 0 : fileVersion.Revision),
					VersionBuild = (byte)(fileVersion.Build < 0 ? 0 : fileVersion.Build),
					CRC16 = crc16.ComputeChecksum(filebytes, (int)fileLengh)
				})
			});
			return msgs;

		}

		private static List<byte[]> BuildListPacket(PacketType type, List<object> data)
		{
			List<byte[]> bytes = new List<byte[]>();
			foreach (var o in data)
			{
				Packet p = new Packet(type)
				{
					Data = o
				};
				bytes.Add(p.Serialize());
			}
			return bytes;
		}

		private static byte[] BuildPacket(PacketType type, object data)
		{
			Packet p = new Packet(type)
			{
				Data = data
			};
			byte[] bytes = p.Serialize();
			return bytes;
		}


	}
}
