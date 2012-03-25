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
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace GEETHREE.Pages
{
    public partial class ComposeMessagePage : PhoneApplicationPage
    {
        Controller ctrl;
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        string receiverID = "";
        string receiverAlias = "";
        string attachmentFlag = "0";
        string attachmentFileName = "none";
        byte[] attachmentContent;
        string attachmentContentstring = "none";
        Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
       
                
        public ComposeMessagePage()
        {
            
            InitializeComponent();
           
            txt_compose_error_label.Text = "";
            DataContext = App.ViewModel;

            ctrl = Controller.Instance;
            ctrl.registerCurrentPage(this, "compose");
            composeReceipientTextBox.Text = "Shout";

            LayoutRoot.Background = backgroundbrush;

           
            if (((Color)Application.Current.Resources["PhoneBackgroundColor"]).ToString() == "#FFFFFFFF")            
                image1.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/add.light.png", UriKind.Relative));
            else
                image1.Source = new BitmapImage(new Uri("/GEETHREE;component/Resources/add.png", UriKind.Relative)); 
                
            

            // Photochoosertask : initializes the task object, and identifies the method to run after the user completes the task
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);

            //Cameracapturetask : initializes the task object, and identifies the method to run after the user completes the task.
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);

            

        }

        //check for message to store to draft
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if ((txt_compose_message.Text != "") && (txt_compose_message.Text != "Type your message here..."))
            {

                // **  ...get the message saving the draft.
                var m = MessageBox.Show("Save to Drafts?", "Do you want to save this message to drafts?", MessageBoxButton.OKCancel);

                if (m == MessageBoxResult.OK)
                {

                    //write code for storing this message to draft
                    Message msg = new Message();
                    msg.TextContent = txt_compose_message.Text;
                    msg.SenderID = Controller.Instance.getCurrentUserID();
                    msg.SenderAlias = Controller.Instance.getCurrentAlias();
                    //msg.ReceiverID = replyID;
                    //msg.PrivateMessage = true;
                    //msg.outgoing = true;

                    // ** add the messages to the draftmessages collection
                    App.ViewModel.DraftMessages.Add(msg);
                    //ctrl.registerPreviousPage(this, "messages_draft");
                    // ** ask the controller, which was the last page
                    //string destination = ctrl.tellPreviousPage();
                    e.Cancel = true;
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", "messages_drafts"), UriKind.Relative));
                    
                }

            }
           

            // ** ask the controller, which was the last page
            string destination = ctrl.tellPreviousPage();

            if (destination == "main_shouts" || destination == "main_alias" || destination == "main_society")
                //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                 NavigationService.Navigate(new Uri(string.Format("/MainPage.xaml?parameter={0}", destination), UriKind.Relative));
            else if(destination == "messages_shouts" || destination == "messages_whispers" || destination == "messages_drafts" || destination == "messages_sent")
                NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", destination), UriKind.Relative));
            else if(destination == "society_users")
                NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", "toPeople"), UriKind.Relative));
            else if(destination == "society_groups")
                NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", "toGroups"), UriKind.Relative));
           
        }

        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {


            recipientListCanvas.Background = backgroundbrush;
            recipientListCanvas.Visibility = Visibility.Visible;
            PageTitle.Text = "Recepient:";
            recipientListBox.Items.Clear();

            foreach (User u in App.ViewModel.Users)
            {
                recipientListBox.Items.Add(u.UserName);
                //receiverListPicker.Items.Add(u.UserName);
            }

            recipientListBox.Visibility = Visibility.Visible;
            ApplicationBar.IsVisible = false;

            //fill the values for receiver alias and receiver ID
            //receiverID=;
            //receiverAlias=;
        }

        private void recipientListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recipientListBox.SelectedItem != null)
            {
                composeReceipientTextBox.Text = recipientListBox.SelectedItem.ToString();
                recipientListBox.Visibility = System.Windows.Visibility.Collapsed;
                recipientListCanvas.Visibility = System.Windows.Visibility.Collapsed;
                PageTitle.Text = "Compose";
                ApplicationBar.IsVisible = true;
                
            }
            


        }
  
        private void txt_compose_message_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
   

            if (txt_compose_message.Text == "Type your message here...")
            txt_compose_message.Text = "";


        }

        // start camera
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

            cameraCaptureTask.Show();
   
        }

        // ** start picture browser
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            photoChooserTask.Show();
        }

        //  send this message
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            // ** ... communicate with networker to create a packet and send it ...
            if (txt_compose_message.Text == "" || txt_compose_message.Text == "Type your message here...")
            {
                txt_compose_error_label.Text = "Please provide the message!";
            }
            else
            {

                Message msg =new Message();
                msg.TextContent=txt_compose_message.Text;
                msg.SenderID=Controller.Instance.getCurrentUserID();
                msg.SenderAlias = Controller.Instance.getCurrentAlias();
                if (composeReceipientTextBox.Text == "" || composeReceipientTextBox.Text == "Shout")
                {
                    msg.ReceiverID = "Shout";
                    msg.PrivateMessage = false;
                }
                else
                {
                    msg.ReceiverID = receiverID;
                    msg.PrivateMessage = true;
                }
            
                msg.outgoing=true;


                if (attachmentFlag == "1")
                {

                    attachmentContentstring = Convert.ToBase64String(attachmentContent);
                }
                else
                {
                    attachmentFileName = "none";
                    attachmentContentstring = "none";
                }
                msg.Attachmentflag = attachmentFlag;
                msg.Attachmentfilename = attachmentFileName;   
                msg.Attachment = attachmentContentstring;
                App.ViewModel.SentMessages.Add(msg);
                Controller.Instance.mh.SendMessage(msg);
                MessageBox.Show("Message sent.");
                
                txt_compose_message.Text = "";
                txt_compose_error_label.Text = "";
                composeReceipientTextBox.Text = "";
                receiverAlias = "";
                receiverID = "";
                attachmentFlag = "0";
                attachmentFileName = "none";
                attachmentContent = null;
                attachmentContentstring = "none";
                attachedImage.Visibility = Visibility.Collapsed;
                image1.Visibility = System.Windows.Visibility.Visible;
                composeReceipientTextBox.IsEnabled = true;
            }

            // ** ask the controller, which was the last page
            string destination = ctrl.tellPreviousPage();

            if (destination == "main_shouts" || destination == "main_alias" || destination == "main_society")
                //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                 NavigationService.Navigate(new Uri(string.Format("/MainPage.xaml?parameter={0}", destination), UriKind.Relative));
            else if(destination == "messages_shouts" || destination == "messages_whispers" || destination == "messages_drafts" || destination == "messages_sent")
                NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", destination), UriKind.Relative));
            else if(destination == "society_users")
                NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", "toPeople"), UriKind.Relative));
            else if(destination == "society_groups")
                NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", "toGroups"), UriKind.Relative));
          

        }

        //browses for the photos and gets the picture in imagebox after selection
        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                // ...communication with the isolated storage...

                BitmapImage bitImage = new BitmapImage();
                bitImage.CreateOptions = BitmapCreateOptions.None;
	            bitImage.SetSource(e.ChosenPhoto);
                attachedImage.Source = bitImage;
                attachedImage.Visibility = Visibility.Visible;

                attachmentFlag = "1";
                attachmentFileName = "img001.gim";
                attachmentContent = new byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Position = 0;                
                e.ChosenPhoto.Read(attachmentContent, 0, attachmentContent.Length);
     
            }
        }

        //Captures the picture using the camera and gets the picture in the imagebox 
        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                // ...communication with the isolated storage...

                BitmapImage bitImage = new BitmapImage();
                bitImage.CreateOptions = BitmapCreateOptions.None;
                bitImage.SetSource(e.ChosenPhoto);
                attachedImage.Source = bitImage;

                attachedImage.Visibility = Visibility.Visible;

                //Convert image to byte array

                attachmentFlag = "1";
                attachmentFileName = "img" + Controller.Instance.getNextRandomNumName() + ".gim";
                attachmentContent = new byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Position = 0;
                e.ChosenPhoto.Read(attachmentContent, 0, attachmentContent.Length); 
            
            }
        }
        // ** when navigated to this page, check the url parameters if they contain some information
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                string previousPage = ctrl.tellPreviousPage();
                if (previousPage != "messages_drafts")
                {// ** try to get sender name 
                    receiverAlias = NavigationContext.QueryString["replyalias"];
                    receiverID = NavigationContext.QueryString["replyid"];
                    composeReceipientTextBox.Text = receiverAlias;
                    image1.Visibility = Visibility.Collapsed;
                    composeReceipientTextBox.IsEnabled = false;
                }
                else // the user clicked draft from drafts page
                {
                    txt_compose_message.Text = NavigationContext.QueryString["draftcontent"];
                
                }

            }
            catch
            { 
                
            }

            base.OnNavigatedTo(e);
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


        //private void recipientCanvasExit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    recipientListCanvas.Visibility = Visibility.Collapsed;
        //    recipientListBox.Visibility = Visibility.Collapsed;
        //    PageTitle.Text = "Compose";

        //}


    }
}