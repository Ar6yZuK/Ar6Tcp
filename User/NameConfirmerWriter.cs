using System;
using System.IO;

namespace Ar6Library.User
{
	public class NameConfirmerWriter
	{
		private BinaryWriter _binaryWriter;
		public NameConfirmerWriter(BinaryWriter binaryWriter)
		{
			_binaryWriter = binaryWriter;
		}
		public void ConfirmName()
		{
			_binaryWriter.Write(true);
		}
		public void RejectName()
		{
			_binaryWriter.Write(false);
		}

		public void WriteName(string name)
		{
			_binaryWriter.Write(name);
		}
	}
}