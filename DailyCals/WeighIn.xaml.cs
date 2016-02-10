using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using DailyCals.DailyCalsWS;

namespace DailyCals
{
    public partial class WeighIn : PhoneApplicationPage
    {
        private int[] idarray;
        private DateTime startDate;
        private int id;

        public WeighIn()
        {
            InitializeComponent();
            PopulateListbox();
        }

        private void PopulateListbox()
        {
            // 1. get profile data.
            RemoteProfileRecordV2 profile = (Application.Current as DailyCals.App).ProfileDataObject;
            startDate = profile.startdate;
            bool isStandard = (profile.isstandard == 1 ? true : false);
            // 2. query the db
            WeightDB wdb = new WeightDB();
            IQueryable<WeightRecord> results = from rec in wdb.weightrecords orderby rec.date descending select rec;
            lbHistory.Items.Clear();
            idarray = new int[results.Count()];
            // 3. populate list box
            foreach (WeightRecord cr in results)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = cr.date.ToShortDateString() + ", " + cr.weight.ToString("0.0") + (isStandard ? " lbs." : " kgs.");
                lbi.FontSize = 32;
                // Don't allow start weight record to be selectable.
                if (cr.isStartWeight)
                {
                    lbi.IsEnabled = false;
                }
                lbHistory.Items.Add(lbi);
                idarray[lbHistory.Items.IndexOf(lbi)] = cr.id;
            }
            // 4. smaato ad
            //somaAdViewer.Pub = 0;       // Developer pub ID for testing
            //somaAdViewer.Adspace = 0;   // Developer adSpace ID for testing
            somaAdViewer.StartAds();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            WeightRecord rec = new WeightRecord();

            // OK to update profile?
            bool isOkToUpdate = true;

            // Validate weigh-in date.
            if (DataValidation.IsValidWeighInDate(dpDate))
            {
                rec.date = (DateTime)dpDate.Value;
                txtDateMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtDateMsg.Text = "Invalid date.";
            }

            // Validate start weight.
            if (DataValidation.IsValidWeight(tbWeight))
            {
                rec.weight = Convert.ToDouble(tbWeight.Text);
                txtWeightMsg.Text = "";
            }
            else
            {
                isOkToUpdate = false;
                txtWeightMsg.Text = "Weight must be greater than 0.";
            }

            if (isOkToUpdate)
            {
                // Data has been validated, store in db.
                WeightDB db = new WeightDB();
                rec.daysOut = Convert.ToInt32((rec.date - startDate).TotalDays);
                db.weightrecords.InsertOnSubmit(rec);
                db.SubmitChanges();
                somaAdViewer.StopAds();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void lbHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbHistory.SelectedIndex < 0)
            {
                return;
            }
            btnDelete.IsEnabled = true;
            id = idarray[lbHistory.SelectedIndex];
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            WeightDB wdb = new WeightDB();
            WeightRecord wr = (from rec in wdb.weightrecords where (rec.id == idarray[lbHistory.SelectedIndex]) select rec).First();
            wdb.weightrecords.DeleteOnSubmit(wr);
            wdb.SubmitChanges();
            // Navigate back to main page, chart will update (hitting Back won't work)
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // When we leave the page -
            // store the semi-completed log text in the application

            // Get a reference to the parent application
            App thisApp = App.Current as App;

            // Store the text in the application
            thisApp.WeighInDate = (DateTime)dpDate.Value;
            thisApp.WeighInWeight = tbWeight.Text;
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
                dpDate.Value = thisApp.WeighInDate;
                tbWeight.Text = thisApp.WeighInWeight;
            }

            base.OnNavigatedTo(e);
        }
    }
}