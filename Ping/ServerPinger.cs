using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Ping
{
	public sealed class ServerPinger
	{
		public ServerPinger(IPAddress ipToCheck, int portToCheck)
		{
			IpToCheck = ipToCheck;
			PortToCheck = portToCheck;
		}

		/// /// <summary>
		/// Default : <code>TimeSpan.FromSeconds(10)</code>
		/// </summary>
		public TimeSpan MaximumTimeOut { get { return TimeOutMaximumValue; } set { TimeOutMaximumValue = value; } }
		/// <summary>
		/// Default : <code>TimeSpan.FromSeconds(10)</code>
		/// </summary>
		static public TimeSpan TimeOutMaximumValue { get; set; } = TimeSpan.FromSeconds(10);
		public IPAddress IpToCheck { get; }
		public int PortToCheck { get; }
		public event Action<ServerPinger, PingReply> PingCompleted;
		public async Task<PingReply> PingAsync(TimeSpan timeout)
		{
			if (timeout.TotalSeconds > 10)
				throw new Exception("Set smaller timeout");

			var client = new TcpClient();
			PingReply pingReply = null;
			try
			{
				var connectionTask = client.ConnectAsync(IpToCheck, PortToCheck);
				var a = await Task.WhenAny(Task.Run(() => connectionTask.Wait(timeout)));
				if (!a.Result)
					return new PingReply() { PingResult = PingResult.TimeOutIsOver, Exception = connectionTask.Exception };
				client.GetStream().Write(PingStandard.BufferToWrite, 0, PingStandard.BufferToWrite.Length);

				int readByteCount = PingStandard.SUCCESSBuffer.Length;
				var bufferToRead = new byte[readByteCount];
				Task<int> receivedCountTask = client.GetStream().ReadAsync(bufferToRead, 0, readByteCount);

				var a2 = await Task.WhenAny(Task.Run(() => receivedCountTask.Wait(timeout)));
				if (!a2.Result)
					return pingReply = new PingReply { PingResult = PingResult.TimeOutIsOver };

				if (bufferToRead.SequenceEqual(PingStandard.SUCCESSBuffer))
					return pingReply = new PingReply { PingResult = PingResult.Success };

				return pingReply = new PingReply { PingResult = PingResult.ReceivedDataNotEqualsStandard, ReceivedData = bufferToRead };
			}
			catch (Exception ex)
			{
				return pingReply = new PingReply { PingResult = PingResult.Error, Exception = ex };
			}
			finally
			{
				PingCompleted?.Invoke(this, pingReply);
			}
		}
	}
}