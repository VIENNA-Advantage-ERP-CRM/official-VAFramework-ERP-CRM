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
/** Generated Model for CM_MediaDeploy
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_MediaDeploy : PO
{
public X_CM_MediaDeploy (Context ctx, int CM_MediaDeploy_ID, Trx trxName) : base (ctx, CM_MediaDeploy_ID, trxName)
{
/** if (CM_MediaDeploy_ID == 0)
{
SetCM_MediaDeploy_ID (0);
SetCM_Media_ID (0);
SetCM_Media_Server_ID (0);
SetIsDeployed (false);
}
 */
}
public X_CM_MediaDeploy (Ctx ctx, int CM_MediaDeploy_ID, Trx trxName) : base (ctx, CM_MediaDeploy_ID, trxName)
{
/** if (CM_MediaDeploy_ID == 0)
{
SetCM_MediaDeploy_ID (0);
SetCM_Media_ID (0);
SetCM_Media_Server_ID (0);
SetIsDeployed (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_MediaDeploy (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_MediaDeploy (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_MediaDeploy (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_MediaDeploy()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369037L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052248L;
/** AD_Table_ID=892 */
public static int Table_ID;
 // =892;

/** TableName=CM_MediaDeploy */
public static String Table_Name="CM_MediaDeploy";

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
StringBuilder sb = new StringBuilder ("X_CM_MediaDeploy[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Media Deploy.
@param CM_MediaDeploy_ID Media Deployment Log */
public void SetCM_MediaDeploy_ID (int CM_MediaDeploy_ID)
{
if (CM_MediaDeploy_ID < 1) throw new ArgumentException ("CM_MediaDeploy_ID is mandatory.");
Set_ValueNoCheck ("CM_MediaDeploy_ID", CM_MediaDeploy_ID);
}
/** Get Media Deploy.
@return Media Deployment Log */
public int GetCM_MediaDeploy_ID() 
{
Object ii = Get_Value("CM_MediaDeploy_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Media Item.
@param CM_Media_ID Contains media content like images, flash movies etc. */
public void SetCM_Media_ID (int CM_Media_ID)
{
if (CM_Media_ID < 1) throw new ArgumentException ("CM_Media_ID is mandatory.");
Set_ValueNoCheck ("CM_Media_ID", CM_Media_ID);
}
/** Get Media Item.
@return Contains media content like images, flash movies etc. */
public int GetCM_Media_ID() 
{
Object ii = Get_Value("CM_Media_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Media Server.
@param CM_Media_Server_ID Media Server list to which content should get transfered */
public void SetCM_Media_Server_ID (int CM_Media_Server_ID)
{
if (CM_Media_Server_ID < 1) throw new ArgumentException ("CM_Media_Server_ID is mandatory.");
Set_ValueNoCheck ("CM_Media_Server_ID", CM_Media_Server_ID);
}
/** Get Media Server.
@return Media Server list to which content should get transfered */
public int GetCM_Media_Server_ID() 
{
Object ii = Get_Value("CM_Media_Server_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Deployed.
@param IsDeployed Entity is deployed */
public void SetIsDeployed (Boolean IsDeployed)
{
Set_Value ("IsDeployed", IsDeployed);
}
/** Get Deployed.
@return Entity is deployed */
public Boolean IsDeployed() 
{
Object oo = Get_Value("IsDeployed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Last Synchronized.
@param LastSynchronized Date when last synchronized */
public void SetLastSynchronized (DateTime? LastSynchronized)
{
Set_Value ("LastSynchronized", (DateTime?)LastSynchronized);
}
/** Get Last Synchronized.
@return Date when last synchronized */
public DateTime? GetLastSynchronized() 
{
return (DateTime?)Get_Value("LastSynchronized");
}
}

}
