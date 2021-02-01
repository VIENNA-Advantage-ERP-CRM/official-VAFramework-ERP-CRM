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
/** Generated Model for VAF_ModuleWindow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleWindow : PO
{
public X_VAF_ModuleWindow (Context ctx, int VAF_ModuleWindow_ID, Trx trxName) : base (ctx, VAF_ModuleWindow_ID, trxName)
{
/** if (VAF_ModuleWindow_ID == 0)
{
SetVAF_ModuleInfo_ID (0);
SetVAF_ModuleWindow_ID (0);
SetVAF_Screen_ID (0);
}
 */
}
public X_VAF_ModuleWindow (Ctx ctx, int VAF_ModuleWindow_ID, Trx trxName) : base (ctx, VAF_ModuleWindow_ID, trxName)
{
/** if (VAF_ModuleWindow_ID == 0)
{
SetVAF_ModuleInfo_ID (0);
SetVAF_ModuleWindow_ID (0);
SetVAF_Screen_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleWindow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleWindow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleWindow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleWindow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811883551L;
/** Last Updated Timestamp 6/26/2012 10:26:06 AM */
public static long updatedMS = 1340686566762L;
/** VAF_TableView_ID=1000055 */
public static int Table_ID;
 // =1000055;

/** TableName=VAF_ModuleWindow */
public static String Table_Name="VAF_ModuleWindow";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleWindow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Module.
@param VAF_ModuleInfo_ID Module */
public void SetVAF_ModuleInfo_ID (int VAF_ModuleInfo_ID)
{
if (VAF_ModuleInfo_ID < 1) throw new ArgumentException ("VAF_ModuleInfo_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleInfo_ID", VAF_ModuleInfo_ID);
}
/** Get Module.
@return Module */
public int GetVAF_ModuleInfo_ID() 
{
Object ii = Get_Value("VAF_ModuleInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ModuleWindow_ID.
@param VAF_ModuleWindow_ID VAF_ModuleWindow_ID */
public void SetVAF_ModuleWindow_ID (int VAF_ModuleWindow_ID)
{
if (VAF_ModuleWindow_ID < 1) throw new ArgumentException ("VAF_ModuleWindow_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleWindow_ID", VAF_ModuleWindow_ID);
}
/** Get VAF_ModuleWindow_ID.
@return VAF_ModuleWindow_ID */
public int GetVAF_ModuleWindow_ID() 
{
Object ii = Get_Value("VAF_ModuleWindow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param VAF_Screen_ID Data entry or display window */
public void SetVAF_Screen_ID (int VAF_Screen_ID)
{
if (VAF_Screen_ID < 1) throw new ArgumentException ("VAF_Screen_ID is mandatory.");
Set_ValueNoCheck ("VAF_Screen_ID", VAF_Screen_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetVAF_Screen_ID() 
{
Object ii = Get_Value("VAF_Screen_ID");
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
