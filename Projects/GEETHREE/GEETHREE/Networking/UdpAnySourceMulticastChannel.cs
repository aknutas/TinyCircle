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

        private receivebuffer buffer;

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
                    _joinPending = true;
                    this.Client.BeginJoinGroup(new AsyncCallback(OpenCallback), null);
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
                this.Client.MulticastLoopback = true;
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
                    Debug.WriteLine("Send buffer size: " + this.Client.SendBufferSize);
                    Debug.WriteLine("Receive buffer size: " + this.Client.ReceiveBufferSize);
                    Debug.WriteLine(message);
                    //Is the message too big?
                    if (message.Length > 256)
                    {
                        string tmpmsg;
                        string[] messageParts = message.Split(Commands.CommandDelimeter.ToCharArray());
                        int index=0;

                        int nopackages = message.Length / 256 + (message.Length % 256 > 0 ? 1 : 0);
                        for (int i = 0; i < nopackages; i++)
                        {
                            if(index+256<=message.Length)
                                tmpmsg = string.Format(Commands.PartialMessageFormat, messageParts[1], i.ToString(), nopackages.ToString(), message.Substring(index, 256));
                            else
                                tmpmsg = string.Format(Commands.PartialMessageFormat, messageParts[1], i.ToString(), nopackages.ToString(), message.Substring(index));

                            index = index + 256;
                            byte[] data = Encoding.UTF8.GetBytes(tmpmsg);
                            this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
                            
                        }
                    }
                    else
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        this.Client.BeginSendToGroup(data, 0, data.Length, new AsyncCallback(SendToGroupCallback), null);
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
                    DiagnosticsHelper.SafeShow("Not joined!");
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
                   
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    this.Client.BeginSendTo(data, 0, data.Length, endPoint, new AsyncCallback(SendToCallback), null);
                }
                else
                {
                    DiagnosticsHelper.SafeShow("Not joined!");
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
                if (HandlePartialMessage(messageParts[1], messageParts[2], messageParts[3], messageParts[4]))
                {
                    if (buffer.problem == false)
                    {
                        EventHandler<UdpPacketReceivedEventArgs> handler = this.PacketReceived;

                        if (handler != null)
                        {
                            handler(this, new UdpPacketReceivedEventArgs(buffer.buffer, source));
                        }
                    }
                    else
                        Debug.WriteLine("Receiving multi-part message failed");
                }
                
            }
            else
            {
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
        private bool HandlePartialMessage(string sender, string packetno, string nopackets, string content)
        {
            Debug.WriteLine("got package: " + packetno + " of " + nopackets);
            if (buffer == null)
                buffer = new receivebuffer();

            if (Convert.ToInt32(packetno) == 0)
            {
                buffer.buffer = content;
                buffer.currentpackage = 0;
                buffer.numberofpackages = Convert.ToInt32(nopackets);
                buffer.id = sender;
                buffer.problem = false;
            }
            else
            {
                if (sender == buffer.id && Convert.ToInt32(packetno) == buffer.currentpackage + 1)
                {
                    buffer.buffer += content;
                    buffer.currentpackage = Convert.ToInt32(packetno);
                }
                else if (Convert.ToInt32(packetno) != buffer.currentpackage + 1)
                {
                    Debug.WriteLine("Error:wrong package number");
                    buffer.problem = true;
                }
                else
                {
                    Debug.WriteLine("Error: partial message from new source");
                    buffer.problem = true;
                }
            }
            if (Convert.ToInt32(packetno) == Convert.ToInt32(nopackets)-1)
                return true;
            else
                return false;

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
                DiagnosticsHelper.SafeShow("A SocketExeption has occurred. Please make sure your device is on a Wi-Fi network and the Wi-Fi network is operational");
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
                DiagnosticsHelper.SafeShow("An error occurred. Try Again.");
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
                DiagnosticsHelper.SafeShow(socketEx.Message);
            }

        }
    }
    public class receivebuffer
    {
        public string buffer{ get; set; }
        public string id { get; set; }
        public int currentpackage { get; set; }
        public int numberofpackages { get; set; }
        public bool problem { get; set; }
    }
}
