using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6TcpLibrary
{
	internal class Messenger
	{
		private Stream stream;
		void SendMessage(string message)
		{
			StreamWriter sw = new StreamWriter(stream);
			sw.Write(message);
		}
	}
}
