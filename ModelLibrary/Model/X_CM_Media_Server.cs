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
/** Generated Model for CM_Media_Server
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Media_Server : PO
{
public X_CM_Media_Server (Context ctx, int CM_Media_Server_ID, Trx trxName) : base (ctx, CM_Media_Server_ID, trxName)
{
/** if (CM_Media_Server_ID == 0)
{
SetCM_Media_Server_ID (0);
SetCM_WebProject_ID (0);
SetIsPassive (false);
SetName (null);
}
 */
}
public X_CM_Media_Server (Ctx ctx, int CM_Media_Server_ID, Trx trxName) : base (ctx, CM_Media_Server_ID, trxName)
{
/** if (CM_Media_Server_ID == 0)
{
SetCM_Media_Server_ID (0);
SetCM_WebProject_ID (0);
SetIsPassive (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Media_Server (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Media_Server (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Media_Server (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Media_Server()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369053L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052264L;
/** AD_Table_ID=859 */
public static int Table_ID;
 // =859;

/** TableName=CM_Media_Server */
public static String Table_Name="CM_Media_Server";

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
StringBuilder sb = new StringBuilder ("X_CM_Media_Server[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Media Server.
@param CM_Media_Server_ID Media Server list to which content should get transfered */
public void SetCM_Media_Server_ID (int CM_Media_Server_ID)
{
if (CM_Media_Server_ID < 1) throw new ArgumentException ("CM_Media_Server_ID is mandatory.");
Set_ValueNoCheck ("CM_Media_Server_ID", CM_Media_Server_ID);
}
/** Get Media Server.
@return Media Server list to which content should get transfered */
public int GetCM_Media_Server_ID() 
{
Object ii = Get_Value("CM_Media_Server_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID < 1) throw new ArgumentException ("CM_WebProject_ID is mandatory.");
Set_ValueNoCheck ("CM_WebProject_ID", CM_WebProject_ID);
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
/** Set Folder.
@param Folder A folder on a local or remote system to store data into */
public void SetFolder (String Folder)
{
if (Folder != null && Folder.Length > 60)
{
log.Warning("Length > 60 - truncated");
Folder = Folder.Substring(0,60);
}
Set_Value ("Folder", Folder);
}
/** Get Folder.
@return A folder on a local or remote system to store data into */
public String GetFolder() 
{
return (String)Get_Value("Folder");
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
if (IP_Address != null && IP_Address.Length > 20)
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
/** Set Transfer passive.
@param IsPassive FTP passive transfer */
public void SetIsPassive (Boolean IsPassive)
{
Set_Value ("IsPassive", IsPassive);
}
/** Get Transfer passive.
@return FTP passive transfer */
public Boolean IsPassive() 
{
Object oo = Get_Value("IsPassive");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Password.
@param Password Password of any length (case sensitive) */
public void SetPassword (String Password)
{
if (Password != null && Password.Length > 40)
{
log.Warning("Length > 40 - truncated");
Password = Password.Substring(0,40);
}
Set_Value ("Password", Password);
}
/** Get Password.
@return Password of any length (case sensitive) */
public String GetPassword() 
{
return (String)Get_Value("Password");
}
/** Set URL.
@param URL Full URL address - e.g. http://www.viennaadvantage.com */
public void SetURL (String URL)
{
if (URL != null && URL.Length > 120)
{
log.Warning("Length > 120 - truncated");
URL = URL.Substring(0,120);
}
Set_Value ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennaadvantage.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}
/** Set Registered EMail.
@param UserName Email of the responsible for the System */
public void SetUserName (String UserName)
{
if (UserName != null && UserName.Length > 40)
{
log.Warning("Length > 40 - truncated");
UserName = UserName.Substring(0,40);
}
Set_Value ("UserName", UserName);
}
/** Get Registered EMail.
@return Email of the responsible for the System */
public String GetUserName() 
{
return (String)Get_Value("UserName");
}
}

}
