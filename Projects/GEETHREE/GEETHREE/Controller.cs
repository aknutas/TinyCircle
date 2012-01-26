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
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using GEETHREE.Pages;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace GEETHREE
{
    public class Controller
    {
        //Variables
        public DataMaster dm;
        private DataClasses.AppSettings appSetting;
        private MessageHandler mh;
        private CommunicationHandler cm;

        //Update view callback list
        private SettingsPage settingspage;
        private MainPage mainpage;

        //Singleton instance
        private static Controller instance;

        //Private constructor, no external access!
        //Don't change this if you really don't know (especially visibility to public)
        private Controller() {
            dm = new DataMaster();
            appSetting = new DataClasses.AppSettings();
            cm = new CommunicationHandler(this);
            mh = new MessageHandler(dm, cm);
            

            // TODO Elegant callbacks
            // callbackList = new List<PhoneApplicationPage>();
        }

        //Get us the singleton instance
        //Don't touch this either (I know you want to)
        public static Controller Instance
        {
            get
            {
                //If not created yet, create one
                if (instance == null)
                {
                    instance = new Controller();
                }
                return instance;
            }
        }

        //All kinds of cool and useful public functions (put your stuff here, I love you for it) -Antti
        public void refreshAvatars() {
            if (settingspage != null)
                settingspage.refreshAvatar();
            if (mainpage != null)
                mainpage.refreshAvatar();
        }

        //Wanting callbacks
        public void registerAvatarUpdates(PhoneApplicationPage regMe)
        {
            if (regMe is SettingsPage)
            {
                settingspage = (SettingsPage) regMe;
            }
            else if (regMe is MainPage)
            {
                mainpage = (MainPage) regMe;
            }
        }

        //File management
        public BitmapSource getCurrentAvatar()
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists("Avatar.jpg"))
                {
                    return dm.fm.readImageFromFile("Avatar.jpg");
                }
                else
                {
                    return new BitmapImage(new Uri("/GEETHREE;component/Resources/anonymous.png", UriKind.Relative));
                }
            }
        }

        public void changeCurrentAvatar(System.IO.Stream newAvatar)
        {
            dm.fm.saveImageToFile(newAvatar, "Avatar.jpg");
            refreshAvatars();
        }

        public string getCurrentAlias()
        {
            
            return appSetting.AliasSetting;
        }
        public void changeCurrentUserID(string id)
        {
            appSetting.UserIDSetting = id;
        }
        public string getCurrentUserID()
        {

            return appSetting.UserIDSetting;
        }
    }
}
