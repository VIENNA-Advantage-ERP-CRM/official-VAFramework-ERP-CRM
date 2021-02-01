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
/** Generated Model for VAF_JInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_JInstance : PO
{
public X_VAF_JInstance (Context ctx, int VAF_JInstance_ID, Trx trxName) : base (ctx, VAF_JInstance_ID, trxName)
{
/** if (VAF_JInstance_ID == 0)
{
SetVAF_JInstance_ID (0);
SetVAF_Job_ID (0);
SetIsProcessing (false);
SetRecord_ID (0);
}
 */
}
public X_VAF_JInstance (Ctx ctx, int VAF_JInstance_ID, Trx trxName) : base (ctx, VAF_JInstance_ID, trxName)
{
/** if (VAF_JInstance_ID == 0)
{
SetVAF_JInstance_ID (0);
SetVAF_Job_ID (0);
SetIsProcessing (false);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_JInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362408L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045619L;
/** VAF_TableView_ID=282 */
public static int Table_ID;
 // =282;

/** TableName=VAF_JInstance */
public static String Table_Name="VAF_JInstance";

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
StringBuilder sb = new StringBuilder ("X_VAF_JInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param VAF_JInstance_ID Instance of the process */
public void SetVAF_JInstance_ID (int VAF_JInstance_ID)
{
if (VAF_JInstance_ID < 1) throw new ArgumentException ("VAF_JInstance_ID is mandatory.");
Set_ValueNoCheck ("VAF_JInstance_ID", VAF_JInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetVAF_JInstance_ID() 
{
Object ii = Get_Value("VAF_JInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_JInstance_ID().ToString());
}
/** Set Process.
@param VAF_Job_ID Process or Report */
public void SetVAF_Job_ID (int VAF_Job_ID)
{
if (VAF_Job_ID < 1) throw new ArgumentException ("VAF_Job_ID is mandatory.");
Set_Value ("VAF_Job_ID", VAF_Job_ID);
}
/** Get Process.
@return Process or Report */
public int GetVAF_Job_ID() 
{
Object ii = Get_Value("VAF_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID <= 0) Set_ValueNoCheck ("VAF_Role_ID", null);
else
Set_ValueNoCheck ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Session.
@param VAF_Session_ID User Session Online or Web */
public void SetVAF_Session_ID (int VAF_Session_ID)
{
if (VAF_Session_ID <= 0) Set_ValueNoCheck ("VAF_Session_ID", null);
else
Set_ValueNoCheck ("VAF_Session_ID", VAF_Session_ID);
}
/** Get Session.
@return User Session Online or Web */
public int GetVAF_Session_ID() 
{
Object ii = Get_Value("VAF_Session_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
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
/** Set Error Msg.
@param ErrorMsg Error Msg */
public void SetErrorMsg (String ErrorMsg)
{
if (ErrorMsg != null && ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ErrorMsg = ErrorMsg.Substring(0,2000);
}
Set_Value ("ErrorMsg", ErrorMsg);
}
/** Get Error Msg.
@return Error Msg */
public String GetErrorMsg() 
{
return (String)Get_Value("ErrorMsg");
}
/** Set Processing.
@param IsProcessing Processing */
public void SetIsProcessing (Boolean IsProcessing)
{
Set_Value ("IsProcessing", IsProcessing);
}
/** Get Processing.
@return Processing */
public Boolean IsProcessing() 
{
Object oo = Get_Value("IsProcessing");
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
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Result.
@param Result Result of the action taken */
public void SetResult (int Result)
{
Set_Value ("Result", Result);
}
/** Get Result.
@return Result of the action taken */
public int GetResult() 
{
Object ii = Get_Value("Result");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
