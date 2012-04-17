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
        private string appKey;
        public Boolean connectionUp { get; set; }

        public WebServiceConnector()
        {
            appKey = DataClasses.AppSettings.appKey;
            connectionUp = false;
        }

        MsgServiceReference.MsgServiceClient initMs()
        {
            return new MsgServiceReference.MsgServiceClient();
        }

        public void getMyMessages(string userId, WebServiceReceiver wr)
        {
            new WSRequest(wr, initMs()).handleGetMyMessages(userId);
        }

        public void postMessage(string userId, string recipient, string messageText, WebServiceReceiver wr)
        {
            new WSRequest(wr, initMs()).handlePostMessage(userId, recipient, messageText);
        }

        public void testConnection(WebServiceReceiver wr)
        {
            new WSRequest(wr, initMs()).handleTestConnection(this, wr);
        }

        public void registerToast(string subscriptionUri, string userId)
        {
            new WSRequest(initMs()).handleRegisterToast(subscriptionUri, userId);
        }

        private class WSRequest
        {
            private WebServiceReceiver wr;
            private WebServiceConnector parent;
            private MsgServiceReference.MsgServiceClient msgService;
            private string appKey = DataClasses.AppSettings.appKey;

            public WSRequest(WebServiceReceiver wr, MsgServiceReference.MsgServiceClient msgService)
            {
                this.wr = wr;
                this.msgService = msgService;
            }

            public WSRequest(MsgServiceReference.MsgServiceClient msgService)
            {
                this.msgService = msgService;
            }

            public void handleGetMyMessages(string userId)
            {
                EventHandler<MsgServiceReference.getMyMessagesCompletedEventArgs> eh = new EventHandler<MsgServiceReference.getMyMessagesCompletedEventArgs>(msgService_getMyMessagesCompleted);
                msgService.getMyMessagesCompleted += eh;
                try
                {
                    msgService.getMyMessagesAsync(userId, appKey);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                }
            }

            public void handlePostMessage(string userId, string recipient, string messageText)
            {
                msgService.postMessageCompleted += new EventHandler<postMessageCompletedEventArgs>(msgService_postMessageCompleted);
                try
                {
                msgService.postMessageAsync(recipient, userId, messageText, appKey);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                }
            }

            private void msgService_postMessageCompleted(object sender, postMessageCompletedEventArgs e)
            {
                if(e.Error==null)
                {
                    if (e.Result == true)
                        wr.webServiceMessageSent(true);
                    else
                        wr.webServiceMessageSent(false);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(e.Error.Message.ToString());
                }
            }

            private void msgService_getMyMessagesCompleted(object sender, MsgServiceReference.getMyMessagesCompletedEventArgs e)
            {
                if(e.Error==null)
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
                else
                {
                    System.Diagnostics.Debug.WriteLine(e.Error.Message.ToString());
                }
            }

            public void handleTestConnection(WebServiceConnector webServiceConnector, WebServiceReceiver wr)
            {
                parent = webServiceConnector;
                msgService.pingCompleted += new EventHandler<pingCompletedEventArgs>(msgService_pingCompleted);
                try
                {
                    msgService.pingAsync(appKey);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                }
            }

            private void msgService_pingCompleted(object sender, pingCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    if (e.Result == true)
                    {
                        parent.connectionUp = e.Result;
                        wr.pingFinished(e.Result);
                    }
                    else
                    {
                        parent.connectionUp = false;
                        wr.pingFinished(false);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(e.Error.Message.ToString());
                }
            }

            public void handleRegisterToast(string subscriptionUri, string userId)
            {
                //TODO
                //Send toast registration to server
            }

        }

    }
}
