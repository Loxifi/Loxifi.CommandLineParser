using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Loxifi.Attributes
{
	public abstract class ValidationAttribute : Attribute
	{
		public abstract void Ensure(PropertyInfo thisPropertyInfo, object property);
	}
}
