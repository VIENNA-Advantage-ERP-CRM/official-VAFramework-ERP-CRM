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
/** Generated Model for VAF_JobRun_Para
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_JobRun_Para : PO
{
public X_VAF_JobRun_Para (Context ctx, int VAF_JobRun_Para_ID, Trx trxName) : base (ctx, VAF_JobRun_Para_ID, trxName)
{
/** if (VAF_JobRun_Para_ID == 0)
{
SetVAF_Job_Para_ID (0);
SetVAF_JobRun_Plan_ID (0);
}
 */
}
public X_VAF_JobRun_Para (Ctx ctx, int VAF_JobRun_Para_ID, Trx trxName) : base (ctx, VAF_JobRun_Para_ID, trxName)
{
/** if (VAF_JobRun_Para_ID == 0)
{
SetVAF_Job_Para_ID (0);
SetVAF_JobRun_Plan_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JobRun_Para (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JobRun_Para (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_JobRun_Para (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_JobRun_Para()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514364037L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047248L;
/** VAF_TableView_ID=698 */
public static int Table_ID;
 // =698;

/** TableName=VAF_JobRun_Para */
public static String Table_Name="VAF_JobRun_Para";

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
StringBuilder sb = new StringBuilder ("X_VAF_JobRun_Para[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Parameter.
@param VAF_Job_Para_ID Process Parameter */
public void SetVAF_Job_Para_ID (int VAF_Job_Para_ID)
{
if (VAF_Job_Para_ID < 1) throw new ArgumentException ("VAF_Job_Para_ID is mandatory.");
Set_ValueNoCheck ("VAF_Job_Para_ID", VAF_Job_Para_ID);
}
/** Get Process Parameter.
@return Process Parameter */
public int GetVAF_Job_Para_ID() 
{
Object ii = Get_Value("VAF_Job_Para_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_JobRun_Plan_ID().ToString());
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
/** Set Default Parameter.
@param ParameterDefault Default value of the parameter */
public void SetParameterDefault (String ParameterDefault)
{
if (ParameterDefault != null && ParameterDefault.Length > 60)
{
log.Warning("Length > 60 - truncated");
ParameterDefault = ParameterDefault.Substring(0,60);
}
Set_Value ("ParameterDefault", ParameterDefault);
}
/** Get Default Parameter.
@return Default value of the parameter */
public String GetParameterDefault() 
{
return (String)Get_Value("ParameterDefault");
}
}

}
