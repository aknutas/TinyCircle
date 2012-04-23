using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using GEETHREE.DataClasses;

namespace GEETHREE
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //Variables
        private Controller c;

        private List<User> usrList;
        private List<Group> grpList;
        private List<Tags> tagList;
        
        private List<Message> draftMessageList;
        private List<Message> sentMessageList;
        private List<Message> privateMessagesList;
        private List<Message> broadcastMessagesList;
        private List<Message> tagMessagesList;

        private List<GroupInfoResponse> grpInfoResponseList;
        private List<UserInfoResponse> usrInfoResponseList;

        private System.ComponentModel.BackgroundWorker loaderBackgroundWorker;
        private bool threadRunning;
        private Object threadLock;

        public MainViewModel()
        {
            //Thread checks
            threadRunning = false;
            threadLock = new Object();

            //Create and get reference to the main controller instance
            c = Controller.Instance;

            //Create data container holders
            usrList = new List<User>();
            grpList = new List<Group>();
            tagList = new List<Tags>();
            tagMessagesList = new List<Message>();

            draftMessageList = new List<Message>();
            privateMessagesList = new List<Message>();
            broadcastMessagesList = new List<Message>();
            grpInfoResponseList = new List<GroupInfoResponse>();
            usrInfoResponseList = new List<UserInfoResponse>();

            this.Items = new ObservableCollection<ItemViewModel>();
            this.Users = new ObservableCollection<User>();
            this.Groups = new ObservableCollection<Group>();
            this.Tagss = new ObservableCollection<Tags>();

            this.DraftMessages = new ObservableCollection<Message>();
            this.SentMessages = new ObservableCollection<Message>();
            this.ReceivedPrivateMessages = new ObservableCollection<Message>();
            this.ReceivedBroadcastMessages = new ObservableCollection<Message>();
            this.TagMessages = new ObservableCollection<Message>();

            this.GroupInfoResponses = new ObservableCollection<GroupInfoResponse>();
            this.UserInfoResponses = new ObservableCollection<UserInfoResponse>();

            //Start async start data loading
            refreshDataAsync();

        }
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Tags> Tagss { get; private set; }
   
        public ObservableCollection<Message> DraftMessages { get; private set; }
        public ObservableCollection<Message> SentMessages { get; private set; }
        public ObservableCollection<Message> ReceivedPrivateMessages { get; private set; }
        public ObservableCollection<Message> ReceivedBroadcastMessages { get; private set; }
        public ObservableCollection<Message> TagMessages { get; private set; }
        
        public ObservableCollection<GroupInfoResponse> GroupInfoResponses { get; private set; }
        public ObservableCollection<UserInfoResponse> UserInfoResponses { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public void refreshDataAsync(){
            //Start async start data loading
            lock (threadLock)
            {
                if (!threadRunning)
                {
                    System.Diagnostics.Debug.WriteLine("MVM: Starting async data load worker");
                    loaderBackgroundWorker = new BackgroundWorker();
                    InitializeBackgroundWorker();
                    loaderBackgroundWorker.RunWorkerAsync();
                    System.Diagnostics.Debug.WriteLine("MVM: Exiting constructor");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        // ** just some test function
        public void AddNewMessage()
        {
            this.ReceivedBroadcastMessages.Add(new Message() { Header = "hello there2", TextContent = "plaa plaa plaa plaa plaa plaa2 " });
            c.notifyViewAboutMessage(true);
        }

        

        private void InitializeBackgroundWorker()
        {
            loaderBackgroundWorker.DoWork +=
                new DoWorkEventHandler(loaderBackgroundWorker_DoWork);
            loaderBackgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            loaderBackgroundWorker_RunWorkerCompleted);
            loaderBackgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            loaderBackgroundWorker_ProgressChanged);
        }

        //This is where the intensive work happens
        private void loaderBackgroundWorker_DoWork(object sender,
             DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
           
            grpList = c.dm.getAllGroups();
            usrList = c.dm.getAllUsers();
            tagList = c.dm.getAllTags();
            //TagMessageList = c.dm.getTagMessages();

            //broadcastMessagesList.Clear();
            //privateMessagesList.Clear();
            //sentMessageList.Clear();
            //draftMessageList.Clear();

            draftMessageList = c.dm.getMyDraftMessages();
            sentMessageList = c.dm.getMySentMessages();
            broadcastMessagesList = c.dm.getBroadcastMessages();
            privateMessagesList = c.dm.getIncomingPrivateMessages();

            //Debug
            System.Diagnostics.Debug.WriteLine("LoadData: Got " + sentMessageList.Count.ToString() + " sent messages");
            System.Diagnostics.Debug.WriteLine("LoadData: Got " + privateMessagesList.Count.ToString() + " private messages");
        }

        //Whole thing completed
        private void loaderBackgroundWorker_RunWorkerCompleted(object sender,
     RunWorkerCompletedEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            LoadGroups();
            LoadFriends();
            LoadTags();
          
            LoadBroadcastMessages();
            LoadPrivateMessages();
            LoadSentMessages();
            

            System.Diagnostics.Debug.WriteLine("LoadData: Data load thread completed");

            //Release "lock"
            lock (threadLock)
            {
                threadRunning = false;
            }
        }

        //Async thread made new progress and triggered progress update
        private void loaderBackgroundWorker_ProgressChanged(object sender,
     ProgressChangedEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            //TODO
            //Add lists to the UI one by one, right away when finished with loading
        }

        public void LoadFriends()
        {
            Users.Clear();
            foreach (User u in usrList)
            {
                this.Users.Insert(0, u);
            }
        }

        public void LoadTags()
        {
            Tagss.Clear();
            foreach (Tags u in tagList)
            {
                this.Tagss.Insert(0, u);
            }
        }

        public void LoadGroups()

        {
            Groups.Clear();
            foreach (Group g in grpList)
            {
                this.Groups.Insert(0, g);
            }   
        }

        public void LoadGroupInfoResponses()
        {
            GroupInfoResponses.Clear();
            grpInfoResponseList.Clear();
            grpInfoResponseList = c.dm.getAllGroupInfoResponses();

            foreach (GroupInfoResponse grp in grpInfoResponseList)
            {
                this.GroupInfoResponses.Insert(0, grp);
            }
        }

        public void LoadUserInfoResponses()
        {
            UserInfoResponses.Clear();
            usrInfoResponseList.Clear();
            usrInfoResponseList = c.dm.getAllUserInfoResponses();

            foreach (UserInfoResponse usr in usrInfoResponseList)
            {
                this.UserInfoResponses.Insert(0, usr);
            }
        }

        //Not needed - drafts saved to memory only!
        //public void LoadDrafts()

        //{
        //    DraftMessages.Clear();
        //    draftMessageList.Clear();
        //    draftMessageList = c.dm.getSendableMessages();

        //    foreach (Message m in draftMessageList)
        //    {
        //        this.DraftMessages.Add(m);           
        //    }
        //}

        public void LoadTagMessages(Tags tag)
        {
            TagMessages.Clear();
            tagMessagesList.Clear();

            tagMessagesList = c.dm.getAllTagMessages(tag);

            foreach (Message m in tagMessagesList)
            {
                this.TagMessages.Insert(0, m);
            }
        }
        
        public void LoadSentMessages()
        {
            SentMessages.Clear();
            //sentMessageList.Clear();

            //sentMessageList = c.dm.getSentMessages();

            foreach (Message m in sentMessageList)
            {
                this.SentMessages.Insert(0, m);
            }
        }

        public void LoadPrivateMessages()
        {
            ReceivedPrivateMessages.Clear();
            foreach (Message m in privateMessagesList)
            {
                this.ReceivedPrivateMessages.Insert(0, m);
            }
        }

        public void LoadBroadcastMessages()
        {
            ReceivedBroadcastMessages.Clear();
            foreach (Message m in broadcastMessagesList)
            {
                //this.ReceivedBroadcastMessages.Add(m);
                this.ReceivedBroadcastMessages.Insert(0, m);
            }
        }

        // clear and load data from database to observable collections
        public void LoadData()
        {

            

            Groups.Clear();
            grpList.Clear();     
            grpList = c.dm.getAllGroups();

            foreach (Group g in grpList)
            { 
                this.Groups.Insert(0,g);
            }

            Tagss.Clear();
            tagList.Clear();
            tagList = c.dm.getAllTags();

            foreach (Tags g in tagList)
            {
                this.Tagss.Insert(0, g);
            }

            Users.Clear();
            usrList.Clear();
            usrList = c.dm.getAllUsers();

            foreach (User u in usrList)
            {
                this.Users.Add(u);
            }

            ReceivedBroadcastMessages.Clear();
            
            DraftMessages.Clear();
            //draftMessageList.Clear();
            draftMessageList = c.dm.getMyDraftMessages();

            foreach (Message m in draftMessageList)
            {
                this.DraftMessages.Insert(0, m); 
            }
            SentMessages.Clear();
            //sentMessageList.Clear();
            sentMessageList = c.dm.getMySentMessages();
            System.Diagnostics.Debug.WriteLine("LoadData: Got " + sentMessageList.Count.ToString() + "sent messages");

            foreach (Message m in sentMessageList)
            {
                this.SentMessages.Add(m);
            }

            ReceivedBroadcastMessages.Clear();
            //broadcaseMessagesList.Clear();
            broadcastMessagesList = c.dm.getBroadcastMessages();
   

            foreach (Message m in broadcastMessagesList)
            {
                this.ReceivedPrivateMessages.Insert(0, m); 
            }

            ReceivedPrivateMessages.Clear();
            //privateMessagesList.Clear();
            privateMessagesList = c.dm.getIncomingPrivateMessages();
            foreach (Message m in privateMessagesList)
            {
                this.ReceivedPrivateMessages.Insert(0, m); 
            }
            // Sample data; replace with real data
            /*
            this.Items.Add(new ItemViewModel() { LineOne = "runtime one", LineTwo = "How are you doing?", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime two", LineTwo = "We need more members in our team.", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime three", LineTwo = "IT can be fun.", LineThree = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime four", LineTwo = "Windows phone 7 developers attention!", LineThree = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime five", LineTwo = "Rabbits are biting cables...", LineThree = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime six", LineTwo = "Language center meeting...", LineThree = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime seven", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime eight", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime nine", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime ten", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
            */
            /*
            this.Users.Add(new User("Tommi", "Having fun at codecamp.")); 
            this.Users.Add(new User("Antti", "Description 2")); 
            this.Users.Add(new User("Bishal", "Description 3")); 
            this.Users.Add(new User("Another Tommi", "Description 4"));
            this.Users.Add(new User("Anni", "Description 5"));

            this.Groups.Add(new Group() { GroupName = ".NET Code Camp", Description= "It is not necessary to wear long-sleeved underwear here." });
            this.Groups.Add(new Group() { GroupName = "Commlab", Description = "We are commlab" });
            this.Groups.Add(new Group() { GroupName = "SWE", Description = "We are from Sweden" });

            this.ReceivedBroadcastMessages.Add(new Message() { SenderAlias = "XXX", Header = "This is HEadertest", TextContent = "Hello there, we are coding .NET" });
            this.ReceivedBroadcastMessages.Add(new Message() { SenderAlias = "XXX", Header = "hello there2", TextContent = "plaa plaa plaa plaa plaa plaa2 " });
            this.ReceivedBroadcastMessages.Add(new Message() { SenderAlias = "XXX", Header = "hello there2", TextContent = "plaa plaa plaa plaa plaa plaa2 " });
            this.ReceivedBroadcastMessages.Add(new Message() { SenderAlias = "XXX", Header = "hello there3", TextContent = "plaa plaa plaa plaa plaa plaa3 " });
            this.ReceivedBroadcastMessages.Add(new Message() { SenderAlias = "XXX", Header = "hello there4", TextContent = "plaa plaa plaa plaa plaa plaa4 " });
           */
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}