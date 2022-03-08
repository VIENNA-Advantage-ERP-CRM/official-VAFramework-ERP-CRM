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
/** Generated Model for C_BP_EDI
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BP_EDI : PO
{
public X_C_BP_EDI (Context ctx, int C_BP_EDI_ID, Trx trxName) : base (ctx, C_BP_EDI_ID, trxName)
{
/** if (C_BP_EDI_ID == 0)
{
SetAD_Sequence_ID (0);
SetC_BP_EDI_ID (0);
SetC_BPartner_ID (0);
SetCustomerNo (null);
SetEDIType (null);
SetEMail_Error_To (null);
SetEMail_Info_To (null);
SetIsAudited (false);
SetIsInfoSent (false);
SetM_Warehouse_ID (0);
SetName (null);
SetReceiveInquiryReply (false);
SetReceiveOrderReply (false);
SetSendInquiry (false);
SetSendOrder (false);
}
 */
}
public X_C_BP_EDI (Ctx ctx, int C_BP_EDI_ID, Trx trxName) : base (ctx, C_BP_EDI_ID, trxName)
{
/** if (C_BP_EDI_ID == 0)
{
SetAD_Sequence_ID (0);
SetC_BP_EDI_ID (0);
SetC_BPartner_ID (0);
SetCustomerNo (null);
SetEDIType (null);
SetEMail_Error_To (null);
SetEMail_Info_To (null);
SetIsAudited (false);
SetIsInfoSent (false);
SetM_Warehouse_ID (0);
SetName (null);
SetReceiveInquiryReply (false);
SetReceiveOrderReply (false);
SetSendInquiry (false);
SetSendOrder (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_EDI (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_EDI (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_EDI (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BP_EDI()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369977L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053188L;
/** AD_Table_ID=366 */
public static int Table_ID;
 // =366;

/** TableName=C_BP_EDI */
public static String Table_Name="C_BP_EDI";

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
StringBuilder sb = new StringBuilder ("X_C_BP_EDI[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Sequence_ID AD_Reference_ID=128 */
public static int AD_SEQUENCE_ID_AD_Reference_ID=128;
/** Set Sequence.
@param AD_Sequence_ID Document Sequence */
public void SetAD_Sequence_ID (int AD_Sequence_ID)
{
if (AD_Sequence_ID < 1) throw new ArgumentException ("AD_Sequence_ID is mandatory.");
Set_Value ("AD_Sequence_ID", AD_Sequence_ID);
}
/** Get Sequence.
@return Document Sequence */
public int GetAD_Sequence_ID() 
{
Object ii = Get_Value("AD_Sequence_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set EDI Definition.
@param C_BP_EDI_ID Electronic Data Interchange */
public void SetC_BP_EDI_ID (int C_BP_EDI_ID)
{
if (C_BP_EDI_ID < 1) throw new ArgumentException ("C_BP_EDI_ID is mandatory.");
Set_ValueNoCheck ("C_BP_EDI_ID", C_BP_EDI_ID);
}
/** Get EDI Definition.
@return Electronic Data Interchange */
public int GetC_BP_EDI_ID() 
{
Object ii = Get_Value("C_BP_EDI_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
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
/** Set Customer No.
@param CustomerNo EDI Identification Number */
public void SetCustomerNo (String CustomerNo)
{
if (CustomerNo == null) throw new ArgumentException ("CustomerNo is mandatory.");
if (CustomerNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
CustomerNo = CustomerNo.Substring(0,20);
}
Set_Value ("CustomerNo", CustomerNo);
}
/** Get Customer No.
@return EDI Identification Number */
public String GetCustomerNo() 
{
return (String)Get_Value("CustomerNo");
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

/** EDIType AD_Reference_ID=201 */
public static int EDITYPE_AD_Reference_ID=201;
/** EDIFACT = E */
public static String EDITYPE_EDIFACT = "E";
/** Email EDI = M */
public static String EDITYPE_EmailEDI = "M";
/** ASC X12 = X */
public static String EDITYPE_ASCX12 = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsEDITypeValid (String test)
{
return test.Equals("E") || test.Equals("M") || test.Equals("X");
}
/** Set EDI Type.
@param EDIType EDI Type */
public void SetEDIType (String EDIType)
{
if (EDIType == null) throw new ArgumentException ("EDIType is mandatory");
if (!IsEDITypeValid(EDIType))
throw new ArgumentException ("EDIType Invalid value - " + EDIType + " - Reference_ID=201 - E - M - X");
if (EDIType.Length > 1)
{
log.Warning("Length > 1 - truncated");
EDIType = EDIType.Substring(0,1);
}
Set_Value ("EDIType", EDIType);
}
/** Get EDI Type.
@return EDI Type */
public String GetEDIType() 
{
return (String)Get_Value("EDIType");
}
/** Set Error EMail.
@param EMail_Error_To Email address to send error messages to */
public void SetEMail_Error_To (String EMail_Error_To)
{
if (EMail_Error_To == null) throw new ArgumentException ("EMail_Error_To is mandatory.");
if (EMail_Error_To.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail_Error_To = EMail_Error_To.Substring(0,60);
}
Set_Value ("EMail_Error_To", EMail_Error_To);
}
/** Get Error EMail.
@return Email address to send error messages to */
public String GetEMail_Error_To() 
{
return (String)Get_Value("EMail_Error_To");
}
/** Set From EMail.
@param EMail_From Full EMail address used to send requests - e.g. edi@organization.com */
public void SetEMail_From (String EMail_From)
{
if (EMail_From != null && EMail_From.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail_From = EMail_From.Substring(0,60);
}
Set_Value ("EMail_From", EMail_From);
}
/** Get From EMail.
@return Full EMail address used to send requests - e.g. edi@organization.com */
public String GetEMail_From() 
{
return (String)Get_Value("EMail_From");
}
/** Set From EMail Password.
@param EMail_From_Pwd Password of the sending EMail address */
public void SetEMail_From_Pwd (String EMail_From_Pwd)
{
if (EMail_From_Pwd != null && EMail_From_Pwd.Length > 20)
{
log.Warning("Length > 20 - truncated");
EMail_From_Pwd = EMail_From_Pwd.Substring(0,20);
}
Set_Value ("EMail_From_Pwd", EMail_From_Pwd);
}
/** Get From EMail Password.
@return Password of the sending EMail address */
public String GetEMail_From_Pwd() 
{
return (String)Get_Value("EMail_From_Pwd");
}
/** Set From EMail User ID.
@param EMail_From_Uid User ID of the sending EMail address (on default SMTP Host) - e.g. edi */
public void SetEMail_From_Uid (String EMail_From_Uid)
{
if (EMail_From_Uid != null && EMail_From_Uid.Length > 20)
{
log.Warning("Length > 20 - truncated");
EMail_From_Uid = EMail_From_Uid.Substring(0,20);
}
Set_Value ("EMail_From_Uid", EMail_From_Uid);
}
/** Get From EMail User ID.
@return User ID of the sending EMail address (on default SMTP Host) - e.g. edi */
public String GetEMail_From_Uid() 
{
return (String)Get_Value("EMail_From_Uid");
}
/** Set Info EMail.
@param EMail_Info_To EMail address to send informational messages and copies */
public void SetEMail_Info_To (String EMail_Info_To)
{
if (EMail_Info_To == null) throw new ArgumentException ("EMail_Info_To is mandatory.");
if (EMail_Info_To.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail_Info_To = EMail_Info_To.Substring(0,60);
}
Set_Value ("EMail_Info_To", EMail_Info_To);
}
/** Get Info EMail.
@return EMail address to send informational messages and copies */
public String GetEMail_Info_To() 
{
return (String)Get_Value("EMail_Info_To");
}
/** Set To EMail.
@param EMail_To EMail address to send requests to - e.g. edi@manufacturer.com */
public void SetEMail_To (String EMail_To)
{
if (EMail_To != null && EMail_To.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail_To = EMail_To.Substring(0,60);
}
Set_Value ("EMail_To", EMail_To);
}
/** Get To EMail.
@return EMail address to send requests to - e.g. edi@manufacturer.com */
public String GetEMail_To() 
{
return (String)Get_Value("EMail_To");
}
/** Set Activate Audit.
@param IsAudited Activate Audit Trail of what numbers are generated */
public void SetIsAudited (Boolean IsAudited)
{
Set_Value ("IsAudited", IsAudited);
}
/** Get Activate Audit.
@return Activate Audit Trail of what numbers are generated */
public Boolean IsAudited() 
{
Object oo = Get_Value("IsAudited");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Send Info.
@param IsInfoSent Send informational messages and copies */
public void SetIsInfoSent (Boolean IsInfoSent)
{
Set_Value ("IsInfoSent", IsInfoSent);
}
/** Get Send Info.
@return Send informational messages and copies */
public Boolean IsInfoSent() 
{
Object oo = Get_Value("IsInfoSent");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_Value ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
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
/** Set Received Inquiry Reply.
@param ReceiveInquiryReply Received Inquiry Reply */
public void SetReceiveInquiryReply (Boolean ReceiveInquiryReply)
{
Set_Value ("ReceiveInquiryReply", ReceiveInquiryReply);
}
/** Get Received Inquiry Reply.
@return Received Inquiry Reply */
public Boolean IsReceiveInquiryReply() 
{
Object oo = Get_Value("ReceiveInquiryReply");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Receive Order Reply.
@param ReceiveOrderReply Receive Order Reply */
public void SetReceiveOrderReply (Boolean ReceiveOrderReply)
{
Set_Value ("ReceiveOrderReply", ReceiveOrderReply);
}
/** Get Receive Order Reply.
@return Receive Order Reply */
public Boolean IsReceiveOrderReply() 
{
Object oo = Get_Value("ReceiveOrderReply");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Send Inquiry.
@param SendInquiry Quantity Availability Inquiry */
public void SetSendInquiry (Boolean SendInquiry)
{
Set_Value ("SendInquiry", SendInquiry);
}
/** Get Send Inquiry.
@return Quantity Availability Inquiry */
public Boolean IsSendInquiry() 
{
Object oo = Get_Value("SendInquiry");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Send Order.
@param SendOrder Send Order */
public void SetSendOrder (Boolean SendOrder)
{
Set_Value ("SendOrder", SendOrder);
}
/** Get Send Order.
@return Send Order */
public Boolean IsSendOrder() 
{
Object oo = Get_Value("SendOrder");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
