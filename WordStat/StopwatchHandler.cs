using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	class StopwatchHandler : IDisposable
	{
		private Stopwatch _Stopwatch;
		private Action<TimeSpan> _ElapsedSetter;

		void IDisposable.Dispose()
		{
			_Stopwatch.Stop();

			if (_ElapsedSetter != null) _ElapsedSetter(_Stopwatch.Elapsed);
		}

		public StopwatchHandler(Action<TimeSpan> setter)
		{
			_ElapsedSetter = setter;
			_Stopwatch = new Stopwatch();
			_Stopwatch.Restart();
		}
	}
}
