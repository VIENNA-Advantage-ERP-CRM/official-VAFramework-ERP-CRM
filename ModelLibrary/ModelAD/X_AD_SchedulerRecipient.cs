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
/** Generated Model for AD_SchedulerRecipient
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_SchedulerRecipient : PO
{
public X_AD_SchedulerRecipient (Context ctx, int AD_SchedulerRecipient_ID, Trx trxName) : base (ctx, AD_SchedulerRecipient_ID, trxName)
{
/** if (AD_SchedulerRecipient_ID == 0)
{
SetAD_SchedulerRecipient_ID (0);
SetAD_Scheduler_ID (0);
}
 */
}
public X_AD_SchedulerRecipient (Ctx ctx, int AD_SchedulerRecipient_ID, Trx trxName) : base (ctx, AD_SchedulerRecipient_ID, trxName)
{
/** if (AD_SchedulerRecipient_ID == 0)
{
SetAD_SchedulerRecipient_ID (0);
SetAD_Scheduler_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SchedulerRecipient (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SchedulerRecipient (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_SchedulerRecipient (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_SchedulerRecipient()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363990L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047201L;
/** AD_Table_ID=704 */
public static int Table_ID;
 // =704;

/** TableName=AD_SchedulerRecipient */
public static String Table_Name="AD_SchedulerRecipient";

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
StringBuilder sb = new StringBuilder ("X_AD_SchedulerRecipient[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Scheduler Recipient.
@param AD_SchedulerRecipient_ID Recipient of the Scheduler Notification */
public void SetAD_SchedulerRecipient_ID (int AD_SchedulerRecipient_ID)
{
if (AD_SchedulerRecipient_ID < 1) throw new ArgumentException ("AD_SchedulerRecipient_ID is mandatory.");
Set_ValueNoCheck ("AD_SchedulerRecipient_ID", AD_SchedulerRecipient_ID);
}
/** Get Scheduler Recipient.
@return Recipient of the Scheduler Notification */
public int GetAD_SchedulerRecipient_ID() 
{
Object ii = Get_Value("AD_SchedulerRecipient_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Scheduler.
@param AD_Scheduler_ID Schedule Processes */
public void SetAD_Scheduler_ID (int AD_Scheduler_ID)
{
if (AD_Scheduler_ID < 1) throw new ArgumentException ("AD_Scheduler_ID is mandatory.");
Set_ValueNoCheck ("AD_Scheduler_ID", AD_Scheduler_ID);
}
/** Get Scheduler.
@return Schedule Processes */
public int GetAD_Scheduler_ID() 
{
Object ii = Get_Value("AD_Scheduler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_User_ID().ToString());
}
}

}
