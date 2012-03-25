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
using Microsoft.Phone.Shell;
using System.Security.Cryptography;
using System.Text;
using System.Collections.ObjectModel;

namespace GEETHREE
{
    public partial class SocietyPivot : PhoneApplicationPage
    {
        private Controller ctrl;
        private Group selectedGroup = null;
        private User selectedUser = null;
        ObservableCollection<GroupInfoResponse> GroupInfoResponseslist = new ObservableCollection<GroupInfoResponse>();
        ObservableCollection<UserInfoResponse> UserInfoResponseslist = new ObservableCollection<UserInfoResponse>();
        Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];

        bool userflag = true;
       
        public SocietyPivot()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            ctrl = Controller.Instance;
            ctrl.registerCurrentPage(this, "society");

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

            if (selectedGroup != null)
            {
                // ** create url and give it gropu name and gropu id as parameters

                string url = string.Format("/Pages/ComposeMessagePage.xaml?replyalias={0}&replyid={1}", selectedGroup.GroupName, selectedGroup.GroupID);

                // ** give the controller the page reference
                ctrl.registerPreviousPage(this, "society_gropus");

                // ** then navigate to Compose.xaml
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
                
                
                //detailsNameTextBlock.Text = selectedGroup.GroupName.ToString();
                //detailsDescriptionText.Text = selectedGroup.Description.ToString();
                //detailsCanvasButton.Content = "Send Message";
                //detailsCanvasTextBox.Text = "";
                //txt_details_error_label.Text = "";
                //details.Visibility = System.Windows.Visibility.Visible;
                //ApplicationBar.IsVisible = false;
            }

            
        }

        private void Users_ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedUser = (User)usersListBox.SelectedItem;

            if (selectedUser != null)
            {
                // ** create url and give it user name and user id as parameters

                string url = string.Format("/Pages/ComposeMessagePage.xaml?replyalias={0}&replyid={1}", selectedUser.UserName, selectedUser.UserID);
                
                // ** give the controller the page reference
                ctrl.registerPreviousPage(this, "society_users");
                
                // ** then navigate to Compose.xaml
                NavigationService.Navigate(new Uri(url, UriKind.Relative));

                //detailsNameTextBlock.Text = selectedUser.UserName.ToString();
                //detailsDescriptionText.Text = selectedUser.Description.ToString();
                //detailsCanvasButton.Content = "Send Message";
                //detailsCanvasTextBox.Text = "";
                //txt_details_error_label.Text = "";
                //details.Visibility = System.Windows.Visibility.Visible;
                //ApplicationBar.IsVisible = false;
            }
            
        }

        // ** must navigate back to the pivot page from details page, not back to panorama page
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (details.Visibility == System.Windows.Visibility.Visible)
            {
                if (detailsCanvasTextBox.Text != "")
                {

                    // **  ...get the message saving the draft.
                    var m = MessageBox.Show("Save to Drafts?", "Do you want to save this message to drafts?", MessageBoxButton.OKCancel);

                    if (m == MessageBoxResult.OK)
                    {
                        //write code for storing this message to draft

                    }

                }
                detailsCanvasTextBox.Text = "";
                txt_details_error_label.Text = "";
                details.Visibility = System.Windows.Visibility.Collapsed;
                
                
                ApplicationBar.IsVisible = true;
                e.Cancel = true; 
            }
            if (addOrJoinCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                addOrJoinCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                txt_addorjoin_errorMessage.Text = "";
                addOrJoinCanvasTextBox.Text = "";
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

       

        private void detailsCanvasExitImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (detailsCanvasTextBox.Text != "")
            {

                 // **  ...get the message saving the draft.
                var m = MessageBox.Show("Save to Drafts?", "Do you want to save this message to drafts?", MessageBoxButton.OKCancel);

                if (m == MessageBoxResult.OK)
                {
                   //write code for storing this message to draft

                }

            }

            detailsCanvasTextBox.Text = "";
            txt_details_error_label.Text = "";
            details.Visibility = System.Windows.Visibility.Collapsed;

            ApplicationBar.IsVisible = true;
        }



        private void addOrJoinCanvasButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            

            //if (addOrJoinCanvasTextBlock.Text == "Type group's name:")
            //{
                if (addOrJoinCanvasTextBox.Text == "")
                {
                    txt_addorjoin_errorMessage.Text = "Please provide a group name!";
                }
                else if (txt_groupDesc.Text == "")
                {
                    txt_addorjoin_errorMessage.Text = "Please provide a group description!";
                }
                else
                {
                    Group g = new Group();
                    g.GroupName = addOrJoinCanvasTextBox.Text;
                    g.GroupID = CreateNewGroupID(addOrJoinCanvasTextBox.Text);
                    g.Description = txt_groupDesc.Text;
                    ctrl.dm.storeNewGroup(g);
                    MessageBox.Show("GroupName : " + addOrJoinCanvasTextBox.Text + "\n" + "Description: " + txt_groupDesc.Text,"Group Created!", MessageBoxButton.OK);
                    addOrJoinCanvasTextBox.Text = "";
                    txt_addorjoin_errorMessage.Text = "";
                    txt_groupDesc.Text = "";
                    App.ViewModel.LoadData();
                    addOrJoinCanvas.Visibility = System.Windows.Visibility.Collapsed;
                    ApplicationBar.IsVisible = true;


                   

                }

           
        }

        private void detailsCanvasButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (detailsCanvasTextBox.Text == "")
            {
                txt_details_error_label.Text = "Message Field is empty!";
            }
            else
            {
                // ask the controller to send message here

                MessageBox.Show("Message successfully sent.");
                detailsCanvasTextBox.Text = "";
                txt_details_error_label.Text = "";
                details.Visibility = System.Windows.Visibility.Collapsed;

                ApplicationBar.IsVisible = true;
            }
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

        

        private void appbar_addButton_Click(object sender, EventArgs e)
        {
            if (socialpivots.SelectedItem == PeoplePivot)
                {
                    //send for getNearbyGroupInfoRequest()
                    ctrl.cm.RequestUserInfo(ctrl.getCurrentUserID());


                    //list allnearbygroups in a new canvas
                    groupCanvasTextBlock.Text = "Add new Friend : ";
                    groupListCanvas.Background = backgroundbrush;
                    userflag = true;
                    groupListCanvas.Visibility = Visibility.Visible;

                    txt_groupList_message.Text = "";
                    grplistBox1.Items.Clear();
                    UserInfoResponseslist.Clear();
                    App.ViewModel.LoadUserInfoResponses();


                    foreach (UserInfoResponse u in App.ViewModel.UserInfoResponses)
                    {
                        if (ctrl.dm.checkFriendID(u.UserID) == false)
                        {
                            bool exists = false;
                            foreach (UserInfoResponse resp in UserInfoResponseslist)
                            {
                                if (resp.UserID == u.UserID)
                                    exists = true;
                            }
                            if (exists == false)
                            {
                                grplistBox1.Items.Add(u.UserAlias);
                                UserInfoResponseslist.Add(u);
                            }
                        }

                    }
                    if (grplistBox1.Items.Count() <= 0)
                        txt_groupList_message.Text = "There are no available nearby Users! \nPlease check again later!";
                    else
                        txt_groupList_message.Text = "";

                    
                    grplistBox1.Visibility = Visibility.Visible;
                    ApplicationBar.IsVisible = false;
                }
            else
                {
                    txt_addorjoin_errorMessage.Text = "";
                    addOrJoinCanvasTextBox.Text = "";
                    addOrJoinCanvas.Background = backgroundbrush;
                    addOrJoinCanvas.Visibility = System.Windows.Visibility.Visible;
                    
                    ApplicationBar.IsVisible = false;
                }


        }

        private void socialpivots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (socialpivots.SelectedItem == GroupsPivot)
            {
                UpdateAppbarButton(0, "/Resources/appbar.add.rest.png", "Create", true, appbar_addButton_Click);
                UpdateAppbarButton(1, "/Resources/appbar.favs.addto.rest.png", "Join", true, appbar_joingroup_Button_Click);

            }
            else
            {
                UpdateAppbarButton(0, "/Resources/appbar.add.rest.png", "Add", true, appbar_addButton_Click);
                UpdateAppbarButton(1, "/Resources/appbar.add.rest.png", "remove", false, appbar_joingroup_Button_Click);
               
            }
            

        }



        
        private void appbar_joingroup_Button_Click(object sender, EventArgs e)
        {
            //send for getNearbyGroupInfoRequest()
            ctrl.cm.RequestGroupInfo(ctrl.getCurrentUserID());
           

            //list allnearbygroups in a new canvas
            groupListCanvas.Background = backgroundbrush;
            groupCanvasTextBlock.Text = "Join new Group : ";
            userflag = false;
            groupListCanvas.Visibility = Visibility.Visible;

            txt_groupList_message.Text = "";
            grplistBox1.Items.Clear();
            GroupInfoResponseslist.Clear();
                App.ViewModel.LoadGroupInfoResponses();
                
            
            foreach (GroupInfoResponse u in App.ViewModel.GroupInfoResponses)
            {
                if (ctrl.dm.checkGroupID(u.GroupID) == false)
                {
                    bool exists = false;
                    foreach (GroupInfoResponse resp in GroupInfoResponseslist)
                    {
                        if (resp.GroupID == u.GroupID)
                            exists = true;
                    }
                    if (exists == false)
                    {
                        grplistBox1.Items.Add(u.GroupName);
                        GroupInfoResponseslist.Add(u);
                    }
                }
                
            }
            if (grplistBox1.Items.Count() <= 0)
                txt_groupList_message.Text = "There are no available nearby groups! \nPlease check again later!";
            else
                txt_groupList_message.Text = "";
            grplistBox1.Visibility = Visibility.Visible;
            ApplicationBar.IsVisible = false;

            


        }

        //Function to add or remove appbar button

        private void UpdateAppbarButton(int index, string uriString, string text, bool visibility, EventHandler handler)
        {
            ApplicationBarIconButton button1 = null;

            if (ApplicationBar.Buttons.Count > index)
            {
                button1 = ApplicationBar.Buttons[index] as ApplicationBarIconButton;
            }

            if (button1 != null)
            {
                {
                    ApplicationBar.Buttons.Remove(button1);
                }
            }
            if (visibility == true)
            {
                button1 = new ApplicationBarIconButton(new Uri(uriString, UriKind.Relative));
                button1.Text = text;
                button1.Click += handler;
                ApplicationBar.Buttons.Insert(index, button1);
            }
        }

        public string CreateNewGroupID(string groupname)
        {
            string id;

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] name = UE.GetBytes(groupname); 
            byte[] time = System.BitConverter.GetBytes(System.DateTime.Now.Ticks);

            var source = new List<byte>();
            source.AddRange(name);
            source.AddRange(time);

            HMACSHA256 sha = new HMACSHA256();
            byte[] hashBytes = sha.ComputeHash(source.ToArray());

            id = Convert.ToBase64String(hashBytes);
            return id;
        }

        private void grouptListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grplistBox1.SelectedItem != null)
            {
                if (userflag == true)
                {
                    User u = new User();
                    u.UserID = UserInfoResponseslist[grplistBox1.SelectedIndex].UserID;
                    u.UserName = UserInfoResponseslist[grplistBox1.SelectedIndex].UserAlias;
                    u.Description = UserInfoResponseslist[grplistBox1.SelectedIndex].Description;
                    ctrl.dm.storeNewUser(u);

                    MessageBox.Show("New Friend : " + UserInfoResponseslist[grplistBox1.SelectedIndex].UserAlias + " added successfully!!");
                    App.ViewModel.LoadData();
                }
                else
                {

                    //save group to database
                    Group g = new Group();
                    g.GroupName = GroupInfoResponseslist[grplistBox1.SelectedIndex].GroupName;
                    g.GroupID = GroupInfoResponseslist[grplistBox1.SelectedIndex].GroupID;
                    g.Description = GroupInfoResponseslist[grplistBox1.SelectedIndex].Description;
                    ctrl.dm.storeNewGroup(g);

                    ////show group joined message
                    MessageBox.Show("Joined group : " + GroupInfoResponseslist[grplistBox1.SelectedIndex].GroupName + " successfully!!");
                    ////make canvas collapsed  
                    App.ViewModel.LoadData();
                }

                grplistBox1.Visibility = System.Windows.Visibility.Collapsed;
                groupListCanvas.Visibility = System.Windows.Visibility.Collapsed;
                
                ApplicationBar.IsVisible = true;
                

            }
        }

    }
}