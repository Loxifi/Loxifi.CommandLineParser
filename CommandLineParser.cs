using Loxifi.Attributes;
using Loxifi.Exceptions;
using Loxifi.Services;
using System.Reflection;

namespace Loxifi
{
	public static class CommandLineParser
	{
		public static TModel Deserialize<TModel>() where TModel : class => Deserialize<TModel>(System.Environment.GetCommandLineArgs().Skip(1));

		public static TModel Deserialize<TModel>(IEnumerable<string> args) where TModel : class
		{
			MatchedPropertyCollection matchedPropertyCollection = BuildMatchedPropertyCollection<TModel>(args);

			ModelBuilder<TModel> builder = new();

			foreach (MatchedProperty property in matchedPropertyCollection)
			{
				builder.SetProperty(property.Property, property.Values);
			}

			TModel toReturn = builder.Build();

			Ensure<TModel>(toReturn);

			return toReturn;
		}

		private static void Ensure<TModel>(TModel model)
		{
			if(model is null)
			{
				throw new NullReferenceException();
			}

			foreach(PropertyInfo pi in typeof(TModel).GetProperties())
			{
				if (pi.GetCustomAttribute<ValidationAttribute>() is ValidationAttribute va) 
				{
					va.Ensure(pi, pi.GetValue(model));
				}
			}
		}

		/// <summary>
		/// Figure out what goes where
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="args"></param>
		/// <returns></returns>
		/// <exception cref="UnmatchedParameterException"></exception>
		private static MatchedPropertyCollection BuildMatchedPropertyCollection<TModel>(IEnumerable<string> args) where TModel : class
		{
			//For figuring out what goes to where
			ParameterResolutionService<TModel> propertyResolutionService = new();

			//For holding what we figured out goes to where
			MatchedPropertyCollection matchedPropertyCollection = new();

			List<string> argsList = args.ToList();

			//Bump this up by one every time we find a property without a name
			//and use it to check our indexed property list to see if its positional
			int unmatchedIndex = 0;

			//since we're looping, we need to keep track of whether or not the last string in the
			//collection represented a name, or a parameter. If this has a value then we know
			//the current string in the loop is actually a parameter
			PropertyInfo? currentMatchedProperty = null;

			while (argsList.Any())
			{
				//Dequeue
				string thisArg = argsList.First().Trim();
				argsList.RemoveAt(0);

				//If we're waiting on the value for a previously matched parameterized
				//property, this argument is that value
				if (currentMatchedProperty is not null)
				{
					matchedPropertyCollection.Add(currentMatchedProperty, thisArg);
					currentMatchedProperty = null;
					continue;
				}

				//Named properties take precedence
				if (propertyResolutionService.TryGet(thisArg, out PropertyInfo namedProperty))
				{
					if (propertyResolutionService.HasParameters(namedProperty))
					{
						//The next argument is this property's value
						currentMatchedProperty = namedProperty;
					}
					else
					{
						//A bool switch is resolved immediately so it can never be lost (e.g.
						//when it's the final argument) and so its value can't bleed into
						//positional resolution. Presence implies true, but an explicit
						//"-Flag true" / "-Flag false" immediately following is honored.
						string value = string.Empty;

						if (argsList.Any() && bool.TryParse(argsList.First().Trim(), out _))
						{
							value = argsList.First().Trim();
							argsList.RemoveAt(0);
						}

						matchedPropertyCollection.Add(namedProperty, value);
					}

					continue;
				}

				//Rear positionals are checked first: an argument that lines up with a
				//rear slot (e.g. [PositionalParameter(-1)]) must claim it before a still
				//unfilled front positional greedily takes it. The index is the argument's
				//distance from the end, so the final argument is -1, the previous -2, etc.
				if (propertyResolutionService.TryGet(-1 - argsList.Count, out PropertyInfo positionalPropertyRear))
				{
					//An indexed property is its own value. Front and rear positionals are
					//counted independently, so don't touch unmatchedIndex here.
					matchedPropertyCollection.Add(positionalPropertyRear, thisArg);
					continue;
				}

				//Then the front. A List<> positional is greedy: it keeps the same index so
				//it can accumulate every remaining front argument (the rest going to rear
				//slots, which were already claimed above).
				if (propertyResolutionService.TryGet(unmatchedIndex, out PropertyInfo positionalProperty))
				{
					if (!propertyResolutionService.IsCollection(positionalProperty))
					{
						unmatchedIndex++;
					}

					//An indexed property is its own value
					matchedPropertyCollection.Add(positionalProperty, thisArg);
					continue;
				}

				//If there was no match, then something is wrong
				throw new UnmatchedParameterException();
			}

			return matchedPropertyCollection;
		}
	}
}