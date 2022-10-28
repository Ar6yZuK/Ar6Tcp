using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.User;

namespace Ar6Library.Server
{
	sealed public class UserAcceptor
	{
		private TimeSpan _timeForNewConnect;
		private TcpListener _acceptorServer;
		private TcpListener _onlineSenderServer;
		private TcpListener _checkerServer;
		private TcpListener _messengerServer;
		private TcpListener _commandInfoAgentServer;

		public delegate void ClientConnectedHandler(UserOnServer userOnServer);

		public event ClientConnectedHandler ClientConnectedEvent;
		public UserAcceptor(
			IPAddress acceptorAddress, int acceptorPort,
			IPAddress senderAddress, int senderPort,
			IPAddress checkerAddress, int checkerPort,
			IPAddress messengerAddress, int messengerPort,
			IPAddress commandInfoAgentAddress, int commandInfoAgentPort,
			TimeSpan timeForNewConnect
		)
		{
			_acceptorServer = new TcpListener(acceptorAddress, acceptorPort);
			_checkerServer = new TcpListener(checkerAddress, checkerPort);
			_onlineSenderServer = new TcpListener(senderAddress, senderPort);
			_messengerServer = new TcpListener(messengerAddress, messengerPort);
			_commandInfoAgentServer = new TcpListener(commandInfoAgentAddress, commandInfoAgentPort);
			StartAllListeners();

			_timeForNewConnect = timeForNewConnect;
			LoopAcceptAndPrepare();
		}
		private void StartAllListeners()
		{
			foreach (var propertyInfo in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (propertyInfo.FieldType == typeof(TcpListener))
					((TcpListener)propertyInfo.GetValue(this)).Start();
			}
		}
		private async void LoopAcceptAndPrepare()
		{
			await Task.Run(() =>
			{
				while (true)
				{
					var user = AcceptUser();
					PrepareUser(user);
					Thread.Sleep(_timeForNewConnect);
				}
				// ReSharper disable once FunctionNeverReturns (Когда нибудь я куплю платную подписку, а пока пробник закончился) оставлю комментарий на память
			});
		}
		private void PrepareUser(UserOnServer user)
		{
			Thread thread = new Thread(() =>
			{
				ClientConnectedEvent?.Invoke(user);
			});
			thread.IsBackground = true;
			thread.Start();
		}
		private UserOnServer AcceptUser()
		{
			var acceptorTcp = _acceptorServer.AcceptTcpClient();

			var onlineCheckerClient = _checkerServer.AcceptTcpClient();
			var onlineSenderClient = _onlineSenderServer.AcceptTcpClient();
			var messengerClient = _messengerServer.AcceptTcpClient();
			var commandInfoAgentClient = _commandInfoAgentServer.AcceptTcpClient();

			var user = new UserOnServer(messenger: messengerClient, onlineSender: onlineSenderClient,
							onlineChecker: onlineCheckerClient, commandInfoAgentClient, acceptorTcp);
			return user;
		}
	}
}