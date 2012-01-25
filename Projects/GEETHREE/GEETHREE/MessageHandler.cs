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

        public void PrivateMessageReceived(Message msg)
        {
            //Is the message for me?
            //Can we forward the message?

        }
        public void BroadcastMessageReceived(Message msg)
        {
        }
        public void NewConnectionFound(User info)
        {
            //Can we forward some stored messages?
        }
        public void Synchronize(User info)
        {
            //How do we synchonize the messages?
           
        }
    }
}
