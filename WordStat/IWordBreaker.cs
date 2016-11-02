using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	public interface IWordBreaker
	{
		int MinWordLength { get; set; }
		IEnumerable<string> GetWords(string phrase);
	}
}
