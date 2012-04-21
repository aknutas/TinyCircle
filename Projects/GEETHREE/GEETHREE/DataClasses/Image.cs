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
using System.Windows.Media.Imaging;
using System.IO;

namespace GEETHREE.DataClasses
{
    [Table]
    public class Image
    {
        //Private variables
        private Stream toBeSavedStream;

        // Define ID: private field, public property, and database column.
        private int _photoDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int photoDbId
        {
            get
            {
                return _photoDbId;
            }
            set
            {
                if (_photoDbId != value)
                {
                    _photoDbId = value;
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

        //We really need the image stream, so no constructing without that one
        private Image()
        {
        }

        public Image(Stream istream)
        {
            photoFileName = "img" + Controller.Instance.getNextRandomNumName() + ".gim";

            //No nulls!
            if (istream == null)
                throw new ArgumentNullException();

            this.toBeSavedStream = istream;
        }

        private string _photoFileName;

        [Column]
        public string photoFileName
        {
            get
            {
                return _photoFileName;
            }
            set
            {
                if (_photoFileName != value)
                {
                    _photoFileName = value;
                    //NotifyPropertyChanged("photoFileName");
                }
            }
        }

        public BitmapSource Bitmap
        {
            //TODO read the files and return them
            get { return Controller.Instance.dm.fm.readImageFromFile(photoFileName); }
            private set { }
        }

        public void saveBitmapFromStream()
        {
            Controller.Instance.dm.fm.saveImageToFile(toBeSavedStream, photoFileName);
            toBeSavedStream = null;
        }

    }
}
