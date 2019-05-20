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
/** Generated Model for IP_Requirement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_IP_Requirement : PO
{
public X_IP_Requirement (Context ctx, int IP_Requirement_ID, Trx trxName) : base (ctx, IP_Requirement_ID, trxName)
{
/** if (IP_Requirement_ID == 0)
{
SetIP_Requirement_ID (0);
SetIsSummary (false);
SetName (null);
SetRequirementType (null);	// O
SetStandardQty (0.0);
SetTaskType (null);	// O
}
 */
}
public X_IP_Requirement (Ctx ctx, int IP_Requirement_ID, Trx trxName) : base (ctx, IP_Requirement_ID, trxName)
{
/** if (IP_Requirement_ID == 0)
{
SetIP_Requirement_ID (0);
SetIsSummary (false);
SetName (null);
SetRequirementType (null);	// O
SetStandardQty (0.0);
SetTaskType (null);	// O
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_IP_Requirement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_IP_Requirement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_IP_Requirement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_IP_Requirement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376670L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059881L;
/** AD_Table_ID=906 */
public static int Table_ID;
 // =906;

/** TableName=IP_Requirement */
public static String Table_Name="IP_Requirement";

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
StringBuilder sb = new StringBuilder ("X_IP_Requirement[").Append(Get_ID()).Append("]");
return sb.ToString();
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

/** C_ProjectPhaseReq_ID AD_Reference_ID=405 */
public static int C_PROJECTPHASEREQ_ID_AD_Reference_ID=405;
/** Set Requirement Phase.
@param C_ProjectPhaseReq_ID Project Requirements Phase */
public void SetC_ProjectPhaseReq_ID (int C_ProjectPhaseReq_ID)
{
if (C_ProjectPhaseReq_ID <= 0) Set_Value ("C_ProjectPhaseReq_ID", null);
else
Set_Value ("C_ProjectPhaseReq_ID", C_ProjectPhaseReq_ID);
}
/** Get Requirement Phase.
@return Project Requirements Phase */
public int GetC_ProjectPhaseReq_ID() 
{
Object ii = Get_Value("C_ProjectPhaseReq_ID");
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
/** Set Consequences.
@param Consequences Consequences of the entry */
public void SetConsequences (String Consequences)
{
if (Consequences != null && Consequences.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Consequences = Consequences.Substring(0,2000);
}
Set_Value ("Consequences", Consequences);
}
/** Get Consequences.
@return Consequences of the entry */
public String GetConsequences() 
{
return (String)Get_Value("Consequences");
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
/** Set Requirement.
@param IP_Requirement_ID Business Requirement */
public void SetIP_Requirement_ID (int IP_Requirement_ID)
{
if (IP_Requirement_ID < 1) throw new ArgumentException ("IP_Requirement_ID is mandatory.");
Set_ValueNoCheck ("IP_Requirement_ID", IP_Requirement_ID);
}
/** Get Requirement.
@return Business Requirement */
public int GetIP_Requirement_ID() 
{
Object ii = Get_Value("IP_Requirement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
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
/** Set Prerequisites.
@param Prerequisites Prerequisites of this entity */
public void SetPrerequisites (String Prerequisites)
{
if (Prerequisites != null && Prerequisites.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Prerequisites = Prerequisites.Substring(0,2000);
}
Set_Value ("Prerequisites", Prerequisites);
}
/** Get Prerequisites.
@return Prerequisites of this entity */
public String GetPrerequisites() 
{
return (String)Get_Value("Prerequisites");
}

/** RequirementType AD_Reference_ID=407 */
public static int REQUIREMENTTYPE_AD_Reference_ID=407;
/** Other = O */
public static String REQUIREMENTTYPE_Other = "O";
/** Determine Scope = S */
public static String REQUIREMENTTYPE_DetermineScope = "S";
/** Implementation Task = T */
public static String REQUIREMENTTYPE_ImplementationTask = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsRequirementTypeValid (String test)
{
return test.Equals("O") || test.Equals("S") || test.Equals("T");
}
/** Set Requirement Type.
@param RequirementType Requirement Type */
public void SetRequirementType (String RequirementType)
{
if (RequirementType == null) throw new ArgumentException ("RequirementType is mandatory");
if (!IsRequirementTypeValid(RequirementType))
throw new ArgumentException ("RequirementType Invalid value - " + RequirementType + " - Reference_ID=407 - O - S - T");
if (RequirementType.Length > 1)
{
log.Warning("Length > 1 - truncated");
RequirementType = RequirementType.Substring(0,1);
}
Set_Value ("RequirementType", RequirementType);
}
/** Get Requirement Type.
@return Requirement Type */
public String GetRequirementType() 
{
return (String)Get_Value("RequirementType");
}
/** Set Standard Quantity.
@param StandardQty Standard Quantity */
public void SetStandardQty (Decimal? StandardQty)
{
if (StandardQty == null) throw new ArgumentException ("StandardQty is mandatory.");
Set_Value ("StandardQty", (Decimal?)StandardQty);
}
/** Get Standard Quantity.
@return Standard Quantity */
public Decimal GetStandardQty() 
{
Object bd =Get_Value("StandardQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** TaskType AD_Reference_ID=408 */
public static int TASKTYPE_AD_Reference_ID=408;
/** Personal Activity = A */
public static String TASKTYPE_PersonalActivity = "A";
/** Delegation = D */
public static String TASKTYPE_Delegation = "D";
/** Other = O */
public static String TASKTYPE_Other = "O";
/** Research = R */
public static String TASKTYPE_Research = "R";
/** Test/Verify = T */
public static String TASKTYPE_TestVerify = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTaskTypeValid (String test)
{
return test.Equals("A") || test.Equals("D") || test.Equals("O") || test.Equals("R") || test.Equals("T");
}
/** Set Task Type.
@param TaskType Type of Project Task */
public void SetTaskType (String TaskType)
{
if (TaskType == null) throw new ArgumentException ("TaskType is mandatory");
if (!IsTaskTypeValid(TaskType))
throw new ArgumentException ("TaskType Invalid value - " + TaskType + " - Reference_ID=408 - A - D - O - R - T");
if (TaskType.Length > 1)
{
log.Warning("Length > 1 - truncated");
TaskType = TaskType.Substring(0,1);
}
Set_Value ("TaskType", TaskType);
}
/** Get Task Type.
@return Type of Project Task */
public String GetTaskType() 
{
return (String)Get_Value("TaskType");
}
}

}
