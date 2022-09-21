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
		private BinaryWriter _WAcceptorName;
		public delegate void UserDisconnectedHandler(UserOnServer disconnectedUser);
		public event UserDisconnectedHandler UserDisconnectedEvent;
		public delegate string BaseNameHandler();

		private BaseNameHandler _getNotContainsNameEvent;
		/// <summary>
		/// allows you to subscribe only once
		/// </summary>
		public event BaseNameHandler GetNotContainsNameEvent
		{
			add
			{
				if (_getNotContainsNameEvent == null)
					_getNotContainsNameEvent += value;
			}
			remove => _getNotContainsNameEvent -= value;
		}

		public delegate bool NewNameHandler(string newName);

		private NewNameHandler _nameNotContainsEvent;
		/// <summary>
		/// allows you to subscribe only once
		/// </summary>
		public event NewNameHandler NameNotContainsEvent
		{
			add
			{
				if (_nameNotContainsEvent == null)
					_nameNotContainsEvent += value;
			}
			remove => _nameNotContainsEvent -= value;
		}
		public delegate void NameChangedHandler(string pastName, string newName);
		public event NameChangedHandler NameChangedEvent;

		public OnlineState State => OnlineInfoOnServer.State;
		private string _name;

		public string Name {
			get => _name;
			set
			{
				if (_nameNotContainsEvent?.Invoke(value) ?? false)
				{
					string pastName = _name;
					_name = value;
					_WAcceptorName.Write(true);
					NameChangedEvent?.Invoke(pastName, _name);
				}
				else
					_WAcceptorName.Write(false);
			}
		}
		public void SetBaseName()
		{
			var baseName = _getNotContainsNameEvent?.Invoke();
			_name = baseName;
		}
		void ReceiveAndSetNameLoop()
		{
			while (true)
			{
				var name = NameReceiver.Receive();
				if (name is null)
					return;
				Name = name;

				Thread.Sleep(100);
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

			_WAcceptorName = new BinaryWriter(ConfirmerClient.GetStream());
			var threadNameSetter = new Thread(ReceiveAndSetNameLoop);
			threadNameSetter.IsBackground = true;
			threadNameSetter.Start();
			Messenger = new MessengerOnServer(messenger);
			OnlineSender = new OnlineSenderOnServer(onlineSender);
			Console.WriteLine("messenger:" + (IPEndPoint)messenger.Client.RemoteEndPoint);
			Console.WriteLine("onlineSender:" + (IPEndPoint)onlineSender.Client.RemoteEndPoint);
			Console.WriteLine("onlineChecker:" + (IPEndPoint)onlineChecker.Client.RemoteEndPoint);
			Console.WriteLine("commandInfoAgentClient:" + (IPEndPoint)commandInfoAgentClient.Client.RemoteEndPoint);
			Console.WriteLine("confirmerClient:" + (IPEndPoint)confirmerClient.Client.RemoteEndPoint);
			OnlineChecker = new OnlineChecker(new TcpClient[] {messenger, onlineSender, onlineChecker, commandInfoAgentClient, confirmerClient });
			OnlineChecker.UserDisconnectedEvent += OnlineChecker_UserDisconnectedEvent;
			OnlineInfoOnServer = new OnlineInfoOnServer(OnlineChecker);
			CommandInfoAgent = new CommandInfoAgent(commandInfoAgentClient);
			OnlineChecker.LoopOnlineSetterAsync();
		}
		/// <summary>
		/// 
		/// </summary>
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
