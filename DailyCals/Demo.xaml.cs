using System;
using System.Windows;
using Microsoft.Phone.Controls;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public partial class Demo : PhoneApplicationPage
    {
        public Demo()
        {
            InitializeComponent();
            PopulateForm();
        }

        private void PopulateForm()
        {
            cbDemo.IsChecked = DailyCalories.demoMode;
            dpStartDate.Value = DailyCalories.demoStartDate;
            if ((Application.Current as DailyCals.App).ProfileDataObject != null)
            {
                cbLogin.IsEnabled = false;
                rbStandard.IsEnabled = false;
                rbMetric.IsEnabled = false;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            // Save to global data.
            DailyCalories.demoMode = (bool)cbDemo.IsChecked;
            DailyCalories.demoStartDate = (DateTime)dpStartDate.Value;
            DailyCalories.demoLocalLogin = (bool)cbLogin.IsChecked;

            if (cbDemo.IsChecked == true)
            {
                // Check for local login, handle accordingly.
                if (cbLogin.IsChecked == false)
                {
                    // this is really not enough, a lot more needs to be unwound 
                    // here besides setting this object to null.
                    (Application.Current as DailyCals.App).ProfileDataObject = null;
                }
                else
                {
                    RemoteProfileRecordV2 profile = new RemoteProfileRecordV2();
                    profile.email = "demo@example.com";
                    profile.birthdate = Convert.ToDateTime("12/12/1976");
                    profile.startdate = DailyCalories.demoStartDate;
                    profile.goaldate = profile.startdate.AddDays(180);
                    profile.isfemale = 1;
                    profile.activitylevel = 1.0;
                    profile.profilep = "demo";
                    if (rbStandard.IsChecked == true)
                    {
                        profile.height = 67;
                        profile.startweight = 215;
                        profile.goalweight = 195;
                        profile.isstandard = 1;
                    }
                    else
                    {
                        profile.height = 170;
                        profile.startweight = 97.5;
                        profile.goalweight = 88.5;
                        profile.isstandard = 0;
                    }
                    (Application.Current as DailyCals.App).ProfileDataObject = profile;
                    (Application.Current as DailyCals.App).SaveProfileDataToIsolatedStorage();
                    DailyCalories.UpdateWeightDB(profile);
                }
            }

            // Navigate to MainPage.xaml. DBs will reload.
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void cbLogin_Checked(object sender, RoutedEventArgs e)
        {
            rbStandard.IsEnabled = true;
            rbMetric.IsEnabled = true;
        }

        private void cbLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            rbStandard.IsEnabled = false;
            rbMetric.IsEnabled = false;
        }

        private void cbDemo_Checked(object sender, RoutedEventArgs e)
        {
            cbLogin.IsEnabled = true;
        }

        private void cbDemo_Unchecked(object sender, RoutedEventArgs e)
        {
            cbLogin.IsEnabled = false;
        }
    }
}