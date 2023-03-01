using Loxifi.Attributes;
using Loxifi.Extensions;
using System.Reflection;

namespace Loxifi
{
	/// <summary>
	/// Command line parameter parsing and deserialization class
	/// </summary>
	public static class CommandLineParser
	{
		/// <summary>
		/// Attempts to deserialize into the given type using the environment detected parameters
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T DeserializeCommandLine<T>() where T : class => DeserializeCommandLine<T>(Environment.GetCommandLineArgs().Skip(1));

		/// <summary>
		/// Attempts to deserialize into the given type using the provided string IEnumerable to supply the args
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static T DeserializeCommandLine<T>(IEnumerable<string> args) where T : class
		{
			T Value = (T)Activator.CreateInstance(typeof(T));

			List<string> arguments = args.ToList();

			Dictionary<char, Action<string>> propertyFuncs = new()
			{
				{
					'/',
					(s) =>
			{
				string propertyName;
				string? value = null;

				int indexOfColon = s.IndexOf(':');

				if(indexOfColon == -1)
				{
					propertyName = s;
				} else
				{
					propertyName = s[..indexOfColon];
					value = s[(indexOfColon + 1)..];
				}

				PropertyInfo pi = typeof(T).GetProperties().Single(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

				if (pi.PropertyType == typeof(bool))
				{
					pi.SetValue(Value, !(bool)pi.GetValue(Value));
				}
				else
				{
					pi.SetValue(Value, value.Convert(pi.PropertyType));
				}
			}
				}
			};

			foreach (PropertyInfo pi in typeof(T).GetProperties())
			{
				if (pi.GetCustomAttribute<CommandCharAttribute>() is CommandCharAttribute cc)
				{
					char c = cc.Value;
					Action<string> func = null;

					if (c == '/')
					{
						continue;
					}

					if (typeof(ICollection<string>).IsAssignableFrom(pi.PropertyType))
					{
						ICollection<string> collection = (ICollection<string>)pi.GetValue(Value);

						if (collection is null)
						{
							collection = (ICollection<string>)Activator.CreateInstance(pi.PropertyType);
							pi.SetValue(Value, collection);
						}

						func = collection.Add;
					}
					else
					{
						func = (s) => pi.SetValue(Value, s.Convert(pi.PropertyType));
					}

					if (func is null)
					{
						throw new NotImplementedException($"Property Type {pi.PropertyType} could not be handled");
					}
					else
					{
						propertyFuncs.Add(c, func);
					}
				}
			}

			foreach (MethodInfo mi in typeof(T).GetMethods())
			{
				if (mi.GetCustomAttribute<CommandCharAttribute>() is CommandCharAttribute cc)
				{
					char c = cc.Value;
					Action<string>? func = null;

					ParameterInfo[] parameters = mi.GetParameters();

					if (parameters.Length != 1)
					{
						throw new NotImplementedException("CommandChar methods can only recieve a single object");
					}

					func = (s) => mi.Invoke(Value, new object?[] { s.Convert(parameters.Single().ParameterType) });

					propertyFuncs.Add(c, func);
				}
			}

			while (arguments.Any())
			{
				Command arg = new(arguments.First().Trim('"'));
				arguments.RemoveAt(0);

				if (propertyFuncs.TryGetValue(arg.FirstChar, out Action<string> func))
				{
					func.Invoke(arg.Data);
				}
			}

			foreach (PropertyInfo pi in typeof(T).GetProperties())
			{
				bool requiredFailure = pi.GetCustomAttribute<RequiredAttribute>() != null;
				requiredFailure = requiredFailure && pi.GetValue(Value).IsDefaultValue();

				bool emptyFailure = pi.GetCustomAttribute<NotNullOrWhiteSpaceAttribute>() != null;
				emptyFailure = emptyFailure && string.IsNullOrWhiteSpace((string)pi.GetValue(Value));

				if (emptyFailure || requiredFailure)
				{
					string setName = "";

					if (pi.GetCustomAttribute<CommandCharAttribute>() is CommandCharAttribute cc)
					{
						setName = $"{cc.Value}Value";
					}
					else
					{
						setName = $"/{pi.Name}:Value";
					}

					throw new ArgumentException($"{pi.Name} is not set. Use the command {setName} to set it");
				}
			}

			return Value;
		}
	}
}