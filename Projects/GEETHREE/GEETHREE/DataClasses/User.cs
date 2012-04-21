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

        //Database internal timestamp for change management
        private Binary _dbTimeStamp;

        [Column(IsDbGenerated = true, DbType = "ROWVERSION NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert, IsVersion = true)]
        public Binary DbTimeStamp
        {
            get
            {
                return _dbTimeStamp;
            }
            set
            {
                if (value != _dbTimeStamp)
                {
                    _dbTimeStamp = value;
                    NotifyPropertyChanged("DbTimeStamp");
                }
            }
        }

        //Database internal timestamp for change management
        private Binary _dbTimeStamp;

        [Column(IsDbGenerated = true, DbType = "ROWVERSION NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert, IsVersion = true)]
        public Binary DbTimeStamp
        {
            get
            {
                return _dbTimeStamp;
            }
            set
            {
                if (value != _dbTimeStamp)
                {
                    _dbTimeStamp = value;
                    NotifyPropertyChanged("DbTimeStamp");
                }
            }
        }

        //Constructors

        public User()
        {
        }

        public User(string username, string description)
        {
            //Assigments
            UserName = username;
            Description = description;
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
