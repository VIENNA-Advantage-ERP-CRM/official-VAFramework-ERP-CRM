namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;

using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for VAF_ScreenLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ScreenLog : PO
{
public X_VAF_ScreenLog (Context ctx, int VAF_ScreenLog_ID, Trx trxName) : base (ctx, VAF_ScreenLog_ID, trxName)
{
/** if (VAF_ScreenLog_ID == 0)
{
SetAD_Session_ID (0);
SetVAF_ScreenLog_ID (0);
}
 */
}
public X_VAF_ScreenLog (Ctx ctx, int VAF_ScreenLog_ID, Trx trxName) : base (ctx, VAF_ScreenLog_ID, trxName)
{
/** if (VAF_ScreenLog_ID == 0)
{
SetAD_Session_ID (0);
SetVAF_ScreenLog_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ScreenLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ScreenLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ScreenLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ScreenLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366451L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049662L;
/** VAF_TableView_ID=941 */
public static int Table_ID;
 // =941;

/** TableName=VAF_ScreenLog */
public static String Table_Name="VAF_ScreenLog";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_VAF_ScreenLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Special Form.
@param VAF_Page_ID Special Form */
public void SetVAF_Page_ID (int VAF_Page_ID)
{
if (VAF_Page_ID <= 0) Set_Value ("VAF_Page_ID", null);
else
Set_Value ("VAF_Page_ID", VAF_Page_ID);
}
/** Get Special Form.
@return Special Form */
public int GetVAF_Page_ID() 
{
Object ii = Get_Value("VAF_Page_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID <= 0) Set_Value ("VAF_Role_ID", null);
else
Set_Value ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Session.
@param AD_Session_ID User Session Online or Web */
public void SetAD_Session_ID (int AD_Session_ID)
{
if (AD_Session_ID < 1) throw new ArgumentException ("AD_Session_ID is mandatory.");
Set_ValueNoCheck ("AD_Session_ID", AD_Session_ID);
}
/** Get Session.
@return User Session Online or Web */
public int GetAD_Session_ID() 
{
Object ii = Get_Value("AD_Session_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window Access.
@param VAF_ScreenLog_ID Window Access Log */
public void SetVAF_ScreenLog_ID (int VAF_ScreenLog_ID)
{
if (VAF_ScreenLog_ID < 1) throw new ArgumentException ("VAF_ScreenLog_ID is mandatory.");
Set_ValueNoCheck ("VAF_ScreenLog_ID", VAF_ScreenLog_ID);
}
/** Get Window Access.
@return Window Access Log */
public int GetVAF_ScreenLog_ID() 
{
Object ii = Get_Value("VAF_ScreenLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_ScreenLog_ID().ToString());
}
/** Set Window.
@param VAF_Screen_ID Data entry or display window */
public void SetVAF_Screen_ID (int VAF_Screen_ID)
{
if (VAF_Screen_ID <= 0) Set_Value ("VAF_Screen_ID", null);
else
Set_Value ("VAF_Screen_ID", VAF_Screen_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetVAF_Screen_ID() 
{
Object ii = Get_Value("VAF_Screen_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
