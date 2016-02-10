using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using DailyCals.DailyCalsWS;
using System.Runtime.Serialization;

namespace DailyCals
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // MBZ: iso storage load if under 10 seconds
            ReadProfileDataFromIsolatedStorage();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                // MBZ: returning from dormant state, no need to restore state.
                // do so at your own peril and app speed considerations.
                WasTombstoned = false;
            }
            else
            {
                // MBZ: returning from a tombstoning, restore state obj.
                WasTombstoned = true;
                LoadFromStateObject();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // MBZ: iso storage save and/or state obj save
            SaveToStateObject();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // MBZ: iso storage save (state obj save not necessary as app is ending)
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // Setting e.Handled to true will prevent app from exiting due to unavailable
            // web services.
            // This will handle web service EndPointNotFoundExceptions, sort of brute force
            // method but would rather not have to add try..catch to System.IAsyncResult calls
            // in Reference.cs.
            e.Handled = true;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        public static bool WasTombstoned { get; private set; }

        // form fields to save state
        public string Email;
        public string Password;
        public string PasswordAgain;
        public DateTime BirthDate;
        public string Height;
        public string StartWeight;
        public string GoalWeight;
        public DateTime GoalDate;
        public bool IsFemale;
        public double ActivityLevel;
        public DateTime WeighInDate;
        public string WeighInWeight;
        public bool IsStandard;

        private RemoteProfileRecordV2 _profile;

        public RemoteProfileRecordV2 ProfileDataObject
        {
            get { return _profile; }
            set { _profile = value; }
        }

        public bool SaveProfileDataToIsolatedStorage()
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("profile.iso", FileMode.Create, isoStore))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(RemoteProfileRecordV2));
                    dcs.WriteObject(stream, ProfileDataObject);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ReadProfileDataFromIsolatedStorage()
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("profile.iso", FileMode.OpenOrCreate, isoStore))
                {
                    if (stream.Length > 0)
                    {
                        DataContractSerializer dcs = new DataContractSerializer(typeof(RemoteProfileRecordV2));
                        ProfileDataObject = dcs.ReadObject(stream) as RemoteProfileRecordV2;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SaveToStateObject()
        {
            SaveTextToStateObject("Email", Email);
            SaveTextToStateObject("Password", Password);
            SaveTextToStateObject("PasswordAgain", PasswordAgain);
            SaveDateToStateObject("BirthDate", BirthDate);
            SaveTextToStateObject("Height", Height);
            SaveTextToStateObject("StartWeight", StartWeight);
            SaveTextToStateObject("GoalWeight", GoalWeight);
            SaveDateToStateObject("GoalDate", GoalDate);
            SaveBoolToStateObject("IsFemale", IsFemale);
            SaveDoubleToStateObject("ActivityLevel", ActivityLevel);
            SaveDateToStateObject("WeighInDate", WeighInDate);
            SaveTextToStateObject("WeighInWeight", WeighInWeight);
            SaveProfileToStateObject("Profile", _profile);
            SaveBoolToStateObject("IsStandard", IsStandard);
        }

        private void LoadFromStateObject()
        {
            LoadTextFromStateObject("Email", out Email);
            LoadTextFromStateObject("Password", out Password);
            LoadTextFromStateObject("PasswordAgain", out PasswordAgain);
            LoadDateFromStateObject("BirthDate", out BirthDate);
            LoadTextFromStateObject("Height", out Height);
            LoadTextFromStateObject("StartWeight", out StartWeight);
            LoadTextFromStateObject("GoalWeight", out GoalWeight);
            LoadDateFromStateObject("GoalDate", out GoalDate);
            LoadBoolFromStateObject("IsFemale", out IsFemale);
            LoadDoubleFromStateObject("ActivityLevel", out ActivityLevel);
            LoadDateFromStateObject("WeighInDate", out WeighInDate);
            LoadTextFromStateObject("WeighInWeight", out WeighInWeight);
            LoadProfileFromStateObject("Profile", out _profile);
            LoadBoolFromStateObject("IsStandard", out IsStandard);
        }

        private void SaveTextToStateObject(string filename, string text)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            stateStore.Remove(filename);
            stateStore.Add(filename, text);
        }

        private void SaveDateToStateObject(string filename, DateTime dt)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            stateStore.Remove(filename);
            stateStore.Add(filename, dt);
        }

        private void SaveDoubleToStateObject(string filename, double d)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            stateStore.Remove(filename);
            stateStore.Add(filename, d);
        }

        private void SaveBoolToStateObject(string filename, bool b)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            stateStore.Remove(filename);
            stateStore.Add(filename, b);
        }

        private void SaveProfileToStateObject(string filename, RemoteProfileRecordV2 profile)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            stateStore.Remove(filename);
            stateStore.Add(filename, profile);
        }

        private bool LoadTextFromStateObject(string filename, out string result)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            result = "";
            if (!stateStore.ContainsKey(filename))
            {
                return false;
            }
            result = (string)stateStore[filename];
            return true;
        }

        private bool LoadDateFromStateObject(string filename, out DateTime result)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            result = DateTime.Today;
            if (!stateStore.ContainsKey(filename))
            {
                return false;
            }
            result = (DateTime)stateStore[filename];
            return true;
        }

        private bool LoadDoubleFromStateObject(string filename, out double result)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            result = 0;
            if (!stateStore.ContainsKey(filename))
            {
                return false;
            }
            result = (double)stateStore[filename];
            return true;
        }

        private bool LoadBoolFromStateObject(string filename, out bool result)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            result = false;
            if (!stateStore.ContainsKey(filename))
            {
                return false;
            }
            result = (bool)stateStore[filename];
            return true;
        }

        private bool LoadProfileFromStateObject(string filename, out RemoteProfileRecordV2 result)
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;
            result = null;
            if (!stateStore.ContainsKey(filename))
            {
                return false;
            }
            result = (RemoteProfileRecordV2)stateStore[filename];
            return true;
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}