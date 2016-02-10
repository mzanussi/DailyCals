using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            // Get the application's profile data object.
            RemoteProfileRecordV2 profile = (Application.Current as DailyCals.App).ProfileDataObject;

            // Hide/display the Demo indicator.
            if (DailyCalories.demoMode)
            {
                txtDemoIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                txtDemoIndicator.Visibility = Visibility.Collapsed;
            }

            if (profile == null)
            {
                // Profile has not been loaded yet. Deactivate
                // controls and activate special message.
                btnWeighIn.IsEnabled = false;
                btnViewPlan.IsEnabled = false;
                chartProgress.Visibility = Visibility.Collapsed;
                tbWelcome.Visibility = Visibility.Visible;
                btnMyProfile.Content = "Login";
                return;
            }

            DateTime p_goaldate = profile.goaldate;
            double p_height = profile.height;
            DateTime p_birthdate = profile.birthdate;
            bool p_isfemale = (profile.isfemale == 1 ? true : false);
            double p_goalweight = profile.goalweight;
            double p_activitylevel = profile.activitylevel;
            double p_startweight = profile.startweight;
            DateTime p_startdate = profile.startdate;
            bool p_isstandard = (profile.isstandard == 1 ? true : false);

            // Activate controls.
            btnWeighIn.IsEnabled = true;
            btnViewPlan.IsEnabled = true;
            chartProgress.Visibility = Visibility.Visible;
            tbWelcome.Visibility = Visibility.Collapsed;
            btnMyProfile.Content = "My Profile";

            // Current weight will be the most recent weigh-in.
            WeightDB wdb = new WeightDB();
            // .Last() throws NotSupportedException
            WeightRecord wr = (from rec in wdb.weightrecords orderby rec.date descending select rec).First();
            double lastWeight = wr.weight;
            DateTime lastDate = wr.date;
            double daysRemaining = (p_goaldate - lastDate).TotalDays;

            // Do some calculations.
            txtSummaryMsg.Text = "Current weight loss progress based on the most ";
            txtSummaryMsg.Text += "recent weigh-in of ";
            txtSummaryMsg.Text += lastWeight.ToString("0.0");
            txtSummaryMsg.Text += (p_isstandard ? " pounds recorded on " : " kilograms recorded on ");
            txtSummaryMsg.Text += lastDate.ToShortDateString();
            txtSummaryMsg.Text += ":";

            if (lastDate >= p_goaldate)
            {
                txtMaxCals.Text = "Goal Date Reached";
                txtMaxCals.TextWrapping = TextWrapping.Wrap;
                txtMaxCals.FontSize = 34;
                txtMaxCals.Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                txtMaxCalsMsg.Text = "You have reached your goal date with your most recent weigh-in.";
            }
            else
            {
                if (lastWeight > p_goalweight)
                {
                    // Last weight not at goal.
                    txtMaxCals.Text = DailyCalories.DailyCals(lastWeight, p_height, p_birthdate, p_isfemale, lastDate, p_goaldate, lastWeight, p_goalweight, p_activitylevel, p_isstandard).ToString("0");
                    txtMaxCalsMsg.Text = "approximate maximum daily calories allowable to lose ";
                    txtMaxCalsMsg.Text += (DailyCalories.WeightLossPerDay(lastDate, p_goaldate, lastWeight, p_goalweight) * 7.0).ToString("0.0");
                    txtMaxCalsMsg.Text += (p_isstandard ? " pounds per week." : " kilograms per week.");
                }
                else
                {
                    // Goal reached.
                    double cpdtr = DailyCalories.CaloriesPerDayToReduce(lastDate, p_goaldate, lastWeight, p_goalweight, p_isstandard);
                    double dc = DailyCalories.DailyCals(lastWeight, p_height, p_birthdate, p_isfemale, lastDate, p_goaldate, lastWeight, p_goalweight, p_activitylevel, p_isstandard);
                    txtMaxCals.Text = (DailyCalories.DailyCals(lastWeight, p_height, p_birthdate, p_isfemale, lastDate, p_goaldate, lastWeight, p_goalweight, p_activitylevel, p_isstandard) + cpdtr).ToString("0");
                    txtMaxCals.Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                    txtMaxCalsMsg.Text = "approximate maximum daily calories allowable to maintain current weight.";
                }
            }

            double pc = DailyCalories.PercentageCompleted(p_startweight, p_goalweight, lastWeight);
            if (pc < 0)
            {
                // Negative percentage (gaining weight)
                txtPercComp.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (pc >= 100)
            {
                // Beyond goal weight.
                txtPercComp.Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
            }
            else
            {
                // Positive percentage (losing weight)
                txtPercComp.Foreground = new SolidColorBrush(Colors.Green);
            }
            txtPercComp.Text = pc.ToString("0") + "%";
            txtPercCompMsg.Text = "percentage to goal having ";
            txtPercCompMsg.Text += (pc < 0 ? "gained " : "lost ");
            txtPercCompMsg.Text += (Math.Abs(p_startweight - lastWeight)).ToString("0.0");
            txtPercCompMsg.Text += (p_isstandard ? " pounds " : " kilograms ");
            if (pc < 0)
            {
                // Negative percentage (gaining weight)
                txtPercCompMsg.Text += "(goal is a ";
                txtPercCompMsg.Text += (p_startweight - p_goalweight).ToString("0.0");
                txtPercCompMsg.Text += (p_isstandard ? " pound weight loss)." : " kilogram weight loss).");
            }
            else
            {
                // Positive percentage (losing weight)
                txtPercCompMsg.Text += "out of ";
                txtPercCompMsg.Text += (p_startweight - p_goalweight).ToString("0.0");
                txtPercCompMsg.Text += (p_isstandard ? " pounds total." : " kilograms total.");
            }

            txtDays.Text = Math.Abs(daysRemaining).ToString("0");
            if (daysRemaining == 0)
            {
                // at goal date
                txtDays.Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                txtDaysMsg.Text = "days until " + p_goaldate.ToShortDateString() + " goal date, ";
            }
            else if (daysRemaining < 0 && lastWeight > p_goalweight)
            {   // beyond goal date but not at goal weight
                txtDays.Foreground = new SolidColorBrush(Colors.Yellow);
                txtDaysMsg.Text = "days since " + p_goaldate.ToShortDateString() + " goal date, ";
            }
            else if (daysRemaining < 0 && lastWeight <= p_goalweight)
            {   // beyond goal date and goal weight
                txtDays.Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
                txtDaysMsg.Text = "days since " + p_goaldate.ToShortDateString() + " goal date, ";
            }
            else
            {
                txtDays.Foreground = new SolidColorBrush(Colors.Green);
                txtDaysMsg.Text = "days until " + p_goaldate.ToShortDateString() + " goal date, ";
            }
            txtDaysMsg.Text += "currently in week " + DailyCalories.CurrentWeek(p_startdate) + " of weight loss plan.";
        }

        private void btnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(btnMyProfile.Content) == "My Profile")
            {
                NavigationService.Navigate(new Uri("/MyProfile.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            }
        }

        private void btnWeighIn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/WeighIn.xaml", UriKind.Relative));
        }

        private void btnViewPlan_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ViewPlan.xaml", UriKind.Relative));
        }

        private void imgAbout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }
}