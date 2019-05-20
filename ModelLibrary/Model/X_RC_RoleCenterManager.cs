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
/** Generated Model for RC_RoleCenterManager
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_RC_RoleCenterManager : PO
{
public X_RC_RoleCenterManager (Context ctx, int RC_RoleCenterManager_ID, Trx trxName) : base (ctx, RC_RoleCenterManager_ID, trxName)
{
/** if (RC_RoleCenterManager_ID == 0)
{
SetRC_RoleCenterManager_ID (0);
}
 */
}
public X_RC_RoleCenterManager (Ctx ctx, int RC_RoleCenterManager_ID, Trx trxName) : base (ctx, RC_RoleCenterManager_ID, trxName)
{
/** if (RC_RoleCenterManager_ID == 0)
{
SetRC_RoleCenterManager_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_RoleCenterManager (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_RoleCenterManager (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_RoleCenterManager (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_RC_RoleCenterManager()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634578386853L;
/** Last Updated Timestamp 11/9/2012 2:54:30 PM */
public static long updatedMS = 1352453070064L;
/** AD_Table_ID=1000232 */
public static int Table_ID;
 // =1000232;

/** TableName=RC_RoleCenterManager */
public static String Table_Name="RC_RoleCenterManager";

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
StringBuilder sb = new StringBuilder ("X_RC_RoleCenterManager[").Append(Get_ID()).Append("]");
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set RoleCenter Manager.
@param RC_RoleCenterManager_ID RoleCenter Manager */
public void SetRC_RoleCenterManager_ID (int RC_RoleCenterManager_ID)
{
if (RC_RoleCenterManager_ID < 1) throw new ArgumentException ("RC_RoleCenterManager_ID is mandatory.");
Set_ValueNoCheck ("RC_RoleCenterManager_ID", RC_RoleCenterManager_ID);
}
/** Get RoleCenter Manager.
@return RoleCenter Manager */
public int GetRC_RoleCenterManager_ID() 
{
Object ii = Get_Value("RC_RoleCenterManager_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
