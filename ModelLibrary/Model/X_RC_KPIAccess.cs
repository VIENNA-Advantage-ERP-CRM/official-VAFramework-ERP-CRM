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
/** Generated Model for RC_KPIAccess
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_RC_KPIAccess : PO
{
public X_RC_KPIAccess (Context ctx, int RC_KPIAccess_ID, Trx trxName) : base (ctx, RC_KPIAccess_ID, trxName)
{
/** if (RC_KPIAccess_ID == 0)
{
SetRC_KPIAccess_ID (0);
SetRC_KPI_ID (0);
}
 */
}
public X_RC_KPIAccess (Ctx ctx, int RC_KPIAccess_ID, Trx trxName) : base (ctx, RC_KPIAccess_ID, trxName)
{
/** if (RC_KPIAccess_ID == 0)
{
SetRC_KPIAccess_ID (0);
SetRC_KPI_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_KPIAccess (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_KPIAccess (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_RC_KPIAccess (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_RC_KPIAccess()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634330740979L;
/** Last Updated Timestamp 11/6/2012 6:07:04 PM */
public static long updatedMS = 1352205424190L;
/** AD_Table_ID=1000229 */
public static int Table_ID;
 // =1000229;

/** TableName=RC_KPIAccess */
public static String Table_Name="RC_KPIAccess";

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
StringBuilder sb = new StringBuilder ("X_RC_KPIAccess[").Append(Get_ID()).Append("]");
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
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
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
/** Set KPI Access.
@param RC_KPIAccess_ID KPI Access */
public void SetRC_KPIAccess_ID (int RC_KPIAccess_ID)
{
if (RC_KPIAccess_ID < 1) throw new ArgumentException ("RC_KPIAccess_ID is mandatory.");
Set_ValueNoCheck ("RC_KPIAccess_ID", RC_KPIAccess_ID);
}
/** Get KPI Access.
@return KPI Access */
public int GetRC_KPIAccess_ID() 
{
Object ii = Get_Value("RC_KPIAccess_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set KPI.
@param RC_KPI_ID KPI */
public void SetRC_KPI_ID (int RC_KPI_ID)
{
if (RC_KPI_ID < 1) throw new ArgumentException ("RC_KPI_ID is mandatory.");
Set_ValueNoCheck ("RC_KPI_ID", RC_KPI_ID);
}
/** Get KPI.
@return KPI */
public int GetRC_KPI_ID() 
{
Object ii = Get_Value("RC_KPI_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
