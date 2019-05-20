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
    /** Generated Model for D_Series
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_D_Series : PO
    {
        public X_D_Series(Context ctx, int D_Series_ID, Trx trxName)
            : base(ctx, D_Series_ID, trxName)
        {
            /** if (D_Series_ID == 0)
            {
            SetAD_Column_X_ID (0);
            SetAD_Column_Y_ID (0);
            SetAD_Table_ID (0);
            SetAlertMessage (null);
            SetAlertValue_X (null);
            SetD_Chart_ID (0);
            SetD_Series_ID (0);
            SetDataType_X (null);
            SetDataType_Y (null);
            SetEndValue (0);
            SetName (null);
            SetOrderByColumn (null);
            SetWhereClause (null);
            SetWhereCondition (null);
            }
             */
        }
        public X_D_Series(Ctx ctx, int D_Series_ID, Trx trxName)
            : base(ctx, D_Series_ID, trxName)
        {
            /** if (D_Series_ID == 0)
            {
            SetAD_Column_X_ID (0);
            SetAD_Column_Y_ID (0);
            SetAD_Table_ID (0);
            SetAlertMessage (null);
            SetAlertValue_X (null);
            SetD_Chart_ID (0);
            SetD_Series_ID (0);
            SetDataType_X (null);
            SetDataType_Y (null);
            SetEndValue (0);
            SetName (null);
            SetOrderByColumn (null);
            SetWhereClause (null);
            SetWhereCondition (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_D_Series(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_D_Series(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_D_Series(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_D_Series()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27701444768925L;
        /** Last Updated Timestamp 23-12-2014 12:54:12 */
        public static long updatedMS = 1419319452136L;
        /** AD_Table_ID=1000007 */
        public static int Table_ID;
        // =1000007;

        /** TableName=D_Series */
        public static String Table_Name = "D_Series";

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
            StringBuilder sb = new StringBuilder("X_D_Series[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_Column_X_ID AD_Reference_ID=414 */
        public static int AD_COLUMN_X_ID_AD_Reference_ID = 414;
        /** Set Column X.
        @param AD_Column_X_ID Column X */
        public void SetAD_Column_X_ID(int AD_Column_X_ID)
        {
            if (AD_Column_X_ID < 1) throw new ArgumentException("AD_Column_X_ID is mandatory.");
            Set_Value("AD_Column_X_ID", AD_Column_X_ID);
        }
        /** Get Column X.
        @return Column X */
        public int GetAD_Column_X_ID()
        {
            Object ii = Get_Value("AD_Column_X_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Column_Y_ID AD_Reference_ID=414 */
        public static int AD_COLUMN_Y_ID_AD_Reference_ID = 414;
        /** Set Column Y.
        @param AD_Column_Y_ID Column Y */
        public void SetAD_Column_Y_ID(int AD_Column_Y_ID)
        {
            if (AD_Column_Y_ID < 1) throw new ArgumentException("AD_Column_Y_ID is mandatory.");
            Set_Value("AD_Column_Y_ID", AD_Column_Y_ID);
        }
        /** Get Column Y.
        @return Column Y */
        public int GetAD_Column_Y_ID()
        {
            Object ii = Get_Value("AD_Column_Y_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_DateColumn_ID AD_Reference_ID=414 */
        public static int AD_DATECOLUMN_ID_AD_Reference_ID = 414;
        /** Set Date Column.
        @param AD_DateColumn_ID Date Column */
        public void SetAD_DateColumn_ID(int AD_DateColumn_ID)
        {
            if (AD_DateColumn_ID <= 0) Set_Value("AD_DateColumn_ID", null);
            else
                Set_Value("AD_DateColumn_ID", AD_DateColumn_ID);
        }
        /** Get Date Column.
        @return Date Column */
        public int GetAD_DateColumn_ID()
        {
            Object ii = Get_Value("AD_DateColumn_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Gradient_Color_ID AD_Reference_ID=266 */
        public static int AD_GRADIENT_COLOR_ID_AD_Reference_ID = 266;
        /** Set Series Gradient Color.
        @param AD_Gradient_Color_ID Series Gradient Color */
        public void SetAD_Gradient_Color_ID(int AD_Gradient_Color_ID)
        {
            if (AD_Gradient_Color_ID <= 0) Set_Value("AD_Gradient_Color_ID", null);
            else
                Set_Value("AD_Gradient_Color_ID", AD_Gradient_Color_ID);
        }
        /** Get Series Gradient Color.
        @return Series Gradient Color */
        public int GetAD_Gradient_Color_ID()
        {
            Object ii = Get_Value("AD_Gradient_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Series_Color_ID AD_Reference_ID=266 */
        public static int AD_SERIES_COLOR_ID_AD_Reference_ID = 266;
        /** Set Series Color.
        @param AD_Series_Color_ID Series Color */
        public void SetAD_Series_Color_ID(int AD_Series_Color_ID)
        {
            if (AD_Series_Color_ID <= 0) Set_Value("AD_Series_Color_ID", null);
            else
                Set_Value("AD_Series_Color_ID", AD_Series_Color_ID);
        }
        /** Get Series Color.
        @return Series Color */
        public int GetAD_Series_Color_ID()
        {
            Object ii = Get_Value("AD_Series_Color_ID");
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

        /** AD_Table_ID AD_Reference_ID=415 */
        public static int AD_TABLE_ID_AD_Reference_ID = 415;
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
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
        /** Set AlertLastRun.
        @param AlertLastRun AlertLastRun */
        public void SetAlertLastRun(DateTime? AlertLastRun)
        {
            Set_Value("AlertLastRun", (DateTime?)AlertLastRun);
        }
        /** Get AlertLastRun.
        @return AlertLastRun */
        public DateTime? GetAlertLastRun()
        {
            return (DateTime?)Get_Value("AlertLastRun");
        }
        /** Set Alert Message.
        @param AlertMessage Message of the Alert */
        public void SetAlertMessage(String AlertMessage)
        {
            if (AlertMessage == null) throw new ArgumentException("AlertMessage is mandatory.");
            if (AlertMessage.Length > 150)
            {
                log.Warning("Length > 150 - truncated");
                AlertMessage = AlertMessage.Substring(0, 150);
            }
            Set_Value("AlertMessage", AlertMessage);
        }
        /** Get Alert Message.
        @return Message of the Alert */
        public String GetAlertMessage()
        {
            return (String)Get_Value("AlertMessage");
        }
        /** Set Alert Value Y.
        @param AlertValue Alert Value Y */
        public void SetAlertValue(String AlertValue)
        {
            if (AlertValue != null && AlertValue.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                AlertValue = AlertValue.Substring(0, 50);
            }
            Set_Value("AlertValue", AlertValue);
        }
        /** Get Alert Value Y.
        @return Alert Value Y */
        public String GetAlertValue()
        {
            return (String)Get_Value("AlertValue");
        }
        /** Set AlertValue_X.
        @param AlertValue_X AlertValue_X */
        public void SetAlertValue_X(String AlertValue_X)
        {
            if (AlertValue_X == null) throw new ArgumentException("AlertValue_X is mandatory.");
            if (AlertValue_X.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                AlertValue_X = AlertValue_X.Substring(0, 100);
            }
            Set_Value("AlertValue_X", AlertValue_X);
        }
        /** Get AlertValue_X.
        @return AlertValue_X */
        public String GetAlertValue_X()
        {
            return (String)Get_Value("AlertValue_X");
        }
        /** Set BindWithView.
        @param BindWithView BindWithView */
        public void SetBindWithView(Boolean BindWithView)
        {
            Set_Value("BindWithView", BindWithView);
        }
        /** Get BindWithView.
        @return BindWithView */
        public Boolean IsBindWithView()
        {
            Object oo = Get_Value("BindWithView");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Series Name.
        @param D_Series_ID Series Name */
        public void SetD_Series_ID(int D_Series_ID)
        {
            if (D_Series_ID < 1) throw new ArgumentException("D_Series_ID is mandatory.");
            Set_ValueNoCheck("D_Series_ID", D_Series_ID);
        }
        /** Get Series Name.
        @return Series Name */
        public int GetD_Series_ID()
        {
            Object ii = Get_Value("D_Series_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Data Type.
        @param DataType Type of data */
        public void SetDataType(Boolean DataType)
        {
            Set_Value("DataType", DataType);
        }
        /** Get Data Type.
        @return Type of data */
        public Boolean IsDataType()
        {
            Object oo = Get_Value("DataType");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** DataType_X AD_Reference_ID=1000001 */
        public static int DATATYPE_X_AD_Reference_ID = 1000001;
        /** String = (10,14)       */
        public static String DATATYPE_X_String = "(10,14)      ";
        /** Date = (15,16)       */
        public static String DATATYPE_X_Date = "(15,16)      ";
        /** Identifier = (19)          */
        public static String DATATYPE_X_Identifier = "(19)         ";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDataType_XValid(String test)
        {
            return test.Equals("(10,14)      ") || test.Equals("(15,16)      ") || test.Equals("(19)         ");
        }
        /** Set Data Type X.
        @param DataType_X Data Type X */
        public void SetDataType_X(String DataType_X)
        {
            if (DataType_X == null) throw new ArgumentException("DataType_X is mandatory");
            if (!IsDataType_XValid(DataType_X))
                throw new ArgumentException("DataType_X Invalid value - " + DataType_X + " - Reference_ID=1000001 - (10,14)       - (15,16)       - (19)         ");
            if (DataType_X.Length > 13)
            {
                log.Warning("Length > 13 - truncated");
                DataType_X = DataType_X.Substring(0, 13);
            }
            Set_Value("DataType_X", DataType_X);
        }
        /** Get Data Type X.
        @return Data Type X */
        public String GetDataType_X()
        {
            return (String)Get_Value("DataType_X");
        }

        /** DataType_Y AD_Reference_ID=1000001 */
        public static int DATATYPE_Y_AD_Reference_ID = 1000001;
        /** String = (10,14)       */
        public static String DATATYPE_Y_String = "(10,14)      ";
        /** Date = (15,16)       */
        public static String DATATYPE_Y_Date = "(15,16)      ";
        /** Identifier = (19)          */
        public static String DATATYPE_Y_Identifier = "(19)         ";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDataType_YValid(String test)
        {
            return test.Equals("(10,14)      ") || test.Equals("(15,16)      ") || test.Equals("(19)         ");
        }
        /** Set DataType_Y.
        @param DataType_Y DataType_Y */
        public void SetDataType_Y(String DataType_Y)
        {
            if (DataType_Y == null) throw new ArgumentException("DataType_Y is mandatory");
            if (!IsDataType_YValid(DataType_Y))
                throw new ArgumentException("DataType_Y Invalid value - " + DataType_Y + " - Reference_ID=1000001 - (10,14)       - (15,16)       - (19)         ");
            if (DataType_Y.Length > 13)
            {
                log.Warning("Length > 13 - truncated");
                DataType_Y = DataType_Y.Substring(0, 13);
            }
            Set_Value("DataType_Y", DataType_Y);
        }
        /** Get DataType_Y.
        @return DataType_Y */
        public String GetDataType_Y()
        {
            return (String)Get_Value("DataType_Y");
        }
        /** Set Date From.
        @param DateFrom Starting date for a range */
        public void SetDateFrom(DateTime? DateFrom)
        {
            Set_Value("DateFrom", (DateTime?)DateFrom);
        }
        /** Get Date From.
        @return Starting date for a range */
        public DateTime? GetDateFrom()
        {
            return (DateTime?)Get_Value("DateFrom");
        }

        /** DateTimeTypes AD_Reference_ID=1000002 */
        public static int DATETIMETYPES_AD_Reference_ID = 1000002;
        /** Last ? Days = A */
        public static String DATETIMETYPES_LastDays = "A";
        /** Last ? Months = B */
        public static String DATETIMETYPES_LastMonths = "B";
        /** Last ? Years = C */
        public static String DATETIMETYPES_LastYears = "C";
        /** Daily = D */
        public static String DATETIMETYPES_Daily = "D";
        /** Monthly = M */
        public static String DATETIMETYPES_Monthly = "M";
        /** None = N */
        public static String DATETIMETYPES_None = "N";
        /** Yearly = Y */
        public static String DATETIMETYPES_Yearly = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDateTimeTypesValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("M") || test.Equals("N") || test.Equals("Y");
        }
        /** Set Date Time Types.
        @param DateTimeTypes Date Time Types */
        public void SetDateTimeTypes(String DateTimeTypes)
        {
            if (!IsDateTimeTypesValid(DateTimeTypes))
                throw new ArgumentException("DateTimeTypes Invalid value - " + DateTimeTypes + " - Reference_ID=1000002 - A - B - C - D - M - N - Y");
            if (DateTimeTypes != null && DateTimeTypes.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DateTimeTypes = DateTimeTypes.Substring(0, 1);
            }
            Set_Value("DateTimeTypes", DateTimeTypes);
        }
        /** Get Date Time Types.
        @return Date Time Types */
        public String GetDateTimeTypes()
        {
            return (String)Get_Value("DateTimeTypes");
        }
        /** Set Date To.
        @param DateTo End date of a date range */
        public void SetDateTo(DateTime? DateTo)
        {
            Set_Value("DateTo", (DateTime?)DateTo);
        }
        /** Get Date To.
        @return End date of a date range */
        public DateTime? GetDateTo()
        {
            return (DateTime?)Get_Value("DateTo");
        }
        /** Set EndValue.
        @param EndValue EndValue */
        public void SetEndValue(int EndValue)
        {
            Set_Value("EndValue", EndValue);
        }
        /** Get EndValue.
        @return EndValue */
        public int GetEndValue()
        {
            Object ii = Get_Value("EndValue");
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
        /** Set Avg.
        @param IsAvg Avg */
        public void SetIsAvg(Boolean IsAvg)
        {
            Set_Value("IsAvg", IsAvg);
        }
        /** Get Avg.
        @return Avg */
        public Boolean IsAvg()
        {
            Object oo = Get_Value("IsAvg");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set IsDisplayedToUser.
        @param IsDisplayedToUser IsDisplayedToUser */
        public void SetIsDisplayedToUser(Boolean IsDisplayedToUser)
        {
            Set_Value("IsDisplayedToUser", IsDisplayedToUser);
        }
        /** Get IsDisplayedToUser.
        @return IsDisplayedToUser */
        public Boolean IsDisplayedToUser()
        {
            Object oo = Get_Value("IsDisplayedToUser");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Is Logarithmic.
        @param IsLogarithmic Is Logarithmic */
        public void SetIsLogarithmic(Boolean IsLogarithmic)
        {
            Set_Value("IsLogarithmic", IsLogarithmic);
        }
        /** Get Is Logarithmic.
        @return Is Logarithmic */
        public Boolean IsLogarithmic()
        {
            Object oo = Get_Value("IsLogarithmic");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set None.
        @param IsNone None */
        public void SetIsNone(Boolean IsNone)
        {
            Set_Value("IsNone", IsNone);
        }
        /** Get None.
        @return None */
        public Boolean IsNone()
        {
            Object oo = Get_Value("IsNone");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Order By Desc.
        @param IsOrderByAsc Order By Desc */
        public void SetIsOrderByAsc(Boolean IsOrderByAsc)
        {
            Set_Value("IsOrderByAsc", IsOrderByAsc);
        }
        /** Get Order By Desc.
        @return Order By Desc */
        public Boolean IsOrderByAsc()
        {
            Object oo = Get_Value("IsOrderByAsc");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Set Alert.
        @param IsSetAlert Set Alert */
        public void SetIsSetAlert(Boolean IsSetAlert)
        {
            Set_Value("IsSetAlert", IsSetAlert);
        }
        /** Get Set Alert.
        @return Set Alert */
        public Boolean IsSetAlert()
        {
            Object oo = Get_Value("IsSetAlert");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Show Label.
        @param IsShowLabel Show Label */
        public void SetIsShowLabel(Boolean IsShowLabel)
        {
            Set_Value("IsShowLabel", IsShowLabel);
        }
        /** Get Show Label.
        @return Show Label */
        public Boolean IsShowLabel()
        {
            Object oo = Get_Value("IsShowLabel");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Disable X Axis.
        @param IsShowXAsix Disable X Axis */
        public void SetIsShowXAsix(Boolean IsShowXAsix)
        {
            Set_Value("IsShowXAsix", IsShowXAsix);
        }
        /** Get Disable X Axis.
        @return Disable X Axis */
        public Boolean IsShowXAsix()
        {
            Object oo = Get_Value("IsShowXAsix");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Disable Y Axis.
        @param IsShowYAxis Disable Y Axis */
        public void SetIsShowYAxis(Boolean IsShowYAxis)
        {
            Set_Value("IsShowYAxis", IsShowYAxis);
        }
        /** Get Disable Y Axis.
        @return Disable Y Axis */
        public Boolean IsShowYAxis()
        {
            Object oo = Get_Value("IsShowYAxis");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set IsShown.
        @param IsShown IsShown */
        public void SetIsShown(Boolean IsShown)
        {
            Set_Value("IsShown", IsShown);
        }
        /** Get IsShown.
        @return IsShown */
        public Boolean IsShown()
        {
            Object oo = Get_Value("IsShown");
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
        /** Set Value.
        @param LastNValue Value */
        public void SetLastNValue(int LastNValue)
        {
            Set_Value("LastNValue", LastNValue);
        }
        /** Get Value.
        @return Value */
        public int GetLastNValue()
        {
            Object ii = Get_Value("LastNValue");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Logarithmic Base.
        @param LogarithmicBase Logarithmic Base */
        public void SetLogarithmicBase(int LogarithmicBase)
        {
            Set_Value("LogarithmicBase", LogarithmicBase);
        }
        /** Get Logarithmic Base.
        @return Logarithmic Base */
        public int GetLogarithmicBase()
        {
            Object ii = Get_Value("LogarithmicBase");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Month.
        @param MONTH Month */
        public void SetMONTH(int MONTH)
        {
            Set_Value("MONTH", MONTH);
        }
        /** Get Month.
        @return Month */
        public int GetMONTH()
        {
            Object ii = Get_Value("MONTH");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 100)
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

        /** OrderByColumn AD_Reference_ID=1000003 */
        public static int ORDERBYCOLUMN_AD_Reference_ID = 1000003;
        /** ColX = 1 */
        public static String ORDERBYCOLUMN_ColX = "1";
        /** ColY = 2 */
        public static String ORDERBYCOLUMN_ColY = "2";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsOrderByColumnValid(String test)
        {
            return test.Equals("1") || test.Equals("2");
        }
        /** Set OrderBy Column.
        @param OrderByColumn OrderBy Column */
        public void SetOrderByColumn(String OrderByColumn)
        {
            if (OrderByColumn == null) throw new ArgumentException("OrderByColumn is mandatory");
            if (!IsOrderByColumnValid(OrderByColumn))
                throw new ArgumentException("OrderByColumn Invalid value - " + OrderByColumn + " - Reference_ID=1000003 - 1 - 2");
            if (OrderByColumn.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                OrderByColumn = OrderByColumn.Substring(0, 1);
            }
            Set_Value("OrderByColumn", OrderByColumn);
        }
        /** Get OrderBy Column.
        @return OrderBy Column */
        public String GetOrderByColumn()
        {
            return (String)Get_Value("OrderByColumn");
        }

        /** ShowView AD_Reference_ID=1000004 */
        public static int SHOWVIEW_AD_Reference_ID = 1000004;
        ///** Set ShowView.
        //@param ShowView ShowView */
        //public void SetShowView(int ShowView)
        //{
        //    Set_Value("ShowView", ShowView);
        //}
        ///** Get ShowView.
        //@return ShowView */
        //public int GetShowView()
        //{
        //    Object ii = Get_Value("ShowView");
        //    if (ii == null) return 0;
        //    return Convert.ToInt32(ii);
        //}
        /** Set Sql Query.
        @param SqlQuery Sql Query */
        public void SetSqlQuery(String SqlQuery)
        {
            if (SqlQuery != null && SqlQuery.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                SqlQuery = SqlQuery.Substring(0, 2000);
            }
            Set_Value("SqlQuery", SqlQuery);
        }
        /** Get Sql Query.
        @return Sql Query */
        public String GetSqlQuery()
        {
            return (String)Get_Value("SqlQuery");
        }
        /** Set StartValue.
        @param StartValue StartValue */
        public void SetStartValue(int StartValue)
        {
            Set_Value("StartValue", StartValue);
        }
        /** Get StartValue.
        @return StartValue */
        public int GetStartValue()
        {
            Object ii = Get_Value("StartValue");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }





        /** TableView_ID AD_Reference_ID=1000186 */
        public static int TABLEVIEW_ID_AD_Reference_ID = 1000186;
        /** Set Table View.
        @param TableView_ID Table View */
        public void SetTableView_ID(int TableView_ID)
        {
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




        /** Set ValueTo.
        @param ValueTo ValueTo */
        public void SetValueTo(String ValueTo)
        {
            if (ValueTo != null && ValueTo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ValueTo = ValueTo.Substring(0, 50);
            }
            Set_Value("ValueTo", ValueTo);
        }
        /** Get ValueTo.
        @return ValueTo */
        public String GetValueTo()
        {
            return (String)Get_Value("ValueTo");
        }
        /** Set Sql WHERE.
        @param WhereClause Fully qualified SQL WHERE clause */
        public void SetWhereClause(String WhereClause)
        {
            if (WhereClause == null) throw new ArgumentException("WhereClause is mandatory.");
            if (WhereClause.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                WhereClause = WhereClause.Substring(0, 50);
            }
            Set_Value("WhereClause", WhereClause);
        }
        /** Get Sql WHERE.
        @return Fully qualified SQL WHERE clause */
        public String GetWhereClause()
        {
            return (String)Get_Value("WhereClause");
        }

        /** WhereCondition AD_Reference_ID=1000005 */
        public static int WHERECONDITION_AD_Reference_ID = 1000005;
        /** = = 1 */
        public static String WHERECONDITION_Eq = "1";
        /** != = 2 */
        public static String WHERECONDITION_NotEq = "2";
        /** >= = 3 */
        public static String WHERECONDITION_GtEq = "3";
        /** <= = 4 */
        public static String WHERECONDITION_LeEq = "4";
        /** >< = 5 */
        public static String WHERECONDITION_ = "5";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWhereConditionValid(String test)
        {
            return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5");
        }
        /** Set Where Condition.
        @param WhereCondition Where Condition */
        public void SetWhereCondition(String WhereCondition)
        {
            if (WhereCondition == null) throw new ArgumentException("WhereCondition is mandatory");
            if (!IsWhereConditionValid(WhereCondition))
                throw new ArgumentException("WhereCondition Invalid value - " + WhereCondition + " - Reference_ID=1000005 - 1 - 2 - 3 - 4 - 5");
            if (WhereCondition.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                WhereCondition = WhereCondition.Substring(0, 1);
            }
            Set_Value("WhereCondition", WhereCondition);
        }
        /** Get Where Condition.
        @return Where Condition */
        public String GetWhereCondition()
        {
            return (String)Get_Value("WhereCondition");
        }
        /** Set X Axis Label.
        @param XAxisLabel X Axis Label */
        public void SetXAxisLabel(String XAxisLabel)
        {
            if (XAxisLabel != null && XAxisLabel.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                XAxisLabel = XAxisLabel.Substring(0, 50);
            }
            Set_Value("XAxisLabel", XAxisLabel);
        }
        /** Get X Axis Label.
        @return X Axis Label */
        public String GetXAxisLabel()
        {
            return (String)Get_Value("XAxisLabel");
        }
        /** Set Y Axis Label.
        @param YAxisLabel Y Axis Label */
        public void SetYAxisLabel(String YAxisLabel)
        {
            if (YAxisLabel != null && YAxisLabel.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                YAxisLabel = YAxisLabel.Substring(0, 50);
            }
            Set_Value("YAxisLabel", YAxisLabel);
        }
        /** Get Y Axis Label.
        @return Y Axis Label */
        public String GetYAxisLabel()
        {
            return (String)Get_Value("YAxisLabel");
        }
        /** Set Year.
        @param YEAR Year */
        public void SetYEAR(int YEAR)
        {
            Set_Value("YEAR", YEAR);
        }
        /** Get Year.
        @return Year */
        public int GetYEAR()
        {
            Object ii = Get_Value("YEAR");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
