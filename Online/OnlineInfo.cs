using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ar6TcpLibrary.Online
{
	public class OnlineInfo
	{
		[JsonIgnore] public OnlineChecker OnlineChecker;
		[JsonIgnore] public OnlineSender OnlineSender;
		public int Id { get; }
		public string Name { get; private set; }
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
		private OnlineState? StateForJson { get; }

		public OnlineInfo(int id, string name = "Default user name")
		{
			OnlineChecker = new OnlineChecker(TimeSpan.FromSeconds(1));
			OnlineSender = new OnlineSender(this);
			Id = id;
			Name = name;
		}
		[JsonConstructor]
		public OnlineInfo(int id, string name, OnlineState? state) : this(id, name)
		{
			StateForJson = state;
		}
		public void Close()
		{
			OnlineSender.Close();
			OnlineChecker.Close();
		}
		public void Connect(IPAddress serverAddress, int portOnlineChecker, int portOnlineSender)
		{
			OnlineChecker.Connect(serverAddress, portOnlineChecker);
			OnlineSender.Connect(serverAddress, portOnlineSender);
		}
	}
}