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
/** Generated Model for AD_Client
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Client : PO
{
public X_AD_Client (Context ctx, int AD_Client_ID, Trx trxName) : base (ctx, AD_Client_ID, trxName)
{
/** if (AD_Client_ID == 0)
{
SetAutoArchive (null);	// N
SetIsCostImmediate (false);	// N
SetIsMultiLingualDocument (false);
SetIsPostImmediate (false);	// N
SetIsServerEMail (false);
SetIsSmtpAuthorization (false);	// N
SetIsUseBetaFunctions (true);	// Y
SetMMPolicy (null);	// F
SetName (null);
SetValue (null);
}
 */
}
public X_AD_Client (Ctx ctx, int AD_Client_ID, Trx trxName) : base (ctx, AD_Client_ID, trxName)
{
/** if (AD_Client_ID == 0)
{
SetAutoArchive (null);	// N
SetIsCostImmediate (false);	// N
SetIsMultiLingualDocument (false);
SetIsPostImmediate (false);	// N
SetIsServerEMail (false);
SetIsSmtpAuthorization (false);	// N
SetIsUseBetaFunctions (true);	// Y
SetMMPolicy (null);	// F
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Client (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Client (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Client (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Client()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360809L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044020L;
/** AD_Table_ID=112 */
public static int Table_ID;
 // =112;

/** TableName=AD_Client */
public static String Table_Name="AD_Client";

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
StringBuilder sb = new StringBuilder ("X_AD_Client[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=327 */
public static int AD_LANGUAGE_AD_Reference_ID=327;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language != null && AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_Value ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}

/** AutoArchive AD_Reference_ID=334 */
public static int AUTOARCHIVE_AD_Reference_ID=334;
/** All (Reports, Documents) = 1 */
public static String AUTOARCHIVE_AllReportsDocuments = "1";
/** Documents = 2 */
public static String AUTOARCHIVE_Documents = "2";
/** External Documents = 3 */
public static String AUTOARCHIVE_ExternalDocuments = "3";
/** None = N */
public static String AUTOARCHIVE_None = "N";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAutoArchiveValid (String test)
{
return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("N");
}
/** Set Auto Archive.
@param AutoArchive Enable and level of automatic Archive of documents */
public void SetAutoArchive (String AutoArchive)
{
if (AutoArchive == null) throw new ArgumentException ("AutoArchive is mandatory");
if (!IsAutoArchiveValid(AutoArchive))
throw new ArgumentException ("AutoArchive Invalid value - " + AutoArchive + " - Reference_ID=334 - 1 - 2 - 3 - N");
if (AutoArchive.Length > 1)
{
log.Warning("Length > 1 - truncated");
AutoArchive = AutoArchive.Substring(0,1);
}
Set_Value ("AutoArchive", AutoArchive);
}
/** Get Auto Archive.
@return Enable and level of automatic Archive of documents */
public String GetAutoArchive() 
{
return (String)Get_Value("AutoArchive");
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
/** Set Document Directory.
@param DocumentDir Directory for documents from the application server */
public void SetDocumentDir (String DocumentDir)
{
if (DocumentDir != null && DocumentDir.Length > 60)
{
log.Warning("Length > 60 - truncated");
DocumentDir = DocumentDir.Substring(0,60);
}
Set_Value ("DocumentDir", DocumentDir);
}
/** Get Document Directory.
@return Directory for documents from the application server */
public String GetDocumentDir() 
{
return (String)Get_Value("DocumentDir");
}
/** Set EMail Test.
@param EMailTest Test EMail */
public void SetEMailTest (String EMailTest)
{
if (EMailTest != null && EMailTest.Length > 1)
{
log.Warning("Length > 1 - truncated");
EMailTest = EMailTest.Substring(0,1);
}
Set_Value ("EMailTest", EMailTest);
}
/** Get EMail Test.
@return Test EMail */
public String GetEMailTest() 
{
return (String)Get_Value("EMailTest");
}
/** Set Cost Immediately.
@param IsCostImmediate Update Costs immediately for testing */
public void SetIsCostImmediate (Boolean IsCostImmediate)
{
Set_Value ("IsCostImmediate", IsCostImmediate);
}
/** Get Cost Immediately.
@return Update Costs immediately for testing */
public Boolean IsCostImmediate() 
{
Object oo = Get_Value("IsCostImmediate");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Multi Lingual Documents.
@param IsMultiLingualDocument Documents are Multi Lingual */
public void SetIsMultiLingualDocument (Boolean IsMultiLingualDocument)
{
Set_Value ("IsMultiLingualDocument", IsMultiLingualDocument);
}
/** Get Multi Lingual Documents.
@return Documents are Multi Lingual */
public Boolean IsMultiLingualDocument() 
{
Object oo = Get_Value("IsMultiLingualDocument");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Post Immediately.
@param IsPostImmediate Post the accounting immediately for testing */
public void SetIsPostImmediate (Boolean IsPostImmediate)
{
Set_Value ("IsPostImmediate", IsPostImmediate);
}
/** Get Post Immediately.
@return Post the accounting immediately for testing */
public Boolean IsPostImmediate() 
{
Object oo = Get_Value("IsPostImmediate");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Server EMail.
@param IsServerEMail Send EMail from Server */
public void SetIsServerEMail (Boolean IsServerEMail)
{
Set_Value ("IsServerEMail", IsServerEMail);
}
/** Get Server EMail.
@return Send EMail from Server */
public Boolean IsServerEMail() 
{
Object oo = Get_Value("IsServerEMail");
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
/** Set SMTP TLS.
@param IsSmtpTLS Use TLS for SMTP communication */
public void SetIsSmtpTLS (Boolean IsSmtpTLS)
{
Set_Value ("IsSmtpTLS", IsSmtpTLS);
}
/** Get SMTP TLS.
@return Use TLS for SMTP communication */
public Boolean IsSmtpTLS() 
{
Object oo = Get_Value("IsSmtpTLS");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Use Beta Functions.
@param IsUseBetaFunctions Enable the use of Beta Functionality */
public void SetIsUseBetaFunctions (Boolean IsUseBetaFunctions)
{
Set_Value ("IsUseBetaFunctions", IsUseBetaFunctions);
}
/** Get Use Beta Functions.
@return Enable the use of Beta Functionality */
public Boolean IsUseBetaFunctions() 
{
Object oo = Get_Value("IsUseBetaFunctions");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set LDAP Query.
@param LDAPQuery Query to authenticate users for that client with LDAP */
public void SetLDAPQuery (String LDAPQuery)
{
if (LDAPQuery != null && LDAPQuery.Length > 255)
{
log.Warning("Length > 255 - truncated");
LDAPQuery = LDAPQuery.Substring(0,255);
}
Set_Value ("LDAPQuery", LDAPQuery);
}
/** Get LDAP Query.
@return Query to authenticate users for that client with LDAP */
public String GetLDAPQuery() 
{
return (String)Get_Value("LDAPQuery");
}

/** MMPolicy AD_Reference_ID=335 */
public static int MMPOLICY_AD_Reference_ID=335;
/** FiFo = F */
public static String MMPOLICY_FiFo = "F";
/** LiFo = L */
public static String MMPOLICY_LiFo = "L";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMMPolicyValid (String test)
{
return test.Equals("F") || test.Equals("L");
}
/** Set Material Policy.
@param MMPolicy Material Movement Policy */
public void SetMMPolicy (String MMPolicy)
{
if (MMPolicy == null) throw new ArgumentException ("MMPolicy is mandatory");
if (!IsMMPolicyValid(MMPolicy))
throw new ArgumentException ("MMPolicy Invalid value - " + MMPolicy + " - Reference_ID=335 - F - L");
if (MMPolicy.Length > 1)
{
log.Warning("Length > 1 - truncated");
MMPolicy = MMPolicy.Substring(0,1);
}
Set_Value ("MMPolicy", MMPolicy);
}
/** Get Material Policy.
@return Material Movement Policy */
public String GetMMPolicy() 
{
return (String)Get_Value("MMPolicy");
}
/** Set Model Validation Classes.
@param ModelValidationClasses List of data model validation classes separated by ;
 */
public void SetModelValidationClasses (String ModelValidationClasses)
{
if (ModelValidationClasses != null && ModelValidationClasses.Length > 255)
{
log.Warning("Length > 255 - truncated");
ModelValidationClasses = ModelValidationClasses.Substring(0,255);
}
Set_Value ("ModelValidationClasses", ModelValidationClasses);
}
/** Get Model Validation Classes.
@return List of data model validation classes separated by ;
 */
public String GetModelValidationClasses() 
{
return (String)Get_Value("ModelValidationClasses");
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
/** Set Request EMail.
@param RequestEMail EMail address to send automated mails from or receive mails for automated processing (fully qualified) */
public void SetRequestEMail (String RequestEMail)
{
if (RequestEMail != null && RequestEMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
RequestEMail = RequestEMail.Substring(0,60);
}
Set_Value ("RequestEMail", RequestEMail);
}
/** Get Request EMail.
@return EMail address to send automated mails from or receive mails for automated processing (fully qualified) */
public String GetRequestEMail() 
{
return (String)Get_Value("RequestEMail");
}
/** Set Request Folder.
@param RequestFolder EMail folder to process incoming emails;
 if empty INBOX is used */
public void SetRequestFolder (String RequestFolder)
{
if (RequestFolder != null && RequestFolder.Length > 20)
{
log.Warning("Length > 20 - truncated");
RequestFolder = RequestFolder.Substring(0,20);
}
Set_Value ("RequestFolder", RequestFolder);
}
/** Get Request Folder.
@return EMail folder to process incoming emails;
 if empty INBOX is used */
public String GetRequestFolder() 
{
return (String)Get_Value("RequestFolder");
}
/** Set Request User.
@param RequestUser User Name (ID) of the email owner */
public void SetRequestUser (String RequestUser)
{
if (RequestUser != null && RequestUser.Length > 60)
{
log.Warning("Length > 60 - truncated");
RequestUser = RequestUser.Substring(0,60);
}
Set_Value ("RequestUser", RequestUser);
}
/** Get Request User.
@return User Name (ID) of the email owner */
public String GetRequestUser() 
{
return (String)Get_Value("RequestUser");
}
/** Set Request User Password.
@param RequestUserPW Password of the user name (ID) for mail processing */
public void SetRequestUserPW (String RequestUserPW)
{
if (RequestUserPW != null && RequestUserPW.Length > 20)
{
log.Warning("Length > 20 - truncated");
RequestUserPW = RequestUserPW.Substring(0,20);
}
Set_Value ("RequestUserPW", RequestUserPW);
}
/** Get Request User Password.
@return Password of the user name (ID) for mail processing */
public String GetRequestUserPW() 
{
return (String)Get_Value("RequestUserPW");
}
/** Set Mail Host.
@param SmtpHost Hostname of Mail Server for SMTP and IMAP */
public void SetSmtpHost (String SmtpHost)
{
if (SmtpHost != null && SmtpHost.Length > 60)
{
log.Warning("Length > 60 - truncated");
SmtpHost = SmtpHost.Substring(0,60);
}
Set_Value ("SmtpHost", SmtpHost);
}
/** Get Mail Host.
@return Hostname of Mail Server for SMTP and IMAP */
public String GetSmtpHost() 
{
return (String)Get_Value("SmtpHost");
}
/** Set SMTP Port.
@param SmtpPort Mail service port */
public void SetSmtpPort (int SmtpPort)
{
Set_Value ("SmtpPort", SmtpPort);
}
/** Get SMTP Port.
@return Mail service port */
public int GetSmtpPort() 
{
Object ii = Get_Value("SmtpPort");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
