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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Collections.Generic;

namespace GEETHREE.DataClasses
{
    [Table]
    public class Me
    {
        // Define ID: private field, public property, and database column.
        private int _meDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int meDbId
        {
            get
            {
                return _meDbId;
            }
            set
            {
                if (_meDbId != value)
                {
                    _meDbId = value;
                    //NotifyPropertyChanged("meDbId");
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

    }
}
