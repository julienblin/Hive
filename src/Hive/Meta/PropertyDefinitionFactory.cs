using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Impl;
using System.Linq;

namespace Hive.Meta
{
	public static class PropertyDefinitionFactory
	{
		public static IPropertyDefinition Create(
			IEntityDefinition entityDefinition,
			IDataType dataType = null,
			PropertyBag propertyBag = null)
		{
			entityDefinition.NotNull(nameof(entityDefinition));

			var result = new PropertyDefinition
			{
				EntityDefinition = entityDefinition,
				PropertyType = dataType,
				PropertyBag = propertyBag,
				Name = propertyBag?["name"] as string,
				Description = propertyBag?["description"] as string,
				DefaultValue = propertyBag?["default"]
			};

			var validatorsData = propertyBag?["validators"] as PropertyBag[];
			if (validatorsData != null)
				result.ValidatorDefinitions = CreateValidatorDefinitions(result, validatorsData);

			return result;
		}

		private static IEnumerable<IPropertyValidatorDefinition> CreateValidatorDefinitions(IPropertyDefinition propertyDefinition, IEnumerable<PropertyBag> validatorsData)
		{
			return validatorsData
				.Select(x => 
					new PropertyValidatorDefinition
					{
						PropertyBag = x,
						PropertyDefinition = propertyDefinition,
						Message = x["message"] as string,
						Validator = propertyDefinition.EntityDefinition.Model.Factories.Validator.GetValidator(x["type"] as string)
					})
				.Where(x => x.Validator != null)
				.ToArray();
		}
	}
}