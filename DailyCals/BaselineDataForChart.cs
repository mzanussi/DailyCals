using System;
using System.Windows;
using System.Collections.Generic;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public class BaselineDataForChart : List<WeightRecord>
    {
        public BaselineDataForChart()
        {
            try
            {
                // Get the application's profile data object.
                RemoteProfileRecordV2 profile = (Application.Current as DailyCals.App).ProfileDataObject;

                // Retrieve the profile. We'll use the profile
                // data to construct the baseline data for the
                // chart.
                if (profile == null)
                {
                    // no data yet to load baseline chart with
                    return;
                }

                // Need to figure out weight per week to lose and
                // the corresponding calories per day to reduce
                // in order to lose that weight per week.
                double wlpd = DailyCalories.WeightLossPerDay(profile.startdate, profile.goaldate, profile.startweight, profile.goalweight);

                // To calculate baseline, current weight should
                // begin with the start weight in profile.
                double currentWeight = profile.startweight;

                // Total days between start and goal date.
                double td = (profile.goaldate - profile.startdate).TotalDays;
                int fullweeks = Convert.ToInt32(Math.Floor(td / 7));
                int leftovers = Convert.ToInt32(td % 7);

                // Create a primary key, and index.
                int index = 0;

                // Then load table with baseline data.
                for (int i = 0; i <= fullweeks; i++)
                {
                    // Create weight record for this week. Store the
                    // beginning date of the week, expected weight at
                    // the start of the week, and daily colorie
                    // requirements to lose X pounds by end of week.
                    // Also record week number, since id is not week.
                    WeightRecord wr = new WeightRecord();
                    wr.date = DailyCalories.GetToday().AddDays(i * 7);
                    wr.weight = currentWeight;
                    wr.dailycals = DailyCalories.DailyCals(currentWeight, profile.height, profile.birthdate, (profile.isfemale == 1 ? true : false), wr.date, profile.goaldate, currentWeight, profile.goalweight, profile.activitylevel, (profile.isstandard == 1 ? true : false));
                    wr.week = i + 1;
                    // Marker week so ViewPlan knows the final weighin.
                    if (i == fullweeks && leftovers == 0)
                    {
                        wr.week = 0;
                    }
                    wr.daysOut = Convert.ToInt32((wr.date - profile.startdate).TotalDays);
                    wr.id = index++;
                    // Insert record.
                    Add(wr);
                    // Current weight is now reduced by weekly weight loss, 
                    // but only if we're not on the last full week.
                    if (i != fullweeks)
                    {
                        currentWeight -= (wlpd * 7);
                    }
                }
                if (leftovers > 0)
                {
                    // Adjust current weight to account for short week.
                    currentWeight -= (wlpd * leftovers);
                    // Create one final record for goal weight.
                    WeightRecord wr = new WeightRecord();
                    wr.date = DailyCalories.GetToday().AddDays((fullweeks * 7) + leftovers);
                    wr.weight = currentWeight;
                    wr.dailycals = DailyCalories.DailyCals(currentWeight, profile.height, profile.birthdate, (profile.isfemale == 1 ? true : false), wr.date, profile.goaldate, currentWeight, profile.goalweight, profile.activitylevel, (profile.isstandard == 1 ? true : false));
                    wr.week = 0;
                    wr.daysOut = Convert.ToInt32((wr.date - profile.startdate).TotalDays);
                    wr.id = index;
                    // Insert record.
                    Add(wr);
                }
            }
            catch
            {
                // No data yet. Ignore. (Prevents errors in Visual Studio.)
            }

        }
    }
}
