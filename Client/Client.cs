using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Commands;
using Ar6Library.Helpers;
using Ar6Library.Onlines;

namespace Ar6Library.Client
{
	public class Client
	{
		private Connector _connector;
		public OnlineReceiverOnClient OnlineReceiver { get; private set; }
		public MessengerOnClient Messenger { get; private set; }
		public CommandInfoAgent CommandInfoAgent { get; private set; }
		public OnlineChecker OnlineChecker { get; private set; }

		private BinaryWriter _WNameGetter;
		private BinaryReader _RNameGetter;
		public string Name { get; private set; }
		/// <exception cref="InvalidOperationException"></exception>
		public bool SetName(string name)
		{
			if (_WNameGetter == null || !_WNameGetter.BaseStream.CanWrite)
				throw new InvalidOperationException("Произошла ошибка или вы не вызвали метод Connect");
			_WNameGetter.Write(name);
			_WNameGetter.Flush();

			if (_RNameGetter.ReadBoolean())
			{
				Name = name;
				return true;
			}

			return false;
		}
		public Client()
		{
			_connector = new Connector();
		}
		public void Connect(IPAddress serverAddress, int confirmerPort, int isOnlinePort, int messengerPort, int onlineReceiverPort, int commandInfoAgentPort)
		{
			if (OnlineChecker?.Connected ?? false)
				return;
			
			_connector.Connect(serverAddress, confirmerPort, isOnlinePort, messengerPort, onlineReceiverPort, commandInfoAgentPort);
			OnlineChecker = new OnlineChecker(new TcpClient[]{
				_connector.ConfirmerClient, _connector.IsOnlineSenderClient,
				_connector.MessengerClient, _connector.OnlineReceiverClient,
				_connector.CommandInfoAgentClient}, TimeSpan.FromSeconds(1));
			
			_WNameGetter = new BinaryWriter(_connector.ConfirmerClient.GetStream());
			_RNameGetter = new BinaryReader(_connector.ConfirmerClient.GetStream());

			Messenger = new MessengerOnClient(_connector.MessengerClient);
			OnlineReceiver = new OnlineReceiverOnClient(_connector.OnlineReceiverClient);
			CommandInfoAgent = new CommandInfoAgent(_connector.CommandInfoAgentClient);
			OnlineChecker.LoopOnlineSetterAsync();
		}
		public void Connect(IpPortsHolder ipPorts)
		{
			Connect(ipPorts.IPAddress,
				ipPorts.ConfirmerPort,
				ipPorts.IsOnlinePort,
				ipPorts.MessengerPort,
				ipPorts.OnlineReceiverPort,
				ipPorts.CommandInfoAgentPort);
		}
		public void CloseConnection()
		{
			if (!OnlineChecker?.IsConnected() ?? true)
				return;

			_connector.ConfirmerClient.Close();
			_connector.IsOnlineSenderClient.Close();
			_connector.OnlineReceiverClient.Close();
			_connector.MessengerClient.Close();
			_connector.CommandInfoAgentClient.Close();
		}
		/// <summary>
		///  Set and Return
		/// </summary>
		/// <returns></returns>
		public string ReceiveBaseName()
		{
			return Name = _RNameGetter.ReadString();
		}
	}
}
