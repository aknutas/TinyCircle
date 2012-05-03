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
using System.Text.RegularExpressions;

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

        public void postMessage(string userId, string recipient, string messageText, WebServiceReceiver wr, DateTime timeStamp)
        {
            new WSRequest(wr, initMs()).handlePostMessage(userId, recipient, messageText, timeStamp);
        }

        public void testConnection(WebServiceReceiver wr)
        {
            new WSRequest(wr, initMs()).handleTestConnection(this, wr);
        }

        public void registerToast(string subscriptionUri, string userId)
        {
            new WSRequest(initMs()).handleRegisterToast(subscriptionUri, userId);
        }

        public void ShareAlias(string uid, string alias, string passwd)
        {
            new WSRequest(initMs()).handleShareAlias(uid, alias, passwd);
        }
        public void FindFriend(string alias, string passwd, WebServiceReceiver wr)
        {
            new WSRequest(wr, initMs()).handleFindFriend(wr, alias, passwd);
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

            public void handlePostMessage(string userId, string recipient, string messageText, DateTime timeStamp)
            {
                msgService.postMessageCompleted += new EventHandler<postMessageCompletedEventArgs>(msgService_postMessageCompleted);
                try
                {
                msgService.postMessageAsync(recipient, userId, messageText, appKey, timeStamp);
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
                        msg.TimeStamp = wmsg.timeStamp;
                    
                        //split using || for (textcontent||senderalias||attachmentflag||attachment)
                        string MessageContent = wmsg.msgText;
                        if (MessageContent.Length > 1)
                        {
                            int separator = MessageContent.IndexOf("|");
                            if (separator != -1 && MessageContent.Substring(separator+1, 1) == "|")
                            {

                                

                                List<string> partsList = new List<string>();
                                string[] divider = new string[] { "||" };
                                string[] list = MessageContent.Split(divider, StringSplitOptions.None);

                                foreach (string line in list)
                                {
                                    partsList.Add(line);
                                }



                                msg.TextContent = partsList[0];
                                msg.SenderAlias = partsList[1];
                                msg.Attachmentflag = partsList[2];
                                msg.Attachment = partsList[3];

                            }
                            else
                            {
                                msg.TextContent = wmsg.msgText;
                                msg.SenderAlias = "Anonymous";
                                msg.Attachmentflag = "0";
                                msg.Attachment = "0";
                               
                            }



                        }

                       
                        
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
                //Send toast registration to server
                msgService.postToastNotificationAddressAsync(userId, subscriptionUri, appKey);
                System.Diagnostics.Debug.WriteLine("WSC: Registered " + userId + " with address " + subscriptionUri);
            }

            public void handleShareAlias(string uid, string alias, string passwd)
            {

                msgService.postHandShakeCompleted +=new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(msgService_postHandShakeCompleted);
                try
                {
                    msgService.postHandShakeAsync(uid, alias, passwd, appKey);
                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                }
            }

            private void msgService_postHandShakeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    ;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(e.Error.Message.ToString());
                }
            }

            public void handleFindFriend(WebServiceReceiver wr, string alias, string passwd)
            {
                msgService.discoverHandShakesCompleted +=new EventHandler<discoverHandShakesCompletedEventArgs>(msgService_discoverHandShakesCompleted);
                try
                {
                    msgService.discoverHandShakesAsync(alias, passwd, appKey);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                }
            }

            private void msgService_discoverHandShakesCompleted(object sender, discoverHandShakesCompletedEventArgs e)
            {
                string uid;
                string alias;

                if (e.Error == null)
                {
                    if (e.Result.Count > 0)
                    {
                        uid = e.Result[0].UserId;
                        alias = e.Result[0].Alias;
                        wr.webServiceFriendEvent(uid, alias);

                    }
                    else
                    {
                        uid ="0";
                        alias ="0";
                        wr.webServiceFriendEvent(uid, alias);

                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(e.Error.Message.ToString());
                }
            }

        }

    }
}
