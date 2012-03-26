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
using System.Windows.Media.Imaging;
using System.IO;

namespace GEETHREE.Pages
{
    public partial class MessagesPage : PhoneApplicationPage
    {
        private Message selectedMessage = null;
        private Controller ctrl;
        string replyID = "";
        string replyAlias = "";
        string groupmessageFlag = "0";

        public MessagesPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;

            ctrl = Controller.Instance;
            ctrl.registerCurrentPage(this, "messages");
            System.Diagnostics.Debug.WriteLine("Messages page constructed");

        }

        // ** When navigated to pivot page, choose which page to display first
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                // when going to compose page when message canvas is visible, returning back here will collapse it
                if (messageCanvas.Visibility == Visibility.Visible)
                {
                    messageCanvas.Visibility = Visibility.Collapsed;
                }

                string newparameter = this.NavigationContext.QueryString["parameter"];
                if (newparameter.Equals("messages_shouts"))
                    messagepivots.SelectedItem = shouts;
                else if (newparameter.Equals("messages_whispers"))
                    messagepivots.SelectedItem = whispers;
                else if (newparameter.Equals("messages_drafts"))
                    messagepivots.SelectedItem = drafts;
                else if (newparameter.Equals("messages_sent"))
                    messagepivots.SelectedItem = sent;
            }
            catch
            {

            }
        }
        private void BroadcastMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            selectedMessage = (Message)ReveicedBroadcastMessages.SelectedItem;
            if (selectedMessage != null)
            {
                receivedimage.Visibility = Visibility.Collapsed;
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

                //BitmapImage bitmapImage = new BitmapImage();
                //MemoryStream ms = new MemoryStream(imageBytes);
                //bitmapImage.SetSource(ms);
                //myImageElement.Source = bitmapImage; 

                replyID = selectedMessage.SenderID;
                replyAlias = selectedMessage.SenderAlias;
                if (selectedMessage.GroupMessage == true)
                    groupmessageFlag = "1";
                else
                    groupmessageFlag = "0";
                
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
                messageCanvas.Background = backgroundbrush;
                messageCanvas.Visibility = System.Windows.Visibility.Visible;
                ApplicationBar.IsVisible = false;
            }
        }
        //private void SentMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    selectedMessage = (Message)SentMessages.SelectedItem;
        //    messageCanvasSenderTextBlock.Text = selectedMessage.ReceiverID;
        //    //messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
        //    messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
        //    messageCanvas.Visibility = System.Windows.Visibility.Visible;
        //    ApplicationBar.IsVisible = false;
        //}

        //private void DraftMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    selectedMessage = (Message)DraftMessages.SelectedItem;
        //    //messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
        //    messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
        //    messageCanvas.Visibility = System.Windows.Visibility.Visible;
        //    ApplicationBar.IsVisible = false;
        //}

        private void PrivateMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            selectedMessage = (Message)ReceivedPrivateMessages.SelectedItem;
            if (selectedMessage != null)
            {
                receivedimage.Visibility = Visibility.Collapsed;
                messageCanvasSenderTextBlock.Text = selectedMessage.SenderAlias;
                //messageCanvasMessageHeader.Text = selectedMessage.Header.ToString();
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
                messageCanvasMessageContent.Text = selectedMessage.TextContent.ToString();
                replyID = selectedMessage.SenderID;
                replyAlias = selectedMessage.SenderAlias;
                groupmessageFlag = "0";
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];

                messageCanvas.Background = backgroundbrush;
                messageCanvas.Visibility = System.Windows.Visibility.Visible;
                ApplicationBar.IsVisible = false;
            }
        }
        private void DraftMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedMessage = (Message)DraftMessages.SelectedItem;
            if (selectedMessage != null)
            {
                // ** create url and give it user name and user id as parameters

                string url = string.Format("/Pages/ComposeMessagePage.xaml?draftcontent={0}", selectedMessage.TextContent);

                // ** give the controller the page reference
                ctrl.registerPreviousPage(this, "messages_drafts");

                // ** then navigate to Compose.xaml
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }

        }

        // ** MESSAGE CANVAS
        private void messageCanvasExitImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
            receivedimage.Visibility = Visibility.Collapsed;
        }
        private void btn_reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // ** ask the controller to register this page as a previous page before going to compose page
            // ** also provide the name of current pivont as a string, so we can navigate back to the same pivot
            if (messagepivots.SelectedItem == shouts)
                ctrl.registerPreviousPage(this, "messages_shouts");
            else if (messagepivots.SelectedItem == whispers)
                ctrl.registerPreviousPage(this, "messages_whispers");
            else if (messagepivots.SelectedItem == drafts)
                ctrl.registerPreviousPage(this, "messages_drafts");
            else if (messagepivots.SelectedItem == sent)
                ctrl.registerPreviousPage(this, "messages_sent");

            string url = string.Format("/Pages/ComposeMessagePage.xaml?replyalias={0}&replyid={1}&groupmessageflag={2}", replyAlias, replyID, groupmessageFlag);
            // ** then navigate to Compose.xaml
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
            //detailsNameTextBlock.Text = messageCanvasSenderTextBlock.Text;
            //detailsDescriptionText.Text = "Description";
            ////detailsCanvasTextBox.Visibility = System.Windows.Visibility.Visible;
            //detailsCanvasButton.Content = "Send Message";
            //detailsCanvasTextBox.Text = "";
            //txt_details_error_label.Text = "";
            //details.Visibility = System.Windows.Visibility.Visible;
            messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
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
                    Message msg = new Message();

                    msg.TextContent = detailsCanvasTextBox.Text;
                    msg.SenderID = Controller.Instance.getCurrentUserID();
                    msg.SenderAlias = Controller.Instance.getCurrentAlias();
                    //msg.ReceiverID = replyID;
                    //msg.PrivateMessage = true;
                    //msg.outgoing = true;

                    // ** add the messages to the draftmessages collection
                    App.ViewModel.DraftMessages.Add(msg);

                }
            }

            detailsCanvasTextBox.Text = "";
            txt_details_error_label.Text = "";
            details.Visibility = System.Windows.Visibility.Collapsed;
            Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];

            messageCanvas.Background = backgroundbrush;
            messageCanvas.Visibility = System.Windows.Visibility.Visible;


        }
        private void detailsCanvasButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (detailsCanvasTextBox.Text == "")
            {
                txt_details_error_label.Text = "Message Field is empty!";
            }
            else
            {
                // ** create a message
                Message msg = new Message();
                msg.TextContent = detailsCanvasTextBox.Text;
                msg.SenderID = Controller.Instance.getCurrentUserID();
                msg.SenderAlias = Controller.Instance.getCurrentAlias();
                msg.ReceiverID = replyID;
                msg.PrivateMessage = true;
                msg.outgoing = true;

                // ** add to sent messages collection
                App.ViewModel.SentMessages.Add(msg);

                // ** ask the controller to send the message 
                Controller.Instance.mh.SendMessage(msg);
                MessageBox.Show("Message sent.");

                // ** reset elements 
                detailsCanvasTextBox.Text = "";
                txt_details_error_label.Text = "";
                details.Visibility = System.Windows.Visibility.Collapsed;
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
                messageCanvas.Background = backgroundbrush;
                messageCanvas.Visibility = System.Windows.Visibility.Visible;

            }
        }

        // ** must navigate back to the pivot page from details page, not back to panorama page
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // cancel the default behaviour

            // **  first, close the canvas if it is visible 
            if (messageCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
                receivedimage.Visibility = Visibility.Collapsed;

            }
            else // ** then, navigate back
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }            
        }
        // ** MENUBAR, COMPOSE CLICKED 
        private void appbar_Message_Compose_Click(object sender, EventArgs e)
        {
            // ** ask the controller to register this page as a previous page before going to compose page
            // ** also provide the name of current pivont as a string, so we can navigate back to the same pivot

            if (messagepivots.SelectedItem == shouts)
                ctrl.registerPreviousPage(this, "messages_shouts");
            else if (messagepivots.SelectedItem == whispers)
                ctrl.registerPreviousPage(this, "messages_whispers");
            else if (messagepivots.SelectedItem == drafts)
                ctrl.registerPreviousPage(this, "messages_drafts");
            else if (messagepivots.SelectedItem == sent)
                ctrl.registerPreviousPage(this, "messages_sent");

            // ** go to compose page
            NavigationService.Navigate(new Uri("/Pages/ComposeMessagePage.xaml", UriKind.Relative));
        }

        // ** a popup message to announce about the message that is just arrived
        public void messageArrived(bool isPrivate)
        {
            // **  ...get the message from datamaster and display it in canvas.
            var m = MessageBox.Show("Read it?", "You have received a message.", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                if (isPrivate == true) // ** change the current pivot if needed
                    if (messagepivots.SelectedItem != whispers)
                        messagepivots.SelectedItem = whispers;

                    else
                        if (messagepivots.SelectedItem != shouts)
                            messagepivots.SelectedItem = shouts;

            }
        }

 


        
        
     
        

        




    }
}