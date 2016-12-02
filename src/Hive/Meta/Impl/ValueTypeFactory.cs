using System;
using System.Collections.Generic;
using System.Linq;
using Hive.Foundation.Extensions;
using Hive.Meta.ValueTypes;

namespace Hive.Meta.Impl
{
	public class ValueTypeFactory : IValueTypeFactory
	{
		private readonly IReadOnlyDictionary<string, IValueType> _valueTypes;

		public ValueTypeFactory()
			: this(new IValueType[]
			{
				new ArrayValueType(),
				new DateTimeValueType(),
				new DateValueType(),
				new EnumValueType(),
				new IntValueType(),
				new StringValueType()
			})
		{
		}

		public ValueTypeFactory(IEnumerable<IValueType> valueTypes)
		{
			_valueTypes = valueTypes.NotNull(nameof(valueTypes)).ToDictionary(x => x.Name);
		}

		public IValueType GetValueType(string name)
		{
			return _valueTypes.SafeGet(name);
		}
	}
}