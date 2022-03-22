/********************************************************
 * Module Name    : Attachment
 * Purpose        : Contains functions used to upload/download/view/delete attachments 
 *                  of a record from "Attachment" window
 * Class Used     : MAttachmentEntry.cs, X_AD_Attachment.cs
 * Chronological Development
 * Veena Pandey     10-March-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using java.util.zip;
using java.io;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.IO;
using javax.crypto;
using System.Net;
using System.Data.SqlClient;
using System.Web;
using VAdvantage.AzureBlob;

namespace VAdvantage.Model
{
    /// <summary>
    /// A class which contains functions used to upload/download/view/delete attachments
    /// of a record from "Attachment" window
    /// </summary>
    public class MAttachment : X_AD_Attachment
    {

        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MAttachment).FullName);
        private bool isForce = true;
        private StringBuilder error = null;
        private string _password = null;

        public List<AttachmentLineInfo> _lines
        {
            get;
            set;
        }
        public string FolderKey
        {
            get;
            set;
        }

        public bool Force
        {
            get
            {
                return isForce;
            }
            set
            {
                isForce = value;
            }
        }

        public string Error
        {
            get
            {
                if (error != null)
                    return error.ToString();
                return string.Empty;
            }

        }
        public int AD_Client_ID
        {
            get;
            set;
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (!(string.IsNullOrEmpty(value)))
                {
                    _password = value;
                }
            }
        }
        public int AD_Attachment_ID
        {
            get;
            set;
        }

        List<MAttachmentEntry> _items = null;


        // Indicator for no data
        //private const string NONE = ".";
        // Indicator for zip data
        private const string ZIP = "zip";

        private string fileName = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID</param>
        /// <param name="Record_ID">Record_ID</param>
        /// <param name="trxName">transaction</param>
        public MAttachment(Ctx ctx, int AD_Table_ID, int Record_ID, Trx trxName)
            : base(ctx, 0, trxName)
        {
            //this(ctx, 0, trxName);
            // set table id
            SetAD_Table_ID(AD_Table_ID);
            // set record id
            SetRecord_ID(Record_ID);
            error = new StringBuilder();
            TryGetLines();

        }

        private void TryGetLines()
        {
            if (IsFromHTML())
            {
                GetLines();
            }
            else
            {
                if (GetAD_Attachment_ID() > 0)
                {
                    FolderKey = DateTime.Now.Ticks.ToString();
                    LoadLOBData();
                    SetIsFromHTML(true);
                    Save();
                    //if (attachmentFiles != null && attachmentFiles.Count > 0)
                    //{
                    //    for (int i = 0; i < attachmentFiles.Count; i++)
                    //    {
                    //        CreateAttachmentLine(attachmentFiles[i].Name, attachmentFiles[i].Size, FolderKey);
                    //    }
                    //}
                    GetLines();
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Attachment_ID">AD_Attachment_ID</param>
        /// <param name="trxName">transaction</param>
        public MAttachment(Ctx ctx, int AD_Attachment_ID, Trx trxName)
            : base(ctx, AD_Attachment_ID, trxName)
        {
            error = new StringBuilder();
            TryGetLines();
            GetSetFileLocation();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Attachment_ID">AD_Attachment_ID</param>
        /// <param name="trxName">transaction</param>
        public MAttachment(Ctx ctx, int AD_Attachment_ID, Trx trxName, string password, int AD_client_ID)
            : base(ctx, AD_Attachment_ID, trxName)
        {
            error = new StringBuilder();
            Password = password;
            AD_Client_ID = AD_client_ID;
            TryGetLines();
            GetSetFileLocation();
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MAttachment(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            error = new StringBuilder();
            TryGetLines();
            GetSetFileLocation();
        }



        private void GetSetFileLocation()
        {

            X_AD_ClientInfo cInfo = null;
            if (AD_Client_ID > 0)
            {
                cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
            }
            else
            {
                cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
            }

            if (string.IsNullOrEmpty(cInfo.GetSaveAttachmentOn()))
            {
                SetFileLocation(X_AD_ClientInfo.SAVEATTACHMENTON_ServerFileSystem);
            }
            else
            {
                SetFileLocation(cInfo.GetSaveAttachmentOn());
            }
        }

        /// <summary>
        /// Get Attachment
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <returns>attachment or null</returns>
        public static MAttachment Get(Ctx ctx, int AD_Table_ID, int Record_ID)
        {
            MAttachment retValue = null;
            String sql = "SELECT * FROM AD_Attachment WHERE AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID + "";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MAttachment(ctx, dr, null);
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
        /// <param name="item">MAttachment object item</param>
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

        public void InitEntries()
        {
            if (_items == null)
            {
                _items = new List<MAttachmentEntry>();
            }
            _items.Clear();
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
                SetTitle(ZIP);
            //	save in BinaryData
            return SaveLOBData();
        }

        /// <summary>
        /// Save Entry Data in Zip File format
        /// </summary>
        /// <returns>bool type true if saved</returns>
        private bool SaveLOBData()
        {
            //File name
            fileName = GetAD_Table_ID().ToString() + "_" + GetRecord_ID().ToString();
            X_AD_ClientInfo cInfo = null;


            if (_items == null || _items.Count == 0)
            {
                // if no items
                // Set binary data in PO and return
                SetBinaryData(null);
                DeleteFileData(fileName);
                //if (AD_Client_ID > 0)
                //{
                //    cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                //}
                //else
                //{
                //    cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                //}
                //SetFileLocation(cInfo.GetSaveAttachmentOn());
                return true;
            }

            ByteArrayOutputStream bOut = new ByteArrayOutputStream();
            // initialize zip
            ZipOutputStream zip = new ZipOutputStream(bOut);
            zip.setMethod(ZipOutputStream.DEFLATED);
            zip.setLevel(Deflater.BEST_COMPRESSION);
            //zip.sete
            //
            try
            {
                byte[] zipData = null;


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
                bOut.close();

                zipData = ConvertToByte(sObjData);


                log.Fine("Length=" + zipData.Length);

                if (string.IsNullOrEmpty(GetFileLocation()))
                {

                    if (AD_Client_ID > 0)
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                    }
                    else
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                    }
                    string fileLocation = cInfo.GetSaveAttachmentOn();
                    if (string.IsNullOrEmpty(fileLocation))
                    {
                        error.Append(Msg.GetMsg(GetCtx(), "NotAValidLocationToSaveFile"));
                        return false;
                    }
                    SetFileLocation(fileLocation);
                }

                if (GetFileLocation() == FILELOCATION_FTPLocation)
                {
                    //String path = GlobalVariable.AttachmentPath;

                    //Create Directory
                    try
                    {
                        //if (!Directory.Exists(path))
                        //{
                        //    Directory.CreateDirectory(path);
                        //}
                        //Encrypt
                        //String data = ByteArrayToString(zipData);
                        //data = Utility.SecureEngine.Encrypt(data);
                        //zipData = StringToByteArray(data);


                        //Is user need to encription
                        SetCryptAndZipWay(CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip);

                        if (GetCryptAndZipWay() == CRYPTANDZIPWAY_KeyEncryptionAndServerSideZip)
                        {
                            char[] data = ByteArrayToCharArray(zipData);
                            zipData = SecureEngine.Encrypt(data);
                        }
                        else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip)
                        {
                            zipData = SecureEngine.EncryptFile(zipData, Password);
                        }
                        else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip)
                        {
                            throw new Exception("CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip Not Implemented yet.");
                        }
                        if (!UploadFtpFile(fileName, zipData, cInfo))
                        {
                            if (!Force)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            SetFileLocation(FILELOCATION_FTPLocation);
                            return true;
                        }

                    }
                    catch (Exception e)
                    {
                        log.Severe("FTP Location->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            return false;
                        }
                    }
                    //SetIsFileSystem(false);
                    SetFileLocation(FILELOCATION_Database);
                    DeleteFileData(fileName);
                    SetBinaryData(zipData);
                    return true;

                }
                else if (GetFileLocation() == FILELOCATION_ServerFileSystem)
                {
                    String path = GetAttachmentPath();

                    //Create Directory
                    try
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Encrypt
                        //String data = ByteArrayToString(zipData);
                        //data = Utility.SecureEngine.Encrypt(data);
                        //zipData = StringToByteArray(data);

                        if (GetCryptAndZipWay() == CRYPTANDZIPWAY_KeyEncryptionAndServerSideZip)
                        {
                            char[] data = ByteArrayToCharArray(zipData);
                            zipData = SecureEngine.Encrypt(data);
                        }
                        else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip)
                        {
                            zipData = SecureEngine.EncryptFile(zipData, Password);
                        }
                        else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip)
                        {
                            throw new Exception("CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip Not Implemented yet.");
                        }

                        //char[] data = ByteArrayToCharArray(zipData);
                        //zipData = Utility.SecureEngine.Encrypt(data);
                        System.IO.File.WriteAllBytes(path + "\\" + fileName, zipData);
                        SetBinaryData(null);
                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Severe("FleSystem->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            return false;
                        }
                    }
                    //SetIsFileSystem(false);
                    SetFileLocation(FILELOCATION_Database);
                    DeleteFileData(fileName);
                    SetBinaryData(zipData);
                    return true;

                } // No need to implement for now, but do not delete
                if (GetFileLocation() == FILELOCATION_WebService)
                {
                    string filePath = GetAttachmentPath();
                    string folderKey = "0";
                    int AD_AttachmentLine_ID = 0;
                    try
                    {
                        // Create client info object
                        if (cInfo == null)
                        {
                            if (AD_Client_ID > 0)
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                            }
                            else
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                            }
                        }

                        // Get file information
                        FileInfo fInfo = new FileInfo(filePath + "\\" + folderKey + "\\" + fileName);
                        byte[] bytes = System.IO.File.ReadAllBytes(filePath + "\\" + folderKey + "\\" + fileName);
                        string file = Convert.ToBase64String(bytes);

                        ExtWebServiceData ewsData = new ExtWebServiceData();
                        ewsData.Token = cInfo.GetAD_WebServiceToken();
                        ewsData.DocumentData = new ExtDocumentData()
                        {
                            DocumentName = fInfo.Name,
                            Size = fInfo.Length,
                            DocumentBytes = file
                        };

                        // Call external web service methods and file info to it
                        ExternalWebMethod ewm = new ExternalWebMethod();
                        string resURI = ewm.CallWebService(cInfo.GetAD_WebServiceURL() + "/StoreDocument", ewsData);

                        MAttachmentReference attRef = new MAttachmentReference(GetCtx(), 0, Get_Trx());

                        attRef.SetAD_AttachmentLine_ID(AD_AttachmentLine_ID);
                        attRef.SetAD_AttachmentRef(resURI);
                        if (!attRef.Save(Get_Trx()))
                        {
                            log.Severe("MAttachmentReference not saved " + VLogger.RetrieveError().Name);
                            return false;
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Severe("Web Service Location->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        CleanUp(filePath + "\\" + folderKey, filePath + "\\" + folderKey + "\\" + fileName, filePath + "\\" + fileName, null);
                    }
                    //SetIsFileSystem(false);
                    SetFileLocation(FILELOCATION_Database);
                    DeleteFileData(fileName);
                    SetBinaryData(zipData);
                    return true;

                }
                else
                {
                    //SetIsFileSystem(false);
                    SetFileLocation(FILELOCATION_Database);
                    DeleteFileData(fileName);
                    SetBinaryData(zipData);
                    return true;
                }


                //if (this.IsFileSystem())
                //{
                //    String path = GlobalVariable.AttachmentPath;

                //    //Create Directory
                //    try
                //    {
                //        if (!Directory.Exists(path))
                //        {
                //            Directory.CreateDirectory(path);
                //        }
                //        //Encrypt
                //        //String data = ByteArrayToString(zipData);
                //        //data = Utility.SecureEngine.Encrypt(data);
                //        //zipData = StringToByteArray(data);

                //        char[] data = ByteArrayToCharArray(zipData);
                //        zipData = Utility.SecureEngine.Encrypt(data);
                //        System.IO.File.WriteAllBytes(path + "\\" + fileName, zipData);
                //        SetBinaryData(null);
                //        return true;
                //    }
                //    catch (Exception e)
                //    {
                //        log.Severe("FleSystem->" + e.Message);
                //        if (!Force)
                //        {
                //            return false;
                //        }
                //    }

                //}

                //SetIsFileSystem(false);
                //DeleteFileData(fileName);
                //SetBinaryData(zipData);
                //return true;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "saveLOBData", ex);
                error.Append(ex.Message);
            }
            // Set binary data in PO and return
            // Set binary data in PO and return
            return false;
        }

        private bool UploadFtpFile(string fileName, byte[] file, X_AD_ClientInfo cInfo)
        {

            FtpWebRequest request;
            try
            {
                if (cInfo == null)
                {
                    // X_AD_ClientInfo cInfo = null;
                    if (AD_Client_ID > 0)
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                    }
                    else
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                    }
                }
                request = WebRequest.Create(new Uri(string.Format(@"{0}/{1}/{2}", cInfo.GetFTPUrl(), cInfo.GetFTPFolder(), fileName))) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Credentials = new NetworkCredential(cInfo.GetFTPUsername(), cInfo.GetFTPPwd());
                request.ConnectionGroupName = "group";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(file, 0, file.Length);
                requestStream.Close();
                requestStream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                error.Append("ErrorWhileUploadingFileOnFTP:" + ex.Message);
                return false;
            }
        }

        private byte[] DownloadFtpFile(string fileName)
        {

            WebClient request = new WebClient();
            byte[] retVal = null;

            try
            {
                X_AD_ClientInfo cInfo = null;
                if (AD_Client_ID > 0)
                {
                    cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                }
                else
                {
                    cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                }
                request.Credentials = new NetworkCredential(cInfo.GetFTPUsername(), cInfo.GetFTPPwd());
                retVal = request.DownloadData(new Uri(string.Format(@"{0}/{1}/{2}", cInfo.GetFTPUrl(), cInfo.GetFTPFolder(), fileName)));
                request.Dispose();
                return retVal;
                //string fileString = System.Text.Encoding.UTF8.GetString(newFileData);

            }
            catch (WebException e)
            {
                error.Append("ErrorWhileDownloadingFileFromFTP:" + e.Message);
                if (request != null)
                    request.Dispose();
                return null;
            }
        }
        /// <summary>
        /// Convert ByteArray to String
        /// </summary>
        /// <param name="ba">byteArray</param>
        /// <returns>Hexed String</returns>
        private string ByteArrayToString(byte[] ba)
        {
            //return  System.Convert.ToBase64String(ba);
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        /// <summary>
        /// Convert ByteArray to String
        /// </summary>
        /// <param name="ba">byteArray</param>
        /// <returns>Hexed String</returns>
        private char[] ByteArrayToCharArray(byte[] ba)
        {
            //return  System.Convert.ToBase64String(ba);
            char[] retVal = new char[ba.Length * 2];
            StringBuilder hex = new StringBuilder();
            //foreach (byte b in ba)
            //{
            //    hex.Clear();
            //    hex.AppendFormat("{0:x2}", b);

            //}
            int index = 0;
            for (int i = 0; i < ba.Length; i++)
            {
                hex.Clear();
                hex.AppendFormat("{0:x2}", ba[i]);
                retVal[index] = hex.ToString().ToCharArray()[0];
                index++;
                retVal[index] = hex.ToString().ToCharArray()[1];
                index++;
            }
            hex = null;
            return retVal;
        }

        /// <summary>
        /// Convert to HexString to Byte Array
        /// </summary>
        /// <param name="hex">Hexed String</param>
        /// <returns>Byte Array</returns>
        private byte[] StringToByteArray(String hex)
        {
            //return System.Convert.FromBase64String(hex);
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        /// <summary>
        /// Delete File form FileSystem
        /// </summary>
        /// <param name="fileName"></param>
        public static void DeleteFileData(string fileName)
        {
            try
            {

                String path = GetAttachmentPath();
                //path = path.Substring(0,path.IndexOf("bin"));
                if (Directory.Exists(path))
                {
                    if (System.IO.File.Exists(path + "\\" + fileName))
                    {
                        System.IO.File.Delete(path + "\\" + fileName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Severe("FleSystem->" + e.Message);
            }
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
            byte[] sdata = null;

            try
            {
                if (GetFileLocation() == FILELOCATION_FTPLocation)
                {
                    fileName = this.GetAD_Table_ID() + "_" + this.GetRecord_ID();
                    sdata = DownloadFtpFile(fileName);
                    if (GetCryptAndZipWay() == CRYPTANDZIPWAY_KeyEncryptionAndServerSideZip)
                    {
                        sdata = SecureEngine.Decrypt(sdata);
                    }
                    else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip)
                    {
                        sdata = SecureEngine.DecryptFile(sdata, Password);
                    }
                    else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip)
                    {
                        throw new Exception("CRYPTANDZIPWAY_PasswordEncryptionAndNewZip Not Implemented yet.");
                    }
                    // sdata = Utility.SecureEngine.DecryptFile(sdata,Password);
                }
                else if (GetFileLocation() == FILELOCATION_ServerFileSystem)
                {
                    fileName = this.GetAD_Table_ID() + "_" + this.GetRecord_ID();
                    String filePath = Path.Combine(GetAttachmentPath(), fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        sdata = System.IO.File.ReadAllBytes(filePath);
                        if (GetCryptAndZipWay() == CRYPTANDZIPWAY_KeyEncryptionAndServerSideZip)
                        {
                            sdata = SecureEngine.Decrypt(sdata);
                        }
                        else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip)
                        {
                            sdata = SecureEngine.DecryptFile(sdata, Password);
                        }
                        else if (GetCryptAndZipWay() == CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip)
                        {
                            throw new Exception("CRYPTANDZIPWAY_PasswordEncryptionAndNewZip Not Implemented yet.");
                        }
                        //sdata = Utility.SecureEngine.Decrypt(sdata);
                    }
                } // No need to implement for now, but do not delete
                if (GetFileLocation() == FILELOCATION_WebService)
                {
                    DataSet ds = DB.ExecuteDataset("SELECT FileName, FileType FROM AD_AttachmentLine WHERE AD_AttachmentLine_ID=" + 0);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        // Get file from web service and save it in temp folder

                        //(filename, Path.Combine(filePath, "TempDownload", folder));


                        string filePath = GetServerLocation();

                        // Create client info object
                        X_AD_ClientInfo cInfo = null;
                        if (AD_Client_ID > 0)
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                        }
                        else
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                        }

                        string documentURI = GetDocumentURI(AD_Attachment_ID);

                        if (!string.IsNullOrEmpty(documentURI))
                        {
                            // Get file information
                            ExtWebServiceData ewsData = new ExtWebServiceData();
                            ewsData.Token = cInfo.GetAD_WebServiceToken();
                            ewsData.DocumentData = new ExtDocumentData()
                            {
                                DocumentName = Util.GetValueOfString(ds.Tables[0].Rows[0]["FileName"]),
                                URI = documentURI
                            };

                            // Call external web service method to get file based on URI
                            ExternalWebMethod ewm = new ExternalWebMethod();
                            string resFile = ewm.CallWebService(cInfo.GetAD_WebServiceURL() + "/GetDocument", ewsData);

                            byte[] byteData = Convert.FromBase64String(resFile);

                            string savedFile = Path.Combine(filePath, "TempDownload", FolderKey, Util.GetValueOfString(ds.Tables[0].Rows[0]["FileName"]));

                            using (FileStream fs = new FileStream(savedFile, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(byteData, 0, byteData.Length);
                            }
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    byte[] data = GetBinaryData();

                    // if no data, then return
                    if (data == null)
                        return true;

                    log.Fine("ZipSize=" + data.Length);
                    if (data.Length == 0)
                        return true;

                    //	Old Format - single file
                    if (!ZIP.Equals(GetTitle()))
                    {
                        _items.Add(new MAttachmentEntry(GetTitle(), data, 1));
                        return true;
                    }
                    // convert byte[] to byte[] data
                    sdata = ConvertTobyte(data);
                }


                ByteArrayInputStream inBt = new ByteArrayInputStream(sdata);
                // initialize zip
                ZipInputStream zip = new ZipInputStream(inBt);
                // get next entry i.e. 1st entry in zip
                ZipEntry entry = zip.getNextEntry();
                // for every entry in zip
                while (entry != null)
                {
                    AttFileInfo itm = new AttFileInfo();

                    // get file name
                    string name = entry.getName();
                    itm.Name = name;
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

                    log.Fine(name
                    + " - size=" + dataEntry.Length + " - zip="
                    + entry.getCompressedSize() + "(" + entry.getSize() + ") "
                    + (entry.getCompressedSize() * 100 / entry.getSize()) + "%");


                    // add the entry into _items list
                    _items.Add(new MAttachmentEntry(name, dataEntry, _items.Count + 1));
                    itm.Size = dataEntry.Length;
                    if (attachmentFiles == null)
                    {
                        attachmentFiles = new List<AttFileInfo>();
                    }
                    attachmentFiles.Add(itm);
                    // get next entry in zip

                    //Write file In Temp Download

                    Directory.CreateDirectory(System.IO.Path.Combine(GetServerLocation(), "TempDownload", FolderKey));

                    System.IO.File.WriteAllBytes(System.IO.Path.Combine(GetServerLocation(), "TempDownload", FolderKey, name), dataEntry);


                    entry = zip.getNextEntry();
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, "loadLOBData", ex);
                error.Append(ex.Message);
                _items = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the server location
        /// </summary>
        /// <returns>Returns server location</returns>
        private static string GetServerLocation()
        {
            string serverLocation = System.Web.Configuration.WebConfigurationManager.AppSettings["serverLocation"];

            if (serverLocation != null && serverLocation != "")
            {
                return serverLocation;
            }
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
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


        public void GetLines()
        {
            string sql = @"SELECT * FROM AD_AttachmentLine WHERE AD_Attachment_ID=" + GetAD_Attachment_ID();
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return;
            }
            AD_Attachment_ID = GetAD_Attachment_ID();
            List<AttachmentLineInfo> lines = new List<AttachmentLineInfo>();
            AttachmentLineInfo item = null;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                item = new AttachmentLineInfo();
                item.Line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_AttachmentLine_ID"]);
                item.FileName = Util.GetValueOfString(ds.Tables[0].Rows[i]["FileName"]);
                item.Filetype = Util.GetValueOfString(ds.Tables[0].Rows[i]["FileType"]);
                item.Size = Util.GetValueOfDecimal(Util.GetValueOfString(ds.Tables[0].Rows[i]["FileSize"]));
                lines.Add(item);
            }

            _lines = lines;
        }


        public string CreateAttachmentLine(string fileName, long size, string folderKey)
        {
            string ext = fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - (fileName.LastIndexOf('.')));
            //save line 
            int AD_AttachmentLine_ID = DB.GetNextID(GetCtx().GetAD_Client_ID(), "AD_AttachmentLine", null);
            string sql = @"INSERT INTO AD_AttachmentLine (AD_ATTACHMENTLINE_ID ,
                                    AD_ATTACHMENT_ID   ,    
                                    AD_CLIENT_ID   ,
                                    AD_ORG_ID       ,
                                    CREATED        ,     
                                    CREATEDBY     ,
                                    EXPORT_ID    ,
                                    FILENAME      ,
                                    FILESIZE       ,
                                    FILETYPE      ,
                                    ISACTIVE       ,
                                    UPDATED       ,
                                    UPDATEDBY) VALUES (" + AD_AttachmentLine_ID + "," + GetAD_Attachment_ID() + "," + GetCtx().GetAD_Client_ID() + "," + GetCtx().GetAD_Org_ID() + @",
                                                        " + GlobalVariable.TO_DATE(DateTime.UtcNow, false) + "," + GetCtx().GetAD_User_ID() + ",NULL,'" + fileName + "'," + size + ",'" + ext + @"','Y',
                                                        " + GlobalVariable.TO_DATE(DateTime.UtcNow, false) + "," + GetCtx().GetAD_User_ID() + ")";
            int res = DB.ExecuteQuery(sql);
            if (res == -1)
            {
                return "False";
            }
            if (!SaveFromFS(GetAD_Table_ID() + "_" + GetRecord_ID() + "_" + AD_AttachmentLine_ID, folderKey, fileName, AD_AttachmentLine_ID + ext, AD_AttachmentLine_ID))
            {
                DB.ExecuteQuery("DELETE AD_AttachmentLine WHERE AD_AttachmentLine_ID=" + AD_AttachmentLine_ID);
                return "False";
            }
            return "";
        }
        private bool SaveFromFS(string outputfileName, string folderKey, string fileName, string newFileName, int AD_AttachmentLine_ID)
        {
            try
            {

                string filePath = System.IO.Path.Combine(GetServerLocation(), "TempDownload");
                string zipinput = filePath + "\\" + folderKey + "\\zipInput";
                string zipfileName = System.IO.Path.Combine(filePath, folderKey, DateTime.Now.Ticks.ToString());

                if (!Directory.Exists(filePath))
                {
                    error.Append("PathNotFound:" + filePath);
                    return false;
                }

                X_AD_ClientInfo cInfo = null;
                if (string.IsNullOrEmpty(GetFileLocation()))
                {

                    if (AD_Client_ID > 0)
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                    }
                    else
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                    }
                    string fileLocation = cInfo.GetSaveAttachmentOn();
                    if (string.IsNullOrEmpty(fileLocation))
                    {
                        error.Append(Msg.GetMsg(GetCtx(), "NotAValidLocationToSaveFile"));
                        CleanUp(filePath + "\\" + folderKey, zipfileName, null, zipinput);
                        return false;
                    }
                    SetFileLocation(fileLocation);
                }
                // If file saving location is web service
                if (GetFileLocation() == FILELOCATION_WebService)
                {
                    try
                    {
                        SetFileLocation(FILELOCATION_WebService);

                        // Create client info object
                        if (cInfo == null)
                        {
                            if (AD_Client_ID > 0)
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                            }
                            else
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                            }
                        }

                        // Get file information
                        FileInfo fInfo = new FileInfo(filePath + "\\" + folderKey + "\\" + fileName);
                        byte[] bytes = System.IO.File.ReadAllBytes(filePath + "\\" + folderKey + "\\" + fileName);
                        string file = Convert.ToBase64String(bytes);

                        ExtWebServiceData ewsData = new ExtWebServiceData();
                        ewsData.Token = cInfo.GetAD_WebServiceToken();
                        ewsData.DocumentData = new ExtDocumentData()
                        {
                            DocumentName = fInfo.Name,
                            Size = fInfo.Length,
                            DocumentBytes = file
                        };

                        // Call external web service methods and file info to it
                        ExternalWebMethod ewm = new ExternalWebMethod();
                        string resURI = ewm.CallWebService(cInfo.GetAD_WebServiceURL() + "/StoreDocument", ewsData);

                        MAttachmentReference attRef = new MAttachmentReference(GetCtx(), 0, Get_Trx());

                        attRef.SetAD_AttachmentLine_ID(AD_AttachmentLine_ID);
                        attRef.SetAD_AttachmentRef(resURI);
                        if (!attRef.Save(Get_Trx()))
                        {
                            log.Severe("MAttachmentReference not saved " + VLogger.RetrieveError().Name);
                            return false;
                        }

                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Severe("Web Service Location->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        CleanUp(filePath + "\\" + folderKey, filePath + "\\" + folderKey + "\\" + fileName, filePath + "\\" + fileName, null);
                    }
                    return true;
                }

                // If file saving location is Identity provider
                if (GetFileLocation() == FILELOCATION_IDP)
                {
                    try
                    {
                        SetFileLocation(FILELOCATION_IDP);

                        // Create client info object
                        if (cInfo == null)
                        {
                            if (AD_Client_ID > 0)
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                            }
                            else
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                            }
                        }

                        // Get file information
                        FileInfo fInfo = new FileInfo(filePath + "\\" + folderKey + "\\" + fileName);
                        byte[] bytes = System.IO.File.ReadAllBytes(filePath + "\\" + folderKey + "\\" + fileName);
                        string file = Convert.ToBase64String(bytes);

                        var DocumentData = new ExtApiDocumentData()
                        {
                            filename = fInfo.Name,
                            fileBytes = file,
                            fileExtension = fInfo.Extension,
                            documentUri = ""
                        };

                        // Call external idp server to get token and upload using external api

                        // Sync call
                        string resURI = new ExternalWebMethod().SaveFileApi(
                            GetCtx(),
                            cInfo.GetAD_IDPServerURL(),
                            cInfo.GetAD_IDPServerClient(),// password, accesskey, dmstoken
                            cInfo.GetAD_WebServiceURL(),
                            cInfo.GetAD_WebServiceToken(),
                            DocumentData
                            );

                        // Add resURI to attachment table
                        MAttachmentReference attRef = new MAttachmentReference(GetCtx(), 0, Get_Trx());

                        attRef.SetAD_AttachmentLine_ID(AD_AttachmentLine_ID);
                        attRef.SetAD_AttachmentRef(resURI);
                        if (!attRef.Save(Get_Trx()))
                        {
                            log.Severe("MAttachmentReference not saved " + VLogger.RetrieveError().Name);
                            return false;
                        }

                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Severe("IDP Location->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        CleanUp(filePath + "\\" + folderKey, filePath + "\\" + folderKey + "\\" + fileName, filePath + "\\" + fileName, null);
                    }
                    return true;
                }

                Directory.CreateDirectory(zipinput);

                System.IO.File.Copy(filePath + "\\" + folderKey + "\\" + fileName, zipinput + "\\" + newFileName);

                System.IO.File.Delete(filePath + "\\" + folderKey + "\\" + fileName);

                ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
                z.CreateZip(zipfileName, zipinput, true, null);
                System.IO.File.Delete(zipinput + "\\" + fileName);
                //log.Fine("Length=" + zipData.Length);

                //Is user need to encription
                SetCryptAndZipWay(CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip);


                //zipData = Utility.SecureEngine.EncryptFile(zipData, Password);
                if (!SecureEngine.EncryptFile(zipfileName, Password, Path.Combine(filePath, outputfileName)))
                {
                    error.Append("ErrorInEncryption:" + zipfileName);
                    CleanUp(filePath + "\\" + folderKey, zipfileName, null, zipinput);
                    return false;
                }

                System.IO.File.Delete(zipfileName);

                
                if (GetFileLocation() == FILELOCATION_FTPLocation)
                {
                    try
                    {
                        if (!UploadFtpFileWithoutRAM(filePath + "\\" + outputfileName, cInfo, outputfileName))
                        {
                            if (!Force)
                            {
                                CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                                return false;
                            }
                        }
                        else
                        {
                            SetFileLocation(FILELOCATION_FTPLocation);
                            CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                            return true;
                        }
                        CleanUp(filePath + "\\" + folderKey, zipfileName, null, zipinput);
                        return false;

                    }
                    catch (Exception e)
                    {
                        log.Severe("FTP Location->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                            return false;
                        }
                    }
                    //SetIsFileSystem(false);
                    //SetFileLocation(FILELOCATION_Database);
                    //DeleteFileData(fileName);
                    //SetBinaryData(zipData);
                    CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                    return true;

                }

                else if (GetFileLocation() == FILELOCATION_ServerFileSystem)
                {
                    string path = GetAttachmentPath();

                    //Create Directory
                    try
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Encrypt                       
                        //System.IO.File.WriteAllBytes(path + "\\" + fileName, zipData);
                        System.IO.File.Copy(filePath + "\\" + outputfileName, path + "\\" + outputfileName, true);
                        SetBinaryData(null);
                        CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Severe("FleSystem->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                            return false;
                        }
                    }
                    //SetIsFileSystem(false);
                    //SetFileLocation(FILELOCATION_Database);
                    //DeleteFileData(fileName);
                    //SetBinaryData(zipData);
                    return true;

                }
                // VIS264 - If file upload location is Azure Blob Storage
                else if (GetFileLocation() == FILELOCATION_AzureBlobStorage)
                {
                    try
                    {
                        SetFileLocation(FILELOCATION_AzureBlobStorage);

                        if (cInfo == null)
                        {
                            if (AD_Client_ID > 0)
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                            }
                            else
                            {
                                cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                            }
                        }

                        if (string.IsNullOrEmpty(cInfo.GetAD_WebServiceURL()))
                        {
                            error.Append(Msg.GetMsg(GetCtx(), "VIS_AzureContainerUriEmpty"));
                            CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                            return false;
                        }

                        string res = AzureBlobStorage.UploadFile(GetCtx(), cInfo.GetAD_WebServiceURL(), filePath + "\\" + outputfileName, outputfileName);

                        if (res != null)
                        {
                            error.Append(res);
                            CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                            return false;
                        }

                        CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                        return true;
                    }
                    catch (Exception e)
                    {
                        log.Severe("AzureBlobStorage Location->" + e.Message);
                        error.Append(e.Message);
                        if (!Force)
                        {
                            CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                            return false;
                        }
                    }
                    finally
                    {
                        CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                    }
                    return true;
                }
                else
                {
                    //SetIsFileSystem(false);
                    SetFileLocation(FILELOCATION_Database);
                    DeleteFileData(fileName);
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath + "\\" + outputfileName);
                    if (fileData != null)
                    {
                        string sql = " UPDATE AD_AttachmentLine SET BinaryData=@data WHERE  AD_AttachmentLine_ID=" + AD_AttachmentLine_ID;
                        SqlParameter[] param = new SqlParameter[1];
                        param[0] = new SqlParameter("@data", fileData);
                        int res = DB.ExecuteQuery(sql, param);
                    }

                    //SetBinaryData(System.IO.File.ReadAllBytes(filePath + "\\" + outputfileName));
                    CleanUp(filePath + "\\" + folderKey + "\\" + fileName, zipfileName, filePath + "\\" + outputfileName, zipinput);
                    return true;
                }

                //Directory.Delete(filePath +"\\"+FolderKey);
                //System.IO.File.Delete(zipfileName);
                //System.IO.File.Delete(filePath + "\\" + outputfileName);
                //return true;
            }
            catch (Exception ex)
            {
                error.Append(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Get attachment path
        /// </summary>
        /// <returns>Returns attachment path</returns>
        private static string GetAttachmentPath()
        {
            return GetServerLocation() == System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?
                        GlobalVariable.AttachmentPath : Path.Combine(GetServerLocation(), "Attachments");
        }

        private void CleanUp(string dirpath, string zipfile, string encryptedfile, string zipInput)
        {
            try
            {
                //if (!string.IsNullOrEmpty(dirpath))
                //{
                //    Directory.Delete(dirpath, true);
                //}

                if (!string.IsNullOrEmpty(zipInput))
                {
                    Directory.Delete(zipInput, true);
                }
                if (!string.IsNullOrEmpty(dirpath))
                {
                    System.IO.File.Delete(zipfile);
                }
                if (!string.IsNullOrEmpty(dirpath))
                {
                    System.IO.File.Delete(encryptedfile);
                }
                //VAdvantage.Classes.CleanUp.Get().Start();
            }
            catch
            {
            }
        }


        private bool UploadFtpFileWithoutRAM(string fullname, X_AD_ClientInfo cInfo, string fNameFTP)
        {

            FtpWebRequest request;
            try
            {
                if (cInfo == null)
                {
                    // X_AD_ClientInfo cInfo = null;
                    if (AD_Client_ID > 0)
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                    }
                    else
                    {
                        cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                    }
                }

                FileInfo fileInfo = new FileInfo(fullname);

                request = WebRequest.Create(new Uri(string.Format(@"{0}/{1}/{2}", cInfo.GetFTPUrl(), cInfo.GetFTPFolder(), fNameFTP))) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Credentials = new NetworkCredential(cInfo.GetFTPUsername(), cInfo.GetFTPPwd());
                request.ConnectionGroupName = "group";

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                FileStream fileStream = fileInfo.OpenRead();
                Stream stream = request.GetRequestStream();
                contentLen = fileStream.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    stream.Write(buff, 0, contentLen);
                    contentLen = fileStream.Read(buff, 0, buffLength);
                }
                stream.Close();
                fileStream.Close();

                //Stream requestStream = request.GetRequestStream();
                //requestStream.Write(file, 0, file.Length);
                //requestStream.Close();
                //requestStream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                error.Append("ErrorWhileUploadingFileOnFTP:" + ex.Message);
                return false;
            }
        }

        List<AttFileInfo> attachmentFiles = null;
        public void SetAttFileInfo(List<AttFileInfo> _files)
        {
            attachmentFiles = _files;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            //return base.AfterSave(newRecord, success);
            if (attachmentFiles != null && attachmentFiles.Count > 0)
            {
                string res = null;
                for (int i = 0; i < attachmentFiles.Count; i++)
                {
                    res = CreateAttachmentLine(attachmentFiles[i].Name, attachmentFiles[i].Size, FolderKey);
                    if (res.Equals("False"))
                    {

                        Directory.Delete(System.IO.Path.Combine(GetServerLocation(), "TempDownload", FolderKey));
                        return false;
                    }
                }
                Directory.Delete(System.IO.Path.Combine(GetServerLocation(), "TempDownload", FolderKey));


            }
            return true;
        }

        public string GetFile(int AD_AttachmentLine_ID)
        {
            try
            {
                DataSet ds = DB.ExecuteDataset("SELECT FileName, FileType FROM AD_AttachmentLine WHERE AD_AttachmentLine_ID=" + AD_AttachmentLine_ID);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string fileLocation = GetFileLocation();
                    string folder = DateTime.Now.Ticks.ToString();

                    string filePath = System.IO.Path.Combine(GetServerLocation());
                    Directory.CreateDirectory(Path.Combine(filePath, "TempDownload", folder));
                    string filename = GetAD_Table_ID() + "_" + GetRecord_ID() + "_" + AD_AttachmentLine_ID;
                    string zipFileName = "zip" + DateTime.Now.Ticks.ToString();
                    if (fileLocation == X_AD_Attachment.FILELOCATION_Database)
                    {
                        byte[] data = null;
                        object d = (DB.ExecuteScalar("SELECT BinaryData FROM AD_AttachmentLine WHERE AD_AttachmentLine_ID=" + AD_AttachmentLine_ID)); //GetBinaryData();
                        if (d != null && d != DBNull.Value)
                        {
                            data = (byte[])d;
                            System.IO.File.WriteAllBytes(Path.Combine(filePath, "TempDownload", folder, filename), data);
                            SecureEngine.DecryptFile(Path.Combine(filePath, "TempDownload", folder, filename), Password, Path.Combine(filePath, "TempDownload", folder, zipFileName));
                            //Delete fle from temp
                            System.IO.File.Delete(Path.Combine(filePath, "TempDownload", folder, filename));
                        }
                    }
                    else if (fileLocation == X_AD_Attachment.FILELOCATION_FTPLocation)
                    {

                        DownloadFtpFileWithoutRAM(filename, Path.Combine(filePath, "TempDownload", folder));
                        SecureEngine.DecryptFile(Path.Combine(filePath, "TempDownload", folder, filename), Password, Path.Combine(filePath, "TempDownload", folder, zipFileName));
                        //Delete fle from temp
                        System.IO.File.Delete(Path.Combine(filePath, "TempDownload", folder, filename));

                    }
                    else if (fileLocation == X_AD_Attachment.FILELOCATION_ServerFileSystem)
                    {
                        //Copy to temp
                        System.IO.File.Copy(Path.Combine(filePath, "Attachments", filename), Path.Combine(filePath, "TempDownload", folder, filename));
                        //Decrypt File

                        SecureEngine.DecryptFile(Path.Combine(filePath, "TempDownload", folder, filename), Password, Path.Combine(filePath, "TempDownload", folder, zipFileName));
                        //Delete fle from temp
                        System.IO.File.Delete(Path.Combine(filePath, "TempDownload", folder, filename));

                    }
                    else if (fileLocation == X_AD_Attachment.FILELOCATION_WebService)
                    {
                        // Get file from web service and save it in temp folder

                        //(filename, Path.Combine(filePath, "TempDownload", folder));

                        // Create client info object
                        X_AD_ClientInfo cInfo = null;
                        if (AD_Client_ID > 0)
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                        }
                        else
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                        }

                        string documentURI = GetDocumentURI(AD_Attachment_ID);

                        if (!string.IsNullOrEmpty(documentURI))
                        {
                            // Get file information
                            ExtWebServiceData ewsData = new ExtWebServiceData();
                            ewsData.Token = cInfo.GetAD_WebServiceToken();
                            ewsData.DocumentData = new ExtDocumentData()
                            {
                                DocumentName = Util.GetValueOfString(ds.Tables[0].Rows[0]["FileName"]),
                                URI = documentURI
                            };

                            // Call external web service method to get file based on URI
                            ExternalWebMethod ewm = new ExternalWebMethod();
                            string resFile = ewm.CallWebService(cInfo.GetAD_WebServiceURL() + "/GetDocument", ewsData);

                            byte[] byteData = Convert.FromBase64String(resFile);

                            string savedFile = Path.Combine(filePath, "TempDownload", folder, Util.GetValueOfString(ds.Tables[0].Rows[0]["FileName"]));

                            using (FileStream fs = new FileStream(savedFile, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(byteData, 0, byteData.Length);
                            }
                            return folder;
                        }
                        return "";
                    }
                    // If file saving location is Identity provider
                    else if (fileLocation == X_AD_Attachment.FILELOCATION_IDP)
                    {
                        // Get file from web service and save it in temp folder

                        // Create client info object
                        X_AD_ClientInfo cInfo = null;
                        if (AD_Client_ID > 0)
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                        }
                        else
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                        }

                        string documentURI = GetDocumentURI(AD_Attachment_ID);

                        if (!string.IsNullOrEmpty(documentURI))
                        {
                            // Get file information
                            var DocumentData = new ExtApiDocumentData()
                            {
                                filename = Util.GetValueOfString(ds.Tables[0].Rows[0]["FileName"]),
                                fileBytes = null,
                                fileExtension = null,
                                documentUri = documentURI
                            };

                            // Call api to get file based on URI (Sync call)
                            string resFile = new ExternalWebMethod().GetFileApi(
                            GetCtx(),
                            cInfo.GetAD_IDPServerURL(),
                            cInfo.GetAD_IDPServerClient(),// password, accesskey, dmstoken
                            cInfo.GetAD_WebServiceURL(),
                            cInfo.GetAD_WebServiceToken(),
                            DocumentData);

                            byte[] byteData = Convert.FromBase64String(resFile);

                            string savedFile = Path.Combine(filePath, "TempDownload", folder, Util.GetValueOfString(ds.Tables[0].Rows[0]["FileName"]));

                            using (FileStream fs = new FileStream(savedFile, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(byteData, 0, byteData.Length);
                            }
                            return folder;
                        }
                        return "";
                    }
                    // VIS264 - If file location is Azure Blob Storage
                    else if(fileLocation == X_AD_Attachment.FILELOCATION_AzureBlobStorage)
                    {
                        // VIS264 - Get file from Azure Blob container and save it in temp folder

                        X_AD_ClientInfo cInfo = null;
                        if (AD_Client_ID > 0)
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                        }
                        else
                        {
                            cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                        }

                        string containerUri = cInfo.GetAD_WebServiceURL();

                        if (!string.IsNullOrEmpty(containerUri))
                        {
                            string downloadFullPath = Path.Combine(Path.Combine(filePath, "TempDownload", folder), filename);

                            string res = AzureBlobStorage.DownloadFile(GetCtx(), containerUri, downloadFullPath, filename);

                            if (res == null)
                            {
                                //Decrypt File
                                SecureEngine.DecryptFile(Path.Combine(filePath, "TempDownload", folder, filename), Password, Path.Combine(filePath, "TempDownload", folder, zipFileName));
                                //Delete file from temp folder
                                System.IO.File.Delete(Path.Combine(filePath, "TempDownload", folder, filename));
                            }
                            else
                            {
                                return Msg.GetMsg(GetCtx(), "VIS_AzureErrorOccurred");
                            }
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "VIS_AzureContainerUriEmpty");
                        }
                    }
                    //unzipfile
                    ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = 720;

                    z.ExtractZip(Path.Combine(filePath, "TempDownload", folder, zipFileName), Path.Combine(filePath, "TempDownload", folder), null);
                    System.IO.File.Copy(Path.Combine(filePath, "TempDownload", folder) + "\\" + AD_AttachmentLine_ID + ds.Tables[0].Rows[0][1], Path.Combine(filePath, "TempDownload", folder) + "\\" + ds.Tables[0].Rows[0][0]);
                    System.IO.File.Delete(Path.Combine(filePath, "TempDownload", folder) + "\\" + AD_AttachmentLine_ID + ds.Tables[0].Rows[0][1]);
                    System.IO.File.Delete(Path.Combine(filePath, "TempDownload", folder, zipFileName));

                    return folder;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "ERROR:" + ex.Message;
            }
        }



        private bool DownloadFtpFileWithoutRAM(string filename, string saveFilePath)
        {
            try
            {


                X_AD_ClientInfo cInfo = null;
                if (AD_Client_ID > 0)
                {
                    cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                }
                else
                {
                    cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                }


                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(cInfo.GetFTPUrl() + "//" + cInfo.GetFTPFolder() + "//" + filename);
                request.Credentials = new NetworkCredential(cInfo.GetFTPUsername(), cInfo.GetFTPPwd());
                request.UseBinary = true; // Use binary to ensure correct dlv!
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                //if (!Directory.Exists(saveFilePath))
                //{
                //    Directory.CreateDirectory(saveFilePath);
                //}

                FileStream writer = new FileStream(saveFilePath + "//" + filename, FileMode.Create);

                long length = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[2048];

                readCount = responseStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    writer.Write(buffer, 0, readCount);
                    readCount = responseStream.Read(buffer, 0, bufferSize);
                }

                responseStream.Close();
                response.Close();
                writer.Close();
                return true;

            }
            catch
            {
                return false;
                //Console.WriteLine(e.ToString());
            }
        }

        public string GetDocumentURI(int AD_Attachment_ID)
        {
            DataSet attRefDs = DB.ExecuteDataset(@"
SELECT  
ar.AD_AttachmentRef AS DocumentURI
FROM AD_Attachment att 
INNER JOIN AD_AttachmentLine al ON att.AD_Attachment_ID = al.AD_Attachment_ID
INNER JOIN AD_AttachmentReference ar ON al.AD_AttachmentLine_ID = ar.AD_AttachmentLine_ID
WHERE att.IsActive = 'Y' AND al.IsActive = 'Y' AND ar.IsActive = 'Y' AND att.AD_Attachment_ID = " + AD_Attachment_ID
);

            if (attRefDs != null && attRefDs.Tables.Count > 0 && attRefDs.Tables[0].Rows.Count > 0)
            {
                return Util.GetValueOfString(attRefDs.Tables[0].Rows[0]["DocumentURI"]);
            }
            return "";
        }

        /// <summary>
        /// Delete actual attachment files
        /// </summary>
        /// <param name="AttachmentLineIDs"></param>
        /// <returns></returns>
        public void DeleteAttachments(string[] AttachmentLineIDs)
        {
            try
            {
                string fileLocation = GetFileLocation();
                string filePath = System.IO.Path.Combine(GetServerLocation(), "Attachments");

                for (int i = 0; i < AttachmentLineIDs.Length; i++)
                {
                    string filename = GetAD_Table_ID() + "_" + GetRecord_ID() + "_" + AttachmentLineIDs[i];

                    if (fileLocation == X_AD_Attachment.FILELOCATION_ServerFileSystem)
                    {
                        // VIS_264: Delete file from attachments folder if exists
                        if (System.IO.File.Exists(Path.Combine(filePath, filename)))
                        {
                            System.IO.File.Delete(Path.Combine(filePath, filename));
                        }
                        continue;
                    }
                    if (fileLocation == X_AD_Attachment.FILELOCATION_FTPLocation)
                    {
                        DeleteFileFromFtpServer(filename);
                        continue;
                    }
                    if (fileLocation == X_AD_Attachment.FILELOCATION_Database)
                    {
                        // VIS_264: If file location is database, no action needed since whole attachment line row 
                        // will be deleted afterwards
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.WARNING, "DeleteAttachments -> ", ex.Message);
            }
        }

        /// <summary>
        /// Delete attachment file from ftp server
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private void DeleteFileFromFtpServer(string filename)
        {
            try
            {
                X_AD_ClientInfo cInfo = null;
                if (AD_Client_ID > 0)
                {
                    cInfo = new X_AD_ClientInfo(GetCtx(), AD_Client_ID, Get_Trx());
                }
                else
                {
                    cInfo = new X_AD_ClientInfo(GetCtx(), GetCtx().GetAD_Client_ID(), Get_Trx());
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(cInfo.GetFTPUrl() + "//" + cInfo.GetFTPFolder() + "//" + filename);
                request.Credentials = new NetworkCredential(cInfo.GetFTPUsername(), cInfo.GetFTPPwd());
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch(Exception ex)
            {
                _log.Log(Level.WARNING, "DeleteFileFromFtpServer -> ", ex.Message);
            }
        }
    }

    public class AttachmentLineInfo
    {
        public int Line_ID
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }

        public Decimal Size
        {
            get;
            set;
        }

        public string Filetype
        {
            get;
            set;
        }



    }


    public class AttFileInfo
    {
        public string Name
        {
            get;
            set;
        }
        public int Size
        {
            get;
            set;
        }
    }
}
