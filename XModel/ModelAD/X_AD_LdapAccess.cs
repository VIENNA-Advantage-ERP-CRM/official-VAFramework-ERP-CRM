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
/** Generated Model for AD_LdapAccess
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_LdapAccess : PO
{
public X_AD_LdapAccess (Context ctx, int AD_LdapAccess_ID, Trx trxName) : base (ctx, AD_LdapAccess_ID, trxName)
{
/** if (AD_LdapAccess_ID == 0)
{
SetAD_LdapAccess_ID (0);
SetAD_LdapProcessor_ID (0);
SetIsError (false);
}
 */
}
public X_AD_LdapAccess (Ctx ctx, int AD_LdapAccess_ID, Trx trxName) : base (ctx, AD_LdapAccess_ID, trxName)
{
/** if (AD_LdapAccess_ID == 0)
{
SetAD_LdapAccess_ID (0);
SetAD_LdapProcessor_ID (0);
SetIsError (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_LdapAccess (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_LdapAccess (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_LdapAccess (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_LdapAccess()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362063L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045274L;
/** AD_Table_ID=904 */
public static int Table_ID;
 // =904;

/** TableName=AD_LdapAccess */
public static String Table_Name="AD_LdapAccess";

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
StringBuilder sb = new StringBuilder ("X_AD_LdapAccess[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Ldap Access.
@param AD_LdapAccess_ID Ldap Access Log */
public void SetAD_LdapAccess_ID (int AD_LdapAccess_ID)
{
if (AD_LdapAccess_ID < 1) throw new ArgumentException ("AD_LdapAccess_ID is mandatory.");
Set_ValueNoCheck ("AD_LdapAccess_ID", AD_LdapAccess_ID);
}
/** Get Ldap Access.
@return Ldap Access Log */
public int GetAD_LdapAccess_ID() 
{
Object ii = Get_Value("AD_LdapAccess_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Ldap Processor.
@param AD_LdapProcessor_ID LDAP Server to authenticate and authorize external systems based on Vienna */
public void SetAD_LdapProcessor_ID (int AD_LdapProcessor_ID)
{
if (AD_LdapProcessor_ID < 1) throw new ArgumentException ("AD_LdapProcessor_ID is mandatory.");
Set_ValueNoCheck ("AD_LdapProcessor_ID", AD_LdapProcessor_ID);
}
/** Get Ldap Processor.
@return LDAP Server to authenticate and authorize external systems based on Vienna */
public int GetAD_LdapProcessor_ID() 
{
Object ii = Get_Value("AD_LdapProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_ValueNoCheck ("AD_User_ID", null);
else
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_User_ID().ToString());
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID <= 0) Set_Value ("A_Asset_ID", null);
else
Set_Value ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Error.
@param IsError An Error occured in the execution */
public void SetIsError (Boolean IsError)
{
Set_ValueNoCheck ("IsError", IsError);
}
/** Get Error.
@return An Error occured in the execution */
public Boolean IsError() 
{
Object oo = Get_Value("IsError");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Interest Area.
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID <= 0) Set_ValueNoCheck ("R_InterestArea_ID", null);
else
Set_ValueNoCheck ("R_InterestArea_ID", R_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("R_InterestArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Remote Addr.
@param Remote_Addr Remote Address */
public void SetRemote_Addr (String Remote_Addr)
{
if (Remote_Addr != null && Remote_Addr.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Addr = Remote_Addr.Substring(0,60);
}
Set_Value ("Remote_Addr", Remote_Addr);
}
/** Get Remote Addr.
@return Remote Address */
public String GetRemote_Addr() 
{
return (String)Get_Value("Remote_Addr");
}
/** Set Remote Host.
@param Remote_Host Remote host Info */
public void SetRemote_Host (String Remote_Host)
{
if (Remote_Host != null && Remote_Host.Length > 120)
{
log.Warning("Length > 120 - truncated");
Remote_Host = Remote_Host.Substring(0,120);
}
Set_Value ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
}
/** Set Summary.
@param Summary Textual summary of this request */
public void SetSummary (String Summary)
{
if (Summary != null && Summary.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Summary = Summary.Substring(0,2000);
}
Set_ValueNoCheck ("Summary", Summary);
}
/** Get Summary.
@return Textual summary of this request */
public String GetSummary() 
{
return (String)Get_Value("Summary");
}
}

}
