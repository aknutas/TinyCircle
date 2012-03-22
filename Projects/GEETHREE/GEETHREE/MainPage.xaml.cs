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
            ctrl.registerCurrentPage(this, "main");
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
            //messageArrived();
        }

        public void refreshAvatar()
        {
            img_Base_Avatar.Source = ctrl.getCurrentAvatar();
            //txt_Base_Alias.Text = ctrl.getCurrentAlias();
        }

      

        private void txt_Base_Settings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
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
            // ** go to compose
            NavigationService.Navigate(new Uri("/Pages/ComposeMessagePage.xaml", UriKind.Relative));
        }

        private void txt_Base_Messages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ComposeMessagePage.xaml", UriKind.Relative));
        }

        // ** some kind of popup needed to announce about the message that is just arrived
        public void messageArrived()
        {
            // **  ...get the message from datamaster and display it in canvas.
            var m = MessageBox.Show("Read it?", "You have received a message.", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));
                
            }
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

        private void txt_Base_Help_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HelpPage.xaml", UriKind.Relative));
        }

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

            Group g = new Group();
            g.GroupName = ".NET Codecamp";
            g.Description = "Here we are still coding at 0:55.";
            ctrl.dm.storeNewGroup(g);

            g = new Group();
            g.GroupName = "Comlab";
            g.Description = "We are Comlab, the one and the only one.";
            ctrl.dm.storeNewGroup(g);

            g = new Group();
            g.GroupName = "SWE";
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
        }

       

        
    }
}