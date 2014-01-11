using System;
using System.IO;

namespace PSOFAPConsole
{
	public class StatsWriter : IDisposable
	{
		private StreamWriter benchmark;
		private StreamWriter stats;
		private int benchCounter, statsCounter;

		public StatsWriter(String benchName)
		{
			benchmark = new StreamWriter(new FileStream("Benchmark-" + benchName, FileMode.CreateNew));
			stats = new StreamWriter(new FileStream("Stats-" + benchName, FileMode.CreateNew));
			benchCounter = 0;
			statsCounter = 0;
		}

		public void initiliaze(string[] benchHeadings, string[] statsHeadings)
		{
			benchmark.WriteLine (String.Join (", ", benchHeadings));
			stats.WriteLine (String.Join (", ", statsHeadings));
		}

		private void CounterIncAndPeriodicFlush(ref int counter, ref StreamWriter writer)
		{
			counter++;
			if (counter % 10 == 0) {
				writer.Flush ();
			}
		}

		public void WriteBenchmarkLine(object val1, object val2) 
		{
			if (val1 is double) {
				val1 = ((double)val1).ToString ("F2");
			}
			if (val2 is double) {
				val1 = ((double)val1).ToString ("F2");
			}
			benchmark.WriteLine ("{0}, {1}", val1, val2);
			CounterIncAndPeriodicFlush (benchCounter, benchmark);
		}

		public void WriteStatsLine(String format, int iteration, double[] values)
		{
			stats.WriteLine(format,
				iteration, values[0].ToString("F"), values[1].ToString("F"), values[2].ToString("F"), 
				values[3].ToString("F"), values[4].ToString("F"), values[5].ToString("F"), 
				values[6].ToString("F"), values[7].ToString("F"), values[8].ToString("F"), 
				values[9].ToString("F"), values[10].ToString("F"), values[11].ToString("F"), 
				values[12].ToString("F"), values[13].ToString("F"), values[14].ToString("F"), 
				values[15].ToString("F"), values[16].ToString("F"), values[17].ToString("F"), 
				values[18].ToString("F"));
			CounterIncAndPeriodicFlush (statsCounter, stats);
		}

		public void Dispose()
		{
			benchmark.Close ();
			stats.Close ();
		}
	}
}

