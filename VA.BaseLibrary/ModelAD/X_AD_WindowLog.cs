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
/** Generated Model for AD_WindowLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WindowLog : PO
{
public X_AD_WindowLog (Context ctx, int AD_WindowLog_ID, Trx trxName) : base (ctx, AD_WindowLog_ID, trxName)
{
/** if (AD_WindowLog_ID == 0)
{
SetAD_Session_ID (0);
SetAD_WindowLog_ID (0);
}
 */
}
public X_AD_WindowLog (Ctx ctx, int AD_WindowLog_ID, Trx trxName) : base (ctx, AD_WindowLog_ID, trxName)
{
/** if (AD_WindowLog_ID == 0)
{
SetAD_Session_ID (0);
SetAD_WindowLog_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WindowLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WindowLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WindowLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WindowLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366451L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049662L;
/** AD_Table_ID=941 */
public static int Table_ID;
 // =941;

/** TableName=AD_WindowLog */
public static String Table_Name="AD_WindowLog";

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
StringBuilder sb = new StringBuilder ("X_AD_WindowLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Special Form.
@param AD_Form_ID Special Form */
public void SetAD_Form_ID (int AD_Form_ID)
{
if (AD_Form_ID <= 0) Set_Value ("AD_Form_ID", null);
else
Set_Value ("AD_Form_ID", AD_Form_ID);
}
/** Get Special Form.
@return Special Form */
public int GetAD_Form_ID() 
{
Object ii = Get_Value("AD_Form_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
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
@param AD_WindowLog_ID Window Access Log */
public void SetAD_WindowLog_ID (int AD_WindowLog_ID)
{
if (AD_WindowLog_ID < 1) throw new ArgumentException ("AD_WindowLog_ID is mandatory.");
Set_ValueNoCheck ("AD_WindowLog_ID", AD_WindowLog_ID);
}
/** Get Window Access.
@return Window Access Log */
public int GetAD_WindowLog_ID() 
{
Object ii = Get_Value("AD_WindowLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_WindowLog_ID().ToString());
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID <= 0) Set_Value ("AD_Window_ID", null);
else
Set_Value ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
