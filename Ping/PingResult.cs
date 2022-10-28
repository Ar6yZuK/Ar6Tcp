using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Ping
{
	public enum PingResult
	{
		Undefined,
		Success,
		TimeOutIsOver,
		ReceivedDataNotEqualsStandard,
		Error
	}
}
