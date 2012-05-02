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
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using GEETHREE.DataClasses;
using GEETHREE.Networking;

namespace GEETHREE
{
    public class CommunicationHandler : IDisposable, WebServiceReceiver, PushListener
    {
        /// <summary>
        /// All communication takes place using a UdpAnySourceMulticastChannel. 
        /// A UdpAnySourceMulticastChannel is a wrapper we create around the UdpAnySourceMulticastClient.
        /// </summary>
        /// <value>The channel.</value>
        private UdpAnySourceMulticastChannel Channel { get; set; }

        /// <summary>
        /// The IP address of the multicast group. 
        /// </summary>
        /// <remarks>
        /// A multicast group is defined by a multicast group address, which is an IP address 
        /// that must be in the range from 224.0.0.0 to 239.255.255.255. Multicast addresses in 
        /// the range from 224.0.0.0 to 224.0.0.255 inclusive are “well-known” reserved multicast 
        /// addresses. For example, 224.0.0.0 is the Base address, 224.0.0.1 is the multicast group 
        /// address that represents all systems on the same physical network, and 224.0.0.2 represents 
        /// all routers on the same physical network.The Internet Assigned Numbers Authority (IANA) is 
        /// responsible for this list of reserved addresses. For more information on the reserved 
        /// address assignments, please see the IANA website.
        /// http://go.microsoft.com/fwlink/?LinkId=221630
        /// </remarks>
        private const string GROUP_ADDRESS = "224.0.1.11";

        /// <summary>
        /// This defines the port number through which all communication with the multicast group will take place. 
        /// </summary>
        /// <remarks>
        /// The value in this example is arbitrary and you are free to choose your own.
        /// </remarks>
        private const int GROUP_PORT = 54329;

        /// <summary>
        /// The id of this user.
        /// </summary>
        private string _userID;
        private IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;

        public ObservableCollection<Connection> Connections { get; private set; }

        /// <summary>
        /// Occurs when a private message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> PrivateMessageReceived;

        /// <summary>
        /// Occurs when a broadcast message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> BroadcastMessageReceived;

        /// <summary>
        /// Occurs when a file is received.
        /// </summary>
        public event EventHandler<MessageEventArgs>FileReceived;

        /// <summary>
        /// Occurs when a Group message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> GroupMessageReceived;

        /// <summary>
        /// Occurs when a new connection is formed
        /// </summary>
        public event EventHandler<ConnectionEventArgs> NewConnection;

        /// <summary>
        /// Occurs when a new connection to server is formed
        /// </summary>
        public event EventHandler<ServerConnectionEventArgs> NewServerConnection;

        /// <summary>
        /// Occurs when a packet is received.
        /// </summary>
        public event EventHandler<UdpPacketReceivedEventArgs> PacketReceived;

        

        private WebServiceConnector wsConnection;
        private ToastProcessor toaster;


       public List<Group> grpList= new List<Group>();
      

        public CommunicationHandler(Controller cm)
        {
            

            
            this.Connections = new ObservableCollection<Connection>();
            //this.Join(cm.getCurrentUserID());

            grpList.Clear();
            grpList = cm.dm.getAllGroups();
           
        }

        /// <summary>
        /// Whether we are joined to the multicast group
        /// </summary>
        public bool IsJoined { get; private set; }

        /// <summary>
        /// Join the multicast group.
        /// </summary>
        /// <param name="playerName">The player name I want to join as.</param>
        /// <remarks>The player name is not needed for multicast communication. it is 
        /// used in this example to identify each member of the multicast group with 
        /// a friendly name. </remarks>
        public void Join(string playerID)
        {
            if (this.Channel == null)
            {
                this.Channel = new UdpAnySourceMulticastChannel(GROUP_ADDRESS, GROUP_PORT);
                //this.wsConnection = new WebServiceConnector();


                // Register for events on the multicast channel.
                RegisterEvents();
            }

            if (IsJoined)
            {
                return;
            }

            // Store my player name
            _userID = playerID;

            

            //Open the connection
            this.Channel.Open();

            // Send a message to the multicast group regularly. This is done because
            // we use UDP unicast messages during the game, sending messages directly to 
            // our opponent. This uses the BeginSendTo method on UpdAnySourceMulticastClient
            // and according to the documentation:
            // "The transmission is only allowed if the address specified in the remoteEndPoint
            // parameter has already sent a multicast packet to this receiver"
            // So, if everyone sends a message to the multicast group, we are guaranteed that this 
            // player (receiver) has been sent a multicast packet by the opponent. 
            StartKeepAlive();

            //Try also to get the messages from server
            GetMessagesFromServer(playerID);
        }

