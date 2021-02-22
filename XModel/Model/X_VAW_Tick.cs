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
/** Generated Model for VAW_Tick
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAW_Tick : PO
{
public X_VAW_Tick (Context ctx, int VAW_Tick_ID, Trx trxName) : base (ctx, VAW_Tick_ID, trxName)
{
/** if (VAW_Tick_ID == 0)
{
SetProcessed (false);	// N
SetVAW_Tick_ID (0);
}
 */
}
public X_VAW_Tick (Ctx ctx, int VAW_Tick_ID, Trx trxName) : base (ctx, VAW_Tick_ID, trxName)
{
/** if (VAW_Tick_ID == 0)
{
SetProcessed (false);	// N
SetVAW_Tick_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAW_Tick (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAW_Tick (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAW_Tick (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAW_Tick()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384929L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068140L;
/** VAF_TableView_ID=550 */
public static int Table_ID;
 // =550;

/** TableName=VAW_Tick */
public static String Table_Name="VAW_Tick";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_VAW_Tick[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
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
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail = EMail.Substring(0,60);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetRemote_Addr());
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
/** Set Target URL.
@param TargetURL URL for the Target */
public void SetTargetURL (String TargetURL)
{
if (TargetURL != null && TargetURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
TargetURL = TargetURL.Substring(0,120);
}
Set_Value ("TargetURL", TargetURL);
}
/** Get Target URL.
@return URL for the Target */
public String GetTargetURL() 
{
return (String)Get_Value("TargetURL");
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
/** Set Click Count.
@param VAW_TickCount_ID Web Click Management */
public void SetVAW_TickCount_ID (int VAW_TickCount_ID)
{
if (VAW_TickCount_ID <= 0) Set_ValueNoCheck ("VAW_TickCount_ID", null);
else
Set_ValueNoCheck ("VAW_TickCount_ID", VAW_TickCount_ID);
}
/** Get Click Count.
@return Web Click Management */
public int GetVAW_TickCount_ID() 
{
Object ii = Get_Value("VAW_TickCount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Click.
@param VAW_Tick_ID Individual Web Click */
public void SetVAW_Tick_ID (int VAW_Tick_ID)
{
if (VAW_Tick_ID < 1) throw new ArgumentException ("VAW_Tick_ID is mandatory.");
Set_ValueNoCheck ("VAW_Tick_ID", VAW_Tick_ID);
}
/** Get Web Click.
@return Individual Web Click */
public int GetVAW_Tick_ID() 
{
Object ii = Get_Value("VAW_Tick_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
