/********************************************************
 * Module Name    : Attachment
 * Purpose        : Contains functions used to get details of a single file attached in attachments.
 * Class Used     : ---
 * Chronological Development
 * Veena Pandey     10-March-2009
 ******************************************************/

using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    /// <summary>
    /// A class which contains functions used to get details of a single file attached in attachments.
    /// </summary>
    public class MAttachmentEntry
    {
        // Name
        private string _name = "";
        // binary data
        private byte[] _data = null;
        // Random Seed
        private static long _seed = long.Parse(System.DateTime.Now.Millisecond.ToString());
        // Random Number
        private Random _random = new Random(int.Parse(_seed.ToString()));
        // Index
        private int _index = 0;

        public string[,] _types = new string[,] { 
            { ".asp", "text/asp"},
            { ".asx", "application/x-mplayer2"}, 
            { ".asx", "video/x-ms-asf"},
            { ".avi", "video/avi"}, 
            { ".bin", "application/x-binary"},
            { ".bmp", "image/bmp"}, 
            { ".c", "text/plain"}, 
            { ".c++", "text/plain"},
            { ".cc", "text/plain"},
            { ".chat", "application/x-chat"},
            { ".class", "application/x-java-vm "},
            { ".com", "application/octet-stream"}, 
            { ".cpp", "text/x-c"}, 
            { ".css", "text/css"},
            { ".cvs", "application/vnd.ms-excel"},
            { ".dir", "application/x-director"}, 
            { ".doc", "application/msword"},
            { ".exe", "application/octet-stream"},
            { ".gif", "image/gif"}, 
            { ".gz", "application/x-gzip"},
            { ".gzip", "application/x-gzip"}, 
            { ".help", "application/x-helpfile"},
            { ".htm", "text/html"},
            { ".html", "text/html"}, 
            { ".htmls", "text/html"},	
            { ".jav", "text/x-java-source"}, 
            { ".java", "text/x-java-source"},
            { ".jar", "application/java-archive"},
            { ".jpe", "image/jpeg"}, 
            { ".jpeg", "image/jpeg"}, 
            { ".jpg", "image/jpeg"}, 
            { ".log", "text/plain"}, 
            { ".mp3", "audio/mpeg3"}, 
            { ".mpeg", "video/mpeg"}, 
            { ".mpg", "video/mpeg"}, 
            { ".pdf", "application/pdf"},
            { ".png", "image/png"},
            { ".pps", "application/mspowerpoint"},
            { ".ppt", "application/mspowerpoint"},
            { ".qt", "video/quicktime"}, 
            { ".rtf", "application/rtf"},
            { ".shtml", "text/html"}, 
            { ".shtml", "text/x-server-parsed-html"},
            { ".text", "text/plain"},
            { ".txt", "text/plain"}, 
            { ".word", "application/msword"}, 
            { ".xls", "application/vnd.ms-excel"},
            { ".xml", "text/xml"}, 
            { ".zip", "application/zip"}
        };


        //public MAttachmentEntry(string name, byte[] data)
        //{
        //    new MAttachmentEntry(name, data, 0);
        //}
        protected VLogger _log = null;
	
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="data">data</param>
        /// <param name="index">index</param>
        public MAttachmentEntry(string name, byte[] data, int index)
	    {
            if (_log == null)
            { 
            _log = VLogger.GetVLogger(this.GetType().FullName);
	
            }
            // set name
		    SetName (name);
            // set data
		    SetData (data);
            if (index > 0)
                _index = index;
            else
            {
                long now = long.Parse(System.DateTime.Now.Millisecond.ToString());
                // if older then 1 hour
                if (_seed + 36000001 < now)
                {
                    _seed = now;
                    _random = new Random(int.Parse(_seed.ToString()));
                }
                _index = _random.Next();
            }
	    }

        /// <summary>
        /// Get Data
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetData()
        {
            return _data;
        }

        /// <summary>
        /// Set Data
        /// </summary>
        /// <param name="data">data</param>
        public void SetData(byte[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Get Name
        /// </summary>
        /// <returns>string</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name">name</param>
        public void SetName(string name)
        {
            if (name != null)
                _name = name;
            if (_name == null)
                _name = "";
        }

        /// <summary>
        /// Get Index
        /// </summary>
        /// <returns>int</returns>
        public int GetIndex()
        {
            return _index;
        }

        /// <summary>
        /// Get Information of a file entry
        /// </summary>
        /// <returns>string</returns>
        public string GetEntryInfo()
        {
            StringBuilder sb = new StringBuilder(_name);
            if (_data != null)
            {
                sb.Append(" (");
                // get size
                float size = _data.Length;
                if (size <= 1024)
                    sb.Append(_data.Length).Append(" B");
                else
                {
                    size /= 1024;
                    if (size > 1024)
                    {
                        size /= 1024;
                        sb.Append(size).Append(" MB");
                    }
                    else
                        sb.Append(size).Append(" kB");
                }
                //
                sb.Append(")");
            }
            // also append content type of the file
            sb.Append(" - ").Append(GetContentType(_name));
            return sb.ToString();
        }

        /// <summary>
        /// Get content type of the given file
        /// </summary>
        /// <param name="fileName">file name with extension</param>
        /// <returns>string</returns>
        private string GetContentType(string fileName)
        {
            if (fileName == null || fileName.IndexOf('.') < 0)
                return "application/octet-stream";
            // get extension of the file
            string extension = fileName.Substring(fileName.LastIndexOf('.'));
            int totCnt = _types.GetUpperBound(0);
            for (int i = 0; i < totCnt; i++)
            {
                string type = _types[i, 0];
                if (type.Equals(extension))
                    return _types[i, 1];
            }
            return "application/octet-stream";
        }

        /// <summary>
        /// Save File to harddisk at given path
        /// </summary>
        /// <param name="filePath">out file</param>
        /// <returns>file</returns>
        public object SaveFile(string filePath)
        {
            // if no data, return
            if (_data == null || _data.Length == 0)
                return null;
            try
            {
                // create file at specified path
                FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fOutStream);
                // write the binary data onto the file
                bw.Write(_data, 0, _data.Length);
                bw.Flush();
                bw.Close();
            }
            catch (Exception e)
            {
                VLogger.Get().Severe("AttachmentEntrySave" + e.Message);
            }
            return "1";
        }

        //public bool IsPDF()
        //{
        //    string fileName = _name.ToLower();
        //    return fileName.EndsWith(".pdf");
        //}

        /// <summary>
        /// Tells if file is an image file
        /// </summary>
        /// <returns>bool type</returns>
        public bool IsGraphic()
        {
            string fileName = _name.ToLower();
            return fileName.EndsWith(".gif") || fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpe") || fileName.EndsWith(".png") || fileName.EndsWith(".bmp");
        }

        //public Stream GetInputStream()
        //{
        //    if (_data == null)
        //    {
        //        return null;
        //    }
        //    //Stream st = null;
        //    Stream input = null;
        //    Stream output = null;
        //    int numBytes;
        //    //using (MemoryStream ms = new MemoryStream(imageData, 0, imageData.Length))
        //    //    output.Write(_data, 0, numBytes);
        //    while ((numBytes = input.Read(_data, 0, _data.Length)) > 0)
        //        output.Write(_data, 0, numBytes);

        //    //st.Write(_data, 0, _data.Length);

        //    return output;
        //}
    }
}
