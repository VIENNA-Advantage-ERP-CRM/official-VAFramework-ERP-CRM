
namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for VAF_Attachment
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_Attachment : PO
    {
        public X_VAF_Attachment(Context ctx, int VAF_Attachment_ID, Trx trxName)
            : base(ctx, VAF_Attachment_ID, trxName)
        {
            /** if (VAF_Attachment_ID == 0)
            {
            SetVAF_Attachment_ID (0);
            SetVAF_TableView_ID (0);
            SetRecord_ID (0);
            SetTitle (null);
            }
             */
        }
        public X_VAF_Attachment(Ctx ctx, int VAF_Attachment_ID, Trx trxName)
            : base(ctx, VAF_Attachment_ID, trxName)
        {
            /** if (VAF_Attachment_ID == 0)
            {
            SetVAF_Attachment_ID (0);
            SetVAF_TableView_ID (0);
            SetRecord_ID (0);
            SetTitle (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_Attachment(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_Attachment(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_Attachment(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_Attachment()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514360668L;
        /** Last Updated Timestamp 7/29/2010 1:07:23 PM */
        public static long updatedMS = 1280389043879L;
        /** VAF_TableView_ID=254 */
        public static int Table_ID;
        // =254;

        /** TableName=VAF_Attachment */
        public static String Table_Name = "VAF_Attachment";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(6);
        /** AccessLevel
        @return 6 - System - Client 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAF_Attachment[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Attachment.
        @param VAF_Attachment_ID Attachment for the document */
        public void SetVAF_Attachment_ID(int VAF_Attachment_ID)
        {
            if (VAF_Attachment_ID < 1) throw new ArgumentException("VAF_Attachment_ID is mandatory.");
            Set_ValueNoCheck("VAF_Attachment_ID", VAF_Attachment_ID);
        }
        /** Get Attachment.
        @return Attachment for the document */
        public int GetVAF_Attachment_ID()
        {
            Object ii = Get_Value("VAF_Attachment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID < 1) throw new ArgumentException("VAF_TableView_ID is mandatory.");
            Set_ValueNoCheck("VAF_TableView_ID", VAF_TableView_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetVAF_TableView_ID()
        {
            Object ii = Get_Value("VAF_TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set BinaryData.
        @param BinaryData Binary Data */
        public void SetBinaryData(Byte[] BinaryData)
        {
            Set_ValueNoCheck("BinaryData", BinaryData);
        }
        /** Get BinaryData.
        @return Binary Data */
        public Byte[] GetBinaryData()
        {
            return (Byte[])Get_Value("BinaryData");
        }
        /** Set Record ID.
        @param Record_ID Direct internal record ID */
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID < 0) throw new ArgumentException("Record_ID is mandatory.");
            Set_ValueNoCheck("Record_ID", Record_ID);
        }
        /** Get Record ID.
        @return Direct internal record ID */
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Text Message.
        @param TextMsg Text Message */
        public void SetTextMsg(String TextMsg)
        {
            if (TextMsg != null && TextMsg.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                TextMsg = TextMsg.Substring(0, 2000);
            }
            Set_Value("TextMsg", TextMsg);
        }
        /** Get Text Message.
        @return Text Message */
        public String GetTextMsg()
        {
            return (String)Get_Value("TextMsg");
        }
        /** Set Title.
        @param Title Title of the Contact */
        public void SetTitle(String Title)
        {
            if (Title == null) throw new ArgumentException("Title is mandatory.");
            if (Title.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Title = Title.Substring(0, 60);
            }
            Set_Value("Title", Title);
        }
        /** Get Title.
        @return Title of the Contact */
        public String GetTitle()
        {
            return (String)Get_Value("Title");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetTitle());
        }


        /** FileLocation VAF_Control_Ref_ID=1000154 */
        public static int FILELOCATION_VAF_Control_Ref_ID = 1000154;
        /** Database = DB */
        public static String FILELOCATION_Database = "DB";
        /** FTP Location = FT */
        public static String FILELOCATION_FTPLocation = "FT";
        /** Server File System = SR */
        public static String FILELOCATION_ServerFileSystem = "SR";

        /** Web Service = WS */
        public static String FILELOCATION_WebService = "WS";

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsFileLocationValid(String test)
        {
            return test.Equals("DB") || test.Equals("FT") || test.Equals("SR") || test.Equals("WS");
        }
        /** Set File Location.
        @param FileLocation File Location */
        public void SetFileLocation(String FileLocation)
        {

            Set_Value("FileLocation", FileLocation);
        }
        /** Get File Location.
        @return File Location */
        public String GetFileLocation()
        {
            return (String)Get_Value("FileLocation");
        }
        /** CryptAndZipWay VAF_Control_Ref_ID=1000155 */
        public static int CRYPTANDZIPWAY_VAF_Control_Ref_ID = 1000155;
        /** Key Encryption And Old Zip = KSZ */
        public static String CRYPTANDZIPWAY_KeyEncryptionAndServerSideZip = "KSZ";
        /** Password Encryption And New Zip = PCZ */
        public static String CRYPTANDZIPWAY_PasswordEncryptionAndClientSideZip = "PCZ";
        /** Password Encryption And Old Zip = PSZ */
        public static String CRYPTANDZIPWAY_PasswordEncryptionAndServerSideZip = "PSZ";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCryptAndZipWayValid(String test)
        {
            return test == null || test.Equals("KSZ") || test.Equals("PCZ") || test.Equals("PSZ");
        }
        /** Set Crypt  And Zip Way.
        @param CryptAndZipWay Crypt  And Zip Way */
        public void SetCryptAndZipWay(String CryptAndZipWay)
        {
            if (!IsCryptAndZipWayValid(CryptAndZipWay))
                throw new ArgumentException("CryptAndZipWay Invalid value - " + CryptAndZipWay + " - Reference_ID=1000155 - KSZ - PCZ - PSZ");
            if (CryptAndZipWay != null && CryptAndZipWay.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                CryptAndZipWay = CryptAndZipWay.Substring(0, 3);
            }
            Set_Value("CryptAndZipWay", CryptAndZipWay);
        }
        /** Get Crypt  And Zip Way.
        @return Crypt  And Zip Way */
        public String GetCryptAndZipWay()
        {
            return (String)Get_Value("CryptAndZipWay");
        }

        ///** Set Default.
        //@param IsDefault Default value */
        //public void SetIsFileSystem(Boolean IsFileSystem)
        //{
        //    Set_Value("IsFileSystem", IsFileSystem);
        //}
        ///** Get Default.
        //@return Default value */
        //public Boolean IsFileSystem()
        //{
        //    Object oo = Get_Value("IsFileSystem");
        //    if (oo != null)
        //    {
        //        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        //        return "Y".Equals(oo);
        //    }
        //    return false;
        //}

        /** Set Valid.
        @param IsValid Element is valid */
        public void SetIsFromHTML(Boolean IsFromHTML)
        {
            Set_Value("IsFromHTML", IsFromHTML);
        }
        /** Get Valid.
        @return Element is valid */
        public Boolean IsFromHTML()
        {
            Object oo = Get_Value("IsFromHTML");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
    }

}
