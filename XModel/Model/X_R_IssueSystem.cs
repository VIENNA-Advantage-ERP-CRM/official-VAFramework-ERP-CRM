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
/** Generated Model for R_IssueSystem
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_IssueSystem : PO
{
public X_R_IssueSystem (Context ctx, int R_IssueSystem_ID, Trx trxName) : base (ctx, R_IssueSystem_ID, trxName)
{
/** if (R_IssueSystem_ID == 0)
{
SetDBAddress (null);
SetR_IssueSystem_ID (0);
SetSystemStatus (null);
}
 */
}
public X_R_IssueSystem (Ctx ctx, int R_IssueSystem_ID, Trx trxName) : base (ctx, R_IssueSystem_ID, trxName)
{
/** if (R_IssueSystem_ID == 0)
{
SetDBAddress (null);
SetR_IssueSystem_ID (0);
SetSystemStatus (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueSystem (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueSystem (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueSystem (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_IssueSystem()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382939L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066150L;
/** AD_Table_ID=843 */
public static int Table_ID;
 // =843;

/** TableName=R_IssueSystem */
public static String Table_Name="R_IssueSystem";

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
StringBuilder sb = new StringBuilder ("X_R_IssueSystem[").Append(Get_ID()).Append("]");
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
/** Set DB Address.
@param DBAddress JDBC URL of the database server */
public void SetDBAddress (String DBAddress)
{
if (DBAddress == null) throw new ArgumentException ("DBAddress is mandatory.");
if (DBAddress.Length > 255)
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDBAddress());
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
/** Set Issue System.
@param R_IssueSystem_ID System creating the issue */
public void SetR_IssueSystem_ID (int R_IssueSystem_ID)
{
if (R_IssueSystem_ID < 1) throw new ArgumentException ("R_IssueSystem_ID is mandatory.");
Set_ValueNoCheck ("R_IssueSystem_ID", R_IssueSystem_ID);
}
/** Get Issue System.
@return System creating the issue */
public int GetR_IssueSystem_ID() 
{
Object ii = Get_Value("R_IssueSystem_ID");
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
Set_ValueNoCheck ("StatisticsInfo", StatisticsInfo);
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