        /// <summary>
        /// Leave the multicast group. We will not show up in the list of opponents on any
        /// other client devices.
        /// </summary>
        public void Leave(bool disconnect)
        {
            if (this.Channel != null)
            {
                // Tell everyone we have left
                //this.Channel.Send(GameCommands.LeaveFormat, _playerName);

                // Only close the underlying communications channel to the multicast group
                // if disconnect == true.
                if (disconnect)
                {
                    this.Channel.Close();
                }

            }

            // Clear the opponent
            this.IsJoined = false;
        }

        /// <summary>
        /// Register for events on the multicast channel.
        /// </summary>
        private void RegisterEvents()
        {
            // Register for events from the multicast channel
            this.Channel.Joined += new EventHandler(Channel_Joined);
            this.Channel.BeforeClose += new EventHandler(Channel_BeforeClose);
            this.Channel.PacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
        }
        /// <summary>
        /// Unregister for events on the multicast channel
        /// </summary>
        private void UnregisterEvents()
        {
            if (this.Channel != null)
            {
                // Register for events from the multicast channel
                this.Channel.Joined -= new EventHandler(Channel_Joined);
                this.Channel.BeforeClose -= new EventHandler(Channel_BeforeClose);
                this.Channel.PacketReceived -= new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
            }
        }

        /// <summary>
        /// Handles the BeforeClose event of the Channel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Channel_BeforeClose(object sender, EventArgs e)
        {
            this.Channel.Send(String.Format(Commands.Leave, _userID));
        }

        /// <summary>
        /// Handles the Joined event of the Channel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Channel_Joined(object sender, EventArgs e)
        {
            this.IsJoined = true;
            this.Channel.Send(Commands.JoinFormat, _userID);
        }

        /// <summary>
        /// requests for group info
        /// </summary>
        
        public void RequestGroupInfo(string UserID)
        {

            this.Channel.Send(Commands.GroupInfoRequestFormat, UserID);
           
        }

        public void RequestUserInfo(string UserID)
        {

            this.Channel.Send(Commands.UserInfoRequestFormat, UserID);

        }

       

        /// <summary>
        /// Handles the PacketReceived event of the Channel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SilverlightPlayground.UDPMulticast.UdpPacketReceivedEventArgs"/> instance containing the event data.</param>
        void Channel_PacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            string message = e.Message.Trim('\0');
            string[] messageParts = message.Split(Commands.CommandDelimeter.ToCharArray());

