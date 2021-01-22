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
/** Generated Model for VAF_JobRun_Recipient
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_JobRun_Recipient : PO
{
public X_VAF_JobRun_Recipient (Context ctx, int VAF_JobRun_Recipient_ID, Trx trxName) : base (ctx, VAF_JobRun_Recipient_ID, trxName)
{
/** if (VAF_JobRun_Recipient_ID == 0)
{
SetVAF_JobRun_Recipient_ID (0);
SetVAF_JobRun_Plan_ID (0);
}
 */
}
public X_VAF_JobRun_Recipient (Ctx ctx, int VAF_JobRun_Recipient_ID, Trx trxName) : base (ctx, VAF_JobRun_Recipient_ID, trxName)
{
/** if (VAF_JobRun_Recipient_ID == 0)
{
SetVAF_JobRun_Recipient_ID (0);
SetVAF_JobRun_Plan_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JobRun_Recipient (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JobRun_Recipient (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JobRun_Recipient (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_JobRun_Recipient()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363990L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047201L;
/** VAF_TableView_ID=704 */
public static int Table_ID;
 // =704;

/** TableName=VAF_JobRun_Recipient */
public static String Table_Name="VAF_JobRun_Recipient";

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
StringBuilder sb = new StringBuilder ("X_VAF_JobRun_Recipient[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID <= 0) Set_Value ("VAF_Role_ID", null);
else
Set_Value ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Scheduler Recipient.
@param VAF_JobRun_Recipient_ID Recipient of the Scheduler Notification */
public void SetVAF_JobRun_Recipient_ID (int VAF_JobRun_Recipient_ID)
{
if (VAF_JobRun_Recipient_ID < 1) throw new ArgumentException ("VAF_JobRun_Recipient_ID is mandatory.");
Set_ValueNoCheck ("VAF_JobRun_Recipient_ID", VAF_JobRun_Recipient_ID);
}
/** Get Scheduler Recipient.
@return Recipient of the Scheduler Notification */
public int GetVAF_JobRun_Recipient_ID() 
{
Object ii = Get_Value("VAF_JobRun_Recipient_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Scheduler.
@param VAF_JobRun_Plan_ID Schedule Processes */
public void SetVAF_JobRun_Plan_ID (int VAF_JobRun_Plan_ID)
{
if (VAF_JobRun_Plan_ID < 1) throw new ArgumentException ("VAF_JobRun_Plan_ID is mandatory.");
Set_ValueNoCheck ("VAF_JobRun_Plan_ID", VAF_JobRun_Plan_ID);
}
/** Get Scheduler.
@return Schedule Processes */
public int GetVAF_JobRun_Plan_ID() 
{
Object ii = Get_Value("VAF_JobRun_Plan_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_UserContact_ID().ToString());
}
}

}
