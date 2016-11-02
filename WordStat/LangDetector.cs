using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	public class LangDetector : ILanguageDetector
	{
		private static readonly LangCharacrterRange[] _CharRanges = new[]
		{
			new LangCharacrterRange {Start = (char)0x0061, End = (char)0x007A, Language = Lang.English}, //latin lowercase
			new LangCharacrterRange {Start = (char)0x0430, End = (char)0x044F, Language = Lang.Russian},//cyrilic lowercase
			new LangCharacrterRange {Start = (char)0x0451, End = (char)0x0451, Language = Lang.Russian}, //ё
		};

		public Lang? GetLanguage(string word)
		{
			if (word == null) return null;

			var langGroup = word
				.Take(3)
				.SelectMany(c => _CharRanges
					.Where(r => r.Start <= c && c <= r.End)
					.Select(r => (Lang?) r.Language)
					.Where(l => l != null)
					.Select(l => l.Value))
				.GroupBy(l => l)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			return langGroup == null ? (Lang?)null : langGroup.Key;
		}
	}
}
