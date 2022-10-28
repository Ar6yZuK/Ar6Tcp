using System.Collections.Generic;
namespace Ar6Library.Server
{
	public static class BaseNameUtil
	{
		public static int Id { get; private set; }
		public const string c_baseName = "BaseName";
		public static string GetBaseName(List<string> names)
		{
			var basename = c_baseName + Id++;
			while (true)
			{
				if (!names.Contains(basename))
				{
					return basename;
				}
				basename = c_baseName + Id++;
			}
		}
	}
}
