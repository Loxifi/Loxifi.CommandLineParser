using Loxifi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loxifi.Tests.Models
{
	internal class TestModel
	{
		public bool BoolTest { get; set; }

		[NotNullOrWhiteSpace]
		public string? NotNullOrWhiteSpaceTest { get; set; }

		[Required]
		public string? RequiredStringTest { get; set; }

		[Required]
		public int RequiredIntTest { get; set; }

		[NamedParameter("CustomName")]
		public string? NamedParameterTest { get; set; }

		[PositionalParameter(2)]
		public string? PositionalParameterTest { get; set; }

		[NamedParameter("L")]
		public List<string> ListStringTest { get; set; } = new List<string>();

		[PositionalParameter(0)]
		public string FirstPosition { get; set; }

		[PositionalParameter(-1)]
		public string LastPosition { get; set; }
	}
}
