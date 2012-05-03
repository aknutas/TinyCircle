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
using GEETHREE.DataClasses;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Coding4Fun.Phone.Controls;



namespace GEETHREE
{
    public partial class MainPage : PhoneApplicationPage, GEETHREE.Pages.AvatarChangeListener
    {
        Controller ctrl;
        private bool arrivedMessageIsPrivate = false;
        bool createUID = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            ctrl = Controller.Instance;
            ctrl.registerAvatarUpdates(this);
            ctrl.registerCurrentPage(this, "main");
         

            refreshAvatar();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            //this.Loaded += new RoutedEventHandler(MainPage_Loaded); // removed this line
            //txt_Base_Alias.Text = appSetting.AliasSetting;

            if (ctrl.getCurrentUserID() == "0000")
            {
                
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];

                UserIDCreateCanvas.Background= backgroundbrush;
                UserIDCreateCanvas.Visibility = Visibility.Visible;
                //ApplicationBar.IsVisible = false;
                createUID = true;
            }
            else
                ctrl.cm.Join(ctrl.getCurrentUserID());

            //ctrl.SetThemeColors();
            System.Diagnostics.Debug.WriteLine("Mainpage constructed");
        }

        // ** Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }


            updateMySocietyNumbers();
            updateWifiAndServerStatuses();

            

            System.Diagnostics.Debug.WriteLine("Mainpage loaded");



        }

        public void refreshAvatar()
        {
            img_Base_Avatar.Source = ctrl.getCurrentAvatar();
            //PanoramaItemAlias.Header = ctrl.getCurrentAlias();
            if (ctrl.getCurrentAlias() != "Alias" && ctrl.getCurrentAlias() != "Anonymous")
                txt_alias_Name.Text = ctrl.getCurrentAlias();
            else
                txt_alias_Name.Text = "Anonymous";
            
        }

        // ** When user clicks menu bar buttons

        private void appbar_settings_Click(object sender, EventArgs e)
        {
            if (UserIDCreateCanvas.Visibility == Visibility.Collapsed)
            {
                // ** Go to settings
                NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
            }
        }

        private void appbar_messages_Click(object sender, EventArgs e)
        {
            if (UserIDCreateCanvas.Visibility == Visibility.Collapsed)
            {
                // ** go to messages   
                App.ViewModel.refreshDataAsync();
                NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));
            }
        }

        private void appbar_compose_Click(object sender, EventArgs e)
        {
            if (UserIDCreateCanvas.Visibility == Visibility.Collapsed)
            {
                // ** ask the controller to register this page as a previous page before going to compose page
                // ** also provide the name of current pivont as a string, so we can navigate back to the same pivot
                if (mainpanorama.SelectedItem == shouts)
                    ctrl.registerPreviousPage(this, "main_shouts");
                else if (mainpanorama.SelectedItem == PanoramaItemAlias)
                    ctrl.registerPreviousPage(this, "main_alias");
                else if (mainpanorama.SelectedItem == society)
                    ctrl.registerPreviousPage(this, "main_society");
                // ** go to compose page
                NavigationService.Navigate(new Uri("/Pages/ComposeMessagePage.xaml", UriKind.Relative));
            }
        }

        //private void txt_Base_Messages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));
        //}

        private void button3_Click(object sender, RoutedEventArgs e)
        {
           
                NavigationService.Navigate(new Uri("/Pages/ComposeMessagePage.xaml", UriKind.Relative));
        }



        private void txt_mySociety_MySociety_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string parameter = "toPeople";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }

        private void txt_mySociety_People_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string parameter = "toPeople";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }

        private void txt_mySociety_Groups_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string parameter = "toGroups";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }

        private void txt_mySociety_morePeopleGroups_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            string parameter = "toPeople";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }

        private void txt_messages_Header_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));
        }

        //private void txt_Base_Help_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/Pages/HelpPage.xaml", UriKind.Relative));
        //}

        private void btn_CreateUserID_OK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            ctrl.changeCurrentUserID(ctrl.CreateNewUserID());
            AboutPrompt p = new AboutPrompt();
            p.Title = "TinyCircle!!";
            p.Body = new UserIDCreateBodyOKCancel(createUID);
            p.Completed += about_Completed;
            p.VersionNumber = "v1.6";
            p.Show();
        }

        private void btn_CreateUserID_Cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            createUID = false;
            AboutPrompt p = new AboutPrompt();
            p.Title = "TinyCircle Exiting!!!";
            p.Body = new UserIDCreateBodyOKCancel(createUID);
            p.Completed += about_Completed;
            p.VersionNumber = "v1.6";
            p.Show(); 
        }


        void about_Completed(object sender, EventArgs e)
        {
             
             UserIDCreateCanvas.Visibility = Visibility.Collapsed;
             if (createUID == false)
             {
                 NavigationService.GoBack();
             }
             else
             {
                 
                 
                 ctrl.cm.Join(ctrl.getCurrentUserID());
                
             }
        }
        private void btn_CreateUserID_Done_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           
        }
        // ** When navigated back to main page
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            updateWifiAndServerStatuses();
            try
            {
                string newparameter = this.NavigationContext.QueryString["parameter"];
                if (newparameter.Equals("main_shouts"))
                {
                    mainpanorama.DefaultItem = shouts;
                }
                else if (newparameter.Equals("main_alias"))
                {
                    mainpanorama.DefaultItem = PanoramaItemAlias;
                }
                else if (newparameter.Equals("main_society"))
                {
                    updateMySocietyNumbers();


                    mainpanorama.DefaultItem = society;
                }

            }
            catch
            {

            }
        }

        public void updateMySocietyNumbers()
        {
            int iNumOfFriends = App.ViewModel.Users.Count;
            int iNumOfGroups = App.ViewModel.Groups.Count;
            int iNumofTags = App.ViewModel.Tagss.Count;

            if (iNumOfFriends > 0)
                numOfFriends.Text = iNumOfFriends.ToString();
            if (iNumOfGroups > 0)
                numOfGroups.Text = iNumOfGroups.ToString();
            if (iNumofTags > 0)
                numofTags.Text = iNumofTags.ToString(); ;
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            if (UserIDCreateCanvas.Visibility == Visibility.Collapsed)
            {
                NavigationService.Navigate(new Uri("/Pages/HelpPage.xaml", UriKind.Relative));
            }
        }

        private void img_Base_Wifi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            

                ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
                connectionSettingsTask.Show();
                // ** check the wifi status

                updateWifiAndServerStatuses();
                /*
                if (DeviceNetworkInformation.IsWiFiEnabled)
                    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
                else
                    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));
             */

        }

        private void img_Base_Server_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void img_Base_AP_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;

            if (NavigationService.CanGoBack)
            {
                
                while (NavigationService.RemoveBackEntry() != null)
                {
                    NavigationService.RemoveBackEntry();
                }
                try
                {
                    NavigationService.GoBack();
                }
                catch { }

            }
            
        }

        private void img_Base_Avatar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // ** Go to settings
            NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
        }

        private void mainpanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateWifiAndServerStatuses();
        }
        public void updateWifiAndServerStatuses()

        {

            // ** check the wifi status
            if (DeviceNetworkInformation.IsWiFiEnabled)
            {
                img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
                wifiStatusTextBlock.Text = "Wifi status: ON";
            }
            else
            {
                img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));
                wifiStatusTextBlock.Text = "Wifi status: OFF";
            }

            if (ctrl.mh.ConnectedToServer == true)
            {
                img_Base_Server.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/server.green.png", UriKind.Relative));
                serverStatusTextBlock.Text = "Server status: ON";
            }

            else
            {
                img_Base_Server.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/server.red.png", UriKind.Relative));
                serverStatusTextBlock.Text = "Server status: OFF";
            }

            if (ctrl.mh.LocalConnections == 0)
            {
                img_Base_ConnectionStatus.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/Empty.png", UriKind.Relative));
                txt_connection.Text = "         No   People Nearby";
            }
            else if (ctrl.mh.LocalConnections > 0 && ctrl.mh.LocalConnections <= 1)
            {
                img_Base_ConnectionStatus.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/few.png", UriKind.Relative));
                txt_connection.Text = "         Few   People Nearby";
            }
            else if (ctrl.mh.LocalConnections >= 2)
            {
                img_Base_ConnectionStatus.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/many.png", UriKind.Relative));
                txt_connection.Text = "        Many   People Nearby";
            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //ctrl.CreateApplicationTile();
            
            
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Mainpage Unloaded");
           
        }

        private void txt_mySociety_Tags_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string parameter = "toTags";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }

        private void ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // ** Go to shouts page

            string parameter = "messages_shouts";
            NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", parameter),  UriKind.Relative));
        }

        // ** toast announces about the message that is just arrived
        public void messageArrived(bool isPrivate)
        {
            ToastPrompt tp = new ToastPrompt();

            if (isPrivate)
            {
                arrivedMessageIsPrivate = true;
                tp.Title = "You have a new whisper.";
            }
            else
            {
                arrivedMessageIsPrivate = false;
                tp.Title = "You have a new  shout.";
            }
            tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
            tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
            tp.Tap += toast_Tap;
            tp.Show();
        }
        void toast_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (arrivedMessageIsPrivate)
            {
                string parameter = "messages_whispers";
                NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", parameter), UriKind.Relative));
            }
            else
            {
                string parameter = "messages_shouts";
                NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", parameter), UriKind.Relative));
            }
        }

        // ** toast announces about user info that arrived
        public void friendInfoArrived(string alias)
        {
            string title;
            ToastPrompt tp = new ToastPrompt();
            if (alias != "0")
            {
                title = "Friend information saved ";
            }
            else
            {
                title = "Friend information not found.\n Check used id and password";
            }
            tp.Title = title;

            tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
            tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
            tp.Tap += friend_Toast_Tap;
            tp.Show();
        }
        void friend_Toast_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //What to do now? Navigate to friendlist?
            string parameter = "PeoplePivot";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }


    }
}