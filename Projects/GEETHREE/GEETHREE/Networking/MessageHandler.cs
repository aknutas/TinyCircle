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
using System.Collections.Generic;
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
        /// List of other messages to be sent forward
        /// </summary>
        public ObservableCollection<Message> TransitMessages { get; private set; }

        private DataMaster dm;
        private CommunicationHandler cm;

        //Public constructor
        public MessageHandler(DataMaster dm, CommunicationHandler cm)
        {
            this.dm = dm;
            this.cm = cm;
            this.TransitMessages = new ObservableCollection<Message>();
            RegisterEvents();
            LoadTransitmessages();
        }

        private void LoadTransitmessages()
        {
            this.TransitMessages.Clear();
            var messages = new List<Message>();
            messages = dm.getSendableMessages();

            for (int i = 0; i < messages.Count; i++)
            {
                this.TransitMessages.Add(messages[i]);
            }
        }
        public void PrivateMessageReceived(object sender, MessageEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine(" Private message received " + e.TextContent);
            Message msg = new Message();
            msg.PrivateMessage = true;
            msg.Hash = e.Hash;
            msg.ReceiverID = e.Receiver;
            msg.SenderID = e.Sender;
            msg.SenderAlias = e.SenderAlias;
            msg.TextContent = e.TextContent;
            msg.outgoing = true;
            msg.Attachmentflag = e.Attachmentflag;
            msg.Attachment = e.Attachment;
            msg.Attachmentfilename = e.Attachmentfilename;
            

            if (e.Receiver == Controller.Instance.getCurrentUserID())
            {
                System.Diagnostics.Debug.WriteLine(" Woohoo, I gots a message");
                msg.outgoing = false;
                App.ViewModel.ReceivedPrivateMessages.Add(msg);
                Controller.Instance.notifyViewAboutMessage(true);
            }
            else
                this.TransitMessages.Add(msg);

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
            msg.SenderAlias = e.SenderAlias;
            msg.TextContent = e.TextContent;
            msg.outgoing = true;
            msg.Attachment = e.Attachment;
            msg.Attachmentfilename = e.Attachmentfilename;
            msg.Attachmentflag = e.Attachmentflag;
            dm.storeNewMessage(msg);
            App.ViewModel.ReceivedBroadcastMessages.Add(msg);
            this.TransitMessages.Add(msg);
            Controller.Instance.notifyViewAboutMessage(false);
        }

        public void FileReceived(object sender, MessageEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("File received ");
            Message msg = new Message();
            msg.PrivateMessage = false;
            msg.Hash = e.Hash;
            msg.ReceiverID = e.Receiver;
            msg.SenderID = e.Sender;
            msg.SenderAlias = e.SenderAlias;
            msg.TextContent = e.TextContent;
            msg.outgoing = true;
            msg.Attachment = e.Attachment;
            msg.Attachmentfilename = e.Attachmentfilename;
            msg.Attachmentflag = e.Attachmentflag;
            dm.storeNewMessage(msg);
            //Where do we add this and who do we tell?
            //App.ViewModel.ReceivedBroadcastMessages.Add(msg);
            //this.TransitMessages.Add(msg);
            Controller.Instance.notifyViewAboutMessage(true);
        }

        public void NewConnectionFound(object sender, ConnectionEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("Got new connection " + e.UserId);

            //Spambot --- Send all saved messages to new user 
            for (int i = 1; i < TransitMessages.Count; i++)
            {
                this.cm.SendTo(TransitMessages[i], e.UserId);
            }
        }

        public void NewServerConnectionFound(object sender, ServerConnectionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Got new server connection ");

            

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
        /// Sending a file with our application:
        /// - Create a new message with sender and receiver info
        /// - Convert image to string
        /// - Add string as textcontent
        /// - Send using SendFile()
        /// - Profit?
        /// </summary>
        public void SendFile(Message msg)
        {
            byte[] temphash = { 0, 0 };
            msg.Hash = temphash;
            this.cm.SendFileToAll(msg);
        }

        public string MessageToString(Message msg)
        {
            string str = string.Format(Commands.MessageFormat, msg.SenderID,  msg.ReceiverID, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash);
            return str;
        }

        public Message StringToMessage(string str)
        {
            string[] tmpmessage = str.Split(Commands.CommandDelimeter.ToCharArray());
            Message msg = new Message();
            msg.SenderID = tmpmessage[1];
            msg.ReceiverID = tmpmessage[2];
            if (msg.ReceiverID == null)
                msg.PrivateMessage = false;
            else
                msg.PrivateMessage = true;
            msg.Attachment = tmpmessage[3];
            msg.Attachmentfilename = tmpmessage[4];
            msg.TextContent = tmpmessage[5];


            return msg;
        }
        /// <summary>
        /// Register for events on communication
        /// </summary>
        private void RegisterEvents()
        {

            this.cm.PrivateMessageReceived += new EventHandler<MessageEventArgs>(PrivateMessageReceived);
            this.cm.BroadcastMessageReceived += new EventHandler<MessageEventArgs>(BroadcastMessageReceived);
            this.cm.NewConnection += new EventHandler<ConnectionEventArgs>(NewConnectionFound);
            this.cm.FileReceived += new EventHandler<MessageEventArgs>(FileReceived);
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
