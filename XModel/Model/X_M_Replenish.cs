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
/** Generated Model for M_Replenish
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Replenish : PO
{
public X_M_Replenish (Context ctx, int M_Replenish_ID, Trx trxName) : base (ctx, M_Replenish_ID, trxName)
{
/** if (M_Replenish_ID == 0)
{
SetLevel_Max (0.0);
SetLevel_Min (0.0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetReplenishType (null);
}
 */
}
public X_M_Replenish (Ctx ctx, int M_Replenish_ID, Trx trxName) : base (ctx, M_Replenish_ID, trxName)
{
/** if (M_Replenish_ID == 0)
{
SetLevel_Max (0.0);
SetLevel_Min (0.0);
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetReplenishType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Replenish (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Replenish (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Replenish (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Replenish()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381089L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064300L;
/** AD_Table_ID=249 */
public static int Table_ID;
 // =249;

/** TableName=M_Replenish */
public static String Table_Name="M_Replenish";

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
StringBuilder sb = new StringBuilder ("X_M_Replenish[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_Product_ID().ToString());
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
}

}
