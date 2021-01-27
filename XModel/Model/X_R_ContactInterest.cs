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
/** Generated Model for VAR_InterestedUser
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAR_InterestedUser : PO
{
public X_VAR_InterestedUser (Context ctx, int VAR_InterestedUser_ID, Trx trxName) : base (ctx, VAR_InterestedUser_ID, trxName)
{
/** if (VAR_InterestedUser_ID == 0)
{
SetVAF_UserContact_ID (0);	// @VAF_UserContact_ID@
SetR_InterestArea_ID (0);
}
 */
}
public X_VAR_InterestedUser (Ctx ctx, int VAR_InterestedUser_ID, Trx trxName) : base (ctx, VAR_InterestedUser_ID, trxName)
{
/** if (VAR_InterestedUser_ID == 0)
{
SetVAF_UserContact_ID (0);	// @VAF_UserContact_ID@
SetR_InterestArea_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_InterestedUser (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_InterestedUser (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_InterestedUser (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAR_InterestedUser()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382719L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065930L;
/** VAF_TableView_ID=528 */
public static int Table_ID;
 // =528;

/** TableName=VAR_InterestedUser */
public static String Table_Name="VAR_InterestedUser";

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
StringBuilder sb = new StringBuilder ("X_VAR_InterestedUser[").Append(Get_ID()).Append("]");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_UserContact_ID().ToString());
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
@param VAR_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int VAR_InterestArea_ID)
{
if (VAR_InterestArea_ID < 1) throw new ArgumentException ("VAR_InterestArea_ID is mandatory.");
Set_ValueNoCheck ("VAR_InterestArea_ID", VAR_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("VAR_InterestArea_ID");
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
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
{
    if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
    else
        Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID()
{
    Object ii = Get_Value("VAB_BusinessPartner_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Partner Location.
@param VAB_BPart_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetVAB_BPart_Location_ID(int VAB_BPart_Location_ID)
{
    if (VAB_BPart_Location_ID <= 0) Set_Value("VAB_BPart_Location_ID", null);
    else
        Set_Value("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetVAB_BPart_Location_ID()
{
    Object ii = Get_Value("VAB_BPart_Location_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Position.
@param VAB_Position_ID Job Position */
public void SetVAB_Position_ID(int VAB_Position_ID)
{
    if (VAB_Position_ID <= 0) Set_Value("VAB_Position_ID", null);
    else
        Set_Value("VAB_Position_ID", VAB_Position_ID);
}
/** Get Position.
@return Job Position */
public int GetVAB_Position_ID()
{
    Object ii = Get_Value("VAB_Position_ID");
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
@param VAB_Address_ID Location or Address */
public void SetVAB_Address_ID(int VAB_Address_ID)
{
    if (VAB_Address_ID <= 0) Set_Value("VAB_Address_ID", null);
    else
        Set_Value("VAB_Address_ID", VAB_Address_ID);
}
/** Get Address.
@return Location or Address */
public int GetVAB_Address_ID()
{
    Object ii = Get_Value("VAB_Address_ID");
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
