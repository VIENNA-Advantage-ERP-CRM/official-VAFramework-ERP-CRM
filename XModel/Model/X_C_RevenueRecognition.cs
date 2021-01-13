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
    using System.Data;/** Generated Model for C_RevenueRecognition
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_RevenueRecognition : PO
    {
        public X_C_RevenueRecognition(Context ctx, int C_RevenueRecognition_ID, Trx trxName) : base(ctx, C_RevenueRecognition_ID, trxName)
        {/** if (C_RevenueRecognition_ID == 0){SetC_RevenueRecognition_ID (0);SetIsTimeBased (false);SetName (null);} */
        }
        public X_C_RevenueRecognition(Ctx ctx, int C_RevenueRecognition_ID, Trx trxName) : base(ctx, C_RevenueRecognition_ID, trxName)
        {/** if (C_RevenueRecognition_ID == 0){SetC_RevenueRecognition_ID (0);SetIsTimeBased (false);SetName (null);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_RevenueRecognition(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_RevenueRecognition(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_RevenueRecognition(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_RevenueRecognition() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27889639009480L;/** Last Updated Timestamp 12/9/2020 11:34:52 AM */
        public static long updatedMS = 1607513692691L;/** AD_Table_ID=336 */
        public static int Table_ID; // =336;
        /** TableName=C_RevenueRecognition */
        public static String Table_Name = "C_RevenueRecognition";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_RevenueRecognition[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Revenue Recognition.
@param C_RevenueRecognition_ID Method for recording revenue */
        public void SetC_RevenueRecognition_ID(int C_RevenueRecognition_ID) { if (C_RevenueRecognition_ID < 1) throw new ArgumentException("C_RevenueRecognition_ID is mandatory."); Set_ValueNoCheck("C_RevenueRecognition_ID", C_RevenueRecognition_ID); }/** Get Revenue Recognition.
@return Method for recording revenue */
        public int GetC_RevenueRecognition_ID() { Object ii = Get_Value("C_RevenueRecognition_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Time based.
@param IsTimeBased Time based Revenue Recognition rather than Service Level based */
        public void SetIsTimeBased(Boolean IsTimeBased) { Set_Value("IsTimeBased", IsTimeBased); }/** Get Time based.
@return Time based Revenue Recognition rather than Service Level based */
        public Boolean IsTimeBased() { Object oo = Get_Value("IsTimeBased"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name == null) throw new ArgumentException("Name is mandatory."); if (Name.Length > 60) { log.Warning("Length > 60 - truncated"); Name = Name.Substring(0, 60); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetName()); }/** Set Number of Months.
@param NoMonths Number of Months */
        public void SetNoMonths(int NoMonths) { Set_Value("NoMonths", NoMonths); }/** Get Number of Months.
@return Number of Months */
        public int GetNoMonths() { Object ii = Get_Value("NoMonths"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** RecognitionFrequency AD_Reference_ID=196 */
        public static int RECOGNITIONFREQUENCY_AD_Reference_ID = 196;/** Day = D */
        public static String RECOGNITIONFREQUENCY_Day = "D";/** Month = M */
        public static String RECOGNITIONFREQUENCY_Month = "M";/** Quarter = Q */
        public static String RECOGNITIONFREQUENCY_Quarter = "Q";/** Year = Y */
        public static String RECOGNITIONFREQUENCY_Year = "Y";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsRecognitionFrequencyValid(String test) { return test == null || test.Equals("D") || test.Equals("M") || test.Equals("Q") || test.Equals("Y"); }/** Set Recognition frequency.
@param RecognitionFrequency   */
        public void SetRecognitionFrequency(String RecognitionFrequency)
        {
            if (!IsRecognitionFrequencyValid(RecognitionFrequency))
                throw new ArgumentException("RecognitionFrequency Invalid value - " + RecognitionFrequency + " - Reference_ID=196 - D - M - Q - Y"); if (RecognitionFrequency != null && RecognitionFrequency.Length > 1) { log.Warning("Length > 1 - truncated"); RecognitionFrequency = RecognitionFrequency.Substring(0, 1); }
            Set_Value("RecognitionFrequency", RecognitionFrequency);
        }/** Get Recognition frequency.
@return   */
        public String GetRecognitionFrequency() { return (String)Get_Value("RecognitionFrequency"); }
        /** RecognitionType AD_Reference_ID=1000235 */
        public static int RECOGNITIONTYPE_AD_Reference_ID = 1000235;/** Expense = E */
        public static String RECOGNITIONTYPE_Expense = "E";/** Revenue = R */
        public static String RECOGNITIONTYPE_Revenue = "R";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsRecognitionTypeValid(String test) { return test == null || test.Equals("E") || test.Equals("R"); }/** Set Recognition Type.
@param RecognitionType Recognition Type */
        public void SetRecognitionType(String RecognitionType)
        {
            if (!IsRecognitionTypeValid(RecognitionType))
                throw new ArgumentException("RecognitionType Invalid value - " + RecognitionType + " - Reference_ID=1000235 - E - R"); if (RecognitionType != null && RecognitionType.Length > 1) { log.Warning("Length > 1 - truncated"); RecognitionType = RecognitionType.Substring(0, 1); }
            Set_Value("RecognitionType", RecognitionType);
        }/** Get Recognition Type.
@return Recognition Type */
        public String GetRecognitionType() { return (String)Get_Value("RecognitionType"); }
    }
}