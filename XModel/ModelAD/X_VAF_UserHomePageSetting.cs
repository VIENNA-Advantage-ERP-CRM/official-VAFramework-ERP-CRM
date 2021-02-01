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
/** Generated Model for VAF_UserHomePageSetting
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_UserHomePageSetting : PO
{
public X_VAF_UserHomePageSetting (Context ctx, int VAF_UserHomePageSetting_ID, Trx trxName) : base (ctx, VAF_UserHomePageSetting_ID, trxName)
{
/** if (VAF_UserHomePageSetting_ID == 0)
{
SetVAF_UserHomePageSetting_ID (0);
}
 */
}
public X_VAF_UserHomePageSetting (Ctx ctx, int VAF_UserHomePageSetting_ID, Trx trxName) : base (ctx, VAF_UserHomePageSetting_ID, trxName)
{
/** if (VAF_UserHomePageSetting_ID == 0)
{
SetVAF_UserHomePageSetting_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserHomePageSetting (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserHomePageSetting (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserHomePageSetting (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_UserHomePageSetting()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27693491989134L;
/** Last Updated Timestamp 2014-09-22 11:47:52 AM */
public static long updatedMS = 1411366672345L;
/** VAF_TableView_ID=1000544 */
public static int Table_ID;
 // =1000544;

/** TableName=VAF_UserHomePageSetting */
public static String Table_Name="VAF_UserHomePageSetting";

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
StringBuilder sb = new StringBuilder ("X_VAF_UserHomePageSetting[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);
else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_UserHomePageSetting_ID.
@param VAF_UserHomePageSetting_ID VAF_UserHomePageSetting_ID */
public void SetVAF_UserHomePageSetting_ID (int VAF_UserHomePageSetting_ID)
{
if (VAF_UserHomePageSetting_ID < 1) throw new ArgumentException ("VAF_UserHomePageSetting_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserHomePageSetting_ID", VAF_UserHomePageSetting_ID);
}
/** Get VAF_UserHomePageSetting_ID.
@return VAF_UserHomePageSetting_ID */
public int GetVAF_UserHomePageSetting_ID() 
{
Object ii = Get_Value("VAF_UserHomePageSetting_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
Set_Value ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
