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
/** Generated Model for VAF_ModuleMenu
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ModuleMenu : PO
{
public X_VAF_ModuleMenu (Context ctx, int VAF_ModuleMenu_ID, Trx trxName) : base (ctx, VAF_ModuleMenu_ID, trxName)
{
/** if (VAF_ModuleMenu_ID == 0)
{
SetVAF_ModuleMenu_ID (0);
}
 */
}
public X_VAF_ModuleMenu (Ctx ctx, int VAF_ModuleMenu_ID, Trx trxName) : base (ctx, VAF_ModuleMenu_ID, trxName)
{
/** if (VAF_ModuleMenu_ID == 0)
{
SetVAF_ModuleMenu_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleMenu (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleMenu (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ModuleMenu (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ModuleMenu()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27626106801675L;
/** Last Updated Timestamp 8/3/2012 1:41:25 PM */
public static long updatedMS = 1343981484886L;
/** VAF_TableView_ID=1000072 */
public static int Table_ID;
 // =1000072;

/** TableName=VAF_ModuleMenu */
public static String Table_Name="VAF_ModuleMenu";

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
StringBuilder sb = new StringBuilder ("X_VAF_ModuleMenu[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAF_ModuleMenu_ID.
@param VAF_ModuleMenu_ID VAF_ModuleMenu_ID */
public void SetVAF_ModuleMenu_ID (int VAF_ModuleMenu_ID)
{
if (VAF_ModuleMenu_ID < 1) throw new ArgumentException ("VAF_ModuleMenu_ID is mandatory.");
Set_ValueNoCheck ("VAF_ModuleMenu_ID", VAF_ModuleMenu_ID);
}
/** Get VAF_ModuleMenu_ID.
@return VAF_ModuleMenu_ID */
public int GetVAF_ModuleMenu_ID() 
{
Object ii = Get_Value("VAF_ModuleMenu_ID");
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
