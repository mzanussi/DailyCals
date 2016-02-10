using System.Data.Linq;

namespace DailyCals
{
    public class WeightDB : DataContext
    {
        private static string cstr = "Data Source=isostore:weightdb.sdf";
        public Table<WeightRecord> weightrecords;

        public WeightDB()
            : base(cstr)
        {
            if (!DatabaseExists())
            {
                CreateDatabase();
            }
        }
    }
}
