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
/** Generated Model for VAF_WFlowHandlerLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlowHandlerLog : PO
{
public X_VAF_WFlowHandlerLog (Context ctx, int VAF_WFlowHandlerLog_ID, Trx trxName) : base (ctx, VAF_WFlowHandlerLog_ID, trxName)
{
/** if (VAF_WFlowHandlerLog_ID == 0)
{
SetVAF_WFlowHandlerLog_ID (0);
SetVAF_WFlowHandler_ID (0);
SetIsError (false);
}
 */
}
public X_VAF_WFlowHandlerLog (Ctx ctx, int VAF_WFlowHandlerLog_ID, Trx trxName) : base (ctx, VAF_WFlowHandlerLog_ID, trxName)
{
/** if (VAF_WFlowHandlerLog_ID == 0)
{
SetVAF_WFlowHandlerLog_ID (0);
SetVAF_WFlowHandler_ID (0);
SetIsError (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlowHandlerLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlowHandlerLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlowHandlerLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlowHandlerLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366921L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050132L;
/** VAF_TableView_ID=696 */
public static int Table_ID;
 // =696;

/** TableName=VAF_WFlowHandlerLog */
public static String Table_Name="VAF_WFlowHandlerLog";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlowHandlerLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Workflow Processorl Log.
@param VAF_WFlowHandlerLog_ID Result of the execution of the Workflow Processor */
public void SetVAF_WFlowHandlerLog_ID (int VAF_WFlowHandlerLog_ID)
{
if (VAF_WFlowHandlerLog_ID < 1) throw new ArgumentException ("VAF_WFlowHandlerLog_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlowHandlerLog_ID", VAF_WFlowHandlerLog_ID);
}
/** Get Workflow Processorl Log.
@return Result of the execution of the Workflow Processor */
public int GetVAF_WFlowHandlerLog_ID() 
{
Object ii = Get_Value("VAF_WFlowHandlerLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Processor.
@param VAF_WFlowHandler_ID Workflow Processor Server */
public void SetVAF_WFlowHandler_ID (int VAF_WFlowHandler_ID)
{
if (VAF_WFlowHandler_ID < 1) throw new ArgumentException ("VAF_WFlowHandler_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlowHandler_ID", VAF_WFlowHandler_ID);
}
/** Get Workflow Processor.
@return Workflow Processor Server */
public int GetVAF_WFlowHandler_ID() 
{
Object ii = Get_Value("VAF_WFlowHandler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BinaryData.
@param BinaryData Binary Data */
public void SetBinaryData (Byte[] BinaryData)
{
Set_Value ("BinaryData", BinaryData);
}
/** Get BinaryData.
@return Binary Data */
public Byte[] GetBinaryData() 
{
return (Byte[])Get_Value("BinaryData");
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
/** Set Error.
@param IsError An Error occured in the execution */
public void SetIsError (Boolean IsError)
{
Set_Value ("IsError", IsError);
}
/** Get Error.
@return An Error occured in the execution */
public Boolean IsError() 
{
Object oo = Get_Value("IsError");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Reference.
@param Reference Reference for this record */
public void SetReference (String Reference)
{
if (Reference != null && Reference.Length > 60)
{
log.Warning("Length > 60 - truncated");
Reference = Reference.Substring(0,60);
}
Set_Value ("Reference", Reference);
}
/** Get Reference.
@return Reference for this record */
public String GetReference() 
{
return (String)Get_Value("Reference");
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
}

}
