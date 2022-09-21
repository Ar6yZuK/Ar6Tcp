using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ar6Library.Onlines;
using Ar6Library.User;

namespace Ar6Library.Server
{
	sealed public class OnlineSenderOnServer
	{
		public TcpClient TcpClient { get; }
		public NetworkStream NetStream { get; }
		private BinaryWriter _binaryWriter;
		public OnlineSenderOnServer(TcpClient tcpClient)
		{
			TcpClient = tcpClient;
			NetStream = tcpClient.GetStream();
			_binaryWriter = new BinaryWriter(NetStream);
		}
		/// <exception cref="InvalidOperationException"></exception>
		public void SendOnline(OnlineInfoWrapper onlineInfos)
		{
			if (!OnlineChecker.CheckClientConnected(TcpClient))
				return;

			var jsonStr = onlineInfos.ToString();

			_binaryWriter.Write(jsonStr);
			_binaryWriter.Flush();
		}
	}
}