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
		private int _id;
		
		public UsersOnServer Users;
		private UserAcceptor UserAcceptor { get; set; } // Надо сделать ограничитель по количеству онлайна или вообще удалить ограничитель из UsersOnServer

		public delegate void UserConnectedHandler(UserOnServer connectedUser);
		public event UserConnectedHandler UserConnectedEvent;

		public delegate void UserChangeNameHandler(string pastName, string newName);
		public event UserChangeNameHandler UserChangeNameEvent;
		public Server(
			IPAddress serverAddress,  int acceptorPort, int onlineCheckerPort, int onlineSenderPort, int messengerPort,
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
			user.GetNotContainsNameEvent += User_GetNotContainsNameEvent;
			user.SetBaseName();
			Users.AddClient(user);
			user.NameNotContainsEvent += UserNameNotContainsEvent;
			user.NameChangedEvent += User_NameChangedEvent;
			
			UserConnectedEvent?.Invoke(user);
		}


		private string User_GetNotContainsNameEvent()
		{
			var basename = "BaseName" + _id++;
			var names = Users.GetNames();
			while (true)
			{
				if (!names.Contains(basename))
				{
					return basename;
				}
				basename = "Base name" + _id++;
			}
		}

		private void User_NameChangedEvent(string pastName, string newName)
		{
			Users.SetOnlineInfoWrapper();
			Console.WriteLine($"[{DateTime.Now}:{DateTime.Now.Millisecond}]Sending online for all");
			foreach (var name in Users.Onlines.Names)
			{
				Users[name].OnlineSender.SendOnline(Users.Onlines);
			}
			UserChangeNameEvent?.Invoke(pastName, newName);
		}
		private bool UserNameNotContainsEvent(string newName)
		{
			return !Users.GetNames().Contains(newName);
		}
	}
}