using System.Collections.Generic;
using System.Linq;

namespace DailyCals
{
    public class WeightDataForChart : List<WeightRecord>
    {
        public WeightDataForChart()
        {
            try
            {
                // Load data here from WeightDB.
                WeightDB wdb = new WeightDB();
                IQueryable<WeightRecord> results = from rec in wdb.weightrecords select rec;
                foreach (WeightRecord rec in results)
                {
                    Add(rec);
                }
                // This is test data. the old way of data population
                /*Add(new WeightRecord { date = DateTime.Today, weight = 215.0 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(7), weight = 213.0 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(14), weight = 212.0 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(21), weight = 211.5 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(28), weight = 212.0 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(35), weight = 210.0 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(42), weight = 207.5 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(49), weight = 207.0 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(56), weight = 204.3 });
                Add(new WeightRecord { date = DateTime.Today.AddDays(63), weight = 203.0 });*/
            }
            catch
            {
                // No data yet. Ignore. (Prevents errors in Visual Studio.)
            }
        }
    }
}
