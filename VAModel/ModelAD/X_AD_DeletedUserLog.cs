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
    /** Generated Model for AD_DeletedUserLog
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_DeletedUserLog : PO
    {
        public X_AD_DeletedUserLog(Context ctx, int AD_DeletedUserLog_ID, Trx trxName)
            : base(ctx, AD_DeletedUserLog_ID, trxName)
        {
            /** if (AD_DeletedUserLog_ID == 0)
            {
            SetAD_DeletedUserLog_ID (0);
            }
             */
        }
        public X_AD_DeletedUserLog(Ctx ctx, int AD_DeletedUserLog_ID, Trx trxName)
            : base(ctx, AD_DeletedUserLog_ID, trxName)
        {
            /** if (AD_DeletedUserLog_ID == 0)
            {
            SetAD_DeletedUserLog_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_DeletedUserLog(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_DeletedUserLog(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_DeletedUserLog(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_DeletedUserLog()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27628349880386L;
        /** Last Updated Timestamp 8/29/2012 12:46:03 PM */
        public static long updatedMS = 1346224563597L;
        /** AD_Table_ID=1000363 */
        public static int Table_ID;
        // =1000363;

        /** TableName=AD_DeletedUserLog */
        public static String Table_Name = "AD_DeletedUserLog";

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
            StringBuilder sb = new StringBuilder("X_AD_DeletedUserLog[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set AD_DeletedUserLog_ID.
        @param AD_DeletedUserLog_ID AD_DeletedUserLog_ID */
        public void SetAD_DeletedUserLog_ID(int AD_DeletedUserLog_ID)
        {
            if (AD_DeletedUserLog_ID < 1) throw new ArgumentException("AD_DeletedUserLog_ID is mandatory.");
            Set_ValueNoCheck("AD_DeletedUserLog_ID", AD_DeletedUserLog_ID);
        }
        /** Get AD_DeletedUserLog_ID.
        @return AD_DeletedUserLog_ID */
        public int GetAD_DeletedUserLog_ID()
        {
            Object ii = Get_Value("AD_DeletedUserLog_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AD_GmailConfiguration_ID.
        @param AD_GmailConfiguration_ID AD_GmailConfiguration_ID */
        public void SetAD_GmailConfiguration_ID(int AD_GmailConfiguration_ID)
        {
            if (AD_GmailConfiguration_ID <= 0) Set_Value("AD_GmailConfiguration_ID", null);
            else
                Set_Value("AD_GmailConfiguration_ID", AD_GmailConfiguration_ID);
        }
        /** Get AD_GmailConfiguration_ID.
        @return AD_GmailConfiguration_ID */
        public int GetAD_GmailConfiguration_ID()
        {
            Object ii = Get_Value("AD_GmailConfiguration_ID");
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
        /** Set Gmail UID.
        @param Gmail_UID Gmail UID */
        public void SetGmail_UID(String Gmail_UID)
        {
            if (Gmail_UID != null && Gmail_UID.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Gmail_UID = Gmail_UID.Substring(0, 100);
            }
            Set_Value("Gmail_UID", Gmail_UID);
        }
        /** Get Gmail UID.
        @return Gmail UID */
        public String GetGmail_UID()
        {
            return (String)Get_Value("Gmail_UID");
        }
        /** Set Deleted From Gmail.
        @param IsDeletedFromGmail Deleted From Gmail */
        public void SetIsDeletedFromGmail(Boolean IsDeletedFromGmail)
        {
            Set_Value("IsDeletedFromGmail", IsDeletedFromGmail);
        }
        /** Get Deleted From Gmail.
        @return Deleted From Gmail */
        public Boolean IsDeletedFromGmail()
        {
            Object oo = Get_Value("IsDeletedFromGmail");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
    }

}
