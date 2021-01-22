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
/** Generated Model for VAF_ModuleFavourite
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleFavourite : PO
{
public X_VAF_ModuleFavourite (Context ctx, int VAF_ModuleFavourite_ID, Trx trxName) : base (ctx, VAF_ModuleFavourite_ID, trxName)
{
/** if (VAF_ModuleFavourite_ID == 0)
{
SetVAF_ModuleFavourite_ID (0);
SetVAF_ModuleRole_ID (0);
}
 */
}
public X_VAF_ModuleFavourite (Ctx ctx, int VAF_ModuleFavourite_ID, Trx trxName) : base (ctx, VAF_ModuleFavourite_ID, trxName)
{
/** if (VAF_ModuleFavourite_ID == 0)
{
SetVAF_ModuleFavourite_ID (0);
SetVAF_ModuleRole_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleFavourite (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleFavourite (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleFavourite (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleFavourite()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634389901217L;
/** Last Updated Timestamp 11/7/2012 10:33:04 AM */
public static long updatedMS = 1352264584428L;
/** VAF_TableView_ID=1000014 */
public static int Table_ID;
 // =1000014;

/** TableName=VAF_ModuleFavourite */
public static String Table_Name="VAF_ModuleFavourite";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleFavourite[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Menu.
@param VAF_MenuConfig_ID Identifies a Menu */
public void SetVAF_MenuConfig_ID (int VAF_MenuConfig_ID)
{
if (VAF_MenuConfig_ID <= 0) Set_Value ("VAF_MenuConfig_ID", null);
else
Set_Value ("VAF_MenuConfig_ID", VAF_MenuConfig_ID);
}
/** Get Menu.
@return Identifies a Menu */
public int GetVAF_MenuConfig_ID() 
{
Object ii = Get_Value("VAF_MenuConfig_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ModuleFavourite_ID.
@param VAF_ModuleFavourite_ID VAF_ModuleFavourite_ID */
public void SetVAF_ModuleFavourite_ID (int VAF_ModuleFavourite_ID)
{
if (VAF_ModuleFavourite_ID < 1) throw new ArgumentException ("VAF_ModuleFavourite_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleFavourite_ID", VAF_ModuleFavourite_ID);
}
/** Get VAF_ModuleFavourite_ID.
@return VAF_ModuleFavourite_ID */
public int GetVAF_ModuleFavourite_ID() 
{
Object ii = Get_Value("VAF_ModuleFavourite_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
