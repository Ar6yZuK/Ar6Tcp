using System.Collections.Generic;
using System.Linq;

namespace Ar6Library.Onlines
{
	public class OnlineInfoWrapper
	{
		public List<string> Names;
		public OnlineInfoWrapper(List<string> names)
		{
			Names = names;
		}
		public static OnlineInfoWrapper Parse(string str)
		{
			var names = str.Split('|').ToList();
			var onlineInfoParser = new OnlineInfoWrapper(names);
			return onlineInfoParser;
		}
		/// <summary>
		/// Переопределено
		/// </summary>
		/// <returns>Строка в формате Name1|Name2</returns>
		public override string ToString()
		{
			return string.Join("|", Names);
		}
	}
}