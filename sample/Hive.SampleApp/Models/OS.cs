using System;
using Hive.Models;

namespace Hive.SampleApp.Models
{
	public class OS : IModel<Guid>
	{
		public Guid Id { get; set; }

		public OSKind Kind { get; set; }

		public string Version { get; set; }

		public string Nickname { get; set; }
	}

	public enum OSKind
	{
		iOS,
		Android,
		Windows
	}
}