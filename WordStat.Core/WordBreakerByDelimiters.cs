using System;
using System.Collections.Generic;

namespace WordStat.Core
{
	public class WordBreakerByDelimiters : IWordBreaker
	{
		private static readonly string[] _Separators = new[] { " ", "_", ":", ";", "!", "?", "+", "-", "/", "\\", "\\t", ",-", "\"", "=", "<", ">", "'", ",", ".", "...", "…", "\r\n" };

		int IWordBreaker.MinWordLength { get; set; }
		IEnumerable<string> IWordBreaker.GetWords(string phrase)
		{
			var minLength = ( (IWordBreaker) this ).MinWordLength;

			foreach (var word in phrase.Split(_Separators, StringSplitOptions.RemoveEmptyEntries))
			{
				if (word.Length >= minLength)
				{
					yield return word.ToLower();
				}
			}
		}
	}
}
