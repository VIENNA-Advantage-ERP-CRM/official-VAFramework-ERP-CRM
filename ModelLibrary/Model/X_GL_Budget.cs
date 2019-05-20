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
/** Generated Model for GL_Budget
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_Budget : PO
{
public X_GL_Budget (Context ctx, int GL_Budget_ID, Trx trxName) : base (ctx, GL_Budget_ID, trxName)
{
/** if (GL_Budget_ID == 0)
{
SetGL_Budget_ID (0);
SetIsPrimary (false);
SetName (null);
}
 */
}
public X_GL_Budget (Ctx ctx, int GL_Budget_ID, Trx trxName) : base (ctx, GL_Budget_ID, trxName)
{
/** if (GL_Budget_ID == 0)
{
SetGL_Budget_ID (0);
SetIsPrimary (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_Budget (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_Budget (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_Budget (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_Budget()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376262L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059473L;
/** AD_Table_ID=271 */
public static int Table_ID;
 // =271;

/** TableName=GL_Budget */
public static String Table_Name="GL_Budget";

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
StringBuilder sb = new StringBuilder ("X_GL_Budget[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** BudgetStatus AD_Reference_ID=178 */
public static int BUDGETSTATUS_AD_Reference_ID=178;
/** Approved = A */
public static String BUDGETSTATUS_Approved = "A";
/** Draft = D */
public static String BUDGETSTATUS_Draft = "D";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBudgetStatusValid (String test)
{
return test == null || test.Equals("A") || test.Equals("D");
}
/** Set Budget Status.
@param BudgetStatus Indicates the current status of this budget */
public void SetBudgetStatus (String BudgetStatus)
{
if (!IsBudgetStatusValid(BudgetStatus))
throw new ArgumentException ("BudgetStatus Invalid value - " + BudgetStatus + " - Reference_ID=178 - A - D");
if (BudgetStatus != null && BudgetStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
BudgetStatus = BudgetStatus.Substring(0,1);
}
Set_Value ("BudgetStatus", BudgetStatus);
}
/** Get Budget Status.
@return Indicates the current status of this budget */
public String GetBudgetStatus() 
{
return (String)Get_Value("BudgetStatus");
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
/** Set Budget.
@param GL_Budget_ID General Ledger Budget */
public void SetGL_Budget_ID (int GL_Budget_ID)
{
if (GL_Budget_ID < 1) throw new ArgumentException ("GL_Budget_ID is mandatory.");
Set_ValueNoCheck ("GL_Budget_ID", GL_Budget_ID);
}
/** Get Budget.
@return General Ledger Budget */
public int GetGL_Budget_ID() 
{
Object ii = Get_Value("GL_Budget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Primary.
@param IsPrimary Indicates if this is the primary budget */
public void SetIsPrimary (Boolean IsPrimary)
{
Set_Value ("IsPrimary", IsPrimary);
}
/** Get Primary.
@return Indicates if this is the primary budget */
public Boolean IsPrimary() 
{
Object oo = Get_Value("IsPrimary");
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
