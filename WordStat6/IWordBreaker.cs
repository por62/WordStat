using System.Collections.Generic;

namespace WordStat6
{
	public interface IWordBreaker
	{
		IEnumerable<string> GetWords(string phrase);
	}
}
