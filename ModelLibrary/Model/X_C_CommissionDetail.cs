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
/** Generated Model for C_CommissionDetail
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CommissionDetail : PO
{
public X_C_CommissionDetail (Context ctx, int C_CommissionDetail_ID, Trx trxName) : base (ctx, C_CommissionDetail_ID, trxName)
{
/** if (C_CommissionDetail_ID == 0)
{
SetActualAmt (0.0);
SetActualQty (0.0);
SetC_CommissionAmt_ID (0);
SetC_CommissionDetail_ID (0);
SetC_Currency_ID (0);
SetConvertedAmt (0.0);
}
 */
}
public X_C_CommissionDetail (Ctx ctx, int C_CommissionDetail_ID, Trx trxName) : base (ctx, C_CommissionDetail_ID, trxName)
{
/** if (C_CommissionDetail_ID == 0)
{
SetActualAmt (0.0);
SetActualQty (0.0);
SetC_CommissionAmt_ID (0);
SetC_CommissionDetail_ID (0);
SetC_Currency_ID (0);
SetConvertedAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CommissionDetail (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CommissionDetail (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CommissionDetail (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CommissionDetail()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371310L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054521L;
/** AD_Table_ID=437 */
public static int Table_ID;
 // =437;

/** TableName=C_CommissionDetail */
public static String Table_Name="C_CommissionDetail";

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
StringBuilder sb = new StringBuilder ("X_C_CommissionDetail[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Actual Amount.
@param ActualAmt The actual amount */
public void SetActualAmt (Decimal? ActualAmt)
{
if (ActualAmt == null) throw new ArgumentException ("ActualAmt is mandatory.");
Set_Value ("ActualAmt", (Decimal?)ActualAmt);
}
/** Get Actual Amount.
@return The actual amount */
public Decimal GetActualAmt() 
{
Object bd =Get_Value("ActualAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Actual Quantity.
@param ActualQty The actual quantity */
public void SetActualQty (Decimal? ActualQty)
{
if (ActualQty == null) throw new ArgumentException ("ActualQty is mandatory.");
Set_Value ("ActualQty", (Decimal?)ActualQty);
}
/** Get Actual Quantity.
@return The actual quantity */
public Decimal GetActualQty() 
{
Object bd =Get_Value("ActualQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Commission Amount.
@param C_CommissionAmt_ID Generated Commission Amount */
public void SetC_CommissionAmt_ID (int C_CommissionAmt_ID)
{
if (C_CommissionAmt_ID < 1) throw new ArgumentException ("C_CommissionAmt_ID is mandatory.");
Set_ValueNoCheck ("C_CommissionAmt_ID", C_CommissionAmt_ID);
}
/** Get Commission Amount.
@return Generated Commission Amount */
public int GetC_CommissionAmt_ID() 
{
Object ii = Get_Value("C_CommissionAmt_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission Detail.
@param C_CommissionDetail_ID Supporting information for Commission Amounts */
public void SetC_CommissionDetail_ID (int C_CommissionDetail_ID)
{
if (C_CommissionDetail_ID < 1) throw new ArgumentException ("C_CommissionDetail_ID is mandatory.");
Set_ValueNoCheck ("C_CommissionDetail_ID", C_CommissionDetail_ID);
}
/** Get Commission Detail.
@return Supporting information for Commission Amounts */
public int GetC_CommissionDetail_ID() 
{
Object ii = Get_Value("C_CommissionDetail_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
public void SetC_InvoiceLine_ID (int C_InvoiceLine_ID)
{
if (C_InvoiceLine_ID <= 0) Set_ValueNoCheck ("C_InvoiceLine_ID", null);
else
Set_ValueNoCheck ("C_InvoiceLine_ID", C_InvoiceLine_ID);
}
/** Get Invoice Line.
@return Invoice Detail Line */
public int GetC_InvoiceLine_ID() 
{
Object ii = Get_Value("C_InvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order Line.
@param C_OrderLine_ID Order Line */
public void SetC_OrderLine_ID (int C_OrderLine_ID)
{
if (C_OrderLine_ID <= 0) Set_ValueNoCheck ("C_OrderLine_ID", null);
else
Set_ValueNoCheck ("C_OrderLine_ID", C_OrderLine_ID);
}
/** Get Order Line.
@return Order Line */
public int GetC_OrderLine_ID() 
{
Object ii = Get_Value("C_OrderLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Converted Amount.
@param ConvertedAmt Converted Amount */
public void SetConvertedAmt (Decimal? ConvertedAmt)
{
if (ConvertedAmt == null) throw new ArgumentException ("ConvertedAmt is mandatory.");
Set_Value ("ConvertedAmt", (Decimal?)ConvertedAmt);
}
/** Get Converted Amount.
@return Converted Amount */
public Decimal GetConvertedAmt() 
{
Object bd =Get_Value("ConvertedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Info.
@param Info Information */
public void SetInfo (String Info)
{
if (Info != null && Info.Length > 60)
{
log.Warning("Length > 60 - truncated");
Info = Info.Substring(0,60);
}
Set_Value ("Info", Info);
}
/** Get Info.
@return Information */
public String GetInfo() 
{
return (String)Get_Value("Info");
}
/** Set Reference.
@param Reference Reference for this record */
public void SetReference (String Reference)
{
if (Reference != null && Reference.Length > 60)
{
log.Warning("Length > 60 - truncated");
Reference = Reference.Substring(0,60);
}
Set_Value ("Reference", Reference);
}
/** Get Reference.
@return Reference for this record */
public String GetReference() 
{
return (String)Get_Value("Reference");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetReference());
}
}

}
