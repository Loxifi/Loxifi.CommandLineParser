using Loxifi.Tests.Models;

namespace Loxifi
{
	[TestClass]
	public class ParserTests
	{
		[TestMethod]
		public void TestBool()
		{
			List<string> args = GetArgs("/BoolTest /NotNullOrWhiteSpaceTest Test /RequiredStringTest Test /RequiredIntTest 1");

			TestModel testModel = CommandLineParser.Deserialize<TestModel>(args);

			Assert.IsTrue(testModel.BoolTest);
		}

		[TestMethod]
		public void TestInt()
		{
			List<string> args = GetArgs("/NotNullOrWhiteSpaceTest Test /RequiredStringTest Test /RequiredIntTest 1");

			TestModel testModel = CommandLineParser.Deserialize<TestModel>(args);

			Assert.AreEqual(1, testModel.RequiredIntTest);
		}

		[TestMethod]
		public void TestCollection()
		{
			List<string> args = GetArgs("/NotNullOrWhiteSpaceTest Test /RequiredStringTest Test /RequiredIntTest 1 /L A /L B /L C");

			TestModel testModel = CommandLineParser.Deserialize<TestModel>(args);

			bool match = Enumerable.SequenceEqual(new[] { "A", "B", "C" }, testModel.ListStringTest);

			Assert.IsTrue(match);
		}

		[TestMethod]
		public void TestFirstPositional()
		{
			List<string> args = GetArgs("FirstProperty /NotNullOrWhiteSpaceTest Test /RequiredStringTest Test /RequiredIntTest 1 /L A /L B /L C");

			TestModel testModel = CommandLineParser.Deserialize<TestModel>(args);

			Assert.AreEqual("FirstProperty", testModel.FirstPosition);
		}

		[TestMethod]
		public void TestLastPositional()
		{
			List<string> args = GetArgs("/NotNullOrWhiteSpaceTest Test /RequiredStringTest Test /RequiredIntTest 1 /L A /L B /L C LastProperty");

			TestModel testModel = CommandLineParser.Deserialize<TestModel>(args);

			Assert.AreEqual("LastProperty", testModel.LastPosition);
		}

		private static List<string> GetArgs(params string[] strings) => strings.SelectMany(s => s.Split(' ')).Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
	}
}