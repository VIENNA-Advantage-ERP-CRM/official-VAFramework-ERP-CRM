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
/** Generated Model for M_ForecastLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ForecastLine : PO
{
public X_M_ForecastLine (Context ctx, int M_ForecastLine_ID, Trx trxName) : base (ctx, M_ForecastLine_ID, trxName)
{
/** if (M_ForecastLine_ID == 0)
{
SetC_Period_ID (0);
SetM_ForecastLine_ID (0);
SetM_Forecast_ID (0);
SetM_Product_ID (0);
SetQty (0.0);
SetQtyCalculated (0.0);
}
 */
}
public X_M_ForecastLine (Ctx ctx, int M_ForecastLine_ID, Trx trxName) : base (ctx, M_ForecastLine_ID, trxName)
{
/** if (M_ForecastLine_ID == 0)
{
SetC_Period_ID (0);
SetM_ForecastLine_ID (0);
SetM_Forecast_ID (0);
SetM_Product_ID (0);
SetQty (0.0);
SetQtyCalculated (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ForecastLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ForecastLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ForecastLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ForecastLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379350L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062561L;
/** AD_Table_ID=722 */
public static int Table_ID;
 // =722;

/** TableName=M_ForecastLine */
public static String Table_Name="M_ForecastLine";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_M_ForecastLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Period.
@param C_Period_ID Period of the Calendar */
public void SetC_Period_ID (int C_Period_ID)
{
if (C_Period_ID < 1) throw new ArgumentException ("C_Period_ID is mandatory.");
Set_ValueNoCheck ("C_Period_ID", C_Period_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetC_Period_ID() 
{
Object ii = Get_Value("C_Period_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Period_ID().ToString());
}
/** Set Forecast Line.
@param M_ForecastLine_ID Forecast Line */
public void SetM_ForecastLine_ID (int M_ForecastLine_ID)
{
if (M_ForecastLine_ID < 1) throw new ArgumentException ("M_ForecastLine_ID is mandatory.");
Set_ValueNoCheck ("M_ForecastLine_ID", M_ForecastLine_ID);
}
/** Get Forecast Line.
@return Forecast Line */
public int GetM_ForecastLine_ID() 
{
Object ii = Get_Value("M_ForecastLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Forecast.
@param M_Forecast_ID Material Forecast */
public void SetM_Forecast_ID (int M_Forecast_ID)
{
if (M_Forecast_ID < 1) throw new ArgumentException ("M_Forecast_ID is mandatory.");
Set_ValueNoCheck ("M_Forecast_ID", M_Forecast_ID);
}
/** Get Forecast.
@return Material Forecast */
public int GetM_Forecast_ID() 
{
Object ii = Get_Value("M_Forecast_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_ValueNoCheck ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
if (Qty == null) throw new ArgumentException ("Qty is mandatory.");
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Calculated Quantity.
@param QtyCalculated Calculated Quantity */
public void SetQtyCalculated (Decimal? QtyCalculated)
{
if (QtyCalculated == null) throw new ArgumentException ("QtyCalculated is mandatory.");
Set_Value ("QtyCalculated", (Decimal?)QtyCalculated);
}
/** Get Calculated Quantity.
@return Calculated Quantity */
public Decimal GetQtyCalculated() 
{
Object bd =Get_Value("QtyCalculated");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
