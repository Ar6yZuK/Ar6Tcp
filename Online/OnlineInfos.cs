using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ar6TcpLibrary.Online
{
	public class OnlineInfos
	{
		public int MaxCount { get; private set; }
		public OnlineInfo[] Infos { get; }

		public OnlineInfos(int maxCount)
		{
			Infos = new OnlineInfo[maxCount];
			for (int i = 0; i < maxCount; i++)
			{
				Infos[i] = new OnlineInfo(i);
			}
			MaxCount = maxCount;
		}
		public OnlineInfos(int maxCount, OnlineInfo[] infos)
		{
			MaxCount = maxCount;
			Infos = infos;
		}
		[JsonConstructor]
        private OnlineInfos()
        {

        }

		#region Getters&Setters
		public OnlineInfo this[string name] => GetInfo(name);

		public OnlineInfo this[int id] => GetInfo(id);

		/// <exception cref="InvalidOperationException"></exception>
		public OnlineInfo GetInfo(int id)
		{
			return Infos.First(x => x.Id == id);
		}

		/// <exception cref="InvalidOperationException"></exception>
		public OnlineInfo GetInfo(string name)
		{
			return Infos.First(x => x.Name == name);
		}
		#endregion
	}
}