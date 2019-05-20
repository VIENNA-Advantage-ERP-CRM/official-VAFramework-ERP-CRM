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
/** Generated Model for R_IssueKnown
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_IssueKnown : PO
{
public X_R_IssueKnown (Context ctx, int R_IssueKnown_ID, Trx trxName) : base (ctx, R_IssueKnown_ID, trxName)
{
/** if (R_IssueKnown_ID == 0)
{
SetIssueSummary (null);
SetR_IssueKnown_ID (0);
SetReleaseNo (null);
}
 */
}
public X_R_IssueKnown (Ctx ctx, int R_IssueKnown_ID, Trx trxName) : base (ctx, R_IssueKnown_ID, trxName)
{
/** if (R_IssueKnown_ID == 0)
{
SetIssueSummary (null);
SetR_IssueKnown_ID (0);
SetReleaseNo (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueKnown (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueKnown (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueKnown (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_IssueKnown()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382829L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066040L;
/** AD_Table_ID=839 */
public static int Table_ID;
 // =839;

/** TableName=R_IssueKnown */
public static String Table_Name="R_IssueKnown";

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
StringBuilder sb = new StringBuilder ("X_R_IssueKnown[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Issue Status.
@param IssueStatus Current Status of the Issue */
public void SetIssueStatus (String IssueStatus)
{
if (IssueStatus != null && IssueStatus.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
IssueStatus = IssueStatus.Substring(0,2000);
}
Set_Value ("IssueStatus", IssueStatus);
}
/** Get Issue Status.
@return Current Status of the Issue */
public String GetIssueStatus() 
{
return (String)Get_Value("IssueStatus");
}
/** Set Issue Summary.
@param IssueSummary Issue Summary */
public void SetIssueSummary (String IssueSummary)
{
if (IssueSummary == null) throw new ArgumentException ("IssueSummary is mandatory.");
if (IssueSummary.Length > 255)
{
log.Warning("Length > 255 - truncated");
IssueSummary = IssueSummary.Substring(0,255);
}
Set_Value ("IssueSummary", IssueSummary);
}
/** Get Issue Summary.
@return Issue Summary */
public String GetIssueSummary() 
{
return (String)Get_Value("IssueSummary");
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
/** Set Known Issue.
@param R_IssueKnown_ID Known Issue */
public void SetR_IssueKnown_ID (int R_IssueKnown_ID)
{
if (R_IssueKnown_ID < 1) throw new ArgumentException ("R_IssueKnown_ID is mandatory.");
Set_ValueNoCheck ("R_IssueKnown_ID", R_IssueKnown_ID);
}
/** Get Known Issue.
@return Known Issue */
public int GetR_IssueKnown_ID() 
{
Object ii = Get_Value("R_IssueKnown_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Issue Recommendation.
@param R_IssueRecommendation_ID Recommendations how to fix an Issue */
public void SetR_IssueRecommendation_ID (int R_IssueRecommendation_ID)
{
if (R_IssueRecommendation_ID <= 0) Set_Value ("R_IssueRecommendation_ID", null);
else
Set_Value ("R_IssueRecommendation_ID", R_IssueRecommendation_ID);
}
/** Get Issue Recommendation.
@return Recommendations how to fix an Issue */
public int GetR_IssueRecommendation_ID() 
{
Object ii = Get_Value("R_IssueRecommendation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Issue Status.
@param R_IssueStatus_ID Status of an Issue */
public void SetR_IssueStatus_ID (int R_IssueStatus_ID)
{
if (R_IssueStatus_ID <= 0) Set_Value ("R_IssueStatus_ID", null);
else
Set_Value ("R_IssueStatus_ID", R_IssueStatus_ID);
}
/** Get Issue Status.
@return Status of an Issue */
public int GetR_IssueStatus_ID() 
{
Object ii = Get_Value("R_IssueStatus_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request.
@param R_Request_ID Request from a Business Partner or Prospect */
public void SetR_Request_ID (int R_Request_ID)
{
if (R_Request_ID <= 0) Set_Value ("R_Request_ID", null);
else
Set_Value ("R_Request_ID", R_Request_ID);
}
/** Get Request.
@return Request from a Business Partner or Prospect */
public int GetR_Request_ID() 
{
Object ii = Get_Value("R_Request_ID");
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
Set_Value ("ReleaseNo", ReleaseNo);
}
/** Get Release No.
@return Internal Release Number */
public String GetReleaseNo() 
{
return (String)Get_Value("ReleaseNo");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetReleaseNo());
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
}

}
