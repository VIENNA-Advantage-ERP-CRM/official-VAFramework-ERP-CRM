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
/** Generated Model for WSP_Note
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_WSP_Note : PO
{
public X_WSP_Note (Context ctx, int WSP_Note_ID, Trx trxName) : base (ctx, WSP_Note_ID, trxName)
{
/** if (WSP_Note_ID == 0)
{
SetWSP_Note_ID (0);
}
 */
}
public X_WSP_Note (Ctx ctx, int WSP_Note_ID, Trx trxName) : base (ctx, WSP_Note_ID, trxName)
{
/** if (WSP_Note_ID == 0)
{
SetWSP_Note_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_WSP_Note (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_WSP_Note (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_WSP_Note (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_WSP_Note()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27688050402024L;
/** Last Updated Timestamp 7/21/2014 12:14:45 PM */
public static long updatedMS = 1405925085235L;
/** AD_Table_ID=1000394 */
public static int Table_ID;
 // =1000394;

/** TableName=WSP_Note */
public static String Table_Name="WSP_Note";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_WSP_Note[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Message.
@param WSP_Message Message */
public void SetWSP_Message (String WSP_Message)
{
if (WSP_Message != null && WSP_Message.Length > 3500)
{
log.Warning("Length > 3500 - truncated");
WSP_Message = WSP_Message.Substring(0,3500);
}
Set_Value ("WSP_Message", WSP_Message);
}
/** Get Message.
@return Message */
public String GetWSP_Message() 
{
return (String)Get_Value("WSP_Message");
}
/** Set Name.
@param WSP_Name Name */
public void SetWSP_Name (String WSP_Name)
{
if (WSP_Name != null && WSP_Name.Length > 500)
{
log.Warning("Length > 500 - truncated");
WSP_Name = WSP_Name.Substring(0,500);
}
Set_Value ("WSP_Name", WSP_Name);
}
/** Get Name.
@return Name */
public String GetWSP_Name() 
{
return (String)Get_Value("WSP_Name");
}
/** Set WSP_Note_ID.
@param WSP_Note_ID WSP_Note_ID */
public void SetWSP_Note_ID (int WSP_Note_ID)
{
if (WSP_Note_ID < 1) throw new ArgumentException ("WSP_Note_ID is mandatory.");
Set_ValueNoCheck ("WSP_Note_ID", WSP_Note_ID);
}
/** Get WSP_Note_ID.
@return WSP_Note_ID */
public int GetWSP_Note_ID() 
{
Object ii = Get_Value("WSP_Note_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
