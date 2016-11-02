using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	public class WordCounterST : WordCounterBase
	{
		protected override IDictionary<string, int> Process(IEnumerable<string> words)
		{
			var stats = new Dictionary<string, int>();

			var wordsProcessed = 0;

			foreach (var word in words)
			{
				wordsProcessed++;

				stats.AddOrUpdate(
					word,
					1,
					(k,v) => v+1);
			}

			Trace.WriteLine(string.Format("exit WordCounterST; Wors Processed: {0}", wordsProcessed));

			return stats;
		}
	}
}
