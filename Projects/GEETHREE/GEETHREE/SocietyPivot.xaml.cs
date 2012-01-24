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

namespace GEETHREE
{
    public partial class SocietyPivot : PhoneApplicationPage
    {
        public SocietyPivot()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void ListBox_Tap(object sender, GestureEventArgs e)
        {
            details.Visibility = System.Windows.Visibility.Visible;
        }

        // must navigate back to the pivot page from details page, not back to panorama page
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (details.Visibility == System.Windows.Visibility.Visible)
            {
                details.Visibility = System.Windows.Visibility.Collapsed;        
            
            }
        }

        private void ListBox_Tap_1(object sender, GestureEventArgs e)
        {
            details.Visibility = System.Windows.Visibility.Visible;
        }
    }
}