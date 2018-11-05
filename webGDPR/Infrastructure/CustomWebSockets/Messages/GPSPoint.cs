using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
    public class GPSPoint
    {
		[JsonProperty(PropertyName = "cn")]
		public byte CollarNumber { get; set; }
		[JsonProperty(PropertyName = "la")]
		public double Latitude { get; set; }
		[JsonProperty(PropertyName = "lo")]
		public double Longitude { get; set; }
		[JsonProperty(PropertyName = "d")]
		[JsonConverter(typeof(Newtonsoft.Json.Converters.UnixDateTimeConverter))]
		public DateTime CreatedDate { get; set; }
	}
}
