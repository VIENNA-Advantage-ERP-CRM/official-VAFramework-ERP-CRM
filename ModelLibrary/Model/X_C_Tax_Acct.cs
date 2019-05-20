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
/** Generated Model for C_Tax_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Tax_Acct : PO
{
public X_C_Tax_Acct (Context ctx, int C_Tax_Acct_ID, Trx trxName) : base (ctx, C_Tax_Acct_ID, trxName)
{
/** if (C_Tax_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Tax_ID (0);
SetT_Credit_Acct (0);
SetT_Due_Acct (0);
SetT_Expense_Acct (0);
SetT_Liability_Acct (0);
SetT_Receivables_Acct (0);
}
 */
}
public X_C_Tax_Acct (Ctx ctx, int C_Tax_Acct_ID, Trx trxName) : base (ctx, C_Tax_Acct_ID, trxName)
{
/** if (C_Tax_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Tax_ID (0);
SetT_Credit_Acct (0);
SetT_Due_Acct (0);
SetT_Expense_Acct (0);
SetT_Liability_Acct (0);
SetT_Receivables_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Tax_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Tax_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Tax_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Tax_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375745L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058956L;
/** AD_Table_ID=399 */
public static int Table_ID;
 // =399;

/** TableName=C_Tax_Acct */
public static String Table_Name="C_Tax_Acct";

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
StringBuilder sb = new StringBuilder ("X_C_Tax_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_AcctSchema_ID().ToString());
}
/** Set Tax.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
if (C_Tax_ID < 1) throw new ArgumentException ("C_Tax_ID is mandatory.");
Set_ValueNoCheck ("C_Tax_ID", C_Tax_ID);
}
/** Get Tax.
@return Tax identifier */
public int GetC_Tax_ID() 
{
Object ii = Get_Value("C_Tax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Credit.
@param T_Credit_Acct Account for Tax you can reclaim */
public void SetT_Credit_Acct (int T_Credit_Acct)
{
Set_Value ("T_Credit_Acct", T_Credit_Acct);
}
/** Get Tax Credit.
@return Account for Tax you can reclaim */
public int GetT_Credit_Acct() 
{
Object ii = Get_Value("T_Credit_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Due.
@param T_Due_Acct Account for Tax you have to pay */
public void SetT_Due_Acct (int T_Due_Acct)
{
Set_Value ("T_Due_Acct", T_Due_Acct);
}
/** Get Tax Due.
@return Account for Tax you have to pay */
public int GetT_Due_Acct() 
{
Object ii = Get_Value("T_Due_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Expense.
@param T_Expense_Acct Account for paid tax you cannot reclaim */
public void SetT_Expense_Acct (int T_Expense_Acct)
{
Set_Value ("T_Expense_Acct", T_Expense_Acct);
}
/** Get Tax Expense.
@return Account for paid tax you cannot reclaim */
public int GetT_Expense_Acct() 
{
Object ii = Get_Value("T_Expense_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Liability.
@param T_Liability_Acct Account for Tax declaration liability */
public void SetT_Liability_Acct (int T_Liability_Acct)
{
Set_Value ("T_Liability_Acct", T_Liability_Acct);
}
/** Get Tax Liability.
@return Account for Tax declaration liability */
public int GetT_Liability_Acct() 
{
Object ii = Get_Value("T_Liability_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Receivables.
@param T_Receivables_Acct Account for Tax credit after tax declaration */
public void SetT_Receivables_Acct (int T_Receivables_Acct)
{
Set_Value ("T_Receivables_Acct", T_Receivables_Acct);
}
/** Get Tax Receivables.
@return Account for Tax credit after tax declaration */
public int GetT_Receivables_Acct() 
{
Object ii = Get_Value("T_Receivables_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
