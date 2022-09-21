using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Server
{
	sealed public class NameReceiver
	{
		private BinaryReaderStringSafe _reader;
		public NameReceiver(TcpClient tcpClient)
		{
			_reader = new BinaryReaderStringSafe(new BinaryReader(tcpClient.GetStream()));
		}
		public string Receive()
		{
			return _reader.ReadSafe();
		}
	}
}
