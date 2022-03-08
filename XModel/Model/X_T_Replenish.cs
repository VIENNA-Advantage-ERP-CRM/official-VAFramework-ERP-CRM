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
/** Generated Model for T_Replenish
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_Replenish : PO
{
public X_T_Replenish (Context ctx, int T_Replenish_ID, Trx trxName) : base (ctx, T_Replenish_ID, trxName)
{
/** if (T_Replenish_ID == 0)
{
SetAD_PInstance_ID (0);
SetLevel_Max (0.0);
SetLevel_Min (0.0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetOrder_Min (0.0);
SetOrder_Pack (0.0);
SetQtyOnHand (0.0);
SetQtyOrdered (0.0);
SetQtyReserved (0.0);
SetQtyToOrder (0.0);
SetReplenishType (null);
}
 */
}
public X_T_Replenish (Ctx ctx, int T_Replenish_ID, Trx trxName) : base (ctx, T_Replenish_ID, trxName)
{
/** if (T_Replenish_ID == 0)
{
SetAD_PInstance_ID (0);
SetLevel_Max (0.0);
SetLevel_Min (0.0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetOrder_Min (0.0);
SetOrder_Pack (0.0);
SetQtyOnHand (0.0);
SetQtyOrdered (0.0);
SetQtyReserved (0.0);
SetQtyToOrder (0.0);
SetReplenishType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Replenish (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Replenish (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Replenish (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_Replenish()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384318L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067529L;
/** AD_Table_ID=364 */
public static int Table_ID;
 // =364;

/** TableName=T_Replenish */
public static String Table_Name="T_Replenish";

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
StringBuilder sb = new StringBuilder ("X_T_Replenish[").Append(Get_ID()).Append("]");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_PInstance_ID().ToString());
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID <= 0) Set_Value ("C_DocType_ID", null);
else
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Maximum Level.
@param Level_Max Maximum Inventory level for this product */
public void SetLevel_Max (Decimal? Level_Max)
{
if (Level_Max == null) throw new ArgumentException ("Level_Max is mandatory.");
Set_Value ("Level_Max", (Decimal?)Level_Max);
}
/** Get Maximum Level.
@return Maximum Inventory level for this product */
public Decimal GetLevel_Max() 
{
Object bd =Get_Value("Level_Max");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Minimum Level.
@param Level_Min Minimum Inventory level for this product */
public void SetLevel_Min (Decimal? Level_Min)
{
if (Level_Min == null) throw new ArgumentException ("Level_Min is mandatory.");
Set_Value ("Level_Min", (Decimal?)Level_Min);
}
/** Get Minimum Level.
@return Minimum Inventory level for this product */
public Decimal GetLevel_Min() 
{
Object bd =Get_Value("Level_Min");
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

/** M_WarehouseSource_ID AD_Reference_ID=197 */
public static int M_WAREHOUSESOURCE_ID_AD_Reference_ID=197;
/** Set Source Warehouse.
@param M_WarehouseSource_ID Optional Warehouse to replenish from */
public void SetM_WarehouseSource_ID (int M_WarehouseSource_ID)
{
if (M_WarehouseSource_ID <= 0) Set_Value ("M_WarehouseSource_ID", null);
else
Set_Value ("M_WarehouseSource_ID", M_WarehouseSource_ID);
}
/** Get Source Warehouse.
@return Optional Warehouse to replenish from */
public int GetM_WarehouseSource_ID() 
{
Object ii = Get_Value("M_WarehouseSource_ID");
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
/** Set Minimum Order Qty.
@param Order_Min Minimum order quantity in UOM */
public void SetOrder_Min (Decimal? Order_Min)
{
if (Order_Min == null) throw new ArgumentException ("Order_Min is mandatory.");
Set_Value ("Order_Min", (Decimal?)Order_Min);
}
/** Get Minimum Order Qty.
@return Minimum order quantity in UOM */
public Decimal GetOrder_Min() 
{
Object bd =Get_Value("Order_Min");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Order Pack Qty.
@param Order_Pack Package order size in UOM (e.g. order set of 5 units) */
public void SetOrder_Pack (Decimal? Order_Pack)
{
if (Order_Pack == null) throw new ArgumentException ("Order_Pack is mandatory.");
Set_Value ("Order_Pack", (Decimal?)Order_Pack);
}
/** Get Order Pack Qty.
@return Package order size in UOM (e.g. order set of 5 units) */
public Decimal GetOrder_Pack() 
{
Object bd =Get_Value("Order_Pack");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set On Hand Quantity.
@param QtyOnHand On Hand Quantity */
public void SetQtyOnHand (Decimal? QtyOnHand)
{
if (QtyOnHand == null) throw new ArgumentException ("QtyOnHand is mandatory.");
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
/** Set Ordered Quantity.
@param QtyOrdered Ordered Quantity */
public void SetQtyOrdered (Decimal? QtyOrdered)
{
if (QtyOrdered == null) throw new ArgumentException ("QtyOrdered is mandatory.");
Set_Value ("QtyOrdered", (Decimal?)QtyOrdered);
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
if (QtyReserved == null) throw new ArgumentException ("QtyReserved is mandatory.");
Set_Value ("QtyReserved", (Decimal?)QtyReserved);
}
/** Get Quantity Reserved.
@return Quantity Reserved */
public Decimal GetQtyReserved() 
{
Object bd =Get_Value("QtyReserved");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity to Order.
@param QtyToOrder Quantity to Order */
public void SetQtyToOrder (Decimal? QtyToOrder)
{
if (QtyToOrder == null) throw new ArgumentException ("QtyToOrder is mandatory.");
Set_Value ("QtyToOrder", (Decimal?)QtyToOrder);
}
/** Get Quantity to Order.
@return Quantity to Order */
public Decimal GetQtyToOrder() 
{
Object bd =Get_Value("QtyToOrder");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** ReplenishType AD_Reference_ID=164 */
public static int REPLENISHTYPE_AD_Reference_ID=164;
/** Manual = 0 */
public static String REPLENISHTYPE_Manual = "0";
/** Reorder below Minimum Level = 1 */
public static String REPLENISHTYPE_ReorderBelowMinimumLevel = "1";
/** Maintain Maximum Level = 2 */
public static String REPLENISHTYPE_MaintainMaximumLevel = "2";
/** Custom = 9 */
public static String REPLENISHTYPE_Custom = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsReplenishTypeValid (String test)
{
return test.Equals("0") || test.Equals("1") || test.Equals("2") || test.Equals("9");
}
/** Set Replenishment Type.
@param ReplenishType Method for re-ordering a product */
public void SetReplenishType (String ReplenishType)
{
if (ReplenishType == null) throw new ArgumentException ("ReplenishType is mandatory");
if (!IsReplenishTypeValid(ReplenishType))
throw new ArgumentException ("ReplenishType Invalid value - " + ReplenishType + " - Reference_ID=164 - 0 - 1 - 2 - 9");
if (ReplenishType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ReplenishType = ReplenishType.Substring(0,1);
}
Set_Value ("ReplenishType", ReplenishType);
}
/** Get Replenishment Type.
@return Method for re-ordering a product */
public String GetReplenishType() 
{
return (String)Get_Value("ReplenishType");
}

/** ReplenishmentCreate AD_Reference_ID=329 */
public static int REPLENISHMENTCREATE_AD_Reference_ID=329;
/** Inventory Move = MMM */
public static String REPLENISHMENTCREATE_InventoryMove = "MMM";
/** Purchase Order = POO */
public static String REPLENISHMENTCREATE_PurchaseOrder = "POO";
/** Requisition = POR */
public static String REPLENISHMENTCREATE_Requisition = "POR";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsReplenishmentCreateValid (String test)
{
return test == null || test.Equals("MMM") || test.Equals("POO") || test.Equals("POR");
}
/** Set Create.
@param ReplenishmentCreate Create from Replenishment */
public void SetReplenishmentCreate (String ReplenishmentCreate)
{
if (!IsReplenishmentCreateValid(ReplenishmentCreate))
throw new ArgumentException ("ReplenishmentCreate Invalid value - " + ReplenishmentCreate + " - Reference_ID=329 - MMM - POO - POR");
if (ReplenishmentCreate != null && ReplenishmentCreate.Length > 3)
{
log.Warning("Length > 3 - truncated");
ReplenishmentCreate = ReplenishmentCreate.Substring(0,3);
}
Set_Value ("ReplenishmentCreate", ReplenishmentCreate);
}
/** Get Create.
@return Create from Replenishment */
public String GetReplenishmentCreate() 
{
return (String)Get_Value("ReplenishmentCreate");
}
}

}
