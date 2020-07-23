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
    /** Generated Model for D_Chart
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_D_Chart : PO
    {
        public X_D_Chart(Context ctx, int D_Chart_ID, Trx trxName)
            : base(ctx, D_Chart_ID, trxName)
        {
            /** if (D_Chart_ID == 0)
            {
            SetD_Chart_ID (0);
            SetName (null);
            }
             */
        }
        public X_D_Chart(Ctx ctx, int D_Chart_ID, Trx trxName)
            : base(ctx, D_Chart_ID, trxName)
        {
            /** if (D_Chart_ID == 0)
            {
            SetD_Chart_ID (0);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_D_Chart(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_D_Chart(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_D_Chart(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_D_Chart()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27716737942354L;
        /** Last Updated Timestamp 6/18/2015 1:00:26 PM */
        public static long updatedMS = 1434612625565L;
        /** AD_Table_ID=1000005 */
        public static int Table_ID;
        // =1000005;

        /** TableName=D_Chart */
        public static String Table_Name = "D_Chart";

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
            StringBuilder sb = new StringBuilder("X_D_Chart[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** Ad_Chart_BG_Color_ID AD_Reference_ID=266 */
        public static int AD_CHART_BG_COLOR_ID_AD_Reference_ID = 266;
        /** Set Chart Background Color.
        @param Ad_Chart_BG_Color_ID Chart Background Color */
        public void SetAd_Chart_BG_Color_ID(int Ad_Chart_BG_Color_ID)
        {
            if (Ad_Chart_BG_Color_ID <= 0) Set_Value("Ad_Chart_BG_Color_ID", null);
            else
                Set_Value("Ad_Chart_BG_Color_ID", Ad_Chart_BG_Color_ID);
        }
        /** Get Chart Background Color.
        @return Chart Background Color */
        public int GetAd_Chart_BG_Color_ID()
        {
            Object ii = Get_Value("Ad_Chart_BG_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** ChartType AD_Reference_ID=1000000 */
        public static int CHARTTYPE_AD_Reference_ID = 1000000;
        /** Column = 1 */
        public static String CHARTTYPE_Column = "1";
        /** Line = 2 */
        public static String CHARTTYPE_Line = "2";
        /** Pie = 3 */
        public static String CHARTTYPE_Pie = "3";
        /** Bar = 4 */
        public static String CHARTTYPE_Bar = "4";
        /** Donut = 5 */
        public static String CHARTTYPE_Donut = "5";
        /** Area = 6 */
        public static String CHARTTYPE_Area = "6";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsChartTypeValid(String test)
        {
            return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6");
        }
        /** Set Chart Type.
        @param ChartType Chart Type */
        public void SetChartType(String ChartType)
        {
            if (!IsChartTypeValid(ChartType))
                throw new ArgumentException("ChartType Invalid value - " + ChartType + " - Reference_ID=1000000 - 1 - 2 - 3 - 4 - 5 - 6");
            if (ChartType != null && ChartType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ChartType = ChartType.Substring(0, 1);
            }
            Set_Value("ChartType", ChartType);
        }
        /** Get Chart Type.
        @return Chart Type */
        public String GetChartType()
        {
            return (String)Get_Value("ChartType");
        }
        /** Set Chart Name.
        @param D_Chart_ID Chart Name */
        public void SetD_Chart_ID(int D_Chart_ID)
        {
            if (D_Chart_ID < 1) throw new ArgumentException("D_Chart_ID is mandatory.");
            Set_ValueNoCheck("D_Chart_ID", D_Chart_ID);
        }
        /** Get Chart Name.
        @return Chart Name */
        public int GetD_Chart_ID()
        {
            Object ii = Get_Value("D_Chart_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Enable 3D.
        @param Enable3D Enable 3D */
        public void SetEnable3D(Boolean Enable3D)
        {
            Set_Value("Enable3D", Enable3D);
        }
        /** Get Enable 3D.
        @return Enable 3D */
        public Boolean IsEnable3D()
        {
            Object oo = Get_Value("Enable3D");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set File Name.
        @param FileName Name of the local file or URL */
        public void SetFileName(String FileName)
        {
            if (FileName != null && FileName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                FileName = FileName.Substring(0, 50);
            }
            Set_Value("FileName", FileName);
        }
        /** Get File Name.
        @return Name of the local file or URL */
        public String GetFileName()
        {
            return (String)Get_Value("FileName");
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
        /** Set Show Legend.
        @param IsShowLegend Show Legend */
        public void SetIsShowLegend(Boolean IsShowLegend)
        {
            Set_Value("IsShowLegend", IsShowLegend);
        }
        /** Get Show Legend.
        @return Show Legend */
        public Boolean IsShowLegend()
        {
            Object oo = Get_Value("IsShowLegend");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Show Zero Series.
        @param IsShowZeroSeries Show Zero Series */
        public void SetIsShowZeroSeries(Boolean IsShowZeroSeries)
        {
            Set_Value("IsShowZeroSeries", IsShowZeroSeries);
        }
        /** Get Show Zero Series.
        @return Show Zero Series */
        public Boolean IsShowZeroSeries()
        {
            Object oo = Get_Value("IsShowZeroSeries");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Stacked Chart.
        @param IsStackedChart Stacked Chart */
        public void SetIsStackedChart(Boolean IsStackedChart)
        {
            Set_Value("IsStackedChart", IsStackedChart);
        }
        /** Get Stacked Chart.
        @return Stacked Chart */
        public Boolean IsStackedChart()
        {
            Object oo = Get_Value("IsStackedChart");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Show Tilt Label.
        @param IsTiltLabel Show Tilt Label */
        public void SetIsTiltLabel(Boolean IsTiltLabel)
        {
            Set_Value("IsTiltLabel", IsTiltLabel);
        }
        /** Get Show Tilt Label.
        @return Show Tilt Label */
        public Boolean IsTiltLabel()
        {
            Object oo = Get_Value("IsTiltLabel");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** LabelColor_ID AD_Reference_ID=266 */
        public static int LABELCOLOR_ID_AD_Reference_ID = 266;
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
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Name = Name.Substring(0, 200);
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
        /** Set Excel File.
        @param VADB_ChartExcelFile_ID Excel File */
        public void SetVADB_ChartExcelFile_ID(int VADB_ChartExcelFile_ID)
        {
            if (VADB_ChartExcelFile_ID <= 0) Set_Value("VADB_ChartExcelFile_ID", null);
            else
                Set_Value("VADB_ChartExcelFile_ID", VADB_ChartExcelFile_ID);
        }
        /** Get Excel File.
        @return Excel File */
        public int GetVADB_ChartExcelFile_ID()
        {
            Object ii = Get_Value("VADB_ChartExcelFile_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Excel Chart.
        @param VADB_IsExcelChart Excel Chart */
        public void SetVADB_IsExcelChart(Boolean VADB_IsExcelChart)
        {
            Set_Value("VADB_IsExcelChart", VADB_IsExcelChart);
        }
        /** Get Excel Chart.
        @return Excel Chart */
        public Boolean IsVADB_IsExcelChart()
        {
            Object oo = Get_Value("VADB_IsExcelChart");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
    }

}
