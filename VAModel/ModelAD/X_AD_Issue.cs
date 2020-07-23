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
/** Generated Model for AD_Issue
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Issue : PO
{
public X_AD_Issue (Context ctx, int AD_Issue_ID, Trx trxName) : base (ctx, AD_Issue_ID, trxName)
{
/** if (AD_Issue_ID == 0)
{
SetAD_Issue_ID (0);
SetIssueSummary (null);
SetName (null);	// .
SetProcessed (false);	// N
SetReleaseNo (null);	// .
SetSystemStatus (null);	// E
SetUserName (null);	// .
SetVersion (null);	// .
}
 */
}
public X_AD_Issue (Ctx ctx, int AD_Issue_ID, Trx trxName) : base (ctx, AD_Issue_ID, trxName)
{
/** if (AD_Issue_ID == 0)
{
SetAD_Issue_ID (0);
SetIssueSummary (null);
SetName (null);	// .
SetProcessed (false);	// N
SetReleaseNo (null);	// .
SetSystemStatus (null);	// E
SetUserName (null);	// .
SetVersion (null);	// .
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Issue (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Issue (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Issue (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Issue()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361859L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045070L;
/** AD_Table_ID=828 */
public static int Table_ID;
 // =828;

/** TableName=AD_Issue */
public static String Table_Name="AD_Issue";

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
StringBuilder sb = new StringBuilder ("X_AD_Issue[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Special Form.
@param AD_Form_ID Special Form */
public void SetAD_Form_ID (int AD_Form_ID)
{
if (AD_Form_ID <= 0) Set_Value ("AD_Form_ID", null);
else
Set_Value ("AD_Form_ID", AD_Form_ID);
}
/** Get Special Form.
@return Special Form */
public int GetAD_Form_ID() 
{
Object ii = Get_Value("AD_Form_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set System Issue.
@param AD_Issue_ID Automatically created or manually entered System Issue */
public void SetAD_Issue_ID (int AD_Issue_ID)
{
if (AD_Issue_ID < 1) throw new ArgumentException ("AD_Issue_ID is mandatory.");
Set_ValueNoCheck ("AD_Issue_ID", AD_Issue_ID);
}
/** Get System Issue.
@return Automatically created or manually entered System Issue */
public int GetAD_Issue_ID() 
{
Object ii = Get_Value("AD_Issue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process.
@param AD_Process_ID Process or Report */
public void SetAD_Process_ID (int AD_Process_ID)
{
if (AD_Process_ID <= 0) Set_Value ("AD_Process_ID", null);
else
Set_Value ("AD_Process_ID", AD_Process_ID);
}
/** Get Process.
@return Process or Report */
public int GetAD_Process_ID() 
{
Object ii = Get_Value("AD_Process_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID <= 0) Set_Value ("AD_Window_ID", null);
else
Set_Value ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID <= 0) Set_ValueNoCheck ("A_Asset_ID", null);
else
Set_ValueNoCheck ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Comments.
@param Comments Comments or additional information */
public void SetComments (String Comments)
{
if (Comments != null && Comments.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Comments = Comments.Substring(0,2000);
}
Set_Value ("Comments", Comments);
}
/** Get Comments.
@return Comments or additional information */
public String GetComments() 
{
return (String)Get_Value("Comments");
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
Set_ValueNoCheck ("DBAddress", DBAddress);
}
/** Get DB Address.
@return JDBC URL of the database server */
public String GetDBAddress() 
{
return (String)Get_Value("DBAddress");
}
/** Set Database.
@param DatabaseInfo Database Information */
public void SetDatabaseInfo (String DatabaseInfo)
{
if (DatabaseInfo != null && DatabaseInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
DatabaseInfo = DatabaseInfo.Substring(0,255);
}
Set_ValueNoCheck ("DatabaseInfo", DatabaseInfo);
}
/** Get Database.
@return Database Information */
public String GetDatabaseInfo() 
{
return (String)Get_Value("DatabaseInfo");
}
/** Set Error Trace.
@param ErrorTrace System Error Trace */
public void SetErrorTrace (String ErrorTrace)
{
if (ErrorTrace != null && ErrorTrace.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ErrorTrace = ErrorTrace.Substring(0,2000);
}
Set_Value ("ErrorTrace", ErrorTrace);
}
/** Get Error Trace.
@return System Error Trace */
public String GetErrorTrace() 
{
return (String)Get_Value("ErrorTrace");
}

/** IsReproducible AD_Reference_ID=319 */
public static int ISREPRODUCIBLE_AD_Reference_ID=319;
/** No = N */
public static String ISREPRODUCIBLE_No = "N";
/** Yes = Y */
public static String ISREPRODUCIBLE_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsReproducibleValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Reproducible.
@param IsReproducible Problem can re reproduced in Gardenworld */
public void SetIsReproducible (String IsReproducible)
{
if (!IsIsReproducibleValid(IsReproducible))
throw new ArgumentException ("IsReproducible Invalid value - " + IsReproducible + " - Reference_ID=319 - N - Y");
if (IsReproducible != null && IsReproducible.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsReproducible = IsReproducible.Substring(0,1);
}
Set_Value ("IsReproducible", IsReproducible);
}
/** Get Reproducible.
@return Problem can re reproduced in Gardenworld */
public String GetIsReproducible() 
{
return (String)Get_Value("IsReproducible");
}

/** IsVanillaSystem AD_Reference_ID=319 */
public static int ISVANILLASYSTEM_AD_Reference_ID=319;
/** No = N */
public static String ISVANILLASYSTEM_No = "N";
/** Yes = Y */
public static String ISVANILLASYSTEM_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsVanillaSystemValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Vanilla System.
@param IsVanillaSystem The system was NOT compiled from Source - i.e. standard distribution */
public void SetIsVanillaSystem (String IsVanillaSystem)
{
if (!IsIsVanillaSystemValid(IsVanillaSystem))
throw new ArgumentException ("IsVanillaSystem Invalid value - " + IsVanillaSystem + " - Reference_ID=319 - N - Y");
if (IsVanillaSystem != null && IsVanillaSystem.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsVanillaSystem = IsVanillaSystem.Substring(0,1);
}
Set_Value ("IsVanillaSystem", IsVanillaSystem);
}
/** Get Vanilla System.
@return The system was NOT compiled from Source - i.e. standard distribution */
public String GetIsVanillaSystem() 
{
return (String)Get_Value("IsVanillaSystem");
}

/** IssueSource AD_Reference_ID=104 */
public static int ISSUESOURCE_AD_Reference_ID=104;
/** Workbench = B */
public static String ISSUESOURCE_Workbench = "B";
/** WorkFlow = F */
public static String ISSUESOURCE_WorkFlow = "F";
/** Process = P */
public static String ISSUESOURCE_Process = "P";
/** Report = R */
public static String ISSUESOURCE_Report = "R";
/** Task = T */
public static String ISSUESOURCE_Task = "T";
/** Window = W */
public static String ISSUESOURCE_Window = "W";
/** Form = X */
public static String ISSUESOURCE_Form = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIssueSourceValid (String test)
{
return test == null || test.Equals("B") || test.Equals("F") || test.Equals("P") || test.Equals("R") || test.Equals("T") || test.Equals("W") || test.Equals("X");
}
/** Set Source.
@param IssueSource Issue Source */
public void SetIssueSource (String IssueSource)
{
if (!IsIssueSourceValid(IssueSource))
throw new ArgumentException ("IssueSource Invalid value - " + IssueSource + " - Reference_ID=104 - B - F - P - R - T - W - X");
if (IssueSource != null && IssueSource.Length > 1)
{
log.Warning("Length > 1 - truncated");
IssueSource = IssueSource.Substring(0,1);
}
Set_Value ("IssueSource", IssueSource);
}
/** Get Source.
@return Issue Source */
public String GetIssueSource() 
{
return (String)Get_Value("IssueSource");
}
/** Set Issue Summary.
@param IssueSummary Issue Summary */
public void SetIssueSummary (String IssueSummary)
{
if (IssueSummary == null) throw new ArgumentException ("IssueSummary is mandatory.");
if (IssueSummary.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
IssueSummary = IssueSummary.Substring(0,2000);
}
Set_Value ("IssueSummary", IssueSummary);
}
/** Get Issue Summary.
@return Issue Summary */
public String GetIssueSummary() 
{
return (String)Get_Value("IssueSummary");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetIssueSummary());
}
/** Set Java Info.
@param JavaInfo Java Version Info */
public void SetJavaInfo (String JavaInfo)
{
if (JavaInfo != null && JavaInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
JavaInfo = JavaInfo.Substring(0,255);
}
Set_ValueNoCheck ("JavaInfo", JavaInfo);
}
/** Get Java Info.
@return Java Version Info */
public String GetJavaInfo() 
{
return (String)Get_Value("JavaInfo");
}
/** Set Line.
@param LineNo Line No */
public void SetLineNo (int LineNo)
{
Set_Value ("LineNo", LineNo);
}
/** Get Line.
@return Line No */
public int GetLineNo() 
{
Object ii = Get_Value("LineNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Local Host.
@param Local_Host Local Host Info */
public void SetLocal_Host (String Local_Host)
{
if (Local_Host != null && Local_Host.Length > 120)
{
log.Warning("Length > 120 - truncated");
Local_Host = Local_Host.Substring(0,120);
}
Set_ValueNoCheck ("Local_Host", Local_Host);
}
/** Get Local Host.
@return Local Host Info */
public String GetLocal_Host() 
{
return (String)Get_Value("Local_Host");
}
/** Set Logger.
@param LoggerName Logger Name */
public void SetLoggerName (String LoggerName)
{
if (LoggerName != null && LoggerName.Length > 60)
{
log.Warning("Length > 60 - truncated");
LoggerName = LoggerName.Substring(0,60);
}
Set_Value ("LoggerName", LoggerName);
}
/** Get Logger.
@return Logger Name */
public String GetLoggerName() 
{
return (String)Get_Value("LoggerName");
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
Set_ValueNoCheck ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Operating System.
@param OperatingSystemInfo Operating System Info */
public void SetOperatingSystemInfo (String OperatingSystemInfo)
{
if (OperatingSystemInfo != null && OperatingSystemInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
OperatingSystemInfo = OperatingSystemInfo.Substring(0,255);
}
Set_ValueNoCheck ("OperatingSystemInfo", OperatingSystemInfo);
}
/** Get Operating System.
@return Operating System Info */
public String GetOperatingSystemInfo() 
{
return (String)Get_Value("OperatingSystemInfo");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_ValueNoCheck ("Processed", Processed);
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
if (ProfileInfo != null && ProfileInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
ProfileInfo = ProfileInfo.Substring(0,255);
}
Set_ValueNoCheck ("ProfileInfo", ProfileInfo);
}
/** Get Profile.
@return Information to help profiling the system for solving support issues */
public String GetProfileInfo() 
{
return (String)Get_Value("ProfileInfo");
}
/** Set Known Issue.
@param R_IssueKnown_ID Known Issue */
public void SetR_IssueKnown_ID (int R_IssueKnown_ID)
{
if (R_IssueKnown_ID <= 0) Set_Value ("R_IssueKnown_ID", null);
else
Set_Value ("R_IssueKnown_ID", R_IssueKnown_ID);
}
/** Get Known Issue.
@return Known Issue */
public int GetR_IssueKnown_ID() 
{
Object ii = Get_Value("R_IssueKnown_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Issue Project.
@param R_IssueProject_ID Implementation Projects */
public void SetR_IssueProject_ID (int R_IssueProject_ID)
{
if (R_IssueProject_ID <= 0) Set_Value ("R_IssueProject_ID", null);
else
Set_Value ("R_IssueProject_ID", R_IssueProject_ID);
}
/** Get Issue Project.
@return Implementation Projects */
public int GetR_IssueProject_ID() 
{
Object ii = Get_Value("R_IssueProject_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Issue System.
@param R_IssueSystem_ID System creating the issue */
public void SetR_IssueSystem_ID (int R_IssueSystem_ID)
{
if (R_IssueSystem_ID <= 0) Set_Value ("R_IssueSystem_ID", null);
else
Set_Value ("R_IssueSystem_ID", R_IssueSystem_ID);
}
/** Get Issue System.
@return System creating the issue */
public int GetR_IssueSystem_ID() 
{
Object ii = Get_Value("R_IssueSystem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Issue User.
@param R_IssueUser_ID User who reported issues */
public void SetR_IssueUser_ID (int R_IssueUser_ID)
{
if (R_IssueUser_ID <= 0) Set_Value ("R_IssueUser_ID", null);
else
Set_Value ("R_IssueUser_ID", R_IssueUser_ID);
}
/** Get Issue User.
@return User who reported issues */
public int GetR_IssueUser_ID() 
{
Object ii = Get_Value("R_IssueUser_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request.
@param R_Request_ID Request from a Business Partner or Prospect */
public void SetR_Request_ID (int R_Request_ID)
{
if (R_Request_ID <= 0) Set_ValueNoCheck ("R_Request_ID", null);
else
Set_ValueNoCheck ("R_Request_ID", R_Request_ID);
}
/** Get Request.
@return Request from a Business Partner or Prospect */
public int GetR_Request_ID() 
{
Object ii = Get_Value("R_Request_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_ValueNoCheck ("Record_ID", null);
else
Set_ValueNoCheck ("Record_ID", Record_ID);
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
/** Set Release Tag.
@param ReleaseTag Release Tag */
public void SetReleaseTag (String ReleaseTag)
{
if (ReleaseTag != null && ReleaseTag.Length > 60)
{
log.Warning("Length > 60 - truncated");
ReleaseTag = ReleaseTag.Substring(0,60);
}
Set_Value ("ReleaseTag", ReleaseTag);
}
/** Get Release Tag.
@return Release Tag */
public String GetReleaseTag() 
{
return (String)Get_Value("ReleaseTag");
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
Set_ValueNoCheck ("Remote_Addr", Remote_Addr);
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
Set_ValueNoCheck ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
}
/** Set Request Document No.
@param RequestDocumentNo Vienna Request Document No */
public void SetRequestDocumentNo (String RequestDocumentNo)
{
if (RequestDocumentNo != null && RequestDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
RequestDocumentNo = RequestDocumentNo.Substring(0,30);
}
Set_ValueNoCheck ("RequestDocumentNo", RequestDocumentNo);
}
/** Get Request Document No.
@return Vienna Request Document No */
public String GetRequestDocumentNo() 
{
return (String)Get_Value("RequestDocumentNo");
}
/** Set Response Text.
@param ResponseText Request Response Text */
public void SetResponseText (String ResponseText)
{
if (ResponseText != null && ResponseText.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ResponseText = ResponseText.Substring(0,2000);
}
Set_ValueNoCheck ("ResponseText", ResponseText);
}
/** Get Response Text.
@return Request Response Text */
public String GetResponseText() 
{
return (String)Get_Value("ResponseText");
}
/** Set Source Class.
@param SourceClassName Source Class Name */
public void SetSourceClassName (String SourceClassName)
{
if (SourceClassName != null && SourceClassName.Length > 60)
{
log.Warning("Length > 60 - truncated");
SourceClassName = SourceClassName.Substring(0,60);
}
Set_Value ("SourceClassName", SourceClassName);
}
/** Get Source Class.
@return Source Class Name */
public String GetSourceClassName() 
{
return (String)Get_Value("SourceClassName");
}
/** Set Source Method.
@param SourceMethodName Source Method Name */
public void SetSourceMethodName (String SourceMethodName)
{
if (SourceMethodName != null && SourceMethodName.Length > 60)
{
log.Warning("Length > 60 - truncated");
SourceMethodName = SourceMethodName.Substring(0,60);
}
Set_Value ("SourceMethodName", SourceMethodName);
}
/** Get Source Method.
@return Source Method Name */
public String GetSourceMethodName() 
{
return (String)Get_Value("SourceMethodName");
}
/** Set Stack Trace.
@param StackTrace System Log Trace */
public void SetStackTrace (String StackTrace)
{
if (StackTrace != null && StackTrace.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
StackTrace = StackTrace.Substring(0,2000);
}
Set_Value ("StackTrace", StackTrace);
}
/** Get Stack Trace.
@return System Log Trace */
public String GetStackTrace() 
{
return (String)Get_Value("StackTrace");
}
/** Set Statistics.
@param StatisticsInfo Information to help profiling the system for solving support issues */
public void SetStatisticsInfo (String StatisticsInfo)
{
if (StatisticsInfo != null && StatisticsInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
StatisticsInfo = StatisticsInfo.Substring(0,255);
}
Set_ValueNoCheck ("StatisticsInfo", StatisticsInfo);
}
/** Get Statistics.
@return Information to help profiling the system for solving support issues */
public String GetStatisticsInfo() 
{
return (String)Get_Value("StatisticsInfo");
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
Set_ValueNoCheck ("UserName", UserName);
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
if (Version.Length > 40)
{
log.Warning("Length > 40 - truncated");
Version = Version.Substring(0,40);
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
