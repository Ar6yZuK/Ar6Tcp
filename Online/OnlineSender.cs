using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Ar6TcpLibrary.Online
{
	public class OnlineSender
	{
		private OnlineInfo _onlineInfo;
		private TcpClient _tcpClient;
		private NetworkStream _streamForSendOnlineInfo;

		public OnlineSender(OnlineInfo onlineInfo)
		{
			_onlineInfo = onlineInfo;
			_tcpClient = new TcpClient();
			_streamForSendOnlineInfo = _tcpClient.GetStream();
		}

		public void SendOnline(OnlineInfos onlineInfos)
		{
			if (!OnlineChecker.CheckClientConnected(_tcpClient))
				throw new InvalidOperationException({nameof(_onlineInfo.State)} is offline");
			var sw = new StreamWriter(_streamForSendOnlineInfo);
			var json = JsonConvert.SerializeObject(onlineInfos, Formatting.Indented);
			sw.Write(json);
		}

		public void Connect(IPAddress serverAddress, int port)
		{
			if (OnlineChecker.CheckClientConnected(_tcpClient))
				return;
			_tcpClient = new TcpClient();
			_tcpClient.Connect(serverAddress, port);

			_streamForSendOnlineInfo = _tcpClient.GetStream();
		}
	}
}