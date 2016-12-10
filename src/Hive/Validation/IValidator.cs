using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Validation;
using Hive.Meta;

namespace Hive.Validation
{
	public interface IValidator
	{
		string Name { get; }

		void ModelLoaded(IPropertyValidatorDefinition validatorDefinition);

		Task<IEnumerable<ValidationError>> Validate(IPropertyValidatorDefinition validatorDefinition, object value, CancellationToken ct);
	}
}