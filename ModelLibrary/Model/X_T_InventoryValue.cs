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
/** Generated Model for T_InventoryValue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_InventoryValue : PO
{
public X_T_InventoryValue (Context ctx, int T_InventoryValue_ID, Trx trxName) : base (ctx, T_InventoryValue_ID, trxName)
{
/** if (T_InventoryValue_ID == 0)
{
SetAD_PInstance_ID (0);
SetM_AttributeSetInstance_ID (0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
}
 */
}
public X_T_InventoryValue (Ctx ctx, int T_InventoryValue_ID, Trx trxName) : base (ctx, T_InventoryValue_ID, trxName)
{
/** if (T_InventoryValue_ID == 0)
{
SetAD_PInstance_ID (0);
SetM_AttributeSetInstance_ID (0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_InventoryValue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_InventoryValue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_InventoryValue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_InventoryValue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384255L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067466L;
/** AD_Table_ID=478 */
public static int Table_ID;
 // =478;

/** TableName=T_InventoryValue */
public static String Table_Name="T_InventoryValue";

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
StringBuilder sb = new StringBuilder ("X_T_InventoryValue[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param AD_PInstance_ID Instance of the process */
public void SetAD_PInstance_ID (int AD_PInstance_ID)
{
if (AD_PInstance_ID < 1) throw new ArgumentException ("AD_PInstance_ID is mandatory.");
Set_ValueNoCheck ("AD_PInstance_ID", AD_PInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetAD_PInstance_ID() 
{
Object ii = Get_Value("AD_PInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
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
/** Set Cost.
@param Cost Cost information */
public void SetCost (Decimal? Cost)
{
Set_Value ("Cost", (Decimal?)Cost);
}
/** Get Cost.
@return Cost information */
public Decimal GetCost() 
{
Object bd =Get_Value("Cost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Cost Value.
@param CostAmt Value with Cost */
public void SetCostAmt (Decimal? CostAmt)
{
Set_Value ("CostAmt", (Decimal?)CostAmt);
}
/** Get Cost Value.
@return Value with Cost */
public Decimal GetCostAmt() 
{
Object bd =Get_Value("CostAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard Cost.
@param CostStandard Standard Costs */
public void SetCostStandard (Decimal? CostStandard)
{
Set_Value ("CostStandard", (Decimal?)CostStandard);
}
/** Get Standard Cost.
@return Standard Costs */
public Decimal GetCostStandard() 
{
Object bd =Get_Value("CostStandard");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard Cost Value.
@param CostStandardAmt Value in Standard Costs */
public void SetCostStandardAmt (Decimal? CostStandardAmt)
{
Set_Value ("CostStandardAmt", (Decimal?)CostStandardAmt);
}
/** Get Standard Cost Value.
@return Value in Standard Costs */
public Decimal GetCostStandardAmt() 
{
Object bd =Get_Value("CostStandardAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Valuation Date.
@param DateValue Date of valuation */
public void SetDateValue (DateTime? DateValue)
{
Set_Value ("DateValue", (DateTime?)DateValue);
}
/** Get Valuation Date.
@return Date of valuation */
public DateTime? GetDateValue() 
{
return (DateTime?)Get_Value("DateValue");
}
/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
public void SetM_CostElement_ID (int M_CostElement_ID)
{
if (M_CostElement_ID <= 0) Set_Value ("M_CostElement_ID", null);
else
Set_Value ("M_CostElement_ID", M_CostElement_ID);
}
/** Get Cost Element.
@return Product Cost Element */
public int GetM_CostElement_ID() 
{
Object ii = Get_Value("M_CostElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Price List Version.
@param M_PriceList_Version_ID Identifies a unique instance of a Price List */
public void SetM_PriceList_Version_ID (int M_PriceList_Version_ID)
{
if (M_PriceList_Version_ID <= 0) Set_Value ("M_PriceList_Version_ID", null);
else
Set_Value ("M_PriceList_Version_ID", M_PriceList_Version_ID);
}
/** Get Price List Version.
@return Identifies a unique instance of a Price List */
public int GetM_PriceList_Version_ID() 
{
Object ii = Get_Value("M_PriceList_Version_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_Value ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Limit Price.
@param PriceLimit Lowest price for a product */
public void SetPriceLimit (Decimal? PriceLimit)
{
Set_Value ("PriceLimit", (Decimal?)PriceLimit);
}
/** Get Limit Price.
@return Lowest price for a product */
public Decimal GetPriceLimit() 
{
Object bd =Get_Value("PriceLimit");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Limit price Value.
@param PriceLimitAmt Value with limit price */
public void SetPriceLimitAmt (Decimal? PriceLimitAmt)
{
Set_Value ("PriceLimitAmt", (Decimal?)PriceLimitAmt);
}
/** Get Limit price Value.
@return Value with limit price */
public Decimal GetPriceLimitAmt() 
{
Object bd =Get_Value("PriceLimitAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List Price.
@param PriceList List Price */
public void SetPriceList (Decimal? PriceList)
{
Set_Value ("PriceList", (Decimal?)PriceList);
}
/** Get List Price.
@return List Price */
public Decimal GetPriceList() 
{
Object bd =Get_Value("PriceList");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List price Value.
@param PriceListAmt Valuation with List Price */
public void SetPriceListAmt (Decimal? PriceListAmt)
{
Set_Value ("PriceListAmt", (Decimal?)PriceListAmt);
}
/** Get List price Value.
@return Valuation with List Price */
public Decimal GetPriceListAmt() 
{
Object bd =Get_Value("PriceListAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set PO Price.
@param PricePO Price based on a purchase order */
public void SetPricePO (Decimal? PricePO)
{
Set_Value ("PricePO", (Decimal?)PricePO);
}
/** Get PO Price.
@return Price based on a purchase order */
public Decimal GetPricePO() 
{
Object bd =Get_Value("PricePO");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set PO Price Value.
@param PricePOAmt Valuation with PO Price */
public void SetPricePOAmt (Decimal? PricePOAmt)
{
Set_Value ("PricePOAmt", (Decimal?)PricePOAmt);
}
/** Get PO Price Value.
@return Valuation with PO Price */
public Decimal GetPricePOAmt() 
{
Object bd =Get_Value("PricePOAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard Price.
@param PriceStd Standard Price */
public void SetPriceStd (Decimal? PriceStd)
{
Set_Value ("PriceStd", (Decimal?)PriceStd);
}
/** Get Standard Price.
@return Standard Price */
public Decimal GetPriceStd() 
{
Object bd =Get_Value("PriceStd");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Std Price Value.
@param PriceStdAmt Valuation with standard price */
public void SetPriceStdAmt (Decimal? PriceStdAmt)
{
Set_Value ("PriceStdAmt", (Decimal?)PriceStdAmt);
}
/** Get Std Price Value.
@return Valuation with standard price */
public Decimal GetPriceStdAmt() 
{
Object bd =Get_Value("PriceStdAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set On Hand Quantity.
@param QtyOnHand On Hand Quantity */
public void SetQtyOnHand (Decimal? QtyOnHand)
{
Set_Value ("QtyOnHand", (Decimal?)QtyOnHand);
}
/** Get On Hand Quantity.
@return On Hand Quantity */
public Decimal GetQtyOnHand() 
{
Object bd =Get_Value("QtyOnHand");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
