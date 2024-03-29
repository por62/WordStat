﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordStat6
{
	public class WordCounterMT : WordCounterBase
	{
		private ConcurrentQueue<string> _WordBuffer = new ConcurrentQueue<string>();
		private ConcurrentDictionary<string, int> _WordStatTotal = new ConcurrentDictionary<string, int>();
		private object _BufferSyncRoot = new object();
		private RWLocker _Lock = new RWLocker();
		private int _WorkersCount;
		private bool _StopWorkers;

		public WordCounterMT(IWordBreaker wb, Stream s, Encoding enc, int workersCount) : base(wb, s, enc)
		{ 
			_WorkersCount = workersCount;
		}

		protected override IDictionary<string, int> Process(IEnumerable<string> words)
		{
			List<Task> tasks = new List<Task>();

			for (int i = 0; i < _WorkersCount; i++)
			{
				tasks.Add(Task.Run(() => CreateWordStat()));
			}

			tasks.Add(Task.Run(() => ReadWordsAndFillQueue(words)));

			Task.WaitAll(tasks.ToArray());

			return _WordStatTotal;
		}

		private void ReadWordsAndFillQueue(IEnumerable<string> words)
		{
			foreach (var word in words)
			{
				WriteToBuffer(word);
			}
			_Lock.Change(() => _StopWorkers = true);
		}
		private void WriteToBuffer(string word)
		{
			_WordBuffer.Enqueue(word);
		}
		private string? ReadFromBuffer()
		{
			if (_WordBuffer.Any())
			{
				string? word;
				if (_WordBuffer.TryDequeue(out word)) return word;
			}

			return null;
		}
		private void CreateWordStat()
		{
			//Thread.CurrentThread.Name = Guid.NewGuid().ToString().Substring(0,4);
			//Trace.WriteLine(string.Format("TH: {0}", Thread.CurrentThread.Name));

			while (true)
			{
				bool stopWorkers = _Lock.Get(() => _StopWorkers);

				var word = ReadFromBuffer();

				if (stopWorkers && word == null)
				{
					break;
				}

				if (word == null)
				{
					continue;
				}

				_WordStatTotal.AddOrUpdate(
					word,
					1,
					(k, s) => s + 1);
			}
		}
	}

	public static class DictUtils
	{
		public static void AddOrUpdate<K, V>(this Dictionary<K, V> dict, K key, V val, Func<K, V, V> update) where K:notnull
		{
			if (dict.ContainsKey(key))
			{
				var v = dict[key];
				dict[key] = update(key, v);
			}
			else
			{
				dict.Add(key, val);
			}
		}
	}
}
