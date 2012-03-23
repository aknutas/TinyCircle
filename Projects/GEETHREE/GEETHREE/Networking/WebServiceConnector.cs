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
using GEETHREE.DataClasses;
using System.Collections.Generic;
using GEETHREE.MsgServiceReference;

namespace GEETHREE.Networking
{
    public class WebServiceConnector
    {
        //Variables
        MsgServiceReference.MsgServiceClient msgService;

        public WebServiceConnector()
        {
            msgService = new MsgServiceReference.MsgServiceClient();
        }

        public void getMyMessages(string userId, WebServiceReceiver wr)
        {
            new WSRequest(wr, msgService).handleGetMyMessages(userId);
        }

        public void postMessage(string userId, string recipient, string messageText, WebServiceReceiver wr)
        {
            new WSRequest(wr, msgService).handlePostMessage(userId, recipient, messageText);
        }

        private class WSRequest
        {
            private WebServiceReceiver wr;
            private MsgServiceReference.MsgServiceClient msgService;

            public WSRequest(WebServiceReceiver wr, MsgServiceReference.MsgServiceClient msgService)
            {
                this.wr = wr;
                this.msgService = msgService;
            }

            public void handleGetMyMessages(string userId)
            {
                EventHandler<MsgServiceReference.getMyMessagesCompletedEventArgs> eh = new EventHandler<MsgServiceReference.getMyMessagesCompletedEventArgs>(msgService_getMyMessagesCompleted);
                msgService.getMyMessagesCompleted += eh;
                msgService.getMyMessagesAsync(userId);
            }

            public void handlePostMessage(string userId, string recipient, string messageText)
            {
                msgService.postMessageCompleted += new EventHandler<postMessageCompletedEventArgs>(msgService_postMessageCompleted);
                msgService.postMessageAsync(recipient, userId, messageText);
            }

            void msgService_postMessageCompleted(object sender, postMessageCompletedEventArgs e)
            {
                wr.webServiceMessageSent(true);
            }

            void msgService_getMyMessagesCompleted(object sender, MsgServiceReference.getMyMessagesCompletedEventArgs e)
            {
                List<Message> returnMsgs = new List<Message>();
                foreach (WireMessage wmsg in e.Result)
                {
                    Message msg = new Message();
                    msg.ReceiverID = wmsg.recipientUserId;
                    msg.SenderID = wmsg.senderUserId;
                    msg.TextContent = wmsg.msgText;
                    msg.PrivateMessage = true;
                    returnMsgs.Add(msg);
                }

                wr.webServiceMessageEvent(returnMsgs);
            }
        }

    }
}
