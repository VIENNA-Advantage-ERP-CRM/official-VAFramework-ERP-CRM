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
/** Generated Model for AppointmentCategory
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AppointmentCategory : PO
{
public X_AppointmentCategory (Context ctx, int AppointmentCategory_ID, Trx trxName) : base (ctx, AppointmentCategory_ID, trxName)
{
/** if (AppointmentCategory_ID == 0)
{
SetAppointmentCategory_ID (0);
}
 */
}
public X_AppointmentCategory (Ctx ctx, int AppointmentCategory_ID, Trx trxName) : base (ctx, AppointmentCategory_ID, trxName)
{
/** if (AppointmentCategory_ID == 0)
{
SetAppointmentCategory_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AppointmentCategory (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AppointmentCategory (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AppointmentCategory (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AppointmentCategory()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634395324576L;
/** Last Updated Timestamp 11/7/2012 12:03:27 PM */
public static long updatedMS = 1352270007787L;
/** AD_Table_ID=1000220 */
public static int Table_ID;
 // =1000220;

/** TableName=AppointmentCategory */
public static String Table_Name="AppointmentCategory";

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
StringBuilder sb = new StringBuilder ("X_AppointmentCategory[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AppointmentCategory_ID.
@param AppointmentCategory_ID AppointmentCategory_ID */
public void SetAppointmentCategory_ID (int AppointmentCategory_ID)
{
if (AppointmentCategory_ID < 1) throw new ArgumentException ("AppointmentCategory_ID is mandatory.");
Set_ValueNoCheck ("AppointmentCategory_ID", AppointmentCategory_ID);
}
/** Get AppointmentCategory_ID.
@return AppointmentCategory_ID */
public int GetAppointmentCategory_ID() 
{
Object ii = Get_Value("AppointmentCategory_ID");
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
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 200)
{
log.Warning("Length > 200 - truncated");
Name = Name.Substring(0,200);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
