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
using GEETHREE.DataClasses;
using GEETHREE.Networking;

namespace GEETHREE
{
    public class CommunicationHandler : IDisposable
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

        private string messagecache;

        public CommunicationHandler(Controller cm)
        {
            this.Channel = new UdpAnySourceMulticastChannel(GROUP_ADDRESS, GROUP_PORT);

            // Register for events on the multicast channel.
            RegisterEvents();

            // Send a message to the multicast group regularly. This is done because
            // we use UDP unicast messages during the game, sending messages directly to 
            // our opponent. This uses the BeginSendTo method on UpdAnySourceMulticastClient
            // and according to the documentation:
            // "The transmission is only allowed if the address specified in the remoteEndPoint
            // parameter has already sent a multicast packet to this receiver"
            // So, if everyone sends a message to the multicast group, we are guaranteed that this 
            // player (receiver) has been sent a multicast packet by the opponent. 
            StartKeepAlive();
            this.Connections = new ObservableCollection<Connection>();
            this.Join(cm.getCurrentUserID());
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
            if (IsJoined)
            {
                return;
            }

            // Store my player name
            _userID = playerID;

            //Open the connection
            this.Channel.Open();
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
                    default:
                        break;
                }               
            }
            else if (messageParts.Length == 5)
            {
                switch (messageParts[0])
                {
                    case Commands.PartialMessage:
                        HandlePartialMessage(messageParts[1], messageParts[2], messageParts[3], messageParts[4]);
                        break;
                    default:
                        break;
                }

            }
            else if (messageParts.Length == 9)
            {
                switch (messageParts[0])
                {
                    case Commands.PrivateMessage:
                        OnPrivateMessageReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8]);
                        break;
                    case Commands.BroadcastMessage:
                        OnBroadcastMessageReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8]);
                        break;
                    case Commands.PrivateFileMessage:
                        OnFileReceived(messageParts[1], messageParts[2], messageParts[3], messageParts[4], messageParts[5], messageParts[6], messageParts[7], messageParts[8]);
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
        private void OnPrivateMessageReceived(string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, string message, string hash)
        {
            //Message msg = new Message(sender, receiver, message, hash, true);
            EventHandler<MessageEventArgs> handler = this.PrivateMessageReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);

            byte[] storedAttachment = UE.GetBytes(attachment);
            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, sender, senderalias, receiver, attachmentflag, storedAttachment, attachmentfilename, storedHash));
            }
            //DiagnosticsHelper.SafeShow(String.Format("You got a message '{0}'", message));
        }
        private void OnBroadcastMessageReceived(string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, string message, string hash)
        {
            EventHandler<MessageEventArgs> handler = this.BroadcastMessageReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);

            byte[] storedAttachment = UE.GetBytes(attachment);
            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, sender, senderalias, receiver, attachmentflag, storedAttachment, attachmentfilename, storedHash));
            }
        }
        private void OnFileReceived(string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, string message, string hash)
        {
            EventHandler<MessageEventArgs> handler = this.FileReceived;
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] storedHash = UE.GetBytes(hash);

            byte[] storedAttachment = UE.GetBytes(attachment);
            if (handler != null)
            {
                handler(this, new MessageEventArgs(message, sender, senderalias, receiver, attachmentflag, storedAttachment, attachmentfilename, storedHash));
            }
        }

        private void HandlePartialMessage(string sender, string packetno, string nopackets, string content)
        {
            if (Convert.ToInt32(packetno) == 1)
            {
                messagecache.Remove(0);
                messagecache.Insert(messagecache.Length, content.to);
            }
            else
                messagecache.Insert(messagecache.Length, content);
            if (Convert.ToInt32(packetno) == Convert.ToInt32(nopackets))
            {
                 EventHandler<UdpPacketReceivedEventArgs> handler = this.PacketReceived;

                if (handler != null)
                {
                    handler(this, new UdpPacketReceivedEventArgs(messagecache, null));
                }
            }

        }
        public void SendToAll(Message msg)
        {
            if (msg.PrivateMessage == false)
                this.Channel.Send(Commands.BroadcastMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash);
            else
                this.Channel.Send(Commands.PrivateMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash);
        }

        public void SendFileToAll(Message msg)
        {
            if (msg.PrivateMessage == false)
                this.Channel.Send(Commands.BroadcastMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash);
            else
                this.Channel.Send(Commands.PrivateMessageFormat, msg.SenderID, msg.SenderAlias, msg.ReceiverID, msg.Attachmentflag, msg.Attachment, msg.Attachmentfilename, msg.TextContent, msg.Hash);
        }

        public void SendTo(Message msg, string id)
        {
           
            //this.Channel.SendTo(endpoint, Commands.BroadcastMessageFormat, msg.SenderID, msg.ReceiverID, msg.TextContent, msg.Hash);
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
