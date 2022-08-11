using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ar6Tcp.MessageWrappers;
using Ar6Tcp.SomethingSend;
using Newtonsoft.Json;

namespace Ar6Tcp
{
	public class Ar6TcpListener
	{
		#region defaults
		/// <summary>
		/// 192.168.1.16
		/// </summary>
		static IPAddress defaultIP = Dns.GetHostAddresses(Dns.GetHostName())[1];
		/// <summary>
		/// 2530
		/// </summary>
		// static int defaultPort = 2530;
		#endregion
		public readonly TcpListener tcpListener;
		public delegate void newClientConnectedHandler(TcpClient newClient, int ID);
		public event newClientConnectedHandler ClientConnectedEvent;
		public event Action Stopping;
		public event Action Stopped;

		public delegate void ClientDisconnectedHandler(int ID);
		public event ClientDisconnectedHandler ClientDisconnectedEvent;

		OnlineInfo _onlineInfos = new OnlineInfo() { IDNames = new Dictionary<int, string>() };
		public Ar6TcpListener() : this(defaultIP, defaultPort)
		{
		}
		public Ar6TcpListener(IPAddress iPAddress, int port)
		{
			tcpListener = new TcpListener(iPAddress, port);
			tcpListener.Start();
			WaitForClient();
		}
		async void WaitForClient()
		{
			await Task.Run(() =>
			{
				TcpClient newClient = tcpListener.AcceptTcpClient();

				ClientConnectedEvent?.Invoke(newClient, 0);

				Thread threadClientDisconnected = new Thread(() => { });
				threadClientDisconnected.IsBackground = true;
				threadClientDisconnected.Start();

				WaitForClient();
			});
		}
		
		public void Stop()
		{
			if (tcpListener != null)
			{
				Stopping?.Invoke();
				tcpListener.Stop();
				Stopped?.Invoke();
			}
		}
	}
}
