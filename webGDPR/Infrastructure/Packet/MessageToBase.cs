using System;
using System.Collections.Generic;
using System.Text;

namespace webGDPR.Infrastructure.Packet
{
	/// <summary>
	/// Contains the same content as the ACK, plus the bytes and an id to identify it in the queue.
	/// </summary>
	public class MessageToBase
	{
		public int MessageId { get; set; }
		public byte Command { get; set; }
		public byte CollarNumber { get; set; }
		public byte? Return { get; set; }
		public byte ExtendedData { get; set; }
		public uint Index { get; set; }
		public byte[] Bytes { get; set; }
	}
}
