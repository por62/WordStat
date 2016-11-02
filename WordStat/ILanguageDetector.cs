using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	interface ILanguageDetector
	{
		Lang? GetLanguage(string text);
	}
}
