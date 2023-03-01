namespace Loxifi.Attributes
{
	/// <summary>
	/// Denotes that a property should be set using the provided value when the parameter matches the
	/// /CValue format. Otherwise properties are deserialized using the /Name:Value pattern
	/// appears on the method
	/// </summary>
	public class CommandCharAttribute : Attribute
	{
		public CommandCharAttribute(char c)
		{
			Value = c;
		}

		public char Value { get; private set; }
	}
}