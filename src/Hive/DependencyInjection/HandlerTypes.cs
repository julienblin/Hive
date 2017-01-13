using System;

namespace Hive.DependencyInjection
{
	[Flags]
	public enum HandlerTypes
	{
		Get    = 1 << 0,
		Create = 1 << 1,
		Update = 1 << 2,
		Delete = 1 << 3,
		All = ~(-1 << 4)
	}
}