/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace GEETHREE
{
    /// <summary>
    /// This contains the UDP multicast sockets code for joining the group, sending and receiving data.
    /// </summary>
    public class UdpAnySourceMulticastChannel : IDisposable
    {
        /// <summary>
        /// Occurs when a packet is received.
        /// </summary>
        public event EventHandler<UdpPacketReceivedEventArgs> PacketReceived;

        /// <summary>
        /// Occurs when the request to join a multicast group has completed.
        /// </summary>
        public event EventHandler Joined;

        /// <summary>
        /// Occurs before closing or shutting down this instance.
        /// </summary>
        public event EventHandler BeforeClose;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is joined.
        /// </summary>
        /// <value><c>true</c> if this instance is joined; otherwise, <c>false</c>.</value>
        public bool IsJoined { get; private set; }

        /// <summary>
        /// Gets or sets the size of the max message.
        /// </summary>
        /// <value>The size of the max message.</value>
        private byte[] ReceiveBuffer { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        private UdpAnySourceMulticastClient Client { get; set; }

        private MessageBuffer sendbuffer;
        private MessageBuffer receivebuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpAnySourceMulticastChannel"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public UdpAnySourceMulticastChannel(string address, int port)
            : this(IPAddress.Parse(address), port, 1024)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpAnySourceMulticastChannel"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="maxMessageSize">Size of the max message.</param>
        public UdpAnySourceMulticastChannel(IPAddress address, int port, int maxMessageSize)
        {
            this.ReceiveBuffer = new byte[maxMessageSize];
            this.Client = new UdpAnySourceMulticastClient(address, port);
            
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                this.IsDisposed = true;

                if (this.Client != null)
                {
                    this.Client.Dispose();
                    StopkeepAlive();
                }
            }
        }

        // Make sure we don't attempt overlapping join requests.
        bool _joinPending = false;

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            try
            {
                if (!this.IsJoined && !_joinPending)
                {
                    //if (Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsWiFiEnabled)
                    //{
                        _joinPending = true;
                        this.Client.BeginJoinGroup(new AsyncCallback(OpenCallback), null);
                        Debug.WriteLine("Joining network");
                    //}
                    //else
                    //    Debug.WriteLine("Wifi off, not joining network");
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
        }

        void OpenCallback(IAsyncResult result)
        {
            try
            {
                this.Client.EndJoinGroup(result);

                this.IsJoined = true;
                _joinPending = false;

                // We don't want to receive the messages we send.
                this.Client.MulticastLoopback = false;
                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        this.OnJoined();
                        this.Receive();
                    });
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
            
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.OnBeforeClose();
            this.IsJoined = false;
            this.Dispose();
        }

        /// <summary>
        /// Sends the specified format. This is a multicast message that is sent to all members of the multicast group.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void Send(string format, params object[] args)
        {
            try
            {
                if (this.IsJoined)
                {
                    byte[] data = Encoding.UTF8.GetBytes(string.Format(format, args));
                    this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
                }
            }
            catch (SocketException socketEx)
            {

                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("BeginSendToGroup IOE");
            }
        }
        public void Send(string message)
        {
            try
            {
                if (this.IsJoined)
                {
                    Debug.WriteLine("Sending a message of size: " + message.Length);
                    //Is the message too big?
                    if (message.Length > 256)
                    {
                        string tmpmsg;
                        string[] messageParts = message.Split(Commands.CommandDelimeter.ToCharArray());
                        int index=0;

                        int nopackages = message.Length / 256 + (message.Length % 256 > 0 ? 1 : 0);
                        if (sendbuffer == null)
                            sendbuffer = new MessageBuffer();

                        sendbuffer.buffer = message;
                        sendbuffer.numberofpackages = nopackages;
                        sendbuffer.sender = messageParts[1];
                        sendbuffer.problem = false;
                        sendbuffer.currentpackage = 0;

                        if(index+256<=message.Length)
                            tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, sendbuffer.currentpackage.ToString(), nopackages.ToString(), message.Substring(index, 256));
                        else
                            tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, sendbuffer.currentpackage.ToString(), nopackages.ToString(), message.Substring(index));

                       
                        byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                        this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);

                        //StartResendTimer();
                    }
                    else
                    {
                        if (sendbuffer == null)
                            sendbuffer = new MessageBuffer();

                        sendbuffer.buffer = message;
                        sendbuffer.numberofpackages = 1;
                        sendbuffer.problem = false;
                        sendbuffer.currentpackage = 0;

                        byte[] data = Encoding.UTF8.GetBytes(message);
                        this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);

                        //StartResendTimer();
                    }
                }
            }
            catch (SocketException socketEx)
            {

                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("BeginSendToGroup IOE");
            }
        }

        void SendToGroupCallback(IAsyncResult result)
        {
            try
            {
                if (!IsDisposed)
                {
                    this.Client.EndSendToGroup(result);
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
        }

        /// <summary>
        /// Sends the specified format. This is a unicast message, sent to the member of the multicast group at the given endPoint.
        /// </summary>
        /// /// <param name="format">The destination.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void SendTo(IPEndPoint endPoint, string format, params object[] args)
        {
            try
            {
                if (this.IsJoined)
                {

                    string msg = string.Format(format, args);
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    this.Client.BeginSendTo(data, 0, data.Length, endPoint, new AsyncCallback(SendToCallback), null);
                }
                else
                {
                    Debug.WriteLine("Not joined!");
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
        }

        /// <summary>
        /// Sends the specified format. This is a unicast message, sent to the member of the multicast group at the given endPoint.
        /// </summary>
        /// /// <param name="format">The destination.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void SendTo(IPEndPoint endPoint, string message)
        {
            try
            {
                if (this.IsJoined)
                {
                    Debug.WriteLine("Sending a message of size: " + message.Length);
                    //Is the message too big?
                    if (message.Length > 256)
                    {
                        string tmpmsg;
                        string[] messageParts = message.Split(Commands.CommandDelimeter.ToCharArray());
                        int index = 0;

                        int nopackages = message.Length / 256 + (message.Length % 256 > 0 ? 1 : 0);
                        if (sendbuffer == null)
                            sendbuffer = new MessageBuffer();

                        sendbuffer.buffer = message;
                        sendbuffer.numberofpackages = nopackages;
                        sendbuffer.sender = messageParts[1];
                        sendbuffer.problem = false;
                        sendbuffer.currentpackage = 0;

                        if (index + 256 <= message.Length)
                            tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, sendbuffer.currentpackage.ToString(), nopackages.ToString(), message.Substring(index, 256));
                        else
                            tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, sendbuffer.currentpackage.ToString(), nopackages.ToString(), message.Substring(index));


                        byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                        this.Client.BeginSendTo(data, 0, data.Length,endPoint, new AsyncCallback(SendToCallback), null);
                    }
                }
                else
                {
                    Debug.WriteLine("Not joined!");
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
        }

        public void SendPartTo(IPEndPoint endPoint, int nopackage)
        {
            try
            {
                if (this.IsJoined)
                {
                    string tmpmsg;
                    //sendbuffer.currentpackage += 1;
                    if (sendbuffer != null && sendbuffer.buffer != null && sendbuffer.sender != null && nopackage < sendbuffer.numberofpackages)
                    {
                        if (nopackage * 256 + 256 <= sendbuffer.buffer.Length)
                        {
                            try
                            {
                                tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, nopackage.ToString(), sendbuffer.numberofpackages.ToString(), sendbuffer.buffer.Substring(nopackage * 256, 256));
                                byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                                this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
                            }
                            catch (System.ArgumentOutOfRangeException)
                            {
                                Debug.WriteLine("Problem with sendbuffer, aborting send: Requesting part from " + (nopackage * 256) + " to " + (nopackage * 256 + 256) + "size of buffer: " + sendbuffer.buffer.Length);
                            }
                        }
                        else if (nopackage * 256 <= sendbuffer.buffer.Length)
                        {
                            try
                            {
                                tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, nopackage.ToString(), sendbuffer.numberofpackages.ToString(), sendbuffer.buffer.Substring(nopackage * 256));
                            byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                            this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
                            }
                            catch (System.ArgumentOutOfRangeException)
                            {
                                Debug.WriteLine("Problem with sendbuffer, aborting send: Requesting part from " + (nopackage * 256) + " to " + (nopackage * 256 + 256) + "size of buffer: " + sendbuffer.buffer.Length);
                            }
                        }
                        else
                            Debug.WriteLine("Request for non-existing package received");
                    }
                    else
                        Debug.WriteLine("Request for non-existing package received");
                }

                else
                {
                    Debug.WriteLine("Not joined!");
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
        }

        void SendToCallback(IAsyncResult result)
        {
            try
            {
                if (!IsDisposed)
                {
                    this.Client.EndSendTo(result);
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
        }

        /// <summary>
        /// Receives this instance.
        /// </summary>
        private void Receive()
        {
            if (this.IsJoined)
            {
                Array.Clear(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length);
                this.Client.BeginReceiveFromGroup(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length, new AsyncCallback(ReceiveFromGroupCallback), null);
            }
        }

        void ReceiveFromGroupCallback(IAsyncResult result)
        {
            try
            {
                if (!IsDisposed)
                {
                    // We can retrieve the endPoint of the member of the multicast group that sent this message.
                    // We will send this on to the game and it will be stored at part of this member's PlayerInfo. 
                    // It can then be used to send unicast messages, targeted to that one recipient. These messsages
                    // will be sent using the SendTo method above.
                    IPEndPoint source;
                    this.Client.EndReceiveFromGroup(result, out source);
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            this.OnReceive(source, this.ReceiveBuffer);
                            this.Receive();
                        });
                }
            }
            catch (SocketException socketEx)
            {
                // See if we can do something when a SocketException occurs.
                HandleSocketException(socketEx);
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("ReceiveFromGroupCallback IOE");
            }
        }

        /// <summary>
        /// Called when [receive].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="data">The data.</param>
        private void OnReceive(IPEndPoint source, byte[] data)
        {
            string message = Encoding.UTF8.GetString(data, 0, data.Length).Trim('\0');
            string[] messageParts = message.Split(Commands.PackageDelimeter.ToCharArray());
            if (messageParts[0] == Commands.PartialMessage)
            {
                if (HandlePartialMessage(messageParts[1], messageParts[2], messageParts[3], messageParts[4], source))
                {
                    if (receivebuffer.problem == false)
                    {
                        Debug.WriteLine("Received a message of size: " + receivebuffer.buffer.Length);
                        EventHandler<UdpPacketReceivedEventArgs> handler = this.PacketReceived;

                        if (handler != null)
                        {
                            handler(this, new UdpPacketReceivedEventArgs(receivebuffer.buffer, source));
                        }
                    }
                    else
                        Debug.WriteLine("Receiving multi-part message failed");
                }
                
            }
            else if (messageParts[0] == Commands.InfoMessage)
            {
                StopResendTimer();
                if (messageParts[1] == Commands.RequestPart)
                {
                    SendPartTo(source, Convert.ToInt32(messageParts[2]));
                }
                
            }
            else
            {
                //Send receive ok message to stop resendtimer
                string tmpmsg;

                tmpmsg = string.Format(Commands.InfoMessageFormat, Commands.Acknowledgement, 0);

                byte[] msgbytes = Encoding.UTF8.GetBytes(tmpmsg);
                this.Client.BeginSendTo(msgbytes, 0, msgbytes.Length, source, new AsyncCallback(SendToCallback), null);

                EventHandler<UdpPacketReceivedEventArgs> handler = this.PacketReceived;

                if (handler != null)
                {
                    handler(this, new UdpPacketReceivedEventArgs(data, source));
                }
            }
        }

        /// <summary>
        /// Called when [after open].
        /// </summary>
        private void OnJoined()
        {
            EventHandler handler = this.Joined;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [before close].
        /// </summary>
        private void OnBeforeClose()
        {
            EventHandler handler = this.BeforeClose;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        private bool HandlePartialMessage(string sender, string packetno, string nopackets, string content, IPEndPoint source)
        {
            Debug.WriteLine("got package: " + packetno + " of " + nopackets);
            StopResendTimer();
            if (receivebuffer == null)
                receivebuffer = new MessageBuffer();

            if (Convert.ToInt32(packetno) == 0)
            {
                receivebuffer.buffer = content;
                receivebuffer.currentpackage = 0;
                receivebuffer.numberofpackages = Convert.ToInt32(nopackets);
                receivebuffer.sender = sender;
                receivebuffer.problem = false;
                receivebuffer.ready = false;
                receivebuffer.source = source;

                //All ok, request for next part
                string tmpmsg;

                if (Convert.ToInt32(packetno) < Convert.ToInt32(nopackets) - 1)
                {
                    tmpmsg = string.Format(Commands.InfoMessageFormat, Commands.RequestPart, receivebuffer.currentpackage + 1);

                    byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                    this.Client.BeginSendTo(data, 0, data.Length, source, new AsyncCallback(SendToCallback), null);
                }

                //Start the timer for request restarting
                StartKeepAlive();
            }
            else
            {
                if (sender == receivebuffer.sender && Convert.ToInt32(packetno) == receivebuffer.currentpackage + 1)
                {
                    receivebuffer.buffer += content;
                    receivebuffer.currentpackage = Convert.ToInt32(packetno);
                    receivebuffer.problem = false;

                    //All ok, request for next part
                    string tmpmsg;

                    if (Convert.ToInt32(packetno) < Convert.ToInt32(nopackets) - 1)
                    {
                        tmpmsg = string.Format(Commands.InfoMessageFormat, Commands.RequestPart, receivebuffer.currentpackage + 1);

                        byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                        this.Client.BeginSendTo(data, 0, data.Length, source, new AsyncCallback(SendToCallback), null);
                    }

                }
                else if (Convert.ToInt32(packetno) != receivebuffer.currentpackage + 1)
                {
                    Debug.WriteLine("Error:wrong package number");
                    receivebuffer.problem = true;

                    //Request for correct part - No, do not request! Causes way too much traffic with many devices in network!
                   /* string tmpmsg;


                    tmpmsg = string.Format(Commands.InfoMessageFormat, Commands.RequestPart, receivebuffer.currentpackage + 1);

                    byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                    this.Client.BeginSendTo(data, 0, data.Length, source, new AsyncCallback(SendToCallback), null);*/
                }
                else
                {
                    Debug.WriteLine("Error: partial message from new source");
                    receivebuffer.problem = true;
                }
            }
            if (Convert.ToInt32(packetno) == Convert.ToInt32(nopackets) - 1)
            {
                receivebuffer.ready = true;
                StopkeepAlive();
                return true;
            }
            else
                return false;

        }
        public void RequestNextPackage()
        {
            if (receivebuffer!=null && !receivebuffer.ready && (receivebuffer.currentpackage < receivebuffer.numberofpackages))
            {
                string tmpmsg;

                tmpmsg = string.Format(Commands.InfoMessageFormat, Commands.RequestPart, receivebuffer.currentpackage + 1);

                byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                this.Client.BeginSendTo(data, 0, data.Length, receivebuffer.source, new AsyncCallback(SendToCallback), null);
            }
        }

        public void ResendMessage()
        {
            if (sendbuffer != null)
            {
                if (this.IsJoined)
                {
                    Debug.WriteLine("Resending a message of size: " + sendbuffer.buffer.Length);
                    //Is the message too big?
                    if (sendbuffer.buffer.Length > 256)
                    {
                        string tmpmsg;
                        string[] messageParts = sendbuffer.buffer.Split(Commands.CommandDelimeter.ToCharArray());
                        int index = 0;

                        int nopackages = sendbuffer.buffer.Length / 256 + (sendbuffer.buffer.Length % 256 > 0 ? 1 : 0);


                        if (index + 256 <= sendbuffer.buffer.Length)
                            tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, sendbuffer.currentpackage.ToString(), nopackages.ToString(), sendbuffer.buffer.Substring(index, 256));
                        else
                            tmpmsg = string.Format(Commands.PartialMessageFormat, sendbuffer.sender, sendbuffer.currentpackage.ToString(), nopackages.ToString(), sendbuffer.buffer.Substring(index));


                        byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                        this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
                    }
                    else
                    {
                        byte[] data = Encoding.UTF8.GetBytes(sendbuffer.buffer);
                        this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
                    }
                }

            }


        }

        /// <summary>
        /// If a Socketexception occurs, it is possible to handle these exceptions gracefully.
        /// </summary>
        /// <remarks>
        /// This method contains examples of what you can do when a SocketException occurs. 
        /// However, it is not exhaustive and you should handle these exceptions according
        /// to your applications specific behavior.
        /// </remarks>
        /// <param name="socketEx"></param>
        private void HandleSocketException(SocketException socketEx)
        {
            if (socketEx.SocketErrorCode == SocketError.NetworkDown)
            {
                Debug.WriteLine("A SocketExeption has occurred. Please make sure your device is on a Wi-Fi network and the Wi-Fi network is operational");
            }
            else if (socketEx.SocketErrorCode == SocketError.ConnectionReset)
            {
                // Try to re-join the multi-cast group. 
                // No retry count has been implemented here. This is left as an exercise.
                this.IsJoined = false;
                this.Open();
            }
            else if (socketEx.SocketErrorCode == SocketError.AccessDenied)
            {
                Debug.WriteLine("An error occurred. Try Again.");
            }
            else if (socketEx.SocketErrorCode == SocketError.OperationAborted)
            {
                // Try to re-join the multi-cast group. 
                // No retry count has been implemented here. This is left as an exercise.
                this.IsJoined = false;
                this.Open();
               //DiagnosticsHelper.SafeShow(socketEx.Message);
            }
            else
            {
                // Just display the message.
                Debug.WriteLine(socketEx.Message);
            }

        }

        //Timer for requesting more packages
        DispatcherTimer _dt;
        private void StartKeepAlive()
        {
            if (_dt == null)
            {
                _dt = new DispatcherTimer();
                _dt.Interval = new TimeSpan(0, 0, 2);
                _dt.Tick +=
                            delegate(object s, EventArgs args)
                            {
                                if (IsJoined)
                                {
                                    this.RequestNextPackage();
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

        //Timer for requesting more packages
        DispatcherTimer _rstimer;
        private void StartResendTimer()
        {
            if (_rstimer == null)
            {
              
                _rstimer = new DispatcherTimer();
                _rstimer.Interval = new TimeSpan(0, 0, 2);
                _rstimer.Tick +=
                            delegate(object s, EventArgs args)
                            {
                                if (IsJoined)
                                {
                                    this.ResendMessage();
                                }
                            };
            }
            _rstimer.Start();

        }
        private void StopResendTimer()
        {
            if (_rstimer != null)
                _rstimer.Stop();
        }
    }


    public class MessageBuffer
    {
        public IPEndPoint source { get; set; }
        public string buffer{ get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public int currentpackage { get; set; }
        public int numberofpackages { get; set; }
        public bool problem { get; set; }
        public bool ready{ get; set; }
    }
}
