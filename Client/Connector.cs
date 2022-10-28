using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Client
{
	sealed public class Connector
	{
		public TcpClient ConfirmerClient;
		public TcpClient IsOnlineSenderClient;
		public TcpClient MessengerClient;
		public TcpClient OnlineReceiverClient;
		public TcpClient CommandInfoAgentClient;
		public void Connect(
			IPAddress serverAddress, int confirmerPort, int isOnlinePort,  int messengerPort,  int onlineReceiverPort, int commandInfoSenderPort
			)
		{
			ConfirmerClient = new TcpClient();
			IsOnlineSenderClient = new TcpClient();
			MessengerClient = new TcpClient();
			OnlineReceiverClient = new TcpClient();
			CommandInfoAgentClient = new TcpClient();
			
			ConfirmerClient.Connect(serverAddress, confirmerPort);
			IsOnlineSenderClient.Connect(serverAddress, isOnlinePort);
			OnlineReceiverClient.Connect(serverAddress, onlineReceiverPort);
			MessengerClient.Connect(serverAddress, messengerPort);
			CommandInfoAgentClient.Connect(serverAddress, commandInfoSenderPort);
		}
	}
}
