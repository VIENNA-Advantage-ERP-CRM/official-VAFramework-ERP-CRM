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
    /** Generated Model for RC_ChartPane
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_RC_ChartPane : PO
    {
        public X_RC_ChartPane(Context ctx, int RC_ChartPane_ID, Trx trxName)
            : base(ctx, RC_ChartPane_ID, trxName)
        {
            /** if (RC_ChartPane_ID == 0)
            {
            SetRC_ChartPane_ID (0);
            }
             */
        }
        public X_RC_ChartPane(Ctx ctx, int RC_ChartPane_ID, Trx trxName)
            : base(ctx, RC_ChartPane_ID, trxName)
        {
            /** if (RC_ChartPane_ID == 0)
            {
            SetRC_ChartPane_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ChartPane(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ChartPane(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ChartPane(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_RC_ChartPane()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27688047957533L;
        /** Last Updated Timestamp 2014-07-21 11:34:00 AM */
        public static long updatedMS = 1405922640744L;
        /** AD_Table_ID=1000226 */
        public static int Table_ID;
        // =1000226;

        /** TableName=RC_ChartPane */
        public static String Table_Name = "RC_ChartPane";

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
            StringBuilder sb = new StringBuilder("X_RC_ChartPane[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Colspan.
        @param Colspan Colspan */
        public void SetColspan(int Colspan)
        {
            Set_Value("Colspan", Colspan);
        }
        /** Get Colspan.
        @return Colspan */
        public int GetColspan()
        {
            Object ii = Get_Value("Colspan");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Chart Name.
        @param D_Chart_ID Chart Name */
        public void SetD_Chart_ID(int D_Chart_ID)
        {
            if (D_Chart_ID <= 0) Set_Value("D_Chart_ID", null);
            else
                Set_Value("D_Chart_ID", D_Chart_ID);
        }
        /** Get Chart Name.
        @return Chart Name */
        public int GetD_Chart_ID()
        {
            Object ii = Get_Value("D_Chart_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set RC_ChartPane_ID.
        @param RC_ChartPane_ID RC_ChartPane_ID */
        public void SetRC_ChartPane_ID(int RC_ChartPane_ID)
        {
            if (RC_ChartPane_ID < 1) throw new ArgumentException("RC_ChartPane_ID is mandatory.");
            Set_ValueNoCheck("RC_ChartPane_ID", RC_ChartPane_ID);
        }
        /** Get RC_ChartPane_ID.
        @return RC_ChartPane_ID */
        public int GetRC_ChartPane_ID()
        {
            Object ii = Get_Value("RC_ChartPane_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Rowspan.
        @param Rowspan Rowspan */
        public void SetRowspan(int Rowspan)
        {
            Set_Value("Rowspan", Rowspan);
        }
        /** Get Rowspan.
        @return Rowspan */
        public int GetRowspan()
        {
            Object ii = Get_Value("Rowspan");
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
