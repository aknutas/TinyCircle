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
    public class MessageHandler : IDisposable
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
            RegisterEvents();
        }

        public void PrivateMessageReceived(object sender, MessageEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine(" Private message received " + e.TextContent);
            Message msg = new Message();
            msg.PrivateMessage = true;
            msg.Hash = e.Hash;
            msg.ReceiverID = e.Receiver;
            msg.SenderID = e.Sender;
            msg.TextContent = e.TextContent;
            msg.outgoing = true;
            

            if (e.Receiver == Controller.Instance.getCurrentUserID())
            {
                System.Diagnostics.Debug.WriteLine(" Woohoo, I gots a message");
                msg.outgoing = false;
            }
            dm.storeNewMessage(msg);
        }
        public void BroadcastMessageReceived(object sender, MessageEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("Broadcast message received " + e.TextContent);
            Message msg = new Message();
            msg.PrivateMessage = false;
            msg.Hash = e.Hash;
            msg.ReceiverID = e.Receiver;
            msg.SenderID = e.Sender;
            msg.TextContent = e.TextContent;
            msg.outgoing = true;
            dm.storeNewMessage(msg);

        }
        public void NewConnectionFound(object sender, ConnectionEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("Got new connection " + e.UserId);

            //Spambot --- Send all saved messages to new user 
            
        }
        public void Synchronize(User info)
        {
            //How do we synchonize the messages?
           
        }
        public void SendMessage(Message msg)
        {
            byte[] temphash={0,0};
            msg.Hash = temphash;
            this.cm.SendToAll(msg);
        }

        /// <summary>
        /// Register for events on communication
        /// </summary>
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

        #region IDisposable Implementation
        public void Dispose()
        {
            UnregisterEvents();
        }
        #endregion
    }
}
