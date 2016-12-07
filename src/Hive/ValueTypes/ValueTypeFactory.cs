using System.Collections.Generic;
using System.Linq;
using Hive.Foundation.Extensions;

namespace Hive.ValueTypes
{
	public class ValueTypeFactory : IValueTypeFactory
	{
		private readonly IReadOnlyDictionary<string, IValueType> _valueTypes;

		public ValueTypeFactory(IEnumerable<IValueType> valueTypes = null)
		{
			var realValueTypes = valueTypes.IsNullOrEmpty()
				? new IValueType[]
					{
						new ArrayValueType(),
						new DateTimeValueType(),
						new DateValueType(),
						new EnumValueType(),
						new GuidValueType(),
						new IntValueType(),
						new StringValueType()
					}
				: valueTypes;

			_valueTypes = realValueTypes.ToDictionary(x => x.Name);
		}

		public IValueType GetValueType(string name)
		{
			return _valueTypes.SafeGet(name);
		}
	}
}