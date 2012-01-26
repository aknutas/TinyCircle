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

        private void MessagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMessage = (Message)MessagesListBox.SelectedItem;

            messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
            messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
            messageCanvas.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;

          
        
        }
    }
}