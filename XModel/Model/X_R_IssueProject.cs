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
/** Generated Model for R_IssueProject
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_IssueProject : PO
{
public X_R_IssueProject (Context ctx, int R_IssueProject_ID, Trx trxName) : base (ctx, R_IssueProject_ID, trxName)
{
/** if (R_IssueProject_ID == 0)
{
SetName (null);
SetR_IssueProject_ID (0);
SetSystemStatus (null);
}
 */
}
public X_R_IssueProject (Ctx ctx, int R_IssueProject_ID, Trx trxName) : base (ctx, R_IssueProject_ID, trxName)
{
/** if (R_IssueProject_ID == 0)
{
SetName (null);
SetR_IssueProject_ID (0);
SetSystemStatus (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueProject (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueProject (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueProject (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_IssueProject()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382845L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066056L;
/** AD_Table_ID=842 */
public static int Table_ID;
 // =842;

/** TableName=R_IssueProject */
public static String Table_Name="R_IssueProject";

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
StringBuilder sb = new StringBuilder ("X_R_IssueProject[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID <= 0) Set_Value ("A_Asset_ID", null);
else
Set_Value ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
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
/** Set Profile.
@param ProfileInfo Information to help profiling the system for solving support issues */
public void SetProfileInfo (String ProfileInfo)
{
if (ProfileInfo != null && ProfileInfo.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProfileInfo = ProfileInfo.Substring(0,60);
}
Set_Value ("ProfileInfo", ProfileInfo);
}
/** Get Profile.
@return Information to help profiling the system for solving support issues */
public String GetProfileInfo() 
{
return (String)Get_Value("ProfileInfo");
}
/** Set Issue Project.
@param R_IssueProject_ID Implementation Projects */
public void SetR_IssueProject_ID (int R_IssueProject_ID)
{
if (R_IssueProject_ID < 1) throw new ArgumentException ("R_IssueProject_ID is mandatory.");
Set_ValueNoCheck ("R_IssueProject_ID", R_IssueProject_ID);
}
/** Get Issue Project.
@return Implementation Projects */
public int GetR_IssueProject_ID() 
{
Object ii = Get_Value("R_IssueProject_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
Set_Value ("StatisticsInfo", StatisticsInfo);
}
/** Get Statistics.
@return Information to help profiling the system for solving support issues */
public String GetStatisticsInfo() 
{
return (String)Get_Value("StatisticsInfo");
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
}

}
