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

namespace GEETHREE.DataClasses
{
    [Table]
    public class User : INotifyPropertyChanged
    {

        
        public User(string username, string description)
        {
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
