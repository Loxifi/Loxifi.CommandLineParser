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

				//If the current property is declared but doesn't actually accept parameters (bool switch)
				if (currentMatchedProperty != null && !propertyResolutionService.HasParameters(currentMatchedProperty))
				{
					//then dump it to the bag and clear so we can pick up the next property
					matchedPropertyCollection.Add(currentMatchedProperty, string.Empty);
					currentMatchedProperty = null;
				}

				if(currentMatchedProperty is not null)
				{
					//If we have a matched property then this is a parameter
					matchedPropertyCollection.Add(currentMatchedProperty, thisArg);
					currentMatchedProperty = null;
					continue;
				}

				//If we're not currently within the parameters of a property
				//We need to figure out what property we should be
				if (currentMatchedProperty is null)
				{
					//Named properties take precedence
					if (propertyResolutionService.TryGet(thisArg, out currentMatchedProperty))
					{
						continue;
					}

					//Then indexed properties. check the front first
					if (propertyResolutionService.TryGet(unmatchedIndex, out PropertyInfo positionalProperty))
					{
						unmatchedIndex++;
						//An indexed property is its own value
						matchedPropertyCollection.Add(positionalProperty, thisArg);
						continue;
					}

					//Then the back
					if (propertyResolutionService.TryGet(- 1 - argsList.Count, out PropertyInfo positionalPropertyRear))
					{
						//Dont increment because we shouldn't be mixing at this point
						//An indexed property is its own value
						matchedPropertyCollection.Add(positionalPropertyRear, thisArg);
						continue;
					}

					//If there was no match, then something is wrong
					throw new UnmatchedParameterException();
				}
			}

			return matchedPropertyCollection;
		}
	}
}