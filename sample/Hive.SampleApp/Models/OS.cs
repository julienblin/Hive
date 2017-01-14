using System;
using Hive.Entities;
using Hive.Meta;

namespace Hive.SampleApp.Models
{
	[Name(SingleName = "os", PluralName = "oses")]
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