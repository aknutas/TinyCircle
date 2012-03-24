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


namespace GEETHREE.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        Controller ctrl;
        DataClasses.AppSettings appSettings;
        Brush backgroundbrush = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
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

        // ** some kind of popup needed to announce about the message that is just arrived
        public void messageArrived(bool isPrivate)
        {
            var m = MessageBox.Show("Read it?", "You have received a message.", MessageBoxButton.OKCancel);

            if (m == MessageBoxResult.OK)
            {
                if (isPrivate == true) // navigate to Messages - whispers
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MessagePage.xaml?parameter={0}", "messages_whispers"), UriKind.Relative));
                else // navigate to messages - shouts
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MessagesPage.xaml?parameter={0}", "messages_shouts"), UriKind.Relative));
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
    }
}