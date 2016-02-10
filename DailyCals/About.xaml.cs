using System;
using Microsoft.Phone.Controls;
using System.Reflection;

namespace DailyCals
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            Display();
        }

        public void Display()
        {
            string str = Assembly.GetExecutingAssembly().FullName;
            txtVersion.Text = "version " + str.Split('=')[1].Split(',')[0];
        }

        private void txtDisclaimer_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Demo.xaml", UriKind.Relative));
        }

    }
}