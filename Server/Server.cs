using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Commands;
using Ar6Library.Onlines;
using Ar6Library.User;

namespace Ar6Library.Server
{
	public class Server
	{
		public bool Working { get; private set; }
		private IPAddress _serverAddress { get; }
		private int _onlineCheckerPort { get; }
		private int _acceptorPort { get; }
		private int _onlineSenderPort { get; }
		private int _messengerPort { get; }
		private int _commandInfoAgentPort { get; }
		private int _pingServerPort { get; }

		public UsersOnServer Users;
		private UserAcceptor UserAcceptor { get; set; } // Надо сделать ограничитель по количеству онлайна или вообще удалить ограничитель из UsersOnServer
		private PingAcceptorServer pingServer { get; set; }
		public delegate void UserConnectedHandler(UserOnServer connectedUser);
		public event UserConnectedHandler UserConnectedEvent;
		public event Action ServerStopping;

		public delegate void UserChangeNameHandler(string pastName, string newName);
		public event UserChangeNameHandler UserChangeNameEvent;
		public Server(
			IPAddress serverAddress, int acceptorPort, int onlineCheckerPort, int onlineSenderPort, int messengerPort,
			int commandInfoAgentPort, int pingServerPort)
		{
			_serverAddress = serverAddress;
			_onlineCheckerPort = onlineCheckerPort;
			_acceptorPort = acceptorPort;
			_onlineSenderPort = onlineSenderPort;
			_messengerPort = messengerPort;
			_commandInfoAgentPort = commandInfoAgentPort;
			_pingServerPort = pingServerPort;
		}
		public void Start()
		{
			if (Working)
				return;

			InitializePingServer();
			InitializeUserAcceptor();
			Users = new UsersOnServer(10);

			Working = true;
		}
		public void Stop()
		{
			ServerStopping?.Invoke();
			Working = false;

			Users = null;
			UserAcceptor = null;
			pingServer = null;
		}
		private void InitializePingServer()
		{
			pingServer = new PingAcceptorServer(new IPEndPoint(_serverAddress, _pingServerPort));
			pingServer.Start();
			var threadForPing = new Thread(() =>
			{
				ReceivePingLoop();
			});
			threadForPing.IsBackground = true;
			threadForPing.Start();
		}
		private void ReceivePingLoop()
		{
			while (true)
			{
				var (pingReply, client) = pingServer.ReceivePingAsync().Result;
				if (pingReply.PingResult == Ping.PingResult.Success)
					Server.Log("PING", $"Success: {client.Client.RemoteEndPoint}", ConsoleColor.Green);
				else
					Server.Log("PING", $"PingReply = {pingReply}: {client.Client.RemoteEndPoint}, " +
						$"Exception: {pingReply.Exception}, " +
						$"ReceivedData(byte[]):{string.Join(" ", pingReply.ReceivedData.Select(x => x.ToString()))}, " +
						$"ReceivedData(UTF8-string): {Encoding.UTF8.GetString(pingReply.ReceivedData)}", ConsoleColor.Red);

			}

		}
		private void InitializeUserAcceptor()
		{
			UserAcceptor = new UserAcceptor(
				_serverAddress, acceptorPort: _acceptorPort,
				_serverAddress, senderPort: _onlineSenderPort,
				_serverAddress, checkerPort: _onlineCheckerPort,
				_serverAddress, messengerPort: _messengerPort,
				_serverAddress, commandInfoAgentPort: _commandInfoAgentPort,
				TimeSpan.FromMilliseconds(100));

			UserAcceptor.ClientConnectedEvent += UserAcceptor_ClientConnectedEvent;
		}
		private void UserAcceptor_ClientConnectedEvent(UserOnServer user)
		{
			user.Name = BaseNameUtil.GetBaseName(Users.Onlines.Names);
			Users.AddClient(user);
			user.NameChangedEvent += User_NameChangedEvent;

			UserConnectedEvent?.Invoke(user);
		}
		/// <summary>
		/// Return true if <paramref name="nameToCheck"/> not contains
		/// </summary>
		/// <param name="nameToCheck"></param>
		/// <returns></returns>
		public bool CheckNameNotContained(string nameToCheck)
		{
			return !Users.Onlines.Names.Contains(nameToCheck);
		}
		private void User_NameChangedEvent(string pastName, string newName)
		{
			Users.SetOnlineInfoWrapper();
			UserChangeNameEvent?.Invoke(pastName, newName);
		}
		public static void Log(string foreword, string text, ConsoleColor forewordColor = ConsoleColor.White, string endLine = "\r\n")
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write($"{{{DateTime.Now}}}");
			Console.ForegroundColor = forewordColor;
			Console.Write($"[{foreword}]");
			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write($"{text}{endLine}");
		}
	}
}