            if (messageParts.Length == 2)
            {
                switch (messageParts[0])
                {
                    case Commands.Join:
                        OnUserJoined(new Connection(messageParts[1], e.Source));
                        break;
                    case Commands.Leave:
                        OnUserLeft(new Connection(messageParts[1], e.Source));
                        break;
                    case Commands.GroupInfoRequest:
                        ResponseWithGroupInfo(messageParts[1]);
                        break;

                    case Commands.UserInfoRequest:
                        ResponseWithUserInfo(messageParts[1]);
                        break;

                    default:
                        break;
                }               
            }
            else if (messageParts.Length == 5)
            {
                switch (messageParts[0])
                {
                    case Commands.UserInfoResponse:
                        OnResponseWithUserInfoReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4]);
                        break;
                    default:
                        break;
                }
            }

            else if (messageParts.Length == 7)
            {
                switch (messageParts[0])
                {
                    case Commands.GroupInfoResponse:
                        OnResponseWithGroupInfoReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6]);
                        break;
                    default:
                        break;
                }
            }

            else if (messageParts.Length == 10)
            {
                switch (messageParts[0])
                {
                    case Commands.PrivateMessage:
                        OnPrivateMessageReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8], messageParts[9]);
                        break;
                    case Commands.BroadcastMessage:
                        OnBroadcastMessageReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8], messageParts[9]);
                        break;
                    case Commands.PrivateFileMessage:
                        OnFileReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8], messageParts[9]);
                        break;
                    case Commands.GroupMessage:
                        OnGroupMessageReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8], messageParts[9]);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Ignore messages that don't have the expected number of parts.
            }
        }

        /// <summary>
        /// Handle a player joining the multicast group.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnUserJoined(Connection userInfo)
        {
            bool add = true;
            int numberAdded = 0;

            foreach (Connection pi in this.Connections)
            {
                if (pi.UserID == userInfo.UserID)
                {

                    pi.UserEndPoint = userInfo.UserEndPoint;

                    add = false;
                    break;
                }
            }

            if (add)
            {
                numberAdded++;
                this.Connections.Add(userInfo);
                EventHandler<ConnectionEventArgs> handler = this.NewConnection;

                if (handler != null)
                {
                    handler(this, new ConnectionEventArgs(userInfo.UserID));
                }
            }

            // If any new players have been added, send out our join message again
            // to make sure we are added to their player list.
            if (numberAdded > 0)
            {
                this.Channel.Send(Commands.JoinFormat, _userID);
            }

        }

        /// <summary>
        /// Handle a player leaving the multicast group.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnUserLeft(Connection userInfo)
        {
            if (userInfo.UserID != _userID)
            {
                foreach (Connection pi in this.Connections)
                {
                    if (pi.UserID == userInfo.UserID)
                    {
                        this.Connections.Remove(pi);
                        break;
                    }
                }
            }
        }
        private void OnPrivateMessageReceived(string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, string message, string hash, string timestamp)
        {
            //Message msg = new Message(sender, receiver, message, hash, true);
            EventHandler<MessageEventArgs> handler = this.PrivateMessageReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);
            DateTime dt_time;

            try
            {
                dt_time = DateTime.ParseExact(timestamp, "MM/dd/yyyy HH:mm:ss", null);
            }
            catch (FormatException e)
            {
                dt_time = DateTime.UtcNow;
            }
            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, sender, senderalias, receiver, attachmentflag, attachment, attachmentfilename, storedHash, dt_time));
            }
            //DiagnosticsHelper.SafeShow(String.Format("You got a message '{0}'", message));
        }
        private void OnBroadcastMessageReceived(string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, string message, string hash, string timestamp)
        {
            EventHandler<MessageEventArgs> handler = this.BroadcastMessageReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);
            DateTime dt_time;

            try
            {
                dt_time = DateTime.ParseExact(timestamp, "MM/dd/yyyy HH:mm:ss", null);
            
            }
            catch(FormatException e)
            {
                dt_time = DateTime.UtcNow;
            }

            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, sender, senderalias, receiver, attachmentflag, attachment, attachmentfilename, storedHash, dt_time));
            }
        }
        private void OnFileReceived(string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, string message, string hash, string timestamp)
        {
            EventHandler<MessageEventArgs> handler = this.FileReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);
            DateTime dt_time;

            try
            {
                dt_time = DateTime.ParseExact(timestamp, "MM/dd/yyyy HH:mm:ss", null);
            
            }
            catch (FormatException e)
            {
                dt_time = DateTime.UtcNow;
            }
            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, sender, senderalias, receiver, attachmentflag, attachment, attachmentfilename, storedHash, dt_time));
            }
        }

        private void OnGroupMessageReceived(string GroupID, string groupname, string grpID, string attachmentflag, string attachment, string attachmentfilename, string message, string hash, string timestamp)
        {
            //Message msg = new Message(sender, receiver, message, hash, true);
            EventHandler<MessageEventArgs> handler = this.GroupMessageReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);
            DateTime dt_time;

            try
            {
                dt_time = DateTime.ParseExact(timestamp, "MM/dd/yyyy HH:mm:ss", null);
            
            }
            catch (FormatException e)
            {
                dt_time = DateTime.UtcNow;
            }
            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, GroupID, groupname, grpID, attachmentflag, attachment, attachmentfilename, storedHash, dt_time));
            }
            //DiagnosticsHelper.SafeShow(String.Format("You got a message '{0}'", message));
        }

        private void OnResponseWithGroupInfoReceived(string sender, string senderalias, string receiver, string groupname, string groupid, string description)
        {
            if (sender != receiver)
            {
                //if (Controller.Instance.dm.checkGroupIDfromGroupInfoResponse(groupid) == false)
                //{
                    GroupInfoResponse grpInfo = new GroupInfoResponse();
                    grpInfo.SenderID = sender;
                    grpInfo.SenderAlias = senderalias;
                    grpInfo.ReceiverID = receiver;
                    grpInfo.GroupName = groupname;
                    grpInfo.GroupID = groupid;
                    grpInfo.Description = description;
                    Controller.Instance.dm.storeNewGroupInfoResponse(grpInfo);
                //}
            }
            
            
        }
        private void OnResponseWithUserInfoReceived(string sender, string senderalias, string description, string receiver)
        {
            if (sender != receiver)
            {
                //if (Controller.Instance.dm.checkUserIDfromUserInfoResponse(sender) == false)
                //{

                    UserInfoResponse u = new UserInfoResponse();
                    u.UserID = sender;
                    u.UserAlias = senderalias;
                    u.Description = description;
                    u.ReceiverID = receiver;

                    Controller.Instance.dm.storeNewUserInfoResponse(u);
                //}


               
            }
            
            
        }

        

        public void SendToAll(Message msg)
        {
            if (msg.PrivateMessage == false)
            {
                if (msg.GroupMessage == true)
                    this.Channel.Send(string.Format(Commands.GroupMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash, msg.TimeStamp.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                else
                    this.Channel.Send(string.Format(Commands.BroadcastMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash, msg.TimeStamp.Value.ToString("MM/dd/yyyy HH:mm:ss")));
            }
            else
            {

                this.Channel.Send(string.Format(Commands.PrivateMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash, msg.TimeStamp.Value.ToString("MM/dd/yyyy HH:mm:ss")));
            }
            //Try also to send to server
            if(msg.GroupMessage || msg.PrivateMessage)
                SendToServer(msg);
        }

        public void SendFileToAll(Message msg)
        {
            if (msg.PrivateMessage == false)
            {
                if (msg.GroupMessage == true)
                    this.Channel.Send(string.Format(Commands.GroupMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash, msg.TimeStamp.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                else
                    this.Channel.Send(string.Format(Commands.BroadcastMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash, msg.TimeStamp.Value.ToString("MM/dd/yyyy HH:mm:ss")));
            }
            else
                this.Channel.Send(string.Format(Commands.PrivateMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash, msg.TimeStamp.Value.ToString("MM/dd/yyyy HH:mm:ss")));
        }

        public void ResponseWithGroupInfo(string ReceiverID)
        {
            if (ReceiverID != Controller.Instance.getCurrentUserID())
            {

                foreach (Group g in grpList)
                {
                    this.Channel.Send(string.Format(Commands.GroupInfoResponseFormat, Controller.Instance.getCurrentUserID(), Controller.Instance.getCurrentAlias(), ReceiverID, g.GroupName, g.GroupID, g.Description));
                }
            }

           

        }

        public void ResponseWithUserInfo(string ReceiverID)
        {
            if (ReceiverID != Controller.Instance.getCurrentUserID())
            {

                
                    this.Channel.Send(string.Format(Commands.UserInfoResponseFormat, Controller.Instance.getCurrentUserID(), Controller.Instance.getCurrentAlias(), "User Description", ReceiverID));
                
            }



        }

        public void SendTo(Message msg, string id)
        {
            IPEndPoint endpoint;

            for (int i=0;i<Connections.Count;i++)
            {
                if(Connections[i].UserID==id)
                {
                    endpoint = Connections[i].UserEndPoint;

                    if (msg.PrivateMessage == false)
                     {
                        if (msg.GroupMessage == true)
                            this.Channel.SendTo(endpoint, string.Format(Commands.GroupMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash));
                        else
                            this.Channel.SendTo(endpoint, string.Format(Commands.BroadcastMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash));
                     }
                     else
                        this.Channel.SendTo(endpoint, string.Format(Commands.PrivateMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash));

                    break;
                }
            }
            
            
        }

        public void SendToServer(Message msg)
        {
            string msgContent;
            if(msg.Attachmentflag=="1")
                msgContent = msg.TextContent + "||" + msg.SenderAlias + "||" + msg.Attachmentflag + "||" + msg.Attachment;
            else
                msgContent = msg.TextContent + "||" + msg.SenderAlias + "||0||0" ;
            //wsConnection.testConnection(this);
            if (wsConnection == null)
                wsConnection = new WebServiceConnector();
            if(wsConnection.connectionUp)
                wsConnection.postMessage(msg.SenderID, msg.ReceiverID, msgContent, this, msg.TimeStamp.Value);
        }

        public void GetMessagesFromServer(string uid)
        {
            if (wsConnection == null)
                wsConnection = new WebServiceConnector();

            if (toaster == null)
            {
                //Register for toasts
                toaster = new ToastProcessor(_userID);
                toaster.registerPushListener(this);
                toaster.registerToast();
            }

            wsConnection.testConnection(this);
            // Temporarily disabled check
            // if (wsConnection.connectionUp)
            wsConnection.getMyMessages(uid, this);
        }

        //Callback for push notification
        public void ReceivedServerPush()
        {
            GetMessagesFromServer(_userID);
        }

        //Web service callbacks
        void WebServiceReceiver.webServiceMessageEvent(List<DataClasses.Message> msgList)
        {
            EventHandler<MessageEventArgs> handler = this.PrivateMessageReceived;

            System.Diagnostics.Debug.WriteLine("Got " + msgList.Count + " messages");

            for (int i = 0; i < msgList.Count; i++)
            {
                Message msg = msgList[i];
                DateTime dt_stamp;

                if (msg.TimeStamp.HasValue)

                    dt_stamp = msg.TimeStamp.Value;
                else
                    dt_stamp = DateTime.UtcNow;



                if (handler != null)
                {
                    handler(this, new MessageEventArgs(msg.TextContent, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.Hash, dt_stamp));
                }              
            }
        }

        void WebServiceReceiver.webServiceMessageSent(Boolean status)
        {
            if (status)
                System.Diagnostics.Debug.WriteLine("Message sent to server");
            else
                System.Diagnostics.Debug.WriteLine("Message sending to server failed");
        }

        void WebServiceReceiver.pingFinished(Boolean status)
        {
            EventHandler<ServerConnectionEventArgs> handler = this.NewServerConnection;

            if (status)
            {
                System.Diagnostics.Debug.WriteLine("Connection to server up and running");
            }
            else
                System.Diagnostics.Debug.WriteLine("Connection to server failed");

            if (handler != null)
            {
                handler(this, new ServerConnectionEventArgs(status));
            }
            
        }

        DispatcherTimer _dt;
        private void StartKeepAlive()
        {
            if (_dt == null)
            {
                _dt = new DispatcherTimer();
                _dt.Interval = new TimeSpan(0, 0, 1);
                _dt.Tick +=
                            delegate(object s, EventArgs args)
                            {
                                if (this.Channel != null && IsJoined)
                                {
                                    this.Channel.Send(Commands.ReadyFormat, _userID);
                                }
                            };
            }
            _dt.Start();

        }
        private void StopkeepAlive()
        {
            if (_dt != null)
                _dt.Stop();
        }
        #region IDisposable Implementation
        public void Dispose()
        {
            UnregisterEvents();
            StopkeepAlive();
        }
        #endregion

    }
   
}
