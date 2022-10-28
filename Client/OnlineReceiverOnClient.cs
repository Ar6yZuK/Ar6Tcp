using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ar6Library.Onlines;

namespace Ar6Library.Client
{
	sealed public class OnlineReceiverOnClient
	{
		public bool DataAvailable => NetStream.DataAvailable;
		
		public TcpClient TcpClient { get; }
		public NetworkStream NetStream { get; }
		private BinaryReaderStringSafe _binaryReader { get; }
		public OnlineReceiverOnClient(TcpClient tcpClient)
		{
			TcpClient = tcpClient;
			NetStream = TcpClient.GetStream();
			_binaryReader = new BinaryReaderStringSafe(new BinaryReader(NetStream));
		}
		
		/// <summary>
		/// return null if no connection
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public OnlineInfoWrapper ReceiveOnlineInfos()
		{
			if (!OnlineChecker.CheckClientConnected(TcpClient))
				return null;

			var strToParse = _binaryReader.ReadSafe();
			if (strToParse is null)
				return null;

			var onlineInfoWrapper = OnlineInfoWrapper.Parse(strToParse);

			return onlineInfoWrapper;
		}
		public override string ToString()
		{
			return $"{nameof(DataAvailable)}: {DataAvailable}";
		}
	}
}
