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
/** Generated Model for VAB_RFQReplyLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_RFQReplyLine : PO
{
public X_VAB_RFQReplyLine (Context ctx, int VAB_RFQReplyLine_ID, Trx trxName) : base (ctx, VAB_RFQReplyLine_ID, trxName)
{
/** if (VAB_RFQReplyLine_ID == 0)
{
SetVAB_RFQLine_ID (0);
SetVAB_RFQReplyLine_ID (0);
SetVAB_RFQReply_ID (0);
SetIsSelectedWinner (false);
SetIsSelfService (false);
}
 */
}
public X_VAB_RFQReplyLine (Ctx ctx, int VAB_RFQReplyLine_ID, Trx trxName) : base (ctx, VAB_RFQReplyLine_ID, trxName)
{
/** if (VAB_RFQReplyLine_ID == 0)
{
SetVAB_RFQLine_ID (0);
SetVAB_RFQReplyLine_ID (0);
SetVAB_RFQReply_ID (0);
SetIsSelectedWinner (false);
SetIsSelfService (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQReplyLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQReplyLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQReplyLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_RFQReplyLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374993L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058204L;
/** VAF_TableView_ID=673 */
public static int Table_ID;
 // =673;

/** TableName=VAB_RFQReplyLine */
public static String Table_Name="VAB_RFQReplyLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_RFQReplyLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set RfQ Line.
@param VAB_RFQLine_ID Request for Quotation Line */
public void SetVAB_RFQLine_ID (int VAB_RFQLine_ID)
{
if (VAB_RFQLine_ID < 1) throw new ArgumentException ("VAB_RFQLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQLine_ID", VAB_RFQLine_ID);
}
/** Get RfQ Line.
@return Request for Quotation Line */
public int GetVAB_RFQLine_ID() 
{
Object ii = Get_Value("VAB_RFQLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response Line.
@param VAB_RFQReplyLine_ID Request for Quotation Response Line */
public void SetVAB_RFQReplyLine_ID (int VAB_RFQReplyLine_ID)
{
if (VAB_RFQReplyLine_ID < 1) throw new ArgumentException ("VAB_RFQReplyLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQReplyLine_ID", VAB_RFQReplyLine_ID);
}
/** Get RfQ Response Line.
@return Request for Quotation Response Line */
public int GetVAB_RFQReplyLine_ID() 
{
Object ii = Get_Value("VAB_RFQReplyLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Response.
@param VAB_RFQReply_ID Request for Quotation Response from a potential Vendor */
public void SetVAB_RFQReply_ID (int VAB_RFQReply_ID)
{
if (VAB_RFQReply_ID < 1) throw new ArgumentException ("VAB_RFQReply_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQReply_ID", VAB_RFQReply_ID);
}
/** Get RfQ Response.
@return Request for Quotation Response from a potential Vendor */
public int GetVAB_RFQReply_ID() 
{
Object ii = Get_Value("VAB_RFQReply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Work Complete.
@param DateWorkComplete Date when work is (planned to be) complete */
public void SetDateWorkComplete (DateTime? DateWorkComplete)
{
Set_Value ("DateWorkComplete", (DateTime?)DateWorkComplete);
}
/** Get Work Complete.
@return Date when work is (planned to be) complete */
public DateTime? GetDateWorkComplete() 
{
return (DateTime?)Get_Value("DateWorkComplete");
}
/** Set Work Start.
@param DateWorkStart Date when work is (planned to be) started */
public void SetDateWorkStart (DateTime? DateWorkStart)
{
Set_Value ("DateWorkStart", (DateTime?)DateWorkStart);
}
/** Get Work Start.
@return Date when work is (planned to be) started */
public DateTime? GetDateWorkStart() 
{
return (DateTime?)Get_Value("DateWorkStart");
}
/** Set Delivery Days.
@param DeliveryDays Number of Days (planned) until Delivery */
public void SetDeliveryDays (int DeliveryDays)
{
Set_Value ("DeliveryDays", DeliveryDays);
}
/** Get Delivery Days.
@return Number of Days (planned) until Delivery */
public int GetDeliveryDays() 
{
Object ii = Get_Value("DeliveryDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Selected Winner.
@param IsSelectedWinner The resonse is the selected winner */
public void SetIsSelectedWinner (Boolean IsSelectedWinner)
{
Set_Value ("IsSelectedWinner", IsSelectedWinner);
}
/** Get Selected Winner.
@return The resonse is the selected winner */
public Boolean IsSelectedWinner() 
{
Object oo = Get_Value("IsSelectedWinner");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
