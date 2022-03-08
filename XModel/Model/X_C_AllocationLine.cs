namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_AllocationLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AllocationLine : PO{public X_C_AllocationLine (Context ctx, int C_AllocationLine_ID, Trx trxName) : base (ctx, C_AllocationLine_ID, trxName){/** if (C_AllocationLine_ID == 0){SetAmount (0.0);SetC_AllocationHdr_ID (0);SetC_AllocationLine_ID (0);SetDiscountAmt (0.0);SetWriteOffAmt (0.0);} */
}public X_C_AllocationLine (Ctx ctx, int C_AllocationLine_ID, Trx trxName) : base (ctx, C_AllocationLine_ID, trxName){/** if (C_AllocationLine_ID == 0){SetAmount (0.0);SetC_AllocationHdr_ID (0);SetC_AllocationLine_ID (0);SetDiscountAmt (0.0);SetWriteOffAmt (0.0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AllocationLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27884379430766L;/** Last Updated Timestamp 10/9/2020 2:35:14 PM */
public static long updatedMS = 1602254113977L;/** AD_Table_ID=390 */
public static int Table_ID; // =390;
/** TableName=C_AllocationLine */
public static String Table_Name="C_AllocationLine";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_AllocationLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Amount.
@param Amount Amount in a defined currency */
public void SetAmount (Decimal? Amount){if (Amount == null) throw new ArgumentException ("Amount is mandatory.");Set_ValueNoCheck ("Amount", (Decimal?)Amount);}/** Get Amount.
@return Amount in a defined currency */
public Decimal GetAmount() {Object bd =Get_Value("Amount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Backup Withholding Amount.
@param BackupWithholdingAmount Backup Withholding Amount */
public void SetBackupWithholdingAmount (Decimal? BackupWithholdingAmount){Set_Value ("BackupWithholdingAmount", (Decimal?)BackupWithholdingAmount);}/** Get Backup Withholding Amount.
@return Backup Withholding Amount */
public Decimal GetBackupWithholdingAmount() {Object bd =Get_Value("BackupWithholdingAmount");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Allocation.
@param C_AllocationHdr_ID Payment allocation */
public void SetC_AllocationHdr_ID (int C_AllocationHdr_ID){if (C_AllocationHdr_ID < 1) throw new ArgumentException ("C_AllocationHdr_ID is mandatory.");Set_ValueNoCheck ("C_AllocationHdr_ID", C_AllocationHdr_ID);}/** Get Allocation.
@return Payment allocation */
public int GetC_AllocationHdr_ID() {Object ii = Get_Value("C_AllocationHdr_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Allocation Line.
@param C_AllocationLine_ID Allocation Line */
public void SetC_AllocationLine_ID (int C_AllocationLine_ID){if (C_AllocationLine_ID < 1) throw new ArgumentException ("C_AllocationLine_ID is mandatory.");Set_ValueNoCheck ("C_AllocationLine_ID", C_AllocationLine_ID);}/** Get Allocation Line.
@return Allocation Line */
public int GetC_AllocationLine_ID() {Object ii = Get_Value("C_AllocationLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
public void SetC_BPartner_ID (int C_BPartner_ID){if (C_BPartner_ID <= 0) Set_ValueNoCheck ("C_BPartner_ID", null);else
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);}/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetC_BPartner_ID() {Object ii = Get_Value("C_BPartner_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cash Book.
@param C_CashBook_ID Cash Book for recording petty cash transactions */
public void SetC_CashBook_ID (int C_CashBook_ID){throw new ArgumentException ("C_CashBook_ID Is virtual column");}/** Get Cash Book.
@return Cash Book for recording petty cash transactions */
public int GetC_CashBook_ID() {Object ii = Get_Value("C_CashBook_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cash Journal Line.
@param C_CashLine_ID Cash Journal Line */
public void SetC_CashLine_ID (int C_CashLine_ID){if (C_CashLine_ID <= 0) Set_ValueNoCheck ("C_CashLine_ID", null);else
Set_ValueNoCheck ("C_CashLine_ID", C_CashLine_ID);}/** Get Cash Journal Line.
@return Cash Journal Line */
public int GetC_CashLine_ID() {Object ii = Get_Value("C_CashLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
public void SetC_InvoicePaySchedule_ID (int C_InvoicePaySchedule_ID){if (C_InvoicePaySchedule_ID <= 0) Set_Value ("C_InvoicePaySchedule_ID", null);else
Set_Value ("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);}/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
public int GetC_InvoicePaySchedule_ID() {Object ii = Get_Value("C_InvoicePaySchedule_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID){if (C_Invoice_ID <= 0) Set_ValueNoCheck ("C_Invoice_ID", null);else
Set_ValueNoCheck ("C_Invoice_ID", C_Invoice_ID);}/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() {Object ii = Get_Value("C_Invoice_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetC_Invoice_ID().ToString());}/** Set Order.
@param C_Order_ID Sales Order */
public void SetC_Order_ID (int C_Order_ID){if (C_Order_ID <= 0) Set_ValueNoCheck ("C_Order_ID", null);else
Set_ValueNoCheck ("C_Order_ID", C_Order_ID);}/** Get Order.
@return Sales Order */
public int GetC_Order_ID() {Object ii = Get_Value("C_Order_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID){if (C_Payment_ID <= 0) Set_ValueNoCheck ("C_Payment_ID", null);else
Set_ValueNoCheck ("C_Payment_ID", C_Payment_ID);}/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() {Object ii = Get_Value("C_Payment_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Transaction Date.
@param DateTrx Transaction Date */
public void SetDateTrx (DateTime? DateTrx){Set_ValueNoCheck ("DateTrx", (DateTime?)DateTrx);}/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetDateTrx() {return (DateTime?)Get_Value("DateTrx");}/** Set Discount Amount.
@param DiscountAmt Calculated amount of discount */
public void SetDiscountAmt (Decimal? DiscountAmt){if (DiscountAmt == null) throw new ArgumentException ("DiscountAmt is mandatory.");Set_ValueNoCheck ("DiscountAmt", (Decimal?)DiscountAmt);}/** Get Discount Amount.
@return Calculated amount of discount */
public Decimal GetDiscountAmt() {Object bd =Get_Value("DiscountAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Total Amount.
@param FRPT_TotalAmt Total Amount */
public void SetFRPT_TotalAmt (Decimal? FRPT_TotalAmt){throw new ArgumentException ("FRPT_TotalAmt Is virtual column");}/** Get Total Amount.
@return Total Amount */
public Decimal GetFRPT_TotalAmt() {Object bd =Get_Value("FRPT_TotalAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Journal Line.
@param GL_JournalLine_ID General Ledger Journal Line */
public void SetGL_JournalLine_ID (int GL_JournalLine_ID){if (GL_JournalLine_ID <= 0) Set_Value ("GL_JournalLine_ID", null);else
Set_Value ("GL_JournalLine_ID", GL_JournalLine_ID);}/** Get Journal Line.
@return General Ledger Journal Line */
public int GetGL_JournalLine_ID() {Object ii = Get_Value("GL_JournalLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Generate Allocation.
@param GenerateAllocation Generate Allocation */
public void SetGenerateAllocation (String GenerateAllocation){if (GenerateAllocation != null && GenerateAllocation.Length > 10){log.Warning("Length > 10 - truncated");GenerateAllocation = GenerateAllocation.Substring(0,10);}Set_Value ("GenerateAllocation", GenerateAllocation);}/** Get Generate Allocation.
@return Generate Allocation */
public String GetGenerateAllocation() {return (String)Get_Value("GenerateAllocation");}/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual){Set_ValueNoCheck ("IsManual", IsManual);}/** Get Manual.
@return This is a manual process */
public Boolean IsManual() {Object oo = Get_Value("IsManual");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set M_CostAllocation_ID.
@param M_CostAllocation_ID M_CostAllocation_ID */
public void SetM_CostAllocation_ID (int M_CostAllocation_ID){if (M_CostAllocation_ID <= 0) Set_Value ("M_CostAllocation_ID", null);else
Set_Value ("M_CostAllocation_ID", M_CostAllocation_ID);}/** Get M_CostAllocation_ID.
@return M_CostAllocation_ID */
public int GetM_CostAllocation_ID() {Object ii = Get_Value("M_CostAllocation_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Over/Under Payment.
@param OverUnderAmt Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
public void SetOverUnderAmt (Decimal? OverUnderAmt){Set_Value ("OverUnderAmt", (Decimal?)OverUnderAmt);}/** Get Over/Under Payment.
@return Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
public Decimal GetOverUnderAmt() {Object bd =Get_Value("OverUnderAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}
/** Ref_C_Invoice_ID AD_Reference_ID=336 */
public static int REF_C_INVOICE_ID_AD_Reference_ID=336;/** Set Invoice Ref.
@param Ref_C_Invoice_ID Invoice Ref */
public void SetRef_C_Invoice_ID (int Ref_C_Invoice_ID){if (Ref_C_Invoice_ID <= 0) Set_Value ("Ref_C_Invoice_ID", null);else
Set_Value ("Ref_C_Invoice_ID", Ref_C_Invoice_ID);}/** Get Invoice Ref.
@return Invoice Ref */
public int GetRef_C_Invoice_ID() {Object ii = Get_Value("Ref_C_Invoice_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Ref_CashLine_ID AD_Reference_ID=1000602 */
public static int REF_CASHLINE_ID_AD_Reference_ID=1000602;/** Set Ref Cash Journal.
@param Ref_CashLine_ID Ref Cash Journal */
public void SetRef_CashLine_ID (int Ref_CashLine_ID){if (Ref_CashLine_ID <= 0) Set_Value ("Ref_CashLine_ID", null);else
Set_Value ("Ref_CashLine_ID", Ref_CashLine_ID);}/** Get Ref Cash Journal.
@return Ref Cash Journal */
public int GetRef_CashLine_ID() {Object ii = Get_Value("Ref_CashLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Ref_GLJournalLine_ID AD_Reference_ID=1000314 */
public static int REF_GLJOURNALLINE_ID_AD_Reference_ID=1000314;/** Set Ref GL Journal Line.
@param Ref_GLJournalLine_ID General Ledger Journal Line */
public void SetRef_GLJournalLine_ID (int Ref_GLJournalLine_ID){if (Ref_GLJournalLine_ID <= 0) Set_Value ("Ref_GLJournalLine_ID", null);else
Set_Value ("Ref_GLJournalLine_ID", Ref_GLJournalLine_ID);}/** Get Ref GL Journal Line.
@return General Ledger Journal Line */
public int GetRef_GLJournalLine_ID() {Object ii = Get_Value("Ref_GLJournalLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Ref_Invoiceschedule_ID AD_Reference_ID=1000603 */
public static int REF_INVOICESCHEDULE_ID_AD_Reference_ID=1000603;/** Set Ref Invoice Schedule.
@param Ref_Invoiceschedule_ID Ref Invoice Schedule */
public void SetRef_Invoiceschedule_ID (int Ref_Invoiceschedule_ID){if (Ref_Invoiceschedule_ID <= 0) Set_Value ("Ref_Invoiceschedule_ID", null);else
Set_Value ("Ref_Invoiceschedule_ID", Ref_Invoiceschedule_ID);}/** Get Ref Invoice Schedule.
@return Ref Invoice Schedule */
public int GetRef_Invoiceschedule_ID() {Object ii = Get_Value("Ref_Invoiceschedule_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** Ref_Payment_ID AD_Reference_ID=343 */
public static int REF_PAYMENT_ID_AD_Reference_ID=343;/** Set Referenced Payment.
@param Ref_Payment_ID Referenced Payment */
public void SetRef_Payment_ID (int Ref_Payment_ID){if (Ref_Payment_ID <= 0) Set_Value ("Ref_Payment_ID", null);else
Set_Value ("Ref_Payment_ID", Ref_Payment_ID);}/** Get Referenced Payment.
@return Referenced Payment */
public int GetRef_Payment_ID() {Object ii = Get_Value("Ref_Payment_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Withholding Amount.
@param WithholdingAmt This field represents the calculated withholding amount */
public void SetWithholdingAmt (Decimal? WithholdingAmt){Set_Value ("WithholdingAmt", (Decimal?)WithholdingAmt);}/** Get Withholding Amount.
@return This field represents the calculated withholding amount */
public Decimal GetWithholdingAmt() {Object bd =Get_Value("WithholdingAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Write-off Amount.
@param WriteOffAmt Amount to write-off */
public void SetWriteOffAmt (Decimal? WriteOffAmt){if (WriteOffAmt == null) throw new ArgumentException ("WriteOffAmt is mandatory.");Set_Value ("WriteOffAmt", (Decimal?)WriteOffAmt);}/** Get Write-off Amount.
@return Amount to write-off */
public Decimal GetWriteOffAmt() {Object bd =Get_Value("WriteOffAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}