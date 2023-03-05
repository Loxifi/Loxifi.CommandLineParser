using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Loxifi.Exceptions
{
	internal class PropertyValidationException : Exception
	{
		public PropertyInfo PropertyInfo { get; private set; }
		public PropertyValidationException(PropertyInfo propertyInfo, string message) : base(message)
		{
			PropertyInfo = propertyInfo;
		}
	}
}
