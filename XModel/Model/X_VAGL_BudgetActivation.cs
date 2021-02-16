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
/** Generated Model for VAGL_BudgetActivation
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAGL_BudgetActivation : PO
{
public X_VAGL_BudgetActivation (Context ctx, int VAGL_BudgetActivation_ID, Trx trxName) : base (ctx, VAGL_BudgetActivation_ID, trxName)
{
/** if (VAGL_BudgetActivation_ID == 0)
{
SetBudgetControlScope (null);
SetVAB_AccountBook_ID (0);
SetCommitmentType (null);	// C
SetVAGL_BudgetActivation_ID (0);
SetVAGL_Budget_ID (0);
SetIsBeforeApproval (false);
SetName (null);
}
 */
}
public X_VAGL_BudgetActivation (Ctx ctx, int VAGL_BudgetActivation_ID, Trx trxName) : base (ctx, VAGL_BudgetActivation_ID, trxName)
{
/** if (VAGL_BudgetActivation_ID == 0)
{
SetBudgetControlScope (null);
SetVAB_AccountBook_ID (0);
SetCommitmentType (null);	// C
SetVAGL_BudgetActivation_ID (0);
SetVAGL_Budget_ID (0);
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
public X_VAGL_BudgetActivation (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_BudgetActivation (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_BudgetActivation (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAGL_BudgetActivation()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376278L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059489L;
/** VAF_TableView_ID=822 */
public static int Table_ID;
 // =822;

/** TableName=VAGL_BudgetActivation */
public static String Table_Name="VAGL_BudgetActivation";

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
StringBuilder sb = new StringBuilder ("X_VAGL_BudgetActivation[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** BudgetControlScope VAF_Control_Ref_ID=361 */
public static int BUDGETCONTROLSCOPE_VAF_Control_Ref_ID=361;
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
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_Value ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** CommitmentType VAF_Control_Ref_ID=359 */
public static int COMMITMENTTYPE_VAF_Control_Ref_ID=359;
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
@param VAGL_BudgetActivation_ID Budget Control */
public void SetVAGL_BudgetActivation_ID (int VAGL_BudgetActivation_ID)
{
if (VAGL_BudgetActivation_ID < 1) throw new ArgumentException ("VAGL_BudgetActivation_ID is mandatory.");
Set_ValueNoCheck ("VAGL_BudgetActivation_ID", VAGL_BudgetActivation_ID);
}
/** Get Budget Control.
@return Budget Control */
public int GetVAGL_BudgetActivation_ID() 
{
Object ii = Get_Value("VAGL_BudgetActivation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Budget.
@param VAGL_Budget_ID General Ledger Budget */
public void SetVAGL_Budget_ID (int VAGL_Budget_ID)
{
if (VAGL_Budget_ID < 1) throw new ArgumentException ("VAGL_Budget_ID is mandatory.");
Set_Value ("VAGL_Budget_ID", VAGL_Budget_ID);
}
/** Get Budget.
@return General Ledger Budget */
public int GetVAGL_Budget_ID() 
{
Object ii = Get_Value("VAGL_Budget_ID");
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