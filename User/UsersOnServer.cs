using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ar6Library.Commands;
using Ar6Library.Onlines;

namespace Ar6Library.User
{
	sealed public class UsersOnServer
	{
		public int MaxCount { get; }
		public List<UserOnServer> Users { get; }
		public OnlineInfoWrapper Onlines { get; private set; }
		public UsersOnServer(int maxCount)
		{
			Users = new List<UserOnServer>(maxCount);
			MaxCount = maxCount;
			Onlines = new OnlineInfoWrapper(new List<string>());
		}
		/// <summary>
		/// Не добавляет клиента если уже содержится или если количество клиентов превышает <see cref="MaxCount"/>,
		/// иначе добавляет
		/// </summary>
		/// <param name="user"></param>
		public void AddClient(UserOnServer user)
		{
			if (Users.Contains(user))
				return;
			if (MaxCount <= Users.Count)
				return;
			user.UserDisconnectedEvent += UserUserDisconnectedEvent;
			Users.Add(user);
			SetOnlineInfoWrapper();
		}
		public void SetOnlineInfoWrapper()
		{
			Onlines = new OnlineInfoWrapper(Users.Select(x => x.Name).ToList());
		}
		private void UserUserDisconnectedEvent(UserOnServer disconnectedUser)
		{
			Users.Remove(disconnectedUser);
			SetOnlineInfoWrapper();
		}
		#region Getters&Setters

		/// <exception cref="InvalidOperationException"></exception>
		public UserOnServer this[string name] => GetInfo(name);
		/// <exception cref="InvalidOperationException"></exception>
		public UserOnServer GetInfo(string name)
		{
			return Users.First(x => x.Name == name);
		}
		#endregion
	}
}