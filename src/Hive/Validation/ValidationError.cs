using System.Collections.Generic;
using Hive.Foundation.Extensions;

namespace Hive.Validation
{
	public class ValidationError
	{
		public ValidationError(string target, IEnumerable<string> messages)
		{
			Target = target;
			Messages = messages;
		}

		public ValidationError(string target, string message)
			: this(target, new[] { message })
		{
		}

		public string Target { get; }

		public IEnumerable<string> Messages { get; }

		public override string ToString() => $"[{Target}]: {string.Join(", ", Messages.Safe())}";
	}
}