using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private void tbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbEmail.Text))
            {
                if (!String.IsNullOrWhiteSpace(pwPassword.Password))
                {
                    btnSubmit.IsEnabled = true;
                    return;
                }
            }
            btnSubmit.IsEnabled = false;
        }

        private void pwPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(pwPassword.Password))
            {
                if (!String.IsNullOrWhiteSpace(tbEmail.Text))
                {
                    btnSubmit.IsEnabled = true;
                    return;
                }
            }
            btnSubmit.IsEnabled = false;
        }

        private void lbCreateProfile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreateProfile.xaml", UriKind.Relative));
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool isOk = true;

            if (DataValidation.IsValidEmail(tbEmail))
            {
                txtEmailMsg.Text = "";
            }
            else
            {
                txtEmailMsg.Text = "Invalid email.";
                isOk = false;
            }

            if (isOk)
            {
                DailyCalsServiceV2Client dcsc = new DailyCalsServiceV2Client();
                dcsc.GetProfileCompleted += new EventHandler<GetProfileCompletedEventArgs>(dcsc_GetProfileCompleted);
                // Set progress bar, and clear some stuff.
                performanceProgressBar.Visibility = Visibility.Visible;
                btnSubmit.IsEnabled = false;
                tbEmail.IsEnabled = false;
                pwPassword.IsEnabled = false;
                // Run...
                dcsc.GetProfileAsync(tbEmail.Text);
            }
        }

        void dcsc_GetProfileCompleted(object sender, GetProfileCompletedEventArgs e)
        {
            performanceProgressBar.Visibility = Visibility.Collapsed;
            if (e.Error != null)
            {
                // WCF web service not available (likely an EndPointNotFoundException).
                MessageBox.Show("We can't access your profile at this time. Please check your network connectivity or wait a few moments for web services to come back online and try again.", "Web Service Unavailable", MessageBoxButton.OK);
            }
            else
            {
                RemoteProfileRecordV2 profile = e.Result;
                if (profile != null)
                {
                    if (profile.profilep == pwPassword.Password)
                    {
                        // Set the application's profile data object.
                        (Application.Current as DailyCals.App).ProfileDataObject = profile;
                        // Save profile data to iso storage so app doesn't have to come to login anymore.
                        bool result = (Application.Current as DailyCals.App).SaveProfileDataToIsolatedStorage();
                        // Update weight db and navigate back to main page.
                        DailyCalories.UpdateWeightDB(profile);
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        txtEmailMsg.Text = "Invalid email or password. Try again.";
                    }
                }
                else
                {
                    txtEmailMsg.Text = "Invalid email or password. Try again.";
                }
            }
            tbEmail.IsEnabled = true;
            pwPassword.IsEnabled = true;
            btnSubmit.IsEnabled = true;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // When we leave the page -
            // store the semi-completed log text in the application

            // Get a reference to the parent application
            App thisApp = App.Current as App;

            // Store the text in the application
            thisApp.Email = tbEmail.Text;
            thisApp.Password = pwPassword.Password;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // When we navigate to the page -
            // put the semi-completed log text in the application
            if (App.WasTombstoned)
            {
                // Get a reference to the parent application
                App thisApp = App.Current as App;

                // Store the text in the application
                tbEmail.Text = thisApp.Email;
                pwPassword.Password = thisApp.Password;
            }

            base.OnNavigatedTo(e);
        }
    }
}