using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordStat
{
	internal class RWLocker
	{
		private ReaderWriterLock _RWLock = new ReaderWriterLock();

		public T Get<T>(Func<T> get)
		{
			_RWLock.AcquireReaderLock(100);
			try
			{
				return get();
			}
			finally
			{
				_RWLock.ReleaseReaderLock();
			}
		}
		public void Change(Action change)
		{
			_RWLock.AcquireWriterLock(100);
			try
			{
				change();
			}
			finally
			{
				_RWLock.ReleaseWriterLock();
			}
		}
	}
	internal class Locker
	{
		public T Get<T>(Func<T> get)
		{
			lock(this) return get();
		}
		public void Change(Action change)
		{
			lock (this)
			{
				if (change != null) change();

				Monitor.PulseAll(this);
			}
		}
		public void Wait(Func<bool> condition)
		{
			lock (this)
			{
				while (condition())
					Monitor.Wait(this);
			}
		}
		public T WaitAndGet<T>(Func<bool> condition, Func<T> getter)
		{
			lock (this)
			{
				while (condition())
					Monitor.Wait(this);

				return getter();
			}
		}
	}
}
