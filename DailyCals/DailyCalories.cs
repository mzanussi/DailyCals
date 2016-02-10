using System;
using System.Linq;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public static class DailyCalories
    {
        public static bool demoMode = false;                                // running in demo mode?
        public static DateTime demoStartDate = new DateTime(2012, 3, 1);    // start date (demo mode)
        public static bool demoLocalLogin = false;                          // local login? (demo mode)

        public static DateTime GetToday()
        {
            // Get today's date. Use canned date if demo mode.
            if (DailyCalories.demoMode)
            {
                return demoStartDate;
            }
            else
            {
                return DateTime.Today;
            }

        }

        public static double BMR(double weight, double height, DateTime birthdate, bool isFemale, bool isStandard)
        {
            // basal metabolic rate
            double bmr = 0.0;

            // Calculate age.
            // TODO: calculate fractional age
            DateTime now = DateTime.Today;
            int age = now.Year - birthdate.Year;
            if (now < birthdate.AddYears(age))
            {
                age--;
            }

            // Calculate BMR using Harris-Benedict equation.
            // http://en.wikipedia.org/wiki/Harris-Benedict_equation
            if (isFemale)
            {
                if (isStandard)
                {
                    // Pounds/Inches
                    double wComp = 4.35 * weight;
                    double hComp = 4.7 * height;
                    double aComp = 4.7 * age;
                    bmr = 655 + wComp + hComp - aComp;
                }
                else
                {
                    // Kilos/Centimetres
                    double wComp = 9.563 * weight;
                    double hComp = 1.85 * height;
                    double aComp = 4.676 * age;
                    bmr = 655.1 + wComp + hComp - aComp;
                }
            }
            else
            {
                if (isStandard)
                {
                    // Pounds/Inches
                    double wComp = 6.23 * weight;
                    double hComp = 12.7 * height;
                    double aComp = 6.76 * age;
                    bmr = 66 + wComp + hComp - aComp;
                }
                else
                {
                    // Kilos/Centimetres
                    double wComp = 13.75 * weight;
                    double hComp = 5.003 * height;
                    double aComp = 6.755 * age;
                    bmr = 66.5 + wComp + hComp - aComp;
                }
            }

            return bmr;
        }

        public static int CurrentWeek(DateTime startDate)
        {
            // Use the number of days since profile start date to
            // calculate which week the user is currently in.
            double days = (DateTime.Today - startDate).TotalDays + 1;
            int week = Convert.ToInt32(Math.Ceiling(days / 7));

            return week;
        }

        public static double WeightLossPerDay(DateTime startDate, DateTime goalDate, double startWeight, double goalWeight)
        {
            // Calculate the average weight per day to lose to reach goal weight.
            // Total weight to lose (startWeight - goalWeight) divided by the number
            // of days between the startDate and goalDate gives a daily weight loss
            // amount.
            return ((startWeight - goalWeight) / (goalDate - startDate).TotalDays);
        }

        public static double CaloriesPerDayToReduce(DateTime startDate, DateTime goalDate, double startWeight, double goalWeight, bool isStandard)
        {
            // Calculate the calories per day reduction necessary to lose WeightLossPerWeek.
            // Multiply the required weight loss per week by 3500 (number of calories in
            // one pound of fat) or by 7709 (number of calories in one kg of fat) to get 
            // the weekly calorie count.
            if (isStandard)
            {
                return (WeightLossPerDay(startDate, goalDate, startWeight, goalWeight) * 3500);
            }
            else
            {
                return (WeightLossPerDay(startDate, goalDate, startWeight, goalWeight) * 7709);
            }
        }

        public static int ConvertActivityLevel(double activityLevel)
        {
            if (activityLevel < 2.5)
            {
                return 0;
            }
            else if (activityLevel >= 2.5 && activityLevel < 5)
            {
                return 1;
            }
            else if (activityLevel >= 5 && activityLevel < 7.5)
            {
                return 2;
            }
            else if (activityLevel >= 7.5)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        public static double DailyCals(double currentWeight, double height, DateTime birthdate, bool isFemale, DateTime startDate, DateTime goalDate, double startWeight, double goalWeight, double activityLevel, bool isStandard)
        {
            double dailyCals = 0.0;

            switch (ConvertActivityLevel(activityLevel))
            {
                case 0:
                    dailyCals = (1.2 * BMR(currentWeight, height, birthdate, isFemale, isStandard)) - CaloriesPerDayToReduce(startDate, goalDate, startWeight, goalWeight, isStandard);
                    break;
                case 1:
                    dailyCals = (1.375 * BMR(currentWeight, height, birthdate, isFemale, isStandard)) - CaloriesPerDayToReduce(startDate, goalDate, startWeight, goalWeight, isStandard);
                    break;
                case 2:
                    dailyCals = (1.55 * BMR(currentWeight, height, birthdate, isFemale, isStandard)) - CaloriesPerDayToReduce(startDate, goalDate, startWeight, goalWeight, isStandard);
                    break;
                case 3:
                    dailyCals = (1.725 * BMR(currentWeight, height, birthdate, isFemale, isStandard)) - CaloriesPerDayToReduce(startDate, goalDate, startWeight, goalWeight, isStandard);
                    break;
            }

            return dailyCals;
        }

        public static double PercentageCompleted(double startWeight, double goalWeight, double currentWeight)
        {
            return ((startWeight - currentWeight) / (startWeight - goalWeight) * 100);
        }

        public static void UpdateWeightDB(RemoteProfileRecordV2 pr)
        {
            // Load the weight db.
            WeightDB wdb = new WeightDB();
            WeightRecord wr = null;

            try
            {
                // Retrieve the initial weigh-in (isStartWeight is true) and update.
                wr = (from rec in wdb.weightrecords where (rec.isStartWeight) select rec).First();
                wr.weight = pr.startweight;
            }
            catch
            {
                // No existing WeightDB. Insert a new record.
                wr = new WeightRecord();
                wr.date = DailyCalories.GetToday();
                wr.weight = pr.startweight;
                wr.isStartWeight = true;
                wdb.weightrecords.InsertOnSubmit(wr);
            }

            // Submit changes to WeightDB.
            wdb.SubmitChanges();
        }
    }
}
