namespace Loxifi.Attributes
{
	public class NamedParameterAttribute : Attribute
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="parameterName"></param>
		public NamedParameterAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="controlChar"></param>
		/// <param name="parameterName"></param>
		public NamedParameterAttribute(char controlChar, string parameterName)
		{
			this.ControlChar = controlChar;
			this.ParameterName = parameterName;
		}

		/// <summary>
		/// If specified, this is the character that the parameter name must start with
		/// </summary>
		public char ControlChar { get; private set; } = '\0';

		/// <summary>
		/// True if this parameter is looking for a specific character to proceed the parameter name
		/// </summary>
		public bool IsControlCharacterSpecified => ControlChar != '\0';

		/// <summary>
		/// The named parameter to look for when setting this value
		/// </summary>
		public string ParameterName { get; private set; }
	}
}