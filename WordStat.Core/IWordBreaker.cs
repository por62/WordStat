using System.Collections.Generic;

namespace WordStat.Core
{
	public interface IWordBreaker
	{
		int MinWordLength { get; set; }
		IEnumerable<string> GetWords(string phrase);
	}
}
