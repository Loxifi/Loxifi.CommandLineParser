using System.Collections;
using System.Reflection;

namespace Loxifi
{
	internal class MatchedPropertyCollection : IEnumerable<MatchedProperty>
	{
		private readonly Dictionary<PropertyInfo, MatchedProperty> _properties = new();

		public void Add(PropertyInfo property, string value)
		{
			if (!_properties.TryGetValue(property, out MatchedProperty matchedProperty))
			{
				matchedProperty = new MatchedProperty()
				{
					Property = property
				};

				_properties.Add(property, matchedProperty);
			}

			matchedProperty.Values.Add(value);
		}

		public IEnumerator<MatchedProperty> GetEnumerator() => _properties.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _properties.Values.GetEnumerator();
	}
}