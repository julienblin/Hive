using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Hive.SampleApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseEnvironment(EnvironmentName.Development)
				.UseKestrel(options => options.AddServerHeader = false)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}