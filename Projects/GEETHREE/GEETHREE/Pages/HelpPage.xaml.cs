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

namespace GEETHREE.Pages
{
    public partial class HelpPage : PhoneApplicationPage
    {
        private Controller ctrl;
        public HelpPage()
        {
            InitializeComponent();

            // ** get the controller reference
            ctrl = Controller.Instance;

            // ** tell the controller which page is now active
            ctrl.registerCurrentPage(this, "help");

        }

        // ** some kind of popup needed to announce about the message that is just arrived
        public void messageArrived(bool isPrivate)
        {
            // **  ...get the message from datamaster and display it in canvas.
            var m = MessageBox.Show("Read it?", "You have received a message.", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                if (isPrivate == true) // navigate to Messages - whispers
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MessagePage.xaml?parameter={0}", "messages_whispers"), UriKind.Relative));
                else // navigate to messages - shouts
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", "messages_shouts"), UriKind.Relative));
            }
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            mainpageHelpTextBlock.Text = @"The main panorama has three pages. 'My Circle' shows you if there are other people in your circles.
            You can also see statuses of your connections (WiFi and Server connection).";
        }

    }





}