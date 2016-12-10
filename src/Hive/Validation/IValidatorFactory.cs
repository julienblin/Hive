namespace Hive.Validation
{
	public interface IValidatorFactory
	{
		IValidator GetValidator(string name);
	}
}