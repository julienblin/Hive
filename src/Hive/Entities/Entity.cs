using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Entities
{
	public class Entity : DynamicObject, IEntity
	{
		private readonly IDictionary<string, object> _propertyValues =
			new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		public Entity(IEntityDefinition definition)
		{
			Definition = definition.NotNull(nameof(definition));
		}

		public Entity(IEntityDefinition definition, object id)
			: this(definition)
		{
			Id = id;
		}

		public Entity(IEntityDefinition definition, PropertyBag propertyBag)
			: this(definition)
		{
			foreach (var property in propertyBag)
			{
				var propertyDefinition = Definition.Properties.SafeGet(property.Key);
				if (propertyDefinition != null)
					_propertyValues[property.Key] = propertyDefinition.PropertyType.ConvertFromPropertyBagValue(propertyDefinition,
						property.Value);
			}
		}

		public IEntityDefinition Definition { get; }

		public object Id
		{
			get { return this[MetaConstants.IdProperty]; }
			set { this[MetaConstants.IdProperty] = value; }
		}

		public object this[string propertyName]
		{
			get { return _propertyValues.SafeGet(propertyName); }
			set
			{
				var propertyDefinition = Definition.Properties.SafeGet(propertyName);
				if (propertyDefinition != null)
				{
					if (propertyDefinition.PropertyType.InternalNetType != value.GetType())
						throw new EntityException(
							$"Unable to set property value {value} for {propertyName} on {this} because types are incompatible (expected: {propertyDefinition.PropertyType.InternalNetType}, actual: {value.GetType()})");
					_propertyValues[propertyName] = value;
				}
			}
		}

		public PropertyBag ToPropertyBag()
		{
			var propertyBag = new PropertyBag();

			foreach (var propertyDefinition in Definition.Properties)
			{
				var propertyValue = this[propertyDefinition.Key];
				var bagValue = propertyDefinition.Value.PropertyType.ConvertToPropertyBagValue(propertyDefinition.Value,
					propertyValue);

				if (bagValue != null)
					propertyBag[propertyDefinition.Key] = bagValue;
			}

			return propertyBag;
		}

		public async Task Init(CancellationToken ct)
		{
			foreach (var propertyDefinition in Definition.Properties.Values)
				if ((propertyDefinition.DefaultValue != null) && (this[propertyDefinition.Name] == null))
					await propertyDefinition.SetDefaultValue(this, ct);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (!Definition.Properties.ContainsKey(binder.Name))
			{
				result = null;
				return false;
			}

			result = this[binder.Name];
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (!Definition.Properties.ContainsKey(binder.Name))
				return false;

			this[binder.Name] = value;
			return true;
		}

		public override string ToString() => $"{Definition} ({Id})";
	}
}