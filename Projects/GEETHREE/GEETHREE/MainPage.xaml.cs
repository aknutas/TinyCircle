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



namespace GEETHREE
{
    public partial class MainPage : PhoneApplicationPage
    {
        Controller ctrl;
        bool createUID = false;
        //bool serverMessageReceived = false;
        
       
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
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            //txt_Base_Alias.Text = appSetting.AliasSetting;

            if (ctrl.getCurrentUserID() == "0000")
            {
                
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];

                UserIDCreateCanvas.Background= backgroundbrush;
                UserIDCreateCanvas.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = false;
                createUID = true;
            }
            else
                ctrl.cm.Join(ctrl.getCurrentUserID());
            System.Diagnostics.Debug.WriteLine("Mainpage constructed");
        }

        // ** Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }


            updateWifiAndServerStatuses();


            System.Diagnostics.Debug.WriteLine("Mainpage loaded");

        }

        public void refreshAvatar()
        {
            img_Base_Avatar.Source = ctrl.getCurrentAvatar();
            PanoramaItemAlias.Header = ctrl.getCurrentAlias();
        }

        // ** When user clicks menu bar buttons

        private void appbar_settings_Click(object sender, EventArgs e)
        {
            // ** Go to settings
            NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
        }

        private void appbar_messages_Click(object sender, EventArgs e)
        {
            // ** go to messages      
            NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));
        }

        private void appbar_compose_Click(object sender, EventArgs e)
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

        // ** these were just for testing purposes
        private void menuItem1_Click(object sender, EventArgs e)
        {
            User u = new User("Anni", "This is Anni");
            u.UserID = "555";
            ctrl.dm.storeNewUser(u);

            u = new User("Thomster", "This is Tommi.");
            u.UserID = "556";
            ctrl.dm.storeNewUser(u);

            u = new User("Bishal", "This is Bishal.");
            u.UserID = "557";
            ctrl.dm.storeNewUser(u);

            u = new User("Antti", "This is antti.");
            u.UserID = "558";
            ctrl.dm.storeNewUser(u);

            u = new User("Tommi K", "This is Tommi K.");
            u.UserID = "559";
            ctrl.dm.storeNewUser(u);

            GroupInfoResponse gri = new GroupInfoResponse();
            gri.GroupName = "G1";
            gri.GroupID = "111";
            gri.Description = "dasdsada";
            gri.SenderAlias = "dasdad";
            gri.SenderID = "SDA";
            gri.ReceiverID = "SADAD";
            ctrl.dm.storeNewGroupInfoResponse(gri);

            gri = new GroupInfoResponse();
            gri.GroupName = "G2";
            gri.GroupID = "112";
            gri.Description = "dasdsada222";
            gri.SenderAlias = "dasdad";
            gri.SenderID = "SDA";
            gri.ReceiverID = "SADAD";
            ctrl.dm.storeNewGroupInfoResponse(gri);

            UserInfoResponse uri = new UserInfoResponse();
            uri.UserID = "115";
            uri.UserAlias = "Samantha";
            uri.Description = "the foxy gal";
            ctrl.dm.storeNewUserInfoResponse(uri);

            uri = new UserInfoResponse();
            uri.UserID = "185";
            uri.UserAlias = "anjelina";
            uri.Description = "the sexy gal";
            ctrl.dm.storeNewUserInfoResponse(uri);

            uri = new UserInfoResponse();
            uri.UserID = "556";
            uri.UserAlias = "bomber";
            uri.Description = "the sexy gal";
            ctrl.dm.storeNewUserInfoResponse(uri);

            Group g = new Group();
            g.GroupName = ".NET Codecamp";
            g.GroupID = "1234";
            g.Description = "Here we are still coding at 0:55.";
            ctrl.dm.storeNewGroup(g);

            g = new Group();
            g.GroupName = "Comlab";
            g.GroupID = "1235";
            g.Description = "We are Comlab, the one and the only one.";
            ctrl.dm.storeNewGroup(g);

            g = new Group();
            g.GroupName = "SWE";
            g.GroupID = "1236";
            g.Description = "We are from Sweden";
            ctrl.dm.storeNewGroup(g);

            App.ViewModel.LoadData();
            //Old debug code
            /*
            DataClasses.Message msg = new DataClasses.Message();
            msg.TextContent = "Pli";
            msg.outgoing = true;
            ctrl.dm.storeNewMessage(msg);

            DataClasses.Message msg2 = new DataClasses.Message();
            msg2.TextContent = "Pla";
            msg2.outgoing = true;
            ctrl.dm.storeNewMessage(msg2);

            List<DataClasses.Message> msgList = ctrl.dm.getAllMessages();
            System.Diagnostics.Debug.WriteLine(msgList.Count);*/
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            ctrl.dm.resetDataBase();
            App.ViewModel.LoadData();
            //DeviceNetworkInformation;
            //NetworkInterfaceInfo netInterfaceInfo = socket.GetCurrentNetworkInterface();
            //var type = netInterfaceInfo.InterfaceType;
            //var subType = netInterfaceInfo.InterfaceSubtype; 
            //MessageBox.Show(NetworkInterface.NetworkInterfaceType.ToString()); 


            //Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            //if (net == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211)
            //{
            //   img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
            //}
            //if (net != Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211)
            //{
            //    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));
            //}


            //if (NetworkInterface.NetworkInterfaceType.ToString() == "Wireless80211")
            //    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
            //else
            //    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));
        }

        private void btn_CreateUserID_OK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            ctrl.changeCurrentUserID(ctrl.CreateNewUserID());
            CreateUserIDTextBlock.Text = "UserID : ";
            CreateUserIDContent.Text = ctrl.getCurrentUserID();
            btn_CreateUserID_OK.Visibility = Visibility.Collapsed;
            btn_CreateUserID_Cancel.Visibility = Visibility.Collapsed;
            btn_CreateUserID_Done.Content = "Done";            
            btn_CreateUserID_Done.Visibility = Visibility.Visible;

            ctrl.cm.Join(ctrl.getCurrentUserID());
        }

        private void btn_CreateUserID_Cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CreateUserIDTextBlock.Text = "Exiting!!!";
            CreateUserIDContent.Text ="UserID not created! \nTinyCircle Exits now. You can always come back and create UserID! \n\nThank You!!!";
            btn_CreateUserID_OK.Visibility = Visibility.Collapsed;
            btn_CreateUserID_Cancel.Visibility = Visibility.Collapsed;
            btn_CreateUserID_Done.Content = "Ok";
            createUID = false;
            btn_CreateUserID_Done.Visibility = Visibility.Visible;
        }

        private void btn_CreateUserID_Done_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UserIDCreateCanvas.Visibility = Visibility.Collapsed;
            if (createUID == false)
            {
                NavigationService.GoBack();
            }
            else
            {
                ApplicationBar.IsVisible = true;
                createUID = true;
                //if (serverMessageReceived == true) // ** now display the server message
                //{
                //    serverMessageReceived = false;
                //    this.messageArrived(true);
                //}
            }
        }
        // ** When navigated back to main page
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // ** check the wifi status
            if (DeviceNetworkInformation.IsWiFiEnabled)
                img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
            else
                img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));
            
            if (ctrl.mh.ConnectedToServer == true)
                img_Base_Server.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/server.green.png", UriKind.Relative));
            else
                img_Base_Server.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/server.red.png", UriKind.Relative));

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
                    mainpanorama.DefaultItem = society;
                }
            }
            catch
            {

            }
        }

        // ** some kind of popup needed to announce about the message that is just arrived
        public void messageArrived(bool isPrivate)
        {

            System.Diagnostics.Debug.WriteLine("MEssage arrived");
            //if (UserIDCreateCanvas.Visibility == Visibility.Visible) //  // ** don't discplay the (server) message yet, if visible, the user is creating ID
            //{
            //    serverMessageReceived = true;
            //}
            //else
            //{
                var m = MessageBox.Show("Read it?", "You have received a message.", MessageBoxButton.OKCancel);

                if (m == MessageBoxResult.OK)
                {
                    if (isPrivate == true) // navigate to Messages - whispers
                        NavigationService.Navigate(new Uri(string.Format("/Pages/MessagePage.xaml?parameter={0}", "messages_whispers"), UriKind.Relative));
                    else // navigate to messages - shouts
                        NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", "messages_shouts"), UriKind.Relative));
                }
            //}
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HelpPage.xaml", UriKind.Relative));
        }

        private void img_Base_Wifi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            

                ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
                connectionSettingsTask.Show();
                // ** check the wifi status
                if (DeviceNetworkInformation.IsWiFiEnabled)
                    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
                else
                    img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));

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
                NavigationService.GoBack();

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
                img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.green.png", UriKind.Relative));
            else
                img_Base_Wifi.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/wifi.red.png", UriKind.Relative));

            if (ctrl.mh.ConnectedToServer == true)
                img_Base_Server.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/server.green.png", UriKind.Relative));
            else
                img_Base_Server.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/server.red.png", UriKind.Relative));

            // ctrl.mh.LocalConnections
        
        }
    }
}