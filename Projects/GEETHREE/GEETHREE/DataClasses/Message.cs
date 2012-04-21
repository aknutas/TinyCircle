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
using System.Windows.Media.Imaging;

namespace GEETHREE.DataClasses
{
    [Table]
    public class Message : INotifyPropertyChanged
    {

        // Define ID: private field, public property, and database column.
        private int _msgDbId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int msgDbId
        {
            get
            {
                return _msgDbId;
            }
            set
            {
                if (_msgDbId != value)
                {
                    _msgDbId = value;
                    NotifyPropertyChanged("msgDbId");
                }
            }
        }

        ////Database internal timestamp for change management
        //private DateTime _dbTimeStamp;

        //[Column]
        //public DateTime DbTimeStamp
        //{
        //    get
        //    {
        //        return _dbTimeStamp;
        //    }
        //    set
        //    {
        //        if (value != _dbTimeStamp)
        //        {
        //            _timeStamp = value;
        //            NotifyPropertyChanged("DbTimeStamp");
        //        }
        //    }
        //}

        //Default constructor

        public Message()
        {
            this.user = null;
        }

        private string _header;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (value != _header)
                {
                    _header = value;
                    NotifyPropertyChanged("Header");
                }
            }
        }




        private DateTime? _timeStamp;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public DateTime? TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                if (value != _timeStamp)
                {
                    _timeStamp = value;
                    NotifyPropertyChanged("Timestamp");
                }
            }
        }
        
        private BitmapImage _messageTypeImage;

        public BitmapImage MessageTypeImage
        {
            get
            {
                if (MessageTypeImageURL == null )
                    return null;
                else
       
                    return new BitmapImage(new Uri(MessageTypeImageURL, UriKind.Relative));
            }
            set
            {
                if (value != _messageTypeImage)
                {
                    MessageTypeImageURL = value.UriSource.ToString();
                    NotifyPropertyChanged("MessageTypeImage");
                }
            }
        }

        private string _messageTypeImageURL;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public string MessageTypeImageURL
        {
            get
            {
                return _messageTypeImageURL;
            }
            set
            {
                if (value != _messageTypeImageURL)
                {
                    _messageTypeImageURL = value;
                    NotifyPropertyChanged("MessageTypeImageURL");
                }
            }
        }

        private SolidColorBrush _messageTextColor;

        public SolidColorBrush MessageTextColor
        {
            get
            {
                if (ColorA == null || ColorR == null || ColorG == null || ColorB == null)
                    return null;
                else
                    return new SolidColorBrush(Color.FromArgb(ColorA.Value, ColorR.Value, ColorG.Value, ColorB.Value));
            }
            set
            {
                if (value != _messageTextColor)
                {
                    _messageTextColor = value;
                    ColorA = value.Color.A;
                    ColorR = value.Color.R;
                    ColorG = value.Color.G;
                    ColorB = value.Color.B;
                    NotifyPropertyChanged("MessageTextColor");
                }
            }
        }


        private byte? _colorR;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public byte? ColorR
        {
            get
            {
                return _colorR;
            }
            set
            {
                if (value != _colorR)
                {
                    _colorR = value;
                    NotifyPropertyChanged("ColorR");
                }
            }
        }

        private byte? _colorG;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public byte? ColorG
        {
            get
            {
                return _colorG;
            }
            set
            {
                if (value != _colorG)
                {
                    _colorG = value;
                    NotifyPropertyChanged("ColorG");
                }
            }
        }

        private byte? _colorB;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public byte? ColorB
        {
            get
            {
                return _colorB;
            }
            set
            {
                if (value != _colorB)
                {
                    _colorB = value;
                    NotifyPropertyChanged("ColorB");
                }
            }
        }

        private byte? _colorA;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public byte? ColorA
        {
            get
            {
                return _colorA;
            }
            set
            {
                if (value != _colorA)
                {
                    _colorA = value;
                    NotifyPropertyChanged("ColorA");
                }
            }
        }



        private string _textContent;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public string TextContent
        {
            get
            {
                return _textContent;
            }
            set
            {
                if (value != _textContent)
                {
                    _textContent = value;
                    NotifyPropertyChanged("TextContent");
                }
            }
        }

        // ** flag to derermine if the user has read the message or not
        private bool _isRead;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [Column]
        public bool IsRead
        {
            get
            {
                return _isRead;
            }
            set
            {
                if (value != _isRead)
                {
                    _isRead = value;
                    NotifyPropertyChanged("ISRead");
                }
            }
        }


        private string _attachment;

        [Column(DbType="ntext", UpdateCheck=UpdateCheck.Never)]
        public string Attachment
        {
            get
            {
                return _attachment;
            }
            set
            {
                if (value != _attachment)
                {
                    _attachment = value;
                    NotifyPropertyChanged("Attachment");
                }
            }
        }

        //private byte[] _attachment;

        //[Column]
        //public byte[] Attachment
        //{
        //    get
        //    {
        //        return _attachment;
        //    }
        //    set
        //    {
        //        if (value != _attachment)
        //        {
        //            _attachment = value;
        //            NotifyPropertyChanged("Attachment");
        //        }
        //    }
        //}

        private string _attachmentfilename;

        [Column]
        public string Attachmentfilename
        {
            get
            {
                return _attachmentfilename;
            }
            set
            {
                if (value != _attachmentfilename)
                {
                    _attachmentfilename = value;
                    NotifyPropertyChanged("Attachmentfilename");
                }
            }
        }

        private bool _outgoing;

        [Column]
        public bool outgoing
        {
            get
            {
                return _outgoing;
            }
            set
            {
                if (value != _outgoing)
                {
                    _outgoing = value;
                    NotifyPropertyChanged("outgoing");
                }
            }
        }

        private string _senderid;

        [Column]
        public string SenderID
        {
            get
            {
                return _senderid;
            }
            set
            {
                if (value != _senderid)
                {
                    _senderid = value;
                    NotifyPropertyChanged("SenderID");
                }
            }
        }

        private string _senderalias;

        [Column]
        public string SenderAlias
        {
            get
            {
                return _senderalias;
            }
            set
            {
                if (value != _senderalias)
                {
                    _senderalias = value;
                    NotifyPropertyChanged("SenderAlias");
                }
            }
        }

        private string _receiverid;

        [Column]
        public string ReceiverID
        {
            get
            {
                return _receiverid;
            }
            set
            {
                if (value != _receiverid)
                {
                    _receiverid = value;
                    NotifyPropertyChanged("ReceiverID");
                }
            }
        }

        private byte[] _hash;

        [Column]
        public byte[] Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                if (value != _hash)
                {
                    _hash = value;
                    NotifyPropertyChanged("Hash");
                }
            }
        }

        private bool _privatemessage;

        [Column]
        public bool PrivateMessage
        {
            get
            {
                return _privatemessage;
            }
            set
            {
                if (value != _privatemessage)
                {
                    _privatemessage = value;
                    NotifyPropertyChanged("PrivateMessage");
                }
            }
        }

        private bool _groupmessage;

        [Column]
        public bool GroupMessage
        {
            get
            {
                return _groupmessage;
            }
            set
            {
                if (value != _groupmessage)
                {
                    _groupmessage = value;
                    NotifyPropertyChanged("GroupMessage");
                }
            }
        }

        private string _attachmentflag;

        [Column]
        public string Attachmentflag
        {
            get
            {
                return _attachmentflag;
            }
            set
            {
                if (value != _attachmentflag)
                {
                    _attachmentflag = value;
                    NotifyPropertyChanged("Attachmentflag");
                }
            }
        }

        //Associations with parent (user)

        // Internal column for the associated User ID value
        [Column]
        internal int? _userId;

        // Entity reference, to identify the user "storage" table
        private EntityRef<User> _user;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_user", ThisKey = "_userId", OtherKey = "userDbId", IsForeignKey = true)]
        public User user
        {
            get { return _user.Entity; }
            set
            {
                _user.Entity = value;

                if (value != null)
                {
                    _userId = value.userDbId;
                }

                NotifyPropertyChanged("user");
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
    /// <summary>
    /// Eventargs for events regarding messages received
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public string TextContent { get; set; }
        public string Attachment { get; set; }
        public string Attachmentfilename { get; set; }
        public string Sender { get; set; }
        public string SenderAlias { get; set; }
        public string Receiver { get; set; }
        public string Attachmentflag { get; set; }
        public byte[] Hash { get; set; }

        public MessageEventArgs(string textContent, string sender, string senderalias, string receiver, string attachmentflag, string attachment, string attachmentfilename, byte[] hash)
        {
            this.TextContent = textContent;
            this.Attachmentflag = attachmentflag;
            this.Attachment = attachment;
            this.Attachmentfilename = attachmentfilename;
            this.Sender = sender;
            this.SenderAlias = senderalias;
            this.Receiver = receiver;
            this.Hash = hash;
        }
    }
}
