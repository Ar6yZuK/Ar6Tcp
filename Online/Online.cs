using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ar6TcpLibrary.Online
{
	public class Online
	{
		public readonly int PortOnlineChecker;
		public readonly int PortOnlineSender;
		public int MaximumOnline => Infos.MaxCount;
		public OnlineInfos Infos { get; }
		public int Count
		{
			get
			{
				return 0;
			}
		}
		public readonly IPAddress ServerAddress;
		public Online(IPAddress serverAddress, int maximumOnline, int portOnlineChecker, int portOnlineSender)
		{
			PortOnlineChecker = portOnlineChecker;
            PortOnlineSender = portOnlineSender;
			ServerAddress = serverAddress;
			Infos = new OnlineInfos(maxCount: maximumOnline);
		}

		/// <summary>
		///	Add client and connect
		/// </summary>
		/// <returns>return <see cref="OnlineInfo.Id"/>. -1 if can't add</returns>
		public int AddClient()
		{
			for (int i = 0; i < Infos.MaxCount; i++)
			{
				if(!Infos[i].OnlineChecker.Connected)
				{
					Infos[i].OnlineChecker.Connect(ServerAddress, PortOnlineChecker);
					Infos[i].OnlineSender.Connect();
					return Infos[i].Id;
				}
			}

			return -1;
		}
	}
}
