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
/** Generated Model for VAB_Charge_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Charge_Acct : PO
{
public X_VAB_Charge_Acct (Context ctx, int VAB_Charge_Acct_ID, Trx trxName) : base (ctx, VAB_Charge_Acct_ID, trxName)
{
/** if (VAB_Charge_Acct_ID == 0)
{
SetVAB_AccountBook_ID (0);
SetVAB_Charge_ID (0);
SetCh_Expense_Acct (0);
SetCh_Revenue_Acct (0);
}
 */
}
public X_VAB_Charge_Acct (Ctx ctx, int VAB_Charge_Acct_ID, Trx trxName) : base (ctx, VAB_Charge_Acct_ID, trxName)
{
/** if (VAB_Charge_Acct_ID == 0)
{
SetVAB_AccountBook_ID (0);
SetVAB_Charge_ID (0);
SetCh_Expense_Acct (0);
SetCh_Revenue_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Charge_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Charge_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Charge_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Charge_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371216L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054427L;
/** VAF_TableView_ID=396 */
public static int Table_ID;
 // =396;

/** TableName=VAB_Charge_Acct */
public static String Table_Name="VAB_Charge_Acct";

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
StringBuilder sb = new StringBuilder ("X_VAB_Charge_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_AccountBook_ID().ToString());
}
/** Set Charge.
@param VAB_Charge_ID Additional document charges */
public void SetVAB_Charge_ID (int VAB_Charge_ID)
{
if (VAB_Charge_ID < 1) throw new ArgumentException ("VAB_Charge_ID is mandatory.");
Set_ValueNoCheck ("VAB_Charge_ID", VAB_Charge_ID);
}
/** Get Charge.
@return Additional document charges */
public int GetVAB_Charge_ID() 
{
Object ii = Get_Value("VAB_Charge_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge Expense.
@param Ch_Expense_Acct Charge Expense Account */
public void SetCh_Expense_Acct (int Ch_Expense_Acct)
{
Set_Value ("Ch_Expense_Acct", Ch_Expense_Acct);
}
/** Get Charge Expense.
@return Charge Expense Account */
public int GetCh_Expense_Acct() 
{
Object ii = Get_Value("Ch_Expense_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge Revenue.
@param Ch_Revenue_Acct Charge Revenue Account */
public void SetCh_Revenue_Acct (int Ch_Revenue_Acct)
{
Set_Value ("Ch_Revenue_Acct", Ch_Revenue_Acct);
}
/** Get Charge Revenue.
@return Charge Revenue Account */
public int GetCh_Revenue_Acct() 
{
Object ii = Get_Value("Ch_Revenue_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
