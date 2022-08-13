using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ar6TcpLibrary.Online
{
	public class OnlineChecker
	{
		private bool _online = false;
		public TimeSpan TimeForCheckEveryTime;
		private TcpClient _tcpClient;
		private bool _checking = false;
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
		/// <exception cref="SocketException"></exception>
		public void Connect(IPAddress serverAddress, int port)
		{
			if (CheckClientConnected(_tcpClient))
				return;
			_tcpClient = new TcpClient();
			_tcpClient.Connect(serverAddress, port);
			if (!_checking)
			{
				LoopOnlineSetter();
				_checking = true;
			}
		}
		public void Close()
		{
			_tcpClient.Close();
		}
		public static bool CheckClientConnected(TcpClient tcpClient)
		{
			if (tcpClient == null)
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
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
