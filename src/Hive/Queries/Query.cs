using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.ValueTypes;

namespace Hive.Queries
{
	public abstract class Query : IQuery
	{
		protected Query(IEntityDefinition entityDefinition)
		{
			EntityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
			Criterions = new List<ICriterion>();
			Orders = new List<Order>();
			SubQueries = new Dictionary<string, IQuery>(StringComparer.OrdinalIgnoreCase);
		}

		public virtual IQuery Add(ICriterion criterion)
		{
			if (criterion != null)
			{
				if(!CheckValidCriterion(EntityDefinition, criterion.PropertyName))
					throw new QueryException(this, $"Unable to filter by {criterion.PropertyName} because {criterion.PropertyName} is not a valid property of {EntityDefinition} or it includes a relation.");

				criterion.PropertyName = criterion.PropertyName;
				Criterions.Add(criterion);
			}
			return this;
		}

		private bool CheckValidCriterion(IEntityDefinition entityDefinition, string propertyName)
		{
			string remaining = null;
			var leading = propertyName.SplitFirst('.', out remaining);
			var propertyDefinition = entityDefinition.Properties.SafeGet(leading);
			if ((propertyDefinition == null)
			 || (propertyDefinition.PropertyType.IsRelation && !remaining.SafeOrdinalEquals(MetaConstants.IdProperty)))
			{
				return false;
			}

			if (remaining == null) return true;

			var targetDataType = propertyDefinition.PropertyType;
			var indirectTargetEntityDefinition =
				targetDataType.GetTargetValueType(propertyDefinition);
			IEntityDefinition targetEntityDefinition;
			if (indirectTargetEntityDefinition != null)
			{
				targetEntityDefinition = indirectTargetEntityDefinition as IEntityDefinition;
			}
			else
			{
				targetEntityDefinition = targetDataType as IEntityDefinition;
			}
			return targetEntityDefinition != null && CheckValidCriterion(targetEntityDefinition, remaining);
		}

		public virtual IQuery GetOrCreateSubQuery(string propertyName)
		{
			propertyName.NotNullOrEmpty(nameof(propertyName));

			if (SubQueries.ContainsKey(propertyName))
				return SubQueries[propertyName];

			var propertyDefinition = EntityDefinition.Properties.SafeGet(propertyName);
			if (propertyDefinition == null)
				throw new QueryException(this, $"Unable to create sub-query for property {propertyName} because it is not a valid property of {EntityDefinition}.");

			if(!propertyDefinition.PropertyType.IsRelation)
				throw new QueryException(this, $"Unable to create sub-query for property {propertyName} because it is not a relation property of {EntityDefinition}.");

			var subquery = InternalCreateSubQuery(propertyDefinition.PropertyType.GetTargetValueType(propertyDefinition) as IEntityDefinition);
			SubQueries[propertyDefinition.Name] = subquery;

			return subquery;
		}

		protected abstract IQuery InternalCreateSubQuery(IEntityDefinition entityDefinition);

		public virtual IQuery AddOrder(Order order)
		{
			if (order != null)
				Orders.Add(order);
			return this;
		}

		public bool IsIdQuery => (Criterions.Count == 1) && Criterions[0].IsIdCriterion && !SubQueries.Any();

		public abstract Task<IEnumerable> ToEnumerable(CancellationToken ct);

		public abstract Task<object> UniqueResult(CancellationToken ct);

		public IEntityDefinition EntityDefinition { get; }

		protected IDictionary<string, IQuery> SubQueries { get; }

		protected IList<ICriterion> Criterions { get; }

		protected IList<Order> Orders { get; }
	}
}