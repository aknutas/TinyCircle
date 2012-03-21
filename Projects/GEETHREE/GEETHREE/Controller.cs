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
        public DataClasses.AppSettings appSetting { get; private set; }
        public MessageHandler mh;
        public CommunicationHandler cm;
        private Random r;

        //Update view callback list
        private SettingsPage settingspage;
        private MainPage mainpage;


        private PhoneApplicationPage currentPage;
        private string currentPageName;
        //private ComposeMessagePage composeMessagePage;
        //private MessagesPage messagePage;
        //private SocietyPivot societyPivot;

        //Singleton instance
        private static Controller instance;

        //Private constructor, no external access!
        //Don't change this if you really don't know (especially visibility to public)
        private Controller() {
            r = new Random();
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


        public void notifyViewAboutMessage()
        {
            if (currentPageName == "main")
            {
                MainPage mp = (MainPage)currentPage;
                mp.messageArrived();
            }
            else if (currentPageName == "compose")
            {
                ComposeMessagePage cmp = (ComposeMessagePage)currentPage;
                cmp.messageArrived();
            }
            else if (currentPageName == "messages")
            {
                MessagesPage msgp = (MessagesPage)currentPage;
                msgp.messageArrived();
            }
            else if (currentPageName == "society")
            {
                SocietyPivot sp = (SocietyPivot)currentPage;
                sp.messageArrived();
            }
            else if(currentPageName == "settings")
            {
                SettingsPage settp = (SettingsPage)currentPage;
                settp.messageArrived();
            }
        
        }



    }
}
