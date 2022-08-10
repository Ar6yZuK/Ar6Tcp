using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Tcp.MessageWrappers
{
	public class MessageWrapper<T>
	{
		public string MessageType { get { return typeof(T).FullName; } }
		public T Message { get; set; }
	}
}
