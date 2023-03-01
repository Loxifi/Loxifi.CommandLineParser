namespace Loxifi.Extensions
{
	internal static class ObjectExtensions
	{
		public static bool IsDefaultValue(this object obj)
		{
			if (obj is null)
			{
				return true;
			}

			Type t = obj.GetType();

			if (t.IsClass)
			{
				return false;
			}

			object dv = Activator.CreateInstance(t);

			return object.Equals(dv, obj);
		}
	}
}