using Loxifi.Exceptions;
using System.Reflection;

namespace Loxifi.Attributes
{
	public class NotNullOrWhiteSpaceAttribute : ValidationAttribute
	{
		public override void Ensure(PropertyInfo thisPropertyInfo, object property)
		{
			if(property is null)
			{
				throw new PropertyValidationException(thisPropertyInfo, "Property can not be null");
			}

			if(property is not string s)
			{
				throw new PropertyValidationException(thisPropertyInfo, "Invalid property type for attribute");
			}

			if(string.IsNullOrWhiteSpace(s))
			{
				throw new PropertyValidationException(thisPropertyInfo, "Property can not be whitespace");
			}
		}
	}
}