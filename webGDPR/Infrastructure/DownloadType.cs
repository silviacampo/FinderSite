using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure
{
	public enum DownloadType : byte
	{
		None = 0,
		GpsEphemeris = 1,
		CollarGpsUpdate = 2,
		BaseLoraUpdate = 3,
		BaseBleUpdate = 4,
		CollarLoraUpdate = 5,
		BaseConfig = 6,
		CollarConfig = 7,
		LastFile,      //This should always be the last file. Do not use. Only for validation / for loops
	}
}
