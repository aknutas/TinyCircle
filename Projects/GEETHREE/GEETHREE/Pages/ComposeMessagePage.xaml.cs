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

namespace GEETHREE.Pages
{
    public partial class ComposeMessagePage : PhoneApplicationPage
    {
        Controller ctrl;
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        string attachmentFlag = "0";
        string attachmentFileName = "";
        byte[] attachmentContent;
        public ComposeMessagePage()
        {
            
            InitializeComponent();
           
            txt_compose_error_label.Text = "";
            DataContext = App.ViewModel;

            ctrl = Controller.Instance;
            ctrl.registerCurrentPage(this, "compose");
 
            foreach (User u in App.ViewModel.Users)
            {
                receiverListPicker.Items.Add(u.UserName);
            }

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
                   

                }

            }
        }

        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            receiverListPicker.Visibility = System.Windows.Visibility.Visible;
        }

        private void receiverListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (receiverListPicker.SelectedItem != null)
            {
                txt_compose_receipient.Text = receiverListPicker.SelectedItem.ToString();
                receiverListPicker.Visibility = System.Windows.Visibility.Collapsed;
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
            if (txt_compose_receipient.Text == "")
            {
                msg.ReceiverID = "0";
                msg.PrivateMessage = false;
            }
            else
            {
                msg.ReceiverID = txt_compose_receipient.Text;
                msg.PrivateMessage = true;
            }
            
            msg.outgoing=true;
            msg.Attachmentflag = attachmentFlag;
            msg.Attachmentfilename = attachmentFileName;
            msg.Attachment = attachmentContent;
            App.ViewModel.SentMessages.Add(msg);
            Controller.Instance.mh.SendMessage(msg);
                MessageBox.Show("Message sent.");
                txt_compose_message.Text = "";
                txt_compose_error_label.Text = "";
            }
        }

        //browses for the photos and gets the picture in imagebox after selection
        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                // ...communication with the isolated storage...

                BitmapImage bitImage = new BitmapImage();
	            bitImage.SetSource(e.ChosenPhoto);
                attachedImage.Source = bitImage;

                attachmentFlag = "1";
                attachmentFileName = "img001.gim";
                attachmentContent = new byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Position = 0;

                //attacmentContent = ima
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
                bitImage.SetSource(e.ChosenPhoto);
                attachedImage.Source = bitImage;

                attachedImage.Source = bitImage;

                //Convert image to byte array

                attachmentFlag = "1";
                attachmentFileName = "img" + Controller.Instance.getNextRandomNumName() + ".gim";
                attachmentContent = new byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Position = 0;
                e.ChosenPhoto.Read(attachmentContent, 0, attachmentContent.Length); 
            
            }
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