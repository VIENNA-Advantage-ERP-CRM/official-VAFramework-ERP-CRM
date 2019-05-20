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
/** Generated Model for R_ContactInterest
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_ContactInterest : PO
{
public X_R_ContactInterest (Context ctx, int R_ContactInterest_ID, Trx trxName) : base (ctx, R_ContactInterest_ID, trxName)
{
/** if (R_ContactInterest_ID == 0)
{
SetAD_User_ID (0);	// @AD_User_ID@
SetR_InterestArea_ID (0);
}
 */
}
public X_R_ContactInterest (Ctx ctx, int R_ContactInterest_ID, Trx trxName) : base (ctx, R_ContactInterest_ID, trxName)
{
/** if (R_ContactInterest_ID == 0)
{
SetAD_User_ID (0);	// @AD_User_ID@
SetR_InterestArea_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_ContactInterest (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_ContactInterest (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_ContactInterest (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_ContactInterest()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382719L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065930L;
/** AD_Table_ID=528 */
public static int Table_ID;
 // =528;

/** TableName=R_ContactInterest */
public static String Table_Name="R_ContactInterest";

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
StringBuilder sb = new StringBuilder ("X_R_ContactInterest[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
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
/** Set Opt-out Date.
@param OptOutDate Date the contact opted out */
public void SetOptOutDate (DateTime? OptOutDate)
{
Set_ValueNoCheck ("OptOutDate", (DateTime?)OptOutDate);
}
/** Get Opt-out Date.
@return Date the contact opted out */
public DateTime? GetOptOutDate() 
{
return (DateTime?)Get_Value("OptOutDate");
}
/** Set Interest Area.
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID < 1) throw new ArgumentException ("R_InterestArea_ID is mandatory.");
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
/** Set Subscribe Date.
@param SubscribeDate Date the contact actively subscribed */
public void SetSubscribeDate (DateTime? SubscribeDate)
{
Set_ValueNoCheck ("SubscribeDate", (DateTime?)SubscribeDate);
}
/** Get Subscribe Date.
@return Date the contact actively subscribed */
public DateTime? GetSubscribeDate() 
{
return (DateTime?)Get_Value("SubscribeDate");
}

/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID(int C_BPartner_ID)
{
    if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
    else
        Set_Value("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID()
{
    Object ii = Get_Value("C_BPartner_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
{
    if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
    else
        Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetC_BPartner_Location_ID()
{
    Object ii = Get_Value("C_BPartner_Location_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Position.
@param C_Job_ID Job Position */
public void SetC_Job_ID(int C_Job_ID)
{
    if (C_Job_ID <= 0) Set_Value("C_Job_ID", null);
    else
        Set_Value("C_Job_ID", C_Job_ID);
}
/** Get Position.
@return Job Position */
public int GetC_Job_ID()
{
    Object ii = Get_Value("C_Job_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail(String EMail)
{
    if (EMail != null && EMail.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        EMail = EMail.Substring(0, 50);
    }
    Set_Value("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail()
{
    return (String)Get_Value("EMail");
}
/** Set Fax.
@param Fax Facsimile number */
public void SetFax(String Fax)
{
    if (Fax != null && Fax.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        Fax = Fax.Substring(0, 50);
    }
    Set_Value("Fax", Fax);
}
/** Get Fax.
@return Facsimile number */
public String GetFax()
{
    return (String)Get_Value("Fax");
}
/** Set Phone.
@param Phone Identifies a telephone number */
public void SetPhone(String Phone)
{
    if (Phone != null && Phone.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        Phone = Phone.Substring(0, 50);
    }
    Set_Value("Phone", Phone);
}
/** Get Phone.
@return Identifies a telephone number */
public String GetPhone()
{
    return (String)Get_Value("Phone");
}

/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID(int C_Location_ID)
{
    if (C_Location_ID <= 0) Set_Value("C_Location_ID", null);
    else
        Set_Value("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID()
{
    Object ii = Get_Value("C_Location_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed(Boolean Processed)
{
    Set_Value("Processed", Processed);
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
}

}
