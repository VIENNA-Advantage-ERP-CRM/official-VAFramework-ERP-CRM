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
/** Generated Model for C_BankStatement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BankStatement : PO
{
public X_C_BankStatement (Context ctx, int C_BankStatement_ID, Trx trxName) : base (ctx, C_BankStatement_ID, trxName)
{
/** if (C_BankStatement_ID == 0)
{
SetC_BankAccount_ID (0);
SetC_BankStatement_ID (0);
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetEndingBalance (0.0);
SetIsApproved (false);	// N
SetIsManual (true);	// Y
SetName (null);	// @#Date@
SetPosted (false);	// N
SetProcessed (false);	// N
SetStatementDate (DateTime.Now);	// @Date@
}
 */
}
public X_C_BankStatement (Ctx ctx, int C_BankStatement_ID, Trx trxName) : base (ctx, C_BankStatement_ID, trxName)
{
/** if (C_BankStatement_ID == 0)
{
SetC_BankAccount_ID (0);
SetC_BankStatement_ID (0);
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetEndingBalance (0.0);
SetIsApproved (false);	// N
SetIsManual (true);	// Y
SetName (null);	// @#Date@
SetPosted (false);	// N
SetProcessed (false);	// N
SetStatementDate (DateTime.Now);	// @Date@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BankStatement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370839L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054050L;
/** AD_Table_ID=392 */
public static int Table_ID;
 // =392;

/** TableName=C_BankStatement */
public static String Table_Name="C_BankStatement";

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
StringBuilder sb = new StringBuilder ("X_C_BankStatement[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Beginning Balance.
@param BeginningBalance Balance prior to any transactions */
public void SetBeginningBalance (Decimal? BeginningBalance)
{
Set_Value ("BeginningBalance", (Decimal?)BeginningBalance);
}
/** Get Beginning Balance.
@return Balance prior to any transactions */
public Decimal GetBeginningBalance() 
{
Object bd =Get_Value("BeginningBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
public void SetC_BankAccount_ID (int C_BankAccount_ID)
{
if (C_BankAccount_ID < 1) throw new ArgumentException ("C_BankAccount_ID is mandatory.");
Set_Value ("C_BankAccount_ID", C_BankAccount_ID);
}
/** Get Bank Account.
@return Account at the Bank */
public int GetC_BankAccount_ID() 
{
Object ii = Get_Value("C_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Statement.
@param C_BankStatement_ID Bank Statement of account */
public void SetC_BankStatement_ID (int C_BankStatement_ID)
{
if (C_BankStatement_ID < 1) throw new ArgumentException ("C_BankStatement_ID is mandatory.");
Set_ValueNoCheck ("C_BankStatement_ID", C_BankStatement_ID);
}
/** Get Bank Statement.
@return Bank Statement of account */
public int GetC_BankStatement_ID() 
{
Object ii = Get_Value("C_BankStatement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Create lines from.
@param CreateFrom Process which will generate a new document lines based on an existing document */
public void SetCreateFrom (String CreateFrom)
{
if (CreateFrom != null && CreateFrom.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateFrom = CreateFrom.Substring(0,1);
}
Set_Value ("CreateFrom", CreateFrom);
}
/** Get Create lines from.
@return Process which will generate a new document lines based on an existing document */
public String GetCreateFrom() 
{
return (String)Get_Value("CreateFrom");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
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
return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
}
/** Set Document Action.
@param DocAction The targeted status of the document */
public void SetDocAction (String DocAction)
{
if (DocAction == null) throw new ArgumentException ("DocAction is mandatory");
if (!IsDocActionValid(DocAction))
throw new ArgumentException ("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
if (DocAction.Length > 2)
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
return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
}
/** Set Document Status.
@param DocStatus The current status of the document */
public void SetDocStatus (String DocStatus)
{
if (DocStatus == null) throw new ArgumentException ("DocStatus is mandatory");
if (!IsDocStatusValid(DocStatus))
throw new ArgumentException ("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
if (DocStatus.Length > 2)
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
/** Set EFT Statement Date.
@param EftStatementDate Electronic Funds Transfer Statement Date */
public void SetEftStatementDate (DateTime? EftStatementDate)
{
Set_Value ("EftStatementDate", (DateTime?)EftStatementDate);
}
/** Get EFT Statement Date.
@return Electronic Funds Transfer Statement Date */
public DateTime? GetEftStatementDate() 
{
return (DateTime?)Get_Value("EftStatementDate");
}
/** Set EFT Statement Reference.
@param EftStatementReference Electronic Funds Transfer Statement Reference */
public void SetEftStatementReference (String EftStatementReference)
{
if (EftStatementReference != null && EftStatementReference.Length > 60)
{
log.Warning("Length > 60 - truncated");
EftStatementReference = EftStatementReference.Substring(0,60);
}
Set_Value ("EftStatementReference", EftStatementReference);
}
/** Get EFT Statement Reference.
@return Electronic Funds Transfer Statement Reference */
public String GetEftStatementReference() 
{
return (String)Get_Value("EftStatementReference");
}
/** Set Ending balance.
@param EndingBalance Ending  or closing balance */
public void SetEndingBalance (Decimal? EndingBalance)
{
if (EndingBalance == null) throw new ArgumentException ("EndingBalance is mandatory.");
Set_Value ("EndingBalance", (Decimal?)EndingBalance);
}
/** Get Ending balance.
@return Ending  or closing balance */
public Decimal GetEndingBalance() 
{
Object bd =Get_Value("EndingBalance");
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
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_Value ("IsManual", IsManual);
}
/** Get Manual.
@return This is a manual process */
public Boolean IsManual() 
{
Object oo = Get_Value("IsManual");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Match Statement.
@param MatchStatement Match Statement */
public void SetMatchStatement (String MatchStatement)
{
if (MatchStatement != null && MatchStatement.Length > 1)
{
log.Warning("Length > 1 - truncated");
MatchStatement = MatchStatement.Substring(0,1);
}
Set_Value ("MatchStatement", MatchStatement);
}
/** Get Match Statement.
@return Match Statement */
public String GetMatchStatement() 
{
return (String)Get_Value("MatchStatement");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
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
/** Set Statement difference.
@param StatementDifference Difference between statement ending balance and actual ending balance */
public void SetStatementDifference (Decimal? StatementDifference)
{
Set_Value ("StatementDifference", (Decimal?)StatementDifference);
}
/** Get Statement difference.
@return Difference between statement ending balance and actual ending balance */
public Decimal GetStatementDifference() 
{
Object bd =Get_Value("StatementDifference");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
