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
/** Generated Model for VAM_TeamForecastLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_TeamForecastLine : PO
{
public X_VAM_TeamForecastLine (Context ctx, int VAM_TeamForecastLine_ID, Trx trxName) : base (ctx, VAM_TeamForecastLine_ID, trxName)
{
/** if (VAM_TeamForecastLine_ID == 0)
{
SetVAB_YearPeriod_ID (0);
SetVAM_TeamForecastLine_ID (0);
SetVAM_TeamForecast_ID (0);
SetVAM_Product_ID (0);
SetQty (0.0);
SetQtyCalculated (0.0);
}
 */
}
public X_VAM_TeamForecastLine (Ctx ctx, int VAM_TeamForecastLine_ID, Trx trxName) : base (ctx, VAM_TeamForecastLine_ID, trxName)
{
/** if (VAM_TeamForecastLine_ID == 0)
{
SetVAB_YearPeriod_ID (0);
SetVAM_TeamForecastLine_ID (0);
SetVAM_TeamForecast_ID (0);
SetVAM_Product_ID (0);
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
public X_VAM_TeamForecastLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_TeamForecastLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_TeamForecastLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_TeamForecastLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379350L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062561L;
/** VAF_TableView_ID=722 */
public static int Table_ID;
 // =722;

/** TableName=VAM_TeamForecastLine */
public static String Table_Name="VAM_TeamForecastLine";

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
StringBuilder sb = new StringBuilder ("X_VAM_TeamForecastLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Period.
@param VAB_YearPeriod_ID Period of the Calendar */
public void SetVAB_YearPeriod_ID (int VAB_YearPeriod_ID)
{
if (VAB_YearPeriod_ID < 1) throw new ArgumentException ("VAB_YearPeriod_ID is mandatory.");
Set_ValueNoCheck ("VAB_YearPeriod_ID", VAB_YearPeriod_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetVAB_YearPeriod_ID() 
{
Object ii = Get_Value("VAB_YearPeriod_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_YearPeriod_ID().ToString());
}
/** Set Forecast Line.
@param VAM_TeamForecastLine_ID Forecast Line */
public void SetVAM_TeamForecastLine_ID (int VAM_TeamForecastLine_ID)
{
if (VAM_TeamForecastLine_ID < 1) throw new ArgumentException ("VAM_TeamForecastLine_ID is mandatory.");
Set_ValueNoCheck ("VAM_TeamForecastLine_ID", VAM_TeamForecastLine_ID);
}
/** Get Forecast Line.
@return Forecast Line */
public int GetVAM_TeamForecastLine_ID() 
{
Object ii = Get_Value("VAM_TeamForecastLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Forecast.
@param VAM_TeamForecast_ID Material Forecast */
public void SetVAM_TeamForecast_ID (int VAM_TeamForecast_ID)
{
if (VAM_TeamForecast_ID < 1) throw new ArgumentException ("VAM_TeamForecast_ID is mandatory.");
Set_ValueNoCheck ("VAM_TeamForecast_ID", VAM_TeamForecast_ID);
}
/** Get Forecast.
@return Material Forecast */
public int GetVAM_TeamForecast_ID() 
{
Object ii = Get_Value("VAM_TeamForecast_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
Set_ValueNoCheck ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
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
