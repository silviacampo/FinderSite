using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace webGDPR.Infrastructure.Packet
{
    public class Packet
	{
		public Header Header { get; set; }
		public PacketType Type { get; set; }
		public object Data { get; set; }

		public Packet(Header hdr)
		{
			Header = hdr;
		}

		public Packet(PacketType type)
		{
			Header = new Header(type);
		}

		public byte[] SerializePayload()
		{ 
			switch (Header.Command)
			{
				case PacketType.ConnectionInfo:
					return DataConverter.Serialize((CollarConnectedStruct)Data);
				case PacketType.InitCollar:
					return DataConverter.Serialize((InitCollarStruct)Data);
				case PacketType.GpsCount:
					return DataConverter.Serialize((GpsCountStruct)Data);
				case PacketType.GpsPoint:
					return DataConverter.Serialize((GpsPointStruct)Data);
				case PacketType.BaseStatus:
					return DataConverter.Serialize((BaseStatusStruct)Data);
				case PacketType.CollarStatus:
					return DataConverter.Serialize((CollarStatusStruct)Data);
				case PacketType.SendFile:
					return DataConverter.Serialize((SendFileStruct)Data);
                case PacketType.FileVersion:
                    return DataConverter.Serialize((FileVersionStruct)Data);
                case PacketType.FileData:
					return DataConverter.Serialize((FileDataStruct)Data);
				case PacketType.Query:
					return DataConverter.Serialize((QueryStruct) Data);
				case PacketType.Acknowledge:
					return DataConverter.Serialize((AcknowledgeStruct)Data);
				case PacketType.CollarMode:
					return DataConverter.Serialize((CollarModeStruct)Data);
				case PacketType.PacketReceived:
					return DataConverter.Serialize((PacketReceivedStruct)Data);
				case PacketType.UnitName:
					return DataConverter.Serialize((UnitNameStruct)Data);
				//case PacketType.Loopback:
				//	return DataConverter.Serialize(Data);
				default:
					return new byte[0];
			}

			
		}
		public void DeserializePayload(byte[] b)
		{
			switch (Header.Command)
			{
				case PacketType.ConnectionInfo:
					Data = DataConverter.Deserialize<CollarConnectedStruct>(b);
					break;
				case PacketType.InitCollar:
					Data = DataConverter.Deserialize<InitCollarStruct>(b);
					break;
				case PacketType.GpsCount:
					Data = DataConverter.Deserialize<GpsCountStruct>(b);
					break;
				case PacketType.GpsPoint:
					Data = DataConverter.Deserialize<GpsPointStruct>(b);
					break;
				case PacketType.BaseStatus:
					Data = DataConverter.Deserialize<BaseStatusStruct>(b);
					break;
				case PacketType.CollarStatus:
					Data = DataConverter.Deserialize<CollarStatusStruct>(b);
					break;
				case PacketType.SendFile:
					Data = DataConverter.Deserialize<SendFileStruct>(b);
					break;
                case PacketType.FileVersion:
                    Data = DataConverter.Deserialize<FileVersionStruct>(b);
                    break;
                case PacketType.FileData:
					Data = DataConverter.Deserialize<FileDataStruct>(b);
					break;
				case PacketType.Query:
					Data = DataConverter.Deserialize<QueryStruct>(b);
					break;
				case PacketType.Acknowledge:
					Data = DataConverter.Deserialize<AcknowledgeStruct>(b);
					break;
				case PacketType.CollarMode:
					Data = DataConverter.Deserialize<CollarModeStruct>(b);
					break;
				case PacketType.PacketReceived:
					Data = DataConverter.Deserialize<PacketReceivedStruct>(b);
					break;
				case PacketType.UnitName:
                    Data = DataConverter.Deserialize<UnitNameStruct>(b);
				//case PacketType.Loopback:
				//	Data = DataConverter.Deserialize<T>(b);
				    break;
				default:
					Data = b;
					break;
			}

					
		}

		public byte[] Serialize()
		{
			byte[] hdr;
			byte[] payload;
			byte[] datagram;

			// Generate Payload portion
			payload = SerializePayload();

			// Adjust deserialized header using actual payload length
			hdr = Header.Serialize((byte)(payload.Length));

			// Assemble the whole datagram
			datagram = new byte[payload.Length + Header.Size + Crc8.Size];
			hdr.CopyTo(datagram, 0);
			payload.CopyTo(datagram, Header.Size);

			// Compute and append a CRC-8 at the end
			byte checkSum;
			if ((datagram[2] == (byte)PacketType.PacketReceived) || (datagram[2] == (byte)PacketType.Loopback))
			{
				checkSum = DataConverter.ComputeChecksum(datagram, datagram.Length - 1);    //simplified checksum
			}
			else
			{
				checkSum = Crc8.ComputeCrc8(datagram, datagram.Length - 1);
			}

			datagram[datagram.Length - 1] = checkSum;

			return datagram;
		}

		public static Packet DeserializeAndValidate(byte[] datagram)
		{
			
			Header hdr = new Header();
			byte[] payload;

			if (datagram == null)
			{
				throw new Exception("Packet.DeserializeAndValidate: null datagram");
			}

			// First attempt to extract Header contents
			try
			{
				hdr.DeserializeAndValidate(datagram);
			}
			catch (Exception)
			{
				throw new Exception("Packet.DeserializeAndValidate: bad header");
			}

			// Verify remaining packet length against what was
			// announced in the Header
			if (datagram.Length != Header.Size + hdr.PayloadLength + Crc8.Size)
			{
				throw new Exception("Packet.DeserializeAndValidate: wrong payload length");
			}

			// Compute and verify CRC-8
			byte checkSum;
			if ((datagram[2] == (byte)PacketType.PacketReceived) || (datagram[2] == (byte)PacketType.Loopback))
			{
				checkSum = DataConverter.ComputeChecksum(datagram, datagram.Length - 1);    //simplified checksum
			}
			else
			{
				checkSum = Crc8.ComputeCrc8(datagram, datagram.Length - 1);
			}

			if (datagram[datagram.Length - 1] != checkSum)
			{
				throw new Exception("Packet.DeserializeAndValidate: bad CRC-8");
			}

			// Construct an empty Packet based on the Operation code in the header.
			// Use already deserialized the Header for this.
			Packet pkt = new Packet(hdr)
			{
				Type = hdr.Command
			};
			// Deserialize payload
			payload = new byte[hdr.PayloadLength];
			Array.Copy(datagram, Header.Size, payload, 0, hdr.PayloadLength);
			pkt.DeserializePayload(payload);
			return pkt;
		}

		public override string ToString()
		{
			Type type = Data.GetType();
			FieldInfo[] fields = type.GetFields();
			PropertyInfo[] properties = type.GetProperties();

			Dictionary<string, object> values = new Dictionary<string, object>
			{
				{ "Packet: ", this.Type.ToString() }
			};
			Array.ForEach(fields, (field) => values.Add(field.Name, field.GetValue(Data)));
			Array.ForEach(properties, (property) =>
			{
				if (property.CanRead)
					values.Add(property.Name, property.GetValue(Data, null));
			});

			return String.Join(", ", values);
		}

		public List<Tuple<string, string>> GetValues() {
			Type type = Data.GetType();
			FieldInfo[] fields = type.GetFields();
			PropertyInfo[] properties = type.GetProperties();

			Dictionary<string, object> values = new Dictionary<string, object>
			{
				{ "Command ", this.Type.ToString() }
			};
			Array.ForEach(fields, (field) => values.Add(field.Name, field.GetValue(Data)));
			Array.ForEach(properties, (property) =>
			{
				if (property.CanRead)
					values.Add(property.Name, property.GetValue(Data, null));
			});

			List<Tuple<string, string>> items = new List<Tuple<string, string>>();
			foreach (var property in values) {
				items.Add(Tuple.Create(property.Key, property.Value.ToString()));
			}
			return items;
		}

		public byte GetCollar()
		{
			List<Tuple<string, string>> values = GetValues();
			Tuple<string, string> collarnumber = values.FirstOrDefault(v => v.Item1 == "CollarNumber");
			if (collarnumber != null)
			{

				return (byte)(byte.Parse(collarnumber.Item2) & Constants.CollarNumberMask);
			}
			else {
				return 0;
			}
		}

		public static PacketType GetCommand(byte command)
		{
			return (PacketType)Enum.ToObject(typeof(PacketType), command);
		}
	}

	public enum PacketType : byte
	{
		None = 0,
		ConnectionInfo = 1,
		InitCollar = 2,
		GpsCount = 3,
		GpsPoint = 4,
		CollarStatus = 5,
		BaseStatus = 6,
		SendFile = 7,
		FileData = 8,
		Query = 9,
		Acknowledge = 10,
		CollarMode = 11,
		PacketReceived = 12,
		Loopback = 13,
        UnitName = 14,
        FileVersion = 15,
		LastCommand,      //This should always be the last command. Do not use. Only for validation
	}
}
