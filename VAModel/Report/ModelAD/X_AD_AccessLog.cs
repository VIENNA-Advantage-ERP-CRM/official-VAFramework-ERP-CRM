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
/** Generated Model for AD_AccessLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AccessLog : PO
{
public X_AD_AccessLog (Context ctx, int AD_AccessLog_ID, Trx trxName) : base (ctx, AD_AccessLog_ID, trxName)
{
/** if (AD_AccessLog_ID == 0)
{
SetAD_AccessLog_ID (0);
}
 */
}
public X_AD_AccessLog (Ctx ctx, int AD_AccessLog_ID, Trx trxName) : base (ctx, AD_AccessLog_ID, trxName)
{
/** if (AD_AccessLog_ID == 0)
{
SetAD_AccessLog_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AccessLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AccessLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AccessLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AccessLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360182L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043393L;
/** AD_Table_ID=717 */
public static int Table_ID;
 // =717;

/** TableName=AD_AccessLog */
public static String Table_Name="AD_AccessLog";

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
StringBuilder sb = new StringBuilder ("X_AD_AccessLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Access Log.
@param AD_AccessLog_ID Log of Access to the System */
public void SetAD_AccessLog_ID (int AD_AccessLog_ID)
{
if (AD_AccessLog_ID < 1) throw new ArgumentException ("AD_AccessLog_ID is mandatory.");
Set_ValueNoCheck ("AD_AccessLog_ID", AD_AccessLog_ID);
}
/** Get Access Log.
@return Log of Access to the System */
public int GetAD_AccessLog_ID() 
{
Object ii = Get_Value("AD_AccessLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_AccessLog_ID().ToString());
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lead.
@param C_Lead_ID Business Lead */
public void SetC_Lead_ID (int C_Lead_ID)
{
if (C_Lead_ID <= 0) Set_Value ("C_Lead_ID", null);
else
Set_Value ("C_Lead_ID", C_Lead_ID);
}
/** Get Lead.
@return Business Lead */
public int GetC_Lead_ID() 
{
Object ii = Get_Value("C_Lead_ID");
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
Set_Value ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Remote Addr.
@param Remote_Addr Remote Address */
public void SetRemote_Addr (String Remote_Addr)
{
if (Remote_Addr != null && Remote_Addr.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Addr = Remote_Addr.Substring(0,60);
}
Set_Value ("Remote_Addr", Remote_Addr);
}
/** Get Remote Addr.
@return Remote Address */
public String GetRemote_Addr() 
{
return (String)Get_Value("Remote_Addr");
}
/** Set Remote Host.
@param Remote_Host Remote host Info */
public void SetRemote_Host (String Remote_Host)
{
if (Remote_Host != null && Remote_Host.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Host = Remote_Host.Substring(0,60);
}
Set_Value ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
}
/** Set Reply.
@param Reply Reply or Answer */
public void SetReply (String Reply)
{
if (Reply != null && Reply.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Reply = Reply.Substring(0,2000);
}
Set_Value ("Reply", Reply);
}
/** Get Reply.
@return Reply or Answer */
public String GetReply() 
{
return (String)Get_Value("Reply");
}
/** Set Text Message.
@param TextMsg Text Message */
public void SetTextMsg (String TextMsg)
{
if (TextMsg != null && TextMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
TextMsg = TextMsg.Substring(0,2000);
}
Set_Value ("TextMsg", TextMsg);
}
/** Get Text Message.
@return Text Message */
public String GetTextMsg() 
{
return (String)Get_Value("TextMsg");
}
}

}
