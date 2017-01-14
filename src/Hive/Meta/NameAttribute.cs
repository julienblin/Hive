using System;
using Hive.Foundation.Extensions;

namespace Hive.Meta
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NameAttribute : Attribute
	{
		public string SingleName { get; set; }

		public string PluralName { get; set; }
	}
}