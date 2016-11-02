using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordStat
{
	public interface IWordCounter
	{
		double Progress { get;}
		Stream Stream {get; set;}
		Encoding StreamEncoding { get; set;}
		IDictionary<string, int> Count(IWordBreaker wb);
	}
}
