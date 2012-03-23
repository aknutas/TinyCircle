using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace TC_WS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMsgService" in both code and config file together.
    [ServiceContract]
    public interface IMsgService
    {
        [OperationContract]
        Boolean postMessage(string receiverId, string senderId, string message, string appKey);

        [OperationContract]
        List<WireMessage> getMyMessages(string receiverId, string appKey);

        [OperationContract]
        Boolean ping(string appKey);
    }

    [DataContract]
    public class WireMessage
    {
        [DataMember]
        public string senderUserId { get; set; }
        [DataMember]
        public string recipientUserId { get; set; }
        [DataMember]
        public string msgText { get; set; }
    }
}
