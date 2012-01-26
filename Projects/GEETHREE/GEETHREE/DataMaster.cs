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

        public DataMaster()
        {
            // Create the database if it does not yet exist.
            db = new G3DataContext("isostore:/G3DB.sdf");
            if (db.DatabaseExists() == false)
            {
                // Create the database.
                db.CreateDatabase();
            }
            fm = new FileMaster();
        }

        public List<User> getAllUsers()
        {
            lock (db)
            {
                var qres = from User user in db.Users select user;
                List<User> returnList = new List<User>(qres);
                return returnList;
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

        public List<Message> getAllMessages()
        {
            lock (db)
            {
                var qres = from Message message in db.Messages select message;
                List<Message> returnList = new List<Message>(qres);
                return returnList;
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

        public void updateChanges()
        {
            lock (db)
            {
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
