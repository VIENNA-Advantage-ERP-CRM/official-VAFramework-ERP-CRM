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
/** Generated Model for VAR_Req_UpdatesAlert
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAR_Req_UpdatesAlert : PO
{
public X_VAR_Req_UpdatesAlert (Context ctx, int VAR_Req_UpdatesAlert_ID, Trx trxName) : base (ctx, VAR_Req_UpdatesAlert_ID, trxName)
{
/** if (VAR_Req_UpdatesAlert_ID == 0)
{
SetVAF_UserContact_ID (0);
SetIsSelfService (false);
SetVAR_Request_ID (0);
}
 */
}
public X_VAR_Req_UpdatesAlert (Ctx ctx, int VAR_Req_UpdatesAlert_ID, Trx trxName) : base (ctx, VAR_Req_UpdatesAlert_ID, trxName)
{
/** if (VAR_Req_UpdatesAlert_ID == 0)
{
SetVAF_UserContact_ID (0);
SetIsSelfService (false);
SetVAR_Request_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_UpdatesAlert (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_UpdatesAlert (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Req_UpdatesAlert (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAR_Req_UpdatesAlert()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383409L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066620L;
/** VAF_TableView_ID=783 */
public static int Table_ID;
 // =783;

/** TableName=VAR_Req_UpdatesAlert */
public static String Table_Name="VAR_Req_UpdatesAlert";

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
StringBuilder sb = new StringBuilder ("X_VAR_Req_UpdatesAlert[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Request.
@param VAR_Request_ID Request from a Business Partner or Prospect */
public void SetVAR_Request_ID (int VAR_Request_ID)
{
if (VAR_Request_ID < 1) throw new ArgumentException ("VAR_Request_ID is mandatory.");
Set_ValueNoCheck ("VAR_Request_ID", VAR_Request_ID);
}
/** Get Request.
@return Request from a Business Partner or Prospect */
public int GetVAR_Request_ID() 
{
Object ii = Get_Value("VAR_Request_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAR_Request_ID().ToString());
}
}

}
