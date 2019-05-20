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
/** Generated Model for R_RequestProcessorLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_RequestProcessorLog : PO
{
public X_R_RequestProcessorLog (Context ctx, int R_RequestProcessorLog_ID, Trx trxName) : base (ctx, R_RequestProcessorLog_ID, trxName)
{
/** if (R_RequestProcessorLog_ID == 0)
{
SetIsError (false);
SetR_RequestProcessorLog_ID (0);
SetR_RequestProcessor_ID (0);
}
 */
}
public X_R_RequestProcessorLog (Ctx ctx, int R_RequestProcessorLog_ID, Trx trxName) : base (ctx, R_RequestProcessorLog_ID, trxName)
{
/** if (R_RequestProcessorLog_ID == 0)
{
SetIsError (false);
SetR_RequestProcessorLog_ID (0);
SetR_RequestProcessor_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestProcessorLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestProcessorLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestProcessorLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_RequestProcessorLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383252L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066463L;
/** AD_Table_ID=659 */
public static int Table_ID;
 // =659;

/** TableName=R_RequestProcessorLog */
public static String Table_Name="R_RequestProcessorLog";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_R_RequestProcessorLog[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Request Processor Log.
@param R_RequestProcessorLog_ID Result of the execution of the Request Processor */
public void SetR_RequestProcessorLog_ID (int R_RequestProcessorLog_ID)
{
if (R_RequestProcessorLog_ID < 1) throw new ArgumentException ("R_RequestProcessorLog_ID is mandatory.");
Set_ValueNoCheck ("R_RequestProcessorLog_ID", R_RequestProcessorLog_ID);
}
/** Get Request Processor Log.
@return Result of the execution of the Request Processor */
public int GetR_RequestProcessorLog_ID() 
{
Object ii = Get_Value("R_RequestProcessorLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Processor.
@param R_RequestProcessor_ID Processor for Requests */
public void SetR_RequestProcessor_ID (int R_RequestProcessor_ID)
{
if (R_RequestProcessor_ID < 1) throw new ArgumentException ("R_RequestProcessor_ID is mandatory.");
Set_ValueNoCheck ("R_RequestProcessor_ID", R_RequestProcessor_ID);
}
/** Get Request Processor.
@return Processor for Requests */
public int GetR_RequestProcessor_ID() 
{
Object ii = Get_Value("R_RequestProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
