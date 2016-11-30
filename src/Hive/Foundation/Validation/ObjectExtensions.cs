using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hive.Foundation.Extensions;
using HiveValidationException = Hive.Foundation.Exceptions.ValidationException;

namespace Hive.Foundation.Validation
{
	public static class ObjectExtensions
	{
		public static ValidationResults TryValidate(this object @object)
		{
			var validationResults = new Collection<ValidationResult>();
			if (Validator.TryValidateObject(@object, new ValidationContext(@object), validationResults))
			{
				return new ValidationResults(null);
			}

			var validationErrors = new Dictionary<string, IList<string>>();
			foreach (var validationResult in validationResults)
			{
				if (validationResult.MemberNames.Safe().Any())
				{
					foreach (var memberName in validationResult.MemberNames)
					{
						if (!validationErrors.ContainsKey(memberName))
						{
							validationErrors[memberName] = new List<string>();
						}
						validationErrors[memberName].Add(validationResult.ErrorMessage);
					}
				}
				else
				{
					if (!validationErrors.ContainsKey(string.Empty))
					{
						validationErrors[string.Empty] = new List<string>();
					}
					validationErrors[string.Empty].Add(validationResult.ErrorMessage);
				}
			}

			return new ValidationResults(validationErrors.Select(x => new ValidationError(x.Key, x.Value)));
		}

		public static void Validate(this object @object)
		{
			var results = @object.TryValidate();
			if (!results.IsValid)
			{
				throw new HiveValidationException(results);
			}
		}
	}
}