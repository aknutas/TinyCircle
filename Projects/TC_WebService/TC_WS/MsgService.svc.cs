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

        public Boolean postMessage(string receiverId, string senderId, string messageText, string appKey)
        {
            if (receiverId == null || messageText == null || senderId == null)
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
                db.Messages.InsertOnSubmit(msgobj);
                db.SubmitChanges();

                try
                {
                    var qres = from MsgToast toast in db.MsgToasts where toast.UserID == receiverId select toast;
                    if (qres.Count() > 0)
                    {
                        MsgToast msgtoast = qres.Last();
                        HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(msgtoast.ToastAddress);
                        sendNotificationRequest.Method = "POST";
                        string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                            "<wp:Notification xmlns:wp=\"WPNotification\">" +
                            "<wp:Toast>" +
                            "<wp:Text1>" + "TinyCircle" + "</wp:Text1>" +
                            "<wp:Text2>" + "You have a new message" + "</wp:Text2>" +
                            "<wp:Param>/Page2.xaml?NavigatedFrom=Toast Notification</wp:Param>" +
                            "</wp:Toast> " +
                            "</wp:Notification>";
                        byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

                        // Set the web request content length.
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
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception caught sending update: " + ex.ToString());
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

                var qres = from MsgToast toast in db.MsgToasts where toast.UserID == userId select toast;
                db.MsgToasts.DeleteAllOnSubmit(qres);

                MsgToast toastadd = new MsgToast();
                toastadd.UserID = userId;
                toastadd.ToastAddress = toastAddress;
                db.MsgToasts.InsertOnSubmit(toastadd);
                db.SubmitChanges();
                return;
            }
            catch (Exception)
            {
                return;
            }
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
    }
}
