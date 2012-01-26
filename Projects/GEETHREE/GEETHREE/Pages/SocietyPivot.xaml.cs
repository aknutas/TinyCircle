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
            //detailsCanvasTextBox.Visibility = System.Windows.Visibility.Collapsed;
            detailsCanvasButton.Content = "Send Message";
            details.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;

            
        }

        private void Users_ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedUser = (User)usersListBox.SelectedItem;
            detailsNameTextBlock.Text = selectedUser.UserName.ToString();
            detailsDescriptionText.Text = selectedUser.Description.ToString();
            //detailsCanvasTextBox.Visibility = System.Windows.Visibility.Visible;
            detailsCanvasButton.Content = "Send Message";
            details.Visibility = System.Windows.Visibility.Visible;
            ApplicationBar.IsVisible = false;
            
        }

        // ** must navigate back to the pivot page from details page, not back to panorama page
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (details.Visibility == System.Windows.Visibility.Visible)
            {
                details.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                e.Cancel = true; 
            }
            if (addOrJoinCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                addOrJoinCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
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

        private void appbar_addFriendButton_Click(object sender, EventArgs e)
        {
            addOrJoinCanvas.Visibility = System.Windows.Visibility.Visible;
            addOrJoinCanvasTextBlock.Text = "Type friends ID:";
            addOrJoinCanvasButton.Content = "Add";
            ApplicationBar.IsVisible = false;
        }



        private void appbar_joinGroupButton_Click(object sender, EventArgs e)
        {
            addOrJoinCanvas.Visibility = System.Windows.Visibility.Visible;
            addOrJoinCanvasTextBlock.Text = "Type group's name:";
            addOrJoinCanvasButton.Content = "Join";
            ApplicationBar.IsVisible = false;
        }

        private void detailsCanvasExitImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            details.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
        }

        private void addOrJoinCanvasExitImageExit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            addOrJoinCanvas.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
        }

        private void addOrJoinCanvasButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (addOrJoinCanvasTextBlock.Text == "Type group's name:")
            {
                // ** ask the network controller to join the group
                MessageBox.Show("You have joined a group\n " + selectedGroup.GroupName.ToString());
            }
            if (addOrJoinCanvasTextBlock.Text == "Type friends ID:")
            {
                // ** ask the nedwork controller to add a friend

                MessageBox.Show("You and " + selectedUser.UserName + "\n are now friends.");
            }  
        }

        private void detailsCanvasButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            // ask the controller to send message here

            MessageBox.Show("Message successfully sent.");
        }
        // ** some kind of popup needed to announce about the message that is just arrived
        public void messageArrived()
        {
            // **  ...get the message from datamaster and display it in canvas.
            var m = MessageBox.Show("Read it?", "You have reveived a message.", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/Pages/MessagesPage.xaml", UriKind.Relative));

            }
        }
    }
}