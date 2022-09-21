using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Onlines
{
	public class OnlineInfoOnServer
	{
		public OnlineChecker OnlineChecker;
		public OnlineState State
		{
			get
			{
				if (OnlineChecker?.Connected ?? false)
					return OnlineState.Online;
				return OnlineState.Offline;
			}
		}
		public OnlineInfoOnServer(OnlineChecker checker)
		{
			OnlineChecker = checker;
		}
	}
}