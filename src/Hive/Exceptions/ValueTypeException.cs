using System;
using Hive.ValueTypes;

namespace Hive.Exceptions
{
	public class ValueTypeException : HiveFatalException
	{
		public ValueTypeException(IValueType valueType, string message) : base(message)
		{
			ValueType = valueType;
		}

		public ValueTypeException(IValueType valueType, string message, Exception inner) : base(message, inner)
		{
			ValueType = valueType;
		}

		public IValueType ValueType { get; set; }
	}
}