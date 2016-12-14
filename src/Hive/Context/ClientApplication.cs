using System;
using Microsoft.Extensions.PlatformAbstractions;

namespace Hive.Context
{
	public class ClientApplication
	{
		public static readonly ClientApplication Hive = new ClientApplication("Hive", PlatformServices.Default.Application.ApplicationVersion);

		public static readonly ClientApplication Unknown = new ClientApplication("Unknown", "Unknown");

		public ClientApplication(string name, string version)
		{
			Name = name;
			Version = version;
		}

		public string Name { get; }

		public string Version { get; }

		public override string ToString() => $"{Name}/{Version}";
	}
}