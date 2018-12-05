using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace webGDPR.Infrastructure.Packet
{
	public class Header
	{
		public const byte Magic = 0x55;
		private PacketHeader hdr;

		static public uint Size
		{
			get
			{
				return (uint)Marshal.SizeOf(typeof(PacketHeader));
			}
		}

		public PacketType Command
		{
			get
			{
				return (PacketType)Enum.ToObject(typeof(PacketType), hdr.Command);
			}
		}
		public int PayloadLength
		{
			get
			{
				return hdr.PayloadLength;
			}
		}
		public int PacketLength
		{
			get
			{
				return (int)Header.Size + hdr.PayloadLength + Crc8.Size;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct PacketHeader
		{
			public byte Magic;
			public byte PayloadLength;
			public byte Command;
		}

		/// <summary>
		///     Header constructor, used only for generated packets (i.e. to be sent out).
		/// </summary>
		/// <param name="command">
		///     Opcode that specifies the binary format for the packet payload.
		/// </param>
		public Header(PacketType command = PacketType.None)
		{
			hdr.Magic = Magic;
			hdr.Command = (byte)command;
			hdr.PayloadLength = 0;
		}

		/// <summary>
		///     Serializes an Header, with adjusted values for some properties.
		/// </summary>
		/// <param name="length">
		///     Length of the payload portion in 32bit words, including padding if required.
		/// </param>
		/// <returns>
		///     Byte array representing the serialized Header, in network byte order.
		/// </returns>
		public byte[] Serialize(byte length)
		{
			hdr.PayloadLength = length;
			return DataConverter.Serialize<PacketHeader>(hdr);
		}
		/// <summary>
		///     Header Factory that deserializes and validates a byte array.
		/// </summary>
		/// <param name="rawData">
		///     Byte array representing the serialized Header, in network byte order.
		/// </param>
		/// <returns>
		///     IF valid.
		/// </returns>
		public bool DeserializeAndValidate(byte[] rawData)
		{
			hdr = DataConverter.Deserialize<PacketHeader>(rawData);
			return Validate();
		}

		private bool Validate()
		{
			if (hdr.Magic != Magic)
				throw new Exception("Header.Validate - fail, magic");
			if (hdr.Command >= (byte)PacketType.LastCommand)
				throw new Exception("Header.Validate - fail, Command");
			return true;
		}
	}

}
