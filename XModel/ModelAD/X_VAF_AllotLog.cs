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
/** Generated Model for VAF_AllotLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_AllotLog : PO
{
public X_VAF_AllotLog (Context ctx, int VAF_AllotLog_ID, Trx trxName) : base (ctx, VAF_AllotLog_ID, trxName)
{
/** if (VAF_AllotLog_ID == 0)
{
SetVAF_AllotLog_ID (0);
SetVAF_AllotTarget_ID (0);
}
 */
}
public X_VAF_AllotLog (Ctx ctx, int VAF_AllotLog_ID, Trx trxName) : base (ctx, VAF_AllotLog_ID, trxName)
{
/** if (VAF_AllotLog_ID == 0)
{
SetVAF_AllotLog_ID (0);
SetVAF_AllotTarget_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AllotLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AllotLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AllotLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_AllotLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360495L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043706L;
/** VAF_TableView_ID=933 */
public static int Table_ID;
 // =933;

/** TableName=VAF_AllotLog */
public static String Table_Name="VAF_AllotLog";

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
StringBuilder sb = new StringBuilder ("X_VAF_AllotLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Assign Log.
@param VAF_AllotLog_ID Assignment Log */
public void SetVAF_AllotLog_ID (int VAF_AllotLog_ID)
{
if (VAF_AllotLog_ID < 1) throw new ArgumentException ("VAF_AllotLog_ID is mandatory.");
Set_ValueNoCheck ("VAF_AllotLog_ID", VAF_AllotLog_ID);
}
/** Get Assign Log.
@return Assignment Log */
public int GetVAF_AllotLog_ID() 
{
Object ii = Get_Value("VAF_AllotLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Assign Target.
@param VAF_AllotTarget_ID Automatic Assignment Target Column */
public void SetVAF_AllotTarget_ID (int VAF_AllotTarget_ID)
{
if (VAF_AllotTarget_ID < 1) throw new ArgumentException ("VAF_AllotTarget_ID is mandatory.");
Set_ValueNoCheck ("VAF_AllotTarget_ID", VAF_AllotTarget_ID);
}
/** Get Assign Target.
@return Automatic Assignment Target Column */
public int GetVAF_AllotTarget_ID() 
{
Object ii = Get_Value("VAF_AllotTarget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
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
}

}
