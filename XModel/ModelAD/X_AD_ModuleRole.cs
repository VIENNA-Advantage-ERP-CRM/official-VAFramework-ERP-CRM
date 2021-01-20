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
/** Generated Model for VAF_ModuleRole
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleRole : PO
{
public X_VAF_ModuleRole (Context ctx, int VAF_ModuleRole_ID, Trx trxName) : base (ctx, VAF_ModuleRole_ID, trxName)
{
/** if (VAF_ModuleRole_ID == 0)
{
SetVAF_ModuleRole_ID (0);
SetVAF_Module_ID (0);
}
 */
}
public X_VAF_ModuleRole (Ctx ctx, int VAF_ModuleRole_ID, Trx trxName) : base (ctx, VAF_ModuleRole_ID, trxName)
{
/** if (VAF_ModuleRole_ID == 0)
{
SetVAF_ModuleRole_ID (0);
SetVAF_Module_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleRole (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleRole (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleRole (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleRole()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634389886506L;
/** Last Updated Timestamp 11/7/2012 10:32:49 AM */
public static long updatedMS = 1352264569717L;
/** VAF_TableView_ID=1000013 */
public static int Table_ID;
 // =1000013;

/** TableName=VAF_ModuleRole */
public static String Table_Name="VAF_ModuleRole";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleRole[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAF_ModuleRole_ID.
@param VAF_ModuleRole_ID VAF_ModuleRole_ID */
public void SetVAF_ModuleRole_ID (int VAF_ModuleRole_ID)
{
if (VAF_ModuleRole_ID < 1) throw new ArgumentException ("VAF_ModuleRole_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleRole_ID", VAF_ModuleRole_ID);
}
/** Get VAF_ModuleRole_ID.
@return VAF_ModuleRole_ID */
public int GetVAF_ModuleRole_ID() 
{
Object ii = Get_Value("VAF_ModuleRole_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_Module_ID.
@param VAF_Module_ID VAF_Module_ID */
public void SetVAF_Module_ID (int VAF_Module_ID)
{
if (VAF_Module_ID < 1) throw new ArgumentException ("VAF_Module_ID is mandatory.");
Set_ValueNoCheck ("VAF_Module_ID", VAF_Module_ID);
}
/** Get VAF_Module_ID.
@return VAF_Module_ID */
public int GetVAF_Module_ID() 
{
Object ii = Get_Value("VAF_Module_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_ValueNoCheck ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
}

}
