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
/** Generated Model for C_AllocationLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AllocationLine : PO
{
public X_C_AllocationLine (Context ctx, int C_AllocationLine_ID, Trx trxName) : base (ctx, C_AllocationLine_ID, trxName)
{
/** if (C_AllocationLine_ID == 0)
{
SetAmount (0.0);
SetC_AllocationHdr_ID (0);
SetC_AllocationLine_ID (0);
SetDiscountAmt (0.0);
SetWriteOffAmt (0.0);
}
 */
}
public X_C_AllocationLine (Ctx ctx, int C_AllocationLine_ID, Trx trxName) : base (ctx, C_AllocationLine_ID, trxName)
{
/** if (C_AllocationLine_ID == 0)
{
SetAmount (0.0);
SetC_AllocationHdr_ID (0);
SetC_AllocationLine_ID (0);
SetDiscountAmt (0.0);
SetWriteOffAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AllocationLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369836L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053047L;
/** AD_Table_ID=390 */
public static int Table_ID;
 // =390;

/** TableName=C_AllocationLine */
public static String Table_Name="C_AllocationLine";

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
StringBuilder sb = new StringBuilder ("X_C_AllocationLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Amount.
@param Amount Amount in a defined currency */
public void SetAmount (Decimal? Amount)
{
if (Amount == null) throw new ArgumentException ("Amount is mandatory.");
Set_ValueNoCheck ("Amount", (Decimal?)Amount);
}
/** Get Amount.
@return Amount in a defined currency */
public Decimal GetAmount() 
{
Object bd =Get_Value("Amount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Allocation.
@param C_AllocationHdr_ID Payment allocation */
public void SetC_AllocationHdr_ID (int C_AllocationHdr_ID)
{
if (C_AllocationHdr_ID < 1) throw new ArgumentException ("C_AllocationHdr_ID is mandatory.");
Set_ValueNoCheck ("C_AllocationHdr_ID", C_AllocationHdr_ID);
}
/** Get Allocation.
@return Payment allocation */
public int GetC_AllocationHdr_ID() 
{
Object ii = Get_Value("C_AllocationHdr_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Allocation Line.
@param C_AllocationLine_ID Allocation Line */
public void SetC_AllocationLine_ID (int C_AllocationLine_ID)
{
if (C_AllocationLine_ID < 1) throw new ArgumentException ("C_AllocationLine_ID is mandatory.");
Set_ValueNoCheck ("C_AllocationLine_ID", C_AllocationLine_ID);
}
/** Get Allocation Line.
@return Allocation Line */
public int GetC_AllocationLine_ID() 
{
Object ii = Get_Value("C_AllocationLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_ValueNoCheck ("C_BPartner_ID", null);
else
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cash Journal Line.
@param C_CashLine_ID Cash Journal Line */
public void SetC_CashLine_ID (int C_CashLine_ID)
{
if (C_CashLine_ID <= 0) Set_ValueNoCheck ("C_CashLine_ID", null);
else
Set_ValueNoCheck ("C_CashLine_ID", C_CashLine_ID);
}
/** Get Cash Journal Line.
@return Cash Journal Line */
public int GetC_CashLine_ID() 
{
Object ii = Get_Value("C_CashLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_ValueNoCheck ("C_Invoice_ID", null);
else
Set_ValueNoCheck ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Invoice_ID().ToString());
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_ValueNoCheck ("C_Order_ID", null);
else
Set_ValueNoCheck ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Order */
public int GetC_Order_ID() 
{
Object ii = Get_Value("C_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID <= 0) Set_ValueNoCheck ("C_Payment_ID", null);
else
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
/** Set Transaction Date.
@param DateTrx Transaction Date */
public void SetDateTrx (DateTime? DateTrx)
{
Set_ValueNoCheck ("DateTrx", (DateTime?)DateTrx);
}
/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetDateTrx() 
{
return (DateTime?)Get_Value("DateTrx");
}
/** Set Discount Amount.
@param DiscountAmt Calculated amount of discount */
public void SetDiscountAmt (Decimal? DiscountAmt)
{
if (DiscountAmt == null) throw new ArgumentException ("DiscountAmt is mandatory.");
Set_ValueNoCheck ("DiscountAmt", (Decimal?)DiscountAmt);
}
/** Get Discount Amount.
@return Calculated amount of discount */
public Decimal GetDiscountAmt() 
{
Object bd =Get_Value("DiscountAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_ValueNoCheck ("IsManual", IsManual);
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
/** Set Over/Under Payment.
@param OverUnderAmt Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
public void SetOverUnderAmt (Decimal? OverUnderAmt)
{
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
/** Set Write-off Amount.
@param WriteOffAmt Amount to write-off */
public void SetWriteOffAmt (Decimal? WriteOffAmt)
{
if (WriteOffAmt == null) throw new ArgumentException ("WriteOffAmt is mandatory.");
Set_ValueNoCheck ("WriteOffAmt", (Decimal?)WriteOffAmt);
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
@param C_InvoicePaySchedule_ID Identifies a Invoice Payment Schedule */
public void SetC_InvoicePaySchedule_ID(int C_InvoicePaySchedule_ID)
{
    if (C_InvoicePaySchedule_ID <= 0) Set_Value("C_InvoicePaySchedule_ID", null);
    else
        Set_Value("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
}
/** Get Invoice Payment Schedule.
@return Identifies a Invoice Payment Schedule */
public int GetC_InvoicePaySchedule_ID()
{
    Object ii = Get_Value("C_InvoicePaySchedule_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
   /* @param M_CostAllocation_ID Cost Allocation */
public void SetM_CostAllocation_ID (int M_CostAllocation_ID){if (M_CostAllocation_ID <= 0) Set_Value ("M_CostAllocation_ID", null);else
Set_Value ("M_CostAllocation_ID", M_CostAllocation_ID);}/** Get Cost Allocation.
@return Cost Allocation */
public int GetM_CostAllocation_ID() {Object ii = Get_Value("M_CostAllocation_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
}

}
