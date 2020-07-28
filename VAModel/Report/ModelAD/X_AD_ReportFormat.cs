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
/** Generated Model for AD_ReportFormat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ReportFormat : PO
{
public X_AD_ReportFormat (Context ctx, int AD_ReportFormat_ID, Trx trxName) : base (ctx, AD_ReportFormat_ID, trxName)
{
/** if (AD_ReportFormat_ID == 0)
{
SetAD_ReportFormat_ID (0);
}
 */
}
public X_AD_ReportFormat (Ctx ctx, int AD_ReportFormat_ID, Trx trxName) : base (ctx, AD_ReportFormat_ID, trxName)
{
/** if (AD_ReportFormat_ID == 0)
{
SetAD_ReportFormat_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReportFormat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReportFormat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReportFormat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ReportFormat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27657404034406L;
/** Last Updated Timestamp 7/31/2013 7:21:57 PM */
public static long updatedMS = 1375278717617L;
/** AD_Table_ID=1000726 */
public static int Table_ID;
 // =1000726;

/** TableName=AD_ReportFormat */
public static String Table_Name="AD_ReportFormat";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_ReportFormat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_ReportFormat_ID.
@param AD_ReportFormat_ID AD_ReportFormat_ID */
public void SetAD_ReportFormat_ID (int AD_ReportFormat_ID)
{
if (AD_ReportFormat_ID < 1) throw new ArgumentException ("AD_ReportFormat_ID is mandatory.");
Set_ValueNoCheck ("AD_ReportFormat_ID", AD_ReportFormat_ID);
}
/** Get AD_ReportFormat_ID.
@return AD_ReportFormat_ID */
public int GetAD_ReportFormat_ID() 
{
Object ii = Get_Value("AD_ReportFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Template.
@param AD_ReportTemplate Report Template */
public void SetAD_ReportTemplate (String AD_ReportTemplate)
{
Set_Value ("AD_ReportTemplate", AD_ReportTemplate);
}
/** Get Report Template.
@return Report Template */
public String GetAD_ReportTemplate() 
{
return (String)Get_Value("AD_ReportTemplate");
}
/** Set Show Grid Format.
@param AD_ShowGridFormat Show Grid Format */
public void SetAD_ShowGridFormat (Boolean AD_ShowGridFormat)
{
Set_Value ("AD_ShowGridFormat", AD_ShowGridFormat);
}
/** Get Show Grid Format.
@return Show Grid Format */
public Boolean IsAD_ShowGridFormat() 
{
Object oo = Get_Value("AD_ShowGridFormat");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 250)
{
log.Warning("Length > 250 - truncated");
Description = Description.Substring(0,250);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
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
/** Set Name.
@param Name Name */
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
@return Name */
public String GetName() 
{
return (String)Get_Value("Name");
}
}

}
