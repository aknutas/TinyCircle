using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEETHREE.Networking
{
    interface WebServiceReceiver
    {
        void webServiceMessageEvent(List<DataClasses.Message> msgList);

        void webServiceMessageSent(Boolean status);
    }
}
