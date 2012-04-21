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
using Microsoft.Phone.Shell;
using System.Linq;
using GEETHREE.DataClasses;
using System.Text.RegularExpressions;
using GEETHREE.Networking;
using System.ComponentModel;





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
        private List<AvatarChangeListener> avatarCallbackList;

        private PhoneApplicationPage currentPage;
        private PhoneApplicationPage previousPage;
        private string currentPageName;
        private string previousPageName;

        //Singleton instance
        private static Controller instance;

        //Private constructor, no external access!
        //Don't change this if you really don't know (especially visibility to public)
        private Controller()
        {
            r = new Random();
            appSetting = new DataClasses.AppSettings();
            dm = new DataMaster(appSetting);
            //wcc = new Networking.WebServiceConnector();
            cm = new CommunicationHandler(this);
            mh = new MessageHandler(dm, cm);
            //dm.resetTableGroupAndUserInfoResponse();
            //cm.RequestGroupInfo(getCurrentUserID());
            //cm.RequestUserInfo(getCurrentUserID());

            // Elegant callbacks
            avatarCallbackList = new List<AvatarChangeListener>();
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
        public void refreshAvatars()
        {
            foreach (AvatarChangeListener acl in avatarCallbackList)
            {
                acl.refreshAvatar();
            }
        }

        //Wanting callbacks
        public void registerAvatarUpdates(AvatarChangeListener regMe)
        {
            avatarCallbackList.Add(regMe);
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
                    if (((Color)Application.Current.Resources["PhoneBackgroundColor"]).ToString() == "#FFFFFFFF")
                        return new BitmapImage(new Uri("/GEETHREE;component/Resources/people.light.png", UriKind.Relative));
                    else
                        return new BitmapImage(new Uri("/GEETHREE;component/Resources/people.png", UriKind.Relative));


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

        // ** registers a page where the user is currently on
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
            // compose page registers itself again when user saves draft. Now we can navigagte back to drafts pivot
            //else if (pageName == "compose_draft")
            //{
            //    currentPage = (ComposeMessagePage)pap;
            //    previousPageName = "messages_drafts"; 
            //}

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
            else if (pageName == "help")
            {
                currentPage = (HelpPage)pap;
            }

        }
        // ** this is needed for determining where we navigate back from compose page
        public void registerPreviousPage(PhoneApplicationPage pap, string pageName)
        {

            previousPageName = pageName;
            if (pageName == "main_shouts" || pageName == "main_alias" || pageName == "main_society")
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

        // ** when message arrives, notify an active views about it
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
            else if (currentPageName == "settings")
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

            if (checkUserIDforinvalidchars(id, "+") == true)
                id = CreateNewUserID();

            return id;
        }

        public bool checkUserIDforinvalidchars(string id, string chr)
        {
            int firstCharacter = -1;
            firstCharacter = id.IndexOf(chr);
            if (firstCharacter >= 0)
            {
                return true;
            }

            else
                return false;


        }

        public void CreateApplicationTile()
        {
            var appTile = ShellTile.ActiveTiles.First();

            if (appTile != null)
            {
                var standardTile = new StandardTileData
                {
                    Title = "Tiny Circle",
                    //BackgroundImage = new Uri("Resources/anonymous.png", UriKind.Relative),
                    Count = 0, // any number can go here, leaving this null shows NO number  
                    
                    //BackBackgroundImage = new Uri("Images/ApplicationTileIcon.jpg", UriKind.Relative),  
                    
                    
                };

                appTile.Update(standardTile);
            }
        }
        public string getLatestMessage()
        {
           
            if (App.ViewModel.ReceivedPrivateMessages.Count != 0)
            {
                Message m = App.ViewModel.ReceivedPrivateMessages.Last();
                return m.TextContent.ToString();
            }
            return "";
        }
        public string getLatestAlias()
        {
            try
            {
                if (App.ViewModel.ReceivedPrivateMessages.Count != 0)
                {
                    Message m = App.ViewModel.ReceivedPrivateMessages.First();
                    return m.SenderAlias.ToString();
                }
            }
            catch { }
            return "";
        }

        public int getUnreadMessages()
        {
            int i = 0;
            List<Message> bcms = dm.getBroadcastMessages();

            List<Message> pm = dm.getIncomingPrivateMessages();

            foreach (Message m in bcms)
            {
                if (!m.IsRead)
                    i++;
            }
            foreach (Message m in pm)
            {
                if (!m.IsRead)
                    i++;
            }

            return i;
        
        
        }

        public void EditExistingTile()
        {
            //var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DetailId=123"));
            var foundTile = ShellTile.ActiveTiles.First();  
            if (foundTile != null)
            {
                // what sucks here is we cannot get access to the information on the existing tile...  
                var liveTile = new StandardTileData
                {
                    BackTitle =  getLatestAlias(),
                    BackContent = getLatestMessage(),
                    //Count = 0 + App.ViewModel.ReceivedBroadcastMessages.Count + App.ViewModel.ReceivedPrivateMessages.Count
                    Count = getUnreadMessages(),
                };

                foundTile.Update(liveTile);
            }
        }

        //String parsing for tagslist
        public List<string> GetTagsList(string messagecontent)
        {

            List<string> tagsList = new List<string>();



            if (messagecontent.Length > 1 && messagecontent.Substring(0, 1) == "#" && (messagecontent.Substring(1, 1) != " " && messagecontent.Substring(1, 1) != "#"))
            {
                int space = messagecontent.IndexOf(" ");
                if (space != -1)
                    tagsList.Add(messagecontent.Substring(0, space));
                else
                    tagsList.Add(messagecontent);


            }

            string[] list = Regex.Split(messagecontent, " #");
            int i = 0;
            foreach (string s in list)
            {
                if (i > 0)
                {
                    if (s.Length > 0)
                    {
                        if (s.Substring(0, 1) != " " && s.Substring(0, 1) != "#")
                        {
                            int space = s.IndexOf(" ");
                            if (space != -1)
                                tagsList.Add("#" + s.Substring(0, space));
                            else
                                tagsList.Add("#" + s);
                        }
                    }
                }
                i++;
            }
            return tagsList;
        } 
  


    }
}
