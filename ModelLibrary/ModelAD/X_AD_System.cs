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
/** Generated Model for AD_System
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_System : PO
{
public X_AD_System (Context ctx, int AD_System_ID, Trx trxName) : base (ctx, AD_System_ID, trxName)
{
/** if (AD_System_ID == 0)
{
SetAD_System_ID (0);
SetIsAllowStatistics (false);
SetIsAutoErrorReport (true);	// Y
SetName (null);
SetPassword (null);
SetReleaseNo (null);
SetReplicationType (null);	// L
SetSystemStatus (null);	// E
SetUserName (null);
SetVersion (null);
}
 */
}
public X_AD_System (Ctx ctx, int AD_System_ID, Trx trxName) : base (ctx, AD_System_ID, trxName)
{
/** if (AD_System_ID == 0)
{
SetAD_System_ID (0);
SetIsAllowStatistics (false);
SetIsAutoErrorReport (true);	// Y
SetName (null);
SetPassword (null);
SetReleaseNo (null);
SetReplicationType (null);	// L
SetSystemStatus (null);	// E
SetUserName (null);
SetVersion (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_System (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_System (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_System (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_System()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364210L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047421L;
/** AD_Table_ID=531 */
public static int Table_ID;
 // =531;

/** TableName=AD_System */
public static String Table_Name="AD_System";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_System[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System.
@param AD_System_ID System Definition */
public void SetAD_System_ID (int AD_System_ID)
{
if (AD_System_ID < 1) throw new ArgumentException ("AD_System_ID is mandatory.");
Set_ValueNoCheck ("AD_System_ID", AD_System_ID);
}
/** Get System.
@return System Definition */
public int GetAD_System_ID() 
{
Object ii = Get_Value("AD_System_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Custom Prefix.
@param CustomPrefix Prefix for Custom entities */
public void SetCustomPrefix (String CustomPrefix)
{
if (CustomPrefix != null && CustomPrefix.Length > 60)
{
log.Warning("Length > 60 - truncated");
CustomPrefix = CustomPrefix.Substring(0,60);
}
Set_Value ("CustomPrefix", CustomPrefix);
}
/** Get Custom Prefix.
@return Prefix for Custom entities */
public String GetCustomPrefix() 
{
return (String)Get_Value("CustomPrefix");
}
/** Set DB Address.
@param DBAddress JDBC URL of the database server */
public void SetDBAddress (String DBAddress)
{
if (DBAddress != null && DBAddress.Length > 255)
{
log.Warning("Length > 255 - truncated");
DBAddress = DBAddress.Substring(0,255);
}
Set_Value ("DBAddress", DBAddress);
}
/** Get DB Address.
@return JDBC URL of the database server */
public String GetDBAddress() 
{
return (String)Get_Value("DBAddress");
}
/** Set Database Name.
@param DBInstance Database Name */
public void SetDBInstance (String DBInstance)
{
if (DBInstance != null && DBInstance.Length > 60)
{
log.Warning("Length > 60 - truncated");
DBInstance = DBInstance.Substring(0,60);
}
Set_Value ("DBInstance", DBInstance);
}
/** Get Database Name.
@return Database Name */
public String GetDBInstance() 
{
return (String)Get_Value("DBInstance");
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
/** Set Encryption Class.
@param EncryptionKey Encryption Class used for securing data content */
public void SetEncryptionKey (String EncryptionKey)
{
if (EncryptionKey != null && EncryptionKey.Length > 255)
{
log.Warning("Length > 255 - truncated");
EncryptionKey = EncryptionKey.Substring(0,255);
}
Set_ValueNoCheck ("EncryptionKey", EncryptionKey);
}
/** Get Encryption Class.
@return Encryption Class used for securing data content */
public String GetEncryptionKey() 
{
return (String)Get_Value("EncryptionKey");
}
/** Set ID Range End.
@param IDRangeEnd End of the ID Range used */
public void SetIDRangeEnd (Decimal? IDRangeEnd)
{
Set_Value ("IDRangeEnd", (Decimal?)IDRangeEnd);
}
/** Get ID Range End.
@return End of the ID Range used */
public Decimal GetIDRangeEnd() 
{
Object bd =Get_Value("IDRangeEnd");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set ID Range Start.
@param IDRangeStart Start of the ID Range used */
public void SetIDRangeStart (Decimal? IDRangeStart)
{
Set_Value ("IDRangeStart", (Decimal?)IDRangeStart);
}
/** Get ID Range Start.
@return Start of the ID Range used */
public Decimal GetIDRangeStart() 
{
Object bd =Get_Value("IDRangeStart");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Info.
@param Info Information */
public void SetInfo (String Info)
{
if (Info != null && Info.Length > 255)
{
log.Warning("Length > 255 - truncated");
Info = Info.Substring(0,255);
}
Set_ValueNoCheck ("Info", Info);
}
/** Get Info.
@return Information */
public String GetInfo() 
{
return (String)Get_Value("Info");
}
/** Set Maintain Statistics.
@param IsAllowStatistics Maintain general statistics */
public void SetIsAllowStatistics (Boolean IsAllowStatistics)
{
Set_Value ("IsAllowStatistics", IsAllowStatistics);
}
/** Get Maintain Statistics.
@return Maintain general statistics */
public Boolean IsAllowStatistics() 
{
Object oo = Get_Value("IsAllowStatistics");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Error Reporting.
@param IsAutoErrorReport Automatically report Errors */
public void SetIsAutoErrorReport (Boolean IsAutoErrorReport)
{
Set_Value ("IsAutoErrorReport", IsAutoErrorReport);
}
/** Get Error Reporting.
@return Automatically report Errors */
public Boolean IsAutoErrorReport() 
{
Object oo = Get_Value("IsAutoErrorReport");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Just Migrated.
@param IsJustMigrated Value set by Migration for post-Migation tasks. */
public void SetIsJustMigrated (Boolean IsJustMigrated)
{
Set_Value ("IsJustMigrated", IsJustMigrated);
}
/** Get Just Migrated.
@return Value set by Migration for post-Migation tasks. */
public Boolean IsJustMigrated() 
{
Object oo = Get_Value("IsJustMigrated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set LDAP Domain.
@param LDAPDomain Directory service domain name - e.g. Vienna.org */
public void SetLDAPDomain (String LDAPDomain)
{
if (LDAPDomain != null && LDAPDomain.Length > 255)
{
log.Warning("Length > 255 - truncated");
LDAPDomain = LDAPDomain.Substring(0,255);
}
Set_Value ("LDAPDomain", LDAPDomain);
}
/** Get LDAP Domain.
@return Directory service domain name - e.g. Vienna.org */
public String GetLDAPDomain() 
{
return (String)Get_Value("LDAPDomain");
}
/** Set LDAP URL.
@param LDAPHost Connection String to LDAP server starting with ldap:// */
public void SetLDAPHost (String LDAPHost)
{
if (LDAPHost != null && LDAPHost.Length > 60)
{
log.Warning("Length > 60 - truncated");
LDAPHost = LDAPHost.Substring(0,60);
}
Set_Value ("LDAPHost", LDAPHost);
}
/** Get LDAP URL.
@return Connection String to LDAP server starting with ldap:// */
public String GetLDAPHost() 
{
return (String)Get_Value("LDAPHost");
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
/** Set Processors.
@param NoProcessors Number of Database Processors */
public void SetNoProcessors (int NoProcessors)
{
Set_Value ("NoProcessors", NoProcessors);
}
/** Get Processors.
@return Number of Database Processors */
public int GetNoProcessors() 
{
Object ii = Get_Value("NoProcessors");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Old Name.
@param OldName Old Name */
public void SetOldName (String OldName)
{
if (OldName != null && OldName.Length > 60)
{
log.Warning("Length > 60 - truncated");
OldName = OldName.Substring(0,60);
}
Set_ValueNoCheck ("OldName", OldName);
}
/** Get Old Name.
@return Old Name */
public String GetOldName() 
{
return (String)Get_Value("OldName");
}
/** Set Password.
@param Password Password of any length (case sensitive) */
public void SetPassword (String Password)
{
if (Password == null) throw new ArgumentException ("Password is mandatory.");
if (Password.Length > 20)
{
log.Warning("Length > 20 - truncated");
Password = Password.Substring(0,20);
}
Set_Value ("Password", Password);
}
/** Get Password.
@return Password of any length (case sensitive) */
public String GetPassword() 
{
return (String)Get_Value("Password");
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
/** Set Profile.
@param ProfileInfo Information to help profiling the system for solving support issues */
public void SetProfileInfo (String ProfileInfo)
{
if (ProfileInfo != null && ProfileInfo.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProfileInfo = ProfileInfo.Substring(0,60);
}
Set_ValueNoCheck ("ProfileInfo", ProfileInfo);
}
/** Get Profile.
@return Information to help profiling the system for solving support issues */
public String GetProfileInfo() 
{
return (String)Get_Value("ProfileInfo");
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
/** Set Release No.
@param ReleaseNo Internal Release Number */
public void SetReleaseNo (String ReleaseNo)
{
if (ReleaseNo == null) throw new ArgumentException ("ReleaseNo is mandatory.");
if (ReleaseNo.Length > 4)
{
log.Warning("Length > 4 - truncated");
ReleaseNo = ReleaseNo.Substring(0,4);
}
Set_ValueNoCheck ("ReleaseNo", ReleaseNo);
}
/** Get Release No.
@return Internal Release Number */
public String GetReleaseNo() 
{
return (String)Get_Value("ReleaseNo");
}

/** ReplicationType AD_Reference_ID=126 */
public static int REPLICATIONTYPE_AD_Reference_ID=126;
/** Local = L */
public static String REPLICATIONTYPE_Local = "L";
/** Merge = M */
public static String REPLICATIONTYPE_Merge = "M";
/** Reference = R */
public static String REPLICATIONTYPE_Reference = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsReplicationTypeValid (String test)
{
return test.Equals("L") || test.Equals("M") || test.Equals("R");
}
/** Set Replication Type.
@param ReplicationType Type of Data Replication */
public void SetReplicationType (String ReplicationType)
{
if (ReplicationType == null) throw new ArgumentException ("ReplicationType is mandatory");
if (!IsReplicationTypeValid(ReplicationType))
throw new ArgumentException ("ReplicationType Invalid value - " + ReplicationType + " - Reference_ID=126 - L - M - R");
if (ReplicationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ReplicationType = ReplicationType.Substring(0,1);
}
Set_Value ("ReplicationType", ReplicationType);
}
/** Get Replication Type.
@return Type of Data Replication */
public String GetReplicationType() 
{
return (String)Get_Value("ReplicationType");
}
/** Set Statistics.
@param StatisticsInfo Information to help profiling the system for solving support issues */
public void SetStatisticsInfo (String StatisticsInfo)
{
if (StatisticsInfo != null && StatisticsInfo.Length > 60)
{
log.Warning("Length > 60 - truncated");
StatisticsInfo = StatisticsInfo.Substring(0,60);
}
Set_ValueNoCheck ("StatisticsInfo", StatisticsInfo);
}
/** Get Statistics.
@return Information to help profiling the system for solving support issues */
public String GetStatisticsInfo() 
{
return (String)Get_Value("StatisticsInfo");
}
/** Set Summary.
@param Summary Textual summary of this request */
public void SetSummary (String Summary)
{
if (Summary != null && Summary.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Summary = Summary.Substring(0,2000);
}
Set_Value ("Summary", Summary);
}
/** Get Summary.
@return Textual summary of this request */
public String GetSummary() 
{
return (String)Get_Value("Summary");
}
/** Set Support EMail.
@param SupportEMail EMail address to send support information and updates to */
public void SetSupportEMail (String SupportEMail)
{
if (SupportEMail != null && SupportEMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
SupportEMail = SupportEMail.Substring(0,60);
}
Set_Value ("SupportEMail", SupportEMail);
}
/** Get Support EMail.
@return EMail address to send support information and updates to */
public String GetSupportEMail() 
{
return (String)Get_Value("SupportEMail");
}
/** Set Support Expires.
@param SupportExpDate Date when the Vienna support expires */
public void SetSupportExpDate (DateTime? SupportExpDate)
{
Set_ValueNoCheck ("SupportExpDate", (DateTime?)SupportExpDate);
}
/** Get Support Expires.
@return Date when the Vienna support expires */
public DateTime? GetSupportExpDate() 
{
return (DateTime?)Get_Value("SupportExpDate");
}

/** SupportLevel AD_Reference_ID=412 */
public static int SUPPORTLEVEL_AD_Reference_ID=412;
/** Enterprise = E */
public static String SUPPORTLEVEL_Enterprise = "E";
/** Standard = S */
public static String SUPPORTLEVEL_Standard = "S";
/** Unsupported = U */
public static String SUPPORTLEVEL_Unsupported = "U";
/** Self-Service = X */
public static String SUPPORTLEVEL_Self_Service = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsSupportLevelValid (String test)
{
return test == null || test.Equals("E") || test.Equals("S") || test.Equals("U") || test.Equals("X");
}
/** Set Support Level.
@param SupportLevel Subscribed Support level */
public void SetSupportLevel (String SupportLevel)
{
if (!IsSupportLevelValid(SupportLevel))
throw new ArgumentException ("SupportLevel Invalid value - " + SupportLevel + " - Reference_ID=412 - E - S - U - X");
if (SupportLevel != null && SupportLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
SupportLevel = SupportLevel.Substring(0,1);
}
Set_Value ("SupportLevel", SupportLevel);
}
/** Get Support Level.
@return Subscribed Support level */
public String GetSupportLevel() 
{
return (String)Get_Value("SupportLevel");
}
/** Set Support Units.
@param SupportUnits Number of Support Units, e.g. Supported Internal Users */
public void SetSupportUnits (int SupportUnits)
{
Set_ValueNoCheck ("SupportUnits", SupportUnits);
}
/** Get Support Units.
@return Number of Support Units, e.g. Supported Internal Users */
public int GetSupportUnits() 
{
Object ii = Get_Value("SupportUnits");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** SystemStatus AD_Reference_ID=374 */
public static int SYSTEMSTATUS_AD_Reference_ID=374;
/** Evaluation = E */
public static String SYSTEMSTATUS_Evaluation = "E";
/** Implementation = I */
public static String SYSTEMSTATUS_Implementation = "I";
/** Production = P */
public static String SYSTEMSTATUS_Production = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsSystemStatusValid (String test)
{
return test.Equals("E") || test.Equals("I") || test.Equals("P");
}
/** Set System Status.
@param SystemStatus Status of the system - Support priority depends on system status */
public void SetSystemStatus (String SystemStatus)
{
if (SystemStatus == null) throw new ArgumentException ("SystemStatus is mandatory");
if (!IsSystemStatusValid(SystemStatus))
throw new ArgumentException ("SystemStatus Invalid value - " + SystemStatus + " - Reference_ID=374 - E - I - P");
if (SystemStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
SystemStatus = SystemStatus.Substring(0,1);
}
Set_Value ("SystemStatus", SystemStatus);
}
/** Get System Status.
@return Status of the system - Support priority depends on system status */
public String GetSystemStatus() 
{
return (String)Get_Value("SystemStatus");
}
/** Set Registered EMail.
@param UserName Email of the responsible for the System */
public void SetUserName (String UserName)
{
if (UserName == null) throw new ArgumentException ("UserName is mandatory.");
if (UserName.Length > 60)
{
log.Warning("Length > 60 - truncated");
UserName = UserName.Substring(0,60);
}
Set_Value ("UserName", UserName);
}
/** Get Registered EMail.
@return Email of the responsible for the System */
public String GetUserName() 
{
return (String)Get_Value("UserName");
}
/** Set Version.
@param Version Version of the table definition */
public void SetVersion (String Version)
{
if (Version == null) throw new ArgumentException ("Version is mandatory.");
if (Version.Length > 20)
{
log.Warning("Length > 20 - truncated");
Version = Version.Substring(0,20);
}
Set_ValueNoCheck ("Version", Version);
}
/** Get Version.
@return Version of the table definition */
public String GetVersion() 
{
return (String)Get_Value("Version");
}
}

}
