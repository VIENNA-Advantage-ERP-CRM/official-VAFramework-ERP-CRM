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
/** Generated Model for AD_UserHomePageSetting
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserHomePageSetting : PO
{
public X_AD_UserHomePageSetting (Context ctx, int AD_UserHomePageSetting_ID, Trx trxName) : base (ctx, AD_UserHomePageSetting_ID, trxName)
{
/** if (AD_UserHomePageSetting_ID == 0)
{
SetAD_UserHomePageSetting_ID (0);
}
 */
}
public X_AD_UserHomePageSetting (Ctx ctx, int AD_UserHomePageSetting_ID, Trx trxName) : base (ctx, AD_UserHomePageSetting_ID, trxName)
{
/** if (AD_UserHomePageSetting_ID == 0)
{
SetAD_UserHomePageSetting_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserHomePageSetting (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserHomePageSetting (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserHomePageSetting (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserHomePageSetting()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27693491989134L;
/** Last Updated Timestamp 2014-09-22 11:47:52 AM */
public static long updatedMS = 1411366672345L;
/** AD_Table_ID=1000544 */
public static int Table_ID;
 // =1000544;

/** TableName=AD_UserHomePageSetting */
public static String Table_Name="AD_UserHomePageSetting";

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
StringBuilder sb = new StringBuilder ("X_AD_UserHomePageSetting[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_UserHomePageSetting_ID.
@param AD_UserHomePageSetting_ID AD_UserHomePageSetting_ID */
public void SetAD_UserHomePageSetting_ID (int AD_UserHomePageSetting_ID)
{
if (AD_UserHomePageSetting_ID < 1) throw new ArgumentException ("AD_UserHomePageSetting_ID is mandatory.");
Set_ValueNoCheck ("AD_UserHomePageSetting_ID", AD_UserHomePageSetting_ID);
}
/** Get AD_UserHomePageSetting_ID.
@return AD_UserHomePageSetting_ID */
public int GetAD_UserHomePageSetting_ID() 
{
Object ii = Get_Value("AD_UserHomePageSetting_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
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
