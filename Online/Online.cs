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
		public readonly IPAddress ServerAddress;
		public readonly int PortForOnlineChecker;
		public readonly int PortForOnlineSender;
		/// <summary>
		/// Get maximum online
		/// </summary>
		public int MaximumOnline => Infos.MaxCount;
		/// <summary>
		/// Get online infos
		/// </summary>
		public OnlineInfos Infos { get; }
		/// <summary>
		/// Get online count
		/// </summary>
		public int CountOnline => Infos.Infos.Count(x => x.State == OnlineState.Online);
		public Online(IPAddress serverAddress, int maximumOnline, int portForOnlineChecker, int portForOnlineSender)
		{
			PortForOnlineChecker = portForOnlineChecker;
			PortForOnlineSender = portForOnlineSender;
			ServerAddress = serverAddress;
			Infos = new OnlineInfos(maxCount: maximumOnline);
		}
		/// <summary>
		///	Add client and connect
		/// </summary>
		/// <returns>return <see cref="OnlineInfo.Id"/>. -1 if can't add</returns>
		public int AddClient()
		{
			foreach (var i in Infos.GetIds())
			{
				if (!Infos[i].OnlineChecker.Connected)
				{
					Infos[i].Connect(ServerAddress, PortForOnlineChecker, PortForOnlineSender);
					return Infos[i].Id;
				}
			}

			return -1;
		}
		public int[] GetIds()
		{
			return Infos.GetIds();
		}
		/// <exception cref="InvalidOperationException"></exception>
		public OnlineInfo GetInfo(int id)
		{
			return Infos.GetInfo(id);
		}
		/// <exception cref="InvalidOperationException"></exception>
		public OnlineInfo GetInfo(string name)
		{
			return Infos.GetInfo(name);
		}
	}
}
