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
        // Pass the connection string to the base class.
        public G3DataContext(string connectionString)
            : base(connectionString)
        { }

        public Table<Group> Groups;
        public Table<DataClasses.Image> Images;
        public Table<Me> Me;
        public Table<Message> Messages;
        public Table<User> Users;
    }

    public class DataMaster
    {
        private G3DataContext db;
        public FileMaster fm;
        private DataClasses.AppSettings settings;

        public DataMaster(DataClasses.AppSettings settings)
        {
            // Create the database if it does not yet exist.
            db = new G3DataContext("isostore:/G3DB.sdf");
            if (db.DatabaseExists() == false)
            {
                // Create the database.
                db.CreateDatabase();
            }
            fm = new FileMaster();
            this.settings = settings;
        }

        public List<User> getAllUsers()
        {
            lock (db)
            {
                try
                {
                    var qres = from User user in db.Users select user;
                    List<User> returnList = new List<User>(qres);
                    return returnList;
                }
                catch (Exception)
                {
                    //If query fails bad, return an empty list
                    return new List<User>();
                }
            }
        }

        public void storeNewUser(User user)
        {
            lock (db)
            {
                db.Users.InsertOnSubmit(user);
                db.SubmitChanges();
            }
        }

        public void deleteUser(User user)
        {
            lock (db)
            {
                db.Users.DeleteOnSubmit(user);
                db.SubmitChanges();
            }
        }

        public List<Message> getAllMessages()
        {
            lock (db)
            {
                try
                {
                    var qres = from Message message in db.Messages select message;
                    List<Message> returnList = new List<Message>(qres);
                    return returnList;
                }
                catch (Exception)
                {
                    //If query fails bad, return an empty list
                    return new List<Message>();
                }
            }
        }

        public List<Group> getAllGroups()
        {
            lock (db)
            {
                try
                {
                    var qres = from Group grps in db.Groups select grps;
                    List<Group> returnList = new List<Group>(qres);
                    return returnList;
                }
                catch (Exception)
                {
                    //If query fails bad, return an empty list
                    return new List<Group>();
                }
            }
        }

        public void storeNewGroup(Group group)
        {
            lock (db)
            {
                db.Groups.InsertOnSubmit(group);
                db.SubmitChanges();
            }
        }

        public void deleteGroup(Group group)
        {
            lock (db)
            {
                db.Groups.DeleteOnSubmit(group);
                db.SubmitChanges();
            }
        }

        public List<Message> getSendableMessages()
        {
            lock (db)
            {
                //Bubblegum fix for the not existing table bug
                try
                {
                    var qres = from Message message in db.Messages where message.ReceiverID != settings.UserIDSetting select message;
                    List<Message> returnList = new List<Message>(qres);
                    return returnList;
                }
                catch (Exception)
                {
                    //If query fails bad, return an empty list
                    return new List<Message>();
                }
            }
        }

        public List<Message> getSentMessages()
        {
            lock (db)
            {
                try
                {
                    var qres = from Message message in db.Messages where message.outgoing == true select message;
                    List<Message> returnList = new List<Message>(qres);
                    return returnList;
                }
                catch (Exception)
                {
                    //If query fails bad, return an empty list
                    return new List<Message>();
                }
            }
        }

        public void storeNewMessage(Message message)
        {
            lock (db)
            {
                db.Messages.InsertOnSubmit(message);
                db.SubmitChanges();
            }
        }

        public void storeNewMessage(List<Message> messages)
        {
            lock (db)
            {
                db.Messages.InsertAllOnSubmit(messages);
                db.SubmitChanges();
            }
        }

        public void storeNewImage(DataClasses.Image image)
        {
            lock (db)
            {
                db.Images.InsertOnSubmit(image);
                db.SubmitChanges();
            }
            image.saveBitmapFromStream();
        }

        public List<DataClasses.Image> getAllImages()
        {
            lock (db)
            {
                try
                {
                    var qres = from DataClasses.Image imgs in db.Images select imgs;
                    List<DataClasses.Image> returnList = new List<DataClasses.Image>(qres);
                    return returnList;
                }
                catch (Exception)
                {
                    //If query fails bad, return an empty list
                    return new List<DataClasses.Image>();
                }
            }
        }

        public void deleteMessage(Message message)
        {
            db.Messages.DeleteOnSubmit(message);
            db.SubmitChanges();
        }

        public void updateChanges()
        {
            lock (db)
            {
                db.SubmitChanges();
            }
        }

        public void resetDataBase()
        {
            lock (db)
            {
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
                db.SubmitChanges();
                qres = null;
                qres3 = null;

                var qres2 = from Message msg in db.Messages select msg;
                foreach (var msg in qres2)
                {
                    db.Messages.DeleteOnSubmit(msg);
                }
                db.SubmitChanges();
            }
        }

        public void refreshObjects(List<Object> updateList)
        {
            db.Refresh(RefreshMode.OverwriteCurrentValues, updateList);
        }

        public void refreshObjects(Object updateObject)
        {
            db.Refresh(RefreshMode.OverwriteCurrentValues, updateObject);
        }

    }

}
