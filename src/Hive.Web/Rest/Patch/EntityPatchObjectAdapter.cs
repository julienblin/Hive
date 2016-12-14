using System;
using Hive.Entities;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Web.Exceptions;

namespace Hive.Web.Rest.Patch
{
	public class EntityPatchObjectAdapter : IObjectAdapter
	{
		public void Add(Operation operation, object objectToApplyTo)
		{
			throw new NotImplementedException();
		}

		public void Copy(Operation operation, object objectToApplyTo)
		{
			throw new NotImplementedException();
		}

		public void Move(Operation operation, object objectToApplyTo)
		{
			throw new NotImplementedException();
		}

		public void Remove(Operation operation, object objectToApplyTo)
		{
			throw new NotImplementedException();
		}

		public void Replace(Operation operation, object objectToApplyTo)
		{
			operation.NotNull(nameof(operation));
			objectToApplyTo.NotNull(nameof(objectToApplyTo)).Is<IEntity>(nameof(objectToApplyTo));
			var entity = (IEntity) objectToApplyTo;

			string remaining = null;
			var currentEntity = entity;
			var leading = operation.path.SplitFirst('/', out remaining);
			IPropertyDefinition propertyDefinition = currentEntity.Definition.Properties.SafeGet(leading);
			if (propertyDefinition == null)
				throw new BadRequestException($"Unable to find property {leading} on {currentEntity}.");

			while (remaining != null)
			{
				currentEntity = (IEntity)currentEntity[propertyDefinition.Name];
				leading = remaining.SplitFirst('/', out remaining);
				propertyDefinition = currentEntity.Definition.Properties.SafeGet(leading);
				if (propertyDefinition == null)
					throw new BadRequestException($"Unable to find property {leading} on {currentEntity}.");
			}
			currentEntity[propertyDefinition.Name] =
				propertyDefinition.PropertyType.ConvertFromPropertyBagValue(propertyDefinition, operation.value);
		}
	}
}