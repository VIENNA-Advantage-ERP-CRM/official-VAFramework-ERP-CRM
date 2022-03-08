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
/** Generated Model for AD_UserMailConfigration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserMailConfigration : PO
{
public X_AD_UserMailConfigration (Context ctx, int AD_UserMailConfigration_ID, Trx trxName) : base (ctx, AD_UserMailConfigration_ID, trxName)
{
/** if (AD_UserMailConfigration_ID == 0)
{
SetAD_UserMailConfigration_ID (0);
}
 */
}
public X_AD_UserMailConfigration (Ctx ctx, int AD_UserMailConfigration_ID, Trx trxName) : base (ctx, AD_UserMailConfigration_ID, trxName)
{
/** if (AD_UserMailConfigration_ID == 0)
{
SetAD_UserMailConfigration_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserMailConfigration (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserMailConfigration (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserMailConfigration (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserMailConfigration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27589121536891L;
/** Last Updated Timestamp 6/2/2011 12:00:20 PM */
public static long updatedMS = 1306996220102L;
/** AD_Table_ID=1000009 */
public static int Table_ID;
 // =1000009;

/** TableName=AD_UserMailConfigration */
public static String Table_Name="AD_UserMailConfigration";

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
StringBuilder sb = new StringBuilder ("X_AD_UserMailConfigration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_UserMailConfigration_ID.
@param AD_UserMailConfigration_ID AD_UserMailConfigration_ID */
public void SetAD_UserMailConfigration_ID (int AD_UserMailConfigration_ID)
{
if (AD_UserMailConfigration_ID < 1) throw new ArgumentException ("AD_UserMailConfigration_ID is mandatory.");
Set_ValueNoCheck ("AD_UserMailConfigration_ID", AD_UserMailConfigration_ID);
}
/** Get AD_UserMailConfigration_ID.
@return AD_UserMailConfigration_ID */
public int GetAD_UserMailConfigration_ID() 
{
Object ii = Get_Value("AD_UserMailConfigration_ID");
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
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 50)
{
log.Warning("Length > 50 - truncated");
EMail = EMail.Substring(0,50);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
}
/** Set ImapHost.
@param ImapHost ImapHost */
public void SetImapHost (String ImapHost)
{
if (ImapHost != null && ImapHost.Length > 50)
{
log.Warning("Length > 50 - truncated");
ImapHost = ImapHost.Substring(0,50);
}
Set_Value ("ImapHost", ImapHost);
}
/** Get ImapHost.
@return ImapHost */
public String GetImapHost() 
{
return (String)Get_Value("ImapHost");
}
/** Set IMAP Is SSL.
@param ImapIsSsl IMAP Is SSL */
public void SetImapIsSsl (Boolean ImapIsSsl)
{
Set_Value ("ImapIsSsl", ImapIsSsl);
}
/** Get IMAP Is SSL.
@return IMAP Is SSL */
public Boolean IsImapIsSsl() 
{
Object oo = Get_Value("ImapIsSsl");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IMAP Password.
@param ImapPassword IMAP Password */
public void SetImapPassword (String ImapPassword)
{
if (ImapPassword != null && ImapPassword.Length > 25)
{
log.Warning("Length > 25 - truncated");
ImapPassword = ImapPassword.Substring(0,25);
}
Set_Value ("ImapPassword", ImapPassword);
}
/** Get IMAP Password.
@return IMAP Password */
public String GetImapPassword() 
{
return (String)Get_Value("ImapPassword");
}
/** Set ImapUsername.
@param ImapUsername ImapUsername */
public void SetImapUsername (String ImapUsername)
{
if (ImapUsername != null && ImapUsername.Length > 50)
{
log.Warning("Length > 50 - truncated");
ImapUsername = ImapUsername.Substring(0,50);
}
Set_Value ("ImapUsername", ImapUsername);
}
/** Get ImapUsername.
@return ImapUsername */
public String GetImapUsername() 
{
return (String)Get_Value("ImapUsername");
}
/** Set IsAutoAttach.
@param IsAutoAttach IsAutoAttach */
public void SetIsAutoAttach (Boolean IsAutoAttach)
{
Set_Value ("IsAutoAttach", IsAutoAttach);
}
/** Get IsAutoAttach.
@return IsAutoAttach */
public Boolean IsAutoAttach() 
{
Object oo = Get_Value("IsAutoAttach");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsAutoDownload.
@param IsAutoDownload IsAutoDownload */
public void SetIsAutoDownload (String IsAutoDownload)
{
if (IsAutoDownload != null && IsAutoDownload.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsAutoDownload = IsAutoDownload.Substring(0,1);
}
Set_Value ("IsAutoDownload", IsAutoDownload);
}
/** Get IsAutoDownload.
@return IsAutoDownload */
public String GetIsAutoDownload() 
{
return (String)Get_Value("IsAutoDownload");
}
/** Set IsAutoLogin.
@param IsAutoLogin IsAutoLogin */
public void SetIsAutoLogin (Boolean IsAutoLogin)
{
Set_Value ("IsAutoLogin", IsAutoLogin);
}
/** Get IsAutoLogin.
@return IsAutoLogin */
public Boolean IsAutoLogin() 
{
Object oo = Get_Value("IsAutoLogin");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set SMTP Authentication.
@param IsSmtpAuthorization Your mail server requires Authentication */
public void SetIsSmtpAuthorization (Boolean IsSmtpAuthorization)
{
Set_Value ("IsSmtpAuthorization", IsSmtpAuthorization);
}
/** Get SMTP Authentication.
@return Your mail server requires Authentication */
public Boolean IsSmtpAuthorization() 
{
Object oo = Get_Value("IsSmtpAuthorization");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mail Host.
@param SmtpHost Hostname of Mail Server for SMTP and IMAP */
public void SetSmtpHost (String SmtpHost)
{
if (SmtpHost != null && SmtpHost.Length > 50)
{
log.Warning("Length > 50 - truncated");
SmtpHost = SmtpHost.Substring(0,50);
}
Set_Value ("SmtpHost", SmtpHost);
}
/** Get Mail Host.
@return Hostname of Mail Server for SMTP and IMAP */
public String GetSmtpHost() 
{
return (String)Get_Value("SmtpHost");
}
/** Set SmtpIsSsl.
@param SmtpIsSsl SmtpIsSsl */
public void SetSmtpIsSsl (Boolean SmtpIsSsl)
{
Set_Value ("SmtpIsSsl", SmtpIsSsl);
}
/** Get SmtpIsSsl.
@return SmtpIsSsl */
public Boolean IsSmtpIsSsl() 
{
Object oo = Get_Value("SmtpIsSsl");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set SmtpPassword.
@param SmtpPassword SmtpPassword */
public void SetSmtpPassword (String SmtpPassword)
{
if (SmtpPassword != null && SmtpPassword.Length > 25)
{
log.Warning("Length > 25 - truncated");
SmtpPassword = SmtpPassword.Substring(0,25);
}
Set_Value ("SmtpPassword", SmtpPassword);
}
/** Get SmtpPassword.
@return SmtpPassword */
public String GetSmtpPassword() 
{
return (String)Get_Value("SmtpPassword");
}
/** Set SmtpUsername.
@param SmtpUsername SmtpUsername */
public void SetSmtpUsername (String SmtpUsername)
{
if (SmtpUsername != null && SmtpUsername.Length > 50)
{
log.Warning("Length > 50 - truncated");
SmtpUsername = SmtpUsername.Substring(0,50);
}
Set_Value ("SmtpUsername", SmtpUsername);
}
/** Get SmtpUsername.
@return SmtpUsername */
public String GetSmtpUsername() 
{
return (String)Get_Value("SmtpUsername");
}

/** TableAttach AD_Reference_ID=1000007 */
public static int TABLEATTACH_AD_Reference_ID=1000007;
/** User Contact = AD_User */
public static String TABLEATTACH_UserContact = "AD_User";
/** Business Partner = C_BPartner */
public static String TABLEATTACH_BusinessPartner = "C_BPartner";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTableAttachValid (String test)
{
return test == null || test.Equals("AD_User") || test.Equals("C_BPartner");
}
/** Set TableAttach.
@param TableAttach TableAttach */
public void SetTableAttach (String TableAttach)
{
if (!IsTableAttachValid(TableAttach))
throw new ArgumentException ("TableAttach Invalid value - " + TableAttach + " - Reference_ID=1000007 - AD_User - C_BPartner");
if (TableAttach != null && TableAttach.Length > 10)
{
log.Warning("Length > 10 - truncated");
TableAttach = TableAttach.Substring(0,10);
}
Set_Value ("TableAttach", TableAttach);
}
/** Get TableAttach.
@return TableAttach */
public String GetTableAttach() 
{
return (String)Get_Value("TableAttach");
}
}

}
