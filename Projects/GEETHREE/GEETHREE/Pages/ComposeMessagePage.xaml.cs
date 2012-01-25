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
    public partial class ComposeMessagePage : PhoneApplicationPage
    {
        public ComposeMessagePage()
        {
            
            InitializeComponent();
            DataContext = App.ViewModel;
            
            foreach (User u in App.ViewModel.Users)
            {
            
                listPicker1.Items.Add(u.UserName);
            
            }
        }

        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            listPicker1.Visibility = System.Windows.Visibility.Visible;
        }

        private void listPicker1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_compose_receipient.Text = listPicker1.SelectedItem.ToString();
            listPicker1.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void txt_compose_message_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_compose_message.Text == "Type your message here...")
            txt_compose_message.Text = "";
        }
    }
}