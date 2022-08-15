using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ar6Library.Onlines
{
	public class OnlineInfo
	{
		[JsonIgnore] public OnlineChecker OnlineChecker;
		public int Id { get; }
		public string Name { get; private set; }
		/// <summary>
		/// If state is received from json returns static state otherwise returns state from <see cref="Onlines.OnlineChecker.Connected"/>
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public OnlineState State
		{
			get
			{
				if (StateForJson.HasValue) return StateForJson.Value;
				if (OnlineChecker?.Connected ?? false)
					return OnlineState.Online;
				return OnlineState.Offline;
			}
		}
		/// <summary>
		/// If not <see langword="null"/> set <see cref="State"/> to the value of <see cref="StateForJson"/>
		/// </summary>
		private OnlineState? StateForJson { get; }

		public OnlineInfo(int id, string name = "Default user name")
		{
			OnlineChecker = new OnlineChecker(TimeSpan.FromSeconds(1));
			Id = id;
			Name = name;
		}
		[JsonConstructor]
		public OnlineInfo(int id, string name, OnlineState? state) : this(id, name)
		{
			StateForJson = state;
		}
		/// <summary>
		/// Close all <see cref="TcpClient"/> on this <see cref="OnlineInfo"/>
		/// </summary>
		public void Close()
		{
			OnlineChecker.Close();
		}
		public void Connect(IPAddress serverAddress, int portOnlineChecker)
		{
			OnlineChecker.Connect(serverAddress, portOnlineChecker);
		}
	}
}