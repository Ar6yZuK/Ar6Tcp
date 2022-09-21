using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ar6Library;

namespace Ar6Library.Commands
{
	sealed public class CommandWrapper : IEquatable<CommandWrapper>
	{
		public CommandInfo CommandInfo;
		public string ToWhomSendName;
		public CommandWrapper(CommandInfo commandInfo, string toWhomSendName)
		{
			CommandInfo = commandInfo;
			ToWhomSendName = toWhomSendName;
		}
		/// <summary>
		/// Переопределено
		/// </summary>
		/// <returns>ToWhomSendName|CommandInfo.ToString()</returns>
		public override string ToString()
		{
			return $"{ToWhomSendName}|{CommandInfo.ToString()}";
		}
		public static CommandWrapper Parse(string strToParse)
		{
			var toParse = strToParse.Split('|', ':');
			var toWhomName = toParse[0];
			var nameWhoseMethod = toParse[1];
			var methodName = toParse[2];

			var commandInfo = new CommandInfo(nameWhoseMethod, methodName, null);
			return new CommandWrapper(commandInfo, toWhomName);
		}
		public bool Equals(CommandWrapper other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(CommandInfo, other.CommandInfo) && ToWhomSendName == other.ToWhomSendName;
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CommandWrapper)obj);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return ((CommandInfo != null ? CommandInfo.GetHashCode() : 0) * 397) ^ (ToWhomSendName != null ? ToWhomSendName.GetHashCode() : 0);
			}
		}
	}
}
