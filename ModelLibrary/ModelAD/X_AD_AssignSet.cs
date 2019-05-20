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
/** Generated Model for AD_AssignSet
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AssignSet : PO
{
public X_AD_AssignSet (Context ctx, int AD_AssignSet_ID, Trx trxName) : base (ctx, AD_AssignSet_ID, trxName)
{
/** if (AD_AssignSet_ID == 0)
{
SetAD_AssignSet_ID (0);
SetAD_Table_ID (0);
SetAutoAssignRule (null);
SetName (null);
}
 */
}
public X_AD_AssignSet (Ctx ctx, int AD_AssignSet_ID, Trx trxName) : base (ctx, AD_AssignSet_ID, trxName)
{
/** if (AD_AssignSet_ID == 0)
{
SetAD_AssignSet_ID (0);
SetAD_Table_ID (0);
SetAutoAssignRule (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AssignSet (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AssignSet (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AssignSet (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AssignSet()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360589L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043800L;
/** AD_Table_ID=930 */
public static int Table_ID;
 // =930;

/** TableName=AD_AssignSet */
public static String Table_Name="AD_AssignSet";

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
StringBuilder sb = new StringBuilder ("X_AD_AssignSet[").Append(Get_ID()).Append("]");
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

/** AutoAssignRule AD_Reference_ID=424 */
public static int AUTOASSIGNRULE_AD_Reference_ID=424;
/** Create and Update = B */
public static String AUTOASSIGNRULE_CreateAndUpdate = "B";
/** Create only = C */
public static String AUTOASSIGNRULE_CreateOnly = "C";
/** Create and Update if not Processed = P */
public static String AUTOASSIGNRULE_CreateAndUpdateIfNotProcessed = "P";
/** Update if not Processed = Q */
public static String AUTOASSIGNRULE_UpdateIfNotProcessed = "Q";
/** Update only = U */
public static String AUTOASSIGNRULE_UpdateOnly = "U";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAutoAssignRuleValid (String test)
{
return test.Equals("B") || test.Equals("C") || test.Equals("P") || test.Equals("Q") || test.Equals("U");
}
/** Set Auto Assign Rule.
@param AutoAssignRule Timing of automatic assignment */
public void SetAutoAssignRule (String AutoAssignRule)
{
if (AutoAssignRule == null) throw new ArgumentException ("AutoAssignRule is mandatory");
if (!IsAutoAssignRuleValid(AutoAssignRule))
throw new ArgumentException ("AutoAssignRule Invalid value - " + AutoAssignRule + " - Reference_ID=424 - B - C - P - Q - U");
if (AutoAssignRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
AutoAssignRule = AutoAssignRule.Substring(0,1);
}
Set_Value ("AutoAssignRule", AutoAssignRule);
}
/** Get Auto Assign Rule.
@return Timing of automatic assignment */
public String GetAutoAssignRule() 
{
return (String)Get_Value("AutoAssignRule");
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
}

}
