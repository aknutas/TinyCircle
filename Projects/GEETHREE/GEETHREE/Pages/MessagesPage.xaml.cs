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
                // ** when going to compose page when message canvas is visible, returning back here will collapse it
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

            selectedMessage.IsRead = true;
 
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

                replyID = selectedMessage.SenderID;
                replyAlias = selectedMessage.SenderAlias;
                if (selectedMessage.GroupMessage == true)
                    groupmessageFlag = "1";
                else
                    groupmessageFlag = "0";
                
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
                messageCanvas.Background = backgroundbrush;
                messageCanvas.Visibility = System.Windows.Visibility.Visible;
                if (replyID == Controller.Instance.getCurrentUserID())
                {
                    buttonReply.Visibility = Visibility.Collapsed;
                }
                else
                {
                    buttonReply.Visibility = Visibility.Visible;
                }
                ApplicationBar.IsVisible = false;
            }
        }

        private void PrivateMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            (ReceivedPrivateMessages.SelectedItem as Message).IsRead = true;
            selectedMessage = (Message)ReceivedPrivateMessages.SelectedItem;
            //ctrl.dm.deleteMessage(selectedMessage);
            //ctrl.dm.storeNewMessage(selectedMessage);
            if (selectedMessage != null)
            {
                receivedimage.Visibility = Visibility.Collapsed;
                messageCanvasSenderTextBlock.Text = selectedMessage.SenderAlias;
                if (messageCanvasSenderTextBlock.Text == "")
                {
                    messageCanvasSenderTextBlock.Text = "Anonymous";
                }
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
                replyAlias = selectedMessage.SenderAlias + " " + selectedMessage.IsRead.ToString();
                groupmessageFlag = "0";
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];

                messageCanvas.Background = backgroundbrush;
                messageCanvas.Visibility = System.Windows.Visibility.Visible;
                buttonReply.Visibility = Visibility.Visible;
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
            messageCanvas.Visibility = System.Windows.Visibility.Collapsed;
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
            if (imagePreviewCanvas.Visibility == Visibility.Visible)
            {
                imagePreviewCanvas.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = true;
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
        // ** we have received an image and user taps it
        private void receivedimage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (receivedimage.Visibility == Visibility.Visible) // ** if there is an image
            {
                ApplicationBar.IsVisible = false;
                //receivedimage.Visibility = Visibility.Collapsed;
                messageCanvas.Visibility = Visibility.Collapsed;

                imegePreviewCanvasImageBig.Source = receivedimage.Source;
                Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
                imagePreviewCanvas.Background = backgroundbrush;
                imagePreviewCanvas.Visibility = Visibility.Visible;          
            }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            
        }

        private void MyContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            

            // ask Viewmodel and database delete this message
        }

        private void contextMenuDelete_Click(object sender, RoutedEventArgs e)
        {

            ListBoxItem selectedListBoxItem = this.ReceivedPrivateMessages.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
            if (selectedListBoxItem == null)
            {
                return;
            }
            
            //Message m = new Message();
            // ** delete the message from viewmodel and also from database
            //m = (Message)selectedListBoxItem.Content;
            App.ViewModel.ReceivedPrivateMessages.Remove((Message)selectedListBoxItem.Content);


        }

        private void contextMenuReply_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}