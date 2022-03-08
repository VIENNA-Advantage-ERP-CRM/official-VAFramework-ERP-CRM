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
/** Generated Model for M_StorageDetail
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_StorageDetail : PO
{
public X_M_StorageDetail (Context ctx, int M_StorageDetail_ID, Trx trxName) : base (ctx, M_StorageDetail_ID, trxName)
{
/** if (M_StorageDetail_ID == 0)
{
SetM_AttributeSetInstance_ID (0);
SetM_Locator_ID (0);
SetM_Product_ID (0);
SetQty (0.0);
SetQtyType (null);
}
 */
}
public X_M_StorageDetail (Ctx ctx, int M_StorageDetail_ID, Trx trxName) : base (ctx, M_StorageDetail_ID, trxName)
{
/** if (M_StorageDetail_ID == 0)
{
SetM_AttributeSetInstance_ID (0);
SetM_Locator_ID (0);
SetM_Product_ID (0);
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
public X_M_StorageDetail (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_StorageDetail (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_StorageDetail (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_StorageDetail()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27581088221905L;
/** Last Updated Timestamp 3/1/2011 12:31:45 PM */
public static long updatedMS = 1298962905116L;
/** AD_Table_ID=2160 */
public static int Table_ID;
 // =2160;

/** TableName=M_StorageDetail */
public static String Table_Name="M_StorageDetail";

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
StringBuilder sb = new StringBuilder ("X_M_StorageDetail[").Append(Get_ID()).Append("]");
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
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID < 1) throw new ArgumentException ("M_Locator_ID is mandatory.");
Set_ValueNoCheck ("M_Locator_ID", M_Locator_ID);
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

/** QtyType AD_Reference_ID=533 */
public static int QTYTYPE_AD_Reference_ID=533;
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
