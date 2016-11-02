using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	public abstract class WordCounterBase : IWordCounter, IDisposable
	{
		protected abstract IDictionary<string, int> Process(IEnumerable<string> words);
	
		private RWLocker _Lock = new RWLocker();
		private double _Progress;
		private IWordBreaker _WordBreaker;
		
		protected StreamReader _StreamReader; 
		protected IEnumerable<string> GetWords()
		{
			while (!_StreamReader.EndOfStream)
			{
				var line = _StreamReader.ReadLine();
				if (line == null) continue;

				Progress = Stream.Position / (double) Stream.Length * 100;

				foreach (var word in _WordBreaker.GetWords(line))
				{
					yield return word;
				}
			}
		}
		
		public double Progress
		{
			get { return _Lock.Get(() => _Progress); }
			private set { _Lock.Change(() => _Progress = value); }
		}
		public Stream Stream { get; set; }
		public Encoding StreamEncoding { get; set; }
		public IDictionary<string, int> Count(IWordBreaker wb)
		{
			_WordBreaker = wb;
			_StreamReader = new StreamReader(Stream, StreamEncoding);

			return Process(GetWords());
		}

		public void Dispose()
		{
			if(_StreamReader != null) _StreamReader.Dispose();
		}
	}
}
