using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Server
{
	sealed public class NameReceiver : BinaryReaderStringSafe
	{
		public NameReceiver(TcpClient tcpClient) : base(new BinaryReader(tcpClient.GetStream()))
		{
		}
	}
}
