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

		[TestMethod]
		public void BareTrailingSwitchIsSet()
		{
			// The switch is the final argument: it must still bind.
			SwitchModel model = CommandLineParser.Deserialize<SwitchModel>(new[] { "somePath", "-Monitor" });

			Assert.IsTrue(model.Monitor);
			Assert.IsTrue(Enumerable.SequenceEqual(new[] { "somePath" }, model.Paths));
		}

		[TestMethod]
		public void BareLeadingSwitchIsSet()
		{
			SwitchModel model = CommandLineParser.Deserialize<SwitchModel>(new[] { "-Monitor", "somePath" });

			Assert.IsTrue(model.Monitor);
			Assert.IsTrue(Enumerable.SequenceEqual(new[] { "somePath" }, model.Paths));
		}

		[TestMethod]
		public void TwoBareSwitchesAreSet()
		{
			SwitchModel model = CommandLineParser.Deserialize<SwitchModel>(new[] { "somePath", "-Monitor", "-DropFrames" });

			Assert.IsTrue(model.Monitor);
			Assert.IsTrue(model.DropFrames);
			Assert.IsTrue(Enumerable.SequenceEqual(new[] { "somePath" }, model.Paths));
		}

		[TestMethod]
		public void ExplicitSwitchValueIsHonored()
		{
			// An explicit true/false must bind to the switch (not leak into Paths) ...
			SwitchModel t = CommandLineParser.Deserialize<SwitchModel>(new[] { "somePath", "-Monitor", "true" });
			Assert.IsTrue(t.Monitor);
			Assert.IsTrue(Enumerable.SequenceEqual(new[] { "somePath" }, t.Paths));

			// ... and "-Monitor false" must actually set it to false.
			SwitchModel f = CommandLineParser.Deserialize<SwitchModel>(new[] { "somePath", "-Monitor", "false" });
			Assert.IsFalse(f.Monitor);
			Assert.IsTrue(Enumerable.SequenceEqual(new[] { "somePath" }, f.Paths));
		}

		[TestMethod]
		public void AbsentSwitchIsFalse()
		{
			SwitchModel model = CommandLineParser.Deserialize<SwitchModel>(new[] { "somePath" });

			Assert.IsFalse(model.Monitor);
			Assert.IsFalse(model.DropFrames);
		}

		private static List<string> GetArgs(params string[] strings) => strings.SelectMany(s => s.Split(' ')).Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
	}
}