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
/** Generated Model for VAM_InvTrf_LineMP
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_InvTrf_LineMP : PO
{
public X_VAM_InvTrf_LineMP (Context ctx, int VAM_InvTrf_LineMP_ID, Trx trxName) : base (ctx, VAM_InvTrf_LineMP_ID, trxName)
{
/** if (VAM_InvTrf_LineMP_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_InvTrf_Line_ID (0);
}
 */
}
public X_VAM_InvTrf_LineMP (Ctx ctx, int VAM_InvTrf_LineMP_ID, Trx trxName) : base (ctx, VAM_InvTrf_LineMP_ID, trxName)
{
/** if (VAM_InvTrf_LineMP_ID == 0)
{
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_InvTrf_Line_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_LineMP (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_LineMP (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_LineMP (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_InvTrf_LineMP()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380196L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063407L;
/** VAF_TableView_ID=764 */
public static int Table_ID;
 // =764;

/** TableName=VAM_InvTrf_LineMP */
public static String Table_Name="VAM_InvTrf_LineMP";

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
StringBuilder sb = new StringBuilder ("X_VAM_InvTrf_LineMP[").Append(Get_ID()).Append("]");
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
/** Set Move Line.
@param VAM_InvTrf_Line_ID Inventory Move document Line */
public void SetVAM_InvTrf_Line_ID (int VAM_InvTrf_Line_ID)
{
if (VAM_InvTrf_Line_ID < 1) throw new ArgumentException ("VAM_InvTrf_Line_ID is mandatory.");
Set_ValueNoCheck ("VAM_InvTrf_Line_ID", VAM_InvTrf_Line_ID);
}
/** Get Move Line.
@return Inventory Move document Line */
public int GetVAM_InvTrf_Line_ID() 
{
Object ii = Get_Value("VAM_InvTrf_Line_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_InvTrf_Line_ID().ToString());
}
/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty)
{
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
