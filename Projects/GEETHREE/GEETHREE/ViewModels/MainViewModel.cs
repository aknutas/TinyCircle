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
        private Controller c;
        private List<Message> msgList;
        private List<User> usrList;
        private List<Group> grpList;
        private List<Message> sendableMsgList;



        public MainViewModel()
        {
            c = Controller.Instance;
            this.Items = new ObservableCollection<ItemViewModel>();
            this.Users = new ObservableCollection<User>();
            this.Groups = new ObservableCollection<Group>();
            this.DraftMessages = new ObservableCollection<Message>();
            this.SentMessages = new ObservableCollection<Message>();
            this.ReceivedPrivateMessages = new ObservableCollection<Message>();
            this.ReceivedBroadcastMessages = new ObservableCollection<Message>();
        }
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Message> Messages { get; private set;}
        public ObservableCollection<Message> SendableMessages { get; private set; }

        public ObservableCollection<Message> DraftMessages { get; private set; }
        public ObservableCollection<Message> SentMessages { get; private set; }
        public ObservableCollection<Message> ReceivedPrivateMessages { get; private set; }
        public ObservableCollection<Message> ReceivedBroadcastMessages { get; private set; }

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

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        /// 
        public void AddNewMessage()
        {
            this.ReceivedBroadcastMessages.Add(new Message() { Header = "hello there2", TextContent = "plaa plaa plaa plaa plaa plaa2 " });
            c.notifyViewAboutMessage();
        }

        // clear and load data from database to observable collections
        public void LoadData()
        {
            Groups.Clear();
            //grpList.Clear();
            Users.Clear();
            //usrList.Clear();
           
            grpList = c.dm.getAllGroups();

            foreach (Group g in grpList)
            { 
                this.Groups.Add(g);
            }

            usrList = c.dm.getAllUsers();

            foreach (User u in usrList)
            {
                this.Users.Add(u);
            }

            
            // Sample data; replace with real data
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

            //this.Users.Add(new User("Tommi", "Having fun at codecamp.")); 
            //this.Users.Add(new User("Antti", "Description 2")); 
            //this.Users.Add(new User("Bishal", "Description 3")); 
            //this.Users.Add(new User("Another Tommi", "Description 4"));
            //this.Users.Add(new User("Anni", "Description 5"));

            //this.Groups.Add(new Group() { GroupName = ".NET Code Camp", Description= "It is not necessary to wear long-sleeved underwear here." });
            //this.Groups.Add(new Group() { GroupName = "Commlab", Description = "We are commlab" });
            //this.Groups.Add(new Group() { GroupName = "SWE", Description = "We are from Sweden" });

            this.ReceivedBroadcastMessages.Add(new Message() {TextContent = "Hello there, we are coding .NET" });
            //this.ReceivedBroadcastMessages.Add(new Message() { Header = "hello there2", TextContent = "plaa plaa plaa plaa plaa plaa2 " });
            //this.ReceivedBroadcastMessages.Add(new Message() { Header = "hello there2", TextContent = "plaa plaa plaa plaa plaa plaa2 " });
            //this.ReceivedBroadcastMessages.Add(new Message() { Header = "hello there3", TextContent = "plaa plaa plaa plaa plaa plaa3 " });
            //this.ReceivedBroadcastMessages.Add(new Message() { Header = "hello there4", TextContent = "plaa plaa plaa plaa plaa plaa4 " });
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