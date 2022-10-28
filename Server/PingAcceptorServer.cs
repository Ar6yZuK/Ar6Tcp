using Ar6Library.Ping;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ar6Library.Server
{
	public class PingAcceptorServer : TcpListener
	{
		public PingAcceptorServer(IPEndPoint serverIpPort) : base(serverIpPort)
		{
		}
		public async Task<(PingReply, TcpClient)> ReceivePingAsync()
		{
			TcpClient client = await AcceptTcpClientAsync();

			try
			{
				byte[] readBuffer = new byte[PingStandard.BufferToWrite.Length];
				var readTask = client.GetStream().ReadAsync(readBuffer, 0, readBuffer.Length);
				if (!readTask.Wait(ServerPinger.TimeOutMaximumValue))
				{
					client.Client.Disconnect(false);
					return (new PingReply{ PingResult = PingResult.TimeOutIsOver}, client);
				}
				if (readBuffer.SequenceEqual(PingStandard.BufferToWrite))
				{
					client.GetStream().Write(PingStandard.SUCCESSBuffer, 0, PingStandard.SUCCESSBuffer.Length);
					client.Client.Disconnect(false);
					return (new PingReply { PingResult = PingResult.Success }, client);
				}
				return (new PingReply { PingResult = PingResult.ReceivedDataNotEqualsStandard, ReceivedData = readBuffer }, client);
			}
			catch (System.Exception ex)
			{
				return (new PingReply() { Exception = ex, PingResult = PingResult.Error }, client);
			}
		}
	}
}