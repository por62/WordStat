using System;
using System.Diagnostics;

namespace WordStat.Core
{
	class StopwatchHandler : IDisposable
	{
		private Stopwatch _Stopwatch;
		private Action<TimeSpan> _ElapsedSetter;

		void IDisposable.Dispose()
		{
			_Stopwatch.Stop();

			_ElapsedSetter?.Invoke(_Stopwatch.Elapsed);
		}

		public StopwatchHandler(Action<TimeSpan> setter)
		{
			_ElapsedSetter = setter;
			_Stopwatch = new Stopwatch();
			_Stopwatch.Restart();
		}
	}
}
