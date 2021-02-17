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
/** Generated Model for VAM_InventoryLineMP
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_InventoryLineMP : PO
{
public X_VAM_InventoryLineMP (Context ctx, int VAM_InventoryLineMP_ID, Trx trxName) : base (ctx, VAM_InventoryLineMP_ID, trxName)
{
/** if (VAM_InventoryLineMP_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_InventoryLine_ID (0);
SetMovementQty (0.0);
}
 */
}
public X_VAM_InventoryLineMP (Ctx ctx, int VAM_InventoryLineMP_ID, Trx trxName) : base (ctx, VAM_InventoryLineMP_ID, trxName)
{
/** if (VAM_InventoryLineMP_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_InventoryLine_ID (0);
SetMovementQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InventoryLineMP (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InventoryLineMP (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InventoryLineMP (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_InventoryLineMP()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379835L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063046L;
/** VAF_TableView_ID=763 */
public static int Table_ID;
 // =763;

/** TableName=VAM_InventoryLineMP */
public static String Table_Name="VAM_InventoryLineMP";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_VAM_InventoryLineMP[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Phys Inventory Line.
@param VAM_InventoryLine_ID Unique line in an Inventory document */
public void SetVAM_InventoryLine_ID (int VAM_InventoryLine_ID)
{
if (VAM_InventoryLine_ID < 1) throw new ArgumentException ("VAM_InventoryLine_ID is mandatory.");
Set_ValueNoCheck ("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
}
/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
public int GetVAM_InventoryLine_ID() 
{
Object ii = Get_Value("VAM_InventoryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_InventoryLine_ID().ToString());
}
/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty)
{
if (MovementQty == null) throw new ArgumentException ("MovementQty is mandatory.");
Set_Value ("MovementQty", (Decimal?)MovementQty);
}
/** Get Movement Quantity.
@return Quantity of a product moved. */
public Decimal GetMovementQty() 
{
Object bd =Get_Value("MovementQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/// <summary>
///  Set Material Policy Date.
/// </summary>
/// <param name="MMPolicyDate">Time used for LIFO and FIFO Material Policy</param>
public void SetMMPolicyDate(DateTime? MMPolicyDate) { if (MMPolicyDate == null) throw new ArgumentException("MMPolicyDate is mandatory."); Set_ValueNoCheck("MMPolicyDate", (DateTime?)MMPolicyDate); }
/// <summary>
/// Get Material Policy Date.
/// </summary>
/// <returns>Time used for LIFO and FIFO Material Policy</returns>
public DateTime? GetMMPolicyDate() { return (DateTime?)Get_Value("MMPolicyDate"); }

}

}
