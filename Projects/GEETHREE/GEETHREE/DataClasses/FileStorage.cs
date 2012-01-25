using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.IO;
using Microsoft.Phone;

namespace GEETHREE.DataClasses
{

    public class FileStorage
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;

        // Our isolated storage settings
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string AliasSettingKeyName = "CheckBoxSetting";
        const string ShowProfileInfoSettingKeyName = "ListBoxSetting";
        const string ShowSharedUploadsSettingKeyName = "RadioButton1Setting";
        


        // The default value of our settings
        const string AliasSettingDefault = "Anonymous";
        const bool ShowProfileInfoSettingDefault = false;
        const bool ShowSharedUploadsSettingDefault = false;
       

        
        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>


        public FileStorage()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;

            // Photochoosertask : initializes the task object, and identifies the method to run after the user completes the task
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);

            //Cameracapturetask : initializes the task object, and identifies the method to run after the user completes the task.
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        //browses for the photos and gets the picture in imagebox after selection

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {


                //Write image to isolated storage
                //appSetting.SaveToIsolatedStorage(e.ChosenPhoto, "Avatar.jpg");


                //display image on imagebox from isolated storage
                //img_Settings_avatar.Source = appSetting.ReadFromIsolatedStorage("Avatar.jpg");
            }
        }

        //Captures the picture using the camera and gets the picture in the imagebox 
        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {

                
            }
        }




        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        /// <summary>
        /// Property to get and set a Alias Setting Key.
        /// </summary>
        public string AliasSetting
        {
            get
            {
                return GetValueOrDefault<string>(AliasSettingKeyName, AliasSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(AliasSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        

        /// <summary>
        /// Property to get and set a ShowProfileInfo(toggleSwitch1) Setting Key.
        /// </summary>
        public bool ShowProfileInfoSetting
        {
            get
            {
                return GetValueOrDefault<bool>(ShowProfileInfoSettingKeyName, ShowProfileInfoSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(ShowProfileInfoSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        /// <summary>
        /// Property to get and set a ShowSharedUploads(toggleSwitch2) Setting Key.
        /// </summary>
        public bool ShowSharedUploadsSetting
        {
            get
            {
                return GetValueOrDefault<bool>(ShowSharedUploadsSettingKeyName, ShowSharedUploadsSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(ShowSharedUploadsSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Function to save avatar image to isolated storage
        /// </summary>
        public void SaveToIsolatedStorage(Stream imageStream, string fileName) 
        { 
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication()) 
            { 
                if (myIsolatedStorage.FileExists(fileName)) 
                { 
                    myIsolatedStorage.DeleteFile(fileName); 
                } 
                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName); 
                BitmapImage bitmap = new BitmapImage(); 
                bitmap.SetSource(imageStream); 
                WriteableBitmap wb = new WriteableBitmap(bitmap); 
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85); fileStream.Close(); 
            } 
        }

        /// <summary>
        /// Function to read avatar image from isolated storage
        /// </summary>
        public WriteableBitmap ReadFromIsolatedStorage(string fileName)
        {    
            WriteableBitmap bitmap = new WriteableBitmap(200,200);    
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())    
            {        
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))        
                {            
                    // Decode the JPEG stream.             
                    bitmap = PictureDecoder.DecodeJpeg(fileStream);        
                }    
            }    
            return bitmap;
        }


    }

}
