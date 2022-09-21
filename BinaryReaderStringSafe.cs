using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Ar6Library
{
	public class BinaryReaderStringSafe
	{
		public BinaryReader Reader;
		public BinaryReaderStringSafe(BinaryReader reader)
		{
			Reader = reader;
		}
		/// <summary>
		/// return null if Reader disposed
		/// </summary>
		/// <returns></returns>
		public string ReadSafe()
		{
			NetworkStream nameReader = (NetworkStream)Reader.BaseStream;
			try
			{
				while (nameReader.CanRead)
				{
					if (nameReader.DataAvailable)
					{
						return Reader.ReadString();
					}
					Thread.Sleep(1);
				}
			}
			catch (System.Exception)
			{
				return null;
			}
			
			return null;
		}
	}
}
