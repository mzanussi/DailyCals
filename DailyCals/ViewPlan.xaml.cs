using System.Linq;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace DailyCals
{
    public partial class ViewPlan : PhoneApplicationPage
    {
        private int[] idarray;

        public ViewPlan()
        {
            InitializeComponent();
            PopulateListbox();
        }

        private void PopulateListbox()
        {
            // Grab the baseline data.
            BaselineDataForChart bdfc = new BaselineDataForChart();
            // Clear the listbox.
            lbPlan.Items.Clear();
            // Instantiate the idarray.
            idarray = new int[bdfc.Count()];
            // Populate list box.
            foreach (WeightRecord cr in bdfc)
            {
                ListBoxItem lbi = new ListBoxItem();
                if (cr.week == 0)
                {
                    lbi.Content = "Goal weight of " + cr.weight.ToString("0.0") + " reached!";
                }
                else
                {
                    lbi.Content = "Week " + cr.week.ToString() + " start weight is " + cr.weight.ToString("0.0");
                }
                lbi.FontSize = 28;
                lbPlan.Items.Add(lbi);
                idarray[lbPlan.Items.IndexOf(lbi)] = cr.id;
            }
        }

        private void lbPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbPlan.SelectedIndex < 0)
            {
                return;
            }
            // Grab the baseline data.
            BaselineDataForChart bdfc = new BaselineDataForChart();
            // Get the record.
            WeightRecord wr = bdfc.ElementAt(idarray[lbPlan.SelectedIndex]);
            // populate controls
            if (wr.week == 0)
            {
                txtWeightMsg.Text = "By your goal date on " + wr.date.ToShortDateString() + " your expected weight should be:";
                txtWeight.Text = wr.weight.ToString("0.0");
                txtDailyCals.Text = "";
                txtDailyCalsMsg.Text = "";
            }
            else
            {
                txtWeightMsg.Text = "Your expected weight at the start of week " + wr.week.ToString() + " on " + wr.date.ToShortDateString() + " should be:";
                txtWeight.Text = wr.weight.ToString("0.0");
                txtDailyCalsMsg.Text = "The maximum daily calories to meet the next week's goal weight will be:";
                txtDailyCals.Text = wr.dailycals.ToString("0");
            }
        }

    }
}