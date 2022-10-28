using Ar6Library.Commands;
using System.Collections.Generic;

namespace Ar6Library.Client
{
	sealed public class ReceivedCommandsInfo
	{
		public List<CommandInfo> ReceivedCommands;
		public ReceivedCommandsInfo()
		{
			ReceivedCommands = new List<CommandInfo>();
		}
		public CommandInfo this[string NameWhoseMethod] 
		{
			get
			{
				foreach (var command in ReceivedCommands)
					if (command.NameWhoseMethod == NameWhoseMethod)
						return command;
				return null;
			}
		}
	}
}
