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
/** Generated Model for M_MovementLineConfirm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_MovementLineConfirm : PO
{
public X_M_MovementLineConfirm (Context ctx, int M_MovementLineConfirm_ID, Trx trxName) : base (ctx, M_MovementLineConfirm_ID, trxName)
{
/** if (M_MovementLineConfirm_ID == 0)
{
SetConfirmedQty (0.0);
SetDifferenceQty (0.0);
SetM_MovementConfirm_ID (0);
SetM_MovementLineConfirm_ID (0);
SetM_MovementLine_ID (0);
SetProcessed (false);	// N
SetScrappedQty (0.0);
SetTargetQty (0.0);
}
 */
}
public X_M_MovementLineConfirm (Ctx ctx, int M_MovementLineConfirm_ID, Trx trxName) : base (ctx, M_MovementLineConfirm_ID, trxName)
{
/** if (M_MovementLineConfirm_ID == 0)
{
SetConfirmedQty (0.0);
SetDifferenceQty (0.0);
SetM_MovementConfirm_ID (0);
SetM_MovementLineConfirm_ID (0);
SetM_MovementLine_ID (0);
SetProcessed (false);	// N
SetScrappedQty (0.0);
SetTargetQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_MovementLineConfirm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_MovementLineConfirm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_MovementLineConfirm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_MovementLineConfirm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380165L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063376L;
/** AD_Table_ID=737 */
public static int Table_ID;
 // =737;

/** TableName=M_MovementLineConfirm */
public static String Table_Name="M_MovementLineConfirm";

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
StringBuilder sb = new StringBuilder ("X_M_MovementLineConfirm[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Confirmed Quantity.
@param ConfirmedQty Confirmation of a received quantity */
public void SetConfirmedQty (Decimal? ConfirmedQty)
{
if (ConfirmedQty == null) throw new ArgumentException ("ConfirmedQty is mandatory.");
Set_Value ("ConfirmedQty", (Decimal?)ConfirmedQty);
}
/** Get Confirmed Quantity.
@return Confirmation of a received quantity */
public Decimal GetConfirmedQty() 
{
Object bd =Get_Value("ConfirmedQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Difference.
@param DifferenceQty Difference Quantity */
public void SetDifferenceQty (Decimal? DifferenceQty)
{
if (DifferenceQty == null) throw new ArgumentException ("DifferenceQty is mandatory.");
Set_Value ("DifferenceQty", (Decimal?)DifferenceQty);
}
/** Get Difference.
@return Difference Quantity */
public Decimal GetDifferenceQty() 
{
Object bd =Get_Value("DifferenceQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Move Confirm.
@param M_MovementConfirm_ID Inventory Move Confirmation */
public void SetM_MovementConfirm_ID (int M_MovementConfirm_ID)
{
if (M_MovementConfirm_ID < 1) throw new ArgumentException ("M_MovementConfirm_ID is mandatory.");
Set_ValueNoCheck ("M_MovementConfirm_ID", M_MovementConfirm_ID);
}
/** Get Move Confirm.
@return Inventory Move Confirmation */
public int GetM_MovementConfirm_ID() 
{
Object ii = Get_Value("M_MovementConfirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_MovementConfirm_ID().ToString());
}
/** Set Move Line Confirm.
@param M_MovementLineConfirm_ID Inventory Move Line Confirmation */
public void SetM_MovementLineConfirm_ID (int M_MovementLineConfirm_ID)
{
if (M_MovementLineConfirm_ID < 1) throw new ArgumentException ("M_MovementLineConfirm_ID is mandatory.");
Set_ValueNoCheck ("M_MovementLineConfirm_ID", M_MovementLineConfirm_ID);
}
/** Get Move Line Confirm.
@return Inventory Move Line Confirmation */
public int GetM_MovementLineConfirm_ID() 
{
Object ii = Get_Value("M_MovementLineConfirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Move Line.
@param M_MovementLine_ID Inventory Move document Line */
public void SetM_MovementLine_ID (int M_MovementLine_ID)
{
if (M_MovementLine_ID < 1) throw new ArgumentException ("M_MovementLine_ID is mandatory.");
Set_Value ("M_MovementLine_ID", M_MovementLine_ID);
}
/** Get Move Line.
@return Inventory Move document Line */
public int GetM_MovementLine_ID() 
{
Object ii = Get_Value("M_MovementLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Scrapped Quantity.
@param ScrappedQty The Quantity scrapped due to QA issues */
public void SetScrappedQty (Decimal? ScrappedQty)
{
if (ScrappedQty == null) throw new ArgumentException ("ScrappedQty is mandatory.");
Set_Value ("ScrappedQty", (Decimal?)ScrappedQty);
}
/** Get Scrapped Quantity.
@return The Quantity scrapped due to QA issues */
public Decimal GetScrappedQty() 
{
Object bd =Get_Value("ScrappedQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Target Quantity.
@param TargetQty Target Movement Quantity */
public void SetTargetQty (Decimal? TargetQty)
{
if (TargetQty == null) throw new ArgumentException ("TargetQty is mandatory.");
Set_Value ("TargetQty", (Decimal?)TargetQty);
}
/** Get Target Quantity.
@return Target Movement Quantity */
public Decimal GetTargetQty() 
{
Object bd =Get_Value("TargetQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
