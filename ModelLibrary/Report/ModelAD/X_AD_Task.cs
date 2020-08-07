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
/** Generated Model for AD_Task
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Task : PO
{
public X_AD_Task (Context ctx, int AD_Task_ID, Trx trxName) : base (ctx, AD_Task_ID, trxName)
{
/** if (AD_Task_ID == 0)
{
SetAD_Task_ID (0);
SetAccessLevel (null);
SetEntityType (null);	// U
SetIsServerProcess (false);	// N
SetName (null);
SetOS_Command (null);
}
 */
}
public X_AD_Task (Ctx ctx, int AD_Task_ID, Trx trxName) : base (ctx, AD_Task_ID, trxName)
{
/** if (AD_Task_ID == 0)
{
SetAD_Task_ID (0);
SetAccessLevel (null);
SetEntityType (null);	// U
SetIsServerProcess (false);	// N
SetName (null);
SetOS_Command (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Task (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Task (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Task (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Task()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364476L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047687L;
/** AD_Table_ID=118 */
public static int Table_ID;
 // =118;

/** TableName=AD_Task */
public static String Table_Name="AD_Task";

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
StringBuilder sb = new StringBuilder ("X_AD_Task[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set OS Task.
@param AD_Task_ID Operation System Task */
public void SetAD_Task_ID (int AD_Task_ID)
{
if (AD_Task_ID < 1) throw new ArgumentException ("AD_Task_ID is mandatory.");
Set_ValueNoCheck ("AD_Task_ID", AD_Task_ID);
}
/** Get OS Task.
@return Operation System Task */
public int GetAD_Task_ID() 
{
Object ii = Get_Value("AD_Task_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AccessLevel AD_Reference_ID=5 */
public static int ACCESSLEVEL_AD_Reference_ID=5;
/** Organization = 1 */
public static String ACCESSLEVEL_Organization = "1";
/** Client only = 2 */
public static String ACCESSLEVEL_ClientOnly = "2";
/** Client+Organization = 3 */
public static String ACCESSLEVEL_ClientPlusOrganization = "3";
/** System only = 4 */
public static String ACCESSLEVEL_SystemOnly = "4";
/** System+Client = 6 */
public static String ACCESSLEVEL_SystemPlusClient = "6";
/** All = 7 */
public static String ACCESSLEVEL_All = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccessLevelValid (String test)
{
return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("6") || test.Equals("7");
}
/** Set Data Access Level.
@param AccessLevel Access Level required */
public void SetAccessLevel (String AccessLevel)
{
if (AccessLevel == null) throw new ArgumentException ("AccessLevel is mandatory");
if (!IsAccessLevelValid(AccessLevel))
throw new ArgumentException ("AccessLevel Invalid value - " + AccessLevel + " - Reference_ID=5 - 1 - 2 - 3 - 4 - 6 - 7");
if (AccessLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccessLevel = AccessLevel.Substring(0,1);
}
Set_Value ("AccessLevel", AccessLevel);
}
/** Get Data Access Level.
@return Access Level required */
public String GetAccessLevel() 
{
return (String)Get_Value("AccessLevel");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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
/** Set Server Process.
@param IsServerProcess Run this Process on Server only */
public void SetIsServerProcess (Boolean IsServerProcess)
{
Set_Value ("IsServerProcess", IsServerProcess);
}
/** Get Server Process.
@return Run this Process on Server only */
public Boolean IsServerProcess() 
{
Object oo = Get_Value("IsServerProcess");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set OS Command.
@param OS_Command Operating System Command */
public void SetOS_Command (String OS_Command)
{
if (OS_Command == null) throw new ArgumentException ("OS_Command is mandatory.");
if (OS_Command.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
OS_Command = OS_Command.Substring(0,2000);
}
Set_Value ("OS_Command", OS_Command);
}
/** Get OS Command.
@return Operating System Command */
public String GetOS_Command() 
{
return (String)Get_Value("OS_Command");
}
}

}
