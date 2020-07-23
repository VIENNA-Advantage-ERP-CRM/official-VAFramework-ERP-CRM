using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Utility
{
    [Serializable]
    public class EmailMessageSerializable
    {
        #region PrivateVarisables

        private string _to;
        private string _from;
        private string _subject;
        private string _message;
        List<byte[]> _attachment;//attachment in byte form
        private List<string> _fileName;//attachment name
        string[] _bccArray;
        string[] _ccArray;

        #endregion

        /// <summary>
        /// default constructor
        /// </summary>
        public EmailMessageSerializable()
        {
        }

        /// <summary>
        /// mail to
        /// </summary>
        public string TO
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        /// <summary>
        /// mail from
        /// </summary>
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        /// <summary>
        /// mail Subject
        /// </summary>
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        /// <summary>
        /// mail Messahe
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        /// <summary>
        /// Byte array list for attachment
        /// </summary>
        public List<byte[]> Attachment
        {
            get
            {
                return _attachment;
            }
            set
            {
                _attachment = value;
            }
        }

        /// <summary>
        /// attachment list name
        /// </summary>
        public List<string> FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        /// <summary>
        /// mail Bcc
        /// </summary>
        public string[] Bcc
        {
            get
            {
                return _bccArray;
            }
            set
            {
                _bccArray = value;
            }
        }

        /// <summary>
        /// mail CC
        /// </summary>
        public string[] Cc
        {
            get
            {
                return _ccArray;
            }
            set
            {
                _ccArray = value;
            }
        }
    }
}
