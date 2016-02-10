using System;
using System.Windows;
using Microsoft.Phone.Controls;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public partial class MyProfile : PhoneApplicationPage
    {
        private RemoteProfileRecordV2 _profile;

        public MyProfile()
        {
            InitializeComponent();
            PopulateForm();
        }

        private void PopulateForm()
        {
            // Get the application's profile data object.
            _profile = (Application.Current as DailyCals.App).ProfileDataObject;

            // Empty message text boxes.
            txtBirthdateMsg.Text = "";
            txtHeightMsg.Text = "";
            txtStartWeightMsg.Text = "";
            txtGoalWeightMsg.Text = "";
            txtGoalDateMsg.Text = "";

            if (_profile == null)
            {
                // user has not created his or her profile yet.
                return;
            }

            // Birthdate.
            dpBirthdate.Value = _profile.birthdate;

            // Height
            tbHeight.Text = Convert.ToString(_profile.height);

            // Female or male?
            if (_profile.isfemale == 1)
            {
                rbFemale.IsChecked = true;
                rbMale.IsChecked = false;
            }
            else
            {
                rbFemale.IsChecked = false;
                rbMale.IsChecked = true;
            }

            // Start and goal weights.
            tbStartWeight.Text = Convert.ToString(_profile.startweight);
            tbGoalWeight.Text = Convert.ToString(_profile.goalweight);

            // Standard or Metric units?
            if (_profile.isstandard == 1)
            {
                rbStandard.IsChecked = true;
                rbMetric.IsChecked = false;
                txtHeight.Text = "Height (in)";
                txtStart.Text = "Start Weight (lbs)";
                txtGoal.Text = "Goal Weight (lbs)";
            }
            else
            {
                rbStandard.IsChecked = false;
                rbMetric.IsChecked = true;
                txtHeight.Text = "Height (cm)";
                txtStart.Text = "Start Weight (kgs)";
                txtGoal.Text = "Goal Weight (kgs)";
            }

            // Goal date.
            dpGoalDate.Value = _profile.goaldate;

            // Activity level.
            slActivityLevel.Value = _profile.activitylevel;
        }

        private void slActivityLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Return from method if text box hasn't been drawn yet.
            if (txtActivityLevelMsg == null)
            {
                return;
            }

            // Convert activity level and display an informative label.
            switch (DailyCalories.ConvertActivityLevel(slActivityLevel.Value))
            {
                case 0:
                    txtActivityLevelMsg.Text = "(0: None)";
                    break;
                case 1:
                    txtActivityLevelMsg.Text = "(1: Normal)";
                    break;
                case 2:
                    txtActivityLevelMsg.Text = "(2: Moderate)";
                    break;
                case 3:
                    txtActivityLevelMsg.Text = "(3: Extreme)";
                    break;
            }
        }

        private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            // Get the application's profile data object.
            // TODO: need to consolidate the profile obj in PopulteForm
            //_profile = (Application.Current as DailyCals.App).ProfileDataObject;

            // Is this profile new?
            bool isNewProfile = false;

            // OK to update profile?
            bool isOkToUpdate = true;

            // Dirty bit.
            bool dirty_bit = false;

            // Date used for total days calculation.
            DateTime startDate = DailyCalories.GetToday();

            if (_profile == null)
            {
                // user has not created his or her profile yet.
                // create new record, and set flag.
                _profile = new RemoteProfileRecordV2();
                isNewProfile = true;
            }
            else
            {
                // Since there exists a profile record, use the setup
                // date as the dateForCalculations.
                startDate = _profile.startdate;
            }

            // Birthdate validation.
            if (DataValidation.IsValidBirthdate(dpBirthdate))
            {
                DateTime birthdate = (DateTime)dpBirthdate.Value;
                if (_profile.birthdate != birthdate)
                {
                    _profile.birthdate = birthdate;
                    dirty_bit = true;
                }
                txtBirthdateMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtBirthdateMsg.Text = "Chk birthdate.";
            }

            // Validate height.
            if (DataValidation.IsValidHeight(tbHeight))
            {
                double height = Convert.ToDouble(tbHeight.Text);
                if (_profile.height != height)
                {
                    _profile.height = height;
                    dirty_bit = true;
                }
                txtHeightMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtHeightMsg.Text = "Chk height.";
            }

            // Is user female?
            if ((bool)rbFemale.IsChecked)
            {
                if (_profile.isfemale == 0)
                {
                    _profile.isfemale = 1;
                    dirty_bit = true;
                }
            }
            else
            {
                if (_profile.isfemale == 1)
                {
                    _profile.isfemale = 0;
                    dirty_bit = true;
                }
            }

            // Validate start weight.
            if (DataValidation.IsValidWeight(tbStartWeight))
            {
                double startweight = Convert.ToDouble(tbStartWeight.Text);
                if (_profile.startweight != startweight)
                {
                    _profile.startweight = startweight;
                    dirty_bit = true;
                }
                txtStartWeightMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtStartWeightMsg.Text = "Weight must be greater than 0.";
            }

            // Validate goal weight.
            if (DataValidation.IsValidWeight(tbGoalWeight))
            {
                double goalweight = Convert.ToDouble(tbGoalWeight.Text);
                if (_profile.goalweight != goalweight)
                {
                    _profile.goalweight = goalweight;
                    dirty_bit = true;
                }
                txtGoalWeightMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtGoalWeightMsg.Text = "Weight must be greater than 0.";
            }

            // Validate start weight > goal weight. This happens only
            // after both weights have already been validated first.
            if (isOkToUpdate)
            {
                if (DataValidation.IsStartWeightGreaterThanGoalWeight(tbStartWeight, tbGoalWeight))
                {
                    txtGoalWeightMsg.Text = "";
                }
                else
                {
                    isOkToUpdate = false;
                    txtStartWeightMsg.Text = "Start weight must be larger than goal.";
                }
            }

            // Standard units?
            if ((bool)rbStandard.IsChecked)
            {
                if (_profile.isstandard == 0)
                {
                    _profile.isstandard = 1;
                    dirty_bit = true;
                }
            }
            else
            {
                if (_profile.isstandard == 1)
                {
                    _profile.isstandard = 0;
                    dirty_bit = true;
                }
            }

            // Validate goal date.
            if (DataValidation.IsValidGoalDate(dpGoalDate))
            {
                DateTime goaldate = (DateTime)dpGoalDate.Value;
                if (_profile.goaldate != goaldate)
                {
                    _profile.goaldate = goaldate;
                    dirty_bit = true;
                }
                txtGoalDateMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtGoalDateMsg.Text = "Goal date must be later than today.";
            }

            // Activity level.
            if (_profile.activitylevel != slActivityLevel.Value)
            {
                _profile.activitylevel = slActivityLevel.Value;
                dirty_bit = true;
            }

            // If everything is OK to update, then update the profile.
            // Otherwise, return the user to the My Profile page.
            if (isOkToUpdate && dirty_bit)
            {
                // If this is a new profile, set the setup date.
                if (isNewProfile)
                {
                    _profile.startdate = DailyCalories.GetToday();
                    _profile.email = CreateProfile.Email;
                    _profile.profilep = CreateProfile.Profilep;
                }

                // Save profile data to db.
                DailyCalsServiceV2Client dcsc = new DailyCalsServiceV2Client();
                // Set progress bar, and clear some stuff.
                performanceProgressBar.Visibility = Visibility.Visible;
                if (isNewProfile)
                {
                    dcsc.InsertProfileCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(dcsc_InsertProfileCompleted);
                    dcsc.InsertProfileAsync(_profile);
                }
                else
                {
                    dcsc.UpdateProfileCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(dcsc_UpdateProfileCompleted);
                    dcsc.UpdateProfileAsync(_profile);
                }
            }
            else if (isOkToUpdate && !dirty_bit)
            {
                // dirty it not set, just go back to main page
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void UpdateProfileDataObject()
        {
            // Set the application's profile data object.
            (Application.Current as DailyCals.App).ProfileDataObject = _profile;
            // Save profile data to iso storage so app doesn't have to come to login anymore.
            bool result = (Application.Current as DailyCals.App).SaveProfileDataToIsolatedStorage();
            //
            DailyCalories.UpdateWeightDB(_profile);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        void dcsc_UpdateProfileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            performanceProgressBar.Visibility = Visibility.Collapsed;
            if (e.Error != null)
            {
                // WCF web service not available (likely an EndPointNotFoundException).
                MessageBox.Show("We can't update your profile at this time. Please check your network connectivity or wait a few moments for web services to come back online and try again.", "Web Service Unavailable", MessageBoxButton.OK);
            }
            else
            {
                UpdateProfileDataObject();
            }
        }

        void dcsc_InsertProfileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            performanceProgressBar.Visibility = Visibility.Collapsed;
            if (e.Error != null)
            {
                // WCF web service not available (likely an EndPointNotFoundException).
                MessageBox.Show("We can't add your profile at this time. Please check your network connectivity or wait a few moments for web services to come back online and try again.", "Web Service Unavailable", MessageBoxButton.OK);
            }
            else
            {
                UpdateProfileDataObject();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // When we leave the page -
            // store the semi-completed log text in the application

            // Get a reference to the parent application
            App thisApp = App.Current as App;

            // Store the text in the application
            thisApp.BirthDate = (DateTime)dpBirthdate.Value;
            thisApp.Height = tbHeight.Text;
            thisApp.StartWeight = tbStartWeight.Text;
            thisApp.GoalWeight = tbGoalWeight.Text;
            thisApp.GoalDate = (DateTime)dpGoalDate.Value;
            thisApp.IsFemale = (bool)rbFemale.IsChecked;
            thisApp.ActivityLevel = slActivityLevel.Value;
            thisApp.IsStandard = (bool)rbStandard.IsChecked;
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
                dpBirthdate.Value = thisApp.BirthDate;
                tbHeight.Text = thisApp.Height;
                tbStartWeight.Text = thisApp.StartWeight;
                tbGoalWeight.Text = thisApp.GoalWeight;
                dpGoalDate.Value = thisApp.GoalDate;
                rbFemale.IsChecked = thisApp.IsFemale;
                slActivityLevel.Value = thisApp.ActivityLevel;
                rbStandard.IsChecked = thisApp.IsStandard;
            }

            base.OnNavigatedTo(e);
        }

        private void rbStandard_Checked(object sender, RoutedEventArgs e)
        {
            if (txtHeight != null)
            {
                txtHeight.Text = "Height (in)";
            }
            if (txtStart != null)
            {
                txtStart.Text = "Start Weight (lbs)";
            }
            if (txtGoal != null)
            {
                txtGoal.Text = "Goal Weight (lbs)";
            }
        }

        private void rbStandard_Unchecked(object sender, RoutedEventArgs e)
        {
            if (txtHeight != null)
            {
                txtHeight.Text = "Height (cm)";
            }
            if (txtStart != null)
            {
                txtStart.Text = "Start Weight (kgs)";
            }
            if (txtGoal != null)
            {
                txtGoal.Text = "Goal Weight (kgs)";
            }
        }
    }
}