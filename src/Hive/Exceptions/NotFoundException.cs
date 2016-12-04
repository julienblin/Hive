namespace Hive.Exceptions
{
	public class NotFoundException : HiveException
	{
		public NotFoundException(string message)
			: base(message)
		{
		}
	}
}