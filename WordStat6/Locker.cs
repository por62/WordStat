using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordStat6
{
	internal class RWLocker
	{
		private ReaderWriterLockSlim _RWLock = new ReaderWriterLockSlim();

		public T Get<T>(Func<T> get)
		{
			_RWLock.EnterReadLock();
			try
			{
				return get();
			}
			finally
			{
				_RWLock.ExitReadLock();
			}
		}
		public void Change(Action change)
		{
			_RWLock.EnterWriteLock();
			try
			{
				change();
			}
			finally
			{
				_RWLock.ExitWriteLock();
			}
		}
	}
	internal class Locker
	{
		public T Get<T>(Func<T> get)
		{
			lock (this) return get();
		}
		public void Change(Action change)
		{
			lock (this)
			{
				if (change != null) change();

				Monitor.PulseAll(this);
			}
		}
	}
}
