using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Commands;
using Ar6Library.Onlines;
using Ar6Library.User;

namespace Ar6Library.Server
{
	public class Server
	{
		private bool _started;
		private IPAddress _serverAddress;
		private int _onlineCheckerPort;
		private int _acceptorPort;
		private int _onlineSenderPort;
		private int _messengerPort;
		private int _commandInfoAgentPort;

		public UsersOnServer Users;
		private UserAcceptor UserAcceptor { get; set; } // Надо сделать ограничитель по количеству онлайна или вообще удалить ограничитель из UsersOnServer

		public delegate void UserConnectedHandler(UserOnServer connectedUser);
		public event UserConnectedHandler UserConnectedEvent;

		public delegate void UserChangeNameHandler(string pastName, string newName);
		public event UserChangeNameHandler UserChangeNameEvent;
		public Server(
			IPAddress serverAddress, int acceptorPort, int onlineCheckerPort, int onlineSenderPort, int messengerPort,
			int commandInfoAgentPort)
		{
			_serverAddress = serverAddress;
			_onlineCheckerPort = onlineCheckerPort;
			_acceptorPort = acceptorPort;
			_onlineSenderPort = onlineSenderPort;
			_messengerPort = messengerPort;
			_commandInfoAgentPort = commandInfoAgentPort;
		}
		public void Start()
		{
			if (_started)
				return;

			InitializeUserAcceptor();
			Users = new UsersOnServer(10);

			_started = true;
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