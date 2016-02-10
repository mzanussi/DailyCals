using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public partial class CreateProfile : PhoneApplicationPage
    {
        private static string _email;
        private static string _profilep;

        public static string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public static string Profilep
        {
            get { return _profilep; }
            set { _profilep = value; }
        }

        public CreateProfile()
        {
            InitializeComponent();
        }

        private void tbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbEmail.Text))
            {
                if (!String.IsNullOrWhiteSpace(pwPassword.Password) && !String.IsNullOrWhiteSpace(pwPasswordAgain.Password) && pwPassword.Password == pwPasswordAgain.Password)
                {
                    btnSubmit.IsEnabled = true;
                    return;
                }
            }
            btnSubmit.IsEnabled = false;
        }

        private void pwPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(pwPassword.Password) && !String.IsNullOrWhiteSpace(pwPasswordAgain.Password) && pwPassword.Password == pwPasswordAgain.Password)
            {
                if (!String.IsNullOrWhiteSpace(tbEmail.Text))
                {
                    btnSubmit.IsEnabled = true;
                    return;
                }
            }
            btnSubmit.IsEnabled = false;
        }

        private void pwPasswordAgain_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(pwPassword.Password) && !String.IsNullOrWhiteSpace(pwPasswordAgain.Password) && pwPassword.Password == pwPasswordAgain.Password)
            {
                if (!String.IsNullOrWhiteSpace(tbEmail.Text))
                {
                    btnSubmit.IsEnabled = true;
                    return;
                }
            }
            btnSubmit.IsEnabled = false;
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
                // See if email is already in use by another profile.
                DailyCalsServiceV2Client dcsc = new DailyCalsServiceV2Client();
                dcsc.DoesProfileExistCompleted += new EventHandler<DoesProfileExistCompletedEventArgs>(dcsc_DoesProfileExistCompleted);
                // Set progress bar, and clear some stuff.
                performanceProgressBar.Visibility = Visibility.Visible;
                btnSubmit.IsEnabled = false;
                tbEmail.IsEnabled = false;
                pwPassword.IsEnabled = false;
                pwPasswordAgain.IsEnabled = false;
                tbEmail.Background = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0xFF, 0xFF));
                txtEmailMsg.Text = "";
                // Run...
                dcsc.DoesProfileExistAsync(tbEmail.Text);
            }
        }

        void dcsc_DoesProfileExistCompleted(object sender, DoesProfileExistCompletedEventArgs e)
        {
            performanceProgressBar.Visibility = Visibility.Collapsed;
            if (e.Error != null)
            {
                // WCF web service not available (likely an EndPointNotFoundException).
                MessageBox.Show("We can't access your profile at this time. Please check your network connectivity or wait a few moments for web services to come back online and try again.", "Web Service Unavailable", MessageBoxButton.OK);
            }
            else
            {
                bool exists = e.Result;
                if (exists)
                {
                    tbEmail.Background = new SolidColorBrush(Colors.Red);
                    txtEmailMsg.Text = "Email is already in use, please try another.";
                }
                else
                {
                    // so user can complete profile. Set also a 'new profile' flag
                    // so an Insert can be performed rather than an Update.
                    // set email and profilep
                    Email = tbEmail.Text;
                    Profilep = pwPassword.Password;
                    NavigationService.Navigate(new Uri("/MyProfile.xaml", UriKind.Relative));
                }
            }
            btnSubmit.IsEnabled = true;
            tbEmail.IsEnabled = true;
            pwPassword.IsEnabled = true;
            pwPasswordAgain.IsEnabled = true;
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
            thisApp.PasswordAgain = pwPasswordAgain.Password;
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
                pwPasswordAgain.Password = thisApp.PasswordAgain;
            }

            base.OnNavigatedTo(e);
        }
    }
}