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
/** Generated Model for VAF_WFlow_Recipient
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlow_Recipient : PO
{
public X_VAF_WFlow_Recipient (Context ctx, int VAF_WFlow_Recipient_ID, Trx trxName) : base (ctx, VAF_WFlow_Recipient_ID, trxName)
{
/** if (VAF_WFlow_Recipient_ID == 0)
{
SetVAF_WFlow_Node_ID (0);
SetVAF_WFlow_Recipient_ID (0);
}
 */
}
public X_VAF_WFlow_Recipient (Ctx ctx, int VAF_WFlow_Recipient_ID, Trx trxName) : base (ctx, VAF_WFlow_Recipient_ID, trxName)
{
/** if (VAF_WFlow_Recipient_ID == 0)
{
SetVAF_WFlow_Node_ID (0);
SetVAF_WFlow_Recipient_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_Recipient (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_Recipient (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlow_Recipient (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlow_Recipient()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27666620426880L;
/** Last Updated Timestamp 11/15/2013 11:28:30 AM */
public static long updatedMS = 1384495110091L;
/** VAF_TableView_ID=1000417 */
public static int Table_ID;
 // =1000417;

/** TableName=VAF_WFlow_Recipient */
public static String Table_Name="VAF_WFlow_Recipient";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlow_Recipient[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Node.
@param VAF_WFlow_Node_ID Workflow Node (activity), step or process */
public void SetVAF_WFlow_Node_ID (int VAF_WFlow_Node_ID)
{
if (VAF_WFlow_Node_ID < 1) throw new ArgumentException ("VAF_WFlow_Node_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_Node_ID", VAF_WFlow_Node_ID);
}
/** Get Node.
@return Workflow Node (activity), step or process */
public int GetVAF_WFlow_Node_ID() 
{
Object ii = Get_Value("VAF_WFlow_Node_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_WFlow_Recipient_ID.
@param VAF_WFlow_Recipient_ID VAF_WFlow_Recipient_ID */
public void SetVAF_WFlow_Recipient_ID (int VAF_WFlow_Recipient_ID)
{
if (VAF_WFlow_Recipient_ID < 1) throw new ArgumentException ("VAF_WFlow_Recipient_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlow_Recipient_ID", VAF_WFlow_Recipient_ID);
}
/** Get VAF_WFlow_Recipient_ID.
@return VAF_WFlow_Recipient_ID */
public int GetVAF_WFlow_Recipient_ID() 
{
Object ii = Get_Value("VAF_WFlow_Recipient_ID");
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
