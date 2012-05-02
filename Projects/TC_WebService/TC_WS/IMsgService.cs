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
        Boolean postMessage(string receiverId, string senderId, string message, string appKey, DateTime timeStamp);

        [OperationContract]
        List<WireMessage> getMyMessages(string receiverId, string appKey);

        [OperationContract]
        Boolean ping(string appKey);

        [OperationContract]
        void postToastNotificationAddress(string userId, string toastAddress, string appKey);

        [OperationContract]
        void postHandShake(string userId, string userAlias, string password, string appKey);

        [OperationContract]
        List<WireHandShake> discoverHandShakes(string userAlias, string password, string appKey);
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
        [DataMember]
        public DateTime timeStamp { get; set; }
    }

    [DataContract]
    public class WireHandShake
    {
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string Alias { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
