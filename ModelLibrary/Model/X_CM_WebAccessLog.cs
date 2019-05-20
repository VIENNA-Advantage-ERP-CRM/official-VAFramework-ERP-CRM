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
/** Generated Model for CM_WebAccessLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_WebAccessLog : PO
{
public X_CM_WebAccessLog (Context ctx, int CM_WebAccessLog_ID, Trx trxName) : base (ctx, CM_WebAccessLog_ID, trxName)
{
/** if (CM_WebAccessLog_ID == 0)
{
SetCM_WebAccessLog_ID (0);
SetIP_Address (null);
SetLogType (null);
SetProtocol (null);
SetRequestType (null);
}
 */
}
public X_CM_WebAccessLog (Ctx ctx, int CM_WebAccessLog_ID, Trx trxName) : base (ctx, CM_WebAccessLog_ID, trxName)
{
/** if (CM_WebAccessLog_ID == 0)
{
SetCM_WebAccessLog_ID (0);
SetIP_Address (null);
SetLogType (null);
SetProtocol (null);
SetRequestType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebAccessLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebAccessLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebAccessLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_WebAccessLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369241L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052452L;
/** AD_Table_ID=894 */
public static int Table_ID;
 // =894;

/** TableName=CM_WebAccessLog */
public static String Table_Name="CM_WebAccessLog";

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
StringBuilder sb = new StringBuilder ("X_CM_WebAccessLog[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Accept Language.
@param AcceptLanguage Language accepted based on browser information */
public void SetAcceptLanguage (String AcceptLanguage)
{
if (AcceptLanguage != null && AcceptLanguage.Length > 60)
{
log.Warning("Length > 60 - truncated");
AcceptLanguage = AcceptLanguage.Substring(0,60);
}
Set_Value ("AcceptLanguage", AcceptLanguage);
}
/** Get Accept Language.
@return Language accepted based on browser information */
public String GetAcceptLanguage() 
{
return (String)Get_Value("AcceptLanguage");
}
/** Set Broadcast Server.
@param CM_BroadcastServer_ID Web Broadcast Server */
public void SetCM_BroadcastServer_ID (int CM_BroadcastServer_ID)
{
if (CM_BroadcastServer_ID <= 0) Set_Value ("CM_BroadcastServer_ID", null);
else
Set_Value ("CM_BroadcastServer_ID", CM_BroadcastServer_ID);
}
/** Get Broadcast Server.
@return Web Broadcast Server */
public int GetCM_BroadcastServer_ID() 
{
Object ii = Get_Value("CM_BroadcastServer_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Media Item.
@param CM_Media_ID Contains media content like images, flash movies etc. */
public void SetCM_Media_ID (int CM_Media_ID)
{
if (CM_Media_ID <= 0) Set_Value ("CM_Media_ID", null);
else
Set_Value ("CM_Media_ID", CM_Media_ID);
}
/** Get Media Item.
@return Contains media content like images, flash movies etc. */
public int GetCM_Media_ID() 
{
Object ii = Get_Value("CM_Media_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Access Log.
@param CM_WebAccessLog_ID Web Access Log Information */
public void SetCM_WebAccessLog_ID (int CM_WebAccessLog_ID)
{
if (CM_WebAccessLog_ID < 1) throw new ArgumentException ("CM_WebAccessLog_ID is mandatory.");
Set_ValueNoCheck ("CM_WebAccessLog_ID", CM_WebAccessLog_ID);
}
/** Get Web Access Log.
@return Web Access Log Information */
public int GetCM_WebAccessLog_ID() 
{
Object ii = Get_Value("CM_WebAccessLog_ID");
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
/** Set File Size.
@param FileSize Size of the File in bytes */
public void SetFileSize (Decimal? FileSize)
{
Set_Value ("FileSize", (Decimal?)FileSize);
}
/** Get File Size.
@return Size of the File in bytes */
public Decimal GetFileSize() 
{
Object bd =Get_Value("FileSize");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Hyphen.
@param Hyphen Hyphen */
public void SetHyphen (String Hyphen)
{
if (Hyphen != null && Hyphen.Length > 20)
{
log.Warning("Length > 20 - truncated");
Hyphen = Hyphen.Substring(0,20);
}
Set_Value ("Hyphen", Hyphen);
}
/** Get Hyphen.
@return Hyphen */
public String GetHyphen() 
{
return (String)Get_Value("Hyphen");
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

/** LogType AD_Reference_ID=390 */
public static int LOGTYPE_AD_Reference_ID=390;
/** Ad display = A */
public static String LOGTYPE_AdDisplay = "A";
/** Redirect = R */
public static String LOGTYPE_Redirect = "R";
/** Web Access = W */
public static String LOGTYPE_WebAccess = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLogTypeValid (String test)
{
return test.Equals("A") || test.Equals("R") || test.Equals("W");
}
/** Set Log Type.
@param LogType Web Log Type */
public void SetLogType (String LogType)
{
if (LogType == null) throw new ArgumentException ("LogType is mandatory");
if (!IsLogTypeValid(LogType))
throw new ArgumentException ("LogType Invalid value - " + LogType + " - Reference_ID=390 - A - R - W");
if (LogType.Length > 1)
{
log.Warning("Length > 1 - truncated");
LogType = LogType.Substring(0,1);
}
Set_Value ("LogType", LogType);
}
/** Get Log Type.
@return Web Log Type */
public String GetLogType() 
{
return (String)Get_Value("LogType");
}
/** Set Page URL.
@param PageURL Page URL */
public void SetPageURL (String PageURL)
{
if (PageURL != null && PageURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
PageURL = PageURL.Substring(0,120);
}
Set_Value ("PageURL", PageURL);
}
/** Get Page URL.
@return Page URL */
public String GetPageURL() 
{
return (String)Get_Value("PageURL");
}
/** Set Protocol.
@param Protocol Protocol */
public void SetProtocol (String Protocol)
{
if (Protocol == null) throw new ArgumentException ("Protocol is mandatory.");
if (Protocol.Length > 20)
{
log.Warning("Length > 20 - truncated");
Protocol = Protocol.Substring(0,20);
}
Set_Value ("Protocol", Protocol);
}
/** Get Protocol.
@return Protocol */
public String GetProtocol() 
{
return (String)Get_Value("Protocol");
}
/** Set Referrer.
@param Referrer Referring web address */
public void SetReferrer (String Referrer)
{
if (Referrer != null && Referrer.Length > 120)
{
log.Warning("Length > 120 - truncated");
Referrer = Referrer.Substring(0,120);
}
Set_Value ("Referrer", Referrer);
}
/** Get Referrer.
@return Referring web address */
public String GetReferrer() 
{
return (String)Get_Value("Referrer");
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
if (Remote_Host != null && Remote_Host.Length > 120)
{
log.Warning("Length > 120 - truncated");
Remote_Host = Remote_Host.Substring(0,120);
}
Set_Value ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
}
/** Set Request Type.
@param RequestType Request Type */
public void SetRequestType (String RequestType)
{
if (RequestType == null) throw new ArgumentException ("RequestType is mandatory.");
if (RequestType.Length > 4)
{
log.Warning("Length > 4 - truncated");
RequestType = RequestType.Substring(0,4);
}
Set_Value ("RequestType", RequestType);
}
/** Get Request Type.
@return Request Type */
public String GetRequestType() 
{
return (String)Get_Value("RequestType");
}
/** Set Status Code.
@param StatusCode Status Code */
public void SetStatusCode (int StatusCode)
{
Set_Value ("StatusCode", StatusCode);
}
/** Get Status Code.
@return Status Code */
public int GetStatusCode() 
{
Object ii = Get_Value("StatusCode");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Agent.
@param UserAgent Browser Used */
public void SetUserAgent (String UserAgent)
{
if (UserAgent != null && UserAgent.Length > 255)
{
log.Warning("Length > 255 - truncated");
UserAgent = UserAgent.Substring(0,255);
}
Set_Value ("UserAgent", UserAgent);
}
/** Get User Agent.
@return Browser Used */
public String GetUserAgent() 
{
return (String)Get_Value("UserAgent");
}
/** Set Web Session.
@param WebSession Web Session ID */
public void SetWebSession (String WebSession)
{
if (WebSession != null && WebSession.Length > 40)
{
log.Warning("Length > 40 - truncated");
WebSession = WebSession.Substring(0,40);
}
Set_Value ("WebSession", WebSession);
}
/** Get Web Session.
@return Web Session ID */
public String GetWebSession() 
{
return (String)Get_Value("WebSession");
}
}

}
