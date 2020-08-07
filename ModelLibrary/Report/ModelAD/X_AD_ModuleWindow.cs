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
/** Generated Model for AD_ModuleWindow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleWindow : PO
{
public X_AD_ModuleWindow (Context ctx, int AD_ModuleWindow_ID, Trx trxName) : base (ctx, AD_ModuleWindow_ID, trxName)
{
/** if (AD_ModuleWindow_ID == 0)
{
SetAD_ModuleInfo_ID (0);
SetAD_ModuleWindow_ID (0);
SetAD_Window_ID (0);
}
 */
}
public X_AD_ModuleWindow (Ctx ctx, int AD_ModuleWindow_ID, Trx trxName) : base (ctx, AD_ModuleWindow_ID, trxName)
{
/** if (AD_ModuleWindow_ID == 0)
{
SetAD_ModuleInfo_ID (0);
SetAD_ModuleWindow_ID (0);
SetAD_Window_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleWindow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleWindow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleWindow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleWindow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811883551L;
/** Last Updated Timestamp 6/26/2012 10:26:06 AM */
public static long updatedMS = 1340686566762L;
/** AD_Table_ID=1000055 */
public static int Table_ID;
 // =1000055;

/** TableName=AD_ModuleWindow */
public static String Table_Name="AD_ModuleWindow";

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
StringBuilder sb = new StringBuilder ("X_AD_ModuleWindow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Module.
@param AD_ModuleInfo_ID Module */
public void SetAD_ModuleInfo_ID (int AD_ModuleInfo_ID)
{
if (AD_ModuleInfo_ID < 1) throw new ArgumentException ("AD_ModuleInfo_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleInfo_ID", AD_ModuleInfo_ID);
}
/** Get Module.
@return Module */
public int GetAD_ModuleInfo_ID() 
{
Object ii = Get_Value("AD_ModuleInfo_ID");
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
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");
Set_ValueNoCheck ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
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
