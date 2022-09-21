using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ar6Library.Onlines
{
	sealed public class OnlineChecker
	{
		private bool _online = false;
		public TimeSpan TimeForCheckEveryTime;
		private TcpClient[] _tcpClients;
		public delegate void UserDisconnectedHandler();
		public event UserDisconnectedHandler UserDisconnectedEvent;
		public bool Connected => _online;
		public async Task LoopOnlineSetterAsync()
		{
			await Task.Run(() =>
			{
				while (true)
				{
					_online = CheckClientsConnected(_tcpClients);
					if (!Connected)
					{
						UserDisconnectedEvent?.Invoke();
						foreach (var tcpClient in _tcpClients)
						{
							tcpClient.Close();
						}
						Array.Clear(_tcpClients, 0, _tcpClients.Length);
						return;
					}
					Thread.Sleep(TimeForCheckEveryTime);
				}
			});
		}
		public bool IsConnected()
		{
			return CheckClientsConnected(_tcpClients);
		}
		/// <exception cref="ArgumentNullException"></exception>
		public OnlineChecker(TcpClient[] tcpClients, TimeSpan? timeForCheckEveryTime = null)
		{
			_tcpClients = tcpClients;
			TimeForCheckEveryTime = timeForCheckEveryTime ?? new TimeSpan(0, 0, 0, 1);
		}
		/// <summary>
		/// throw exception if <paramref name="tcpClients"/>.Length == 0
		/// </summary>
		/// <exception cref="Exception"></exception>
		public static bool CheckClientsConnected(TcpClient[] tcpClients)
		{
			if (tcpClients.Length == 0)
			{
				throw new Exception("Length equals 0");
			}

			foreach (var tcpClient in tcpClients)
			{
				if (!CheckClientConnected(tcpClient))
					return false;
			}

			return true;
		}
		public static bool CheckClientConnected(TcpClient tcpClient)
		{
			if (tcpClient is null)
				return false;
			if (tcpClient.Client is null)
				return false;

			if (!tcpClient.Client.Connected)
				return false;
			if (!tcpClient.Connected)
				return false;

			NetworkStream clientStream;
			try
			{
				clientStream = tcpClient.GetStream();
			}
			catch (Exception) { return false; }
			
			if (!clientStream.CanRead)
				return false;
			if (!clientStream.CanWrite)
				return false;
			var buffer = new byte[1];
			try
			{
				clientStream.Write(buffer, 0, 0);
				if(tcpClient.Client.Poll(0, SelectMode.SelectRead)) // Без этих двух условных конструкций сервер отслеживал отключение только в режиме Debug когда я ставил точку останова на строчку с обращением к Socket.Connected
					if (tcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
						return false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
