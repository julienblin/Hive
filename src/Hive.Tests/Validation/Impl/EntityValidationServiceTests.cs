using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hive.Entities;
using Hive.Foundation.Validation;
using Hive.Meta;
using Hive.Meta.Data;
using Hive.Meta.Impl;
using Hive.Meta.ValueTypes;
using Hive.Tests.Mocks;
using Hive.Validation.Impl;
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
			var modelData = new ModelData
			{
				Name = "UnitTests",
				Version = "1.0.0",
				Entities = new []
				{
					new EntityDefinitionData
					{
						SingleName = "None",
						PluralName = "Nones",
						Type = "none"
					},

					new EntityDefinitionData
					{
						SingleName = "Ref",
						PluralName = "Refs",
						Type = "reference"
					}
				}
			};

			var modelLoader = new ModelLoader(new ValueTypeFactory());
			var model = modelLoader.Load(modelData);

			yield return new object[]
			{
				new Entity(model.EntitiesBySingleName["None"]),
				new Action<ValidationResults>(r => r.IsValid.Should().BeTrue())
			};

			yield return new object[]
			{
				new Entity(model.EntitiesBySingleName["Ref"]),
				new Action<ValidationResults>(r => r.Errors.First().Target.Should().Be(nameof(IEntity.Id)))
			};

			yield return new object[]
			{
				new Entity(model.EntitiesBySingleName["Ref"], string.Empty),
				new Action<ValidationResults>(r => r.Errors.First().Target.Should().Be(nameof(IEntity.Id)))
			};

			yield return new object[]
			{
				new Entity(model.EntitiesBySingleName["Ref"], Guid.NewGuid()),
				new Action<ValidationResults>(r => r.IsValid.Should().BeTrue())
			};
		}
	}
}