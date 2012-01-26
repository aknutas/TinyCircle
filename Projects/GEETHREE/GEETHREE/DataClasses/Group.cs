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
    public class Group : INotifyPropertyChanged
    {

        // Define ID: private field, public property, and database column.
        private int _groupDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int groupDbId
        {
            get
            {
                return _groupDbId;
            }
            set
            {
                if (_groupDbId != value)
                {
                    _groupDbId = value;
                    NotifyPropertyChanged("groupDbId");
                }
            }
        }

        private string _groupName;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        ///
        [Column]
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (value != _groupName)
                {
                    _groupName = value;
                    NotifyPropertyChanged("GroupName");
                }
            }
        }

        private string _description;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        ///
        [Column]
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
