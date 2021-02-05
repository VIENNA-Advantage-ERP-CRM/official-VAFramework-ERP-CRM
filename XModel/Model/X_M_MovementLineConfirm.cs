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
/** Generated Model for VAM_InvTrf_LineConfirm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_InvTrf_LineConfirm : PO
{
public X_VAM_InvTrf_LineConfirm (Context ctx, int VAM_InvTrf_LineConfirm_ID, Trx trxName) : base (ctx, VAM_InvTrf_LineConfirm_ID, trxName)
{
/** if (VAM_InvTrf_LineConfirm_ID == 0)
{
SetConfirmedQty (0.0);
SetDifferenceQty (0.0);
SetVAM_InvTrf_Confirm_ID (0);
SetVAM_InvTrf_LineConfirm_ID (0);
SetVAM_InvTrf_Line_ID (0);
SetProcessed (false);	// N
SetScrappedQty (0.0);
SetTargetQty (0.0);
}
 */
}
public X_VAM_InvTrf_LineConfirm (Ctx ctx, int VAM_InvTrf_LineConfirm_ID, Trx trxName) : base (ctx, VAM_InvTrf_LineConfirm_ID, trxName)
{
/** if (VAM_InvTrf_LineConfirm_ID == 0)
{
SetConfirmedQty (0.0);
SetDifferenceQty (0.0);
SetVAM_InvTrf_Confirm_ID (0);
SetVAM_InvTrf_LineConfirm_ID (0);
SetVAM_InvTrf_Line_ID (0);
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
public X_VAM_InvTrf_LineConfirm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_LineConfirm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_LineConfirm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_InvTrf_LineConfirm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380165L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063376L;
/** VAF_TableView_ID=737 */
public static int Table_ID;
 // =737;

/** TableName=VAM_InvTrf_LineConfirm */
public static String Table_Name="VAM_InvTrf_LineConfirm";

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
StringBuilder sb = new StringBuilder ("X_VAM_InvTrf_LineConfirm[").Append(Get_ID()).Append("]");
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
@param VAM_InventoryLine_ID Unique line in an Inventory document */
public void SetVAM_InventoryLine_ID (int VAM_InventoryLine_ID)
{
if (VAM_InventoryLine_ID <= 0) Set_Value ("VAM_InventoryLine_ID", null);
else
Set_Value ("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
}
/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
public int GetVAM_InventoryLine_ID() 
{
Object ii = Get_Value("VAM_InventoryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Move Confirm.
@param VAM_InvTrf_Confirm_ID Inventory Move Confirmation */
public void SetVAM_InvTrf_Confirm_ID (int VAM_InvTrf_Confirm_ID)
{
if (VAM_InvTrf_Confirm_ID < 1) throw new ArgumentException ("VAM_InvTrf_Confirm_ID is mandatory.");
Set_ValueNoCheck ("VAM_InvTrf_Confirm_ID", VAM_InvTrf_Confirm_ID);
}
/** Get Move Confirm.
@return Inventory Move Confirmation */
public int GetVAM_InvTrf_Confirm_ID() 
{
Object ii = Get_Value("VAM_InvTrf_Confirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAM_InvTrf_Confirm_ID().ToString());
}
/** Set Move Line Confirm.
@param VAM_InvTrf_LineConfirm_ID Inventory Move Line Confirmation */
public void SetVAM_InvTrf_LineConfirm_ID (int VAM_InvTrf_LineConfirm_ID)
{
if (VAM_InvTrf_LineConfirm_ID < 1) throw new ArgumentException ("VAM_InvTrf_LineConfirm_ID is mandatory.");
Set_ValueNoCheck ("VAM_InvTrf_LineConfirm_ID", VAM_InvTrf_LineConfirm_ID);
}
/** Get Move Line Confirm.
@return Inventory Move Line Confirmation */
public int GetVAM_InvTrf_LineConfirm_ID() 
{
Object ii = Get_Value("VAM_InvTrf_LineConfirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Move Line.
@param VAM_InvTrf_Line_ID Inventory Move document Line */
public void SetVAM_InvTrf_Line_ID (int VAM_InvTrf_Line_ID)
{
if (VAM_InvTrf_Line_ID < 1) throw new ArgumentException ("VAM_InvTrf_Line_ID is mandatory.");
Set_Value ("VAM_InvTrf_Line_ID", VAM_InvTrf_Line_ID);
}
/** Get Move Line.
@return Inventory Move document Line */
public int GetVAM_InvTrf_Line_ID() 
{
Object ii = Get_Value("VAM_InvTrf_Line_ID");
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
