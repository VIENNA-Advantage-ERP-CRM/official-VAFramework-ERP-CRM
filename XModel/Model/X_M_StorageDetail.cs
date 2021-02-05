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
/** Generated Model for VAM_StorageDetail
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_StorageDetail : PO
{
public X_VAM_StorageDetail (Context ctx, int VAM_StorageDetail_ID, Trx trxName) : base (ctx, VAM_StorageDetail_ID, trxName)
{
/** if (VAM_StorageDetail_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_Locator_ID (0);
SetVAM_Product_ID (0);
SetQty (0.0);
SetQtyType (null);
}
 */
}
public X_VAM_StorageDetail (Ctx ctx, int VAM_StorageDetail_ID, Trx trxName) : base (ctx, VAM_StorageDetail_ID, trxName)
{
/** if (VAM_StorageDetail_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_Locator_ID (0);
SetVAM_Product_ID (0);
SetQty (0.0);
SetQtyType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_StorageDetail (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_StorageDetail (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_StorageDetail (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_StorageDetail()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27581088221905L;
/** Last Updated Timestamp 3/1/2011 12:31:45 PM */
public static long updatedMS = 1298962905116L;
/** VAF_TableView_ID=2160 */
public static int Table_ID;
 // =2160;

/** TableName=VAM_StorageDetail */
public static String Table_Name="VAM_StorageDetail";

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
StringBuilder sb = new StringBuilder ("X_VAM_StorageDetail[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Date last inventory count.
@param DateLastInventory Date of Last Inventory Count */
public void SetDateLastInventory (DateTime? DateLastInventory)
{
Set_ValueNoCheck ("DateLastInventory", (DateTime?)DateLastInventory);
}
/** Get Date last inventory count.
@return Date of Last Inventory Count */
public DateTime? GetDateLastInventory() 
{
return (DateTime?)Get_Value("DateLastInventory");
}
/** Set Attribute Set Instance.
@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
public void SetVAM_PFeature_SetInstance_ID (int VAM_PFeature_SetInstance_ID)
{
if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException ("VAM_PFeature_SetInstance_ID is mandatory.");
Set_ValueNoCheck ("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetVAM_PFeature_SetInstance_ID() 
{
Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param VAM_Locator_ID Warehouse Locator */
public void SetVAM_Locator_ID (int VAM_Locator_ID)
{
if (VAM_Locator_ID < 1) throw new ArgumentException ("VAM_Locator_ID is mandatory.");
Set_ValueNoCheck ("VAM_Locator_ID", VAM_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetVAM_Locator_ID() 
{
Object ii = Get_Value("VAM_Locator_ID");
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
Set_ValueNoCheck ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** QtyType VAF_Control_Ref_ID=533 */
public static int QTYTYPE_VAF_Control_Ref_ID=533;
/** Allocated = A */
public static String QTYTYPE_Allocated = "A";
/** Dedicated = D */
public static String QTYTYPE_Dedicated = "D";
/** Expected = E */
public static String QTYTYPE_Expected = "E";
/** On Hand = H */
public static String QTYTYPE_OnHand = "H";
/** Ordered = O */
public static String QTYTYPE_Ordered = "O";
/** Reserved = R */
public static String QTYTYPE_Reserved = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsQtyTypeValid (String test)
{
return test.Equals("A") || test.Equals("D") || test.Equals("E") || test.Equals("H") || test.Equals("O") || test.Equals("R");
}
/** Set Quantity Type.
@param QtyType Quantity Type */
public void SetQtyType (String QtyType)
{
if (QtyType == null) throw new ArgumentException ("QtyType is mandatory");
if (!IsQtyTypeValid(QtyType))
throw new ArgumentException ("QtyType Invalid value - " + QtyType + " - Reference_ID=533 - A - D - E - H - O - R");
if (QtyType.Length > 1)
{
log.Warning("Length > 1 - truncated");
QtyType = QtyType.Substring(0,1);
}
Set_ValueNoCheck ("QtyType", QtyType);
}
/** Get Quantity Type.
@return Quantity Type */
public String GetQtyType() 
{
return (String)Get_Value("QtyType");
}
}

}
