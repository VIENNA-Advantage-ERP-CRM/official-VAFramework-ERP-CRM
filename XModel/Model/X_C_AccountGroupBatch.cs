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
/** Generated Model for VAB_AccountGroupBatch
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_AccountGroupBatch : PO
{
public X_VAB_AccountGroupBatch (Context ctx, int VAB_AccountGroupBatch_ID, Trx trxName) : base (ctx, VAB_AccountGroupBatch_ID, trxName)
{
/** if (VAB_AccountGroupBatch_ID == 0)
{
SetVAB_AccountGroupBatch_ID (0);
SetName (null);
}
 */
}
public X_VAB_AccountGroupBatch (Ctx ctx, int VAB_AccountGroupBatch_ID, Trx trxName) : base (ctx, VAB_AccountGroupBatch_ID, trxName)
{
/** if (VAB_AccountGroupBatch_ID == 0)
{
SetVAB_AccountGroupBatch_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AccountGroupBatch (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AccountGroupBatch (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AccountGroupBatch (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_AccountGroupBatch()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27640875294150L;
/** Last Updated Timestamp 1/21/2013 12:02:57 PM */
public static long updatedMS = 1358749977361L;
/** VAF_TableView_ID=1000412 */
public static int Table_ID;
 // =1000412;

/** TableName=VAB_AccountGroupBatch */
public static String Table_Name="VAB_AccountGroupBatch";

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
StringBuilder sb = new StringBuilder ("X_VAB_AccountGroupBatch[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAB_AccountGroupBatch_ID.
@param VAB_AccountGroupBatch_ID VAB_AccountGroupBatch_ID */
public void SetVAB_AccountGroupBatch_ID (int VAB_AccountGroupBatch_ID)
{
if (VAB_AccountGroupBatch_ID < 1) throw new ArgumentException ("VAB_AccountGroupBatch_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountGroupBatch_ID", VAB_AccountGroupBatch_ID);
}
/** Get VAB_AccountGroupBatch_ID.
@return VAB_AccountGroupBatch_ID */
public int GetVAB_AccountGroupBatch_ID() 
{
Object ii = Get_Value("VAB_AccountGroupBatch_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 1000)
{
log.Warning("Length > 1000 - truncated");
Description = Description.Substring(0,1000);
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
/** Set For New Tenant.
@param IsForNewTenant For New Tenant */
public void SetIsForNewTenant (Boolean IsForNewTenant)
{
Set_Value ("IsForNewTenant", IsForNewTenant);
}
/** Get For New Tenant.
@return For New Tenant */
public Boolean IsForNewTenant() 
{
Object oo = Get_Value("IsForNewTenant");
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
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
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
if (Value != null && Value.Length > 50)
{
log.Warning("Length > 50 - truncated");
Value = Value.Substring(0,50);
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
