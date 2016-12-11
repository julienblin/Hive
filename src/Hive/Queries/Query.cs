using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Queries
{
	public abstract class Query : IQuery
	{
		protected Query(IEntityDefinition entityDefinition)
		{
			EntityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
			Criterions = new List<ICriterion>();
			Orders = new List<Order>();
		}

		public virtual IQuery Add(ICriterion criterion)
		{
			if (criterion != null)
			{
				var propertyDefinition = EntityDefinition.Properties.SafeGet(criterion.PropertyName);
				if (propertyDefinition == null)
					throw new QueryException(this, $"Unable to filter by {criterion.PropertyName} because {criterion.PropertyName} is not a valid property of {EntityDefinition}.");
				criterion.PropertyName = propertyDefinition.Name;
				Criterions.Add(criterion);
			}
			return this;
		}

		public virtual IQuery AddOrder(Order order)
		{
			if (order != null)
				Orders.Add(order);
			return this;
		}

		public bool IsIdQuery => (Criterions.Count == 1) && Criterions[0].IsIdCriterion;

		public abstract Task<IEnumerable> ToEnumerable(CancellationToken ct);

		public abstract Task<object> UniqueResult(CancellationToken ct);

		protected IEntityDefinition EntityDefinition { get; }

		protected IList<ICriterion> Criterions { get; }

		protected IList<Order> Orders { get; }
	}
}