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
/** Generated Model for I_Inventory
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_Inventory : PO
{
public X_I_Inventory (Context ctx, int I_Inventory_ID, Trx trxName) : base (ctx, I_Inventory_ID, trxName)
{
/** if (I_Inventory_ID == 0)
{
SetI_Inventory_ID (0);
SetI_IsImported (null);	// N
}
 */
}
public X_I_Inventory (Ctx ctx, int I_Inventory_ID, Trx trxName) : base (ctx, I_Inventory_ID, trxName)
{
/** if (I_Inventory_ID == 0)
{
SetI_Inventory_ID (0);
SetI_IsImported (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Inventory (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Inventory (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Inventory (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_Inventory()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377187L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060398L;
/** AD_Table_ID=572 */
public static int Table_ID;
 // =572;

/** TableName=I_Inventory */
public static String Table_Name="I_Inventory";

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
StringBuilder sb = new StringBuilder ("X_I_Inventory[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Import Error Message.
@param I_ErrorMsg Messages generated from import process */
public void SetI_ErrorMsg (String I_ErrorMsg)
{
if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
I_ErrorMsg = I_ErrorMsg.Substring(0,2000);
}
Set_Value ("I_ErrorMsg", I_ErrorMsg);
}
/** Get Import Error Message.
@return Messages generated from import process */
public String GetI_ErrorMsg() 
{
return (String)Get_Value("I_ErrorMsg");
}
/** Set Import Inventory.
@param I_Inventory_ID Import Inventory Transactions */
public void SetI_Inventory_ID (int I_Inventory_ID)
{
if (I_Inventory_ID < 1) throw new ArgumentException ("I_Inventory_ID is mandatory.");
Set_ValueNoCheck ("I_Inventory_ID", I_Inventory_ID);
}
/** Get Import Inventory.
@return Import Inventory Transactions */
public int GetI_Inventory_ID() 
{
Object ii = Get_Value("I_Inventory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetI_Inventory_ID().ToString());
}

/** I_IsImported AD_Reference_ID=420 */
public static int I_ISIMPORTED_AD_Reference_ID=420;
/** Error = E */
public static String I_ISIMPORTED_Error = "E";
/** No = N */
public static String I_ISIMPORTED_No = "N";
/** Yes = Y */
public static String I_ISIMPORTED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsI_IsImportedValid (String test)
{
return test.Equals("E") || test.Equals("N") || test.Equals("Y");
}
/** Set Imported.
@param I_IsImported Has this import been processed */
public void SetI_IsImported (String I_IsImported)
{
if (I_IsImported == null) throw new ArgumentException ("I_IsImported is mandatory");
if (!IsI_IsImportedValid(I_IsImported))
throw new ArgumentException ("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
if (I_IsImported.Length > 1)
{
log.Warning("Length > 1 - truncated");
I_IsImported = I_IsImported.Substring(0,1);
}
Set_Value ("I_IsImported", I_IsImported);
}
/** Get Imported.
@return Has this import been processed */
public String GetI_IsImported() 
{
return (String)Get_Value("I_IsImported");
}
/** Set Locator Key.
@param LocatorValue Key of the Warehouse Locator */
public void SetLocatorValue (String LocatorValue)
{
if (LocatorValue != null && LocatorValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
LocatorValue = LocatorValue.Substring(0,40);
}
Set_Value ("LocatorValue", LocatorValue);
}
/** Get Locator Key.
@return Key of the Warehouse Locator */
public String GetLocatorValue() 
{
return (String)Get_Value("LocatorValue");
}
/** Set Lot No.
@param Lot Lot number (alphanumeric) */
public void SetLot (String Lot)
{
if (Lot != null && Lot.Length > 20)
{
log.Warning("Length > 20 - truncated");
Lot = Lot.Substring(0,20);
}
Set_Value ("Lot", Lot);
}
/** Get Lot No.
@return Lot number (alphanumeric) */
public String GetLot() 
{
return (String)Get_Value("Lot");
}
/** Set Phys Inventory Line.
@param M_InventoryLine_ID Unique line in an Inventory document */
public void SetM_InventoryLine_ID (int M_InventoryLine_ID)
{
if (M_InventoryLine_ID <= 0) Set_Value ("M_InventoryLine_ID", null);
else
Set_Value ("M_InventoryLine_ID", M_InventoryLine_ID);
}
/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
public int GetM_InventoryLine_ID() 
{
Object ii = Get_Value("M_InventoryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Phys.Inventory.
@param M_Inventory_ID Parameters for a Physical Inventory */
public void SetM_Inventory_ID (int M_Inventory_ID)
{
if (M_Inventory_ID <= 0) Set_Value ("M_Inventory_ID", null);
else
Set_Value ("M_Inventory_ID", M_Inventory_ID);
}
/** Get Phys.Inventory.
@return Parameters for a Physical Inventory */
public int GetM_Inventory_ID() 
{
Object ii = Get_Value("M_Inventory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID <= 0) Set_Value ("M_Locator_ID", null);
else
Set_Value ("M_Locator_ID", M_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() 
{
Object ii = Get_Value("M_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
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
if (M_Warehouse_ID <= 0) Set_Value ("M_Warehouse_ID", null);
else
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
/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate)
{
Set_Value ("MovementDate", (DateTime?)MovementDate);
}
/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() 
{
return (DateTime?)Get_Value("MovementDate");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Quantity book.
@param QtyBook Book Quantity */
public void SetQtyBook (Decimal? QtyBook)
{
Set_Value ("QtyBook", (Decimal?)QtyBook);
}
/** Get Quantity book.
@return Book Quantity */
public Decimal GetQtyBook() 
{
Object bd =Get_Value("QtyBook");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity count.
@param QtyCount Counted Quantity */
public void SetQtyCount (Decimal? QtyCount)
{
Set_Value ("QtyCount", (Decimal?)QtyCount);
}
/** Get Quantity count.
@return Counted Quantity */
public Decimal GetQtyCount() 
{
Object bd =Get_Value("QtyCount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Serial No.
@param SerNo Product Serial Number */
public void SetSerNo (String SerNo)
{
if (SerNo != null && SerNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
SerNo = SerNo.Substring(0,20);
}
Set_Value ("SerNo", SerNo);
}
/** Get Serial No.
@return Product Serial Number */
public String GetSerNo() 
{
return (String)Get_Value("SerNo");
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
Set_Value ("UPC", UPC);
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
if (Value != null && Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Set Warehouse Key.
@param WarehouseValue Key of the Warehouse */
public void SetWarehouseValue (String WarehouseValue)
{
if (WarehouseValue != null && WarehouseValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
WarehouseValue = WarehouseValue.Substring(0,40);
}
Set_Value ("WarehouseValue", WarehouseValue);
}
/** Get Warehouse Key.
@return Key of the Warehouse */
public String GetWarehouseValue() 
{
return (String)Get_Value("WarehouseValue");
}
/** Set Aisle (X).
@param X X dimension, e.g., Aisle */
public void SetX (String X)
{
if (X != null && X.Length > 60)
{
log.Warning("Length > 60 - truncated");
X = X.Substring(0,60);
}
Set_Value ("X", X);
}
/** Get Aisle (X).
@return X dimension, e.g., Aisle */
public String GetX() 
{
return (String)Get_Value("X");
}
/** Set Bin (Y).
@param Y Y dimension, e.g., Bin */
public void SetY (String Y)
{
if (Y != null && Y.Length > 60)
{
log.Warning("Length > 60 - truncated");
Y = Y.Substring(0,60);
}
Set_Value ("Y", Y);
}
/** Get Bin (Y).
@return Y dimension, e.g., Bin */
public String GetY() 
{
return (String)Get_Value("Y");
}
/** Set Level (Z).
@param Z Z dimension, e.g., Level */
public void SetZ (String Z)
{
if (Z != null && Z.Length > 60)
{
log.Warning("Length > 60 - truncated");
Z = Z.Substring(0,60);
}
Set_Value ("Z", Z);
}
/** Get Level (Z).
@return Z dimension, e.g., Level */
public String GetZ() 
{
return (String)Get_Value("Z");
}
}

}
