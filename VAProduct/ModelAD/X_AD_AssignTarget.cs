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
/** Generated Model for AD_AssignTarget
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AssignTarget : PO
{
public X_AD_AssignTarget (Context ctx, int AD_AssignTarget_ID, Trx trxName) : base (ctx, AD_AssignTarget_ID, trxName)
{
/** if (AD_AssignTarget_ID == 0)
{
SetAD_AssignSet_ID (0);
SetAD_AssignTarget_ID (0);
SetAD_TargetColumn_ID (0);
SetAssignRule (null);	// A
}
 */
}
public X_AD_AssignTarget (Ctx ctx, int AD_AssignTarget_ID, Trx trxName) : base (ctx, AD_AssignTarget_ID, trxName)
{
/** if (AD_AssignTarget_ID == 0)
{
SetAD_AssignSet_ID (0);
SetAD_AssignTarget_ID (0);
SetAD_TargetColumn_ID (0);
SetAssignRule (null);	// A
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AssignTarget (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AssignTarget (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AssignTarget (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AssignTarget()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360605L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043816L;
/** AD_Table_ID=931 */
public static int Table_ID;
 // =931;

/** TableName=AD_AssignTarget */
public static String Table_Name="AD_AssignTarget";

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
StringBuilder sb = new StringBuilder ("X_AD_AssignTarget[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Auto Assignment.
@param AD_AssignSet_ID Automatic Assignment of values */
public void SetAD_AssignSet_ID (int AD_AssignSet_ID)
{
if (AD_AssignSet_ID < 1) throw new ArgumentException ("AD_AssignSet_ID is mandatory.");
Set_ValueNoCheck ("AD_AssignSet_ID", AD_AssignSet_ID);
}
/** Get Auto Assignment.
@return Automatic Assignment of values */
public int GetAD_AssignSet_ID() 
{
Object ii = Get_Value("AD_AssignSet_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Assign Target.
@param AD_AssignTarget_ID Automatic Assignment Target Column */
public void SetAD_AssignTarget_ID (int AD_AssignTarget_ID)
{
if (AD_AssignTarget_ID < 1) throw new ArgumentException ("AD_AssignTarget_ID is mandatory.");
Set_ValueNoCheck ("AD_AssignTarget_ID", AD_AssignTarget_ID);
}
/** Get Assign Target.
@return Automatic Assignment Target Column */
public int GetAD_AssignTarget_ID() 
{
Object ii = Get_Value("AD_AssignTarget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_TargetColumn_ID AD_Reference_ID=414 */
public static int AD_TARGETCOLUMN_ID_AD_Reference_ID=414;
/** Set Target Column.
@param AD_TargetColumn_ID Target column to be set */
public void SetAD_TargetColumn_ID (int AD_TargetColumn_ID)
{
if (AD_TargetColumn_ID < 1) throw new ArgumentException ("AD_TargetColumn_ID is mandatory.");
Set_Value ("AD_TargetColumn_ID", AD_TargetColumn_ID);
}
/** Get Target Column.
@return Target column to be set */
public int GetAD_TargetColumn_ID() 
{
Object ii = Get_Value("AD_TargetColumn_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_TargetColumn_ID().ToString());
}

/** AssignRule AD_Reference_ID=425 */
public static int ASSIGNRULE_AD_Reference_ID=425;
/** Always = A */
public static String ASSIGNRULE_Always = "A";
/** Only if NULL = N */
public static String ASSIGNRULE_OnlyIfNULL = "N";
/** Only if NOT NULL = X */
public static String ASSIGNRULE_OnlyIfNOTNULL = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAssignRuleValid (String test)
{
return test.Equals("A") || test.Equals("N") || test.Equals("X");
}
/** Set Assignment Rule.
@param AssignRule Assignment Rule */
public void SetAssignRule (String AssignRule)
{
if (AssignRule == null) throw new ArgumentException ("AssignRule is mandatory");
if (!IsAssignRuleValid(AssignRule))
throw new ArgumentException ("AssignRule Invalid value - " + AssignRule + " - Reference_ID=425 - A - N - X");
if (AssignRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
AssignRule = AssignRule.Substring(0,1);
}
Set_Value ("AssignRule", AssignRule);
}
/** Get Assignment Rule.
@return Assignment Rule */
public String GetAssignRule() 
{
return (String)Get_Value("AssignRule");
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
/** Set Value.
@param ValueString Value as String */
public void SetValueString (String ValueString)
{
if (ValueString != null && ValueString.Length > 40)
{
log.Warning("Length > 40 - truncated");
ValueString = ValueString.Substring(0,40);
}
Set_Value ("ValueString", ValueString);
}
/** Get Value.
@return Value as String */
public String GetValueString() 
{
return (String)Get_Value("ValueString");
}
}

}
