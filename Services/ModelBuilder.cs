using System.Collections;
using System.Reflection;

namespace Loxifi.Services
{
	public class ModelBuilder<TModel>
	{
		private readonly TModel _model;

		public ModelBuilder()
		{
			_model = Activator.CreateInstance<TModel>();
		}

		public TModel Build() => _model;

		public void SetProperty(PropertyInfo propertyInfo, List<string> values)
		{
			if (propertyInfo.PropertyType == typeof(bool))
			{
				//Presence implies true; an explicit "-Flag true" / "-Flag false" overrides.
				bool value = true;

				string lastExplicit = values.LastOrDefault(v => !string.IsNullOrEmpty(v));

				if (lastExplicit != null && bool.TryParse(lastExplicit, out bool parsed))
				{
					value = parsed;
				}

				propertyInfo.SetValue(_model, value);
				return;
			}

			if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
			{
				IList propertyBag = (IList)Activator.CreateInstance(propertyInfo.PropertyType);

				Type collectionType = propertyInfo.PropertyType.GetGenericArguments()[0];

				foreach (string s in values)
				{
					object? collectionPropertyValue = s.Convert(collectionType);
					_ = propertyBag.Add(collectionPropertyValue);
				}

				propertyInfo.SetValue(_model, propertyBag);

				return;
			}

			object? propertyValue = values.Single().Convert(propertyInfo.PropertyType);

			propertyInfo.SetValue(_model, propertyValue);
		}
	}
}