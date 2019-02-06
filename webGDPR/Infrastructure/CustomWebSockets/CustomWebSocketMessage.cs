using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets
{
	public class CustomWebSocketMessage
	{
		public const string SystemUserId = "system";

		[JsonProperty(PropertyName = "m")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "d")]
		[JsonConverter(typeof(Newtonsoft.Json.Converters.UnixDateTimeConverter))]
		public DateTime MessagDateTime { get; set; }

		[JsonProperty(PropertyName = "u")]
		public string UserId { get; set; }

		[JsonProperty(PropertyName = "t")]
		public WSMessageType Type { get; set; }
	}

	

	public enum WSMessageType
	{
		None = 0,
		Base = 1,
		Collar = 2,
		Bases = 3,
		Collars = 4,
		DeviceInfo = 5,
		LastGPS = 6,
		TrackingInfo = 7,
		DeletedBase = 8,
		DeletedCollar = 9,
		BaseCore = 10,
		CollarCore = 11,
		BaseStatus = 12,
		CollarStatus = 13,
		DiscoverBases = 14,
		DeviceBanned = 15,
		DownloadFile = 16,
		Packet = 17,
		SwitchMode = 18,
		MissingSubscription = 19,
		Login = 20
	}
}
