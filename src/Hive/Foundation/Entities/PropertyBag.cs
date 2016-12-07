using System;
using System.Collections;
using System.Collections.Generic;
using Hive.Exceptions;
using Hive.Foundation.Entities.Converters;
using Hive.Foundation.Extensions;
using Newtonsoft.Json;

namespace Hive.Foundation.Entities
{
	[JsonConverter(typeof(PropertyBagJsonConverter))]
	public class PropertyBag : IEnumerable<KeyValuePair<string, object>>
	{
#if DEBUG
		private static readonly HashSet<Type> TypeWhitelist = new HashSet<Type>
		{
			typeof(bool),
			typeof(byte),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(long),
			typeof(string),
			typeof(Guid),
			typeof(PropertyBag)
		};
#endif

		private readonly IDictionary<string, object> _values;

		public PropertyBag()
		{
			_values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public PropertyBag(IDictionary<string, object> values)
		{
			_values = values ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public PropertyBag(PropertyBag reference)
		{
			_values = reference._values ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public object this[string key]
		{
			get { return _values.SafeGet(key); }
			set
			{
#if DEBUG
				if (value != null)
				{
					var valueType = value.GetType();

					if (valueType.IsArray)
					{
						var elementType = valueType.GetElementType();
						while (elementType.IsArray)
							elementType = elementType.GetElementType();

						if (!TypeWhitelist.Contains(elementType))
							throw new DebugException($"Unable to set value {value} for key {key} of array type {valueType.GetElementType()} on PropertyBag.");
					}
					else
					{
						if (!TypeWhitelist.Contains(valueType))
							throw new DebugException($"Unable to set value {value} for key {key} of type {valueType} on PropertyBag.");
					}
				}
#endif
				_values[key] = value;
			}
		}

		public void Clear()
		{
			_values.Clear();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}