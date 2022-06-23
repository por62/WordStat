using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WordStat6
{
	public class WordBreakerRegEx : IWordBreaker
	{
		private Regex _Regex;
		private int _MinWordLength;

		public WordBreakerRegEx(int minWordLength)
		{
			_MinWordLength = minWordLength;

			var pattern = string.Format("\\w{{{0},}}", _MinWordLength);
			_Regex = new Regex(pattern, RegexOptions.IgnoreCase);
		}
		public IEnumerable<string> GetWords(string phrase)
		{
			foreach (Match match in _Regex.Matches(phrase))
			{
				if(match.Success) yield return match.Value.ToLower();
			}
		}
	}

}
