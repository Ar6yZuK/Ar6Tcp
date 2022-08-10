using Ar6Tcp.MessageWrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Tcp.SomethingSend
{
	public class SomethingToSend
	{
		private SomethingToSend Get(string s)
		{
			var messageWrapper = JsonConvert.DeserializeObject<MessageWrapper>(s);
			object someChild = JsonConvert.DeserializeObject(Convert.ToString(messageWrapper.Message), Type.GetType(messageWrapper.MessageType));
			if (someChild is DataForSend dfs)
			{
				return dfs;
			}
			else if(someChild is OnlineInfo oi)
			{
				return oi;
			}
			else
			{
				return null;
			}
		}
		//public object GetObjectFromMessageWrapper(string s)
		//{
		//	var messageWrapper = JsonConvert.DeserializeObject<MessageWrapper>(s);
		//	object maybeSomeChild = JsonConvert.DeserializeObject(Convert.ToString(messageWrapper.Message), Type.GetType(messageWrapper.MessageType));
		//	return maybeSomeChild;
		//}
	}
}
