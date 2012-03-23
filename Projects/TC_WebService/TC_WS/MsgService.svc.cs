using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

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
                return true;
            }
            catch (Exception)
            {
                return false;
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
