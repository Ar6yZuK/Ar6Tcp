using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Tcp.SomethingSend
{
	public class DataForSend : SomethingToSend
	{
		public string Data;
		public int Length;
		public string EncodingName;
		public int ID;
		public int SenderID;
		public string SenderName;

		public DataForSend GetDataForSendWithoutData()
		{
			DataForSend w = ((DataForSend)MemberwiseClone());
			w.Data = "";
			return w;
		}
	}
}
