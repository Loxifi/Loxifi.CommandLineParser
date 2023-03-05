using Loxifi.Attributes;
using System.Reflection;

namespace Loxifi.Services
{
	internal class ParameterResolutionService<TModel> where TModel : class
	{
		private readonly Dictionary<int, PropertyInfo> _indexedProperties = new();

		private readonly Dictionary<string, PropertyInfo> _namedProperties = new(StringComparer.OrdinalIgnoreCase);

		public ParameterResolutionService()
		{
			Type modelType = typeof(TModel);
			foreach (PropertyInfo propertyInfo in modelType.GetProperties())
			{
				if (propertyInfo.GetCustomAttribute<PositionalParameterAttribute>() is PositionalParameterAttribute paa)
				{
					_indexedProperties.Add(paa.Ordinal, propertyInfo);
					continue;
				}

				List<char> controlCharacters = CharacterService.CONTROL_CHARACTERS.ToList();
				string name = propertyInfo.Name;

				if (propertyInfo.GetCustomAttribute<NamedParameterAttribute>() is NamedParameterAttribute naa)
				{

					name = naa.ParameterName;

					if (naa.IsControlCharacterSpecified)
					{
						controlCharacters = new List<char>() { naa.ControlChar };
					}
				}

				foreach (char c in controlCharacters)
				{
					_namedProperties.Add(c + name, propertyInfo);
				}
			}
		}

		public bool HasParameters(PropertyInfo pi)
		{
			if (pi.PropertyType == typeof(bool))
			{
				return false;
			}

			return true;
		}

		public bool TryGet(string name, out PropertyInfo propertyInfo) => _namedProperties.TryGetValue(name, out propertyInfo);

		public bool TryGet(int index, out PropertyInfo propertyInfo) => _indexedProperties.TryGetValue(index, out propertyInfo);
	}
}