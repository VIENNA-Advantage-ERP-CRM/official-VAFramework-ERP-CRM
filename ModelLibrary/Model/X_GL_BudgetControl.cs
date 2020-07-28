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
/** Generated Model for GL_BudgetControl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_BudgetControl : PO
{
public X_GL_BudgetControl (Context ctx, int GL_BudgetControl_ID, Trx trxName) : base (ctx, GL_BudgetControl_ID, trxName)
{
/** if (GL_BudgetControl_ID == 0)
{
SetBudgetControlScope (null);
SetC_AcctSchema_ID (0);
SetCommitmentType (null);	// C
SetGL_BudgetControl_ID (0);
SetGL_Budget_ID (0);
SetIsBeforeApproval (false);
SetName (null);
}
 */
}
public X_GL_BudgetControl (Ctx ctx, int GL_BudgetControl_ID, Trx trxName) : base (ctx, GL_BudgetControl_ID, trxName)
{
/** if (GL_BudgetControl_ID == 0)
{
SetBudgetControlScope (null);
SetC_AcctSchema_ID (0);
SetCommitmentType (null);	// C
SetGL_BudgetControl_ID (0);
SetGL_Budget_ID (0);
SetIsBeforeApproval (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_BudgetControl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_BudgetControl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_BudgetControl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_BudgetControl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376278L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059489L;
/** AD_Table_ID=822 */
public static int Table_ID;
 // =822;

/** TableName=GL_BudgetControl */
public static String Table_Name="GL_BudgetControl";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_GL_BudgetControl[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** BudgetControlScope AD_Reference_ID=361 */
public static int BUDGETCONTROLSCOPE_AD_Reference_ID=361;
/** Period only = P */
public static String BUDGETCONTROLSCOPE_PeriodOnly = "P";
/** Total = T */
public static String BUDGETCONTROLSCOPE_Total = "T";
/** Year To Date = Y */
public static String BUDGETCONTROLSCOPE_YearToDate = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBudgetControlScopeValid (String test)
{
return test.Equals("P") || test.Equals("T") || test.Equals("Y");
}
/** Set Control Scope.
@param BudgetControlScope Scope of the Budget Control */
public void SetBudgetControlScope (String BudgetControlScope)
{
if (BudgetControlScope == null) throw new ArgumentException ("BudgetControlScope is mandatory");
if (!IsBudgetControlScopeValid(BudgetControlScope))
throw new ArgumentException ("BudgetControlScope Invalid value - " + BudgetControlScope + " - Reference_ID=361 - P - T - Y");
if (BudgetControlScope.Length > 1)
{
log.Warning("Length > 1 - truncated");
BudgetControlScope = BudgetControlScope.Substring(0,1);
}
Set_Value ("BudgetControlScope", BudgetControlScope);
}
/** Get Control Scope.
@return Scope of the Budget Control */
public String GetBudgetControlScope() 
{
return (String)Get_Value("BudgetControlScope");
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_Value ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** CommitmentType AD_Reference_ID=359 */
public static int COMMITMENTTYPE_AD_Reference_ID=359;
/** Commitment & Reservation = B */
public static String COMMITMENTTYPE_CommitmentReservation = "B";
/** Commitment only = C */
public static String COMMITMENTTYPE_CommitmentOnly = "C";
/** None = N */
public static String COMMITMENTTYPE_None = "N";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCommitmentTypeValid (String test)
{
return test.Equals("B") || test.Equals("C") || test.Equals("N");
}
/** Set Commitment Type.
@param CommitmentType Create Commitment and/or Reservations for Budget Control */
public void SetCommitmentType (String CommitmentType)
{
if (CommitmentType == null) throw new ArgumentException ("CommitmentType is mandatory");
if (!IsCommitmentTypeValid(CommitmentType))
throw new ArgumentException ("CommitmentType Invalid value - " + CommitmentType + " - Reference_ID=359 - B - C - N");
if (CommitmentType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CommitmentType = CommitmentType.Substring(0,1);
}
Set_Value ("CommitmentType", CommitmentType);
}
/** Get Commitment Type.
@return Create Commitment and/or Reservations for Budget Control */
public String GetCommitmentType() 
{
return (String)Get_Value("CommitmentType");
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
/** Set Budget Control.
@param GL_BudgetControl_ID Budget Control */
public void SetGL_BudgetControl_ID (int GL_BudgetControl_ID)
{
if (GL_BudgetControl_ID < 1) throw new ArgumentException ("GL_BudgetControl_ID is mandatory.");
Set_ValueNoCheck ("GL_BudgetControl_ID", GL_BudgetControl_ID);
}
/** Get Budget Control.
@return Budget Control */
public int GetGL_BudgetControl_ID() 
{
Object ii = Get_Value("GL_BudgetControl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Budget.
@param GL_Budget_ID General Ledger Budget */
public void SetGL_Budget_ID (int GL_Budget_ID)
{
if (GL_Budget_ID < 1) throw new ArgumentException ("GL_Budget_ID is mandatory.");
Set_Value ("GL_Budget_ID", GL_Budget_ID);
}
/** Get Budget.
@return General Ledger Budget */
public int GetGL_Budget_ID() 
{
Object ii = Get_Value("GL_Budget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Before Approval.
@param IsBeforeApproval The Check is before the (manual) approval */
public void SetIsBeforeApproval (Boolean IsBeforeApproval)
{
Set_Value ("IsBeforeApproval", IsBeforeApproval);
}
/** Get Before Approval.
@return The Check is before the (manual) approval */
public Boolean IsBeforeApproval() 
{
Object oo = Get_Value("IsBeforeApproval");
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
}

}
