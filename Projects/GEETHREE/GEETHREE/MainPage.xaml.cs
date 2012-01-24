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


namespace GEETHREE
{
    public partial class MainPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            // Photochoosertask : initializes the task object, and identifies the method to run after the user completes the task
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);

            //Cameracapturetask : initializes the task object, and identifies the method to run after the user completes the task.
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);

        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SocietyPivot.xaml", UriKind.Relative));
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

        private void img_Settings_camera_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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

        //browses for the photos and gets the picture in imagebox after selection

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {


                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                img_Settings_avatar.Source = bmp;
            }
        }

        //Captures the picture using the camera and gets the picture in the imagebox 
        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {

                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                img_Settings_avatar.Source = bmp;
            }
        }

    }
}