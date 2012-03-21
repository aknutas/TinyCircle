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
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GEETHREE.DataClasses
{
    [Table]
    public class User : INotifyPropertyChanged
    {

        //Constructors
        public User()
        {
            // Assign handlers for the add and remove operations, respectively.
            _messages = new EntitySet<Message>(
                new Action<Message>(this.attach_User),
                new Action<Message>(this.detach_User)
                );
            CreateNewUserID();
        }

        public User(string username, string description)
        {
            // Assign handlers for the add and remove operations, respectively.
            _messages = new EntitySet<Message>(
                new Action<Message>(this.attach_User),
                new Action<Message>(this.detach_User)
                );

            //Assigments
            UserName = username;
            Description = description;
        }

        // Define ID: private field, public property, and database column.
        private int _userDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int userDbId
        {
            get
            {
                return _userDbId;
            }
            set
            {
                if (_userDbId != value)
                {
                    _userDbId = value;
                    NotifyPropertyChanged("userDbId");
                }
            }
        }

        [Column]
        private string _userName;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }


        [Column]
        private string _description;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        [Column]
        private string _userid;
        public string UserID
        {
            get
            {
                return _userid;
            }
            set
            {
                if (value != _userid)
                {
                    _userid = value;
                    NotifyPropertyChanged("UserID");
                }
            }
        }

        // Define the entity set for the collection side of the relationship.
        private EntitySet<Message> _messages;

        [Association(Storage = "_messages", OtherKey = "_userId", ThisKey = "userDbId")]
        public EntitySet<Message> messages
        {
            get { return this._messages; }
            set { this._messages.Assign(value); }
        }

        // Called during an add operation
        private void attach_User(Message msg)
        {
            msg.user = this;
        }

        // Called during a remove operation
        private void detach_User(Message msg)
        {
            msg.user = null;
        }

        public static string CreateNewUserID()
        {
            string id;
           

            byte[] mac = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            byte[] time = System.BitConverter.GetBytes(System.DateTime.Now.Ticks);

            var source = new List<byte>();
            source.AddRange(mac);
            source.AddRange(time);

            HMACSHA256 sha = new HMACSHA256();
            byte[] hashBytes = sha.ComputeHash(source.ToArray());

            id = Convert.ToBase64String(hashBytes);


            return id;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
