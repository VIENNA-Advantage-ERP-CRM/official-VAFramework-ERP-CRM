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
    /** Generated Model for RC_KPIPane
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_RC_KPIPane : PO
    {
        public X_RC_KPIPane(Context ctx, int RC_KPIPane_ID, Trx trxName)
            : base(ctx, RC_KPIPane_ID, trxName)
        {
            /** if (RC_KPIPane_ID == 0)
            {
            SetRC_KPIPane_ID (0);
            }
             */
        }
        public X_RC_KPIPane(Ctx ctx, int RC_KPIPane_ID, Trx trxName)
            : base(ctx, RC_KPIPane_ID, trxName)
        {
            /** if (RC_KPIPane_ID == 0)
            {
            SetRC_KPIPane_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_KPIPane(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_KPIPane(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_KPIPane(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_RC_KPIPane()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27688047911889L;
        /** Last Updated Timestamp 2014-07-21 11:33:16 AM */
        public static long updatedMS = 1405922595100L;
        /** AD_Table_ID=1000231 */
        public static int Table_ID;
        // =1000231;

        /** TableName=RC_KPIPane */
        public static String Table_Name = "RC_KPIPane";

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
            StringBuilder sb = new StringBuilder("X_RC_KPIPane[").Append(Get_ID()).Append("]");
            return sb.ToString();
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

        /** Height AD_Reference_ID=1000210 */
        public static int HEIGHT_AD_Reference_ID = 1000210;
        /** 100% = 100 */
        public static String HEIGHT_100 = "100";
        /** 50% = 50 */
        public static String HEIGHT_50 = "50";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsHeightValid(String test)
        {
            return test == null || test.Equals("100") || test.Equals("50");
        }
        /** Set Height.
        @param Height Height */
        public void SetHeight(String Height)
        {
            if (!IsHeightValid(Height))
                throw new ArgumentException("Height Invalid value - " + Height + " - Reference_ID=1000210 - 100 - 50");
            if (Height != null && Height.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                Height = Height.Substring(0, 3);
            }
            Set_Value("Height", Height);
        }
        /** Get Height.
        @return Height */
        public String GetHeight()
        {
            return (String)Get_Value("Height");
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
        /** Set KPI Pane.
        @param RC_KPIPane_ID KPI Pane */
        public void SetRC_KPIPane_ID(int RC_KPIPane_ID)
        {
            if (RC_KPIPane_ID < 1) throw new ArgumentException("RC_KPIPane_ID is mandatory.");
            Set_ValueNoCheck("RC_KPIPane_ID", RC_KPIPane_ID);
        }
        /** Get KPI Pane.
        @return KPI Pane */
        public int GetRC_KPIPane_ID()
        {
            Object ii = Get_Value("RC_KPIPane_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sequence.
        @param SeqNo Method of ordering elements;
         lowest number comes first */
        public void SetSeqNo(int SeqNo)
        {
            Set_Value("SeqNo", SeqNo);
        }
        /** Get Sequence.
        @return Method of ordering elements;
         lowest number comes first */
        public int GetSeqNo()
        {
            Object ii = Get_Value("SeqNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Width AD_Reference_ID=1000201 */
        public static int WIDTH_AD_Reference_ID = 1000201;
        /** 100% = 100 */
        public static String WIDTH_100 = "100";
        /** 25% = 25 */
        public static String WIDTH_25 = "25";
        /** 50% = 50 */
        public static String WIDTH_50 = "50";
        /** 75% = 75 */
        public static String WIDTH_75 = "75";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWidthValid(String test)
        {
            return test == null || test.Equals("100") || test.Equals("25") || test.Equals("50") || test.Equals("75");
        }
        /** Set Width.
        @param Width Width */
        public void SetWidth(String Width)
        {
            if (!IsWidthValid(Width))
                throw new ArgumentException("Width Invalid value - " + Width + " - Reference_ID=1000201 - 100 - 25 - 50 - 75");
            if (Width != null && Width.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                Width = Width.Substring(0, 3);
            }
            Set_Value("Width", Width);
        }
        /** Get Width.
        @return Width */
        public String GetWidth()
        {
            return (String)Get_Value("Width");
        }
    }

}
