using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Linq;

namespace DailyCals
{
    public class DataValidation
    {
        public static bool IsValidWeight(TextBox tb)
        {
            try
            {
                // Convert to double.
                double weight = Convert.ToDouble(tb.Text);

                // Verify weight > 0.
                if (weight <= 0.0)
                {
                    tb.Background = new SolidColorBrush(Colors.Red);
                    return false;
                }
            }
            catch
            {
                tb.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            // Default background color.
            tb.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
            return true;
        }

        public static bool IsStartWeightGreaterThanGoalWeight(TextBox startWeight, TextBox goalWeight)
        {
            try
            {
                // Convert contents to doubles.
                double start = Convert.ToDouble(startWeight.Text);
                double goal = Convert.ToDouble(goalWeight.Text);

                // Test weight range.
                if (start <= goal)
                {
                    startWeight.Background = new SolidColorBrush(Colors.Red);
                    return false;
                }
            }
            catch
            {
                startWeight.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            // Default background color.
            startWeight.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
            return true;
        }

        public static bool IsValidGoalDate(Microsoft.Phone.Controls.DatePicker dp)
        {
            // Convert the datepicker value.
            DateTime goalDate = (DateTime)dp.Value;

			// Do not allow goal dates equal to or earlier than today.
            if (goalDate <= DailyCalories.GetToday())
            {
                dp.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            // Default background color.
            dp.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
            return true;
        }

        public static bool IsValidWeighInDate(Microsoft.Phone.Controls.DatePicker dp)
        {
            // Convert the datepicker value.
            DateTime date = (DateTime)dp.Value;

            // Get the most recent weigh-in date.
            WeightDB wdb = new WeightDB();
            WeightRecord wr = (from rec in wdb.weightrecords orderby rec.date descending select rec).First();
            DateTime lastDate = wr.date;

            // Get today's date. Use canned date if demo mode.
            DateTime today = DateTime.Today;
            if (DailyCalories.demoMode)
            {
                today = DateTime.Today.AddYears(2);
            }

            // Date is invalid if it occurs prior to most recent weigh-in
            // or if the date is in the future. Today is ok. This code
            // will also prevent a duplicate date from being entered.
            if (date <= lastDate || date > today)
            {
                dp.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            // Default background color.
            dp.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
            return true;
        }

        public static bool IsValidBirthdate(Microsoft.Phone.Controls.DatePicker dp)
        {
            DateTime birthdate = (DateTime)dp.Value;

            if (birthdate >= DateTime.Today)
            {
                dp.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            // Default background color.
            dp.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
            return true;
        }

        public static bool IsValidHeight(TextBox tb)
        {
            try
            {
                // Convert to double.
                double height = Convert.ToDouble(tb.Text);

                // Verify height > 0 and height < 1000.
                if (height <= 0.0 || height >= 1000.0)
                {
                    tb.Background = new SolidColorBrush(Colors.Red);
                    return false;
                }
            }
            catch
            {
                tb.Background = new SolidColorBrush(Colors.Red);
                return false;
            }

            // Default background color.
            tb.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
            return true;
        }

        public static bool IsValidEmail(TextBox tb)
        {
            string str = tb.Text.Replace(" ", "");
            tb.Text = str;
            if (Regex.IsMatch(str, @"^[a-zA-Z][a-zA-Z0-9_\-\.]*@[a-zA-Z]([a-zA-Z0-9_\-\.]*)\.[a-zA-Z]+$"))
            {
                // Default background color.
                tb.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
                return true;
            }
            else
            {
                tb.Background = new SolidColorBrush(Colors.Red);
                return false;
            }
        }
    }
}
