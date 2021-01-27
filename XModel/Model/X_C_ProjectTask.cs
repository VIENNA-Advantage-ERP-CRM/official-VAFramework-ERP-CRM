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
/** Generated Model for VAB_ProjectJob
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_ProjectJob : PO
{
public X_VAB_ProjectJob (Context ctx, int VAB_ProjectJob_ID, Trx trxName) : base (ctx, VAB_ProjectJob_ID, trxName)
{
/** if (VAB_ProjectJob_ID == 0)
{
SetVAB_ProjectStage_ID (0);
SetVAB_ProjectJob_ID (0);
SetCommittedAmt (0.0);
SetName (null);
SetPlannedAmt (0.0);
SetPlannedQty (0.0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_ProjectJob WHERE VAB_ProjectStage_ID=@VAB_ProjectStage_ID@
}
 */
}
public X_VAB_ProjectJob (Ctx ctx, int VAB_ProjectJob_ID, Trx trxName) : base (ctx, VAB_ProjectJob_ID, trxName)
{
/** if (VAB_ProjectJob_ID == 0)
{
SetVAB_ProjectStage_ID (0);
SetVAB_ProjectJob_ID (0);
SetCommittedAmt (0.0);
SetName (null);
SetPlannedAmt (0.0);
SetPlannedQty (0.0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_ProjectJob WHERE VAB_ProjectStage_ID=@VAB_ProjectStage_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectJob (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectJob (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectJob (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProjectJob()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374350L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057561L;
/** VAF_TableView_ID=584 */
public static int Table_ID;
 // =584;

/** TableName=VAB_ProjectJob */
public static String Table_Name="VAB_ProjectJob";

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
StringBuilder sb = new StringBuilder ("X_VAB_ProjectJob[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Project Phase.
@param VAB_ProjectStage_ID Phase of a Project */
public void SetVAB_ProjectStage_ID (int VAB_ProjectStage_ID)
{
if (VAB_ProjectStage_ID < 1) throw new ArgumentException ("VAB_ProjectStage_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectStage_ID", VAB_ProjectStage_ID);
}
/** Get Project Phase.
@return Phase of a Project */
public int GetVAB_ProjectStage_ID() 
{
Object ii = Get_Value("VAB_ProjectStage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_ProjectJobPrerequisite_ID VAF_Control_Ref_ID=409 */
public static int VAB_PROJECTJOBPREREQUISITE_ID_VAF_Control_Ref_ID=409;
/** Set Prerequisite Task.
@param VAB_ProjectJobPrerequisite_ID Task to be completed first before this can start */
public void SetVAB_ProjectJobPrerequisite_ID (int VAB_ProjectJobPrerequisite_ID)
{
if (VAB_ProjectJobPrerequisite_ID <= 0) Set_Value ("VAB_ProjectJobPrerequisite_ID", null);
else
Set_Value ("VAB_ProjectJobPrerequisite_ID", VAB_ProjectJobPrerequisite_ID);
}
/** Get Prerequisite Task.
@return Task to be completed first before this can start */
public int GetVAB_ProjectJobPrerequisite_ID() 
{
Object ii = Get_Value("VAB_ProjectJobPrerequisite_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Task.
@param VAB_ProjectJob_ID Actual Project Task in a Phase */
public void SetVAB_ProjectJob_ID (int VAB_ProjectJob_ID)
{
if (VAB_ProjectJob_ID < 1) throw new ArgumentException ("VAB_ProjectJob_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectJob_ID", VAB_ProjectJob_ID);
}
/** Get Project Task.
@return Actual Project Task in a Phase */
public int GetVAB_ProjectJob_ID() 
{
Object ii = Get_Value("VAB_ProjectJob_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Standard Task.
@param VAB_Std_Task_ID Standard Project Type Task */
public void SetVAB_Std_Task_ID (int VAB_Std_Task_ID)
{
if (VAB_Std_Task_ID <= 0) Set_ValueNoCheck ("VAB_Std_Task_ID", null);
else
Set_ValueNoCheck ("VAB_Std_Task_ID", VAB_Std_Task_ID);
}
/** Get Standard Task.
@return Standard Project Type Task */
public int GetVAB_Std_Task_ID() 
{
Object ii = Get_Value("VAB_Std_Task_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Committed Amount.
@param CommittedAmt The (legal) commitment amount */
public void SetCommittedAmt (Decimal? CommittedAmt)
{
if (CommittedAmt == null) throw new ArgumentException ("CommittedAmt is mandatory.");
Set_Value ("CommittedAmt", (Decimal?)CommittedAmt);
}
/** Get Committed Amount.
@return The (legal) commitment amount */
public Decimal GetCommittedAmt() 
{
Object bd =Get_Value("CommittedAmt");
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
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Planned Amount.
@param PlannedAmt Planned amount for this project */
public void SetPlannedAmt (Decimal? PlannedAmt)
{
if (PlannedAmt == null) throw new ArgumentException ("PlannedAmt is mandatory.");
Set_Value ("PlannedAmt", (Decimal?)PlannedAmt);
}
/** Get Planned Amount.
@return Planned amount for this project */
public Decimal GetPlannedAmt() 
{
Object bd =Get_Value("PlannedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Planned Quantity.
@param PlannedQty Planned quantity for this project */
public void SetPlannedQty (Decimal? PlannedQty)
{
if (PlannedQty == null) throw new ArgumentException ("PlannedQty is mandatory.");
Set_Value ("PlannedQty", (Decimal?)PlannedQty);
}
/** Get Planned Quantity.
@return Planned quantity for this project */
public Decimal GetPlannedQty() 
{
Object bd =Get_Value("PlannedQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** ProjInvoiceRule VAF_Control_Ref_ID=383 */
public static int PROJINVOICERULE_VAF_Control_Ref_ID=383;
/** None = - */
public static String PROJINVOICERULE_None = "-";
/** Committed Amount = C */
public static String PROJINVOICERULE_CommittedAmount = "C";
/** Product  Quantity = P */
public static String PROJINVOICERULE_ProductQuantity = "P";
/** Time&Material = T */
public static String PROJINVOICERULE_TimeMaterial = "T";
/** Time&Material max Comitted = c */
public static String PROJINVOICERULE_TimeMaterialMaxComitted = "c";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsProjInvoiceRuleValid (String test)
{
return test == null || test.Equals("-") || test.Equals("C") || test.Equals("P") || test.Equals("T") || test.Equals("c");
}
/** Set Invoice Rule.
@param ProjInvoiceRule Invoice Rule for the project */
public void SetProjInvoiceRule (String ProjInvoiceRule)
{
if (!IsProjInvoiceRuleValid(ProjInvoiceRule))
throw new ArgumentException ("ProjInvoiceRule Invalid value - " + ProjInvoiceRule + " - Reference_ID=383 - - - C - P - T - c");
if (ProjInvoiceRule != null && ProjInvoiceRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
ProjInvoiceRule = ProjInvoiceRule.Substring(0,1);
}
Set_Value ("ProjInvoiceRule", ProjInvoiceRule);
}
/** Get Invoice Rule.
@return Invoice Rule for the project */
public String GetProjInvoiceRule() 
{
return (String)Get_Value("ProjInvoiceRule");
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetSeqNo().ToString());
}

/** TaskType VAF_Control_Ref_ID=408 */
public static int TASKTYPE_VAF_Control_Ref_ID=408;
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
return test == null || test.Equals("A") || test.Equals("D") || test.Equals("O") || test.Equals("R") || test.Equals("T");
}
/** Set Task Type.
@param TaskType Type of Project Task */
public void SetTaskType (String TaskType)
{
if (!IsTaskTypeValid(TaskType))
throw new ArgumentException ("TaskType Invalid value - " + TaskType + " - Reference_ID=408 - A - D - O - R - T");
if (TaskType != null && TaskType.Length > 1)
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

/** Set AppointmentsInfo_ID.
@param AppointmentsInfo_ID  */
public void SetAppointmentsInfo_ID(int AppointmentsInfo_ID)
{
    if (AppointmentsInfo_ID <= 0) Set_Value("AppointmentsInfo_ID", null);
    else
        Set_Value("AppointmentsInfo_ID", AppointmentsInfo_ID);
}
/** Get AppointmentsInfo_ID.
@return AppointmentsInfo_ID*/
public int GetAppointmentsInfo_ID()
{
    Object ii = Get_Value("AppointmentsInfo_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate(DateTime? StartDate)
{
    Set_Value("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate()
{
    return (DateTime?)Get_Value("StartDate");
}
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate(DateTime? EndDate)
{
    Set_Value("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate()
{
    return (DateTime?)Get_Value("EndDate");
}

/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID(int SalesRep_ID)
{
    if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
    else
        Set_Value("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID()
{
    Object ii = Get_Value("SalesRep_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
