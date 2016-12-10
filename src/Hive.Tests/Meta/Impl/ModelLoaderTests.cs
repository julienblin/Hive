using System;
using System.Collections.Generic;
using FluentAssertions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Meta.Impl;
using Hive.ValueTypes;
using Xunit;

namespace Hive.Tests.Meta.Impl
{
	public class ModelLoaderTests
	{
		[Theory]
		[MemberData(nameof(LoadModelOkData))]
		public void LoadModelOk(PropertyBag modelData, Action<IModel> asserts)
		{
			var modelLoader = new ModelLoader(new ValueTypeFactory(new IValueType[] { new StringValueType(), new GuidValueType(), new ArrayValueType() }));

			var model = modelLoader.Load(modelData);

			asserts(model);
		}

		public static IEnumerable<object[]> LoadModelOkData()
		{
			yield return new object[]
			{
				new PropertyBag
				{
					["name"] = "TestModel",
					["version"] = "1.2.3"
				},
				new Action<IModel>(model =>
				{
					model.Name.Should().Be("TestModel");
					model.Version.Should().Be(new SemVer(1, 2, 3));
				})
			};

			yield return new object[]
			{
				new PropertyBag
				{
					["name"] = "TestModel",
					["version"] = "1.2.3",
					["entities"] = new[]
					{
						new PropertyBag
						{
							["singlename"] = "foo",
							["pluralname"] = "foos",
							["type"] = "masterdata"
						},
						new PropertyBag
						{
							["singlename"] = "bar",
							["pluralname"] = "bars",
							["type"] = "masterdata"
						}
					}
				},
				new Action<IModel>(model =>
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
				new PropertyBag
				{
					["name"] = "TestModel",
					["version"] = "1.2.3",
					["entities"] = new[]
					{
						new PropertyBag
						{
							["singlename"] = "email",
							["pluralname"] = "emails",
							["type"] = "none",
							["properties"] = new[]
							{
								new PropertyBag
								{
									["name"] = "type",
									["type"] = "string"
								},
								new PropertyBag
								{
									["name"] = "email",
									["type"] = "string"
								}
							}
						},
						new PropertyBag
						{
							["singlename"] = "foo",
							["pluralname"] = "foos",
							["type"] = "masterdata",
							["properties"] = new[]
							{
								new PropertyBag
								{
									["name"] = "emails",
									["type"] = "array",
									["items"] = "email"
								}
							}
						}
					}
				},
				new Action<IModel>(model =>
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
				})
			};
		}
	}
}