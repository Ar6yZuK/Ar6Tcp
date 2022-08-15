using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Library.Onlines
{
	public class Online
	{
		public readonly IPAddress ServerAddress;
		public readonly int PortForOnlineChecker;
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
		public Online(IPAddress serverAddress, int maximumOnline, int portForOnlineChecker)
		{
			PortForOnlineChecker = portForOnlineChecker;
			ServerAddress = serverAddress;

			Infos = new OnlineInfos(maxCount: maximumOnline);
		}
		public OnlineInfos GetAllOnlineState()
		{
			var tmp = Infos.Infos.Where(x=> x.State == OnlineState.Online).ToArray();
			return new OnlineInfos(tmp.Length, tmp);
		}
		/// <summary>
		///	Add client and connect
		/// </summary>
		/// <returns>return <see cref="OnlineInfo.Id"/>. -1 if can't add</returns>
		public int AddClient()
		{
			foreach (var id in Infos.GetIds())
			{
				if (!Infos[id].OnlineChecker.Connected)
				{
					Infos[id].Connect(ServerAddress, PortForOnlineChecker);
					return Infos[id].Id;
				}
			}

			return -1;
		}

#region Getters
		public int[] GetIds()
		{
			return Infos.GetIds();
		}
		public string[] GetNames()
		{
			return Infos.GetNames();
		}
		/// <exception cref="InvalidOperationException"></exception>
		public OnlineInfo GetInfo(int id)
		{
			return Infos.GetInfo(id);
		}
		///// <exception cref="InvalidOperationException"></exception>
		//public OnlineInfo GetInfo(string name)
		//{
		//	return Infos.GetInfo(name);
		//}
#endregion
	}
}
