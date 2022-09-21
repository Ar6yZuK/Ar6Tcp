using System;
using System.Linq;
using System.Text;

namespace Ar6Library.Commands
{
	sealed public class CommandInfo : IEquatable<CommandInfo>
	{
		public delegate void MethodToInvoke();
		public string NameWhoseMethod { get; }
		public string MethodName { get; }
		public MethodToInvoke Method;
		public CommandInfo(string nameWhoseMethod, string methodName, MethodToInvoke method)
		{
			NameWhoseMethod = nameWhoseMethod;
			MethodName = methodName;
			Method = method;
		}
		/// <summary>
		/// Переопределено
		/// </summary>
		/// <returns>Name1:MethodName</returns>
		public override string ToString()
		{
			return $"{NameWhoseMethod}:{MethodName}";
		}
		public bool Equals(CommandInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return NameWhoseMethod == other.NameWhoseMethod && MethodName == other.MethodName;
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CommandInfo)obj);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (NameWhoseMethod != null ? NameWhoseMethod.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}