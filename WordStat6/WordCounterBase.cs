using System.Diagnostics;
using System.Text;

namespace WordStat6
{
	public abstract class WordCounterBase : IWordCounter, IDisposable
	{
		protected abstract IDictionary<string, int> Process(IEnumerable<string> words);
	
		private RWLocker _Lock = new RWLocker();
		private double _Progress = 0F;
		private IWordBreaker _WordBreaker;
		
		protected StreamReader _StreamReader;

		public WordCounterBase(IWordBreaker wordBreaker, Stream stream, Encoding streamEncoding)
		{
			_StreamReader = new StreamReader(stream, streamEncoding);
			_WordBreaker = wordBreaker;
		}	

		protected IEnumerable<string> GetWords(Action<double>? pa)
		{
			var sw = new Stopwatch();
			sw.Restart();

			while (!_StreamReader.EndOfStream)
			{
				var line = _StreamReader.ReadLine();
				if (line == null) continue;

				if (pa != null && sw.Elapsed.TotalMilliseconds % 1000 == 0)
				{
					pa(_StreamReader.BaseStream.Position / (double)_StreamReader.BaseStream.Length * 100);
				}

				foreach (var word in _WordBreaker.GetWords(line))
				{
					yield return word;
				}
			}

			if (pa != null)
			{
				pa(100);
			}

		}

		public IDictionary<string, int> Count(Action<double>? pa)
		{
			return Process(GetWords(pa));
		}
		public void Dispose()
		{
			if(_StreamReader != null) _StreamReader.Dispose();
		}
	}
}
