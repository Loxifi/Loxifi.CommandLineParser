namespace Loxifi.Attributes
{
	/// <summary>
	/// Denotes that a parameter should be parsed based on its position in the input string
	/// </summary>
	public class PositionalParameterAttribute : Attribute
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="ordinal"></param>
		public PositionalParameterAttribute(int ordinal)
		{
			this.Ordinal = ordinal;
		}

		/// <summary>
		/// The index that the parameter is expected at.
		/// If negative, searches from the end of the string
		/// </summary>
		public int Ordinal { get; private set; }
	}
}