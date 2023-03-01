namespace Loxifi
{
	internal class Command
	{
		private readonly string _arg;

		public Command(string arg)
		{
			_arg = arg;
		}

		public string Data => _arg[1..].Trim('"').Trim();

		public char FirstChar => _arg[0];
	}
}