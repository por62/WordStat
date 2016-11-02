using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordStat
{
	class Program
	{
		private const int _TopWordsCount = 10;
		private const int _Padding = 15;

		private static CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
		private static Task _ProgressTask;
		static void Main(string[] args)
		{
			//var folderName = @"D:\Temp\MyghtyCall";
			//var outFileName = Path.Combine(folderName, "alltexts.txt");

			//foreach (var item in Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories))
			//{
			//	var txt = File.ReadAllText(item, Encoding.UTF8);

			//	File.AppendAllText(outFileName, txt, Encoding.UTF8);
			//}

			//return;

			if (args == null || args.Length == 0 || args[0] == "?" || args[0] == "-h")
			{
				Console.WriteLine(
@"wordstat.exe filepath [minWordLength] [threadCount] [blockSize] [regex]
filepath - path to the text file
[minWordLength] - default 5
[threadCount] - default 10
[blockSize] - default 500, if blockSize = 0 used old MT algoritm
[regex] - use RegEx word breaker, default used by delimiters");
				return;
			}

			try
			{
				Settings settings = ParseSettings(args);
				if (!File.Exists(settings.FileName)) throw new FileNotFoundException(settings.FileName);

				using (var fs = File.Open(settings.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					TimeSpan elapsed = TimeSpan.Zero;
					IDictionary<string, int> statistics;
					var enWords = new List<WordStat>();
					var ruWords = new List<WordStat>();

					using (var sw = new StopwatchHandler(ts => elapsed = ts))
					{
						IWordBreaker wb =
							settings.UseRegex ? (IWordBreaker) new WordBreakerRegEx() :
							new WordBreakerByDelimiters(); 

						wb.MinWordLength = settings.MinWordLength;

						IWordCounter wc = 
							settings.ThreadCount == 1 ? new WordCounterST() :
							settings.BlockSize == 0 ? (IWordCounter)new WordCounterMT(settings.ThreadCount) :
							new WordCounterMTm(settings.ThreadCount, settings.BlockSize);

						_ProgressTask = Task.Factory.StartNew(() => ShowProgress(wc), _CancellationTokenSource.Token);

						wc.Stream = fs;
						wc.StreamEncoding = Encoding.UTF8;

						statistics = wc.Count(wb);

						_CancellationTokenSource.Cancel();
						_ProgressTask.Wait();


						var orderedStat = statistics
							.OrderByDescending(kvp => kvp.Value)
							.Select(kvp => new { word = kvp.Key, count = kvp.Value });

						var ld = new LangDetector();

						foreach (var item in orderedStat)
						{
							var lang = ld.GetLanguage(item.word);
							if (lang != null)
							{
								var ws = new WordStat
								{
									Text = item.word,
									Count = item.count,
									Lang = lang.Value,
								};

								if (lang.Value == Lang.English)
								{
									if (enWords.Count < _TopWordsCount)
									{
										enWords.Add(ws);
									}

								}
								else
								{
									if (ruWords.Count < _TopWordsCount)
									{
										ruWords.Add(ws);
									}
								}

								if (ruWords.Count == _TopWordsCount && enWords.Count == _TopWordsCount)
								{
									break;
								}
							}
						}
					}

					PrintResult(Lang.English, enWords);
					PrintResult(Lang.Russian, ruWords);

					WriteElapsed(elapsed);
				}
			}
			catch (Exception x)
			{
				Console.WriteLine();
				ConsoleColorUtils.WriteLine(ConsoleColor.Magenta, x.GetType().FullName);
				ConsoleColorUtils.WriteLine(ConsoleColor.Red, x.Message);
				ConsoleColorUtils.WriteLine(ConsoleColor.DarkGray, x.StackTrace);
			}

			ConsoleColorUtils.WriteLine(ConsoleColor.White, "\r\nPress any key to exit");
			Console.ReadKey();
		}

		private static Settings ParseSettings(string[] args)
		{
			var settings = new Settings
			{
				FileName = args[0],
				MinWordLength = args.Length > 1 ? int.Parse(args[1]) : 5,
				ThreadCount = args.Length > 2 ? int.Parse(args[2]) : 10,
				BlockSize = args.Length > 3 ? int.Parse(args[3]) : 500,
				UseRegex = args.Length > 4 && args[4] == "regex",

			};

			ConsoleColorUtils.WriteLine(ConsoleColor.Yellow, "Settings");

			WriteSettingsRow("FileName", settings.FileName);
			WriteSettingsRow("MinWordLength", settings.MinWordLength);
			WriteSettingsRow("ThreadCount", settings.ThreadCount);
			WriteSettingsRow("BlockSize", settings.BlockSize);
			WriteSettingsRow("UseRegex", settings.UseRegex);

			Console.WriteLine();

			return settings;
		}

		private static void WriteSettingsRow(string name, object value)
		{
			ConsoleColorUtils.Write(ConsoleColor.Gray, name.PadRight(_Padding));
			ConsoleColorUtils.Write(ConsoleColor.Gray, " : ");
			ConsoleColorUtils.WriteLine(ConsoleColor.White, string.Format("{0}", value));
		}
		private static void PrintResult(Lang lang, List<WordStat> allWords)
		{
			if (allWords == null || !allWords.Any()) return;

			ConsoleColorUtils.WriteLine(ConsoleColor.Yellow, lang.ToString().PadRight(20));

			foreach (var ws in allWords)
			{
				WriteResultRow(ws.Text, ws.Count);
			}

			Console.WriteLine();
		}
		private static void WriteResultRow(string text, int count)
		{
			ConsoleColorUtils.Write(ConsoleColor.Gray, string.Format("{0:N0}", count).PadLeft(_Padding));
			ConsoleColorUtils.Write(ConsoleColor.Gray, " : ");
			ConsoleColorUtils.WriteLine(ConsoleColor.White, string.Format("{0}", text));
		}
		private static void WriteProgress(double percents)
		{
			ConsoleColorUtils.Write(ConsoleColor.Cyan, "Progress".PadRight(_Padding));
			ConsoleColorUtils.Write(ConsoleColor.Gray, " : ");
			ConsoleColorUtils.WriteLine(ConsoleColor.White, string.Format("{0:N2}%", percents).PadRight(_Padding));
			Console.WriteLine();
		}
		private static void WriteElapsed(TimeSpan elapsed)
		{
			ConsoleColorUtils.Write(ConsoleColor.Cyan, "Elapsed".PadRight(_Padding));
			ConsoleColorUtils.Write(ConsoleColor.Gray, " : ");
			ConsoleColorUtils.WriteLine(ConsoleColor.Green, elapsed.ToString( ));
		}
		private static void ShowProgress(IWordCounter wc)
		{
			int cursorLeft = Console.CursorLeft;
			int cursorTop = Console.CursorTop;

			while (true)
			{
				if (_CancellationTokenSource.IsCancellationRequested)
				{
					Console.SetCursorPosition(cursorLeft, cursorTop);
					WriteProgress(100);
					break;
				}

				Console.SetCursorPosition(cursorLeft, cursorTop);
				WriteProgress(wc.Progress);

				Thread.Sleep(3000);
			}
		}
	}

	class Word
	{
		public string Text;
		public Lang Lang;
	}
	class WordStat : Word
	{
		public int Count;
	}

	public enum Lang
	{
		English,
		Russian,
	}

	class LangCharacrterRange
	{
		public char Start;
		public char End;
		public Lang Language;
	}

	class Settings
	{
		public string FileName;
		public int MinWordLength;
		public int ThreadCount;
		public int BlockSize;
		public bool UseRegex;
	}

	static class ConsoleColorUtils
	{
		public static void Write(ConsoleColor foreColor, string msg)
		{
			Console.ForegroundColor = foreColor;
			Console.Write(msg);
			Console.ResetColor();
		}
		public static void WriteLine(ConsoleColor foreColor, string msg)
		{
			Write(foreColor, msg);
			Console.WriteLine();
		}
	}
}
