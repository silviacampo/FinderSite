using System;
using System.Collections.Generic;
using System.Text;

namespace webGDPR.Infrastructure.Packet
{
    class Constants
    {
		public const byte CollarNumberMask = 0b01111111;
		public const byte CollarAny = 0b01111111;
		public const byte CollarNone = 0b01111110;
		public const byte KeepAliveMask = 0b10000000;
		public const byte ValidGps = 0b10000000;
		public const byte ActivityMask = 0b00001111;
		public const byte ErrorMask = 0b11110000;
		public const byte NumSatMask = 0b00001111;
		public const byte ConnectedMask = 0b00000001;
		public const byte DisconnectedMask = 0b00000010;
		public const byte TimeoutMask = 0b00000100;

		public const byte CollarLoraFail = 0b00000010;
		public const byte CollarGpsFail = 0b00000001;

		#region SwitchMode
		public const byte SwitchModeEmergencyGpsPeriod = 0;
		public const byte SwitchModeEmergencyGpsDuration = 60;
		public const byte SwitchModeEmergencyBandwidth = 10;
		public const byte SwitchModeEmergencySpreadFactor = 15;
		public const byte SwitchModeEmergencyBaseTimeout = 30;

		public const byte SwitchModeNormalGpsPeriod = 10;
		public const byte SwitchModeNormalGpsDuration = 30;
		public const byte SwitchModeNormalBandwidth = 5;
		public const byte SwitchModeNormalSpreadFactor = 10;
		public const byte SwitchModeNormalBaseTimeout = 10;
		#endregion
	}
}
