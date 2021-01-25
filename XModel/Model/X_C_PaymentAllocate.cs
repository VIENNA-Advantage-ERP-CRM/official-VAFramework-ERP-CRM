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
/** Generated Model for C_PaymentAllocate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_PaymentAllocate : PO
{
public X_C_PaymentAllocate (Context ctx, int C_PaymentAllocate_ID, Trx trxName) : base (ctx, C_PaymentAllocate_ID, trxName)
{
/** if (C_PaymentAllocate_ID == 0)
{
SetAmount (0.0);
SetVAB_Invoice_ID (0);
SetC_PaymentAllocate_ID (0);
SetC_Payment_ID (0);
SetDiscountAmt (0.0);
SetOverUnderAmt (0.0);
SetWriteOffAmt (0.0);
}
 */
}
public X_C_PaymentAllocate (Ctx ctx, int C_PaymentAllocate_ID, Trx trxName) : base (ctx, C_PaymentAllocate_ID, trxName)
{
/** if (C_PaymentAllocate_ID == 0)
{
SetAmount (0.0);
SetVAB_Invoice_ID (0);
SetC_PaymentAllocate_ID (0);
SetC_Payment_ID (0);
SetDiscountAmt (0.0);
SetOverUnderAmt (0.0);
SetWriteOffAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentAllocate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentAllocate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentAllocate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_PaymentAllocate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373943L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057154L;
/** VAF_TableView_ID=812 */
public static int Table_ID;
 // =812;

/** TableName=C_PaymentAllocate */
public static String Table_Name="C_PaymentAllocate";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_C_PaymentAllocate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Amount.
@param Amount Amount in a defined currency */
public void SetAmount (Decimal? Amount)
{
if (Amount == null) throw new ArgumentException ("Amount is mandatory.");
Set_Value ("Amount", (Decimal?)Amount);
}
/** Get Amount.
@return Amount in a defined currency */
public Decimal GetAmount() 
{
Object bd =Get_Value("Amount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Allocation Line.
@param VAB_DocAllocationLine_ID Allocation Line */
public void SetVAB_DocAllocationLine_ID (int VAB_DocAllocationLine_ID)
{
if (VAB_DocAllocationLine_ID <= 0) Set_Value ("VAB_DocAllocationLine_ID", null);
else
Set_Value ("VAB_DocAllocationLine_ID", VAB_DocAllocationLine_ID);
}
/** Get Allocation Line.
@return Allocation Line */
public int GetVAB_DocAllocationLine_ID() 
{
Object ii = Get_Value("VAB_DocAllocationLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param VAB_Invoice_ID Invoice Identifier */
public void SetVAB_Invoice_ID (int VAB_Invoice_ID)
{
if (VAB_Invoice_ID < 1) throw new ArgumentException ("VAB_Invoice_ID is mandatory.");
Set_Value ("VAB_Invoice_ID", VAB_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetVAB_Invoice_ID() 
{
Object ii = Get_Value("VAB_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_Invoice_ID().ToString());
}
/** Set Allocate Payment.
@param C_PaymentAllocate_ID Allocate Payment to Invoices */
public void SetC_PaymentAllocate_ID (int C_PaymentAllocate_ID)
{
if (C_PaymentAllocate_ID < 1) throw new ArgumentException ("C_PaymentAllocate_ID is mandatory.");
Set_ValueNoCheck ("C_PaymentAllocate_ID", C_PaymentAllocate_ID);
}
/** Get Allocate Payment.
@return Allocate Payment to Invoices */
public int GetC_PaymentAllocate_ID() 
{
Object ii = Get_Value("C_PaymentAllocate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID < 1) throw new ArgumentException ("C_Payment_ID is mandatory.");
Set_ValueNoCheck ("C_Payment_ID", C_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() 
{
Object ii = Get_Value("C_Payment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Discount Amount.
@param DiscountAmt Calculated amount of discount */
public void SetDiscountAmt (Decimal? DiscountAmt)
{
if (DiscountAmt == null) throw new ArgumentException ("DiscountAmt is mandatory.");
Set_Value ("DiscountAmt", (Decimal?)DiscountAmt);
}
/** Get Discount Amount.
@return Calculated amount of discount */
public Decimal GetDiscountAmt() 
{
Object bd =Get_Value("DiscountAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Invoice Amt.
@param InvoiceAmt Invoice Amt */
public void SetInvoiceAmt (Decimal? InvoiceAmt)
{
Set_Value ("InvoiceAmt", (Decimal?)InvoiceAmt);
}
/** Get Invoice Amt.
@return Invoice Amt */
public Decimal GetInvoiceAmt() 
{
Object bd =Get_Value("InvoiceAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Over/Under Payment.
@param OverUnderAmt Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
public void SetOverUnderAmt (Decimal? OverUnderAmt)
{
if (OverUnderAmt == null) throw new ArgumentException ("OverUnderAmt is mandatory.");
Set_Value ("OverUnderAmt", (Decimal?)OverUnderAmt);
}
/** Get Over/Under Payment.
@return Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
public Decimal GetOverUnderAmt() 
{
Object bd =Get_Value("OverUnderAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Remaining Amt.
@param RemainingAmt Remaining Amount */
public void SetRemainingAmt (Decimal? RemainingAmt)
{
throw new ArgumentException ("RemainingAmt Is virtual column");
}
/** Get Remaining Amt.
@return Remaining Amount */
public Decimal GetRemainingAmt() 
{
Object bd =Get_Value("RemainingAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Write-off Amount.
@param WriteOffAmt Amount to write-off */
public void SetWriteOffAmt (Decimal? WriteOffAmt)
{
if (WriteOffAmt == null) throw new ArgumentException ("WriteOffAmt is mandatory.");
Set_Value ("WriteOffAmt", (Decimal?)WriteOffAmt);
}
/** Get Write-off Amount.
@return Amount to write-off */
public Decimal GetWriteOffAmt() 
{
Object bd =Get_Value("WriteOffAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Set Invoice Payment Schedule.
@param VAB_sched_InvoicePayment_ID Invoice Payment Schedule */
public void SetVAB_sched_InvoicePayment_ID(int VAB_sched_InvoicePayment_ID)
{
    if (VAB_sched_InvoicePayment_ID <= 0) Set_Value("VAB_sched_InvoicePayment_ID", null);
    else
        Set_Value("VAB_sched_InvoicePayment_ID", VAB_sched_InvoicePayment_ID);
}
/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
public int GetVAB_sched_InvoicePayment_ID()
{
    Object ii = Get_Value("VAB_sched_InvoicePayment_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Reversal Document.
@param ReversalDoc_ID Reference of its original document */
public void SetReversalDoc_ID(int ReversalDoc_ID)
{
    if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
    else
        Set_Value("ReversalDoc_ID", ReversalDoc_ID);
}
/** Get Reversal Document.
@return Reference of its original document */
public int GetReversalDoc_ID()
{
    Object ii = Get_Value("ReversalDoc_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
