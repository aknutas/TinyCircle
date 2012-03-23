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
using System.Security.Cryptography;

namespace GEETHREE
{
    public class Controller
    {
        //Variables
        public DataMaster dm;
        public DataClasses.AppSettings appSetting { get; private set; }
        public MessageHandler mh;
        public Networking.WebServiceConnector wcc;
        public CommunicationHandler cm;
        private Random r;

        //Update view callback list
        private SettingsPage settingspage;
        private MainPage mainpage;


        private PhoneApplicationPage currentPage;
        private PhoneApplicationPage previousPage;
        private string currentPageName;
        private string previousPageName;
        //private ComposeMessagePage composeMessagePage;
        //private MessagesPage messagePage;
        //private SocietyPivot societyPivot;

        //Singleton instance
        private static Controller instance;

        //Private constructor, no external access!
        //Don't change this if you really don't know (especially visibility to public)
        private Controller() {
            r = new Random();
            appSetting = new DataClasses.AppSettings();
            dm = new DataMaster(appSetting);
            wcc = new Networking.WebServiceConnector();
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
            {
                settingspage.refreshAvatar();   
            }
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

        public String getNextRandomNumName()
        {
            return r.Next().ToString();
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
        public void registerCurrentPage(PhoneApplicationPage pap, string pageName)
        {
            if (previousPageName == null)
            {
                previousPageName = pageName;
            }
            
            currentPageName = pageName;
            if (pageName == "main")
            {
                currentPage = (MainPage)pap; 
            }
            else if (pageName == "compose")
            {
                currentPage = (ComposeMessagePage)pap;
            }
            else if (pageName == "messages")
            {
                currentPage = (MessagesPage)pap;
            }
            else if (pageName == "society")
            {
                currentPage = (SocietyPivot)pap;
            }
            else if (pageName == "settings")
            {
                currentPage = (SettingsPage)pap;
            }
        
        }
        public void registerPreviousPage(PhoneApplicationPage pap, string pageName)
        {

            previousPageName = pageName;
            if (pageName == "main_shouts" ||pageName == "main_alias" || pageName == "main_society")
                previousPage = (MainPage)pap;
            
            else if (pageName == "messages_shouts" || pageName == "messages_whispers" || pageName == "messages_drafts" || pageName == "messages_sent")
                previousPage = (MessagesPage)pap;
            
            else if (pageName == "society_users" || pageName == "society_gropus")
                previousPage = (SocietyPivot)pap;
        }
        public string tellPreviousPage()
        {
            return previousPageName;
        }

        // 
        public void notifyViewAboutMessage(bool isPrivate)
        {
            if (currentPageName == "main")
            {
                MainPage mp = (MainPage)currentPage;
                mp.messageArrived(isPrivate);
            }
            else if (currentPageName == "compose")
            {
                ComposeMessagePage cmp = (ComposeMessagePage)currentPage;
                cmp.messageArrived(isPrivate);
            }
            else if (currentPageName == "messages")
            {
                MessagesPage msgp = (MessagesPage)currentPage;
                msgp.messageArrived(isPrivate);
            }
            else if (currentPageName == "society")
            {
                SocietyPivot sp = (SocietyPivot)currentPage;
                sp.messageArrived(isPrivate);
            }
            else if(currentPageName == "settings")
            {
                SettingsPage settp = (SettingsPage)currentPage;
                settp.messageArrived(isPrivate);
            }
        
        }


        public string CreateNewUserID()
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
