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
/** Generated Model for AD_Group_Workflow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Group_Workflow : PO
{
public X_AD_Group_Workflow (Context ctx, int AD_Group_Workflow_ID, Trx trxName) : base (ctx, AD_Group_Workflow_ID, trxName)
{
/** if (AD_Group_Workflow_ID == 0)
{
SetAD_GroupInfo_ID (0);
SetAD_Group_Workflow_ID (0);
}
 */
}
public X_AD_Group_Workflow(Ctx ctx, int AD_Group_Workflow_ID, Trx trxName)
    : base(ctx, AD_Group_Workflow_ID, trxName)
{
/** if (AD_Group_Workflow_ID == 0)
{
SetAD_GroupInfo_ID (0);
SetAD_Group_Workflow_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Group_Workflow(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Group_Workflow(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Group_Workflow(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Group_Workflow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27712845008858L;
/** Last Updated Timestamp 5/4/2015 11:38:12 AM */
public static long updatedMS = 1430719692069L;
/** AD_Table_ID=1000490 */
public static int Table_ID;
 // =1000490;

/** TableName=AD_Group_Workflow */
public static String Table_Name="AD_Group_Workflow";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_AD_Group_Workflow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_GroupInfo_ID.
@param AD_GroupInfo_ID AD_GroupInfo_ID */
public void SetAD_GroupInfo_ID (int AD_GroupInfo_ID)
{
if (AD_GroupInfo_ID < 1) throw new ArgumentException ("AD_GroupInfo_ID is mandatory.");
Set_ValueNoCheck ("AD_GroupInfo_ID", AD_GroupInfo_ID);
}
/** Get AD_GroupInfo_ID.
@return AD_GroupInfo_ID */
public int GetAD_GroupInfo_ID() 
{
Object ii = Get_Value("AD_GroupInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_Group_Workflow_ID.
@param AD_Group_Workflow_ID AD_Group_Workflow_ID */
public void SetAD_Group_Workflow_ID (int AD_Group_Workflow_ID)
{
if (AD_Group_Workflow_ID < 1) throw new ArgumentException ("AD_Group_Workflow_ID is mandatory.");
Set_ValueNoCheck ("AD_Group_Workflow_ID", AD_Group_Workflow_ID);
}
/** Get AD_Group_Workflow_ID.
@return AD_Group_Workflow_ID */
public int GetAD_Group_Workflow_ID() 
{
Object ii = Get_Value("AD_Group_Workflow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow.
@param AD_Workflow_ID Workflow or combination of tasks */
public void SetAD_Workflow_ID (int AD_Workflow_ID)
{
if (AD_Workflow_ID <= 0) Set_Value ("AD_Workflow_ID", null);
else
Set_Value ("AD_Workflow_ID", AD_Workflow_ID);
}
/** Get Workflow.
@return Workflow or combination of tasks */
public int GetAD_Workflow_ID() 
{
Object ii = Get_Value("AD_Workflow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
}

}
