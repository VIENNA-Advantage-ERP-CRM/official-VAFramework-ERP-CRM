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
/** Generated Model for VAB_WorkCommission_Amt
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_WorkCommission_Amt : PO
{
public X_VAB_WorkCommission_Amt (Context ctx, int VAB_WorkCommission_Amt_ID, Trx trxName) : base (ctx, VAB_WorkCommission_Amt_ID, trxName)
{
/** if (VAB_WorkCommission_Amt_ID == 0)
{
SetActualQty (0.0);
SetVAB_WorkCommission_Amt_ID (0);
SetVAB_WorkCommissionLine_ID (0);
SetVAB_WorkCommission_Calc_ID (0);
SetCommissionAmt (0.0);
SetConvertedAmt (0.0);
}
 */
}
public X_VAB_WorkCommission_Amt (Ctx ctx, int VAB_WorkCommission_Amt_ID, Trx trxName) : base (ctx, VAB_WorkCommission_Amt_ID, trxName)
{
/** if (VAB_WorkCommission_Amt_ID == 0)
{
SetActualQty (0.0);
SetVAB_WorkCommission_Amt_ID (0);
SetVAB_WorkCommissionLine_ID (0);
SetVAB_WorkCommission_Calc_ID (0);
SetCommissionAmt (0.0);
SetConvertedAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommission_Amt (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommission_Amt (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommission_Amt (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_WorkCommission_Amt()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371294L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054505L;
/** VAF_TableView_ID=430 */
public static int Table_ID;
 // =430;

/** TableName=VAB_WorkCommission_Amt */
public static String Table_Name="VAB_WorkCommission_Amt";

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
StringBuilder sb = new StringBuilder ("X_VAB_WorkCommission_Amt[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Commission Line.
@param VAB_WorkCommissionLine_ID Commission Line */
public void SetVAB_WorkCommissionLine_ID (int VAB_WorkCommissionLine_ID)
{
if (VAB_WorkCommissionLine_ID < 1) throw new ArgumentException ("VAB_WorkCommissionLine_ID is mandatory.");
Set_Value ("VAB_WorkCommissionLine_ID", VAB_WorkCommissionLine_ID);
}
/** Get Commission Line.
@return Commission Line */
public int GetVAB_WorkCommissionLine_ID() 
{
Object ii = Get_Value("VAB_WorkCommissionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission Run.
@param VAB_WorkCommission_Calc_ID Commission Run or Process */
public void SetVAB_WorkCommission_Calc_ID (int VAB_WorkCommission_Calc_ID)
{
if (VAB_WorkCommission_Calc_ID < 1) throw new ArgumentException ("VAB_WorkCommission_Calc_ID is mandatory.");
Set_ValueNoCheck ("VAB_WorkCommission_Calc_ID", VAB_WorkCommission_Calc_ID);
}
/** Get Commission Run.
@return Commission Run or Process */
public int GetVAB_WorkCommission_Calc_ID() 
{
Object ii = Get_Value("VAB_WorkCommission_Calc_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_WorkCommission_Calc_ID().ToString());
}
/** Set Commission Amount.
@param CommissionAmt Commission Amount */
public void SetCommissionAmt (Decimal? CommissionAmt)
{
if (CommissionAmt == null) throw new ArgumentException ("CommissionAmt is mandatory.");
Set_Value ("CommissionAmt", (Decimal?)CommissionAmt);
}
/** Get Commission Amount.
@return Commission Amount */
public Decimal GetCommissionAmt() 
{
Object bd =Get_Value("CommissionAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
}

}
