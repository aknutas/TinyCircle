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
using Coding4Fun.Phone.Controls;
using System.Windows.Media.Imaging;

namespace GEETHREE.Pages
{
    public partial class HelpPage : PhoneApplicationPage
    {
        private Controller ctrl;
        private bool arrivedMessageIsPrivate = false;
        public HelpPage()
        {
            InitializeComponent();

            // ** get the controller reference
            ctrl = Controller.Instance;

            // ** tell the controller which page is now active
            ctrl.registerCurrentPage(this, "help");

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

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //mainpageHelpTextBlock.Text = @"The main panorama has three pages. 'My Circle' shows you if there are other people in your circles.
           // You can also see statuses of your connections (WiFi and Server connection).";
        }

    }





}