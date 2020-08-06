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
/** Generated Model for M_Product_Costing
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Product_Costing : PO
{
public X_M_Product_Costing (Context ctx, int M_Product_Costing_ID, Trx trxName) : base (ctx, M_Product_Costing_ID, trxName)
{
/** if (M_Product_Costing_ID == 0)
{
SetC_AcctSchema_ID (0);
SetCostAverage (0.0);
SetCostAverageCumAmt (0.0);
SetCostAverageCumQty (0.0);
SetCostStandard (0.0);
SetCostStandardCumAmt (0.0);
SetCostStandardCumQty (0.0);
SetCostStandardPOAmt (0.0);
SetCostStandardPOQty (0.0);
SetCurrentCostPrice (0.0);
SetFutureCostPrice (0.0);
SetM_Product_ID (0);
SetPriceLastInv (0.0);
SetPriceLastPO (0.0);
SetTotalInvAmt (0.0);
SetTotalInvQty (0.0);
}
 */
}
public X_M_Product_Costing (Ctx ctx, int M_Product_Costing_ID, Trx trxName) : base (ctx, M_Product_Costing_ID, trxName)
{
/** if (M_Product_Costing_ID == 0)
{
SetC_AcctSchema_ID (0);
SetCostAverage (0.0);
SetCostAverageCumAmt (0.0);
SetCostAverageCumQty (0.0);
SetCostStandard (0.0);
SetCostStandardCumAmt (0.0);
SetCostStandardCumQty (0.0);
SetCostStandardPOAmt (0.0);
SetCostStandardPOQty (0.0);
SetCurrentCostPrice (0.0);
SetFutureCostPrice (0.0);
SetM_Product_ID (0);
SetPriceLastInv (0.0);
SetPriceLastPO (0.0);
SetTotalInvAmt (0.0);
SetTotalInvQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Costing (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Costing (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Costing (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Product_Costing()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380854L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064065L;
/** AD_Table_ID=327 */
public static int Table_ID;
 // =327;

/** TableName=M_Product_Costing */
public static String Table_Name="M_Product_Costing";

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
StringBuilder sb = new StringBuilder ("X_M_Product_Costing[").Append(Get_ID()).Append("]");
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
/** Set Average Cost.
@param CostAverage Weighted average costs */
public void SetCostAverage (Decimal? CostAverage)
{
if (CostAverage == null) throw new ArgumentException ("CostAverage is mandatory.");
Set_ValueNoCheck ("CostAverage", (Decimal?)CostAverage);
}
/** Get Average Cost.
@return Weighted average costs */
public Decimal GetCostAverage() 
{
Object bd =Get_Value("CostAverage");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Average Cost Amount Sum.
@param CostAverageCumAmt Cumulative average cost amounts (internal) */
public void SetCostAverageCumAmt (Decimal? CostAverageCumAmt)
{
if (CostAverageCumAmt == null) throw new ArgumentException ("CostAverageCumAmt is mandatory.");
Set_ValueNoCheck ("CostAverageCumAmt", (Decimal?)CostAverageCumAmt);
}
/** Get Average Cost Amount Sum.
@return Cumulative average cost amounts (internal) */
public Decimal GetCostAverageCumAmt() 
{
Object bd =Get_Value("CostAverageCumAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Average Cost Quantity Sum.
@param CostAverageCumQty Cumulative average cost quantities (internal) */
public void SetCostAverageCumQty (Decimal? CostAverageCumQty)
{
if (CostAverageCumQty == null) throw new ArgumentException ("CostAverageCumQty is mandatory.");
Set_ValueNoCheck ("CostAverageCumQty", (Decimal?)CostAverageCumQty);
}
/** Get Average Cost Quantity Sum.
@return Cumulative average cost quantities (internal) */
public Decimal GetCostAverageCumQty() 
{
Object bd =Get_Value("CostAverageCumQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard Cost.
@param CostStandard Standard Costs */
public void SetCostStandard (Decimal? CostStandard)
{
if (CostStandard == null) throw new ArgumentException ("CostStandard is mandatory.");
Set_ValueNoCheck ("CostStandard", (Decimal?)CostStandard);
}
/** Get Standard Cost.
@return Standard Costs */
public Decimal GetCostStandard() 
{
Object bd =Get_Value("CostStandard");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Std Cost Amount Sum.
@param CostStandardCumAmt Standard Cost Invoice Amount Sum (internal) */
public void SetCostStandardCumAmt (Decimal? CostStandardCumAmt)
{
if (CostStandardCumAmt == null) throw new ArgumentException ("CostStandardCumAmt is mandatory.");
Set_ValueNoCheck ("CostStandardCumAmt", (Decimal?)CostStandardCumAmt);
}
/** Get Std Cost Amount Sum.
@return Standard Cost Invoice Amount Sum (internal) */
public Decimal GetCostStandardCumAmt() 
{
Object bd =Get_Value("CostStandardCumAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Std Cost Quantity Sum.
@param CostStandardCumQty Standard Cost Invoice Quantity Sum (internal) */
public void SetCostStandardCumQty (Decimal? CostStandardCumQty)
{
if (CostStandardCumQty == null) throw new ArgumentException ("CostStandardCumQty is mandatory.");
Set_ValueNoCheck ("CostStandardCumQty", (Decimal?)CostStandardCumQty);
}
/** Get Std Cost Quantity Sum.
@return Standard Cost Invoice Quantity Sum (internal) */
public Decimal GetCostStandardCumQty() 
{
Object bd =Get_Value("CostStandardCumQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Std PO Cost Amount Sum.
@param CostStandardPOAmt Standard Cost Purchase Order Amount Sum (internal) */
public void SetCostStandardPOAmt (Decimal? CostStandardPOAmt)
{
if (CostStandardPOAmt == null) throw new ArgumentException ("CostStandardPOAmt is mandatory.");
Set_ValueNoCheck ("CostStandardPOAmt", (Decimal?)CostStandardPOAmt);
}
/** Get Std PO Cost Amount Sum.
@return Standard Cost Purchase Order Amount Sum (internal) */
public Decimal GetCostStandardPOAmt() 
{
Object bd =Get_Value("CostStandardPOAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Std PO Cost Quantity Sum.
@param CostStandardPOQty Standard Cost Purchase Order Quantity Sum (internal) */
public void SetCostStandardPOQty (Decimal? CostStandardPOQty)
{
if (CostStandardPOQty == null) throw new ArgumentException ("CostStandardPOQty is mandatory.");
Set_ValueNoCheck ("CostStandardPOQty", (Decimal?)CostStandardPOQty);
}
/** Get Std PO Cost Quantity Sum.
@return Standard Cost Purchase Order Quantity Sum (internal) */
public Decimal GetCostStandardPOQty() 
{
Object bd =Get_Value("CostStandardPOQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Current Cost Price.
@param CurrentCostPrice The currently used cost price */
public void SetCurrentCostPrice (Decimal? CurrentCostPrice)
{
if (CurrentCostPrice == null) throw new ArgumentException ("CurrentCostPrice is mandatory.");
Set_Value ("CurrentCostPrice", (Decimal?)CurrentCostPrice);
}
/** Get Current Cost Price.
@return The currently used cost price */
public Decimal GetCurrentCostPrice() 
{
Object bd =Get_Value("CurrentCostPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Future Cost Price.
@param FutureCostPrice Future Cost Price */
public void SetFutureCostPrice (Decimal? FutureCostPrice)
{
if (FutureCostPrice == null) throw new ArgumentException ("FutureCostPrice is mandatory.");
Set_Value ("FutureCostPrice", (Decimal?)FutureCostPrice);
}
/** Get Future Cost Price.
@return Future Cost Price */
public Decimal GetFutureCostPrice() 
{
Object bd =Get_Value("FutureCostPrice");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Last Invoice Price.
@param PriceLastInv Price of the last invoice for the product */
public void SetPriceLastInv (Decimal? PriceLastInv)
{
if (PriceLastInv == null) throw new ArgumentException ("PriceLastInv is mandatory.");
Set_ValueNoCheck ("PriceLastInv", (Decimal?)PriceLastInv);
}
/** Get Last Invoice Price.
@return Price of the last invoice for the product */
public Decimal GetPriceLastInv() 
{
Object bd =Get_Value("PriceLastInv");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Last PO Price.
@param PriceLastPO Price of the last purchase order for the product */
public void SetPriceLastPO (Decimal? PriceLastPO)
{
if (PriceLastPO == null) throw new ArgumentException ("PriceLastPO is mandatory.");
Set_ValueNoCheck ("PriceLastPO", (Decimal?)PriceLastPO);
}
/** Get Last PO Price.
@return Price of the last purchase order for the product */
public Decimal GetPriceLastPO() 
{
Object bd =Get_Value("PriceLastPO");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Total Invoice Amount.
@param TotalInvAmt Cumulative total lifetime invoice amount */
public void SetTotalInvAmt (Decimal? TotalInvAmt)
{
if (TotalInvAmt == null) throw new ArgumentException ("TotalInvAmt is mandatory.");
Set_ValueNoCheck ("TotalInvAmt", (Decimal?)TotalInvAmt);
}
/** Get Total Invoice Amount.
@return Cumulative total lifetime invoice amount */
public Decimal GetTotalInvAmt() 
{
Object bd =Get_Value("TotalInvAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Total Invoice Quantity.
@param TotalInvQty Cumulative total lifetime invoice quantity */
public void SetTotalInvQty (Decimal? TotalInvQty)
{
if (TotalInvQty == null) throw new ArgumentException ("TotalInvQty is mandatory.");
Set_ValueNoCheck ("TotalInvQty", (Decimal?)TotalInvQty);
}
/** Get Total Invoice Quantity.
@return Cumulative total lifetime invoice quantity */
public Decimal GetTotalInvQty() 
{
Object bd =Get_Value("TotalInvQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
