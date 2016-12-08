using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hive.Entities;
using Hive.Entities.Impl;
using Hive.Foundation.Entities;
using Hive.Foundation.Validation;
using Hive.Meta.Impl;
using Hive.Validation.Impl;
using Hive.ValueTypes;
using Xunit;

namespace Hive.Tests.Validation.Impl
{
	public class EntityValidationServiceTests
	{
		[Theory]
		[MemberData(nameof(TryValidateData))]
		public async Task TryValidate(IEntity entity, Action<ValidationResults> asserts)
		{
			var validationService = new EntityValidationService();
			var results = await validationService.TryValidate(entity, CancellationToken.None);
			asserts(results);
		}

		public static IEnumerable<object[]> TryValidateData()
		{
			var modelData = new PropertyBag
			{
				["name"] = "UnitTests",
				["version"] = "1.0.0",
				["entities"] = new[]
				{
					new PropertyBag
					{
						["singlename"] = "None",
						["pluralname"] = "Nones",
						["type"] = "none"
					},
					new PropertyBag
					{
						["singlename"] = "Ref",
						["pluralname"] = "Refs",
						["type"] = "reference"
					},
					new PropertyBag
					{
						["singlename"] = "RefIdString",
						["pluralname"] = "RefIdStrings",
						["type"] = "reference",
						["properties"] = new[]
						{
							new PropertyBag
							{
								["name"] = "id",
								["type"] = "string"
							}
						}
					}
				}
			};

			var modelLoader = new ModelLoader(new ValueTypeFactory());
			var model = modelLoader.Load(modelData);
			var entityFactory = new EntityFactory();

			yield return new object[]
			{
				entityFactory.Hydrate(model.EntitiesBySingleName["None"], new PropertyBag()),
				new Action<ValidationResults>(r => r.IsValid.Should().BeTrue())
			};

			yield return new object[]
			{
				entityFactory.Hydrate(model.EntitiesBySingleName["Ref"], new PropertyBag()),
				new Action<ValidationResults>(r => r.Errors.First().Target.Should().Be(nameof(IEntity.Id)))
			};

			yield return new object[]
			{
				entityFactory.Hydrate(model.EntitiesBySingleName["RefIdString"], new PropertyBag { ["Id"] = string.Empty }),
				new Action<ValidationResults>(r => r.Errors.First().Target.Should().Be(nameof(IEntity.Id)))
			};

			yield return new object[]
			{
				entityFactory.Hydrate(model.EntitiesBySingleName["RefIdString"], new PropertyBag { ["Id"] = Guid.NewGuid() }),
				new Action<ValidationResults>(r => r.IsValid.Should().BeTrue())
			};
		}
	}
}