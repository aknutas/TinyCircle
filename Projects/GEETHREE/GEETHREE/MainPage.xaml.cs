using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;


namespace GEETHREE
{
    public partial class MainPage : PhoneApplicationPage
    {
        Controller ctrl;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ctrl = Controller.Instance;
            ctrl.registerAvatarUpdates(this);

            refreshAvatar();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            //txt_Base_Alias.Text = appSetting.AliasSetting;
           
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

        }

        public void refreshAvatar()
        {
            img_Base_Avatar.Source = ctrl.getCurrentAvatar();
            txt_Base_Alias.Text = ctrl.getCurrentAlias();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            string parameter = "toPeople";
            NavigationService.Navigate(new Uri(string.Format("/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
            //NavigationService.Navigate(new Uri("/SocietyPivot.xaml", UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string parameter = "toGroups";
            NavigationService.Navigate(new Uri(string.Format("/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
            //NavigationService.Navigate(new Uri("/SocietyPivot.xaml", UriKind.Relative));
        }

        
       

        

        private void txt_Base_Settings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
        }

    }
}