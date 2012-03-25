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
using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace GEETHREE.DataClasses
{
    
    [Table]
    public class UserInfoResponse : INotifyPropertyChanged
    {



        private int _userInfoResponseDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int userInfoResponseDbId
        {
            get
            {
                return _userInfoResponseDbId;
            }
            set
            {
                if (_userInfoResponseDbId != value)
                {
                    _userInfoResponseDbId = value;
                    NotifyPropertyChanged("userInfoResponseDbId");
                }
            }
        }

        [Column]
        private string _userID;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                if (value != _userID)
                {
                    _userID = value;
                    NotifyPropertyChanged("UserID");
                }
            }
        }

        [Column]
        private string _userAlias;
        public string UserAlias
        {
            get
            {
                return _userAlias;
            }
            set
            {
                if (value != _userAlias)
                {
                    _userAlias = value;
                    NotifyPropertyChanged("UserAlias");
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
        private string _receiverID;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string ReceiverID
        {
            get
            {
                return _receiverID;
            }
            set
            {
                if (value != _receiverID)
                {
                    _receiverID = value;
                    NotifyPropertyChanged("ReceiverID");
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
