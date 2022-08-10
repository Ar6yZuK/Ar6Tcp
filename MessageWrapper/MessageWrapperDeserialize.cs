using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Tcp.MessageWrappers
{
	public class MessageWrapper
	{
		public string MessageType { get; set; }
		public object Message { get; set; }
		static public (MessageWrapper, object) Deserialize(string json)
		{
			var messageWrapper = JsonConvert.DeserializeObject<MessageWrapper>(json);
			object somethingToSend = JsonConvert.DeserializeObject(Convert.ToString(messageWrapper.Message), Type.GetType(messageWrapper.MessageType));
			return (messageWrapper, somethingToSend);
		}
	}
}
