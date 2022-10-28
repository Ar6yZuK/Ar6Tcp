using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Commands;

namespace Ar6Library.Client
{
	sealed public class CommandInfoAgent
	{
		public TcpClient TcpClient { get; }
		private BinaryWriter _binaryWriter;
		private BinaryReaderStringSafe _binaryReader;
		public delegate void CommandReceivedHandler(CommandInfoAgent me);
		public event CommandReceivedHandler CommandReceivedEvent;

		public delegate void CommandInvokedHandler(CommandInfo command);
		public event CommandInvokedHandler CommandInvokedEvent;
		public delegate void CommandEndedInvokingHanlder(CommandInfo command);
		public event CommandEndedInvokingHanlder CommandEndedInvokingEvent;
		/// <summary>
		/// Очищается при вызове метода <see cref="ClearMethodsOnServer"/>
		/// </summary>
		public List<CommandInfo> SentCommands;
		public ReceivedCommandsInfo ReceivedCommands;
		public CommandInfoAgent(TcpClient tcpClient)
		{
			this.TcpClient = tcpClient;
			_binaryWriter = new BinaryWriter(tcpClient.GetStream());
			_binaryReader = new BinaryReaderStringSafe(new BinaryReader(tcpClient.GetStream()));
			SentCommands = new List<CommandInfo>();
			ReceivedCommands = new ReceivedCommandsInfo();
			var threadReceiveCommandsLoop = new Thread(ReceiveCommandsLoop);
			threadReceiveCommandsLoop.IsBackground = true;
			threadReceiveCommandsLoop.Start();
		}
		public void SendMethod(CommandWrapper wrapper)
		{
			_binaryWriter.Write(wrapper.ToString());
			_binaryWriter.Flush();

			if(!SentCommands.Contains(wrapper.CommandInfo))
				SentCommands.Add(wrapper.CommandInfo);
		}
		private void ReceiveCommandsLoop()
		{
			while (true)
			{
				if (!Receive())
					return;
				Thread.Sleep(1);
			}
		}
		private bool Receive()
		{
			var receivedCommandStr = _binaryReader.ReadSafe();
			if (receivedCommandStr is null)
				return false;

			var commandWrapper = CommandWrapper.Parse(receivedCommandStr);
			Console.WriteLine($"Received command:{commandWrapper.ToString()}");
			var index = SentCommands.IndexOf(commandWrapper.CommandInfo);
			if (index == -1) // Если команда другого клиента
			{
				AddReceivedCommand(commandWrapper.CommandInfo);
				CommandReceivedEvent?.Invoke(this);
				return true;
			}
			CommandReceivedEvent?.Invoke(this);
			AddReceivedCommand(commandWrapper.CommandInfo);

			CommandInvokedEvent?.Invoke(commandWrapper.CommandInfo);
			SentCommands[index].Method?.Invoke(); // Если моя команда вызывается клиентом
			CommandEndedInvokingEvent?.Invoke(commandWrapper.CommandInfo);

			return true;
		}
		private void AddReceivedCommand(CommandInfo command)
		{
			if (ReceivedCommands.ReceivedCommands.Contains(command))
				return;
			ReceivedCommands.ReceivedCommands.Add(command);
		}
		public void InvokeReceivedMethod(CommandInfo command)
		{
			var command2 = new CommandWrapper(command, command.NameWhoseMethod);
			SendMethod(command2);
		}
		public void ClearMethodsOnServer() // Refactor this
		{
			_binaryWriter.Write("-1");
			_binaryWriter.Flush();
			SentCommands = new List<CommandInfo>();
		}
	}
}
