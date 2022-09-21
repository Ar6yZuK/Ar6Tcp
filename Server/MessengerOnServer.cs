using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Ar6Library.Server
{
	sealed public class MessengerOnServer
	{
		public TcpClient TcpClient { get; }
		public NetworkStream NetStream { get; }
		public BinaryWriter StreamForSendMessage { get; }
		public BinaryReaderStringSafe StreamForReceiveMessage { get; }
		public bool DataAvailable => NetStream.DataAvailable;
		public MessengerOnServer(TcpClient client)
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