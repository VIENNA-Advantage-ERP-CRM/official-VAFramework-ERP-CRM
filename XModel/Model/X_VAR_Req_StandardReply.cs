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
/** Generated Model for VAR_Req_StandardReply
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAR_Req_StandardReply : PO
{
public X_VAR_Req_StandardReply (Context ctx, int VAR_Req_StandardReply_ID, Trx trxName) : base (ctx, VAR_Req_StandardReply_ID, trxName)
{
/** if (VAR_Req_StandardReply_ID == 0)
{
SetName (null);
SetVAR_Req_StandardReply_ID (0);
SetResponseText (null);
}
 */
}
public X_VAR_Req_StandardReply (Ctx ctx, int VAR_Req_StandardReply_ID, Trx trxName) : base (ctx, VAR_Req_StandardReply_ID, trxName)
{
/** if (VAR_Req_StandardReply_ID == 0)
{
SetName (null);
SetVAR_Req_StandardReply_ID (0);
SetResponseText (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_StandardReply (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_StandardReply (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_StandardReply (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAR_Req_StandardReply()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383456L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066667L;
/** VAF_TableView_ID=775 */
public static int Table_ID;
 // =775;

/** TableName=VAR_Req_StandardReply */
public static String Table_Name="VAR_Req_StandardReply";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_VAR_Req_StandardReply[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Standard Response.
@param VAR_Req_StandardReply_ID Request Standard Response */
public void SetVAR_Req_StandardReply_ID (int VAR_Req_StandardReply_ID)
{
if (VAR_Req_StandardReply_ID < 1) throw new ArgumentException ("VAR_Req_StandardReply_ID is mandatory.");
Set_ValueNoCheck ("VAR_Req_StandardReply_ID", VAR_Req_StandardReply_ID);
}
/** Get Standard Response.
@return Request Standard Response */
public int GetVAR_Req_StandardReply_ID() 
{
Object ii = Get_Value("VAR_Req_StandardReply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Response Text.
@param ResponseText Request Response Text */
public void SetResponseText (String ResponseText)
{
if (ResponseText == null) throw new ArgumentException ("ResponseText is mandatory.");
if (ResponseText.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ResponseText = ResponseText.Substring(0,2000);
}
Set_Value ("ResponseText", ResponseText);
}
/** Get Response Text.
@return Request Response Text */
public String GetResponseText() 
{
return (String)Get_Value("ResponseText");
}
}

}
