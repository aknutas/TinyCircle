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
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using GEETHREE.DataClasses;
using System.Collections.Generic;
using System.Threading;
using System.IO.IsolatedStorage;


namespace GEETHREE
{
    public class G3DataContext : DataContext
    {
        //Should not be needed
        //public static string DBConnectionString = "Data Source='isostore:/G3DB.sdf'";

        // Pass the connection string to the base class.
        public G3DataContext(string connectionString)
            : base(connectionString)
        { }

        public Table<Group> Groups;
        public Table<DataClasses.Image> Images;
        public Table<Me> Me;
        public Table<Message> Messages;
        public Table<User> Users;
        public Table<GroupInfoResponse> GroupInfoResponses;
        public Table<UserInfoResponse> UserInfoResponses;
        public Table<Tags> Tags;
        public Table<TagMessage> TagMessages;
    }

    public class DataMaster
    {
        private Object dblock;
        public FileMaster fm;
        private DataClasses.AppSettings settings;
        private List<Message> draftMessages;
        private G3DataContext db;

        public DataMaster(DataClasses.AppSettings settings)
        {
            dblock = new Object();

            // Create the database if it does not yet exist.
            openDb();
            lock (dblock)
            {
                if (db.DatabaseExists() == false)
                {
                    // Create the database.
                    db.CreateDatabase();
                }
            }
            closeDb();

            fm = new FileMaster();
            this.settings = settings;
            draftMessages = new List<Message>();
        }

        //Closing and disposing of the database resource
        public void closeDb()
        {
                db.Dispose();
                db = null;
        }

        //Closing and disposing of the database resource
        public void closeDbOnExit()
        {
            if (db != null)
            {
                lock (dblock)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void openDb()
        {
            db = new G3DataContext("Data Source='isostore:/G3DB.sdf'");
            db.DeferredLoadingEnabled = false;
        }

        public List<Tags> getAllTags()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Tags tag in db.Tags select tag;
                    List<Tags> returnList = new List<Tags>(qres);
                    closeDb();
                    return returnList;
            }

        }

