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
/** Generated Model for T_Aging
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_Aging : PO
{
public X_T_Aging (Context ctx, int T_Aging_ID, Trx trxName) : base (ctx, T_Aging_ID, trxName)
{
/** if (T_Aging_ID == 0)
{
SetAD_PInstance_ID (0);
SetC_BP_Group_ID (0);
SetC_BPartner_ID (0);
SetC_Currency_ID (0);
SetC_InvoicePaySchedule_ID (0);
SetC_Invoice_ID (0);
SetDue0 (0.0);
SetDue0_30 (0.0);
SetDue0_7 (0.0);
SetDue1_7 (0.0);
SetDue31_60 (0.0);
SetDue31_Plus (0.0);
SetDue61_90 (0.0);
SetDue61_Plus (0.0);
SetDue8_30 (0.0);
SetDue91_Plus (0.0);
SetDueAmt (0.0);
SetDueDate (DateTime.Now);
SetInvoicedAmt (0.0);
SetIsListInvoices (false);
SetIsSOTrx (false);
SetOpenAmt (0.0);
SetPastDue1_30 (0.0);
SetPastDue1_7 (0.0);
SetPastDue31_60 (0.0);
SetPastDue31_Plus (0.0);
SetPastDue61_90 (0.0);
SetPastDue61_Plus (0.0);
SetPastDue8_30 (0.0);
SetPastDue91_Plus (0.0);
SetPastDueAmt (0.0);
SetStatementDate (DateTime.Now);
}
 */
}
public X_T_Aging (Ctx ctx, int T_Aging_ID, Trx trxName) : base (ctx, T_Aging_ID, trxName)
{
/** if (T_Aging_ID == 0)
{
SetAD_PInstance_ID (0);
SetC_BP_Group_ID (0);
SetC_BPartner_ID (0);
SetC_Currency_ID (0);
SetC_InvoicePaySchedule_ID (0);
SetC_Invoice_ID (0);
SetDue0 (0.0);
SetDue0_30 (0.0);
SetDue0_7 (0.0);
SetDue1_7 (0.0);
SetDue31_60 (0.0);
SetDue31_Plus (0.0);
SetDue61_90 (0.0);
SetDue61_Plus (0.0);
SetDue8_30 (0.0);
SetDue91_Plus (0.0);
SetDueAmt (0.0);
SetDueDate (DateTime.Now);
SetInvoicedAmt (0.0);
SetIsListInvoices (false);
SetIsSOTrx (false);
SetOpenAmt (0.0);
SetPastDue1_30 (0.0);
SetPastDue1_7 (0.0);
SetPastDue31_60 (0.0);
SetPastDue31_Plus (0.0);
SetPastDue61_90 (0.0);
SetPastDue61_Plus (0.0);
SetPastDue8_30 (0.0);
SetPastDue91_Plus (0.0);
SetPastDueAmt (0.0);
SetStatementDate (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Aging (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Aging (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Aging (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_Aging()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384130L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067341L;
/** AD_Table_ID=631 */
public static int Table_ID;
 // =631;

/** TableName=T_Aging */
public static String Table_Name="T_Aging";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_T_Aging[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param AD_PInstance_ID Instance of the process */
public void SetAD_PInstance_ID (int AD_PInstance_ID)
{
if (AD_PInstance_ID < 1) throw new ArgumentException ("AD_PInstance_ID is mandatory.");
Set_ValueNoCheck ("AD_PInstance_ID", AD_PInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetAD_PInstance_ID() 
{
Object ii = Get_Value("AD_PInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_PInstance_ID().ToString());
}
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);
else
Set_Value ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner Group.
@param C_BP_Group_ID Business Partner Group */
public void SetC_BP_Group_ID (int C_BP_Group_ID)
{
if (C_BP_Group_ID < 1) throw new ArgumentException ("C_BP_Group_ID is mandatory.");
Set_Value ("C_BP_Group_ID", C_BP_Group_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetC_BP_Group_ID() 
{
Object ii = Get_Value("C_BP_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);
else
Set_Value ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
Set_ValueNoCheck ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
public void SetC_InvoicePaySchedule_ID (int C_InvoicePaySchedule_ID)
{
if (C_InvoicePaySchedule_ID < 1) throw new ArgumentException ("C_InvoicePaySchedule_ID is mandatory.");
Set_ValueNoCheck ("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
}
/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
public int GetC_InvoicePaySchedule_ID() 
{
Object ii = Get_Value("C_InvoicePaySchedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID < 1) throw new ArgumentException ("C_Invoice_ID is mandatory.");
Set_ValueNoCheck ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Days due.
@param DaysDue Number of days due (negative: due in number of days) */
public void SetDaysDue (int DaysDue)
{
Set_Value ("DaysDue", DaysDue);
}
/** Get Days due.
@return Number of days due (negative: due in number of days) */
public int GetDaysDue() 
{
Object ii = Get_Value("DaysDue");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Due Today.
@param Due0 Due Today */
public void SetDue0 (Decimal? Due0)
{
if (Due0 == null) throw new ArgumentException ("Due0 is mandatory.");
Set_Value ("Due0", (Decimal?)Due0);
}
/** Get Due Today.
@return Due Today */
public Decimal GetDue0() 
{
Object bd =Get_Value("Due0");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due Today-30.
@param Due0_30 Due Today-30 */
public void SetDue0_30 (Decimal? Due0_30)
{
if (Due0_30 == null) throw new ArgumentException ("Due0_30 is mandatory.");
Set_Value ("Due0_30", (Decimal?)Due0_30);
}
/** Get Due Today-30.
@return Due Today-30 */
public Decimal GetDue0_30() 
{
Object bd =Get_Value("Due0_30");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due Today-7.
@param Due0_7 Due Today-7 */
public void SetDue0_7 (Decimal? Due0_7)
{
if (Due0_7 == null) throw new ArgumentException ("Due0_7 is mandatory.");
Set_Value ("Due0_7", (Decimal?)Due0_7);
}
/** Get Due Today-7.
@return Due Today-7 */
public Decimal GetDue0_7() 
{
Object bd =Get_Value("Due0_7");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due 1-7.
@param Due1_7 Due 1-7 */
public void SetDue1_7 (Decimal? Due1_7)
{
if (Due1_7 == null) throw new ArgumentException ("Due1_7 is mandatory.");
Set_Value ("Due1_7", (Decimal?)Due1_7);
}
/** Get Due 1-7.
@return Due 1-7 */
public Decimal GetDue1_7() 
{
Object bd =Get_Value("Due1_7");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due 31-60.
@param Due31_60 Due 31-60 */
public void SetDue31_60 (Decimal? Due31_60)
{
if (Due31_60 == null) throw new ArgumentException ("Due31_60 is mandatory.");
Set_Value ("Due31_60", (Decimal?)Due31_60);
}
/** Get Due 31-60.
@return Due 31-60 */
public Decimal GetDue31_60() 
{
Object bd =Get_Value("Due31_60");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due > 31.
@param Due31_Plus Due > 31 */
public void SetDue31_Plus (Decimal? Due31_Plus)
{
if (Due31_Plus == null) throw new ArgumentException ("Due31_Plus is mandatory.");
Set_Value ("Due31_Plus", (Decimal?)Due31_Plus);
}
/** Get Due > 31.
@return Due > 31 */
public Decimal GetDue31_Plus() 
{
Object bd =Get_Value("Due31_Plus");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due 61-90.
@param Due61_90 Due 61-90 */
public void SetDue61_90 (Decimal? Due61_90)
{
if (Due61_90 == null) throw new ArgumentException ("Due61_90 is mandatory.");
Set_Value ("Due61_90", (Decimal?)Due61_90);
}
/** Get Due 61-90.
@return Due 61-90 */
public Decimal GetDue61_90() 
{
Object bd =Get_Value("Due61_90");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due > 61.
@param Due61_Plus Due > 61 */
public void SetDue61_Plus (Decimal? Due61_Plus)
{
if (Due61_Plus == null) throw new ArgumentException ("Due61_Plus is mandatory.");
Set_Value ("Due61_Plus", (Decimal?)Due61_Plus);
}
/** Get Due > 61.
@return Due > 61 */
public Decimal GetDue61_Plus() 
{
Object bd =Get_Value("Due61_Plus");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due 8-30.
@param Due8_30 Due 8-30 */
public void SetDue8_30 (Decimal? Due8_30)
{
if (Due8_30 == null) throw new ArgumentException ("Due8_30 is mandatory.");
Set_Value ("Due8_30", (Decimal?)Due8_30);
}
/** Get Due 8-30.
@return Due 8-30 */
public Decimal GetDue8_30() 
{
Object bd =Get_Value("Due8_30");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due > 91.
@param Due91_Plus Due > 91 */
public void SetDue91_Plus (Decimal? Due91_Plus)
{
if (Due91_Plus == null) throw new ArgumentException ("Due91_Plus is mandatory.");
Set_Value ("Due91_Plus", (Decimal?)Due91_Plus);
}
/** Get Due > 91.
@return Due > 91 */
public Decimal GetDue91_Plus() 
{
Object bd =Get_Value("Due91_Plus");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Amount due.
@param DueAmt Amount of the payment due */
public void SetDueAmt (Decimal? DueAmt)
{
if (DueAmt == null) throw new ArgumentException ("DueAmt is mandatory.");
Set_Value ("DueAmt", (Decimal?)DueAmt);
}
/** Get Amount due.
@return Amount of the payment due */
public Decimal GetDueAmt() 
{
Object bd =Get_Value("DueAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due Date.
@param DueDate Date when the payment is due */
public void SetDueDate (DateTime? DueDate)
{
if (DueDate == null) throw new ArgumentException ("DueDate is mandatory.");
Set_Value ("DueDate", (DateTime?)DueDate);
}
/** Get Due Date.
@return Date when the payment is due */
public DateTime? GetDueDate() 
{
return (DateTime?)Get_Value("DueDate");
}
/** Set Invoiced Amount.
@param InvoicedAmt The amount invoiced */
public void SetInvoicedAmt (Decimal? InvoicedAmt)
{
if (InvoicedAmt == null) throw new ArgumentException ("InvoicedAmt is mandatory.");
Set_Value ("InvoicedAmt", (Decimal?)InvoicedAmt);
}
/** Get Invoiced Amount.
@return The amount invoiced */
public Decimal GetInvoicedAmt() 
{
Object bd =Get_Value("InvoicedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List Invoices.
@param IsListInvoices Include List of Invoices */
public void SetIsListInvoices (Boolean IsListInvoices)
{
Set_Value ("IsListInvoices", IsListInvoices);
}
/** Get List Invoices.
@return Include List of Invoices */
public Boolean IsListInvoices() 
{
Object oo = Get_Value("IsListInvoices");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
public void SetIsSOTrx (Boolean IsSOTrx)
{
Set_Value ("IsSOTrx", IsSOTrx);
}
/** Get Sales Transaction.
@return This is a Sales Transaction */
public Boolean IsSOTrx() 
{
Object oo = Get_Value("IsSOTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Open Amount.
@param OpenAmt Open item amount */
public void SetOpenAmt (Decimal? OpenAmt)
{
if (OpenAmt == null) throw new ArgumentException ("OpenAmt is mandatory.");
Set_Value ("OpenAmt", (Decimal?)OpenAmt);
}
/** Get Open Amount.
@return Open item amount */
public Decimal GetOpenAmt() 
{
Object bd =Get_Value("OpenAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due 1-30.
@param PastDue1_30 Past Due 1-30 */
public void SetPastDue1_30 (Decimal? PastDue1_30)
{
if (PastDue1_30 == null) throw new ArgumentException ("PastDue1_30 is mandatory.");
Set_Value ("PastDue1_30", (Decimal?)PastDue1_30);
}
/** Get Past Due 1-30.
@return Past Due 1-30 */
public Decimal GetPastDue1_30() 
{
Object bd =Get_Value("PastDue1_30");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due 1-7.
@param PastDue1_7 Past Due 1-7 */
public void SetPastDue1_7 (Decimal? PastDue1_7)
{
if (PastDue1_7 == null) throw new ArgumentException ("PastDue1_7 is mandatory.");
Set_Value ("PastDue1_7", (Decimal?)PastDue1_7);
}
/** Get Past Due 1-7.
@return Past Due 1-7 */
public Decimal GetPastDue1_7() 
{
Object bd =Get_Value("PastDue1_7");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due 31-60.
@param PastDue31_60 Past Due 31-60 */
public void SetPastDue31_60 (Decimal? PastDue31_60)
{
if (PastDue31_60 == null) throw new ArgumentException ("PastDue31_60 is mandatory.");
Set_Value ("PastDue31_60", (Decimal?)PastDue31_60);
}
/** Get Past Due 31-60.
@return Past Due 31-60 */
public Decimal GetPastDue31_60() 
{
Object bd =Get_Value("PastDue31_60");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due > 31.
@param PastDue31_Plus Past Due > 31 */
public void SetPastDue31_Plus (Decimal? PastDue31_Plus)
{
if (PastDue31_Plus == null) throw new ArgumentException ("PastDue31_Plus is mandatory.");
Set_Value ("PastDue31_Plus", (Decimal?)PastDue31_Plus);
}
/** Get Past Due > 31.
@return Past Due > 31 */
public Decimal GetPastDue31_Plus() 
{
Object bd =Get_Value("PastDue31_Plus");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due 61-90.
@param PastDue61_90 Past Due 61-90 */
public void SetPastDue61_90 (Decimal? PastDue61_90)
{
if (PastDue61_90 == null) throw new ArgumentException ("PastDue61_90 is mandatory.");
Set_Value ("PastDue61_90", (Decimal?)PastDue61_90);
}
/** Get Past Due 61-90.
@return Past Due 61-90 */
public Decimal GetPastDue61_90() 
{
Object bd =Get_Value("PastDue61_90");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due > 61.
@param PastDue61_Plus Past Due > 61 */
public void SetPastDue61_Plus (Decimal? PastDue61_Plus)
{
if (PastDue61_Plus == null) throw new ArgumentException ("PastDue61_Plus is mandatory.");
Set_Value ("PastDue61_Plus", (Decimal?)PastDue61_Plus);
}
/** Get Past Due > 61.
@return Past Due > 61 */
public Decimal GetPastDue61_Plus() 
{
Object bd =Get_Value("PastDue61_Plus");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due 8-30.
@param PastDue8_30 Past Due 8-30 */
public void SetPastDue8_30 (Decimal? PastDue8_30)
{
if (PastDue8_30 == null) throw new ArgumentException ("PastDue8_30 is mandatory.");
Set_Value ("PastDue8_30", (Decimal?)PastDue8_30);
}
/** Get Past Due 8-30.
@return Past Due 8-30 */
public Decimal GetPastDue8_30() 
{
Object bd =Get_Value("PastDue8_30");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due > 91.
@param PastDue91_Plus Past Due > 91 */
public void SetPastDue91_Plus (Decimal? PastDue91_Plus)
{
if (PastDue91_Plus == null) throw new ArgumentException ("PastDue91_Plus is mandatory.");
Set_Value ("PastDue91_Plus", (Decimal?)PastDue91_Plus);
}
/** Get Past Due > 91.
@return Past Due > 91 */
public Decimal GetPastDue91_Plus() 
{
Object bd =Get_Value("PastDue91_Plus");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Past Due.
@param PastDueAmt Past Due */
public void SetPastDueAmt (Decimal? PastDueAmt)
{
if (PastDueAmt == null) throw new ArgumentException ("PastDueAmt is mandatory.");
Set_Value ("PastDueAmt", (Decimal?)PastDueAmt);
}
/** Get Past Due.
@return Past Due */
public Decimal GetPastDueAmt() 
{
Object bd =Get_Value("PastDueAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Statement date.
@param StatementDate Date of the statement */
public void SetStatementDate (DateTime? StatementDate)
{
if (StatementDate == null) throw new ArgumentException ("StatementDate is mandatory.");
Set_Value ("StatementDate", (DateTime?)StatementDate);
}
/** Get Statement date.
@return Date of the statement */
public DateTime? GetStatementDate() 
{
return (DateTime?)Get_Value("StatementDate");
}
}

}
