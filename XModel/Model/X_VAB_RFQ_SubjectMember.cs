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
/** Generated Model for VAB_RFQ_SubjectMember
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_RFQ_SubjectMember : PO
{
public X_VAB_RFQ_SubjectMember (Context ctx, int VAB_RFQ_SubjectMember_ID, Trx trxName) : base (ctx, VAB_RFQ_SubjectMember_ID, trxName)
{
/** if (VAB_RFQ_SubjectMember_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAB_BPart_Location_ID (0);
SetVAB_RFQ_SubjectMember_ID (0);
SetVAB_RFQ_Subject_ID (0);
}
 */
}
public X_VAB_RFQ_SubjectMember (Ctx ctx, int VAB_RFQ_SubjectMember_ID, Trx trxName) : base (ctx, VAB_RFQ_SubjectMember_ID, trxName)
{
/** if (VAB_RFQ_SubjectMember_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAB_BPart_Location_ID (0);
SetVAB_RFQ_SubjectMember_ID (0);
SetVAB_RFQ_Subject_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQ_SubjectMember (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQ_SubjectMember (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RFQ_SubjectMember (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_RFQ_SubjectMember()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375055L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058266L;
/** VAF_TableView_ID=670 */
public static int Table_ID;
 // =670;

/** TableName=VAB_RFQ_SubjectMember */
public static String Table_Name="VAB_RFQ_SubjectMember";

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
StringBuilder sb = new StringBuilder ("X_VAB_RFQ_SubjectMember[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID < 1) throw new ArgumentException ("VAB_BusinessPartner_ID is mandatory.");
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
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
public void SetVAB_BPart_Location_ID (int VAB_BPart_Location_ID)
{
if (VAB_BPart_Location_ID < 1) throw new ArgumentException ("VAB_BPart_Location_ID is mandatory.");
Set_Value ("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetVAB_BPart_Location_ID() 
{
Object ii = Get_Value("VAB_BPart_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Subscriber.
@param VAB_RFQ_SubjectMember_ID Request for Quotation Topic Subscriber */
public void SetVAB_RFQ_SubjectMember_ID (int VAB_RFQ_SubjectMember_ID)
{
if (VAB_RFQ_SubjectMember_ID < 1) throw new ArgumentException ("VAB_RFQ_SubjectMember_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQ_SubjectMember_ID", VAB_RFQ_SubjectMember_ID);
}
/** Get RfQ Subscriber.
@return Request for Quotation Topic Subscriber */
public int GetVAB_RFQ_SubjectMember_ID() 
{
Object ii = Get_Value("VAB_RFQ_SubjectMember_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set RfQ Topic.
@param VAB_RFQ_Subject_ID Topic for Request for Quotations */
public void SetVAB_RFQ_Subject_ID (int VAB_RFQ_Subject_ID)
{
if (VAB_RFQ_Subject_ID < 1) throw new ArgumentException ("VAB_RFQ_Subject_ID is mandatory.");
Set_ValueNoCheck ("VAB_RFQ_Subject_ID", VAB_RFQ_Subject_ID);
}
/** Get RfQ Topic.
@return Topic for Request for Quotations */
public int GetVAB_RFQ_Subject_ID() 
{
Object ii = Get_Value("VAB_RFQ_Subject_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_RFQ_Subject_ID().ToString());
}
/** Set Opt-out Date.
@param OptOutDate Date the contact opted out */
public void SetOptOutDate (DateTime? OptOutDate)
{
Set_Value ("OptOutDate", (DateTime?)OptOutDate);
}
/** Get Opt-out Date.
@return Date the contact opted out */
public DateTime? GetOptOutDate() 
{
return (DateTime?)Get_Value("OptOutDate");
}
/** Set Subscribe Date.
@param SubscribeDate Date the contact actively subscribed */
public void SetSubscribeDate (DateTime? SubscribeDate)
{
Set_Value ("SubscribeDate", (DateTime?)SubscribeDate);
}
/** Get Subscribe Date.
@return Date the contact actively subscribed */
public DateTime? GetSubscribeDate() 
{
return (DateTime?)Get_Value("SubscribeDate");
}
}

}
