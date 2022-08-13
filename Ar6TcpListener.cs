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
using Ar6TcpLibrary.Online;
using Newtonsoft.Json;

namespace Ar6Tcp
{
	public class Ar6TcpListener
	{
		public Online online;
		public readonly TcpListener tcpListener;
		public event Action ClientConnectedEvent;
		public event Action Stopping;
		public event Action Stopped;
		public event Action ClientDisconnectedEvent;
		public Ar6TcpListener(IPAddress iPAddress, int port, int maximumOnline, int portForOnlineChecker, int portForOnlineSender)
		{
			online = new Online(iPAddress, maximumOnline, portForOnlineChecker, portForOnlineSender);
			tcpListener = new TcpListener(iPAddress, port);
			tcpListener.Start();
			WaitForClient();
		}
		async void WaitForClient()
		{
			await Task.Run(() =>
			{
				TcpClient newClient = tcpListener.AcceptTcpClient();
				online.AddClient();

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