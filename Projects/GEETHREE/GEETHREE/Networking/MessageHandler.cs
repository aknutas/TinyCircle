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
using System.Windows.Media.Imaging;


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
        private Controller c;
        private DataMaster dm;
        private CommunicationHandler cm;
       
        private bool themeColor;
        public bool ConnectedToServer { get; private set; }
        public int LocalConnections { get; private set; }

        //Public constructor
        public MessageHandler(DataMaster dm, CommunicationHandler cm)
        {
            this.dm = dm;
            this.cm = cm;   
            this.TransitMessages = new ObservableCollection<Message>();
            this.ConnectedToServer = false;
            this.LocalConnections = 0;
            RegisterEvents();
            SetThemeColors();          
        }

        public void SetThemeColors()
        {

            System.Diagnostics.Debug.WriteLine("MH: GETTING PHONE THEME");          
        
            var currentPhoneTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"];
            if (currentPhoneTheme == Visibility.Visible == true) // light theme
            {
                themeColor = true;
            }
            else
                themeColor = false;
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
           
            if (dm.checkMessageExist(e.Sender, e.Receiver, e.timestamp) == false)
            {
                System.Diagnostics.Debug.WriteLine("MH : Private message received " + e.TextContent);
                Message msg = new Message();
                msg.PrivateMessage = true;
                msg.GroupMessage = false;
                msg.Hash = e.Hash;
                msg.ReceiverID = e.Receiver;
                msg.SenderID = e.Sender;
                if (e.SenderAlias == null || e.SenderAlias == "")
                    msg.SenderAlias = "Anonymous";
                else
                    msg.SenderAlias = e.SenderAlias;
                msg.TextContent = e.TextContent;

                msg.outgoing = true;
                msg.Attachmentflag = e.Attachmentflag;
                msg.Attachment = e.Attachment;
                msg.Attachmentfilename = e.Attachmentfilename;
                msg.IsRead = false;
                msg.TimeStamp = e.timestamp;  
                
                if (themeColor == true) // light theme
                {
                    msg.MessageTypeImageURL = "/Resources/appbar.download.rest_black.png";
                }
                else
                    msg.MessageTypeImageURL = "/Resources/appbar.download.rest.png";
            
                if (e.Receiver == Controller.Instance.getCurrentUserID())
                {
                    System.Diagnostics.Debug.WriteLine(" Woohoo, I got a message");
                    msg.outgoing = false;
                    dm.storeNewMessage(msg);
                    int msgDbID = msg.msgDbId;
                    bool tagsFlag = false;


                    if (Controller.Instance.GetTagsList(e.TextContent).Count > 0)
                    {
                        tagsFlag = true;
                    }
                    if (tagsFlag == true)
                    {
                        //store new tags if any
                        foreach (string tagfromMessage in Controller.Instance.GetTagsList(e.TextContent))
                        {

                            TagMessage tagMessage = new TagMessage();
                            tagMessage.MessageID = msgDbID;
                            tagMessage.TagName = tagfromMessage;

                            dm.storeNewTagMessage(tagMessage);
                        }
                    }
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {

                        Controller.Instance.notifyViewAboutMessage(true);
                    });

                    
                }
                else
                {
                    this.TransitMessages.Add(msg);
                    //Try to send to server
                    cm.SendToServer(msg);
                }
            }
            
           
        }
        public void BroadcastMessageReceived(object sender, MessageEventArgs e)
        {
            if (c == null)
                c = Controller.Instance;

            if (dm.checkMessageExist(e.Sender, e.Receiver, e.timestamp) == false)
            {
                System.Diagnostics.Debug.WriteLine("MH : Broadcast message received " + e.TextContent);
                Message msg = new Message();
                msg.PrivateMessage = false;
                msg.GroupMessage = false;
                msg.Hash = e.Hash;
                msg.ReceiverID = e.Receiver;
                msg.SenderID = e.Sender;
                if (e.SenderAlias == null || e.SenderAlias == "")
                    msg.SenderAlias = "Anonymous";
                else
                    msg.SenderAlias = e.SenderAlias;
                msg.TextContent = e.TextContent;
                msg.outgoing = true;

              
                if (themeColor == true) // light theme
                {
                    msg.MessageTypeImageURL = "/Resources/appbar.download.rest_black.png";
                }
                else
                    msg.MessageTypeImageURL = "/Resources/appbar.download.rest.png";

                msg.Attachment = e.Attachment;
                msg.Attachmentfilename = e.Attachmentfilename;
                msg.Attachmentflag = e.Attachmentflag;
                msg.IsRead = false;
                msg.TimeStamp = e.timestamp;

                dm.storeNewMessage(msg);

                int msgDbID = msg.msgDbId;
                bool tagsFlag = false;


                if (Controller.Instance.GetTagsList(e.TextContent).Count > 0)
                {
                    tagsFlag = true;
                }
                if (tagsFlag == true)
                {
                    //store new tags if any
                    foreach (string tagfromMessage in Controller.Instance.GetTagsList(e.TextContent))
                    {

                        TagMessage tagMessage = new TagMessage();
                        tagMessage.MessageID = msgDbID;
                        tagMessage.TagName = tagfromMessage;

                        dm.storeNewTagMessage(tagMessage);
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Controller.Instance.notifyViewAboutMessage(false);
                });

                this.TransitMessages.Add(msg);
            }

               
           
        }
        public void GroupMessageReceived(object sender, MessageEventArgs e)
        {
            if (dm.checkMessageExist(e.Sender, e.Receiver, e.timestamp) == false)
            {
                System.Diagnostics.Debug.WriteLine("Group message received " + e.TextContent);
                Message msg = new Message();
                msg.PrivateMessage = false;
                msg.GroupMessage = true;
                msg.Hash = e.Hash;
                msg.ReceiverID = e.Receiver;
                msg.SenderID = e.Sender;
                if (e.SenderAlias == null || e.SenderAlias == "")
                    msg.SenderAlias = "Anonymous";
                else
                    msg.SenderAlias = e.SenderAlias;
                msg.TextContent = e.TextContent;
                msg.outgoing = true;
                msg.Attachment = e.Attachment;
                msg.Attachmentfilename = e.Attachmentfilename;
                msg.Attachmentflag = e.Attachmentflag;
                msg.TimeStamp = e.timestamp;
                msg.IsRead = false;
                //if the groupID is one of my groupID
                bool mygroup = false;
                foreach (Group g in dm.getAllGroups())
                {
                    if (g.GroupID == e.Sender)
                        mygroup = true;
                }

                if (mygroup == true)
                {
                    dm.storeNewMessage(msg);
                    int msgDbID = msg.msgDbId;
                    bool tagsFlag = false;


                    if (Controller.Instance.GetTagsList(e.TextContent).Count > 0)
                    {
                        tagsFlag = true;
                    }
                    if (tagsFlag == true)
                    {
                        //store new tags if any
                        foreach (string tagfromMessage in Controller.Instance.GetTagsList(e.TextContent))
                        {
                            TagMessage tagMessage = new TagMessage();
                            tagMessage.MessageID = msgDbID;
                            tagMessage.TagName = tagfromMessage;

                            dm.storeNewTagMessage(tagMessage);

                        }
                    }
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Controller.Instance.notifyViewAboutMessage(false);
                    });
                }
                this.TransitMessages.Add(msg);
            }
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
            msg.TimeStamp = e.timestamp;
            dm.storeNewMessage(msg);
            
            //Where do we add this and who do we tell?
            //App.ViewModel.ReceivedBroadcastMessages.Add(msg);
            if(e.Attachmentflag=="0")
                this.TransitMessages.Add(msg);
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

            this.LocalConnections = this.cm.Connections.Count;

            //How do we tell the UI about connection?
        }

        public void NewServerConnectionFound(object sender, ServerConnectionEventArgs e)
        {
            if (e.Running)
            {
                System.Diagnostics.Debug.WriteLine("Got new server connection ");
                this.ConnectedToServer = true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Server connection lost");
                this.ConnectedToServer = false;
            }
            
        
        }

        public void Synchronize(User info)
        {
            //How do we synchonize the messages?
           
        }
        public void SendMessage(Message msg)
        {
            byte[] temphash={0,0};
            msg.Hash = temphash;
            msg.TimeStamp = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("mh: Asked to send message");            
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
            msg.TimeStamp = DateTime.Now;
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
            if (msg.ReceiverID == null || msg.ReceiverID == "Shout")
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
            this.cm.GroupMessageReceived += new EventHandler<MessageEventArgs>(GroupMessageReceived);
            this.cm.NewConnection += new EventHandler<ConnectionEventArgs>(NewConnectionFound);
            this.cm.FileReceived += new EventHandler<MessageEventArgs>(FileReceived);
            this.cm.NewServerConnection += new EventHandler<ServerConnectionEventArgs>(NewServerConnectionFound);
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
                this.cm.GroupMessageReceived -= new EventHandler<MessageEventArgs>(GroupMessageReceived);
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
