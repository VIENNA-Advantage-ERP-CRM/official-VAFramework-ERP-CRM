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
/** Generated Model for AD_Workflow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Workflow : PO
{
public X_AD_Workflow (Context ctx, int AD_Workflow_ID, Trx trxName) : base (ctx, AD_Workflow_ID, trxName)
{
/** if (AD_Workflow_ID == 0)
{
SetAD_Workflow_ID (0);
SetAccessLevel (null);
SetAuthor (null);
SetDuration (0);
SetEntityType (null);	// U
SetIsDefault (false);
SetIsValid (false);
SetName (null);
SetPublishStatus (null);	// U
SetValue (null);
SetVersion (0);
SetWaitingTime (0);
SetWorkflowType (null);	// G
SetWorkingTime (0);
}
 */
}
public X_AD_Workflow (Ctx ctx, int AD_Workflow_ID, Trx trxName) : base (ctx, AD_Workflow_ID, trxName)
{
/** if (AD_Workflow_ID == 0)
{
SetAD_Workflow_ID (0);
SetAccessLevel (null);
SetAuthor (null);
SetDuration (0);
SetEntityType (null);	// U
SetIsDefault (false);
SetIsValid (false);
SetName (null);
SetPublishStatus (null);	// U
SetValue (null);
SetVersion (0);
SetWaitingTime (0);
SetWorkflowType (null);	// G
SetWorkingTime (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Workflow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Workflow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Workflow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Workflow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366827L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050038L;
/** AD_Table_ID=117 */
public static int Table_ID;
 // =117;

/** TableName=AD_Workflow */
public static String Table_Name="AD_Workflow";

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
StringBuilder sb = new StringBuilder ("X_AD_Workflow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Node.
@param AD_WF_Node_ID Workflow Node (activity), step or process */
public void SetAD_WF_Node_ID (int AD_WF_Node_ID)
{
if (AD_WF_Node_ID <= 0) Set_Value ("AD_WF_Node_ID", null);
else
Set_Value ("AD_WF_Node_ID", AD_WF_Node_ID);
}
/** Get Node.
@return Workflow Node (activity), step or process */
public int GetAD_WF_Node_ID() 
{
Object ii = Get_Value("AD_WF_Node_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Responsible.
@param AD_WF_Responsible_ID Responsible for Workflow Execution */
public void SetAD_WF_Responsible_ID (int AD_WF_Responsible_ID)
{
if (AD_WF_Responsible_ID <= 0) Set_Value ("AD_WF_Responsible_ID", null);
else
Set_Value ("AD_WF_Responsible_ID", AD_WF_Responsible_ID);
}
/** Get Workflow Responsible.
@return Responsible for Workflow Execution */
public int GetAD_WF_Responsible_ID() 
{
Object ii = Get_Value("AD_WF_Responsible_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Processor.
@param AD_WorkflowProcessor_ID Workflow Processor Server */
public void SetAD_WorkflowProcessor_ID (int AD_WorkflowProcessor_ID)
{
if (AD_WorkflowProcessor_ID <= 0) Set_Value ("AD_WorkflowProcessor_ID", null);
else
Set_Value ("AD_WorkflowProcessor_ID", AD_WorkflowProcessor_ID);
}
/** Get Workflow Processor.
@return Workflow Processor Server */
public int GetAD_WorkflowProcessor_ID() 
{
Object ii = Get_Value("AD_WorkflowProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow.
@param AD_Workflow_ID Workflow or combination of tasks */
public void SetAD_Workflow_ID (int AD_Workflow_ID)
{
if (AD_Workflow_ID < 1) throw new ArgumentException ("AD_Workflow_ID is mandatory.");
Set_ValueNoCheck ("AD_Workflow_ID", AD_Workflow_ID);
}
/** Get Workflow.
@return Workflow or combination of tasks */
public int GetAD_Workflow_ID() 
{
Object ii = Get_Value("AD_Workflow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AccessLevel AD_Reference_ID=5 */
public static int ACCESSLEVEL_AD_Reference_ID=5;
/** Organization = 1 */
public static String ACCESSLEVEL_Organization = "1";
/** Client only = 2 */
public static String ACCESSLEVEL_ClientOnly = "2";
/** Client+Organization = 3 */
public static String ACCESSLEVEL_ClientPlusOrganization = "3";
/** System only = 4 */
public static String ACCESSLEVEL_SystemOnly = "4";
/** System+Client = 6 */
public static String ACCESSLEVEL_SystemPlusClient = "6";
/** All = 7 */
public static String ACCESSLEVEL_All = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccessLevelValid (String test)
{
return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("6") || test.Equals("7");
}
/** Set Data Access Level.
@param AccessLevel Access Level required */
public void SetAccessLevel (String AccessLevel)
{
if (AccessLevel == null) throw new ArgumentException ("AccessLevel is mandatory");
if (!IsAccessLevelValid(AccessLevel))
throw new ArgumentException ("AccessLevel Invalid value - " + AccessLevel + " - Reference_ID=5 - 1 - 2 - 3 - 4 - 6 - 7");
if (AccessLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccessLevel = AccessLevel.Substring(0,1);
}
Set_Value ("AccessLevel", AccessLevel);
}
/** Get Data Access Level.
@return Access Level required */
public String GetAccessLevel() 
{
return (String)Get_Value("AccessLevel");
}
/** Set Author.
@param Author Author/Creator of the Entity */
public void SetAuthor (String Author)
{
if (Author == null) throw new ArgumentException ("Author is mandatory.");
if (Author.Length > 20)
{
log.Warning("Length > 20 - truncated");
Author = Author.Substring(0,20);
}
Set_Value ("Author", Author);
}
/** Get Author.
@return Author/Creator of the Entity */
public String GetAuthor() 
{
return (String)Get_Value("Author");
}
/** Set Cost.
@param Cost Cost information */
public void SetCost (Decimal? Cost)
{
Set_Value ("Cost", (Decimal?)Cost);
}
/** Get Cost.
@return Cost information */
public Decimal GetCost() 
{
Object bd =Get_Value("Cost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Document Value Logic.
@param DocValueLogic Logic to determine Workflow Start - If true, a workflow process is started for the document */
public void SetDocValueLogic (String DocValueLogic)
{
if (DocValueLogic != null && DocValueLogic.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DocValueLogic = DocValueLogic.Substring(0,2000);
}
Set_Value ("DocValueLogic", DocValueLogic);
}
/** Get Document Value Logic.
@return Logic to determine Workflow Start - If true, a workflow process is started for the document */
public String GetDocValueLogic() 
{
return (String)Get_Value("DocValueLogic");
}
/** Set Duration.
@param Duration Normal Duration in Duration Unit */
public void SetDuration (int Duration)
{
Set_Value ("Duration", Duration);
}
/** Get Duration.
@return Normal Duration in Duration Unit */
public int GetDuration() 
{
Object ii = Get_Value("Duration");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Duration Limit.
@param DurationLimit Maximum Duration in Duration Unit */
public void SetDurationLimit (int DurationLimit)
{
Set_Value ("DurationLimit", DurationLimit);
}
/** Get Duration Limit.
@return Maximum Duration in Duration Unit */
public int GetDurationLimit() 
{
Object ii = Get_Value("DurationLimit");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** DurationUnit AD_Reference_ID=299 */
public static int DURATIONUNIT_AD_Reference_ID=299;
/** Day = D */
public static String DURATIONUNIT_Day = "D";
/** Month = M */
public static String DURATIONUNIT_Month = "M";
/** Year = Y */
public static String DURATIONUNIT_Year = "Y";
/** hour = h */
public static String DURATIONUNIT_Hour = "h";
/** minute = m */
public static String DURATIONUNIT_Minute = "m";
/** second = s */
public static String DURATIONUNIT_Second = "s";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDurationUnitValid (String test)
{
return test == null || test.Equals("D") || test.Equals("M") || test.Equals("Y") || test.Equals("h") || test.Equals("m") || test.Equals("s");
}
/** Set Duration Unit.
@param DurationUnit Unit of Duration */
public void SetDurationUnit (String DurationUnit)
{
if (!IsDurationUnitValid(DurationUnit))
throw new ArgumentException ("DurationUnit Invalid value - " + DurationUnit + " - Reference_ID=299 - D - M - Y - h - m - s");
if (DurationUnit != null && DurationUnit.Length > 1)
{
log.Warning("Length > 1 - truncated");
DurationUnit = DurationUnit.Substring(0,1);
}
Set_Value ("DurationUnit", DurationUnit);
}
/** Get Duration Unit.
@return Unit of Duration */
public String GetDurationUnit() 
{
return (String)Get_Value("DurationUnit");
}

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Priority.
@param Priority Indicates if this request is of a high, medium or low priority. */
public void SetPriority (int Priority)
{
Set_Value ("Priority", Priority);
}
/** Get Priority.
@return Indicates if this request is of a high, medium or low priority. */
public int GetPriority() 
{
Object ii = Get_Value("Priority");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PublishStatus AD_Reference_ID=310 */
public static int PUBLISHSTATUS_AD_Reference_ID=310;
/** Released = R */
public static String PUBLISHSTATUS_Released = "R";
/** Test = T */
public static String PUBLISHSTATUS_Test = "T";
/** Under Revision = U */
public static String PUBLISHSTATUS_UnderRevision = "U";
/** Void = V */
public static String PUBLISHSTATUS_Void = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPublishStatusValid (String test)
{
return test.Equals("R") || test.Equals("T") || test.Equals("U") || test.Equals("V");
}
/** Set Publication Status.
@param PublishStatus Status of Publication */
public void SetPublishStatus (String PublishStatus)
{
if (PublishStatus == null) throw new ArgumentException ("PublishStatus is mandatory");
if (!IsPublishStatusValid(PublishStatus))
throw new ArgumentException ("PublishStatus Invalid value - " + PublishStatus + " - Reference_ID=310 - R - T - U - V");
if (PublishStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
PublishStatus = PublishStatus.Substring(0,1);
}
Set_Value ("PublishStatus", PublishStatus);
}
/** Get Publication Status.
@return Status of Publication */
public String GetPublishStatus() 
{
return (String)Get_Value("PublishStatus");
}
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
/** Set Validate Workflow.
@param ValidateWorkflow Validate Workflow */
public void SetValidateWorkflow (String ValidateWorkflow)
{
if (ValidateWorkflow != null && ValidateWorkflow.Length > 1)
{
log.Warning("Length > 1 - truncated");
ValidateWorkflow = ValidateWorkflow.Substring(0,1);
}
Set_Value ("ValidateWorkflow", ValidateWorkflow);
}
/** Get Validate Workflow.
@return Validate Workflow */
public String GetValidateWorkflow() 
{
return (String)Get_Value("ValidateWorkflow");
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
/** Set Version.
@param Version Version of the table definition */
public void SetVersion (int Version)
{
Set_Value ("Version", Version);
}
/** Get Version.
@return Version of the table definition */
public int GetVersion() 
{
Object ii = Get_Value("Version");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Waiting Time.
@param WaitingTime Workflow Simulation Waiting time */
public void SetWaitingTime (int WaitingTime)
{
Set_Value ("WaitingTime", WaitingTime);
}
/** Get Waiting Time.
@return Workflow Simulation Waiting time */
public int GetWaitingTime() 
{
Object ii = Get_Value("WaitingTime");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** WorkflowType AD_Reference_ID=328 */
public static int WORKFLOWTYPE_AD_Reference_ID=328;
/** General = G */
public static String WORKFLOWTYPE_General = "G";
/** Document Process = P */
public static String WORKFLOWTYPE_DocumentProcess = "P";
/** Document Value = V */
public static String WORKFLOWTYPE_DocumentValue = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsWorkflowTypeValid (String test)
{
return test.Equals("G") || test.Equals("P") || test.Equals("V");
}
/** Set Workflow Type.
@param WorkflowType Type of Worflow */
public void SetWorkflowType (String WorkflowType)
{
if (WorkflowType == null) throw new ArgumentException ("WorkflowType is mandatory");
if (!IsWorkflowTypeValid(WorkflowType))
throw new ArgumentException ("WorkflowType Invalid value - " + WorkflowType + " - Reference_ID=328 - G - P - V");
if (WorkflowType.Length > 1)
{
log.Warning("Length > 1 - truncated");
WorkflowType = WorkflowType.Substring(0,1);
}
Set_Value ("WorkflowType", WorkflowType);
}
/** Get Workflow Type.
@return Type of Worflow */
public String GetWorkflowType() 
{
return (String)Get_Value("WorkflowType");
}
/** Set Working Time.
@param WorkingTime Workflow Simulation Execution Time */
public void SetWorkingTime (int WorkingTime)
{
Set_Value ("WorkingTime", WorkingTime);
}
/** Get Working Time.
@return Workflow Simulation Execution Time */
public int GetWorkingTime() 
{
Object ii = Get_Value("WorkingTime");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
