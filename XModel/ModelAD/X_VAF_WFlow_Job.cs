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
/** Generated Model for VAF_WFlow_Job
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlow_Job : PO
{
public X_VAF_WFlow_Job (Context ctx, int VAF_WFlow_Job_ID, Trx trxName) : base (ctx, VAF_WFlow_Job_ID, trxName)
{
/** if (VAF_WFlow_Job_ID == 0)
{
SetVAF_TableView_ID (0);
SetVAF_WFlow_Job_ID (0);
SetVAF_WFlow_Incharge_ID (0);
SetVAF_Workflow_ID (0);
SetProcessed (false);	// N
SetRecord_ID (0);
SetWFState (null);
}
 */
}
public X_VAF_WFlow_Job (Ctx ctx, int VAF_WFlow_Job_ID, Trx trxName) : base (ctx, VAF_WFlow_Job_ID, trxName)
{
/** if (VAF_WFlow_Job_ID == 0)
{
SetVAF_TableView_ID (0);
SetVAF_WFlow_Job_ID (0);
SetVAF_WFlow_Incharge_ID (0);
SetVAF_Workflow_ID (0);
SetProcessed (false);	// N
SetRecord_ID (0);
SetWFState (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_Job (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_Job (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_Job (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlow_Job()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366294L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049505L;
/** VAF_TableView_ID=645 */
public static int Table_ID;
 // =645;

/** TableName=VAF_WFlow_Job */
public static String Table_Name="VAF_WFlow_Job";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlow_Job[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Message.
@param AD_Message_ID System Message */
public void SetAD_Message_ID (int AD_Message_ID)
{
if (AD_Message_ID <= 0) Set_Value ("AD_Message_ID", null);
else
Set_Value ("AD_Message_ID", AD_Message_ID);
}
/** Get Message.
@return System Message */
public int GetAD_Message_ID() 
{
Object ii = Get_Value("AD_Message_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_UserContact_ID AD_Reference_ID=286 */
public static int VAF_UserContact_ID_AD_Reference_ID=286;
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Process.
@param VAF_WFlow_Job_ID Actual Workflow Process Instance */
public void SetVAF_WFlow_Job_ID (int VAF_WFlow_Job_ID)
{
if (VAF_WFlow_Job_ID < 1) throw new ArgumentException ("VAF_WFlow_Job_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_Job_ID", VAF_WFlow_Job_ID);
}
/** Get Workflow Process.
@return Actual Workflow Process Instance */
public int GetVAF_WFlow_Job_ID() 
{
Object ii = Get_Value("VAF_WFlow_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Responsible.
@param VAF_WFlow_Incharge_ID Responsible for Workflow Execution */
public void SetVAF_WFlow_Incharge_ID (int VAF_WFlow_Incharge_ID)
{
if (VAF_WFlow_Incharge_ID < 1) throw new ArgumentException ("VAF_WFlow_Incharge_ID is mandatory.");
Set_Value ("VAF_WFlow_Incharge_ID", VAF_WFlow_Incharge_ID);
}
/** Get Workflow Responsible.
@return Responsible for Workflow Execution */
public int GetVAF_WFlow_Incharge_ID() 
{
Object ii = Get_Value("VAF_WFlow_Incharge_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow.
@param VAF_Workflow_ID Workflow or combination of tasks */
public void SetVAF_Workflow_ID (int VAF_Workflow_ID)
{
if (VAF_Workflow_ID < 1) throw new ArgumentException ("VAF_Workflow_ID is mandatory.");
Set_Value ("VAF_Workflow_ID", VAF_Workflow_ID);
}
/** Get Workflow.
@return Workflow or combination of tasks */
public int GetVAF_Workflow_ID() 
{
Object ii = Get_Value("VAF_Workflow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_Workflow_ID().ToString());
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
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) throw new ArgumentException ("Record_ID is mandatory.");
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
/** Set Text Message.
@param TextMsg Text Message */
public void SetTextMsg (String TextMsg)
{
if (TextMsg != null && TextMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
TextMsg = TextMsg.Substring(0,2000);
}
Set_Value ("TextMsg", TextMsg);
}
/** Get Text Message.
@return Text Message */
public String GetTextMsg() 
{
return (String)Get_Value("TextMsg");
}

/** WFState AD_Reference_ID=305 */
public static int WFSTATE_AD_Reference_ID=305;
/** Aborted = CA */
public static String WFSTATE_Aborted = "CA";
/** Completed = CC */
public static String WFSTATE_Completed = "CC";
/** Terminated = CT */
public static String WFSTATE_Terminated = "CT";
/** Not Started = ON */
public static String WFSTATE_NotStarted = "ON";
/** Running = OR */
public static String WFSTATE_Running = "OR";
/** Suspended = OS */
public static String WFSTATE_Suspended = "OS";
/** Background = BK */
public static String WFSTATE_Background = "BK";
    /** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsWFStateValid (String test)
{
return test.Equals("CA") || test.Equals("CC") || test.Equals("CT") || test.Equals("ON") || test.Equals("OR") || test.Equals("OS") || test.Equals("BK");
}
/** Set Workflow State.
@param WFState State of the execution of the workflow */
public void SetWFState (String WFState)
{
if (WFState == null) throw new ArgumentException ("WFState is mandatory");
if (!IsWFStateValid(WFState))
throw new ArgumentException ("WFState Invalid value - " + WFState + " - Reference_ID=305 - CA - CC - CT - ON - OR - OS - BK");
if (WFState.Length > 2)
{
log.Warning("Length > 2 - truncated");
WFState = WFState.Substring(0,2);
}
Set_Value ("WFState", WFState);
}
/** Get Workflow State.
@return State of the execution of the workflow */
public String GetWFState() 
{
return (String)Get_Value("WFState");
}

// vinay bhatt window id
private int AD_Window_ID;

public void SetAD_Window_ID(int ADWindow_ID)
{
    AD_Window_ID = (ADWindow_ID == null? 0 : ADWindow_ID);
}
public int GetAD_Window_ID()
{
    return AD_Window_ID;
}
}

}
