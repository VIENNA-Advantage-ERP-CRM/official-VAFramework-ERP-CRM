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
/** Generated Model for C_IncomeTax
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_IncomeTax : PO
{
public X_C_IncomeTax (Context ctx, int C_IncomeTax_ID, Trx trxName) : base (ctx, C_IncomeTax_ID, trxName)
{
/** if (C_IncomeTax_ID == 0)
{
SetC_IncomeTax_ID (0);
SetPosted (false);
}
 */
}
public X_C_IncomeTax (Ctx ctx, int C_IncomeTax_ID, Trx trxName) : base (ctx, C_IncomeTax_ID, trxName)
{
/** if (C_IncomeTax_ID == 0)
{
SetC_IncomeTax_ID (0);
SetPosted (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_IncomeTax (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_IncomeTax (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_IncomeTax (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_IncomeTax()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27681650386553L;
/** Last Updated Timestamp 5/8/2014 10:27:50 AM */
public static long updatedMS = 1399525069764L;
/** AD_Table_ID=1000441 */
public static int Table_ID;
 // =1000441;

/** TableName=C_IncomeTax */
public static String Table_Name="C_IncomeTax";

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
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_IncomeTax[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID <= 0) Set_Value ("C_DocType_ID", null);
else
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Income Tax.
@param C_IncomeTax_ID Income Tax */
public void SetC_IncomeTax_ID (int C_IncomeTax_ID)
{
if (C_IncomeTax_ID < 1) throw new ArgumentException ("C_IncomeTax_ID is mandatory.");
Set_ValueNoCheck ("C_IncomeTax_ID", C_IncomeTax_ID);
}
/** Get Income Tax.
@return Income Tax */
public int GetC_IncomeTax_ID() 
{
Object ii = Get_Value("C_IncomeTax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Profit Dimension.
@param C_ProfitAndLoss_ID Profit Dimension */
public void SetC_ProfitAndLoss_ID (int C_ProfitAndLoss_ID)
{
if (C_ProfitAndLoss_ID <= 0) Set_Value ("C_ProfitAndLoss_ID", null);
else
Set_Value ("C_ProfitAndLoss_ID", C_ProfitAndLoss_ID);
}
/** Get Profit Dimension.
@return Profit Dimension */
public int GetC_ProfitAndLoss_ID() 
{
Object ii = Get_Value("C_ProfitAndLoss_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Profit Loss.
@param C_ProfitLoss_ID Profit Loss */
public void SetC_ProfitLoss_ID (int C_ProfitLoss_ID)
{
if (C_ProfitLoss_ID <= 0) Set_Value ("C_ProfitLoss_ID", null);
else
Set_Value ("C_ProfitLoss_ID", C_ProfitLoss_ID);
}
/** Get Profit Loss.
@return Profit Loss */
public int GetC_ProfitLoss_ID() 
{
Object ii = Get_Value("C_ProfitLoss_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Income Tax Rate.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
if (C_Tax_ID <= 0) Set_Value ("C_Tax_ID", null);
else
Set_Value ("C_Tax_ID", C_Tax_ID);
}
/** Get Income Tax Rate.
@return Tax identifier */
public int GetC_Tax_ID() 
{
Object ii = Get_Value("C_Tax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Year.
@param C_Year_ID Calendar Year */
public void SetC_Year_ID (int C_Year_ID)
{
if (C_Year_ID <= 0) Set_Value ("C_Year_ID", null);
else
Set_Value ("C_Year_ID", C_Year_ID);
}
/** Get Year.
@return Calendar Year */
public int GetC_Year_ID() 
{
Object ii = Get_Value("C_Year_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Calculate Income Tax.
@param CalculateTax Calculate Income Tax */
public void SetCalculateTax (String CalculateTax)
{
if (CalculateTax != null && CalculateTax.Length > 10)
{
log.Warning("Length > 10 - truncated");
CalculateTax = CalculateTax.Substring(0,10);
}
Set_Value ("CalculateTax", CalculateTax);
}
/** Get Calculate Income Tax.
@return Calculate Income Tax */
public String GetCalculateTax() 
{
return (String)Get_Value("CalculateTax");
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
Set_Value ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
}
/** Set Transaction Date.
@param DateTrx Transaction Date */
public void SetDateTrx (DateTime? DateTrx)
{
Set_Value ("DateTrx", (DateTime?)DateTrx);
}
/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetDateTrx() 
{
return (DateTime?)Get_Value("DateTrx");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 1000)
{
log.Warning("Length > 1000 - truncated");
Description = Description.Substring(0,1000);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}

/** DocAction AD_Reference_ID=135 */
public static int DOCACTION_AD_Reference_ID=135;
/** <None> = -- */
public static String DOCACTION_None = "--";
/** Approve = AP */
public static String DOCACTION_Approve = "AP";
/** Close = CL */
public static String DOCACTION_Close = "CL";
/** Complete = CO */
public static String DOCACTION_Complete = "CO";
/** Invalidate = IN */
public static String DOCACTION_Invalidate = "IN";
/** Post = PO */
public static String DOCACTION_Post = "PO";
/** Prepare = PR */
public static String DOCACTION_Prepare = "PR";
/** Reverse - Accrual = RA */
public static String DOCACTION_Reverse_Accrual = "RA";
/** Reverse - Correct = RC */
public static String DOCACTION_Reverse_Correct = "RC";
/** Re-activate = RE */
public static String DOCACTION_Re_Activate = "RE";
/** Reject = RJ */
public static String DOCACTION_Reject = "RJ";
/** Void = VO */
public static String DOCACTION_Void = "VO";
/** Wait Complete = WC */
public static String DOCACTION_WaitComplete = "WC";
/** Unlock = XL */
public static String DOCACTION_Unlock = "XL";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocActionValid (String test)
{
return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
}
/** Set Document Action.
@param DocAction The targeted status of the document */
public void SetDocAction (String DocAction)
{
if (!IsDocActionValid(DocAction))
throw new ArgumentException ("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
if (DocAction != null && DocAction.Length > 2)
{
log.Warning("Length > 2 - truncated");
DocAction = DocAction.Substring(0,2);
}
Set_Value ("DocAction", DocAction);
}
/** Get Document Action.
@return The targeted status of the document */
public String GetDocAction() 
{
return (String)Get_Value("DocAction");
}

/** DocStatus AD_Reference_ID=131 */
public static int DOCSTATUS_AD_Reference_ID=131;
/** Unknown = ?? */
public static String DOCSTATUS_Unknown = "??";
/** Approved = AP */
public static String DOCSTATUS_Approved = "AP";
/** Closed = CL */
public static String DOCSTATUS_Closed = "CL";
/** Completed = CO */
public static String DOCSTATUS_Completed = "CO";
/** Drafted = DR */
public static String DOCSTATUS_Drafted = "DR";
/** Invalid = IN */
public static String DOCSTATUS_Invalid = "IN";
/** In Progress = IP */
public static String DOCSTATUS_InProgress = "IP";
/** Not Approved = NA */
public static String DOCSTATUS_NotApproved = "NA";
/** Reversed = RE */
public static String DOCSTATUS_Reversed = "RE";
/** Voided = VO */
public static String DOCSTATUS_Voided = "VO";
/** Waiting Confirmation = WC */
public static String DOCSTATUS_WaitingConfirmation = "WC";
/** Waiting Payment = WP */
public static String DOCSTATUS_WaitingPayment = "WP";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocStatusValid (String test)
{
return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
}
/** Set Document Status.
@param DocStatus The current status of the document */
public void SetDocStatus (String DocStatus)
{
if (!IsDocStatusValid(DocStatus))
throw new ArgumentException ("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
if (DocStatus != null && DocStatus.Length > 2)
{
log.Warning("Length > 2 - truncated");
DocStatus = DocStatus.Substring(0,2);
}
Set_Value ("DocStatus", DocStatus);
}
/** Get Document Status.
@return The current status of the document */
public String GetDocStatus() 
{
return (String)Get_Value("DocStatus");
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Income Tax Amount.
@param IncomeTaxAmount Income Tax Amount */
public void SetIncomeTaxAmount (Decimal? IncomeTaxAmount)
{
Set_Value ("IncomeTaxAmount", (Decimal?)IncomeTaxAmount);
}
/** Get Income Tax Amount.
@return Income Tax Amount */
public Decimal GetIncomeTaxAmount() 
{
Object bd =Get_Value("IncomeTaxAmount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Approved.
@param IsApproved Indicates if this document requires approval */
public void SetIsApproved (Boolean IsApproved)
{
Set_Value ("IsApproved", IsApproved);
}
/** Get Approved.
@return Indicates if this document requires approval */
public Boolean IsApproved() 
{
Object oo = Get_Value("IsApproved");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 100)
{
log.Warning("Length > 100 - truncated");
Name = Name.Substring(0,100);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Posted.
@param Posted Posting status */
public void SetPosted (Boolean Posted)
{
Set_Value ("Posted", Posted);
}
/** Get Posted.
@return Posting status */
public Boolean IsPosted() 
{
Object oo = Get_Value("Posted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Profit After Tax.
@param ProfitAfterTax Profit After Tax */
public void SetProfitAfterTax (Decimal? ProfitAfterTax)
{
Set_Value ("ProfitAfterTax", (Decimal?)ProfitAfterTax);
}
/** Get Profit After Tax.
@return Profit After Tax */
public Decimal GetProfitAfterTax() 
{
Object bd =Get_Value("ProfitAfterTax");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Profit Before Tax.
@param ProfitBeforeTax Profit Before Tax */
public void SetProfitBeforeTax (Decimal? ProfitBeforeTax)
{
Set_Value ("ProfitBeforeTax", (Decimal?)ProfitBeforeTax);
}
/** Get Profit Before Tax.
@return Profit Before Tax */
public Decimal GetProfitBeforeTax() 
{
Object bd =Get_Value("ProfitBeforeTax");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Transfer Profit To Retained Earning.
@param TransferProfit Transfer Profit To Retained Earning */
public void SetTransferProfit (String TransferProfit)
{
if (TransferProfit != null && TransferProfit.Length > 10)
{
log.Warning("Length > 10 - truncated");
TransferProfit = TransferProfit.Substring(0,10);
}
Set_Value ("TransferProfit", TransferProfit);
}
/** Get Transfer Profit To Retained Earning.
@return Transfer Profit To Retained Earning */
public String GetTransferProfit() 
{
return (String)Get_Value("TransferProfit");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 20)
{
log.Warning("Length > 20 - truncated");
Value = Value.Substring(0,20);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
