using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure.CustomWebSockets.Messages
{
	public class File
	{ 
		public int Type { get; set; }
		public string Filename { get; set; }
	}
}
