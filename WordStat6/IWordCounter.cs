using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordStat6
{
	public interface IWordCounter
	{
		IDictionary<string, int> Count(Action<double>? progressAction);
	}
}
