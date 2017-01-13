using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Models;

namespace Hive.Data
{
	public interface IModelRepository
	{
		Task<IModel> Create(IModel model, CancellationToken ct);

		Task<IModel> Update(IModel model, CancellationToken ct);

		Task<bool> Delete(Type modelType, object id, CancellationToken ct);
	}
}