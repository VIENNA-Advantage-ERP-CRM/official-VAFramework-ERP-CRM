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
/** Generated Model for VAB_WorkCommissionDetail
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_WorkCommissionDetail : PO
{
public X_VAB_WorkCommissionDetail (Context ctx, int VAB_WorkCommissionDetail_ID, Trx trxName) : base (ctx, VAB_WorkCommissionDetail_ID, trxName)
{
/** if (VAB_WorkCommissionDetail_ID == 0)
{
SetActualAmt (0.0);
SetActualQty (0.0);
SetVAB_WorkCommission_Amt_ID (0);
SetVAB_WorkCommissionDetail_ID (0);
SetVAB_Currency_ID (0);
SetConvertedAmt (0.0);
}
 */
}
public X_VAB_WorkCommissionDetail (Ctx ctx, int VAB_WorkCommissionDetail_ID, Trx trxName) : base (ctx, VAB_WorkCommissionDetail_ID, trxName)
{
/** if (VAB_WorkCommissionDetail_ID == 0)
{
SetActualAmt (0.0);
SetActualQty (0.0);
SetVAB_WorkCommission_Amt_ID (0);
SetVAB_WorkCommissionDetail_ID (0);
SetVAB_Currency_ID (0);
SetConvertedAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommissionDetail (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommissionDetail (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommissionDetail (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_WorkCommissionDetail()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371310L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054521L;
/** VAF_TableView_ID=437 */
public static int Table_ID;
 // =437;

/** TableName=VAB_WorkCommissionDetail */
public static String Table_Name="VAB_WorkCommissionDetail";

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
StringBuilder sb = new StringBuilder ("X_VAB_WorkCommissionDetail[").Append(Get_ID()).Append("]");
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
@param VAB_WorkCommission_Amt_ID Generated Commission Amount */
public void SetVAB_WorkCommission_Amt_ID (int VAB_WorkCommission_Amt_ID)
{
if (VAB_WorkCommission_Amt_ID < 1) throw new ArgumentException ("VAB_WorkCommission_Amt_ID is mandatory.");
Set_ValueNoCheck ("VAB_WorkCommission_Amt_ID", VAB_WorkCommission_Amt_ID);
}
/** Get Commission Amount.
@return Generated Commission Amount */
public int GetVAB_WorkCommission_Amt_ID() 
{
Object ii = Get_Value("VAB_WorkCommission_Amt_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission Detail.
@param VAB_WorkCommissionDetail_ID Supporting information for Commission Amounts */
public void SetVAB_WorkCommissionDetail_ID (int VAB_WorkCommissionDetail_ID)
{
if (VAB_WorkCommissionDetail_ID < 1) throw new ArgumentException ("VAB_WorkCommissionDetail_ID is mandatory.");
Set_ValueNoCheck ("VAB_WorkCommissionDetail_ID", VAB_WorkCommissionDetail_ID);
}
/** Get Commission Detail.
@return Supporting information for Commission Amounts */
public int GetVAB_WorkCommissionDetail_ID() 
{
Object ii = Get_Value("VAB_WorkCommissionDetail_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID)
{
if (VAB_Currency_ID < 1) throw new ArgumentException ("VAB_Currency_ID is mandatory.");
Set_Value ("VAB_Currency_ID", VAB_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() 
{
Object ii = Get_Value("VAB_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Line.
@param VAB_InvoiceLine_ID Invoice Detail Line */
public void SetVAB_InvoiceLine_ID (int VAB_InvoiceLine_ID)
{
if (VAB_InvoiceLine_ID <= 0) Set_ValueNoCheck ("VAB_InvoiceLine_ID", null);
else
Set_ValueNoCheck ("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
}
/** Get Invoice Line.
@return Invoice Detail Line */
public int GetVAB_InvoiceLine_ID() 
{
Object ii = Get_Value("VAB_InvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order Line.
@param VAB_OrderLine_ID Order Line */
public void SetVAB_OrderLine_ID (int VAB_OrderLine_ID)
{
if (VAB_OrderLine_ID <= 0) Set_ValueNoCheck ("VAB_OrderLine_ID", null);
else
Set_ValueNoCheck ("VAB_OrderLine_ID", VAB_OrderLine_ID);
}
/** Get Order Line.
@return Order Line */
public int GetVAB_OrderLine_ID() 
{
Object ii = Get_Value("VAB_OrderLine_ID");
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
