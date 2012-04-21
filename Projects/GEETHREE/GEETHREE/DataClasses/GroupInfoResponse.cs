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
    public class GroupInfoResponse : INotifyPropertyChanged
    {
        
       
            
            private int _groupInfoResponseDbId;

            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int groupInfoResponseDbId
            {
                get
                {
                    return _groupInfoResponseDbId;
                }
                set
                {
                    if (_groupInfoResponseDbId != value)
                    {
                        _groupInfoResponseDbId = value;
                        NotifyPropertyChanged("groupInfoResponseDbId");
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

            [Column]
            private string _senderID;
            /// <summary>
            /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
            /// </summary>
            /// <returns></returns>
            public string SenderID
            {
                get
                {
                    return _senderID;
                }
                set
                {
                    if (value != _senderID)
                    {
                        _senderID = value;
                        NotifyPropertyChanged("SenderID");
                    }
                }
            }

            [Column]
            private string _senderAlias;
            public string SenderAlias
            {
                get
                {
                    return _senderAlias;
                }
                set
                {
                    if (value != _senderAlias)
                    {
                        _senderAlias = value;
                        NotifyPropertyChanged("SenderAlias");
                    }
                }
            }
            
            [Column]
            private string _receiverID;
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

            [Column]
            private string _groupName;
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

            [Column]
            private string _groupID;
            public string GroupID
            {
                get
                {
                    return _groupID;
                }
                set
                {
                    if (value != _groupID)
                    {
                        _groupID = value;
                        NotifyPropertyChanged("GroupID");
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
