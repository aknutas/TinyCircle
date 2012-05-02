using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Net;
using System.IO;

namespace TC_WS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MsgService" in code, svc and config file together.
    public class MsgService : IMsgService
    {
        public const string appkey = "abfaxor";

        public Boolean postMessage(string receiverId, string senderId, string messageText, string appKey, DateTime timeStamp)
        {
            if (receiverId == null || messageText == null || senderId == null || timeStamp == null)
                throw new ArgumentNullException();

            if (appKey != appkey)
                throw new InvalidOperationException();

            try
            {
                DataClassesDataContext db = new DataClassesDataContext();
                Message msgobj = new Message();
                msgobj.UserID = receiverId;
                msgobj.Payload = messageText;
                msgobj.SenderID = senderId;
                msgobj.TimeStamp = timeStamp;
                db.Messages.InsertOnSubmit(msgobj);
                db.SubmitChanges();

                var qres = from MsgToast toast in db.MsgToasts where toast.UserID == receiverId select toast;
                System.Diagnostics.Debug.WriteLine("Sending toast notifications to " + qres.Count() + " users.");

                foreach (MsgToast msgtoast in qres) {
                    try
                    {
                        // Get the Uri that the Microsoft Push Notification Service returns to the Push Client when creating a notification channel.
                        // Normally, a web service would listen for Uri's coming from the web client and maintain a list of Uri's to send
                        // notifications out to.
                        string subscriptionUri = msgtoast.ToastAddress;

                        HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

                        // We will create a HTTPWebRequest that posts the toast notification to the Microsoft Push Notification Service.
                        // HTTP POST is the only allowed method to send the notification.
                        sendNotificationRequest.Method = "POST";

                        // The optional custom header X-MessageID uniquely identifies a notification message. 
                        // If it is present, the // same value is returned in the notification response. It must be a string that contains a UUID.
                        // sendNotificationRequest.Headers.Add("X-MessageID", "<UUID>");

                        // Create the toast message.
                        string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\">" +
                           "<wp:Toast>" +
                                "<wp:Text1>" + "TinyCircle" + "</wp:Text1>" +
                                "<wp:Text2>" + "You have a new message!" + "</wp:Text2>" +
                                "<wp:Param>/MainPage.xaml?NavigatedFrom=Toast Notification</wp:Param>" +
                           "</wp:Toast> " +
                        "</wp:Notification>";

                        // Sets the notification payload to send.
                        byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

                        // Sets the web request content length.
                        sendNotificationRequest.ContentLength = notificationMessage.Length;
                        sendNotificationRequest.ContentType = "text/xml";
                        sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
                        sendNotificationRequest.Headers.Add("X-NotificationClass", "2");


                        using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                        {
                            requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                        }

                        // Send the notification and get the response.
                        HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                        string notificationStatus = response.Headers["X-NotificationStatus"];
                        string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                        string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                        System.Diagnostics.Debug.WriteLine("Sent toast notification to " + msgtoast.UserID + " at " + msgtoast.ToastAddress);

                        // Display the response from the Microsoft Push Notification Service.  
                        // Normally, error handling code would be here.  In the real world, because data connections are not always available,
                        // notifications may need to be throttled back if the device cannot be reached.
                        //TextBoxResponse.Text = notificationStatus + " | " + deviceConnectionStatus + " | " + notificationChannelStatus;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception caught sending update: " + ex.ToString());
                        //TextBoxResponse.Text = "Exception caught sending update: " + ex.ToString();
                    }
                }
                

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void postToastNotificationAddress(string userId, string toastAddress, string appKey)
        {
            if (userId == null || toastAddress == null)
                throw new ArgumentNullException();

            if (appKey != appkey)
                throw new InvalidOperationException();

            try
            {
                DataClassesDataContext db = new DataClassesDataContext();

                //Cleanup for old addresses
                var qres = from MsgToast toast in db.MsgToasts where toast.UserID == userId select toast;
                db.MsgToasts.DeleteAllOnSubmit(qres);
                db.SubmitChanges();

                //Add new address
                MsgToast toastadd = new MsgToast();
                toastadd.UserID = userId;
                toastadd.ToastAddress = toastAddress;
                db.MsgToasts.InsertOnSubmit(toastadd);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Got exception " + ex.ToString());
            }

            return;
        }

        public List<WireMessage> getMyMessages(string receiverId, string appKey)
        {
            if (receiverId == null)
                throw new ArgumentNullException();

            if (appKey != appkey)
                throw new InvalidOperationException();

            DataClassesDataContext db = new DataClassesDataContext();
            var qres = from Message message in db.Messages where message.UserID == receiverId select message;

            List<Message> msgList = new List<Message>(qres);
            List<WireMessage> sendMsg = new List<WireMessage>();

            foreach (Message dbMsg in msgList)
            {
                WireMessage wmsg = new WireMessage();
                wmsg.recipientUserId = dbMsg.UserID;
                wmsg.msgText = dbMsg.Payload;
                wmsg.senderUserId = dbMsg.SenderID;
                if (dbMsg.TimeStamp.HasValue)
                    wmsg.timeStamp = dbMsg.TimeStamp.Value;
                else
                    wmsg.timeStamp = DateTime.Now;
                sendMsg.Add(wmsg);
            }

            db.Messages.DeleteAllOnSubmit(msgList);
            db.SubmitChanges();

            var uqu = (from User user in db.Users where user.UserID == receiverId select user).Count();
            if (uqu == 0)
            {
                WireMessage wmsg = new WireMessage();
                wmsg.recipientUserId = receiverId;
                wmsg.msgText = "Welcome to the TinyCircle messaging service!";
                wmsg.senderUserId = "XXXADMINXXX";
                wmsg.timeStamp = DateTime.Now;
                sendMsg.Add(wmsg);

                User user = new User();
                user.UserID = receiverId;
                db.Users.InsertOnSubmit(user);
                db.SubmitChanges();
            }

            return sendMsg;
        }

        public Boolean ping(string appKey)
        {
            if (appKey != appkey)
                throw new InvalidOperationException();

            return true;
        }

        public void postHandShake(string userId, string userAlias, string password, string appKey)
        {
            if (userId == null || userAlias == null || password == null)
                throw new ArgumentNullException();

            if (appKey != appkey)
                throw new InvalidOperationException();

            try
            {
                DataClassesDataContext db = new DataClassesDataContext();

                //Cleanup for old addresses
                var qres = from Handshake hs in db.Handshakes where hs.UserID == userId select hs;
                db.Handshakes.DeleteAllOnSubmit(qres);
                db.SubmitChanges();

                //Add new address
                Handshake newhs = new Handshake();
                newhs.Alias = userAlias;
                newhs.Password = password;
                newhs.UserID = userId;
                db.Handshakes.InsertOnSubmit(newhs);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Got exception " + ex.ToString());
            }

            return;
        }

        public List<WireHandShake> discoverHandShakes(string userAlias, string password, string appKey)
        {
            throw new NotImplementedException();
            return null;
        }
    }
}
