using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Ping
{
	internal class PingStandard
	{
		internal static byte[] BufferToWrite => Encoding.UTF8.GetBytes("Ping");
		internal static byte[] SUCCESSBuffer => Encoding.UTF8.GetBytes("SUCCESS"); // Standard
	}
}