        public void storeNewTag(Tags tag)
        {
            lock (dblock)
            {
                    openDb();
                    db.Tags.InsertOnSubmit(tag);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void deleteTag(Tags tag)
        {
            lock (dblock)
            {
                openDb();
                db.Tags.DeleteOnSubmit(tag);
                db.SubmitChanges();
                closeDb();
            }
        }

       

        public void storeNewTagMessage(TagMessage tagMessage)
        {
            lock (dblock)
            {
                openDb();
                db.TagMessages.InsertOnSubmit(tagMessage);
                db.SubmitChanges();
                closeDb();
            }
        }

        public void deleteTagMessage(TagMessage tagMessage)
        {
            lock (dblock)
            {
                openDb();
                db.TagMessages.DeleteOnSubmit(tagMessage);
                db.SubmitChanges();
                closeDb();
            }
        }

        public void deleteTagMessagebyTagName(Tags tag)
        {
            lock (dblock)
            {
                openDb();
                var qres = from TagMessage tagMessage in db.TagMessages where tagMessage.TagName == tag.TagName select tagMessage;
                List<TagMessage> returnList = new List<TagMessage>(qres);
                db.TagMessages.DeleteAllOnSubmit(returnList);
                db.SubmitChanges();
                closeDb();
            }
        }

        public List<User> getAllUsers()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from User user in db.Users select user;
                    List<User> returnList = new List<User>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public void storeNewUser(User user)
        {
            lock (dblock)
            {
                    openDb();
                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void deleteUser(User user)
        {
            lock (dblock)
            {
                    openDb();
                    db.Users.DeleteOnSubmit(user);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public List<Message> getAllTagMessages(Tags tag)
        {
            lock (dblock)
            {
                openDb();
                var selectTagMessages = from TagMessage tagMessage in db.TagMessages where tagMessage.TagName == tag.TagName select tagMessage.MessageID;

                var qres = from Message message in db.Messages where selectTagMessages.Contains(message.msgDbId) select message;
                List<Message> returnList = new List<Message>(qres);

                //foreach (TagMessage tagMessage in TagMessageList)
                //{
                //    var qres1 = from Message message in db.Messages where message.msgDbId==tagMessage.MessageID select message;
                //    Message aMessage = qres1.First();
                //    returnList.Add(aMessage);
                //}
                closeDb();
                return returnList;
            }
        }

        public List<Message> getAllMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }


       
        public Message getMessageByMessageID(int msgID)
        {
            lock (dblock)
            {
                openDb();
                var qres = from Message message in db.Messages where message.msgDbId == msgID select message;
                Message aMessage = qres.First();
                closeDb();
                return aMessage;
            }
        }

        public List<GroupInfoResponse> getAllGroupInfoResponses()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from GroupInfoResponse grpinforesponses in db.GroupInfoResponses select grpinforesponses;
                    List<GroupInfoResponse> returnresponseList = new List<GroupInfoResponse>(qres);
                    closeDb();
                    return returnresponseList;
            }
        }


        public void storeNewGroupInfoResponse(GroupInfoResponse groupInfoResponse)
        {
            lock (dblock)
            {
                    openDb();
                    db.GroupInfoResponses.InsertOnSubmit(groupInfoResponse);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void deleteGroupInfoResponse(GroupInfoResponse groupInfoResponse)
        {
            lock (dblock)
            {
                    openDb();
                    db.GroupInfoResponses.DeleteOnSubmit(groupInfoResponse);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public List<UserInfoResponse> getAllUserInfoResponses()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from UserInfoResponse usrinforesponses in db.UserInfoResponses select usrinforesponses;
                    List<UserInfoResponse> returnresponseList = new List<UserInfoResponse>(qres);
                    closeDb();
                    return returnresponseList;
            }
        }


        public void storeNewUserInfoResponse(UserInfoResponse userInfoResponse)
        {
            lock (dblock)
            {
                    openDb();
                    db.UserInfoResponses.InsertOnSubmit(userInfoResponse);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void deleteUserInfoResponse(UserInfoResponse userInfoResponse)
        {
            lock (dblock)
            {
                    openDb();
                    db.UserInfoResponses.DeleteOnSubmit(userInfoResponse);
                    db.SubmitChanges();
                    closeDb();
            }
        }


        public List<Group> getAllGroups()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Group grps in db.Groups select grps;
                    List<Group> returnList = new List<Group>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public void storeNewGroup(Group group)
        {
            lock (dblock)
            {
                    openDb();
                    db.Groups.InsertOnSubmit(group);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void deleteGroup(Group group)
        {
            lock (dblock)
            {
                    openDb();
                    db.Groups.DeleteOnSubmit(group);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public bool checkGroupID(string grpID)
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Group grp in db.Groups where grp.GroupID == grpID select grp;
                    List<Group> returnList = new List<Group>(qres);
                    if (returnList.Count() <= 0)
                    {
                        closeDb();
                        return false;
                    }
                    else
                    {
                        closeDb();
                        return true;
                    }
            }
        }

        public bool checkGroupIDfromGroupInfoResponse(string grpID)
        {
            lock (dblock)
            {
                openDb();
                    var qres = (from GroupInfoResponse grp in db.GroupInfoResponses where grp.GroupID == grpID select grp).Count();
                    if (qres <= 0)
                    {
                        closeDb();
                        return false;
                    }
                    else
                    {
                        closeDb();
                        return true;
                    }
            }
        }

        public bool checkUserIDfromUserInfoResponse(string usrID)
        {
            lock (dblock)
            {
                    openDb();
                    var qres = (from UserInfoResponse usr in db.UserInfoResponses where usr.UserID == usrID select usr).Count();
                    if (qres <= 0)
                    {
                        closeDb();
                        return false;
                    }
                    else
                    {
                        closeDb();
                        return true;
                    }
                        
            }
        }


        public bool checkFriendID(string usrID)
        {
            lock (dblock)
            {
                    openDb();
                    var qres = (from User user in db.Users where user.UserID == usrID select user).Count();
                    if (qres <= 0)
                    {
                        openDb();
                        return false;
                    }
                    else
                    {
                        closeDb();
                        return true;
                    }
            }
        }


        public List<Message> getSendableMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where message.ReceiverID != settings.UserIDSetting select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public List<Message> getSentMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where message.outgoing == true select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public List<Message> getMySentMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where message.SenderID == settings.UserIDSetting select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public List<Message> getMySentBroadcasts()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where (message.SenderID == settings.UserIDSetting && message.PrivateMessage != true) select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public List<Message> getMySentPrivateMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where (message.SenderID == settings.UserIDSetting && message.PrivateMessage == true) select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public List<Message> getIncomingPrivateMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where (message.ReceiverID == settings.UserIDSetting && message.PrivateMessage == true) select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public List<Message> getMyDraftMessages()
        {
            lock (dblock)
            {
                openDb();
                List<Message> returnList = new List<Message>(draftMessages);
                closeDb();
                return returnList;
            }
        }

        public void storeDraftMessage(Message message)
        {
            lock (dblock)
            {
                openDb();
                draftMessages.Add(message);
                closeDb();
                return;
            }
        }

        public void deleteDraftMessage(Message message)
        {
            lock (dblock)
            {
                draftMessages.Remove(message);
                return;
            }
        }

        public void emptyDraftMessages()
        {
            draftMessages = new List<Message>();
        }

        public void storeNewMessage(Message message)
        {
            System.Diagnostics.Debug.WriteLine("Asked to store message");
            if(message.TextContent != null)
                System.Diagnostics.Debug.WriteLine(message.TextContent);

            lock (dblock)
            {
                    openDb();
                    db.Messages.InsertOnSubmit(message);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void storeNewMessage(List<Message> messages)
        {
            System.Diagnostics.Debug.WriteLine("Asked to store a message list");

            lock (dblock)
            {
                    openDb();
                    db.Messages.InsertAllOnSubmit(messages);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void storeNewImage(DataClasses.Image image)
        {
            lock (dblock)
            {
                    openDb();
                    db.Images.InsertOnSubmit(image);
                    db.SubmitChanges();
                    closeDb();
            }
            image.saveBitmapFromStream();
        }

        public List<DataClasses.Image> getAllImages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from DataClasses.Image imgs in db.Images select imgs;
                    List<DataClasses.Image> returnList = new List<DataClasses.Image>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public void deleteMessage(Message message)
        {
            //Debug message
            System.Diagnostics.Debug.WriteLine("DM: Deleting message");

            lock (dblock)
            {
                    openDb();
                    db.Messages.DeleteOnSubmit(message);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void updateMessage(Message message)
        {
            //Debug message
            System.Diagnostics.Debug.WriteLine("DM: Updating message");

            lock (dblock)
            {
                    openDb();
                    //var qres = from DataClasses.Message msgs in db.Messages where message.msgDbId == msgs.msgDbId select msgs;
                    //Message oldmessage = qres.Single();
                    db.Messages.Attach(message, true);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void updateMessage(List<Message> message)
        {
            //Debug message
            System.Diagnostics.Debug.WriteLine("DM: Updating message list");
            
            lock (dblock)
            {
                    openDb();
                    db.Messages.AttachAll(message, true);
                    db.SubmitChanges();
                    closeDb();
            }
        }

        public List<Message> getBroadcastMessages()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Message message in db.Messages where message.PrivateMessage != true select message;
                    List<Message> returnList = new List<Message>(qres);
                    closeDb();
                    return returnList;
            }
        }

        public void updateChanges()
        {
            lock (dblock)
            {
                    //db.SubmitChanges();
            }
        }

        public void resetDataBase()
        {
            lock (dblock)
            {
                    openDb();
                    var qres = from Group grps in db.Groups select grps;
                    foreach (var grp in qres)
                    {
                        db.Groups.DeleteOnSubmit(grp);
                    }

                    var qres3 = from User usr in db.Users select usr;
                    foreach (var usr in qres3)
                    {
                        db.Users.DeleteOnSubmit(usr);
                    }

                    var qres2 = from Message msg in db.Messages select msg;
                    foreach (var msg in qres2)
                    {
                        db.Messages.DeleteOnSubmit(msg);
                    }

                    var qres4 = from GroupInfoResponse grpi in db.GroupInfoResponses select grpi;
                    foreach (var grpi in qres4)
                    {
                        db.GroupInfoResponses.DeleteOnSubmit(grpi);
                    }

                    var qres5 = from UserInfoResponse useri in db.UserInfoResponses select useri;
                    foreach (var useri in qres5)
                    {
                        db.UserInfoResponses.DeleteOnSubmit(useri);
                    }

                    db.SubmitChanges();
                    closeDb();
            }
        }

        public void resetTableGroupAndUserInfoResponse()
        {
            lock (dblock)
            {
                openDb();
                    try
                    {

                        var qres1 = from GroupInfoResponse grpi in db.GroupInfoResponses select grpi;
                        foreach (var grpi in qres1)
                        {
                            db.GroupInfoResponses.DeleteOnSubmit(grpi);
                        }

                        var qres2 = from UserInfoResponse useri in db.UserInfoResponses select useri;
                        foreach (var useri in qres2)
                        {
                            db.UserInfoResponses.DeleteOnSubmit(useri);
                        }
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                    }
                closeDb();
            }
        }

        public void storeObjects(Object o)
        {
            if (o is Message){
                updateMessage((Message) o);
            }
            else if (o is List<Message>)
            {
                updateMessage((List<Message>)o);
            }
            else
            {
                throw new NotImplementedException("Updating not implemented: " + o.GetType().ToString());
            }
        }

        public void updateObjects(List<Object> updateList)
        {
            lock (dblock)
            {
                    db.Refresh(RefreshMode.KeepCurrentValues, updateList);
            }
        }

        public void updateObjects(Object updateObject)
        {
            lock (dblock)
            {
                    db.Refresh(RefreshMode.KeepCurrentValues, updateObject);
            }
        }

    }

}
