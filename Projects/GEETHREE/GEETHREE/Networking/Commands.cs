/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Net;
using System.Windows;

namespace GEETHREE
{
    /// <summary>
    /// This class holds all the commands a that can be passed between clients of the multicast. 
    /// The goal is to demonstrate the creation of a small protocol for the purpose of communicating ]
    /// and understanding each message. 
    /// </summary>
    public class Commands
    {
        public const string CommandDelimeter = "|";
        public const string Join = "J";
        public const string Leave = "L";
        public const string Ready = "G";
        public const string PrivateMessage = "P";
        public const string BroadcastMessage = "B";
        public const string PrivateFileMessage = "F";
        public const string PartialMessage = "O";

        public const string JoinFormat = Join + CommandDelimeter + "{0}";
        public const string LeaveFormat = Leave + CommandDelimeter + "{0}";
        public const string ReadyFormat = Ready + CommandDelimeter + "{0}";
        public const string PrivateMessageFormat = PrivateMessage + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}" + CommandDelimeter + "{3}" + CommandDelimeter + "{4}" + CommandDelimeter + "{5}" + CommandDelimeter + "{6}" + CommandDelimeter + "{7}";//senderID + senderAlias + receiverID + attachmentflag + storedAttachment + attachmentfilename + message + hash
        public const string BroadcastMessageFormat = BroadcastMessage + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}" + CommandDelimeter + "{3}" + CommandDelimeter + "{4}" + CommandDelimeter + "{5}" + CommandDelimeter + "{6}" + CommandDelimeter + "{7}"; //senderID + senderAlias + receiverID + attachmentflag + storedAttachment + attachmentfilename + message + hash
        public const string PrivateFileMessageFormat = PrivateFileMessage + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}" + CommandDelimeter + "{3}" + CommandDelimeter + "{4}" + CommandDelimeter + "{5}" + CommandDelimeter + "{6}" + CommandDelimeter + "{7}";//senderID + senderAlias + receiverID + file contents + hash
        public const string PartialMessageFormat = PartialMessageFormat + CommandDelimeter + "{0}" + CommandDelimeter + "{1}" + CommandDelimeter + "{2}" + CommandDelimeter + "{3}";//senderID + package number + number of packages + content
    }
}
