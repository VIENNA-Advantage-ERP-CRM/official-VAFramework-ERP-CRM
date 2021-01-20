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
/** Generated Model for VAF_ModuleTab
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleTab : PO
{
public X_VAF_ModuleTab (Context ctx, int VAF_ModuleTab_ID, Trx trxName) : base (ctx, VAF_ModuleTab_ID, trxName)
{
/** if (VAF_ModuleTab_ID == 0)
{
SetVAF_ModuleTab_ID (0);
SetVAF_ModuleWindow_ID (0);
}
 */
}
public X_VAF_ModuleTab (Ctx ctx, int VAF_ModuleTab_ID, Trx trxName) : base (ctx, VAF_ModuleTab_ID, trxName)
{
/** if (VAF_ModuleTab_ID == 0)
{
SetVAF_ModuleTab_ID (0);
SetVAF_ModuleWindow_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleTab (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleTab (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleTab (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleTab()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622811892148L;
/** Last Updated Timestamp 6/26/2012 10:26:15 AM */
public static long updatedMS = 1340686575359L;
/** VAF_TableView_ID=1000056 */
public static int Table_ID;
 // =1000056;

/** TableName=VAF_ModuleTab */
public static String Table_Name="VAF_ModuleTab";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleTab[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAF_ModuleTab_ID.
@param VAF_ModuleTab_ID VAF_ModuleTab_ID */
public void SetVAF_ModuleTab_ID (int VAF_ModuleTab_ID)
{
if (VAF_ModuleTab_ID < 1) throw new ArgumentException ("VAF_ModuleTab_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleTab_ID", VAF_ModuleTab_ID);
}
/** Get VAF_ModuleTab_ID.
@return VAF_ModuleTab_ID */
public int GetVAF_ModuleTab_ID() 
{
Object ii = Get_Value("VAF_ModuleTab_ID");
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
/** Set Tab.
@param VAF_Tab_ID Tab within a Window */
public void SetVAF_Tab_ID (int VAF_Tab_ID)
{
if (VAF_Tab_ID <= 0) Set_Value ("VAF_Tab_ID", null);
else
Set_Value ("VAF_Tab_ID", VAF_Tab_ID);
}
/** Get Tab.
@return Tab within a Window */
public int GetVAF_Tab_ID() 
{
Object ii = Get_Value("VAF_Tab_ID");
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
