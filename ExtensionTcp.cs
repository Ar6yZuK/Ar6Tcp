using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ar6Tcp
{
	public static class ExtensionTcp
	{
		public static TcpState GetState(this TcpClient tcpClient)
		{
			var foo = IPGlobalProperties.GetIPGlobalProperties()
			  .GetActiveTcpConnections()
			  .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint));
			return foo != null ? foo.State : TcpState.Unknown;
		}
	}
}