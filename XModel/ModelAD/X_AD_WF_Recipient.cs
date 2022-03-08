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
/** Generated Model for AD_WF_Recipient
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_WF_Recipient : PO
{
public X_AD_WF_Recipient (Context ctx, int AD_WF_Recipient_ID, Trx trxName) : base (ctx, AD_WF_Recipient_ID, trxName)
{
/** if (AD_WF_Recipient_ID == 0)
{
SetAD_WF_Node_ID (0);
SetAD_WF_Recipient_ID (0);
}
 */
}
public X_AD_WF_Recipient (Ctx ctx, int AD_WF_Recipient_ID, Trx trxName) : base (ctx, AD_WF_Recipient_ID, trxName)
{
/** if (AD_WF_Recipient_ID == 0)
{
SetAD_WF_Node_ID (0);
SetAD_WF_Recipient_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Recipient (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Recipient (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_WF_Recipient (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_WF_Recipient()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27666620426880L;
/** Last Updated Timestamp 11/15/2013 11:28:30 AM */
public static long updatedMS = 1384495110091L;
/** AD_Table_ID=1000417 */
public static int Table_ID;
 // =1000417;

/** TableName=AD_WF_Recipient */
public static String Table_Name="AD_WF_Recipient";

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
StringBuilder sb = new StringBuilder ("X_AD_WF_Recipient[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Node.
@param AD_WF_Node_ID Workflow Node (activity), step or process */
public void SetAD_WF_Node_ID (int AD_WF_Node_ID)
{
if (AD_WF_Node_ID < 1) throw new ArgumentException ("AD_WF_Node_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Node_ID", AD_WF_Node_ID);
}
/** Get Node.
@return Workflow Node (activity), step or process */
public int GetAD_WF_Node_ID() 
{
Object ii = Get_Value("AD_WF_Node_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_WF_Recipient_ID.
@param AD_WF_Recipient_ID AD_WF_Recipient_ID */
public void SetAD_WF_Recipient_ID (int AD_WF_Recipient_ID)
{
if (AD_WF_Recipient_ID < 1) throw new ArgumentException ("AD_WF_Recipient_ID is mandatory.");
Set_ValueNoCheck ("AD_WF_Recipient_ID", AD_WF_Recipient_ID);
}
/** Get AD_WF_Recipient_ID.
@return AD_WF_Recipient_ID */
public int GetAD_WF_Recipient_ID() 
{
Object ii = Get_Value("AD_WF_Recipient_ID");
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
}

}
