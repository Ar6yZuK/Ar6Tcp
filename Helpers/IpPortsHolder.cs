using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Helpers
{
	public class IpPortsHolder
	{
		public IpPortsHolder(IPAddress iPAddress, int confirmerPort, int isOnlinePort, int messengerPort, int onlineReceiverPort, int commandInfoAgentPort)
		{
			IPAddress = iPAddress;
			ConfirmerPort = confirmerPort;
			IsOnlinePort = isOnlinePort;
			MessengerPort = messengerPort;
			OnlineReceiverPort = onlineReceiverPort;
			CommandInfoAgentPort = commandInfoAgentPort;
		}
		public IPAddress IPAddress { get; set; }
		public int ConfirmerPort { get; set; }
		public int IsOnlinePort { get; set; }
		public int MessengerPort { get; set; }
		public int OnlineReceiverPort { get; set; }
		public int CommandInfoAgentPort { get; set; }
	}
}
