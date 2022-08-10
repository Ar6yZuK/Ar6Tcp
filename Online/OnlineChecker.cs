using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6TcpLibrary.Online
{
	public class OnlineChecker
	{
		private bool _online = false;
		public TimeSpan TimeForCheckEveryTime;
		private TcpClient _tcpClient;
		private async void LoopOnlineSetter()
		{
			while (true)
			{
				_online = CheckClientConnected(_tcpClient);
#if DEBUG
				Console.WriteLine($"OnlineChecked:{_online}");
#endif
				await Task.Delay(TimeForCheckEveryTime);
			}
		}
		public bool Connected
		{
			get
			{
				return _online; 
			}
		}
		public OnlineChecker(TimeSpan? timeForCheckEveryTime = null)
		{
			_tcpClient = new TcpClient();
			TimeForCheckEveryTime = timeForCheckEveryTime ?? new TimeSpan(0, 0, 0, 1);
		}

		private bool _isFirstConnect = true;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="port"></param>
		/// <exception cref="SocketException"></exception>
		public void Connect(IPAddress address, int port)
		{
			if (CheckClientConnected(_tcpClient))
				return;
			_tcpClient = new TcpClient();
			_tcpClient.Connect(address, port);
			if (_isFirstConnect)
			{
				LoopOnlineSetter();
				_isFirstConnect = false;
			}
		}
		public static bool CheckClientConnected(TcpClient tcpClient)
		{
			if (tcpClient == null)
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
				clientStream.Write(buffer, 0, 1);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
