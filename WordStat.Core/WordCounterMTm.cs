using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordStat.Core
{
	public class WordCounterMTm : WordCounterBase
	{
		[ThreadStatic]
		private static int _TotalWords;

		private Locker _InputLock = new Locker();
		private Locker _OutputLock = new Locker();

		private Dictionary<string, int> _Result = new  Dictionary<string, int>();

		private int _BlockSize;
		private int _WorkersCount;
		
		public WordCounterMTm(int threadsCount, int blockSize)
		{
			_WorkersCount = threadsCount;
			_BlockSize = blockSize;
		}

		protected override IDictionary<string, int> Process(IEnumerable<string> words)
		{
			List<Task> tasks = new List<Task>();

			var enumerator = words.GetEnumerator();

			for (int i = 0; i < _WorkersCount; i++)
			{
				tasks.Add(Task.Run(() => CreateWordStat(enumerator)));
			}

			Task.WaitAll(tasks.ToArray());

			return _Result;
		}

		private void WriteToResult(Dictionary<string, int> dict)
		{
			if(dict == null) return;

			foreach (var kvp in dict)
			{
				var key = kvp.Key;
				var val = kvp.Value;

				_OutputLock.Change(() =>
					_Result.AddOrUpdate(
						key,
						val,
						(k, v) => v + val));
			}
		}
		private void CreateWordStat(IEnumerator<string> input)
		{
			//Thread.CurrentThread.Name = Guid.NewGuid().ToString().Substring(0,4);
			//Trace.WriteLine(string.Format("TH: {0}", Thread.CurrentThread.Name));

			while (true)
			{
				string[] words = _InputLock.Get(() => 
				{
					var items = new string[_BlockSize];
					int i;
					for (i = 0; i < _BlockSize; i++)
					{
						if(!input.MoveNext()) break;
						items[i] = input.Current;
					}
					return
						i == 0 ? null :
						i == _BlockSize ? items : 
						items
							.Take(i)
							.ToArray();
				});

				if (words == null)
				{
					break;
				}

				var dict = new Dictionary<string, int>();

				foreach (var word in words)
				{
					_TotalWords++;

					//Trace.WriteLine(string.Format("TH: {0}; {1}", Thread.CurrentThread.Name, word));

					dict.AddOrUpdate(
						word,
						1,
						(k, v) => v + 1);
				}

				WriteToResult(dict);
			}

			//Trace.WriteLine(string.Format("exit: CreateWordStat; TotalWords Processed: {0}", _TotalWords));

		}
	}

}
