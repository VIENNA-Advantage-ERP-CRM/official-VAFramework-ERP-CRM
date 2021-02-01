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
/** Generated Model for VACM_ChatTypeUpdateAlert
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VACM_ChatTypeUpdateAlert : PO
{
public X_VACM_ChatTypeUpdateAlert (Context ctx, int VACM_ChatTypeUpdateAlert_ID, Trx trxName) : base (ctx, VACM_ChatTypeUpdateAlert_ID, trxName)
{
/** if (VACM_ChatTypeUpdateAlert_ID == 0)
{
SetVAF_UserContact_ID (0);
SetCM_ChatType_ID (0);
SetIsSelfService (false);
}
 */
}
public X_VACM_ChatTypeUpdateAlert (Ctx ctx, int VACM_ChatTypeUpdateAlert_ID, Trx trxName) : base (ctx, VACM_ChatTypeUpdateAlert_ID, trxName)
{
/** if (VACM_ChatTypeUpdateAlert_ID == 0)
{
SetVAF_UserContact_ID (0);
SetCM_ChatType_ID (0);
SetIsSelfService (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VACM_ChatTypeUpdateAlert (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VACM_ChatTypeUpdateAlert (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VACM_ChatTypeUpdateAlert (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VACM_ChatTypeUpdateAlert()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368551L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051762L;
/** VAF_TableView_ID=875 */
public static int Table_ID;
 // =875;

/** TableName=VACM_ChatTypeUpdateAlert */
public static String Table_Name="VACM_ChatTypeUpdateAlert";

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
StringBuilder sb = new StringBuilder ("X_VACM_ChatTypeUpdateAlert[").Append(Get_ID()).Append("]");
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
/** Set Chat Type.
@param VACM_ChatType_ID Type of discussion / chat */
public void SetCM_ChatType_ID (int VACM_ChatType_ID)
{
if (VACM_ChatType_ID < 1) throw new ArgumentException ("VACM_ChatType_ID is mandatory.");
Set_ValueNoCheck ("VACM_ChatType_ID", VACM_ChatType_ID);
}
/** Get Chat Type.
@return Type of discussion / chat */
public int GetCM_ChatType_ID() 
{
Object ii = Get_Value("VACM_ChatType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetCM_ChatType_ID().ToString());
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
}

}
