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
/** Generated Model for RV_WarehousePrice
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_RV_WarehousePrice : PO
{
public X_RV_WarehousePrice (Context ctx, int RV_WarehousePrice_ID, Trx trxName) : base (ctx, RV_WarehousePrice_ID, trxName)
{
/** if (RV_WarehousePrice_ID == 0)
{
SetC_UOM_ID (0);
SetM_PriceList_Version_ID (0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetName (null);
SetValue (null);
SetWarehouseName (null);
}
 */
}
public X_RV_WarehousePrice (Ctx ctx, int RV_WarehousePrice_ID, Trx trxName) : base (ctx, RV_WarehousePrice_ID, trxName)
{
/** if (RV_WarehousePrice_ID == 0)
{
SetC_UOM_ID (0);
SetM_PriceList_Version_ID (0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetName (null);
SetValue (null);
SetWarehouseName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RV_WarehousePrice (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RV_WarehousePrice (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RV_WarehousePrice (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_RV_WarehousePrice()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
////static long serialVersionUID 27562514382641L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065852L;
/** AD_Table_ID=639 */
public static int Table_ID;
 // =639;

/** TableName=RV_WarehousePrice */
public static String Table_Name="RV_WarehousePrice";

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
StringBuilder sb = new StringBuilder ("X_RV_WarehousePrice[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID < 1) throw new ArgumentException ("C_UOM_ID is mandatory.");
Set_ValueNoCheck ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Instance Attribute.
@param IsInstanceAttribute The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
public void SetIsInstanceAttribute (Boolean IsInstanceAttribute)
{
Set_ValueNoCheck ("IsInstanceAttribute", IsInstanceAttribute);
}
/** Get Instance Attribute.
@return The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
public Boolean IsInstanceAttribute() 
{
Object oo = Get_Value("IsInstanceAttribute");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Price List Version.
@param M_PriceList_Version_ID Identifies a unique instance of a Price List */
public void SetM_PriceList_Version_ID (int M_PriceList_Version_ID)
{
if (M_PriceList_Version_ID < 1) throw new ArgumentException ("M_PriceList_Version_ID is mandatory.");
Set_ValueNoCheck ("M_PriceList_Version_ID", M_PriceList_Version_ID);
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
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_ValueNoCheck ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Margin %.
@param Margin Margin for a product as a percentage */
public void SetMargin (Decimal? Margin)
{
Set_ValueNoCheck ("Margin", (Decimal?)Margin);
}
/** Get Margin %.
@return Margin for a product as a percentage */
public Decimal GetMargin() 
{
Object bd =Get_Value("Margin");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_ValueNoCheck ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Limit Price.
@param PriceLimit Lowest price for a product */
public void SetPriceLimit (Decimal? PriceLimit)
{
Set_ValueNoCheck ("PriceLimit", (Decimal?)PriceLimit);
}
/** Get Limit Price.
@return Lowest price for a product */
public Decimal GetPriceLimit() 
{
Object bd =Get_Value("PriceLimit");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set List Price.
@param PriceList List Price */
public void SetPriceList (Decimal? PriceList)
{
Set_ValueNoCheck ("PriceList", (Decimal?)PriceList);
}
/** Get List Price.
@return List Price */
public Decimal GetPriceList() 
{
Object bd =Get_Value("PriceList");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Standard Price.
@param PriceStd Standard Price */
public void SetPriceStd (Decimal? PriceStd)
{
Set_ValueNoCheck ("PriceStd", (Decimal?)PriceStd);
}
/** Get Standard Price.
@return Standard Price */
public Decimal GetPriceStd() 
{
Object bd =Get_Value("PriceStd");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Available Quantity.
@param QtyAvailable Available Quantity (On Hand - Reserved) */
public void SetQtyAvailable (Decimal? QtyAvailable)
{
Set_ValueNoCheck ("QtyAvailable", (Decimal?)QtyAvailable);
}
/** Get Available Quantity.
@return Available Quantity (On Hand - Reserved) */
public Decimal GetQtyAvailable() 
{
Object bd =Get_Value("QtyAvailable");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set On Hand Quantity.
@param QtyOnHand On Hand Quantity */
public void SetQtyOnHand (Decimal? QtyOnHand)
{
Set_ValueNoCheck ("QtyOnHand", (Decimal?)QtyOnHand);
}
/** Get On Hand Quantity.
@return On Hand Quantity */
public Decimal GetQtyOnHand() 
{
Object bd =Get_Value("QtyOnHand");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Ordered Quantity.
@param QtyOrdered Ordered Quantity */
public void SetQtyOrdered (Decimal? QtyOrdered)
{
Set_ValueNoCheck ("QtyOrdered", (Decimal?)QtyOrdered);
}
/** Get Ordered Quantity.
@return Ordered Quantity */
public Decimal GetQtyOrdered() 
{
Object bd =Get_Value("QtyOrdered");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity Reserved.
@param QtyReserved Quantity Reserved */
public void SetQtyReserved (Decimal? QtyReserved)
{
Set_ValueNoCheck ("QtyReserved", (Decimal?)QtyReserved);
}
/** Get Quantity Reserved.
@return Quantity Reserved */
public Decimal GetQtyReserved() 
{
Object bd =Get_Value("QtyReserved");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set SKU.
@param SKU Stock Keeping Unit */
public void SetSKU (String SKU)
{
if (SKU != null && SKU.Length > 30)
{
log.Warning("Length > 30 - truncated");
SKU = SKU.Substring(0,30);
}
Set_ValueNoCheck ("SKU", SKU);
}
/** Get SKU.
@return Stock Keeping Unit */
public String GetSKU() 
{
return (String)Get_Value("SKU");
}
/** Set Symbol.
@param UOMSymbol Symbol for a Unit of Measure */
public void SetUOMSymbol (String UOMSymbol)
{
if (UOMSymbol != null && UOMSymbol.Length > 10)
{
log.Warning("Length > 10 - truncated");
UOMSymbol = UOMSymbol.Substring(0,10);
}
Set_ValueNoCheck ("UOMSymbol", UOMSymbol);
}
/** Get Symbol.
@return Symbol for a Unit of Measure */
public String GetUOMSymbol() 
{
return (String)Get_Value("UOMSymbol");
}
/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
public void SetUPC (String UPC)
{
if (UPC != null && UPC.Length > 30)
{
log.Warning("Length > 30 - truncated");
UPC = UPC.Substring(0,30);
}
Set_ValueNoCheck ("UPC", UPC);
}
/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
public String GetUPC() 
{
return (String)Get_Value("UPC");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_ValueNoCheck ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Set Warehouse.
@param WarehouseName Warehouse Name */
public void SetWarehouseName (String WarehouseName)
{
if (WarehouseName == null) throw new ArgumentException ("WarehouseName is mandatory.");
if (WarehouseName.Length > 60)
{
log.Warning("Length > 60 - truncated");
WarehouseName = WarehouseName.Substring(0,60);
}
Set_ValueNoCheck ("WarehouseName", WarehouseName);
}
/** Get Warehouse.
@return Warehouse Name */
public String GetWarehouseName() 
{
return (String)Get_Value("WarehouseName");
}
}

}
