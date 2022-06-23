using System;
using System.Collections.Generic;

namespace WordStat6
{
	public class WordBreakerByDelimiters : IWordBreaker
	{
		private static readonly string[] _Separators = new[] { " ", "_", ":", ";", "!", "?", "+", "-", "/", "\\", "\\t", ",-", "\"", "=", "<", ">", "'", ",", ".", "...", "…", "\r\n","{","}", "(", ")", "[", "]" };
		private int _MinWordLength;

		public WordBreakerByDelimiters(int minWordLength)
		{
			this._MinWordLength = minWordLength;
		}
		IEnumerable<string> IWordBreaker.GetWords(string phrase)
		{
			foreach (var word in phrase.Split(_Separators, StringSplitOptions.RemoveEmptyEntries))
			{
				if (word.Length >= _MinWordLength)
				{
					yield return word.ToLower();
				}
			}
		}
	}
}
