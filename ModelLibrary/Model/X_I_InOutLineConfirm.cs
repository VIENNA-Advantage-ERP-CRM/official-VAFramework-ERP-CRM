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
/** Generated Model for I_InOutLineConfirm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_InOutLineConfirm : PO
{
public X_I_InOutLineConfirm (Context ctx, int I_InOutLineConfirm_ID, Trx trxName) : base (ctx, I_InOutLineConfirm_ID, trxName)
{
/** if (I_InOutLineConfirm_ID == 0)
{
SetI_InOutLineConfirm_ID (0);
SetI_IsImported (null);	// N
SetProcessed (false);	// N
}
 */
}
public X_I_InOutLineConfirm (Ctx ctx, int I_InOutLineConfirm_ID, Trx trxName) : base (ctx, I_InOutLineConfirm_ID, trxName)
{
/** if (I_InOutLineConfirm_ID == 0)
{
SetI_InOutLineConfirm_ID (0);
SetI_IsImported (null);	// N
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_InOutLineConfirm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_InOutLineConfirm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_InOutLineConfirm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_InOutLineConfirm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377155L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060366L;
/** AD_Table_ID=740 */
public static int Table_ID;
 // =740;

/** TableName=I_InOutLineConfirm */
public static String Table_Name="I_InOutLineConfirm";

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
StringBuilder sb = new StringBuilder ("X_I_InOutLineConfirm[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Confirmation No.
@param ConfirmationNo Confirmation Number */
public void SetConfirmationNo (String ConfirmationNo)
{
if (ConfirmationNo != null && ConfirmationNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
ConfirmationNo = ConfirmationNo.Substring(0,20);
}
Set_Value ("ConfirmationNo", ConfirmationNo);
}
/** Get Confirmation No.
@return Confirmation Number */
public String GetConfirmationNo() 
{
return (String)Get_Value("ConfirmationNo");
}
/** Set Confirmed Quantity.
@param ConfirmedQty Confirmation of a received quantity */
public void SetConfirmedQty (Decimal? ConfirmedQty)
{
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
/** Set Ship/Receipt Confirmation Import Line.
@param I_InOutLineConfirm_ID Material Shipment or Receipt Confirmation Import Line */
public void SetI_InOutLineConfirm_ID (int I_InOutLineConfirm_ID)
{
if (I_InOutLineConfirm_ID < 1) throw new ArgumentException ("I_InOutLineConfirm_ID is mandatory.");
Set_ValueNoCheck ("I_InOutLineConfirm_ID", I_InOutLineConfirm_ID);
}
/** Get Ship/Receipt Confirmation Import Line.
@return Material Shipment or Receipt Confirmation Import Line */
public int GetI_InOutLineConfirm_ID() 
{
Object ii = Get_Value("I_InOutLineConfirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetI_InOutLineConfirm_ID().ToString());
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
/** Set Ship/Receipt Confirmation Line.
@param M_InOutLineConfirm_ID Material Shipment or Receipt Confirmation Line */
public void SetM_InOutLineConfirm_ID (int M_InOutLineConfirm_ID)
{
if (M_InOutLineConfirm_ID <= 0) Set_Value ("M_InOutLineConfirm_ID", null);
else
Set_Value ("M_InOutLineConfirm_ID", M_InOutLineConfirm_ID);
}
/** Get Ship/Receipt Confirmation Line.
@return Material Shipment or Receipt Confirmation Line */
public int GetM_InOutLineConfirm_ID() 
{
Object ii = Get_Value("M_InOutLineConfirm_ID");
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
/** Set Scrapped Quantity.
@param ScrappedQty The Quantity scrapped due to QA issues */
public void SetScrappedQty (Decimal? ScrappedQty)
{
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
}

}
