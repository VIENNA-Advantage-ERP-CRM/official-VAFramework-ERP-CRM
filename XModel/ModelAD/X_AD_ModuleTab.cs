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
/** Generated Model for AD_ModuleTab
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleTab : PO
{
public X_AD_ModuleTab (Context ctx, int AD_ModuleTab_ID, Trx trxName) : base (ctx, AD_ModuleTab_ID, trxName)
{
/** if (AD_ModuleTab_ID == 0)
{
SetAD_ModuleTab_ID (0);
SetAD_ModuleWindow_ID (0);
}
 */
}
public X_AD_ModuleTab (Ctx ctx, int AD_ModuleTab_ID, Trx trxName) : base (ctx, AD_ModuleTab_ID, trxName)
{
/** if (AD_ModuleTab_ID == 0)
{
SetAD_ModuleTab_ID (0);
SetAD_ModuleWindow_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleTab (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleTab (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleTab (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleTab()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811892148L;
/** Last Updated Timestamp 6/26/2012 10:26:15 AM */
public static long updatedMS = 1340686575359L;
/** AD_Table_ID=1000056 */
public static int Table_ID;
 // =1000056;

/** TableName=AD_ModuleTab */
public static String Table_Name="AD_ModuleTab";

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
StringBuilder sb = new StringBuilder ("X_AD_ModuleTab[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_ModuleTab_ID.
@param AD_ModuleTab_ID AD_ModuleTab_ID */
public void SetAD_ModuleTab_ID (int AD_ModuleTab_ID)
{
if (AD_ModuleTab_ID < 1) throw new ArgumentException ("AD_ModuleTab_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleTab_ID", AD_ModuleTab_ID);
}
/** Get AD_ModuleTab_ID.
@return AD_ModuleTab_ID */
public int GetAD_ModuleTab_ID() 
{
Object ii = Get_Value("AD_ModuleTab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_ModuleWindow_ID.
@param AD_ModuleWindow_ID AD_ModuleWindow_ID */
public void SetAD_ModuleWindow_ID (int AD_ModuleWindow_ID)
{
if (AD_ModuleWindow_ID < 1) throw new ArgumentException ("AD_ModuleWindow_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleWindow_ID", AD_ModuleWindow_ID);
}
/** Get AD_ModuleWindow_ID.
@return AD_ModuleWindow_ID */
public int GetAD_ModuleWindow_ID() 
{
Object ii = Get_Value("AD_ModuleWindow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tab.
@param AD_Tab_ID Tab within a Window */
public void SetAD_Tab_ID (int AD_Tab_ID)
{
if (AD_Tab_ID <= 0) Set_Value ("AD_Tab_ID", null);
else
Set_Value ("AD_Tab_ID", AD_Tab_ID);
}
/** Get Tab.
@return Tab within a Window */
public int GetAD_Tab_ID() 
{
Object ii = Get_Value("AD_Tab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 200)
{
log.Warning("Length > 200 - truncated");
Description = Description.Substring(0,200);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
}

}
