using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ar6Tcp.SomethingSend
{
	public class OnlineInfo : SomethingToSend
	{
		static public readonly OnlineInfo noInfo = new OnlineInfo();
		public int Count { get => IDNames?.Count ?? 0; }
		public int YourID;
		public int YourName;
		public Dictionary<int, string> IDNames;
		/// <summary>
		/// Возвращает или задает Name по <paramref name="ID"/> из коллекции <see cref="IDNames"/>
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public string this[int ID]
		{
			get { return IDNames[ID]; }
			set { IDNames[ID] = value; }
		}
		public override string ToString()
		{
			StringBuilder result = new StringBuilder($"Count: {Count}\n\n");
			foreach (var itemID in IDNames.Keys)
			{
				result.Append("ID: " + itemID + "|");
			}
			foreach (var itemName in IDNames.Values)
			{
				result.AppendLine("Name: "+ itemName);
			}
			return result.ToString();
		}
	}
}
