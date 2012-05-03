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
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Coding4Fun.Phone.Controls;


namespace GEETHREE.Pages
{
    public partial class SettingsPage : PhoneApplicationPage, AvatarChangeListener
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        Controller ctrl;
        private bool arrivedMessageIsPrivate = false; 
        DataClasses.AppSettings appSettings;
        Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
        string aliasPassword = "";
        public SettingsPage()
        {

            InitializeComponent();
            ctrl = Controller.Instance;
            appSettings = ctrl.appSetting;

            ctrl.registerAvatarUpdates(this);
            ctrl.registerCurrentPage(this, "settings");
            refreshAvatar();

            LayoutRoot.Background = backgroundbrush;



            // Photochoosertask : initializes the task object, and identifies the method to run after the user completes the task
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);

            //Cameracapturetask : initializes the task object, and identifies the method to run after the user completes the task.
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

       

        public void refreshAvatar(){
            img_Settings_avatar.Source = ctrl.getCurrentAvatar();
        }

        private void img_Settings_avatar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {

                photoChooserTask.Show();
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred.");
            }
        }

        //private void img_Settings_camera_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    try
        //    {
        //        cameraCaptureTask.Show();
        //    }
        //    catch (System.InvalidOperationException ex)
        //    {
        //        MessageBox.Show("An error occurred.");
        //    }

        //}

        //browses for the photos and gets the picture in imagebox after selection
        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Write image to isolated storage
                ctrl.changeCurrentAvatar(e.ChosenPhoto);
            }
        }

        //Captures the picture using the camera and gets the picture in the imagebox 
        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Write image to isolated storage
                ctrl.changeCurrentAvatar(e.ChosenPhoto);
            }
        }

        private void img_Settings_camera1_Click_1(object sender, EventArgs e)
        {
            try
            {
                cameraCaptureTask.Show();
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred.");
            }
        }
        // ** toast announces about the message that is just arrived
        public void messageArrived(bool isPrivate)
        {
            ToastPrompt tp = new ToastPrompt();

            if (isPrivate)
            {
                arrivedMessageIsPrivate = true;
                tp.Title = "You have a new whisper.";
            }
            else
            {
                arrivedMessageIsPrivate = false;
                tp.Title = "You have a new  shout.";
            }
            tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
            tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
            tp.Tap += toast_Tap;
            tp.Show();
        }
        // ** toast announces about user info that arrived
        public void friendInfoArrived(string alias)
        {
            string title;
            ToastPrompt tp = new ToastPrompt();
            if (alias != "0")
            {
                title = "You have recieved profile of " + alias;
            }
            else
            {
                title = "Friend information not found. Check used id and password";
            }
            tp.Title = title;

            tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
            tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
            tp.Tap += friend_Toast_Tap;
            tp.Show();
        }
        void friend_Toast_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //What to do now? Navigate to friendlist?
            string parameter = "PeoplePivot";
            NavigationService.Navigate(new Uri(string.Format("/Pages/SocietyPivot.xaml?parameter={0}", parameter), UriKind.Relative));
        }

        void toast_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (arrivedMessageIsPrivate)
            {
                string parameter = "messages_whispers";
                NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", parameter), UriKind.Relative));
            }
            else
            {
                string parameter = "messages_shouts";
                NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", parameter), UriKind.Relative));
            }
        }

      

        private void tgs_Settings_shareProfileInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (tgs_Settings_shareProfileInfo.IsChecked == true)
            {
                ToastPrompt tp = new ToastPrompt();
                if (ctrl.mh.ConnectedToServer == true)
                {
                    if (txt_Settings_alias.Text == "" || txt_Settings_alias.Text == "Anonymous" || txt_Settings_alias.Text == "anonymous")
                    {
                        tp.Title = "Alias cannot be empty or Anonymous.\nChange your alias and share again!";

                        tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
                        tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
                        tp.Show();
                        tgs_Settings_shareProfileInfo.IsChecked = false;

                        System.Diagnostics.Debug.WriteLine("Alias not Shared due to no alias error!");
                    }
                    else
                    {
                        var input = new InputPrompt(); 
                        
                        input.Title = "Alias Password"; 
                        input.Message = "Enter new password to share your alias!";
                        SolidColorBrush x = (SolidColorBrush)Resources["PhoneAccentBrush"];
                        input.Background = x;
                        input.Completed += input_Completed;
                            
                        input.Show();
                        
                        
                        //System.Diagnostics.Debug.WriteLine("Alias Shared");
                    }
                }
                else
                {
                    
                    tp.Title = "No Server Connection! \nYou cannot Share your Alias.";

                    tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
                    tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
                    tp.Show();
                    tgs_Settings_shareProfileInfo.IsChecked = false;

                    System.Diagnostics.Debug.WriteLine("Alias not Shared due to no server connection");
                }
            }
            
        }
        void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            ToastPrompt tp = new ToastPrompt();
            if (e.Result == null || e.Result.ToString() == "")
            {
                tp.Title = "Password cannot be empty.\nProvide a password and share again!";

                tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
                tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
                tp.Show();
                tgs_Settings_shareProfileInfo.IsChecked = false;

                System.Diagnostics.Debug.WriteLine("Alias not Shared due to password empty!");
            }
            else
            {
                aliasPassword = e.Result;


                ctrl.cm.ShareUserInfo(ctrl.getCurrentUserID(), txt_Settings_alias.Text, aliasPassword);

                tp.Title = "Alias Shared!";

                tp.ImageSource = new BitmapImage(new Uri("/GEETHREE;component/g3aicon2_62x62.png", UriKind.Relative));
                tp.TextOrientation = System.Windows.Controls.Orientation.Vertical;
                tp.Show();
                System.Diagnostics.Debug.WriteLine("Alias Shared : " + aliasPassword);
            }
            //add some code here    
        }

    }
}