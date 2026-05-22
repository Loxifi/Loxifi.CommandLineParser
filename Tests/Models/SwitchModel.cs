using Loxifi.Attributes;
using System.Collections.Generic;

namespace Loxifi.Tests.Models
{
	/// <summary>
	/// Mirrors a real consumer: a positional list of paths plus a few bool switches.
	/// </summary>
	internal class SwitchModel
	{
		[PositionalParameter(0)]
		public List<string> Paths { get; set; } = new List<string>();

		public bool Monitor { get; set; }

		public bool DropFrames { get; set; }
	}
}
