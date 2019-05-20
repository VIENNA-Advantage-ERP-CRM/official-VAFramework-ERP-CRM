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
/** Generated Model for C_AcctProcessorLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AcctProcessorLog : PO
{
public X_C_AcctProcessorLog (Context ctx, int C_AcctProcessorLog_ID, Trx trxName) : base (ctx, C_AcctProcessorLog_ID, trxName)
{
/** if (C_AcctProcessorLog_ID == 0)
{
SetC_AcctProcessorLog_ID (0);
SetC_AcctProcessor_ID (0);
SetIsError (false);
}
 */
}
public X_C_AcctProcessorLog (Ctx ctx, int C_AcctProcessorLog_ID, Trx trxName) : base (ctx, C_AcctProcessorLog_ID, trxName)
{
/** if (C_AcctProcessorLog_ID == 0)
{
SetC_AcctProcessorLog_ID (0);
SetC_AcctProcessor_ID (0);
SetIsError (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctProcessorLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctProcessorLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AcctProcessorLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AcctProcessorLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369413L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052624L;
/** AD_Table_ID=694 */
public static int Table_ID;
 // =694;

/** TableName=C_AcctProcessorLog */
public static String Table_Name="C_AcctProcessorLog";

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
StringBuilder sb = new StringBuilder ("X_C_AcctProcessorLog[").Append(Get_ID()).Append("]");
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
/** Set Accounting Processor Log.
@param C_AcctProcessorLog_ID Result of the execution of the Accounting Processor */
public void SetC_AcctProcessorLog_ID (int C_AcctProcessorLog_ID)
{
if (C_AcctProcessorLog_ID < 1) throw new ArgumentException ("C_AcctProcessorLog_ID is mandatory.");
Set_ValueNoCheck ("C_AcctProcessorLog_ID", C_AcctProcessorLog_ID);
}
/** Get Accounting Processor Log.
@return Result of the execution of the Accounting Processor */
public int GetC_AcctProcessorLog_ID() 
{
Object ii = Get_Value("C_AcctProcessorLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounting Processor.
@param C_AcctProcessor_ID Accounting Processor/Server Parameters */
public void SetC_AcctProcessor_ID (int C_AcctProcessor_ID)
{
if (C_AcctProcessor_ID < 1) throw new ArgumentException ("C_AcctProcessor_ID is mandatory.");
Set_ValueNoCheck ("C_AcctProcessor_ID", C_AcctProcessor_ID);
}
/** Get Accounting Processor.
@return Accounting Processor/Server Parameters */
public int GetC_AcctProcessor_ID() 
{
Object ii = Get_Value("C_AcctProcessor_ID");
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
