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
		Base = 1,  // O
		Collar = 2,  // O
		Bases = 3, // O
		Collars = 4,  // O
		DeviceInfo = 5,
		LastGPS = 6,
		TrackingInfo = 7,
		DeletedBase = 8,  // O
		DeletedCollar = 9,  // O
		BaseCore = 10,  // O
		CollarCore = 11,  // O
		BaseStatus = 12,
		CollarStatus = 13,
		DiscoverBases = 14,
		DeviceBanned = 15,  // O
		DownloadFile = 16,  // O
		Packet = 17,
		SwitchMode = 18, // O
		MissingSubscription = 19, // O
		Login = 20
	}
}
