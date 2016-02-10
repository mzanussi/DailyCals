using System;
using System.Data.Linq.Mapping;

namespace DailyCals
{
    [Table]
    public class WeightRecord
    {
        private int _id;            // record id (auto)
        private DateTime _date;     // date of weigh-in
        private double _weight;     // weight
        private double _dailycals;  // daily cals - BaselineDB
        private int _week;          // week number - BaselineDB
        private bool _start;        // start weight - WeightDB
        private int _daysout;       // days since start

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int id
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        [Column]
        public DateTime date
        {
            set
            {
                _date = value;
            }
            get
            {
                return _date;
            }
        }

        [Column]
        public double weight
        {
            set
            {
                _weight = value;
            }
            get
            {
                return _weight;
            }
        }

        [Column]
        public double dailycals
        {
            set
            {
                _dailycals = value;
            }
            get
            {
                return _dailycals;
            }
        }

        [Column]
        public int week
        {
            set
            {
                _week = value;
            }
            get
            {
                return _week;
            }
        }

        [Column]
        public bool isStartWeight
        {
            set
            {
                _start = value;
            }
            get
            {
                return _start;
            }
        }

        [Column]
        public int daysOut
        {
            set
            {
                _daysout = value;
            }
            get
            {
                return _daysout;
            }
        }
    }
}
