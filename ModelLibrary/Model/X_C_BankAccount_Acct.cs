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
/** Generated Model for C_BankAccount_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BankAccount_Acct : PO
{
public X_C_BankAccount_Acct (Context ctx, int C_BankAccount_Acct_ID, Trx trxName) : base (ctx, C_BankAccount_Acct_ID, trxName)
{
/** if (C_BankAccount_Acct_ID == 0)
{
SetB_Asset_Acct (0);
SetB_Expense_Acct (0);
SetB_InTransit_Acct (0);
SetB_InterestExp_Acct (0);
SetB_InterestRev_Acct (0);
SetB_PaymentSelect_Acct (0);
SetB_RevaluationGain_Acct (0);
SetB_RevaluationLoss_Acct (0);
SetB_SettlementGain_Acct (0);
SetB_SettlementLoss_Acct (0);
SetB_UnallocatedCash_Acct (0);
SetB_Unidentified_Acct (0);
SetC_AcctSchema_ID (0);
SetC_BankAccount_ID (0);
}
 */
}
public X_C_BankAccount_Acct (Ctx ctx, int C_BankAccount_Acct_ID, Trx trxName) : base (ctx, C_BankAccount_Acct_ID, trxName)
{
/** if (C_BankAccount_Acct_ID == 0)
{
SetB_Asset_Acct (0);
SetB_Expense_Acct (0);
SetB_InTransit_Acct (0);
SetB_InterestExp_Acct (0);
SetB_InterestRev_Acct (0);
SetB_PaymentSelect_Acct (0);
SetB_RevaluationGain_Acct (0);
SetB_RevaluationLoss_Acct (0);
SetB_SettlementGain_Acct (0);
SetB_SettlementLoss_Acct (0);
SetB_UnallocatedCash_Acct (0);
SetB_Unidentified_Acct (0);
SetC_AcctSchema_ID (0);
SetC_BankAccount_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankAccount_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankAccount_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankAccount_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BankAccount_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370683L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053894L;
/** AD_Table_ID=391 */
public static int Table_ID;
 // =391;

/** TableName=C_BankAccount_Acct */
public static String Table_Name="C_BankAccount_Acct";

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
StringBuilder sb = new StringBuilder ("X_C_BankAccount_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Bank Asset.
@param B_Asset_Acct Bank Asset Account */
public void SetB_Asset_Acct (int B_Asset_Acct)
{
Set_Value ("B_Asset_Acct", B_Asset_Acct);
}
/** Get Bank Asset.
@return Bank Asset Account */
public int GetB_Asset_Acct() 
{
Object ii = Get_Value("B_Asset_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Expense.
@param B_Expense_Acct Bank Expense Account */
public void SetB_Expense_Acct (int B_Expense_Acct)
{
Set_Value ("B_Expense_Acct", B_Expense_Acct);
}
/** Get Bank Expense.
@return Bank Expense Account */
public int GetB_Expense_Acct() 
{
Object ii = Get_Value("B_Expense_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank In Transit.
@param B_InTransit_Acct Bank In Transit Account */
public void SetB_InTransit_Acct (int B_InTransit_Acct)
{
Set_Value ("B_InTransit_Acct", B_InTransit_Acct);
}
/** Get Bank In Transit.
@return Bank In Transit Account */
public int GetB_InTransit_Acct() 
{
Object ii = Get_Value("B_InTransit_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Interest Expense.
@param B_InterestExp_Acct Bank Interest Expense Account */
public void SetB_InterestExp_Acct (int B_InterestExp_Acct)
{
Set_Value ("B_InterestExp_Acct", B_InterestExp_Acct);
}
/** Get Bank Interest Expense.
@return Bank Interest Expense Account */
public int GetB_InterestExp_Acct() 
{
Object ii = Get_Value("B_InterestExp_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Interest Revenue.
@param B_InterestRev_Acct Bank Interest Revenue Account */
public void SetB_InterestRev_Acct (int B_InterestRev_Acct)
{
Set_Value ("B_InterestRev_Acct", B_InterestRev_Acct);
}
/** Get Bank Interest Revenue.
@return Bank Interest Revenue Account */
public int GetB_InterestRev_Acct() 
{
Object ii = Get_Value("B_InterestRev_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Selection.
@param B_PaymentSelect_Acct AP Payment Selection Clearing Account */
public void SetB_PaymentSelect_Acct (int B_PaymentSelect_Acct)
{
Set_Value ("B_PaymentSelect_Acct", B_PaymentSelect_Acct);
}
/** Get Payment Selection.
@return AP Payment Selection Clearing Account */
public int GetB_PaymentSelect_Acct() 
{
Object ii = Get_Value("B_PaymentSelect_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Revaluation Gain.
@param B_RevaluationGain_Acct Bank Revaluation Gain Account */
public void SetB_RevaluationGain_Acct (int B_RevaluationGain_Acct)
{
Set_Value ("B_RevaluationGain_Acct", B_RevaluationGain_Acct);
}
/** Get Bank Revaluation Gain.
@return Bank Revaluation Gain Account */
public int GetB_RevaluationGain_Acct() 
{
Object ii = Get_Value("B_RevaluationGain_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Revaluation Loss.
@param B_RevaluationLoss_Acct Bank Revaluation Loss Account */
public void SetB_RevaluationLoss_Acct (int B_RevaluationLoss_Acct)
{
Set_Value ("B_RevaluationLoss_Acct", B_RevaluationLoss_Acct);
}
/** Get Bank Revaluation Loss.
@return Bank Revaluation Loss Account */
public int GetB_RevaluationLoss_Acct() 
{
Object ii = Get_Value("B_RevaluationLoss_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Settlement Gain.
@param B_SettlementGain_Acct Bank Settlement Gain Account */
public void SetB_SettlementGain_Acct (int B_SettlementGain_Acct)
{
Set_Value ("B_SettlementGain_Acct", B_SettlementGain_Acct);
}
/** Get Bank Settlement Gain.
@return Bank Settlement Gain Account */
public int GetB_SettlementGain_Acct() 
{
Object ii = Get_Value("B_SettlementGain_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Settlement Loss.
@param B_SettlementLoss_Acct Bank Settlement Loss Account */
public void SetB_SettlementLoss_Acct (int B_SettlementLoss_Acct)
{
Set_Value ("B_SettlementLoss_Acct", B_SettlementLoss_Acct);
}
/** Get Bank Settlement Loss.
@return Bank Settlement Loss Account */
public int GetB_SettlementLoss_Acct() 
{
Object ii = Get_Value("B_SettlementLoss_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Unallocated Cash.
@param B_UnallocatedCash_Acct Unallocated Cash Clearing Account */
public void SetB_UnallocatedCash_Acct (int B_UnallocatedCash_Acct)
{
Set_Value ("B_UnallocatedCash_Acct", B_UnallocatedCash_Acct);
}
/** Get Unallocated Cash.
@return Unallocated Cash Clearing Account */
public int GetB_UnallocatedCash_Acct() 
{
Object ii = Get_Value("B_UnallocatedCash_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Unidentified Receipts.
@param B_Unidentified_Acct Bank Unidentified Receipts Account */
public void SetB_Unidentified_Acct (int B_Unidentified_Acct)
{
Set_Value ("B_Unidentified_Acct", B_Unidentified_Acct);
}
/** Get Bank Unidentified Receipts.
@return Bank Unidentified Receipts Account */
public int GetB_Unidentified_Acct() 
{
Object ii = Get_Value("B_Unidentified_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
public void SetC_BankAccount_ID (int C_BankAccount_ID)
{
if (C_BankAccount_ID < 1) throw new ArgumentException ("C_BankAccount_ID is mandatory.");
Set_ValueNoCheck ("C_BankAccount_ID", C_BankAccount_ID);
}
/** Get Bank Account.
@return Account at the Bank */
public int GetC_BankAccount_ID() 
{
Object ii = Get_Value("C_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
