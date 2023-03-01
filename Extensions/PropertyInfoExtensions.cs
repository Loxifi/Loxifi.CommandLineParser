using System.Reflection;

namespace Loxifi.Extensions
{
	internal static class PropertyInfoExtensions
	{
		public static T GetValue<T>(this PropertyInfo pi, object? value) => (T)pi.GetValue(value);
	}
}