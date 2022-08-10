using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6TcpLibrary.Online
{
	public class OnlineInfo
	{

		public OnlineChecker OnlineChecker;
		public OnlineSender OnlineSender;
		public int Id { get; }
		public string Name { get; private set; }
		public OnlineState State
		{
			get
			{
				if (OnlineChecker.Connected)
					return OnlineState.Online;
				return OnlineState.Offline;
			}
		}

		public OnlineInfo(int id, string name = "Default user name")
		{
			OnlineChecker = new OnlineChecker();
			OnlineSender = new OnlineSender(this);
			Id = id;
			Name = name;
		}
	}
	public enum OnlineState
	{
		Offline,
		Online
	}
}
