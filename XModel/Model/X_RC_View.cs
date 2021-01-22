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
        /** VAF_TableView_ID=1000235 */
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
        @param VAF_Role_ID Responsibility Role */
        public void SetVAF_Role_ID(int VAF_Role_ID)
        {
            if (VAF_Role_ID <= 0) Set_Value("VAF_Role_ID", null);
            else
                Set_Value("VAF_Role_ID", VAF_Role_ID);
        }
        /** Get Role.
        @return Responsibility Role */
        public int GetVAF_Role_ID()
        {
            Object ii = Get_Value("VAF_Role_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
            if (VAF_TableView_ID <= 0) Set_Value("VAF_TableView_ID", null);
            else
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
            if (VAF_UserSearch_ID <= 0) Set_Value("VAF_UserSearch_ID", null);
            else
                Set_Value("VAF_UserSearch_ID", VAF_UserSearch_ID);
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
        @param VAF_UserContact_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Customer/Prospect Contact. */
        public int GetVAF_UserContact_ID()
        {
            Object ii = Get_Value("VAF_UserContact_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** BG_Color_ID VAF_Control_Ref_ID=266 */
        public static int BG_COLOR_ID_VAF_Control_Ref_ID = 266;
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

        /** Font_Color_ID VAF_Control_Ref_ID=266 */
        public static int FONT_COLOR_ID_VAF_Control_Ref_ID = 266;
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

        /** HeaderBG_Color_ID VAF_Control_Ref_ID=266 */
        public static int HEADERBG_COLOR_ID_VAF_Control_Ref_ID = 266;
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

        /** HeaderFont_Color_ID VAF_Control_Ref_ID=266 */
        public static int HEADERFONT_COLOR_ID_VAF_Control_Ref_ID = 266;
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
