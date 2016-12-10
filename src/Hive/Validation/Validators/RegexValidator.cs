using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hive.Exceptions;
using Hive.Foundation.Validation;
using Hive.Meta;

namespace Hive.Validation.Validators
{
	public class RegexValidator : Validator
	{
		private const string PropertyRegex = "regex";
		private const string PropertyPattern = "pattern";

		public RegexValidator()
			:base("regex")
		{
		}

		public override void ModelLoaded(IPropertyValidatorDefinition validatorDefinition)
		{
			var patternProperty = validatorDefinition.PropertyBag[PropertyPattern] as string;
			if (patternProperty == null)
				throw new ModelLoadingException(
					$"When using regex validator, the {PropertyPattern} property is mandatory (on {validatorDefinition}).");

			try
			{
				validatorDefinition.AdditionalProperties[PropertyPattern] = patternProperty;
				validatorDefinition.AdditionalProperties[PropertyRegex] = new Regex(patternProperty, RegexOptions.Compiled);
			}
			catch (ArgumentException ex)
			{
				throw new ModelLoadingException($"Error while creating regular expression: {ex.Message}", ex);
			}
		}

		protected override IEnumerable<ValidationError> ValidateSync(IPropertyValidatorDefinition validatorDefinition, object value)
		{
			if (value == null) yield break;

			var regex = (Regex) validatorDefinition.AdditionalProperties[PropertyRegex];
			if(!regex.IsMatch(value.ToString()))
				yield return CreateError(validatorDefinition, $"{validatorDefinition.PropertyDefinition.Name} does not match the pattern ({validatorDefinition.AdditionalProperties[PropertyPattern]}).");
		}
	}
}