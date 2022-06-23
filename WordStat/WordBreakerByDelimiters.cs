using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	public class WordBreakerByDelimiters : IWordBreaker
	{
		private static readonly string[] _Separators = new[] { " ", "_", ":", ";", "!", "?", "+", "-", "/", "\\", "\\t", ",-", "\"", "=", "<", ">", "'", ",", ".", "...", "…", "\r\n", "{", "}", "(", ")", "[", "]" };

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
