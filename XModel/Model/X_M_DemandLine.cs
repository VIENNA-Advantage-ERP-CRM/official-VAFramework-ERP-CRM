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
/** Generated Model for M_DemandLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_DemandLine : PO
{
public X_M_DemandLine (Context ctx, int M_DemandLine_ID, Trx trxName) : base (ctx, M_DemandLine_ID, trxName)
{
/** if (M_DemandLine_ID == 0)
{
SetC_Period_ID (0);
SetM_DemandLine_ID (0);
SetM_Demand_ID (0);
SetM_Product_ID (0);
SetQty (0.0);
SetQtyCalculated (0.0);
}
 */
}
public X_M_DemandLine (Ctx ctx, int M_DemandLine_ID, Trx trxName) : base (ctx, M_DemandLine_ID, trxName)
{
/** if (M_DemandLine_ID == 0)
{
SetC_Period_ID (0);
SetM_DemandLine_ID (0);
SetM_Demand_ID (0);
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
public X_M_DemandLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DemandLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_DemandLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_DemandLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379052L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062263L;
/** AD_Table_ID=719 */
public static int Table_ID;
 // =719;

/** TableName=M_DemandLine */
public static String Table_Name="M_DemandLine";

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
StringBuilder sb = new StringBuilder ("X_M_DemandLine[").Append(Get_ID()).Append("]");
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
/** Set Demand Line.
@param M_DemandLine_ID Material Demand Line */
public void SetM_DemandLine_ID (int M_DemandLine_ID)
{
if (M_DemandLine_ID < 1) throw new ArgumentException ("M_DemandLine_ID is mandatory.");
Set_ValueNoCheck ("M_DemandLine_ID", M_DemandLine_ID);
}
/** Get Demand Line.
@return Material Demand Line */
public int GetM_DemandLine_ID() 
{
Object ii = Get_Value("M_DemandLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Demand.
@param M_Demand_ID Material Demand */
public void SetM_Demand_ID (int M_Demand_ID)
{
if (M_Demand_ID < 1) throw new ArgumentException ("M_Demand_ID is mandatory.");
Set_ValueNoCheck ("M_Demand_ID", M_Demand_ID);
}
/** Get Demand.
@return Material Demand */
public int GetM_Demand_ID() 
{
Object ii = Get_Value("M_Demand_ID");
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
