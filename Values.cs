using System.Net.Sockets;

namespace Ar6Tcp
{
	public class Values
	{
		public TcpClient Client { get; private set; }
		public string DisplayName { get; set; }

		public Values(TcpClient tcpClient, string displayName)
		{
			Client = tcpClient;
			DisplayName = displayName;
		}

		public override string ToString()
		{
			return string.Format("{0};{1}", Client, DisplayName);
		}
	}
}
