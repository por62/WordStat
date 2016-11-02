using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordStat
{
	public class WordBreakerRegEx : IWordBreaker
	{
		private Regex _Regex;
		private int _MinWordLength;
		public int MinWordLength
		{
			get { return _MinWordLength; }
			set
			{
				_MinWordLength = value;

				var pattern = string.Format("\\w{{{0},}}", _MinWordLength);
				_Regex = new Regex(pattern, RegexOptions.IgnoreCase);
			}
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
