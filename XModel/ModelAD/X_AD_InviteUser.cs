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
/** Generated Model for AD_InviteUser
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_InviteUser : PO
{
public X_AD_InviteUser (Context ctx, int AD_InviteUser_ID, Trx trxName) : base (ctx, AD_InviteUser_ID, trxName)
{
/** if (AD_InviteUser_ID == 0)
{
SetAD_InviteUser_ID (0);
}
 */
}
public X_AD_InviteUser(Ctx ctx, int AD_InviteUser_ID, Trx trxName)
    : base(ctx, AD_InviteUser_ID, trxName)
{
/** if (AD_InviteUser_ID == 0)
{
SetAD_InviteUser_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_InviteUser(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_InviteUser(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_InviteUser(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_InviteUser()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27713540408526L;
/** Last Updated Timestamp 5/12/2015 12:48:15 PM */
public static long updatedMS = 1431415091737L;
/** AD_Table_ID=1000500 */
public static int Table_ID;
 // =1000500;

/** TableName=AD_InviteUser */
public static String Table_Name="AD_InviteUser";

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
StringBuilder sb = new StringBuilder ("X_AD_InviteUser[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_InviteUser_ID.
@param AD_InviteUser_ID AD_InviteUser_ID */
public void SetAD_InviteUser_ID (int AD_InviteUser_ID)
{
if (AD_InviteUser_ID < 1) throw new ArgumentException ("AD_InviteUser_ID is mandatory.");
Set_ValueNoCheck ("AD_InviteUser_ID", AD_InviteUser_ID);
}
/** Get AD_InviteUser_ID.
@return AD_InviteUser_ID */
public int GetAD_InviteUser_ID() 
{
Object ii = Get_Value("AD_InviteUser_ID");
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
