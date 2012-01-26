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

namespace GEETHREE
{
    public partial class SocietyPivot : PhoneApplicationPage
    {

        private Group selectedGroup = null;
        private User selectedUser = null;
        
        public SocietyPivot()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }
       

        // ** aks data from view model
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void Groups_ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedGroup = (Group) groupsListBox.SelectedItem;
            detailsNameTextBlock.Text = selectedGroup.GroupName.ToString();
            detailsDescriptionText.Text = selectedGroup.Description.ToString();
            details.Visibility = System.Windows.Visibility.Visible;
            
        }

        private void Users_ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedUser = (User)usersListBox.SelectedItem;
            detailsNameTextBlock.Text = selectedUser.UserName.ToString();
            detailsDescriptionText.Text = selectedUser.Description.ToString();
            details.Visibility = System.Windows.Visibility.Visible;
            
        }

        // ** must navigate back to the pivot page from details page, not back to panorama page
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (details.Visibility == System.Windows.Visibility.Visible)
            {
                details.Visibility = System.Windows.Visibility.Collapsed;
                e.Cancel = true;
            }
            
        }
        // ** When navigated to pivot page, choose which page to display first
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string newparameter = this.NavigationContext.QueryString["parameter"];
            if (newparameter.Equals("toPeople"))
            {
                socialpivots.SelectedItem = PeoplePivot;
            }
            else if (newparameter.Equals("toGroups"))
            {
                socialpivots.SelectedItem = GroupsPivot;
              
            }
        }

        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            details.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}