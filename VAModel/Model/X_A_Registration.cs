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
/** Generated Model for A_Registration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_A_Registration : PO
{
public X_A_Registration (Context ctx, int A_Registration_ID, Trx trxName) : base (ctx, A_Registration_ID, trxName)
{
/** if (A_Registration_ID == 0)
{
SetA_Registration_ID (0);
SetIsAllowPublish (false);
SetIsInProduction (false);
SetIsRegistered (false);
SetName (null);
}
 */
}
public X_A_Registration (Ctx ctx, int A_Registration_ID, Trx trxName) : base (ctx, A_Registration_ID, trxName)
{
/** if (A_Registration_ID == 0)
{
SetA_Registration_ID (0);
SetIsAllowPublish (false);
SetIsInProduction (false);
SetIsRegistered (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Registration (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Registration (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_A_Registration (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_A_Registration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367109L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050320L;
/** AD_Table_ID=651 */
public static int Table_ID;
 // =651;

/** TableName=A_Registration */
public static String Table_Name="A_Registration";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_A_Registration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Registration.
@param A_Registration_ID User Asset Registration */
public void SetA_Registration_ID (int A_Registration_ID)
{
if (A_Registration_ID < 1) throw new ArgumentException ("A_Registration_ID is mandatory.");
Set_ValueNoCheck ("A_Registration_ID", A_Registration_ID);
}
/** Get Registration.
@return User Asset Registration */
public int GetA_Registration_ID() 
{
Object ii = Get_Value("A_Registration_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set In Service Date.
@param AssetServiceDate Date when Asset was put into service */
public void SetAssetServiceDate (DateTime? AssetServiceDate)
{
Set_ValueNoCheck ("AssetServiceDate", (DateTime?)AssetServiceDate);
}
/** Get In Service Date.
@return Date when Asset was put into service */
public DateTime? GetAssetServiceDate() 
{
return (DateTime?)Get_Value("AssetServiceDate");
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Allowed to be Published.
@param IsAllowPublish You allow to publish the information, not just statistical summary info */
public void SetIsAllowPublish (Boolean IsAllowPublish)
{
Set_Value ("IsAllowPublish", IsAllowPublish);
}
/** Get Allowed to be Published.
@return You allow to publish the information, not just statistical summary info */
public Boolean IsAllowPublish() 
{
Object oo = Get_Value("IsAllowPublish");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set In Production.
@param IsInProduction The system is in production */
public void SetIsInProduction (Boolean IsInProduction)
{
Set_Value ("IsInProduction", IsInProduction);
}
/** Get In Production.
@return The system is in production */
public Boolean IsInProduction() 
{
Object oo = Get_Value("IsInProduction");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Registered.
@param IsRegistered The application is registered. */
public void SetIsRegistered (Boolean IsRegistered)
{
Set_Value ("IsRegistered", IsRegistered);
}
/** Get Registered.
@return The application is registered. */
public Boolean IsRegistered() 
{
Object oo = Get_Value("IsRegistered");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Note.
@param Note Optional additional user defined information */
public void SetNote (String Note)
{
if (Note != null && Note.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Note = Note.Substring(0,2000);
}
Set_Value ("Note", Note);
}
/** Get Note.
@return Optional additional user defined information */
public String GetNote() 
{
return (String)Get_Value("Note");
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}
