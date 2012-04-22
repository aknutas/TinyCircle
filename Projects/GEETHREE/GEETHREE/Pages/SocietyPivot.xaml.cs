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
using System.Windows.Media.Imaging;
using System.IO;

namespace GEETHREE
{
    public partial class SocietyPivot : PhoneApplicationPage
    {
        private Message selectedMessage = null;
        private Controller ctrl;
        private Group selectedGroup = null;
        private User selectedUser = null;
        ObservableCollection<GroupInfoResponse> GroupInfoResponseslist = new ObservableCollection<GroupInfoResponse>();
        ObservableCollection<UserInfoResponse> UserInfoResponseslist = new ObservableCollection<UserInfoResponse>();
        List<User> Userslist = new List<User>();
        Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
       

        bool userflag = true;
       
        public SocietyPivot()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            ctrl = Controller.Instance;
            ctrl.registerCurrentPage(this, "society");
            ctrl.cm.RequestUserInfo(ctrl.getCurrentUserID());
            ctrl.cm.RequestGroupInfo(ctrl.getCurrentUserID());

        }
       

        // ** aks data from view model
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            //ContextMenu PopUpMenu = new ContextMenu();
            //MenuItem item1 = new MenuItem() { Header = "Delete Group" };
            //item1.Click += new RoutedEventHandler(MenuItem_Click);
            //PopUpMenu.Items.Add(item1);
            //ContextMenuService.SetContextMenu(this.groupsListBox, PopUpMenu);
            
        }
        
        private void Groups_ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedGroup = (Group) groupsListBox.SelectedItem;

            if (selectedGroup != null)
            {
                // ** create url and give it gropu name and gropu id as parameters
                
                string url = string.Format("/Pages/ComposeMessagePage.xaml?replyalias={0}&replyid={1}&groupmessageflag={2}", selectedGroup.GroupName, selectedGroup.GroupID, "1");

                // ** give the controller the page reference
                ctrl.registerPreviousPage(this, "society_groups");

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

                string url = string.Format("/Pages/ComposeMessagePage.xaml?replyalias={0}&replyid={1}&groupmessageflag={2}", selectedUser.UserName, selectedUser.UserID,"0");
                
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

            e.Cancel = true; // cancel the default behaviour

            if (addOrJoinCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                addOrJoinCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                txt_addorjoin_errorMessage.Text = "";
                addOrJoinCanvasTextBox.Text = "";
                
            }
            else if (groupListCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                groupListCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                txt_groupList_message.Text = "";
               

            }
            else if (addtagsCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                addtagsCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;

                addtagTextBox.Text = "";
                txt_addtag_errorMessage.Text = "";
                
                
            }
            else if (TagListCanvas.Visibility == Visibility.Visible)
            {
                TagListCanvas.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                App.ViewModel.refreshDataAsync();
            }
            else if (imagePreviewCanvas.Visibility == Visibility.Visible)
            {
                imagePreviewCanvas.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
                messageCanvas.Visibility = Visibility.Visible;
                
            }
            else if (messageCanvas.Visibility == Visibility.Visible)
            {
                messageCanvas.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
                receivedimage.Visibility = Visibility.Collapsed;
                TagListCanvas.Visibility = Visibility.Visible;

            }

            else // ** then, navigate back
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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
            else if (newparameter.Equals("toTags"))
            {
                socialpivots.SelectedItem = TagsPivot;

            }
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
                    App.ViewModel.refreshDataAsync();
                    addOrJoinCanvas.Visibility = System.Windows.Visibility.Collapsed;
                    ApplicationBar.IsVisible = true;


                   

                }

           
        }

        private void addtagButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (addtagTextBox.Text == "")
            {
                txt_addtag_errorMessage.Text = "Please provide a #tag!";
            }
            
            else
            {

                string newtag = addtagTextBox.Text;
                if (newtag.Substring(0, 1) != "#")
                {
                    newtag = "#" + newtag;
                }



                bool mytag = false;
                foreach (Tags t in ctrl.dm.getAllTags())
                {
                    if (t.TagName == newtag)
                        mytag = true;
                }

                if (mytag == true)
                {
                    txt_addtag_errorMessage.Text = "#Tag already exist!";
                }
                else
                {


                    Tags tag = new Tags();
                    tag.TagName = newtag;


                    ctrl.dm.storeNewTag(tag);
                    MessageBox.Show("#Tag : " + newtag, "#Tag Added!", MessageBoxButton.OK);
                    addtagTextBox.Text = "";
                    txt_addtag_errorMessage.Text = "";

                    App.ViewModel.refreshDataAsync();
                    addtagsCanvas.Visibility = System.Windows.Visibility.Collapsed;
                    ApplicationBar.IsVisible = true;

                }


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

        public bool checkOnline(string userid, List<User> userlist)
        {
            bool online = false;
            foreach (User us in Userslist)
            {
                
                if (us.UserID ==userid)
                {
                    online = true;
                }
            }
            return online;
        }

        private void appbar_addButton_Click(object sender, EventArgs e)
        {
            if (socialpivots.SelectedItem == PeoplePivot)
                {
                    //send for getNearbyGroupInfoRequest()
                    ctrl.cm.RequestUserInfo(ctrl.getCurrentUserID());


                    //list allnearbygroups in a new canvas
                    groupCanvasTextBlock.Text = "Add Friend";
                    groupListCanvas.Background = backgroundbrush;
                    userflag = true;
                    groupListCanvas.Visibility = Visibility.Visible;

                    txt_groupList_message.Text = "";
                    grplistBox1.Items.Clear();
                    UserInfoResponseslist.Clear();
                    Userslist.Clear();
                    Userslist = ctrl.dm.getAllUsers();
                    App.ViewModel.LoadUserInfoResponses();
                    


                    foreach (UserInfoResponse u in App.ViewModel.UserInfoResponses)
                    {
                        bool exists_in_userTable = false;
                        foreach (User us in Userslist)
                        {
                            
                            if (us.UserID == u.UserID)
                            {
                                exists_in_userTable = true;
                            }
                            
                        }

                        if (exists_in_userTable == false)
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
            else if (socialpivots.SelectedItem == TagsPivot)
            {
                txt_addtag_errorMessage.Text = "";
                addtagTextBox.Text = "";
                addtagsCanvas.Background = backgroundbrush;
                addtagsCanvas.Visibility = System.Windows.Visibility.Visible;
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
            else if (socialpivots.SelectedItem == TagsPivot)
            {
                UpdateAppbarButton(0, "/Resources/appbar.add.rest.png", "Add", true, appbar_addButton_Click);
                UpdateAppbarButton(1, "/Resources/appbar.add.rest.png", "remove", false, appbar_joingroup_Button_Click);
               
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
            groupCanvasTextBlock.Text = "Join Group";
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
            if (checkStringforinvalidchars(id, "+") == true)
                id = CreateNewGroupID(groupname);
            return id;
        }

        public bool checkStringforinvalidchars(string id, string chr)
        {
            int firstCharacter = -1;
            firstCharacter = id.IndexOf(chr);
            if (firstCharacter >= 0)
            {
                return true;
            }

            else
                return false;


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
                    App.ViewModel.refreshDataAsync();
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
                    App.ViewModel.refreshDataAsync();
                }

                grplistBox1.Visibility = System.Windows.Visibility.Collapsed;
                groupListCanvas.Visibility = System.Windows.Visibility.Collapsed;
                
                ApplicationBar.IsVisible = true;
                

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (socialpivots.SelectedItem == TagsPivot)
            {
                ListBoxItem selectedListBoxItem = this.tagsListBox.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
                if (selectedListBoxItem == null)
                {
                    return;
                }
                //App.ViewModel.Tagss.Remove((Tags)selectedListBoxItem.Content);
                ctrl.dm.deleteTag((Tags)selectedListBoxItem.Content);

                //delete tags messages
                ctrl.dm.deleteTagMessagebyTagName((Tags)selectedListBoxItem.Content);

                App.ViewModel.refreshDataAsync();
            }
            else if(socialpivots.SelectedItem == GroupsPivot )
            {
                ListBoxItem selectedListBoxItem = this.groupsListBox.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
                if (selectedListBoxItem == null)
                {
                    return;
                }
                //App.ViewModel.Groups.Remove((Group)selectedListBoxItem.Content);
                ctrl.dm.deleteTag((Tags)selectedListBoxItem.Content);
                App.ViewModel.refreshDataAsync();
            }
            else if (socialpivots.SelectedItem == PeoplePivot)
            {
                ListBoxItem selectedListBoxItem = this.usersListBox.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
                if (selectedListBoxItem == null)
                {
                    return;
                }
                //App.ViewModel.Users.Remove((User)selectedListBoxItem.Content);
                ctrl.dm.deleteTag((Tags)selectedListBoxItem.Content);
                App.ViewModel.refreshDataAsync();
            }
    
            

        }

        private void tagsListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((Tags)tagsListBox.SelectedItem != null)
            {
                App.ViewModel.LoadTagMessages((Tags)tagsListBox.SelectedItem);
                tagCanvasTextBlock.Text = ((Tags)tagsListBox.SelectedItem).TagName;
                TagListCanvas.Background = backgroundbrush;
                TagListCanvas.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = false;
            }
            
            
            
        }

       

        private void tagMessage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedMessage = (Message)tagMessageListBox.SelectedItem;

            if (selectedMessage != null)
            {
                receivedimage.Visibility = Visibility.Collapsed;
                if (selectedMessage.IsRead == false)
                {
                    selectedMessage.IsRead = true;
                    // ** ftw ?!?!?!
                    ctrl.dm.storeObjects(selectedMessage);

                }
                if (selectedMessage.TimeStamp.ToString() != null)
                    messageCanvasDateTime.Text = selectedMessage.TimeStamp.ToString();
                //messageCanvasDateTime.Text += selectedMessage.IsRead.ToString();

                //ctrl.dm.updateMessage(selectedMessage);
                
                messageCanvasSenderTextBlock.Text = selectedMessage.SenderAlias;

                //messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();

                messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
                byte[] attachmentContent = null;
                if (selectedMessage.Attachmentflag == "1")
                {

                    attachmentContent = Convert.FromBase64String(selectedMessage.Attachment);
                    BitmapImage bitImage = new BitmapImage();
                    MemoryStream ms = new MemoryStream(attachmentContent, 0, attachmentContent.Length);
                    ms.Write(attachmentContent, 0, attachmentContent.Length);
                    bitImage.SetSource(ms);
                    receivedimage.Source = bitImage;
                    receivedimage.Visibility = Visibility.Visible;
                }



                
                messageCanvas.Background = backgroundbrush;
                TagListCanvas.Visibility = Visibility.Collapsed;
                messageCanvas.Visibility = System.Windows.Visibility.Visible;
                
                ApplicationBar.IsVisible = false;
            }
        }

        // ** we have received an image and user taps it
        private void receivedimage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (receivedimage.Visibility == Visibility.Visible) // ** if there is an image
            {
                ApplicationBar.IsVisible = false;
                //receivedimage.Visibility = Visibility.Collapsed;
                messageCanvas.Visibility = Visibility.Collapsed;

                imegePreviewCanvasImageBig.Source = receivedimage.Source;
                //Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
                imagePreviewCanvas.Background = backgroundbrush;
                imagePreviewCanvas.Visibility = Visibility.Visible;
            }
        }

       

        

          

   }
            
}