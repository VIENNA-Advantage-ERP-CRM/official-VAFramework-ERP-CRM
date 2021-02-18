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
    using System.Data;/** Generated Model for VAB_Rev_RecognitionRun
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_Rev_RecognitionRun : PO
    {
        public X_VAB_Rev_RecognitionRun(Context ctx, int VAB_Rev_RecognitionRun_ID, Trx trxName) : base(ctx, VAB_Rev_RecognitionRun_ID, trxName)
        {/** if (VAB_Rev_RecognitionRun_ID == 0){SetVAB_Rev_RecognitionStrtgy_ID (0);SetVAB_Rev_RecognitionRun_ID (0);SetVAGL_JRNL_ID (0);SetRecognizedAmt (0.0);} */
        }
        public X_VAB_Rev_RecognitionRun(Ctx ctx, int VAB_Rev_RecognitionRun_ID, Trx trxName) : base(ctx, VAB_Rev_RecognitionRun_ID, trxName)
        {/** if (VAB_Rev_RecognitionRun_ID == 0){SetVAB_Rev_RecognitionStrtgy_ID (0);SetVAB_Rev_RecognitionRun_ID (0);SetVAGL_JRNL_ID (0);SetRecognizedAmt (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_Rev_RecognitionRun(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_Rev_RecognitionRun(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_Rev_RecognitionRun(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAB_Rev_RecognitionRun() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27889642906645L;/** Last Updated Timestamp 12/9/2020 12:39:49 PM */
        public static long updatedMS = 1607517589856L;/** VAF_TableView_ID=444 */
        public static int Table_ID; // =444;
        /** TableName=VAB_Rev_RecognitionRun */
        public static String Table_Name = "VAB_Rev_RecognitionRun";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAB_Rev_RecognitionRun[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Revenue Recognition Plan.
@param VAB_Rev_RecognitionStrtgy_ID Plan for recognizing or recording revenue */
        public void SetVAB_Rev_RecognitionStrtgy_ID(int VAB_Rev_RecognitionStrtgy_ID) { if (VAB_Rev_RecognitionStrtgy_ID < 1) throw new ArgumentException("VAB_Rev_RecognitionStrtgy_ID is mandatory."); Set_ValueNoCheck("VAB_Rev_RecognitionStrtgy_ID", VAB_Rev_RecognitionStrtgy_ID); }/** Get Revenue Recognition Plan.
@return Plan for recognizing or recording revenue */
        public int GetVAB_Rev_RecognitionStrtgy_ID() { Object ii = Get_Value("VAB_Rev_RecognitionStrtgy_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetVAB_Rev_RecognitionStrtgy_ID().ToString()); }/** Set Revenue Recognition Run.
@param VAB_Rev_RecognitionRun_ID Revenue Recognition Run or Process */
        public void SetVAB_Rev_RecognitionRun_ID(int VAB_Rev_RecognitionRun_ID) { if (VAB_Rev_RecognitionRun_ID < 1) throw new ArgumentException("VAB_Rev_RecognitionRun_ID is mandatory."); Set_ValueNoCheck("VAB_Rev_RecognitionRun_ID", VAB_Rev_RecognitionRun_ID); }/** Get Revenue Recognition Run.
@return Revenue Recognition Run or Process */
        public int GetVAB_Rev_RecognitionRun_ID() { Object ii = Get_Value("VAB_Rev_RecognitionRun_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Journal.
@param VAGL_JRNL_ID General Ledger Journal */
        public void SetVAGL_JRNL_ID(int VAGL_JRNL_ID) { if (VAGL_JRNL_ID < 1) throw new ArgumentException("VAGL_JRNL_ID is mandatory."); Set_ValueNoCheck("VAGL_JRNL_ID", VAGL_JRNL_ID); }/** Get Journal.
@return General Ledger Journal */
        public int GetVAGL_JRNL_ID() { Object ii = Get_Value("VAGL_JRNL_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Recognition Date.
@param RecognitionDate Recognition Date */
        public void SetRecognitionDate(DateTime? RecognitionDate) { Set_Value("RecognitionDate", (DateTime?)RecognitionDate); }/** Get Recognition Date.
@return Recognition Date */
        public DateTime? GetRecognitionDate() { return (DateTime?)Get_Value("RecognitionDate"); }/** Set Recognized Amount.
@param RecognizedAmt Recognized Amount */
        public void SetRecognizedAmt(Decimal? RecognizedAmt) { if (RecognizedAmt == null) throw new ArgumentException("RecognizedAmt is mandatory."); Set_ValueNoCheck("RecognizedAmt", (Decimal?)RecognizedAmt); }/** Get Recognized Amount.
@return Recognized Amount */
        public Decimal GetRecognizedAmt() { Object bd = Get_Value("RecognizedAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}