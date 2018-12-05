using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace webGDPR.Infrastructure.Packet
{
    class MessageFromBaseHandler
    {
		//public delegate void MessageSender(byte[] message);

		//public void HandleACK(List<MessageToBase> messagesToSend, List<Tuple<string, string>> valuesInMessage, MessageSender sender) {
		//	foreach (var msg in messagesToSend.OrderBy(m => m.MessageId))
		//	{
		//		if (msg.Command == byte.Parse(valuesInMessage.First(v => v.Item1 == "Command").Item2) &&
		//			msg.CollarNumber == byte.Parse(valuesInMessage.First(v => v.Item1 == "CollarNumber").Item2) &&
		//			msg.ExtendedData == byte.Parse(valuesInMessage.First(v => v.Item1 == "ExtendedData").Item2) &&
		//			msg.Index == uint.Parse(valuesInMessage.First(v => v.Item1 == "Index").Item2))
		//		{
		//			Console.WriteLine($"ack message {msg.MessageId} received");

		//			if (msg.Return != 0 && byte.Parse(valuesInMessage.First(v => v.Item1 == "Return").Item2) != 0)
		//			{
		//				messagesToSend.Remove(msg);
		//				Console.WriteLine($"message {msg.MessageId} dump");
		//				//execute the next
		//				var nextMsg = messagesToSend.FirstOrDefault(m => m.MessageId == (msg.MessageId + 1));
		//				if (nextMsg != null) {
		//					Console.WriteLine($"message {nextMsg.MessageId} sent");
		//					sender(nextMsg.Bytes);
		//				}
		//				break;
		//			}

		//			msg.Return = byte.Parse(valuesInMessage.First(v => v.Item1 == "Return").Item2);
		//			if (msg.Return == 0)
		//			{
		//				//execute the next
		//				var nextMsg = messagesToSend.FirstOrDefault(m => m.MessageId == (msg.MessageId + 1));
		//				if (nextMsg != null) {
		//					Console.WriteLine($"message {nextMsg.MessageId} sent");
		//					sender(nextMsg.Bytes);
		//				}
		//			}
		//			else
		//			{
		//				//reexecute
		//				Console.WriteLine($"message {msg.MessageId} sent");
		//				sender(msg.Bytes);
		//			}
		//			break;
		//		}
		//	}
		//	messagesToSend.RemoveAll(m => m.Return == 0);
		//}

		//public CollarStatus HandleCollarConnectionInfo(List<Tuple<string, string>> valuesInMessage, byte reporterBaseNumber, List<BLEModels.Collar> collars) {
		//	string unitid = valuesInMessage.First(v => v.Item1 == "UnitId").Item2;
		//	var hwid = int.Parse(unitid);
		//	var b = collars.Find(c => c.HWId == hwid);
		//	b.IsConnected = true;
		//	CollarStatus bs = new CollarStatus
		//	{
		//		CollarNumber = b.CollarNumber,
		//		BaseNumber =  reporterBaseNumber,
		//		IsConnected = b.IsConnected,
		//		ConnectedTo = "Me",
		//		IsGPSConnected = false,
		//		Battery = 0,
		//		Radio = 0
		//	};
		//	return bs;
		//}

		//public CollarStatus HandleCollarStatus(List<Tuple<string, string>> valuesInMessage, byte reporterBaseNumber, List<BLEModels.Collar> collars)
		//{
		//	string collarn = valuesInMessage.First(v => v.Item1 == "CollarNumber").Item2;
		//	var collarnumber = byte.Parse(collarn);
		//	var co = collars.Find(c => c.CollarNumber == collarnumber);
		//	co.IsGPSConnected = bool.Parse(valuesInMessage.First(v => v.Item1 == "GpsLocks").Item2);
		//	co.Battery = int.Parse(valuesInMessage.First(v => v.Item1 == "BatVoltage").Item2);
		//	co.Radio = int.Parse(valuesInMessage.First(v => v.Item1 == "LoraRxPower").Item2);
		//	CollarStatus cs = new CollarStatus
		//	{
		//		CollarNumber = co.CollarNumber,
		//		BaseNumber = reporterBaseNumber,
		//		IsConnected = true,
		//		ConnectedTo = "Me",
		//		IsGPSConnected = co.IsGPSConnected,
		//		Battery = 0,
		//		Radio = co.Radio
		//	};
		//	return cs;
		//}

		//public GPSPoint HandleCollarGPSPoint(List<Tuple<string, string>> valuesInMessage, byte reporterBaseNumber, List<BLEModels.Collar> collars) {
		//	GPSPoint point = new GPSPoint
		//	{
		//		CollarNumber = byte.Parse(valuesInMessage.First(v => v.Item1 == "CollarNumber").Item2),
		//		CreatedDate = UnixTimestampToDateTime(valuesInMessage.First(v => v.Item1 == "Epoch").Item2),
		//		Latitude = double.Parse(valuesInMessage.First(v => v.Item1 == "Latitude").Item2) / 10000000f,
		//		Longitude = double.Parse(valuesInMessage.First(v => v.Item1 == "Longitude").Item2) / 10000000f
		//	};
		//	return point;
		//}

		//private static DateTime UnixTimestampToDateTime(string time)
		//{
		//	double unixTime = double.Parse(time);
		//	DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		//	long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
		//	return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
		//}

		//public BaseStatus HandleBaseStatus(List<Tuple<string, string>> valuesInMessage, byte reporterBaseNumber, List<BLEModels.Base> bases)
		//{
		//	string basen = valuesInMessage.First(v => v.Item1 == "BaseNumber").Item2;
		//	var basenumber = byte.Parse(basen);
		//	var co = bases.Find(c => c.BaseNumber == basenumber);
		//	co.IsPlugged = bool.Parse(valuesInMessage.First(v => v.Item1 == "ChargingStatus").Item2);
		//	co.IsCharging = bool.Parse(valuesInMessage.First(v => v.Item1 == "ChargingStatus").Item2);
		//	co.HasBattery = bool.Parse(valuesInMessage.First(v => v.Item1 == "GpsFail").Item2);
		//	co.Battery = int.Parse(valuesInMessage.First(v => v.Item1 == "BatVoltage").Item2);
		//	co.Radio = int.Parse(valuesInMessage.First(v => v.Item1 == "LoraRxPower").Item2);
		//	BaseStatus cs = new BaseStatus
		//	{
		//		BaseNumber = co.BaseNumber,
		//		IsConnected = true,
		//		ConnectedTo = "Me",
		//		IsPlugged = co.IsPlugged,
		//		IsCharging = co.IsCharging,
		//		HasBattery = co.HasBattery,
		//		Battery = co.Battery,
		//		Radio = co.Radio
		//	};
		//	return cs;
		//}
	}
}
