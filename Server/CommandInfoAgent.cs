using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Commands;
using Ar6Library.Onlines;

namespace Ar6Library.Server
{
	sealed public class CommandInfoAgent
	{
		public TcpClient TcpClient { get; }
		private NetworkStream _netStream;
		private BinaryReaderStringSafe _binaryReader;
		private BinaryWriter _binaryWriter;

		public List<CommandInfo> ReceivedCommands;

		public delegate void CommandSendHandler(string toWhomName, CommandInfo info);
		public event CommandSendHandler CommandSendEvent;
		public CommandInfoAgent(TcpClient tcpClient)
		{
			TcpClient = tcpClient;
			_netStream = tcpClient.GetStream();
			_binaryReader = new BinaryReaderStringSafe(new BinaryReader(_netStream));
			_binaryWriter = new BinaryWriter(_netStream);
			ReceivedCommands = new List<CommandInfo>();
			var threadLoop = new Thread(ReceiveCommandsLoop);
			threadLoop.IsBackground = true;
			threadLoop.Start();
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
		/// <summary>
		/// return false if Client disposed
		/// </summary>
		/// <returns></returns>
		private bool Receive()
		{
			var receivedCommandStr = _binaryReader.ReadSafe();
			if (receivedCommandStr is null)
				return false;
			if (receivedCommandStr == "-1") // Refactor THIS
			{
				ReceivedCommands = new List<CommandInfo>();
				return true;
			}

			var commandWrapper = CommandWrapper.Parse(receivedCommandStr);

			var recieviedCommandInfo = new CommandInfo(commandWrapper.CommandInfo.NameWhoseMethod, commandWrapper.CommandInfo.MethodName, null);

			if (!ReceivedCommands.Contains(recieviedCommandInfo))
			{
				ReceivedCommands.Add(recieviedCommandInfo);
			}
			CommandSendEvent?.Invoke(commandWrapper.ToWhomSendName, recieviedCommandInfo);
			return true;
		}
		public void Send(CommandWrapper info)
		{
			string result = info.ToString();
			
			_binaryWriter.Write(result);
			_binaryWriter.Flush();
		}
		public override string ToString()
		{
			return $"{nameof(_netStream.DataAvailable)}: {_netStream.DataAvailable}";
		}
	}
}