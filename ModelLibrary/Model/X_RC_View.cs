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
    /** Generated Model for RC_View
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_RC_View : PO
    {
        public X_RC_View(Context ctx, int RC_View_ID, Trx trxName)
            : base(ctx, RC_View_ID, trxName)
        {
            /** if (RC_View_ID == 0)
            {
            SetRC_View_ID (0);
            }
             */
        }
        public X_RC_View(Ctx ctx, int RC_View_ID, Trx trxName)
            : base(ctx, RC_View_ID, trxName)
        {
            /** if (RC_View_ID == 0)
            {
            SetRC_View_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_View(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_View(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_View(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_RC_View()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27706189007700L;
        /** Last Updated Timestamp 2/16/2015 10:44:50 AM */
        public static long updatedMS = 1424063690911L;
        /** AD_Table_ID=1000235 */
        public static int Table_ID;
        // =1000235;

        /** TableName=RC_View */
        public static String Table_Name = "RC_View";

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
            StringBuilder sb = new StringBuilder("X_RC_View[").Append(Get_ID()).Append("]");
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
        /** Set Tab.
        @param AD_Tab_ID Tab within a Window */
        public void SetAD_Tab_ID(int AD_Tab_ID)
        {
            if (AD_Tab_ID <= 0) Set_Value("AD_Tab_ID", null);
            else
                Set_Value("AD_Tab_ID", AD_Tab_ID);
        }
        /** Get Tab.
        @return Tab within a Window */
        public int GetAD_Tab_ID()
        {
            Object ii = Get_Value("AD_Tab_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID <= 0) Set_Value("AD_Table_ID", null);
            else
                Set_Value("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User Query.
        @param AD_UserQuery_ID Saved User Query */
        public void SetAD_UserQuery_ID(int AD_UserQuery_ID)
        {
            if (AD_UserQuery_ID <= 0) Set_Value("AD_UserQuery_ID", null);
            else
                Set_Value("AD_UserQuery_ID", AD_UserQuery_ID);
        }
        /** Get User Query.
        @return Saved User Query */
        public int GetAD_UserQuery_ID()
        {
            Object ii = Get_Value("AD_UserQuery_ID");
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

        /** BG_Color_ID AD_Reference_ID=266 */
        public static int BG_COLOR_ID_AD_Reference_ID = 266;
        /** Set BG Color.
        @param BG_Color_ID BG Color */
        public void SetBG_Color_ID(int BG_Color_ID)
        {
            if (BG_Color_ID <= 0) Set_Value("BG_Color_ID", null);
            else
                Set_Value("BG_Color_ID", BG_Color_ID);
        }
        /** Get BG Color.
        @return BG Color */
        public int GetBG_Color_ID()
        {
            Object ii = Get_Value("BG_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Description = Description.Substring(0, 50);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }

        /** Font_Color_ID AD_Reference_ID=266 */
        public static int FONT_COLOR_ID_AD_Reference_ID = 266;
        /** Set Font Color.
        @param Font_Color_ID Font Color */
        public void SetFont_Color_ID(int Font_Color_ID)
        {
            if (Font_Color_ID <= 0) Set_Value("Font_Color_ID", null);
            else
                Set_Value("Font_Color_ID", Font_Color_ID);
        }
        /** Get Font Color.
        @return Font Color */
        public int GetFont_Color_ID()
        {
            Object ii = Get_Value("Font_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** HeaderBG_Color_ID AD_Reference_ID=266 */
        public static int HEADERBG_COLOR_ID_AD_Reference_ID = 266;
        /** Set Header BG Color.
        @param HeaderBG_Color_ID Header BG Color */
        public void SetHeaderBG_Color_ID(int HeaderBG_Color_ID)
        {
            if (HeaderBG_Color_ID <= 0) Set_Value("HeaderBG_Color_ID", null);
            else
                Set_Value("HeaderBG_Color_ID", HeaderBG_Color_ID);
        }
        /** Get Header BG Color.
        @return Header BG Color */
        public int GetHeaderBG_Color_ID()
        {
            Object ii = Get_Value("HeaderBG_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** HeaderFont_Color_ID AD_Reference_ID=266 */
        public static int HEADERFONT_COLOR_ID_AD_Reference_ID = 266;
        /** Set Header Font Color.
        @param HeaderFont_Color_ID Header Font Color */
        public void SetHeaderFont_Color_ID(int HeaderFont_Color_ID)
        {
            if (HeaderFont_Color_ID <= 0) Set_Value("HeaderFont_Color_ID", null);
            else
                Set_Value("HeaderFont_Color_ID", HeaderFont_Color_ID);
        }
        /** Get Header Font Color.
        @return Header Font Color */
        public int GetHeaderFont_Color_ID()
        {
            Object ii = Get_Value("HeaderFont_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set For New Tenant.
        @param IsForNewTenant For New Tenant */
        public void SetIsForNewTenant(Boolean IsForNewTenant)
        {
            Set_Value("IsForNewTenant", IsForNewTenant);
        }
        /** Get For New Tenant.
        @return For New Tenant */
        public Boolean IsForNewTenant()
        {
            Object oo = Get_Value("IsForNewTenant");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Max Value.
        @param MaxValue Max Value */
        public void SetMaxValue(int MaxValue)
        {
            Set_Value("MaxValue", MaxValue);
        }
        /** Get Max Value.
        @return Max Value */
        public int GetMaxValue()
        {
            Object ii = Get_Value("MaxValue");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Min Value.
        @param MinValue Min Value */
        public void SetMinValue(int MinValue)
        {
            Set_Value("MinValue", MinValue);
        }
        /** Get Min Value.
        @return Min Value */
        public int GetMinValue()
        {
            Object ii = Get_Value("MinValue");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Name = Name.Substring(0, 50);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Set Role Center View.
        @param RC_View_ID Role Center View */
        public void SetRC_View_ID(int RC_View_ID)
        {
            if (RC_View_ID < 1) throw new ArgumentException("RC_View_ID is mandatory.");
            Set_ValueNoCheck("RC_View_ID", RC_View_ID);
        }
        /** Get Role Center View.
        @return Role Center View */
        public int GetRC_View_ID()
        {
            Object ii = Get_Value("RC_View_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Record ID.
        @param Record_ID Direct internal record ID */
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID <= 0) Set_Value("Record_ID", null);
            else
                Set_Value("Record_ID", Record_ID);
        }
        /** Get Record ID.
        @return Direct internal record ID */
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Title.
        @param Title Title of the Contact */
        public void SetTitle(String Title)
        {
            if (Title != null && Title.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Title = Title.Substring(0, 50);
            }
            Set_Value("Title", Title);
        }
        /** Get Title.
        @return Title of the Contact */
        public String GetTitle()
        {
            return (String)Get_Value("Title");
        }
        /** Set Sql WHERE.
        @param WhereClause Fully qualified SQL WHERE clause */
        public void SetWhereClause(String WhereClause)
        {
            if (WhereClause != null && WhereClause.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                WhereClause = WhereClause.Substring(0, 500);
            }
            Set_Value("WhereClause", WhereClause);
        }
        /** Get Sql WHERE.
        @return Fully qualified SQL WHERE clause */
        public String GetWhereClause()
        {
            return (String)Get_Value("WhereClause");
        }
    }

}
