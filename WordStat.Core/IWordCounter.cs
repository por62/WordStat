using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordStat.Core
{
	public interface IWordCounter
	{
		double Progress { get;}
		Stream Stream {get; set;}
		Encoding StreamEncoding { get; set;}
		IDictionary<string, int> Count(IWordBreaker wb);
	}
}
