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
using System.Security.Cryptography;

namespace GEETHREE.DataClasses
{
     

    public class AppSettings
    {
        // Our isolated storage settings
        public IsolatedStorageSettings settings { get; private set; }

        // The isolated storage key names of our settings
        const string AliasSettingKeyName = "CheckBoxSetting";
        const string UserIDSettingKeyName = "UserIDSetting";
        const string ShowProfileInfoSettingKeyName = "ListBoxSetting";
        const string ShowSharedUploadsSettingKeyName = "RadioButton1Setting";

        // The default value of our settings
        //User newuser = new User();
        const string AliasSettingDefault = "Anonymous";
        const string UserIDSettingDefault = "0000";
        const bool ShowProfileInfoSettingDefault = false;
        const bool ShowSharedUploadsSettingDefault = false;
        

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>

        public AppSettings()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
            if (UserIDSetting == "0000")
            {
                UserIDSetting = CreateNewUserID();
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
                    //Kinda slow, but safe(r)
                    Controller.Instance.refreshAvatars();
                }
            }
        }
        /// <summary>
        /// Property to get and set a Alias Setting Key.
        /// </summary>
        public string UserIDSetting
        {
            get
            {
                return GetValueOrDefault<string>(UserIDSettingKeyName, UserIDSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(UserIDSettingKeyName, value))
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

        public static string CreateNewUserID()
        {
            string id;


            byte[] mac = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            byte[] time = System.BitConverter.GetBytes(System.DateTime.Now.Ticks);

            var source = new List<byte>();
            source.AddRange(mac);
            source.AddRange(time);

            HMACSHA256 sha = new HMACSHA256();
            byte[] hashBytes = sha.ComputeHash(source.ToArray());

            id = Convert.ToBase64String(hashBytes);           
            return id;
        }
    }

}
