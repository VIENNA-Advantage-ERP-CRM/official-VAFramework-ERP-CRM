namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_DunningRunLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_DunningRunLine : PO{public X_C_DunningRunLine (Context ctx, int C_DunningRunLine_ID, Trx trxName) : base (ctx, C_DunningRunLine_ID, trxName){/** if (C_DunningRunLine_ID == 0){SetAmt (0.0);SetC_DunningRunEntry_ID (0);SetC_DunningRunLine_ID (0);SetConvertedAmt (0.0);SetDaysDue (0);SetFeeAmt (0.0);SetInterestAmt (0.0);SetIsInDispute (false);SetOpenAmt (0.0);SetProcessed (false);// N
SetTimesDunned (0);SetTotalAmt (0.0);} */
}public X_C_DunningRunLine (Ctx ctx, int C_DunningRunLine_ID, Trx trxName) : base (ctx, C_DunningRunLine_ID, trxName){/** if (C_DunningRunLine_ID == 0){SetAmt (0.0);SetC_DunningRunEntry_ID (0);SetC_DunningRunLine_ID (0);SetConvertedAmt (0.0);SetDaysDue (0);SetFeeAmt (0.0);SetInterestAmt (0.0);SetIsInDispute (false);SetOpenAmt (0.0);SetProcessed (false);// N
SetTimesDunned (0);SetTotalAmt (0.0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningRunLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningRunLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningRunLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_DunningRunLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27837534353617L;/** Last Updated Timestamp 4/16/2019 3:33:56 PM */
public static long updatedMS = 1555409036828L;/** AD_Table_ID=524 */
public static int Table_ID; // =524;
/** TableName=C_DunningRunLine */
public static String Table_Name="C_DunningRunLine";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_DunningRunLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Amount.
@param Amt Amount */
public void SetAmt (Decimal? Amt){if (Amt == null) throw new ArgumentException ("Amt is mandatory.");Set_Value ("Amt", (Decimal?)Amt);}/** Get Amount.
@return Amount */
public Decimal GetAmt() {Object bd =Get_Value("Amt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Dunning Run Entry.
@param C_DunningRunEntry_ID Dunning Run Entry */
public void SetC_DunningRunEntry_ID (int C_DunningRunEntry_ID){if (C_DunningRunEntry_ID < 1) throw new ArgumentException ("C_DunningRunEntry_ID is mandatory.");Set_ValueNoCheck ("C_DunningRunEntry_ID", C_DunningRunEntry_ID);}/** Get Dunning Run Entry.
@return Dunning Run Entry */
public int GetC_DunningRunEntry_ID() {Object ii = Get_Value("C_DunningRunEntry_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Dunning Run Line.
@param C_DunningRunLine_ID Dunning Run Line */
public void SetC_DunningRunLine_ID (int C_DunningRunLine_ID){if (C_DunningRunLine_ID < 1) throw new ArgumentException ("C_DunningRunLine_ID is mandatory.");Set_ValueNoCheck ("C_DunningRunLine_ID", C_DunningRunLine_ID);}/** Get Dunning Run Line.
@return Dunning Run Line */
public int GetC_DunningRunLine_ID() {Object ii = Get_Value("C_DunningRunLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
public void SetC_InvoicePaySchedule_ID (int C_InvoicePaySchedule_ID){if (C_InvoicePaySchedule_ID <= 0) Set_Value ("C_InvoicePaySchedule_ID", null);else
Set_Value ("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);}/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
public int GetC_InvoicePaySchedule_ID() {Object ii = Get_Value("C_InvoicePaySchedule_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID){if (C_Invoice_ID <= 0) Set_Value ("C_Invoice_ID", null);else
Set_Value ("C_Invoice_ID", C_Invoice_ID);}/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() {Object ii = Get_Value("C_Invoice_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetC_Invoice_ID().ToString());}/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID){if (C_Payment_ID <= 0) Set_Value ("C_Payment_ID", null);else
Set_Value ("C_Payment_ID", C_Payment_ID);}/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() {Object ii = Get_Value("C_Payment_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Converted Amount.
@param ConvertedAmt Converted Amount */
public void SetConvertedAmt (Decimal? ConvertedAmt){if (ConvertedAmt == null) throw new ArgumentException ("ConvertedAmt is mandatory.");Set_Value ("ConvertedAmt", (Decimal?)ConvertedAmt);}/** Get Converted Amount.
@return Converted Amount */
public Decimal GetConvertedAmt() {Object bd =Get_Value("ConvertedAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Days due.
@param DaysDue Number of days due (negative: due in number of days) */
public void SetDaysDue (int DaysDue){Set_Value ("DaysDue", DaysDue);}/** Get Days due.
@return Number of days due (negative: due in number of days) */
public int GetDaysDue() {Object ii = Get_Value("DaysDue");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Fee Amount.
@param FeeAmt Fee amount in invoice currency */
public void SetFeeAmt (Decimal? FeeAmt){if (FeeAmt == null) throw new ArgumentException ("FeeAmt is mandatory.");Set_Value ("FeeAmt", (Decimal?)FeeAmt);}/** Get Fee Amount.
@return Fee amount in invoice currency */
public Decimal GetFeeAmt() {Object bd =Get_Value("FeeAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Interest Amount.
@param InterestAmt Interest Amount */
public void SetInterestAmt (Decimal? InterestAmt){if (InterestAmt == null) throw new ArgumentException ("InterestAmt is mandatory.");Set_Value ("InterestAmt", (Decimal?)InterestAmt);}/** Get Interest Amount.
@return Interest Amount */
public Decimal GetInterestAmt() {Object bd =Get_Value("InterestAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set In Dispute.
@param IsInDispute Document is in dispute */
public void SetIsInDispute (Boolean IsInDispute){Set_Value ("IsInDispute", IsInDispute);}/** Get In Dispute.
@return Document is in dispute */
public Boolean IsInDispute() {Object oo = Get_Value("IsInDispute");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Open Amount.
@param OpenAmt Open item amount */
public void SetOpenAmt (Decimal? OpenAmt){if (OpenAmt == null) throw new ArgumentException ("OpenAmt is mandatory.");Set_Value ("OpenAmt", (Decimal?)OpenAmt);}/** Get Open Amount.
@return Open item amount */
public Decimal GetOpenAmt() {Object bd =Get_Value("OpenAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed){Set_Value ("Processed", Processed);}/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() {Object oo = Get_Value("Processed");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Times Dunned.
@param TimesDunned Number of times dunned previously */
public void SetTimesDunned (int TimesDunned){Set_Value ("TimesDunned", TimesDunned);}/** Get Times Dunned.
@return Number of times dunned previously */
public int GetTimesDunned() {Object ii = Get_Value("TimesDunned");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Total Amount.
@param TotalAmt Total Amount */
public void SetTotalAmt (Decimal? TotalAmt){if (TotalAmt == null) throw new ArgumentException ("TotalAmt is mandatory.");Set_Value ("TotalAmt", (Decimal?)TotalAmt);}/** Get Total Amount.
@return Total Amount */
public Decimal GetTotalAmt() {Object bd =Get_Value("TotalAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}