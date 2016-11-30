using System.Collections;
using System.Collections.Generic;

namespace Hive.Foundation.Validation
{
	public class ValidationError
	{
		public ValidationError(string target, IEnumerable<string> messages)
		{
			Target = target;
			Messages = messages;
		}

		public ValidationError(string target, string message)
			: this(target, new []{ message })
		{
		}

		public string Target { get; }

		public IEnumerable<string> Messages { get; }
	}
}