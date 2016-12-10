using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Hive.Foundation.Extensions;

namespace Hive.ValueTypes
{
	public class ValueTypeFactory : IValueTypeFactory
	{
		private readonly IImmutableDictionary<string, IValueType> _valueTypes;

		public ValueTypeFactory(IEnumerable<IValueType> valueTypes = null)
		{
			_valueTypes = valueTypes.Safe().ToImmutableDictionary(x => x.Name);
		}

		public IValueType GetValueType(string name)
		{
			return _valueTypes.SafeGet(name);
		}
	}
}