using System.Reflection;

namespace Loxifi
{
	internal class MatchedProperty
	{
		/// <summary>
		/// The propertyInfo that was identified to match
		/// </summary>
		public PropertyInfo Property { get; set; }

		/// <summary>
		/// Cumulative values following the switch command
		/// </summary>
		public List<string> Values { get; set; } = new List<string>();
	}
}