namespace ViennaAdvantage.Model
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
/** Generated Model for WSP_GmailConfiguration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_WSP_GmailConfiguration : PO
{
public X_WSP_GmailConfiguration (Context ctx, int WSP_GmailConfiguration_ID, Trx trxName) : base (ctx, WSP_GmailConfiguration_ID, trxName)
{
/** if (WSP_GmailConfiguration_ID == 0)
{
SetWSP_GmailConfiguration_ID (0);
}
 */
}
public X_WSP_GmailConfiguration(Ctx ctx, int WSP_GmailConfiguration_ID, Trx trxName)
    : base(ctx, WSP_GmailConfiguration_ID, trxName)
{
/** if (WSP_GmailConfiguration_ID == 0)
{
SetWSP_GmailConfiguration_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_WSP_GmailConfiguration(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_WSP_GmailConfiguration(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_WSP_GmailConfiguration(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_WSP_GmailConfiguration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27715892032840L;
/** Last Updated Timestamp 6/8/2015 6:01:56 PM */
public static long updatedMS = 1433766716051L;
/** AD_Table_ID=1000388 */
public static int Table_ID;
 // =1000388;

/** TableName=WSP_GmailConfiguration */
public static String Table_Name="WSP_GmailConfiguration";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_WSP_GmailConfiguration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Calendar Refresh Token.
@param WSP_CalendarRefreshToken Calendar Refresh Token */
public void SetWSP_CalendarRefreshToken (String WSP_CalendarRefreshToken)
{
if (WSP_CalendarRefreshToken != null && WSP_CalendarRefreshToken.Length > 200)
{
log.Warning("Length > 200 - truncated");
WSP_CalendarRefreshToken = WSP_CalendarRefreshToken.Substring(0,200);
}
Set_Value ("WSP_CalendarRefreshToken", WSP_CalendarRefreshToken);
}
/** Get Calendar Refresh Token.
@return Calendar Refresh Token */
public String GetWSP_CalendarRefreshToken() 
{
return (String)Get_Value("WSP_CalendarRefreshToken");
}
/** Set Contact Refresh Token.
@param WSP_ContactRefreshToken Contact Refresh Token */
public void SetWSP_ContactRefreshToken (String WSP_ContactRefreshToken)
{
if (WSP_ContactRefreshToken != null && WSP_ContactRefreshToken.Length > 200)
{
log.Warning("Length > 200 - truncated");
WSP_ContactRefreshToken = WSP_ContactRefreshToken.Substring(0,200);
}
Set_Value ("WSP_ContactRefreshToken", WSP_ContactRefreshToken);
}
/** Get Contact Refresh Token.
@return Contact Refresh Token */
public String GetWSP_ContactRefreshToken() 
{
return (String)Get_Value("WSP_ContactRefreshToken");
}
/** Set WSP_GmailConfiguration_ID.
@param WSP_GmailConfiguration_ID WSP_GmailConfiguration_ID */
public void SetWSP_GmailConfiguration_ID (int WSP_GmailConfiguration_ID)
{
if (WSP_GmailConfiguration_ID < 1) throw new ArgumentException ("WSP_GmailConfiguration_ID is mandatory.");
Set_ValueNoCheck ("WSP_GmailConfiguration_ID", WSP_GmailConfiguration_ID);
}
/** Get WSP_GmailConfiguration_ID.
@return WSP_GmailConfiguration_ID */
public int GetWSP_GmailConfiguration_ID() 
{
Object ii = Get_Value("WSP_GmailConfiguration_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Import Contacts.
@param WSP_ImportContacts Import Contacts */
public void SetWSP_ImportContacts (String WSP_ImportContacts)
{
if (WSP_ImportContacts != null && WSP_ImportContacts.Length > 50)
{
log.Warning("Length > 50 - truncated");
WSP_ImportContacts = WSP_ImportContacts.Substring(0,50);
}
Set_Value ("WSP_ImportContacts", WSP_ImportContacts);
}
/** Get Import Contacts.
@return Import Contacts */
public String GetWSP_ImportContacts() 
{
return (String)Get_Value("WSP_ImportContacts");
}
/** Set Export Background.
@param WSP_IsExportBackground Export Background */
public void SetWSP_IsExportBackground (Boolean WSP_IsExportBackground)
{
Set_Value ("WSP_IsExportBackground", WSP_IsExportBackground);
}
/** Get Export Background.
@return Export Background */
public Boolean IsWSP_IsExportBackground() 
{
Object oo = Get_Value("WSP_IsExportBackground");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sync Calendar Background.
@param WSP_IsSyncCalendarBackground Sync Calendar Background */
public void SetWSP_IsSyncCalendarBackground (Boolean WSP_IsSyncCalendarBackground)
{
Set_Value ("WSP_IsSyncCalendarBackground", WSP_IsSyncCalendarBackground);
}
/** Get Sync Calendar Background.
@return Sync Calendar Background */
public Boolean IsWSP_IsSyncCalendarBackground() 
{
Object oo = Get_Value("WSP_IsSyncCalendarBackground");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sync Contact Background.
@param WSP_IsSyncContactBackground Sync Contact Background */
public void SetWSP_IsSyncContactBackground (Boolean WSP_IsSyncContactBackground)
{
Set_Value ("WSP_IsSyncContactBackground", WSP_IsSyncContactBackground);
}
/** Get Sync Contact Background.
@return Sync Contact Background */
public Boolean IsWSP_IsSyncContactBackground() 
{
Object oo = Get_Value("WSP_IsSyncContactBackground");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sync Deleted User.
@param WSP_IsSyncDeletedUser Sync Deleted User */
public void SetWSP_IsSyncDeletedUser (Boolean WSP_IsSyncDeletedUser)
{
Set_Value ("WSP_IsSyncDeletedUser", WSP_IsSyncDeletedUser);
}
/** Get Sync Deleted User.
@return Sync Deleted User */
public Boolean IsWSP_IsSyncDeletedUser() 
{
Object oo = Get_Value("WSP_IsSyncDeletedUser");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sync Task BackGround.
@param WSP_IsSyncTaskBackGround Sync Task BackGround */
public void SetWSP_IsSyncTaskBackGround (Boolean WSP_IsSyncTaskBackGround)
{
Set_Value ("WSP_IsSyncTaskBackGround", WSP_IsSyncTaskBackGround);
}
/** Get Sync Task BackGround.
@return Sync Task BackGround */
public Boolean IsWSP_IsSyncTaskBackGround() 
{
Object oo = Get_Value("WSP_IsSyncTaskBackGround");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Update Existing Record.
@param WSP_IsUpdateExistingRecord Update Existing Record */
public void SetWSP_IsUpdateExistingRecord (Boolean WSP_IsUpdateExistingRecord)
{
Set_Value ("WSP_IsUpdateExistingRecord", WSP_IsUpdateExistingRecord);
}
/** Get Update Existing Record.
@return Update Existing Record */
public Boolean IsWSP_IsUpdateExistingRecord() 
{
Object oo = Get_Value("WSP_IsUpdateExistingRecord");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Password.
@param WSP_Password Password */
public void SetWSP_Password (String WSP_Password)
{
if (WSP_Password != null && WSP_Password.Length > 50)
{
log.Warning("Length > 50 - truncated");
WSP_Password = WSP_Password.Substring(0,50);
}
Set_Value ("WSP_Password", WSP_Password);
}
/** Get Password.
@return Password */
public String GetWSP_Password() 
{
return (String)Get_Value("WSP_Password");
}
/** Set Task Refresh Token.
@param WSP_TaskRefreshToken Task Refresh Token */
public void SetWSP_TaskRefreshToken (String WSP_TaskRefreshToken)
{
if (WSP_TaskRefreshToken != null && WSP_TaskRefreshToken.Length > 200)
{
log.Warning("Length > 200 - truncated");
WSP_TaskRefreshToken = WSP_TaskRefreshToken.Substring(0,200);
}
Set_Value ("WSP_TaskRefreshToken", WSP_TaskRefreshToken);
}
/** Get Task Refresh Token.
@return Task Refresh Token */
public String GetWSP_TaskRefreshToken() 
{
return (String)Get_Value("WSP_TaskRefreshToken");
}
/** Set Username.
@param WSP_Username Username */
public void SetWSP_Username (String WSP_Username)
{
if (WSP_Username != null && WSP_Username.Length > 50)
{
log.Warning("Length > 50 - truncated");
WSP_Username = WSP_Username.Substring(0,50);
}
Set_Value ("WSP_Username", WSP_Username);
}
/** Get Username.
@return Username */
public String GetWSP_Username() 
{
return (String)Get_Value("WSP_Username");
}
}

}
