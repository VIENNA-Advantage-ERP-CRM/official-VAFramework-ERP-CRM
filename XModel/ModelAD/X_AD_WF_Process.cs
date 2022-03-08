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
/** Generated Model for AD_WF_Process
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_Process : PO
{
public X_AD_WF_Process (Context ctx, int AD_WF_Process_ID, Trx trxName) : base (ctx, AD_WF_Process_ID, trxName)
{
/** if (AD_WF_Process_ID == 0)
{
SetAD_Table_ID (0);
SetAD_WF_Process_ID (0);
SetAD_WF_Responsible_ID (0);
SetAD_Workflow_ID (0);
SetProcessed (false);	// N
SetRecord_ID (0);
SetWFState (null);
}
 */
}
public X_AD_WF_Process (Ctx ctx, int AD_WF_Process_ID, Trx trxName) : base (ctx, AD_WF_Process_ID, trxName)
{
/** if (AD_WF_Process_ID == 0)
{
SetAD_Table_ID (0);
SetAD_WF_Process_ID (0);
SetAD_WF_Responsible_ID (0);
SetAD_Workflow_ID (0);
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
public X_AD_WF_Process (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Process (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Process (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_Process()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366294L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049505L;
/** AD_Table_ID=645 */
public static int Table_ID;
 // =645;

/** TableName=AD_WF_Process */
public static String Table_Name="AD_WF_Process";

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
StringBuilder sb = new StringBuilder ("X_AD_WF_Process[").Append(Get_ID()).Append("]");
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
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
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

/** AD_User_ID AD_Reference_ID=286 */
public static int AD_USER_ID_AD_Reference_ID=286;
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Process.
@param AD_WF_Process_ID Actual Workflow Process Instance */
public void SetAD_WF_Process_ID (int AD_WF_Process_ID)
{
if (AD_WF_Process_ID < 1) throw new ArgumentException ("AD_WF_Process_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Process_ID", AD_WF_Process_ID);
}
/** Get Workflow Process.
@return Actual Workflow Process Instance */
public int GetAD_WF_Process_ID() 
{
Object ii = Get_Value("AD_WF_Process_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Responsible.
@param AD_WF_Responsible_ID Responsible for Workflow Execution */
public void SetAD_WF_Responsible_ID (int AD_WF_Responsible_ID)
{
if (AD_WF_Responsible_ID < 1) throw new ArgumentException ("AD_WF_Responsible_ID is mandatory.");
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
/** Set Workflow.
@param AD_Workflow_ID Workflow or combination of tasks */
public void SetAD_Workflow_ID (int AD_Workflow_ID)
{
if (AD_Workflow_ID < 1) throw new ArgumentException ("AD_Workflow_ID is mandatory.");
Set_Value ("AD_Workflow_ID", AD_Workflow_ID);
}
/** Get Workflow.
@return Workflow or combination of tasks */
public int GetAD_Workflow_ID() 
{
Object ii = Get_Value("AD_Workflow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Workflow_ID().ToString());
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
