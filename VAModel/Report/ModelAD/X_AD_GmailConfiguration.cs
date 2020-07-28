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
    /** Generated Model for AD_GmailConfiguration
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_GmailConfiguration : PO
    {
        public X_AD_GmailConfiguration(Context ctx, int AD_GmailConfiguration_ID, Trx trxName)
            : base(ctx, AD_GmailConfiguration_ID, trxName)
        {
            /** if (AD_GmailConfiguration_ID == 0)
            {
            SetAD_GmailConfiguration_ID (0);
            }
             */
        }
        public X_AD_GmailConfiguration(Ctx ctx, int AD_GmailConfiguration_ID, Trx trxName)
            : base(ctx, AD_GmailConfiguration_ID, trxName)
        {
            /** if (AD_GmailConfiguration_ID == 0)
            {
            SetAD_GmailConfiguration_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_GmailConfiguration(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_GmailConfiguration(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_GmailConfiguration(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_GmailConfiguration()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27630940028191L;
        /** Last Updated Timestamp 9/28/2012 12:15:11 PM */
        public static long updatedMS = 1348814711402L;
        /** AD_Table_ID=1000338 */
        public static int Table_ID;
        // =1000338;

        /** TableName=AD_GmailConfiguration */
        public static String Table_Name = "AD_GmailConfiguration";

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
            StringBuilder sb = new StringBuilder("X_AD_GmailConfiguration[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set AD_GmailConfiguration_ID.
        @param AD_GmailConfiguration_ID AD_GmailConfiguration_ID */
        public void SetAD_GmailConfiguration_ID(int AD_GmailConfiguration_ID)
        {
            if (AD_GmailConfiguration_ID < 1) throw new ArgumentException("AD_GmailConfiguration_ID is mandatory.");
            Set_ValueNoCheck("AD_GmailConfiguration_ID", AD_GmailConfiguration_ID);
        }
        /** Get AD_GmailConfiguration_ID.
        @return AD_GmailConfiguration_ID */
        public int GetAD_GmailConfiguration_ID()
        {
            Object ii = Get_Value("AD_GmailConfiguration_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Calender RefreshToken.
        @param CalenderRefreshToken Calender RefreshToken */
        public void SetCalenderRefreshToken(String CalenderRefreshToken)
        {
            if (CalenderRefreshToken != null && CalenderRefreshToken.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                CalenderRefreshToken = CalenderRefreshToken.Substring(0, 100);
            }
            Set_Value("CalenderRefreshToken", CalenderRefreshToken);
        }
        /** Get Calender RefreshToken.
        @return Calender RefreshToken */
        public String GetCalenderRefreshToken()
        {
            return (String)Get_Value("CalenderRefreshToken");
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
        /** Set Export Task BackGround.
        @param IsExportTaskBackGround Export Task BackGround */
        public void SetIsExportTaskBackGround(Boolean IsExportTaskBackGround)
        {
            Set_Value("IsExportTaskBackGround", IsExportTaskBackGround);
        }
        /** Get Export Task BackGround.
        @return Export Task BackGround */
        public Boolean IsExportTaskBackGround()
        {
            Object oo = Get_Value("IsExportTaskBackGround");
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
        /** Set Sync Task BackGround.
        @param IsSyncTaskBackGround Sync Task BackGround */
        public void SetIsSyncTaskBackGround(Boolean IsSyncTaskBackGround)
        {
            Set_Value("IsSyncTaskBackGround", IsSyncTaskBackGround);
        }
        /** Get Sync Task BackGround.
        @return Sync Task BackGround */
        public Boolean IsSyncTaskBackGround()
        {
            Object oo = Get_Value("IsSyncTaskBackGround");
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
        /** Set Refresh Token.
        @param RefreshToken Refresh Token */
        public void SetRefreshToken(String RefreshToken)
        {
            if (RefreshToken != null && RefreshToken.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                RefreshToken = RefreshToken.Substring(0, 200);
            }
            Set_Value("RefreshToken", RefreshToken);
        }
        /** Get Refresh Token.
        @return Refresh Token */
        public String GetRefreshToken()
        {
            return (String)Get_Value("RefreshToken");
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
