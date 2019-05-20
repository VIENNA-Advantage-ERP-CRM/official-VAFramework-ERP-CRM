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
/** Generated Model for AD_ModuleMenu
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ModuleMenu : PO
{
public X_AD_ModuleMenu (Context ctx, int AD_ModuleMenu_ID, Trx trxName) : base (ctx, AD_ModuleMenu_ID, trxName)
{
/** if (AD_ModuleMenu_ID == 0)
{
SetAD_ModuleMenu_ID (0);
}
 */
}
public X_AD_ModuleMenu (Ctx ctx, int AD_ModuleMenu_ID, Trx trxName) : base (ctx, AD_ModuleMenu_ID, trxName)
{
/** if (AD_ModuleMenu_ID == 0)
{
SetAD_ModuleMenu_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleMenu (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleMenu (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ModuleMenu (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ModuleMenu()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27626106801675L;
/** Last Updated Timestamp 8/3/2012 1:41:25 PM */
public static long updatedMS = 1343981484886L;
/** AD_Table_ID=1000072 */
public static int Table_ID;
 // =1000072;

/** TableName=AD_ModuleMenu */
public static String Table_Name="AD_ModuleMenu";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_AD_ModuleMenu[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_ModuleMenu_ID.
@param AD_ModuleMenu_ID AD_ModuleMenu_ID */
public void SetAD_ModuleMenu_ID (int AD_ModuleMenu_ID)
{
if (AD_ModuleMenu_ID < 1) throw new ArgumentException ("AD_ModuleMenu_ID is mandatory.");
Set_ValueNoCheck ("AD_ModuleMenu_ID", AD_ModuleMenu_ID);
}
/** Get AD_ModuleMenu_ID.
@return AD_ModuleMenu_ID */
public int GetAD_ModuleMenu_ID() 
{
Object ii = Get_Value("AD_ModuleMenu_ID");
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
/** Set Parent Folder.
@param IsParentFolder Parent Folder */
public void SetIsParentFolder (Boolean IsParentFolder)
{
Set_Value ("IsParentFolder", IsParentFolder);
}
/** Get Parent Folder.
@return Parent Folder */
public Boolean IsParentFolder() 
{
Object oo = Get_Value("IsParentFolder");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}
