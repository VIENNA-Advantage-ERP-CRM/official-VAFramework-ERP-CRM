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
    /** Generated Model for VARC_KPI
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VARC_KPI : PO
    {
        public X_VARC_KPI(Context ctx, int VARC_KPI_ID, Trx trxName)
            : base(ctx, VARC_KPI_ID, trxName)
        {
            /** if (VARC_KPI_ID == 0)
            {
            SetVAF_TableView_ID (0);	// 0
            SetVARC_KPI_ID (0);
            }
             */
        }
        public X_VARC_KPI(Ctx ctx, int VARC_KPI_ID, Trx trxName)
            : base(ctx, VARC_KPI_ID, trxName)
        {
            /** if (VARC_KPI_ID == 0)
            {
            SetVAF_TableView_ID (0);	// 0
            SetVARC_KPI_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VARC_KPI(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VARC_KPI(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VARC_KPI(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VARC_KPI()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27699638137869L;
        /** Last Updated Timestamp 12/2/2014 3:03:41 PM */
        public static long updatedMS = 1417512821080L;
        /** VAF_TableView_ID=1000227 */
        public static int Table_ID;
        // =1000227;

        /** TableName=VARC_KPI */
        public static String Table_Name = "VARC_KPI";

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
            StringBuilder sb = new StringBuilder("X_VARC_KPI[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Column.
        @param VAF_Column_ID Column in the table */
        public void SetVAF_Column_ID(int VAF_Column_ID)
        {
            if (VAF_Column_ID <= 0) Set_Value("VAF_Column_ID", null);
            else
                Set_Value("VAF_Column_ID", VAF_Column_ID);
        }
        /** Get Column.
        @return Column in the table */
        public int GetVAF_Column_ID()
        {
            Object ii = Get_Value("VAF_Column_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Image.
        @param VAF_Image_ID Image or Icon */
        public void SetVAF_Image_ID(int VAF_Image_ID)
        {
            if (VAF_Image_ID <= 0) Set_Value("VAF_Image_ID", null);
            else
                Set_Value("VAF_Image_ID", VAF_Image_ID);
        }
        /** Get Image.
        @return Image or Icon */
        public int GetVAF_Image_ID()
        {
            Object ii = Get_Value("VAF_Image_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
           // if (VAF_TableView_ID < 1) throw new ArgumentException("VAF_TableView_ID is mandatory.");
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

        /** VAF_UserSearch_ID_1 VAF_Control_Ref_ID=1000182 */
        public static int VAF_USERSEARCH_ID_1_VAF_Control_Ref_ID = 1000182;
        /** Set User Query for Comparison.
        @param VAF_UserSearch_ID_1 User Query for Comparison */
        public void SetVAF_UserSearch_ID_1(int VAF_UserSearch_ID_1)
        {
            Set_Value("VAF_UserSearch_ID_1", VAF_UserSearch_ID_1);
        }
        /** Get User Query for Comparison.
        @return User Query for Comparison */
        public int GetVAF_UserSearch_ID_1()
        {
            Object ii = Get_Value("VAF_UserSearch_ID_1");
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

        /** VAF_Chart_BG_Color_ID VAF_Control_Ref_ID=266 */
        public static int VAF_Chart_BG_Color_ID_VAF_Control_Ref_ID = 266;
        /** Set Chart Background Color.
        @param VAF_Chart_BG_Color_ID Chart Background Color */
        public void SetVAF_Chart_BG_Color_ID(int VAF_Chart_BG_Color_ID)
        {
            if (VAF_Chart_BG_Color_ID <= 0) Set_Value("VAF_Chart_BG_Color_ID", null);
            else
                Set_Value("VAF_Chart_BG_Color_ID", VAF_Chart_BG_Color_ID);
        }
        /** Get Chart Background Color.
        @return Chart Background Color */
        public int GetVAF_Chart_BG_Color_ID()
        {
            Object ii = Get_Value("VAF_Chart_BG_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param VAB_UOM_ID Unit of Measure */
        public void SetVAB_UOM_ID(int VAB_UOM_ID)
        {
            if (VAB_UOM_ID <= 0) Set_Value("VAB_UOM_ID", null);
            else
                Set_Value("VAB_UOM_ID", VAB_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetVAB_UOM_ID()
        {
            Object ii = Get_Value("VAB_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Where Clause for Comparison.
        @param CompareWhereClause Where Clause for Comparison */
        public void SetCompareWhereClause(String CompareWhereClause)
        {
            if (CompareWhereClause != null && CompareWhereClause.Length > 250)
            {
                log.Warning("Length > 250 - truncated");
                CompareWhereClause = CompareWhereClause.Substring(0, 250);
            }
            Set_Value("CompareWhereClause", CompareWhereClause);
        }
        /** Get Where Clause for Comparison.
        @return Where Clause for Comparison */
        public String GetCompareWhereClause()
        {
            return (String)Get_Value("CompareWhereClause");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Description = Description.Substring(0, 200);
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
        /** Set IsCount.
        @param IsCount IsCount */
        public void SetIsCount(Boolean IsCount)
        {
            Set_Value("IsCount", IsCount);
        }
        /** Get IsCount.
        @return IsCount */
        public Boolean IsCount()
        {
            Object oo = Get_Value("IsCount");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Maximum.
        @param IsMaximum Maximum */
        public void SetIsMaximum(Boolean IsMaximum)
        {
            Set_Value("IsMaximum", IsMaximum);
        }
        /** Get Maximum.
        @return Maximum */
        public Boolean IsMaximum()
        {
            Object oo = Get_Value("IsMaximum");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Minimum.
        @param IsMinimum Minimum */
        public void SetIsMinimum(Boolean IsMinimum)
        {
            Set_Value("IsMinimum", IsMinimum);
        }
        /** Get Minimum.
        @return Minimum */
        public Boolean IsMinimum()
        {
            Object oo = Get_Value("IsMinimum");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Show Currency Symbol.
        @param IsShowCurrencySymbol Show Currency Symbol */
        public void SetIsShowCurrencySymbol(Boolean IsShowCurrencySymbol)
        {
            Set_Value("IsShowCurrencySymbol", IsShowCurrencySymbol);
        }
        /** Get Show Currency Symbol.
        @return Show Currency Symbol */
        public Boolean IsShowCurrencySymbol()
        {
            Object oo = Get_Value("IsShowCurrencySymbol");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sum.
        @param IsSum Sum */
        public void SetIsSum(Boolean IsSum)
        {
            Set_Value("IsSum", IsSum);
        }
        /** Get Sum.
        @return Sum */
        public Boolean IsSum()
        {
            Object oo = Get_Value("IsSum");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set View.
        @param IsView This is a view */
        public void SetIsView(Boolean IsView)
        {
            Set_Value("IsView", IsView);
        }
        /** Get View.
        @return This is a view */
        public Boolean IsView()
        {
            Object oo = Get_Value("IsView");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** KPIType VAF_Control_Ref_ID=1000179 */
        public static int KPITYPE_VAF_Control_Ref_ID = 1000179;
        /** Linear = Li */
        public static String KPITYPE_Linear = "Li";
        /** Radial = Ra */
        public static String KPITYPE_Radial = "Ra";
        /** Text = Te */
        public static String KPITYPE_Text = "Te";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsKPITypeValid(String test)
        {
            return test == null || test.Equals("Li") || test.Equals("Ra") || test.Equals("Te");
        }
        /** Set KPI Type.
        @param KPIType KPI Type */
        public void SetKPIType(String KPIType)
        {
            if (!IsKPITypeValid(KPIType))
                throw new ArgumentException("KPIType Invalid value - " + KPIType + " - Reference_ID=1000179 - Li - Ra - Te");
            if (KPIType != null && KPIType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                KPIType = KPIType.Substring(0, 2);
            }
            Set_Value("KPIType", KPIType);
        }
        /** Get KPI Type.
        @return KPI Type */
        public String GetKPIType()
        {
            return (String)Get_Value("KPIType");
        }

        /** LabelColor_ID VAF_Control_Ref_ID=266 */
        public static int LABELCOLOR_ID_VAF_Control_Ref_ID = 266;
        /** Set Label Color.
        @param LabelColor_ID Label Color */
        public void SetLabelColor_ID(int LabelColor_ID)
        {
            if (LabelColor_ID <= 0) Set_Value("LabelColor_ID", null);
            else
                Set_Value("LabelColor_ID", LabelColor_ID);
        }
        /** Get Label Color.
        @return Label Color */
        public int GetLabelColor_ID()
        {
            Object ii = Get_Value("LabelColor_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Show loss if value is greater than comparison value.
        @param LossForHeightValue Show loss if value is greater than comparison value */
        public void SetLossForHeightValue(Boolean LossForHeightValue)
        {
            Set_Value("LossForHeightValue", LossForHeightValue);
        }
        /** Get Show loss if value is greater than comparison value.
        @return Show loss if value is greater than comparison value */
        public Boolean IsLossForHeightValue()
        {
            Object oo = Get_Value("LossForHeightValue");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Major Unit.
        @param MajorUnit Major Unit */
        public void SetMajorUnit(String MajorUnit)
        {
            if (MajorUnit != null && MajorUnit.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                MajorUnit = MajorUnit.Substring(0, 50);
            }
            Set_Value("MajorUnit", MajorUnit);
        }
        /** Get Major Unit.
        @return Major Unit */
        public String GetMajorUnit()
        {
            return (String)Get_Value("MajorUnit");
        }
        /** Set Max Value.
        @param MaxValue Max Value */
        public void SetMaxValue(String MaxValue)
        {
            if (MaxValue != null && MaxValue.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                MaxValue = MaxValue.Substring(0, 50);
            }
            Set_Value("MaxValue", MaxValue);
        }
        /** Get Max Value.
        @return Max Value */
        public String GetMaxValue()
        {
            return (String)Get_Value("MaxValue");
        }
        /** Set Min Value.
        @param MinValue Min Value */
        public void SetMinValue(String MinValue)
        {
            if (MinValue != null && MinValue.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                MinValue = MinValue.Substring(0, 50);
            }
            Set_Value("MinValue", MinValue);
        }
        /** Get Min Value.
        @return Min Value */
        public String GetMinValue()
        {
            return (String)Get_Value("MinValue");
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Name = Name.Substring(0, 100);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Set Sql ORDER BY.
        @param OrderByClause Fully qualified ORDER BY clause */
        public void SetOrderByClause(String OrderByClause)
        {
            if (OrderByClause != null && OrderByClause.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                OrderByClause = OrderByClause.Substring(0, 50);
            }
            Set_Value("OrderByClause", OrderByClause);
        }
        /** Get Sql ORDER BY.
        @return Fully qualified ORDER BY clause */
        public String GetOrderByClause()
        {
            return (String)Get_Value("OrderByClause");
        }
        /** Set KPI.
        @param VARC_KPI_ID KPI */
        public void SetVARC_KPI_ID(int VARC_KPI_ID)
        {
            if (VARC_KPI_ID < 1) throw new ArgumentException("VARC_KPI_ID is mandatory.");
            Set_ValueNoCheck("VARC_KPI_ID", VARC_KPI_ID);
        }
        /** Get KPI.
        @return KPI */
        public int GetVARC_KPI_ID()
        {
            Object ii = Get_Value("VARC_KPI_ID");
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
        /** Set SearchKey.
        @param SEARCHKEY SearchKey */
        public void SetSEARCHKEY(String SEARCHKEY)
        {
            if (SEARCHKEY != null && SEARCHKEY.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                SEARCHKEY = SEARCHKEY.Substring(0, 50);
            }
            Set_Value("SEARCHKEY", SEARCHKEY);
        }
        /** Get SearchKey.
        @return SearchKey */
        public String GetSEARCHKEY()
        {
            return (String)Get_Value("SEARCHKEY");
        }
        /** Set Sequence.
        @param SeqNo Method of ordering elements;
         lowest number comes first */
        public void SetSeqNo(String SeqNo)
        {
            if (SeqNo != null && SeqNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                SeqNo = SeqNo.Substring(0, 50);
            }
            Set_Value("SeqNo", SeqNo);
        }
        /** Get Sequence.
        @return Method of ordering elements;
         lowest number comes first */
        public String GetSeqNo()
        {
            return (String)Get_Value("SeqNo");
        }
        /** Set Show On Home Screen.
        @param ShowOnHomeScreen Show On Home Screen */
        public void SetShowOnHomeScreen(Boolean ShowOnHomeScreen)
        {
            Set_Value("ShowOnHomeScreen", ShowOnHomeScreen);
        }
        /** Get Show On Home Screen.
        @return Show On Home Screen */
        public Boolean IsShowOnHomeScreen()
        {
            Object oo = Get_Value("ShowOnHomeScreen");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** TableView_ID VAF_Control_Ref_ID=1000186 */
        public static int TABLEVIEW_ID_VAF_Control_Ref_ID = 1000186;
        /** Set Table View.
        @param TableView_ID Table View */
        public void SetTableView_ID(int TableView_ID)
        {
            //if (TableView_ID <= 0) Set_Value("TableView_ID", null);
            //else
                Set_Value("TableView_ID", TableView_ID);
        }
        /** Get Table View.
        @return Table View */
        public int GetTableView_ID()
        {
            Object ii = Get_Value("TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sql WHERE.
        @param WhereClause Fully qualified SQL WHERE clause */
        public void SetWhereClause(String WhereClause)
        {
            if (WhereClause != null && WhereClause.Length > 250)
            {
                log.Warning("Length > 250 - truncated");
                WhereClause = WhereClause.Substring(0, 250);
            }
            Set_Value("WhereClause", WhereClause);
        }
        /** Get Sql WHERE.
        @return Fully qualified SQL WHERE clause */
        public String GetWhereClause()
        {
            return (String)Get_Value("WhereClause");
        }
        /** Set Width.
        @param Width Width */
        public void SetWidth(String Width)
        {
            if (Width != null && Width.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Width = Width.Substring(0, 50);
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
