using System.Collections.Generic;
using System.Text;

namespace WordStat6
{
	public class WordCounterST : WordCounterBase
	{
		public WordCounterST(IWordBreaker wb, Stream s, Encoding enc) :base(wb,s,enc)
		{ }
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

			//Trace.WriteLine(string.Format("exit WordCounterST; Wors Processed: {0}", wordsProcessed));

			return stats;
		}
	}
}
