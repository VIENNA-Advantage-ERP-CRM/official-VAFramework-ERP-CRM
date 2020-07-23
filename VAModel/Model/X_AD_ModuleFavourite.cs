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
/** Generated Model for AD_ModuleFavourite
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleFavourite : PO
{
public X_AD_ModuleFavourite (Context ctx, int AD_ModuleFavourite_ID, Trx trxName) : base (ctx, AD_ModuleFavourite_ID, trxName)
{
/** if (AD_ModuleFavourite_ID == 0)
{
SetAD_ModuleFavourite_ID (0);
SetAD_ModuleRole_ID (0);
}
 */
}
public X_AD_ModuleFavourite (Ctx ctx, int AD_ModuleFavourite_ID, Trx trxName) : base (ctx, AD_ModuleFavourite_ID, trxName)
{
/** if (AD_ModuleFavourite_ID == 0)
{
SetAD_ModuleFavourite_ID (0);
SetAD_ModuleRole_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleFavourite (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleFavourite (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleFavourite (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleFavourite()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634389901217L;
/** Last Updated Timestamp 11/7/2012 10:33:04 AM */
public static long updatedMS = 1352264584428L;
/** AD_Table_ID=1000014 */
public static int Table_ID;
 // =1000014;

/** TableName=AD_ModuleFavourite */
public static String Table_Name="AD_ModuleFavourite";

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
StringBuilder sb = new StringBuilder ("X_AD_ModuleFavourite[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Menu.
@param AD_Menu_ID Identifies a Menu */
public void SetAD_Menu_ID (int AD_Menu_ID)
{
if (AD_Menu_ID <= 0) Set_Value ("AD_Menu_ID", null);
else
Set_Value ("AD_Menu_ID", AD_Menu_ID);
}
/** Get Menu.
@return Identifies a Menu */
public int GetAD_Menu_ID() 
{
Object ii = Get_Value("AD_Menu_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_ModuleFavourite_ID.
@param AD_ModuleFavourite_ID AD_ModuleFavourite_ID */
public void SetAD_ModuleFavourite_ID (int AD_ModuleFavourite_ID)
{
if (AD_ModuleFavourite_ID < 1) throw new ArgumentException ("AD_ModuleFavourite_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleFavourite_ID", AD_ModuleFavourite_ID);
}
/** Get AD_ModuleFavourite_ID.
@return AD_ModuleFavourite_ID */
public int GetAD_ModuleFavourite_ID() 
{
Object ii = Get_Value("AD_ModuleFavourite_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_ModuleRole_ID.
@param AD_ModuleRole_ID AD_ModuleRole_ID */
public void SetAD_ModuleRole_ID (int AD_ModuleRole_ID)
{
if (AD_ModuleRole_ID < 1) throw new ArgumentException ("AD_ModuleRole_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleRole_ID", AD_ModuleRole_ID);
}
/** Get AD_ModuleRole_ID.
@return AD_ModuleRole_ID */
public int GetAD_ModuleRole_ID() 
{
Object ii = Get_Value("AD_ModuleRole_ID");
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
