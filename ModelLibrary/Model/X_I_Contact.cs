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
/** Generated Model for I_Contact
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_Contact : PO
{
public X_I_Contact (Context ctx, int I_Contact_ID, Trx trxName) : base (ctx, I_Contact_ID, trxName)
{
/** if (I_Contact_ID == 0)
{
SetI_Contact_ID (0);
}
 */
}
public X_I_Contact (Ctx ctx, int I_Contact_ID, Trx trxName) : base (ctx, I_Contact_ID, trxName)
{
/** if (I_Contact_ID == 0)
{
SetI_Contact_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Contact (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Contact (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Contact (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_Contact()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376952L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060163L;
/** AD_Table_ID=929 */
public static int Table_ID;
 // =929;

/** TableName=I_Contact */
public static String Table_Name="I_Contact";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_I_Contact[").Append(Get_ID()).Append("]");
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
/** Set Bounced Info.
@param BouncedInfo Information about the cause of bounce */
public void SetBouncedInfo (String BouncedInfo)
{
if (BouncedInfo != null && BouncedInfo.Length > 60)
{
log.Warning("Length > 60 - truncated");
BouncedInfo = BouncedInfo.Substring(0,60);
}
Set_Value ("BouncedInfo", BouncedInfo);
}
/** Get Bounced Info.
@return Information about the cause of bounce */
public String GetBouncedInfo() 
{
return (String)Get_Value("BouncedInfo");
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
/** Set Lead.
@param C_Lead_ID Business Lead */
public void SetC_Lead_ID (int C_Lead_ID)
{
if (C_Lead_ID <= 0) Set_Value ("C_Lead_ID", null);
else
Set_Value ("C_Lead_ID", C_Lead_ID);
}
/** Get Lead.
@return Business Lead */
public int GetC_Lead_ID() 
{
Object ii = Get_Value("C_Lead_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Contact Description.
@param ContactDescription Description of Contact */
public void SetContactDescription (String ContactDescription)
{
if (ContactDescription != null && ContactDescription.Length > 255)
{
log.Warning("Length > 255 - truncated");
ContactDescription = ContactDescription.Substring(0,255);
}
Set_Value ("ContactDescription", ContactDescription);
}
/** Get Contact Description.
@return Description of Contact */
public String GetContactDescription() 
{
return (String)Get_Value("ContactDescription");
}
/** Set Contact Name.
@param ContactName Business Partner Contact Name */
public void SetContactName (String ContactName)
{
if (ContactName != null && ContactName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ContactName = ContactName.Substring(0,60);
}
Set_Value ("ContactName", ContactName);
}
/** Get Contact Name.
@return Business Partner Contact Name */
public String GetContactName() 
{
return (String)Get_Value("ContactName");
}
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail = EMail.Substring(0,60);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetEMail());
}
/** Set Import Contact.
@param I_Contact_ID Import Contact */
public void SetI_Contact_ID (int I_Contact_ID)
{
if (I_Contact_ID < 1) throw new ArgumentException ("I_Contact_ID is mandatory.");
Set_ValueNoCheck ("I_Contact_ID", I_Contact_ID);
}
/** Get Import Contact.
@return Import Contact */
public int GetI_Contact_ID() 
{
Object ii = Get_Value("I_Contact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Import Error Message.
@param I_ErrorMsg Messages generated from import process */
public void SetI_ErrorMsg (String I_ErrorMsg)
{
if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
I_ErrorMsg = I_ErrorMsg.Substring(0,2000);
}
Set_Value ("I_ErrorMsg", I_ErrorMsg);
}
/** Get Import Error Message.
@return Messages generated from import process */
public String GetI_ErrorMsg() 
{
return (String)Get_Value("I_ErrorMsg");
}
/** Set Imported.
@param I_IsImported Has this import been processed */
public void SetI_IsImported (Boolean I_IsImported)
{
Set_Value ("I_IsImported", I_IsImported);
}
/** Get Imported.
@return Has this import been processed */
public Boolean IsI_IsImported() 
{
Object oo = Get_Value("I_IsImported");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Interest Area.
@param InterestAreaName Name of the Interest Area */
public void SetInterestAreaName (String InterestAreaName)
{
if (InterestAreaName != null && InterestAreaName.Length > 40)
{
log.Warning("Length > 40 - truncated");
InterestAreaName = InterestAreaName.Substring(0,40);
}
Set_Value ("InterestAreaName", InterestAreaName);
}
/** Get Interest Area.
@return Name of the Interest Area */
public String GetInterestAreaName() 
{
return (String)Get_Value("InterestAreaName");
}
/** Set Create Business Partner.
@param IsCreateBP If selected, also create Business Partner */
public void SetIsCreateBP (Boolean IsCreateBP)
{
Set_Value ("IsCreateBP", IsCreateBP);
}
/** Get Create Business Partner.
@return If selected, also create Business Partner */
public Boolean IsCreateBP() 
{
Object oo = Get_Value("IsCreateBP");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Create Lead.
@param IsCreateLead If selected, also create Lead */
public void SetIsCreateLead (Boolean IsCreateLead)
{
Set_Value ("IsCreateLead", IsCreateLead);
}
/** Get Create Lead.
@return If selected, also create Lead */
public Boolean IsCreateLead() 
{
Object oo = Get_Value("IsCreateLead");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set EMail Bounced.
@param IsEMailBounced The email delivery bounced */
public void SetIsEMailBounced (Boolean IsEMailBounced)
{
Set_Value ("IsEMailBounced", IsEMailBounced);
}
/** Get EMail Bounced.
@return The email delivery bounced */
public Boolean IsEMailBounced() 
{
Object oo = Get_Value("IsEMailBounced");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Interest Area.
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID <= 0) Set_Value ("R_InterestArea_ID", null);
else
Set_Value ("R_InterestArea_ID", R_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("R_InterestArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
