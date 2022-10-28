using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Onlines;
using Ar6Library.Server;

namespace Ar6Library.User
{
	sealed public class UserOnServer : IEquatable<UserOnServer>
	{
		public MessengerOnServer Messenger { get; }
		public OnlineChecker OnlineChecker { get; }
		public OnlineSenderOnServer OnlineSender { get; }
		public OnlineInfoOnServer OnlineInfoOnServer { get; }
		public CommandInfoAgent CommandInfoAgent { get; }
		public TcpClient ConfirmerClient { get; }
		public NameReceiver NameReceiver { get; }
		public NameConfirmerWriter NameConfirmerWriter { get; }
		public delegate void UserDisconnectedHandler(UserOnServer disconnectedUser);
		public event UserDisconnectedHandler UserDisconnectedEvent;
		public delegate string BaseNameHandler();

		public delegate void NameChangedHandler(string pastName, string newName);
		public event NameChangedHandler NameChangedEvent;

		public OnlineState State => OnlineInfoOnServer.State;
		private string _name;

		public string Name {
			get => _name;
			set
			{
				string pastName = _name;
				_name = value;
				Server.Server.Log("UserOnServer: NameSetted", $"{_name}", ConsoleColor.Green);
				NameChangedEvent?.Invoke(pastName, _name);
			}
		}
		private void OnlineChecker_UserDisconnectedEvent()
		{
			UserDisconnectedEvent?.Invoke(this);
		}
		public UserOnServer(TcpClient messenger, TcpClient onlineSender, TcpClient onlineChecker, TcpClient commandInfoAgentClient, TcpClient confirmerClient)
		{
			ConfirmerClient = confirmerClient;
			NameReceiver = new NameReceiver(confirmerClient);
			var nameConfirmerNetworkStream = ConfirmerClient.GetStream();
			var nameConfirmerBinaryWriter = new BinaryWriter(nameConfirmerNetworkStream);
			NameConfirmerWriter = new NameConfirmerWriter(nameConfirmerBinaryWriter);
			
			Messenger = new MessengerOnServer(messenger);
			OnlineSender = new OnlineSenderOnServer(onlineSender);
			Server.Server.Log($"{nameof(UserOnServer)}: Client connected","messenger:" + (IPEndPoint)messenger.Client.RemoteEndPoint, ConsoleColor.Yellow);
			Server.Server.Log($"{nameof(UserOnServer)}: Client connected","onlineSender:" + (IPEndPoint)onlineSender.Client.RemoteEndPoint, ConsoleColor.Yellow);
			Server.Server.Log($"{nameof(UserOnServer)}: Client connected","onlineChecker:" + (IPEndPoint)onlineChecker.Client.RemoteEndPoint, ConsoleColor.Yellow);
			Server.Server.Log($"{nameof(UserOnServer)}: Client connected","commandInfoAgentClient:" + (IPEndPoint)commandInfoAgentClient.Client.RemoteEndPoint, ConsoleColor.Yellow);
			Server.Server.Log($"{nameof(UserOnServer)}: Client connected","confirmerClient:" + (IPEndPoint)confirmerClient.Client.RemoteEndPoint, ConsoleColor.Yellow);
			OnlineChecker = new OnlineChecker(new TcpClient[] {messenger, onlineSender, onlineChecker, commandInfoAgentClient, confirmerClient });
			OnlineChecker.UserDisconnectedEvent += OnlineChecker_UserDisconnectedEvent;
			OnlineInfoOnServer = new OnlineInfoOnServer(OnlineChecker);
			CommandInfoAgent = new CommandInfoAgent(commandInfoAgentClient);
			OnlineChecker.LoopOnlineSetterAsync();
		}
		/// <returns>Name: Name1</returns>
		public override string ToString()
		{
			return $"{nameof(Name)}: {Name}";
		}

#region Equals
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			UserOnServer other = obj as UserOnServer;
			if (other == null) return false;

			return this.Equals(other);
		}
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
		public bool Equals(UserOnServer other)
		{
			if (other == null) return false;
			return this.Name.Equals(other.Name);
		}
#endregion
	}
}
