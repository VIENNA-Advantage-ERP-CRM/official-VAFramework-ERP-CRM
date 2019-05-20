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
    /** Generated Model for GmailContactSettings
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_GmailContactSettings : PO
    {
        public X_GmailContactSettings(Context ctx, int GmailContactSettings_ID, Trx trxName)
            : base(ctx, GmailContactSettings_ID, trxName)
        {
            /** if (GmailContactSettings_ID == 0)
            {
            SetGmailContactSettings_ID (0);
            }
             */
        }
        public X_GmailContactSettings(Ctx ctx, int GmailContactSettings_ID, Trx trxName)
            : base(ctx, GmailContactSettings_ID, trxName)
        {
            /** if (GmailContactSettings_ID == 0)
            {
            SetGmailContactSettings_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GmailContactSettings(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GmailContactSettings(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GmailContactSettings(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_GmailContactSettings()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27627241055580L;
        /** Last Updated Timestamp 8/16/2012 4:45:38 PM */
        public static long updatedMS = 1345115738791L;
        /** AD_Table_ID=1000338 */
        public static int Table_ID;
        // =1000338;

        /** TableName=GmailContactSettings */
        public static String Table_Name = "GmailContactSettings";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_GmailContactSettings[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Role.
        @param AD_Role_ID Responsibility Role */
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            if (AD_Role_ID <= 0) Set_Value("AD_Role_ID", null);
            else
                Set_Value("AD_Role_ID", AD_Role_ID);
        }
        /** Get Role.
        @return Responsibility Role */
        public int GetAD_Role_ID()
        {
            Object ii = Get_Value("AD_Role_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set GmailContactSettings_ID.
        @param GmailContactSettings_ID GmailContactSettings_ID */
        public void SetGmailContactSettings_ID(int GmailContactSettings_ID)
        {
            if (GmailContactSettings_ID < 1) throw new ArgumentException("GmailContactSettings_ID is mandatory.");
            Set_ValueNoCheck("GmailContactSettings_ID", GmailContactSettings_ID);
        }
        /** Get GmailContactSettings_ID.
        @return GmailContactSettings_ID */
        public int GetGmailContactSettings_ID()
        {
            Object ii = Get_Value("GmailContactSettings_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Import Contacts.
        @param ImportContacts Import Contacts */
        public void SetImportContacts(String ImportContacts)
        {
            if (ImportContacts != null && ImportContacts.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ImportContacts = ImportContacts.Substring(0, 50);
            }
            Set_Value("ImportContacts", ImportContacts);
        }
        /** Get Import Contacts.
        @return Import Contacts */
        public String GetImportContacts()
        {
            return (String)Get_Value("ImportContacts");
        }
        /** Set Export Background.
        @param IsExportBackground Export Background */
        public void SetIsExportBackground(Boolean IsExportBackground)
        {
            Set_Value("IsExportBackground", IsExportBackground);
        }
        /** Get Export Background.
        @return Export Background */
        public Boolean IsExportBackground()
        {
            Object oo = Get_Value("IsExportBackground");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sync Background.
        @param IsSyncBackground Sync Background */
        public void SetIsSyncBackground(Boolean IsSyncBackground)
        {
            Set_Value("IsSyncBackground", IsSyncBackground);
        }
        /** Get Sync Background.
        @return Sync Background */
        public Boolean IsSyncBackground()
        {
            Object oo = Get_Value("IsSyncBackground");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sync Deleted User.
        @param IsSyncDeletedUser Sync Deleted User */
        public void SetIsSyncDeletedUser(Boolean IsSyncDeletedUser)
        {
            Set_Value("IsSyncDeletedUser", IsSyncDeletedUser);
        }
        /** Get Sync Deleted User.
        @return Sync Deleted User */
        public Boolean IsSyncDeletedUser()
        {
            Object oo = Get_Value("IsSyncDeletedUser");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set IsUpdateExistingRecord.
        @param IsUpdateExistingRecord IsUpdateExistingRecord */
        public void SetIsUpdateExistingRecord(Boolean IsUpdateExistingRecord)
        {
            Set_Value("IsUpdateExistingRecord", IsUpdateExistingRecord);
        }
        /** Get IsUpdateExistingRecord.
        @return IsUpdateExistingRecord */
        public Boolean IsUpdateExistingRecord()
        {
            Object oo = Get_Value("IsUpdateExistingRecord");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Password.
        @param Password Password of any length (case sensitive) */
        public void SetPassword(String Password)
        {
            if (Password != null && Password.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Password = Password.Substring(0, 50);
            }
            Set_Value("Password", Password);
        }
        /** Get Password.
        @return Password of any length (case sensitive) */
        public String GetPassword()
        {
            return (String)Get_Value("Password");
        }
        /** Set Registered EMail.
        @param UserName Email of the responsible for the System */
        public void SetUserName(String UserName)
        {
            if (UserName != null && UserName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                UserName = UserName.Substring(0, 50);
            }
            Set_Value("UserName", UserName);
        }
        /** Get Registered EMail.
        @return Email of the responsible for the System */
        public String GetUserName()
        {
            return (String)Get_Value("UserName");
        }
    }

}
