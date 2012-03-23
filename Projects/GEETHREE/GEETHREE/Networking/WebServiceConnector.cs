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

        List<Message> getMyMessages(string userId)
        {
            return null;
        }

        Boolean postMessage(string userId, string recipient, string messageText)
        {
            return true;
        }

    }
}
