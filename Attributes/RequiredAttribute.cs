using Loxifi.Exceptions;
using System.Reflection;

namespace Loxifi.Attributes
{
	public class RequiredAttribute : ValidationAttribute
	{
		public override void Ensure(PropertyInfo thisPropertyInfo, object property)
		{
			if (property is null)
			{
				throw new PropertyValidationException(thisPropertyInfo, "A required property was not set");
			}

			//Only check default if not a reference type
			if (!thisPropertyInfo.PropertyType.IsClass)
			{
				object d = Activator.CreateInstance(property.GetType());

				if (object.Equals(d, property))
				{
					throw new PropertyValidationException(thisPropertyInfo, "A required property was not set");
				}
			}
		}
	}
}