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
using GEETHREE.DataClasses;

namespace GEETHREE.Pages
{
    public partial class MessagesPage : PhoneApplicationPage
    {
        private Message selectedMessage = null;
        public MessagesPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        private void messageCanvasExitImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SentMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedMessage = (Message)SentMessages.SelectedItem;
            messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
            messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
            messageCanvas.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;
        }

        private void ReveicedMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedMessage = (Message)ReveicedMessages.SelectedItem;
            messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
            messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
            messageCanvas.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;
        }

        private void DraftMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedMessage = (Message)DraftMessages.SelectedItem;
            messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
            messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
            messageCanvas.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;
        }

        // ** must navigate back to the pivot page from details page, not back to panorama page
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (messageCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                e.Cancel = true;
            }
            if (messageCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                e.Cancel = true;
            }
        }
    }
}