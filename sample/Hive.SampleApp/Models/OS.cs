using System;
using Hive.Entities;

namespace Hive.SampleApp.Models
{
	public class OS : IEntity<Guid>
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