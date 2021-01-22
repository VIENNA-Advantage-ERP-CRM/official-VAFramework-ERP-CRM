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
    /** Generated Model for VAF_UserSearch
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_UserSearch : PO
    {
        public X_VAF_UserSearch(Context ctx, int VAF_UserSearch_ID, Trx trxName)
            : base(ctx, VAF_UserSearch_ID, trxName)
        {
            /** if (VAF_UserSearch_ID == 0)
            {
            SetVAF_TableView_ID (0);
            SetVAF_UserSearch_ID (0);
            SetName (null);
            }
             */
        }
        public X_VAF_UserSearch(Ctx ctx, int VAF_UserSearch_ID, Trx trxName)
            : base(ctx, VAF_UserSearch_ID, trxName)
        {
            /** if (VAF_UserSearch_ID == 0)
            {
            SetVAF_TableView_ID (0);
            SetVAF_UserSearch_ID (0);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_UserSearch(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_UserSearch(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_UserSearch(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_UserSearch()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514365573L;
        /** Last Updated Timestamp 7/29/2010 1:07:28 PM */
        public static long updatedMS = 1280389048784L;
        /** VAF_TableView_ID=814 */
        public static int Table_ID;
        // =814;

        /** TableName=VAF_UserSearch */
        public static String Table_Name = "VAF_UserSearch";

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
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
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
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAF_UserSearch[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Tab.
        @param VAF_Tab_ID Tab within a Window */
        public void SetVAF_Tab_ID(int VAF_Tab_ID)
        {
            if (VAF_Tab_ID <= 0) Set_Value("VAF_Tab_ID", null);
            else
                Set_Value("VAF_Tab_ID", VAF_Tab_ID);
        }
        /** Get Tab.
        @return Tab within a Window */
        public int GetVAF_Tab_ID()
        {
            Object ii = Get_Value("VAF_Tab_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID < 1) throw new ArgumentException("VAF_TableView_ID is mandatory.");
            Set_Value("VAF_TableView_ID", VAF_TableView_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetVAF_TableView_ID()
        {
            Object ii = Get_Value("VAF_TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User Query.
        @param VAF_UserSearch_ID Saved User Query */
        public void SetVAF_UserSearch_ID(int VAF_UserSearch_ID)
        {
            if (VAF_UserSearch_ID < 1) throw new ArgumentException("VAF_UserSearch_ID is mandatory.");
            Set_ValueNoCheck("VAF_UserSearch_ID", VAF_UserSearch_ID);
        }
        /** Get User Query.
        @return Saved User Query */
        public int GetVAF_UserSearch_ID()
        {
            Object ii = Get_Value("VAF_UserSearch_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User/Contact.
        @param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetVAF_UserContact_ID()
        {
            Object ii = Get_Value("VAF_UserContact_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Code.
        @param Code Code to execute or to validate */
        public void SetCode(String Code)
        {
            if (Code != null && Code.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Code = Code.Substring(0, 2000);
            }
            Set_Value("Code", Code);
        }
        /** Get Code.
        @return Code to execute or to validate */
        public String GetCode()
        {
            return (String)Get_Value("Code");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }

        
    }

}
