using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Client
{
	sealed public class MessengerOnClient
	{
		public TcpClient TcpClient { get; }
		public NetworkStream NetStream { get; }
		public BinaryWriter StreamForSendMessage { get; }
		public BinaryReaderStringSafe StreamForReceiveMessage { get; }
		public bool DataAvailable => NetStream.DataAvailable;
		public MessengerOnClient(TcpClient client)
		{
			NetStream = client.GetStream();
			TcpClient = client;
			StreamForSendMessage = new BinaryWriter(NetStream, Encoding.UTF8);
			StreamForReceiveMessage = new BinaryReaderStringSafe(new BinaryReader(NetStream, Encoding.UTF8));
		}
		public void SendMessage(string text)
		{
			StreamForSendMessage.Write(text);
			StreamForSendMessage.Flush();
		}
		/// <summary>
		/// return null if Server offline
		/// </summary>
		public string ReceiveMessage()
		{
			return StreamForReceiveMessage.ReadSafe();
		}
		public override string ToString()
		{
			return $"{nameof(DataAvailable)}: {DataAvailable}";
		}
	}
}
