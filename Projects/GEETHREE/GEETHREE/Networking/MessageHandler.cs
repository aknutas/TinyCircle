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
using System.Collections.ObjectModel;
using GEETHREE.DataClasses;
using GEETHREE.Networking;

namespace GEETHREE
{
    /// <summary>
    /// MessageHandler class handles the storaging and transmitting of messages. 
    /// </summary>
    public class MessageHandler
    {
        /// <summary>
        /// List of messages for local user
        /// </summary>
        public static ObservableCollection<Message> MyPrivateMessages = new ObservableCollection<Message>();
        /// <summary>
        /// List of broadcast messages the local user wants to see
        /// </summary>
        public static ObservableCollection<Message> MyBroadcastMessages = new ObservableCollection<Message>();
        /// <summary>
        /// List of other messages to be sent forward
        /// </summary>
        public static ObservableCollection<Message> TransitMessages = new ObservableCollection<Message>();

        private DataMaster dm;
        private CommunicationHandler cm;

        //Public constructor
        public MessageHandler(DataMaster dm, CommunicationHandler cm)
        {
            this.dm = dm;
            this.cm = cm;
        }

        public void PrivateMessageReceived(object sender, MessageEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(" Private message received" + e.TextContent);

#endif

        }
        public void BroadcastMessageReceived(object sender, MessageEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Broadcast message received" + e.TextContent);

#endif
        }
        public void NewConnectionFound(object sender, ConnectionEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Got new connection" + e.UserId);

#endif
        }
        public void Synchronize(User info)
        {
            //How do we synchonize the messages?
           
        }

        /// <summary>
        /// Register for events on communication
        private void RegisterEvents()
        {

            this.cm.PrivateMessageReceived += new EventHandler<MessageEventArgs>(PrivateMessageReceived);
            this.cm.BroadcastMessageReceived += new EventHandler<MessageEventArgs>(BroadcastMessageReceived);
            this.cm.NewConnection += new EventHandler<ConnectionEventArgs>(NewConnectionFound);
        }
        /// <summary>
        /// Unregister for events on communication
        /// </summary>
        private void UnregisterEvents()
        {
            if (this.cm != null)
            {
                this.cm.PrivateMessageReceived -= new EventHandler<MessageEventArgs>(PrivateMessageReceived);
                this.cm.BroadcastMessageReceived -= new EventHandler<MessageEventArgs>(BroadcastMessageReceived);
                this.cm.NewConnection -= new EventHandler<ConnectionEventArgs>(NewConnectionFound);
            }
        }
    }
}
