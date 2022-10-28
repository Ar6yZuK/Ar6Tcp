using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Ping
{
	public class PingReply
	{
		public PingResult PingResult = PingResult.Undefined;
		public Exception Exception;
		public byte[] ReceivedData;
	}
}
