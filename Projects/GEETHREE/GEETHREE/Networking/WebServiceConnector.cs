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
            throw new NotImplementedException();
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

            void msgService_getMyMessagesCompleted(object sender, MsgServiceReference.getMyMessagesCompletedEventArgs e)
            {
                throw new NotImplementedException();
            }
        }

    }
}
