using System;
using System.Collections.Generic;
using FluentAssertions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Meta.Data;
using Hive.Meta.Impl;
using Hive.Tests.Mocks;
using Hive.ValueTypes;
using Xunit;

namespace Hive.Tests.Meta.Impl
{
	public class ModelLoaderTests
	{
		[Theory]
		[MemberData(nameof(LoadModelOkData))]
		public void LoadModelOk(ModelData modelData, Action<IModel, ModelData> asserts)
		{
			var modelLoader = new ModelLoader(new ValueTypeFactory());

			var model = modelLoader.Load(modelData);

			asserts(model, modelData);
		}

		public static IEnumerable<object[]> LoadModelOkData()
		{
			yield return new object[]
			{
				new ModelData
				{
					Name = "TestModel",
					Version = "1.2.3"
				},
				new Action<IModel, ModelData>((model, modelData) =>
				{
					model.Name.Should().Be(modelData.Name);
					model.Version.Should().Be(new SemVer(1,2,3));
				})
			};

			yield return new object[]
			{
				new ModelData
				{
					Name = "TestModel",
					Version = "1.2.3",
					Entities = new[]
					{
						new EntityDefinitionData
						{
							SingleName = "foo",
							PluralName = "foos",
							Type = "masterdata"
						},
						new EntityDefinitionData
						{
							SingleName = "bar",
							PluralName = "bars",
							Type = "masterdata"
						}
					}
				},
				new Action<IModel, ModelData>((model, modelData) =>
				{
					model.EntitiesBySingleName.Should().HaveCount(2);
					model.EntitiesBySingleName.Keys.Should().Contain("foo");
					model.EntitiesByPluralName.Keys.Should().Contain("foos");
					model.EntitiesBySingleName["foo"].Should().BeSameAs(model.EntitiesByPluralName["foos"]);

					var fooEntity = model.EntitiesBySingleName["foo"];
					fooEntity.Model.Should().BeSameAs(model);
					fooEntity.SingleName.Should().Be("foo");
					fooEntity.PluralName.Should().Be("foos");
					fooEntity.EntityType.Should().Be(EntityType.MasterData);
				})
			};

			yield return new object[]
			{
				new ModelData
				{
					Name = "TestModel",
					Version = "1.2.3",
					Entities = new[]
					{
						new EntityDefinitionData
						{
							SingleName = "email",
							PluralName = "emails",
							Type = "none",
							Properties = new []
							{
								new PropertyDefinitionDataMock("type", "string"),
								new PropertyDefinitionDataMock("email", "string")
							}
						},
						new EntityDefinitionData
						{
							SingleName = "foo",
							PluralName = "foos",
							Type = "masterdata",
							Properties = new []
							{
								new PropertyDefinitionDataMock("emails", "array", new Dictionary<string, object> { { "items", "email"} })
							}
						}
					}
				},
				new Action<IModel, ModelData>((model, modelData) =>
				{
					var emailEntity = model.EntitiesBySingleName["email"];
					var typePropertyDefinition = emailEntity.Properties.SafeGet("type");
					typePropertyDefinition.Should().NotBeNull();
					typePropertyDefinition.EntityDefinition.Should().BeSameAs(emailEntity);
					typePropertyDefinition.Name.Should().Be("type");
					typePropertyDefinition.PropertyType.Should().BeOfType<StringValueType>();

					var fooEntity = model.EntitiesBySingleName["foo"];
					var emailsProperty = fooEntity.Properties.SafeGet("emails");
					emailsProperty.Should().NotBeNull();
					emailsProperty.PropertyType.Should().BeOfType<ArrayValueType>();
					emailsProperty.GetProperty<IDataType>("items").Should().Be(emailEntity);
				})
			};
		}
	}
}