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
/** Generated Model for W_Counter
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_W_Counter : PO
{
public X_W_Counter (Context ctx, int W_Counter_ID, Trx trxName) : base (ctx, W_Counter_ID, trxName)
{
/** if (W_Counter_ID == 0)
{
SetPageURL (null);
SetProcessed (false);	// N
SetW_Counter_ID (0);
}
 */
}
public X_W_Counter (Ctx ctx, int W_Counter_ID, Trx trxName) : base (ctx, W_Counter_ID, trxName)
{
/** if (W_Counter_ID == 0)
{
SetPageURL (null);
SetProcessed (false);	// N
SetW_Counter_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Counter (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Counter (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_W_Counter (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_W_Counter()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384976L;
/** Last Updated Timestamp 7/29/2010 1:07:48 PM */
public static long updatedMS = 1280389068187L;
/** AD_Table_ID=403 */
public static int Table_ID;
 // =403;

/** TableName=W_Counter */
public static String Table_Name="W_Counter";

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
StringBuilder sb = new StringBuilder ("X_W_Counter[").Append(Get_ID()).Append("]");
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
/** Set Page URL.
@param PageURL Page URL */
public void SetPageURL (String PageURL)
{
if (PageURL == null) throw new ArgumentException ("PageURL is mandatory.");
if (PageURL.Length > 120)
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
/** Set Counter Count.
@param W_CounterCount_ID Web Counter Count Management */
public void SetW_CounterCount_ID (int W_CounterCount_ID)
{
if (W_CounterCount_ID <= 0) Set_ValueNoCheck ("W_CounterCount_ID", null);
else
Set_ValueNoCheck ("W_CounterCount_ID", W_CounterCount_ID);
}
/** Get Counter Count.
@return Web Counter Count Management */
public int GetW_CounterCount_ID() 
{
Object ii = Get_Value("W_CounterCount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Counter.
@param W_Counter_ID Individual Count hit */
public void SetW_Counter_ID (int W_Counter_ID)
{
if (W_Counter_ID < 1) throw new ArgumentException ("W_Counter_ID is mandatory.");
Set_ValueNoCheck ("W_Counter_ID", W_Counter_ID);
}
/** Get Web Counter.
@return Individual Count hit */
public int GetW_Counter_ID() 
{
Object ii = Get_Value("W_Counter_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
