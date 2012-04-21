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
    public class TagMessage : INotifyPropertyChanged
    {
        // Define ID: private field, public property, and database column.
        private int _tagMessageDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int tagMessageDbId
        {
            get
            {
                return _tagMessageDbId;
            }
            set
            {
                if (_tagMessageDbId != value)
                {
                    _tagMessageDbId = value;
                    NotifyPropertyChanged("tagMessageDbId");
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

        private string _tagName;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        ///
        [Column]
        public string TagName
        {
            get
            {
                return _tagName;
            }
            set
            {
                if (value != _tagName)
                {
                    _tagName = value;
                    NotifyPropertyChanged("TagName");
                }
            }
        }

        private int _messageID;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        ///
        [Column]
        public int MessageID
        {
            get
            {
                return _messageID;
            }
            set
            {
                if (value != _messageID)
                {
                    _messageID = value;
                    NotifyPropertyChanged("MessageID");
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
