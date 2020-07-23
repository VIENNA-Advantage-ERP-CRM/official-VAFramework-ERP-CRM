/********************************************************
 * Module Name    : MailAttachment
 * Purpose        : save Mail attachment on mail send.
 * Class Used     : X_MailAttachment1,MAttachmentEntry
 * Chronological Development
 * Raghunandan    3rd-April-2010
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using java.util.zip;
using java.io;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMailAttachment1 : X_MailAttachment1
    {

        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MMailAttachment1).FullName);

        List<MAttachmentEntry> _items = null;
        // Indicator for no data
        //private const string NONE = ".";
        // Indicator for zip data
        private const string ZIP = "zip";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID</param>
        /// <param name="Record_ID">Record_ID</param>
        /// <param name="trxName">transaction</param>
        public MMailAttachment1(Ctx ctx, int AD_Table_ID, int Record_ID, Trx trxName)
            : base(ctx, 0, trxName)
        {
            //this(ctx, 0, trxName);
            // set table id
            SetAD_Table_ID(AD_Table_ID);
            // set record id
            SetRecord_ID(Record_ID);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Attachment_ID">AD_Attachment_ID</param>
        /// <param name="trxName">transaction</param>
        public MMailAttachment1(Ctx ctx, int AD_Attachment_ID, Trx trxName)
            : base(ctx, AD_Attachment_ID, trxName)
        {
            //GetLines();
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MMailAttachment1(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get Attachment
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <returns>attachment or null</returns>
        public static MMailAttachment1 Get(Ctx ctx, int AD_Table_ID, int Record_ID)
        {
            MMailAttachment1 retValue = null;
            String sql = "SELECT * FROM mailattachment1 WHERE AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID + "";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MMailAttachment1(ctx, dr, null);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Add a new entry
        /// </summary>
        /// <param name="file">file path</param>
        /// <returns>bool type true if file added sucessfully</returns>
        public bool AddEntry(string filePath)
        {
            if (filePath == null)
            {
                log.Warning("No File");
                return false;
            }
            string fullName = filePath;
            string[] splitName = fullName.Split('\\');
            string name = splitName[splitName.Length - 1];

            //Initialize byte array with a null value initially.
            byte[] data = null;
            //string comment = filePath.GetType().ToString();
            try
            {
                // can use net methods to convert to byte[]
                //java.io.FileInputStream fis = new java.io.FileInputStream(filePath);
                //ByteArrayOutputStream os = new ByteArrayOutputStream();
                //byte[] buffer = new byte[1024 * 8];   //  8kB
                //int length = -1;
                //while ((length = fis.read(buffer)) != -1)
                //{
                //    os.write(buffer, 0, length);
                //}
                //fis.close();
                //byte[] objData = os.toByteArray();
                //data = ConvertToByte(objData);
                //os.close();

                data = null;
                //Use FileInfo object to get file size.
                System.IO.FileInfo fInfo = new System.IO.FileInfo(filePath);
                long numBytes = fInfo.Length;

                //Open FileStream to read file
                System.IO.FileStream fStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                //Use BinaryReader to read file stream into byte array.
                System.IO.BinaryReader br = new System.IO.BinaryReader(fStream);

                //When you use BinaryReader, you need to supply number of bytes to read from file.
                //In this case we want to read entire file. So supplying total number of bytes.
                data = br.ReadBytes((int)numBytes);
                br.Close();
                fStream.Close();

            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "(file)", ex);
            }
            return AddEntry(name, data);
        }


        public void InitEntries()
        {
            if (_items == null)
            {
                _items = new List<MAttachmentEntry>();
            }
            _items.Clear();
        }



        /// <summary>
        /// Add a new entry
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="data">data</param>
        /// <returns>bool type true if file added sucessfully</returns>
        public bool AddEntry(string name, byte[] data)
        {
            if (name == null || data == null)
            {
                return false;
            }
            return AddEntry(new MAttachmentEntry(name, data, 0));	//	random number at index 0
        }

        /// <summary>
        /// Add a new entry
        /// </summary>
        /// <param name="item">MMailAttachment1 object item</param>
        /// <returns>bool type true if file added sucessfully</returns>
        public bool AddEntry(MAttachmentEntry item)
        {
            if (item == null)
            {
                return false;
            }
            if (_items == null)
            {
                // load data
                LoadLOBData();
            }

            bool retValue = false;

            try
            {
                _items.Add(item);

                retValue = true;
            }
            catch 
            { }
            log.Fine(item.ToString());
            AddTextMsg(" ");	//	otherwise not saved
            return retValue;
        }

        /// <summary>
        /// Appends text message to existing text message
        /// </summary>
        /// <param name="added">text message</param>
        public void AddTextMsg(string added)
        {
            string oldTextMsg = GetTextMsg();
            if (oldTextMsg == null)
                SetTextMsg(added);
            else if (added != null)
                SetTextMsg(oldTextMsg + added);
        }

        //public string GetTextMsg()
        //{
        //    string msg = base.GetTextMsg();
        //    if (msg == null)
        //        return null;
        //    return msg.Trim();
        //}

        /// <summary>
        /// Get Entry at specified index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        public MAttachmentEntry GetEntry(int index)
        {
            if (_items == null)
            {
                // load data
                LoadLOBData();
            }
            if (index < 0 || index >= _items.Count)
            {
                return null;
            }
            return (MAttachmentEntry)_items[index];
        }

        public MAttachmentEntry[] GetEntries()
        {
            if (_items == null)
            {
                LoadLOBData();
            }
            MAttachmentEntry[] retValue = new MAttachmentEntry[_items.Count];
            retValue = _items.ToArray();
            return retValue;
        }

        /// <summary>
        /// Remove an entry at given index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>bool type true if removed successfully</returns>
        public bool DeleteEntry(int index)
        {
            if (_items == null)
            {
                // load data
                LoadLOBData();
            }
            if (index >= 0 && index < _items.Count)
            {
                // remove entry
                _items.RemoveAt(index);
                log.Config("Index=" + index + " - NewSize=" + _items.Count);
                return true;
            }
            log.Warning("Not deleted Index=" + index + " - Size=" + _items.Count);
            return false;
        }

        /// <summary>
        /// Get total number of entries
        /// </summary>
        /// <returns>int</returns>
        public int GetEntryCount()
        {
            if (_items == null)
            {
                // load data
                LoadLOBData();
            }
            return _items.Count;
        }

        /// <summary>
        /// Get file name of the entry at given index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>string</returns>
        public string GetEntryName(int index)
        {
            // get item
            MAttachmentEntry item = GetEntry(index);
            // get name
            if (item != null)
                return item.GetName();
            return null;
        }

        /// <summary>
        /// Get file data of the entry at given index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>byte[] data</returns>
        public byte[] GetEntryData(int index)
        {
            // get item
            MAttachmentEntry item = GetEntry(index);
            // get data
            if (item != null)
                return item.GetData();
            return null;
        }

        /// <summary>
        /// Saves the file at given index to harddisk at given path
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="filePath">file path</param>
        public void SaveEntryFile(int index, string filePath)
        {
            // get item
            MAttachmentEntry item = GetEntry(index);
            // save file
            if (item != null)
                item.SaveFile(filePath);
            //return null;
        }

        /// <summary>
        /// This function is called before final saving of records
        /// </summary>
        /// <returns>bool type true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetTitle() == null || !GetTitle().Equals(ZIP))
            {
                //comented becouse title text change here to zip
                // SetTitle(ZIP);
            }
            //	save in BinaryData
            return SaveLOBData();
        }

        /// <summary>
        /// Save Entry Data in Zip File format
        /// </summary>
        /// <returns>bool type true if saved</returns>
        private bool SaveLOBData()
        {
            if (_items == null || _items.Count == 0)
            {
                // if no items
                // Set binary data in PO and return
                SetBinaryData(null);
                return true;
            }
            ByteArrayOutputStream bOut = new ByteArrayOutputStream();
            // initialize zip
            ZipOutputStream zip = new ZipOutputStream(bOut);
            zip.setMethod(ZipOutputStream.DEFLATED);
            zip.setLevel(Deflater.BEST_COMPRESSION);
            //
            try
            {
                // for every item in list
                int isize = _items.Count;
                for (int i = 0; i < isize; i++)
                {
                    // get item
                    MAttachmentEntry item = GetEntry(i);
                    // make zip entry
                    ZipEntry entry = new ZipEntry(item.GetName());
                    // set time
                    entry.setTime(long.Parse(System.DateTime.Now.Millisecond.ToString()));
                    entry.setMethod(ZipEntry.DEFLATED);
                    // start setting zip entry into zip file
                    zip.putNextEntry(entry);
                    byte[] data = item.GetData();
                    object obj = (object)data;
                    // set data into zip
                    zip.write((byte[])obj, 0, data.Length);
                    // close zip entry
                    zip.closeEntry();
                }
                // close zip
                zip.close();

                byte[] sObjData = bOut.toByteArray();
                byte[] zipData = ConvertToByte(sObjData);

                log.Fine("Length=" + zipData.Length);

                // Set binary data in PO and return
                SetBinaryData(zipData);
                return true;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "saveLOBData", ex);
            }
            // Set binary data in PO and return
            SetBinaryData(null);
            return false;
        }

        /// <summary>
        /// Load Data into local _data
        /// </summary>
        /// <returns>bool type true if success</returns>
        private bool LoadLOBData()
        {
            //	Reset
            _items = new List<MAttachmentEntry>();
            // Get binaryData from DB
            byte[] data = GetBinaryData();

            // if no data, then return
            if (data == null)
                return true;

            log.Fine("ZipSize=" + data.Length);
            if (data.Length == 0)
                return true;

            //	Old Format - single file
            //here we store title in zip format 
            //if (!ZIP.Equals(GetTitle()))
            //{
            //    _items.Add(new MAttachmentEntry(GetTitle(), data, 1));
            //    return true;
            //}
            // convert byte[] to byte[] data
            byte[] sdata = ConvertTobyte(data);
            try
            {
                ByteArrayInputStream inBt = new ByteArrayInputStream(sdata);
                // initialize zip
                ZipInputStream zip = new ZipInputStream(inBt);
                // get next entry i.e. 1st entry in zip
                ZipEntry entry = zip.getNextEntry();
                // for every entry in zip
                while (entry != null)
                {
                    // get file name
                    string name = entry.getName();
                    ByteArrayOutputStream outBt = new ByteArrayOutputStream();
                    byte[] buffer = new byte[2048];
                    int length = zip.read(buffer);
                    while (length != -1)
                    {
                        // get data
                        outBt.write(buffer, 0, length);
                        length = zip.read(buffer);
                    }
                    //
                    byte[] sdataEntry = outBt.toByteArray();
                    byte[] dataEntry = ConvertToByte(sdataEntry);

                    try
                    {
                        log.Fine(name
                        + " - size=" + dataEntry.Length + " - zip="
                        + entry.getCompressedSize() + "(" + entry.getSize() + ") "
                        + (entry.getCompressedSize() * 100 / entry.getSize()) + "%");
                    }
                    catch { }


                    // add the entry into _items list
                    _items.Add(new MAttachmentEntry(name, dataEntry, _items.Count + 1));
                    // get next entry in zip
                    entry = zip.getNextEntry();
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "loadLOBData", ex);
                _items = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Convert byte[] to byte[]
        /// </summary>
        /// <param name="byteVar">byte[]</param>
        /// <returns>byte[]</returns>
        private byte[] ConvertToByte(byte[] byteVar)
        {
            //int iLen = byteVar.Length;
            //byte[] byteData = new byte[iLen];
            //for (int i = 0; i < iLen; i++)
            //{
            //    byteData[i] = (byte)byteVar[i];
            //}

            if (byteVar != null)
            {
                byte[] byteData = new byte[byteVar.Length];
                System.Buffer.BlockCopy(byteVar, 0, byteData, 0, byteVar.Length);

                return byteData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert byte[] to byte[]
        /// </summary>
        /// <param name="byteVar">byte[]</param>
        /// <returns>byte[]</returns>
        private byte[] ConvertTobyte(byte[] byteVar)
        {
            if (byteVar != null)
            {
                //int len = byteVar.Length;
                //byte[] byteData = new byte[len];
                //for (int i = 0; i < len; i++)
                //{
                //    byteData[i] = (byte)byteVar[i];
                //}
                byte[] byteData = new byte[byteVar.Length];
                System.Buffer.BlockCopy(byteVar, 0, byteData, 0, byteVar.Length);

                return byteData;
            }
            else
            {
                return null;
            }
        }

        public void NewRecord()
        {
            base.CreateNewRecord = true;
        }


        //public List<AttachmentLineInfo> _lines
        //{
        //    get;
        //    set;
        //}


        //public void GetLines()
        //{
        //    string sql = @"SELECT * FROM AD_AttachmentLine WHERE AD_Attachment_ID=" + GetMailAttachment1_ID();
        //    DataSet ds = DB.ExecuteDataset(sql);
        //    if (ds == null || ds.Tables[0].Rows.Count == 0)
        //    {
        //        return;
        //    }
        //    List<AttachmentLineInfo> lines = new List<AttachmentLineInfo>();
        //    AttachmentLineInfo item = null;
        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        item = new AttachmentLineInfo();
        //        item.Line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_AttachmentLine_ID"]);
        //        item.FileName = Util.GetValueOfString(ds.Tables[0].Rows[i]["FileName"]);
        //        item.Filetype = Util.GetValueOfString(ds.Tables[0].Rows[i]["FileType"]);
        //        item.Size = Util.GetValueOfDecimal(Util.GetValueOfString(ds.Tables[0].Rows[i]["FileSize"]));
        //        lines.Add(item);
        //    }
        //    _lines = lines;
        //}



    }

 

}
