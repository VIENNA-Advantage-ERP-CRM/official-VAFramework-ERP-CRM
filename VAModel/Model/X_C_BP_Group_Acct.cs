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
/** Generated Model for C_BP_Group_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BP_Group_Acct : PO
{
public X_C_BP_Group_Acct (Context ctx, int C_BP_Group_Acct_ID, Trx trxName) : base (ctx, C_BP_Group_Acct_ID, trxName)
{
/** if (C_BP_Group_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_BP_Group_ID (0);
SetC_Prepayment_Acct (0);
SetC_Receivable_Acct (0);
SetC_Receivable_Services_Acct (0);
SetNotInvoicedReceipts_Acct (0);
SetNotInvoicedReceivables_Acct (0);
SetNotInvoicedRevenue_Acct (0);
SetPayDiscount_Exp_Acct (0);
SetPayDiscount_Rev_Acct (0);
SetUnEarnedRevenue_Acct (0);
SetV_Liability_Acct (0);
SetV_Liability_Services_Acct (0);
SetV_Prepayment_Acct (0);
SetWriteOff_Acct (0);
}
 */
}
public X_C_BP_Group_Acct (Ctx ctx, int C_BP_Group_Acct_ID, Trx trxName) : base (ctx, C_BP_Group_Acct_ID, trxName)
{
/** if (C_BP_Group_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_BP_Group_ID (0);
SetC_Prepayment_Acct (0);
SetC_Receivable_Acct (0);
SetC_Receivable_Services_Acct (0);
SetNotInvoicedReceipts_Acct (0);
SetNotInvoicedReceivables_Acct (0);
SetNotInvoicedRevenue_Acct (0);
SetPayDiscount_Exp_Acct (0);
SetPayDiscount_Rev_Acct (0);
SetUnEarnedRevenue_Acct (0);
SetV_Liability_Acct (0);
SetV_Liability_Services_Acct (0);
SetV_Prepayment_Acct (0);
SetWriteOff_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Group_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Group_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Group_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BP_Group_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370197L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053408L;
/** AD_Table_ID=395 */
public static int Table_ID;
 // =395;

/** TableName=C_BP_Group_Acct */
public static String Table_Name="C_BP_Group_Acct";

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
StringBuilder sb = new StringBuilder ("X_C_BP_Group_Acct[").Append(Get_ID()).Append("]");
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
/** Set Business Partner Group.
@param C_BP_Group_ID Business Partner Group */
public void SetC_BP_Group_ID (int C_BP_Group_ID)
{
if (C_BP_Group_ID < 1) throw new ArgumentException ("C_BP_Group_ID is mandatory.");
Set_ValueNoCheck ("C_BP_Group_ID", C_BP_Group_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetC_BP_Group_ID() 
{
Object ii = Get_Value("C_BP_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Customer Prepayment.
@param C_Prepayment_Acct Account for customer prepayments */
public void SetC_Prepayment_Acct (int C_Prepayment_Acct)
{
Set_Value ("C_Prepayment_Acct", C_Prepayment_Acct);
}
/** Get Customer Prepayment.
@return Account for customer prepayments */
public int GetC_Prepayment_Acct() 
{
Object ii = Get_Value("C_Prepayment_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Customer Receivables.
@param C_Receivable_Acct Account for Customer Receivables */
public void SetC_Receivable_Acct (int C_Receivable_Acct)
{
Set_Value ("C_Receivable_Acct", C_Receivable_Acct);
}
/** Get Customer Receivables.
@return Account for Customer Receivables */
public int GetC_Receivable_Acct() 
{
Object ii = Get_Value("C_Receivable_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Receivable Services.
@param C_Receivable_Services_Acct Customer Accounts Receivables Services Account */
public void SetC_Receivable_Services_Acct (int C_Receivable_Services_Acct)
{
Set_Value ("C_Receivable_Services_Acct", C_Receivable_Services_Acct);
}
/** Get Receivable Services.
@return Customer Accounts Receivables Services Account */
public int GetC_Receivable_Services_Acct() 
{
Object ii = Get_Value("C_Receivable_Services_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Not-invoiced Receipts.
@param NotInvoicedReceipts_Acct Account for not-invoiced Material Receipts */
public void SetNotInvoicedReceipts_Acct (int NotInvoicedReceipts_Acct)
{
Set_Value ("NotInvoicedReceipts_Acct", NotInvoicedReceipts_Acct);
}
/** Get Not-invoiced Receipts.
@return Account for not-invoiced Material Receipts */
public int GetNotInvoicedReceipts_Acct() 
{
Object ii = Get_Value("NotInvoicedReceipts_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Not-invoiced Receivables.
@param NotInvoicedReceivables_Acct Account for not invoiced Receivables */
public void SetNotInvoicedReceivables_Acct (int NotInvoicedReceivables_Acct)
{
Set_Value ("NotInvoicedReceivables_Acct", NotInvoicedReceivables_Acct);
}
/** Get Not-invoiced Receivables.
@return Account for not invoiced Receivables */
public int GetNotInvoicedReceivables_Acct() 
{
Object ii = Get_Value("NotInvoicedReceivables_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Not-invoiced Revenue.
@param NotInvoicedRevenue_Acct Account for not invoiced Revenue */
public void SetNotInvoicedRevenue_Acct (int NotInvoicedRevenue_Acct)
{
Set_Value ("NotInvoicedRevenue_Acct", NotInvoicedRevenue_Acct);
}
/** Get Not-invoiced Revenue.
@return Account for not invoiced Revenue */
public int GetNotInvoicedRevenue_Acct() 
{
Object ii = Get_Value("NotInvoicedRevenue_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Discount Expense.
@param PayDiscount_Exp_Acct Payment Discount Expense Account */
public void SetPayDiscount_Exp_Acct (int PayDiscount_Exp_Acct)
{
Set_Value ("PayDiscount_Exp_Acct", PayDiscount_Exp_Acct);
}
/** Get Payment Discount Expense.
@return Payment Discount Expense Account */
public int GetPayDiscount_Exp_Acct() 
{
Object ii = Get_Value("PayDiscount_Exp_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Discount Revenue.
@param PayDiscount_Rev_Acct Payment Discount Revenue Account */
public void SetPayDiscount_Rev_Acct (int PayDiscount_Rev_Acct)
{
Set_Value ("PayDiscount_Rev_Acct", PayDiscount_Rev_Acct);
}
/** Get Payment Discount Revenue.
@return Payment Discount Revenue Account */
public int GetPayDiscount_Rev_Acct() 
{
Object ii = Get_Value("PayDiscount_Rev_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Unearned Revenue.
@param UnEarnedRevenue_Acct Account for unearned revenue */
public void SetUnEarnedRevenue_Acct (int UnEarnedRevenue_Acct)
{
Set_Value ("UnEarnedRevenue_Acct", UnEarnedRevenue_Acct);
}
/** Get Unearned Revenue.
@return Account for unearned revenue */
public int GetUnEarnedRevenue_Acct() 
{
Object ii = Get_Value("UnEarnedRevenue_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Vendor Liability.
@param V_Liability_Acct Account for Vendor Liability */
public void SetV_Liability_Acct (int V_Liability_Acct)
{
Set_Value ("V_Liability_Acct", V_Liability_Acct);
}
/** Get Vendor Liability.
@return Account for Vendor Liability */
public int GetV_Liability_Acct() 
{
Object ii = Get_Value("V_Liability_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Vendor Service Liability.
@param V_Liability_Services_Acct Account for Vender Service Liability */
public void SetV_Liability_Services_Acct (int V_Liability_Services_Acct)
{
Set_Value ("V_Liability_Services_Acct", V_Liability_Services_Acct);
}
/** Get Vendor Service Liability.
@return Account for Vender Service Liability */
public int GetV_Liability_Services_Acct() 
{
Object ii = Get_Value("V_Liability_Services_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Vendor Prepayment.
@param V_Prepayment_Acct Account for Vendor Prepayments */
public void SetV_Prepayment_Acct (int V_Prepayment_Acct)
{
Set_Value ("V_Prepayment_Acct", V_Prepayment_Acct);
}
/** Get Vendor Prepayment.
@return Account for Vendor Prepayments */
public int GetV_Prepayment_Acct() 
{
Object ii = Get_Value("V_Prepayment_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Write-off.
@param WriteOff_Acct Account for Receivables write-off */
public void SetWriteOff_Acct (int WriteOff_Acct)
{
Set_Value ("WriteOff_Acct", WriteOff_Acct);
}
/** Get Write-off.
@return Account for Receivables write-off */
public int GetWriteOff_Acct() 
{
Object ii = Get_Value("WriteOff_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
