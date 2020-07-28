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
/** Generated Model for CM_BroadcastServer
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_BroadcastServer : PO
{
public X_CM_BroadcastServer (Context ctx, int CM_BroadcastServer_ID, Trx trxName) : base (ctx, CM_BroadcastServer_ID, trxName)
{
/** if (CM_BroadcastServer_ID == 0)
{
SetCM_BroadcastServer_ID (0);
SetIP_Address (null);
SetName (null);
}
 */
}
public X_CM_BroadcastServer (Ctx ctx, int CM_BroadcastServer_ID, Trx trxName) : base (ctx, CM_BroadcastServer_ID, trxName)
{
/** if (CM_BroadcastServer_ID == 0)
{
SetCM_BroadcastServer_ID (0);
SetIP_Address (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_BroadcastServer (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_BroadcastServer (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_BroadcastServer (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_BroadcastServer()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368363L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051574L;
/** AD_Table_ID=893 */
public static int Table_ID;
 // =893;

/** TableName=CM_BroadcastServer */
public static String Table_Name="CM_BroadcastServer";

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
StringBuilder sb = new StringBuilder ("X_CM_BroadcastServer[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Broadcast Server.
@param CM_BroadcastServer_ID Web Broadcast Server */
public void SetCM_BroadcastServer_ID (int CM_BroadcastServer_ID)
{
if (CM_BroadcastServer_ID < 1) throw new ArgumentException ("CM_BroadcastServer_ID is mandatory.");
Set_ValueNoCheck ("CM_BroadcastServer_ID", CM_BroadcastServer_ID);
}
/** Get Broadcast Server.
@return Web Broadcast Server */
public int GetCM_BroadcastServer_ID() 
{
Object ii = Get_Value("CM_BroadcastServer_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID <= 0) Set_Value ("CM_WebProject_ID", null);
else
Set_Value ("CM_WebProject_ID", CM_WebProject_ID);
}
/** Get Web Project.
@return A web project is the main data container for Containers, URLs, Ads, Media etc. */
public int GetCM_WebProject_ID() 
{
Object ii = Get_Value("CM_WebProject_ID");
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
/** Set IP Address.
@param IP_Address Defines the IP address to transfer data to */
public void SetIP_Address (String IP_Address)
{
if (IP_Address == null) throw new ArgumentException ("IP_Address is mandatory.");
if (IP_Address.Length > 20)
{
log.Warning("Length > 20 - truncated");
IP_Address = IP_Address.Substring(0,20);
}
Set_Value ("IP_Address", IP_Address);
}
/** Get IP Address.
@return Defines the IP address to transfer data to */
public String GetIP_Address() 
{
return (String)Get_Value("IP_Address");
}
/** Set Last Synchronized.
@param LastSynchronized Date when last synchronized */
public void SetLastSynchronized (DateTime? LastSynchronized)
{
Set_Value ("LastSynchronized", (DateTime?)LastSynchronized);
}
/** Get Last Synchronized.
@return Date when last synchronized */
public DateTime? GetLastSynchronized() 
{
return (DateTime?)Get_Value("LastSynchronized");
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
}

}
