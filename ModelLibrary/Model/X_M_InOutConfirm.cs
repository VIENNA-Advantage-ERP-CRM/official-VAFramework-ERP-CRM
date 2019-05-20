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
/** Generated Model for M_InOutConfirm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_InOutConfirm : PO
{
public X_M_InOutConfirm (Context ctx, int M_InOutConfirm_ID, Trx trxName) : base (ctx, M_InOutConfirm_ID, trxName)
{
/** if (M_InOutConfirm_ID == 0)
{
SetConfirmType (null);
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);
SetIsCancelled (false);
SetIsInDispute (false);	// N
SetM_InOutConfirm_ID (0);
SetM_InOut_ID (0);
SetProcessed (false);	// N
}
 */
}
public X_M_InOutConfirm (Ctx ctx, int M_InOutConfirm_ID, Trx trxName) : base (ctx, M_InOutConfirm_ID, trxName)
{
/** if (M_InOutConfirm_ID == 0)
{
SetConfirmType (null);
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);
SetIsCancelled (false);
SetIsInDispute (false);	// N
SetM_InOutConfirm_ID (0);
SetM_InOut_ID (0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_InOutConfirm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_InOutConfirm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_InOutConfirm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_InOutConfirm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379600L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062811L;
/** AD_Table_ID=727 */
public static int Table_ID;
 // =727;

/** TableName=M_InOutConfirm */
public static String Table_Name="M_InOutConfirm";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_M_InOutConfirm[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Approval Amount.
@param ApprovalAmt Document Approval Amount */
public void SetApprovalAmt (Decimal? ApprovalAmt)
{
Set_Value ("ApprovalAmt", (Decimal?)ApprovalAmt);
}
/** Get Approval Amount.
@return Document Approval Amount */
public Decimal GetApprovalAmt() 
{
Object bd =Get_Value("ApprovalAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_Value ("C_Invoice_ID", null);
else
Set_Value ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ConfirmType AD_Reference_ID=320 */
public static int CONFIRMTYPE_AD_Reference_ID=320;
/** Drop Ship Confirm = DS */
public static String CONFIRMTYPE_DropShipConfirm = "DS";
/** Pick/QA Confirm = PC */
public static String CONFIRMTYPE_PickQAConfirm = "PC";
/** Ship/Receipt Confirm = SC */
public static String CONFIRMTYPE_ShipReceiptConfirm = "SC";
/** Customer Confirmation = XC */
public static String CONFIRMTYPE_CustomerConfirmation = "XC";
/** Vendor Confirmation = XV */
public static String CONFIRMTYPE_VendorConfirmation = "XV";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfirmTypeValid (String test)
{
return test.Equals("DS") || test.Equals("PC") || test.Equals("SC") || test.Equals("XC") || test.Equals("XV");
}
/** Set Confirmation Type.
@param ConfirmType Type of confirmation */
public void SetConfirmType (String ConfirmType)
{
if (ConfirmType == null) throw new ArgumentException ("ConfirmType is mandatory");
if (!IsConfirmTypeValid(ConfirmType))
throw new ArgumentException ("ConfirmType Invalid value - " + ConfirmType + " - Reference_ID=320 - DS - PC - SC - XC - XV");
if (ConfirmType.Length > 2)
{
log.Warning("Length > 2 - truncated");
ConfirmType = ConfirmType.Substring(0,2);
}
Set_Value ("ConfirmType", ConfirmType);
}
/** Get Confirmation Type.
@return Type of confirmation */
public String GetConfirmType() 
{
return (String)Get_Value("ConfirmType");
}
/** Set Confirmation No.
@param ConfirmationNo Confirmation Number */
public void SetConfirmationNo (String ConfirmationNo)
{
if (ConfirmationNo != null && ConfirmationNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
ConfirmationNo = ConfirmationNo.Substring(0,20);
}
Set_Value ("ConfirmationNo", ConfirmationNo);
}
/** Get Confirmation No.
@return Confirmation Number */
public String GetConfirmationNo() 
{
return (String)Get_Value("ConfirmationNo");
}
/** Set Create Package.
@param CreatePackage Create Package */
public void SetCreatePackage (String CreatePackage)
{
if (CreatePackage != null && CreatePackage.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreatePackage = CreatePackage.Substring(0,1);
}
Set_Value ("CreatePackage", CreatePackage);
}
/** Get Create Package.
@return Create Package */
public String GetCreatePackage() 
{
return (String)Get_Value("CreatePackage");
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

/** DocAction AD_Reference_ID=135 */
public static int DOCACTION_AD_Reference_ID=135;
/** <None> = -- */
public static String DOCACTION_None = "--";
/** Approve = AP */
public static String DOCACTION_Approve = "AP";
/** Close = CL */
public static String DOCACTION_Close = "CL";
/** Complete = CO */
public static String DOCACTION_Complete = "CO";
/** Invalidate = IN */
public static String DOCACTION_Invalidate = "IN";
/** Post = PO */
public static String DOCACTION_Post = "PO";
/** Prepare = PR */
public static String DOCACTION_Prepare = "PR";
/** Reverse - Accrual = RA */
public static String DOCACTION_Reverse_Accrual = "RA";
/** Reverse - Correct = RC */
public static String DOCACTION_Reverse_Correct = "RC";
/** Re-activate = RE */
public static String DOCACTION_Re_Activate = "RE";
/** Reject = RJ */
public static String DOCACTION_Reject = "RJ";
/** Void = VO */
public static String DOCACTION_Void = "VO";
/** Wait Complete = WC */
public static String DOCACTION_WaitComplete = "WC";
/** Unlock = XL */
public static String DOCACTION_Unlock = "XL";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocActionValid (String test)
{
return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
}
/** Set Document Action.
@param DocAction The targeted status of the document */
public void SetDocAction (String DocAction)
{
if (DocAction == null) throw new ArgumentException ("DocAction is mandatory");
if (!IsDocActionValid(DocAction))
throw new ArgumentException ("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
if (DocAction.Length > 2)
{
log.Warning("Length > 2 - truncated");
DocAction = DocAction.Substring(0,2);
}
Set_Value ("DocAction", DocAction);
}
/** Get Document Action.
@return The targeted status of the document */
public String GetDocAction() 
{
return (String)Get_Value("DocAction");
}

/** DocStatus AD_Reference_ID=131 */
public static int DOCSTATUS_AD_Reference_ID=131;
/** Unknown = ?? */
public static String DOCSTATUS_Unknown = "??";
/** Approved = AP */
public static String DOCSTATUS_Approved = "AP";
/** Closed = CL */
public static String DOCSTATUS_Closed = "CL";
/** Completed = CO */
public static String DOCSTATUS_Completed = "CO";
/** Drafted = DR */
public static String DOCSTATUS_Drafted = "DR";
/** Invalid = IN */
public static String DOCSTATUS_Invalid = "IN";
/** In Progress = IP */
public static String DOCSTATUS_InProgress = "IP";
/** Not Approved = NA */
public static String DOCSTATUS_NotApproved = "NA";
/** Reversed = RE */
public static String DOCSTATUS_Reversed = "RE";
/** Voided = VO */
public static String DOCSTATUS_Voided = "VO";
/** Waiting Confirmation = WC */
public static String DOCSTATUS_WaitingConfirmation = "WC";
/** Waiting Payment = WP */
public static String DOCSTATUS_WaitingPayment = "WP";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocStatusValid (String test)
{
return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
}
/** Set Document Status.
@param DocStatus The current status of the document */
public void SetDocStatus (String DocStatus)
{
if (DocStatus == null) throw new ArgumentException ("DocStatus is mandatory");
if (!IsDocStatusValid(DocStatus))
throw new ArgumentException ("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
if (DocStatus.Length > 2)
{
log.Warning("Length > 2 - truncated");
DocStatus = DocStatus.Substring(0,2);
}
Set_Value ("DocStatus", DocStatus);
}
/** Get Document Status.
@return The current status of the document */
public String GetDocStatus() 
{
return (String)Get_Value("DocStatus");
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDocumentNo());
}
/** Set Approved.
@param IsApproved Indicates if this document requires approval */
public void SetIsApproved (Boolean IsApproved)
{
Set_Value ("IsApproved", IsApproved);
}
/** Get Approved.
@return Indicates if this document requires approval */
public Boolean IsApproved() 
{
Object oo = Get_Value("IsApproved");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Cancelled.
@param IsCancelled The transaction was cancelled */
public void SetIsCancelled (Boolean IsCancelled)
{
Set_Value ("IsCancelled", IsCancelled);
}
/** Get Cancelled.
@return The transaction was cancelled */
public Boolean IsCancelled() 
{
Object oo = Get_Value("IsCancelled");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set In Dispute.
@param IsInDispute Document is in dispute */
public void SetIsInDispute (Boolean IsInDispute)
{
Set_Value ("IsInDispute", IsInDispute);
}
/** Get In Dispute.
@return Document is in dispute */
public Boolean IsInDispute() 
{
Object oo = Get_Value("IsInDispute");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Ship/Receipt Confirmation.
@param M_InOutConfirm_ID Material Shipment or Receipt Confirmation */
public void SetM_InOutConfirm_ID (int M_InOutConfirm_ID)
{
if (M_InOutConfirm_ID < 1) throw new ArgumentException ("M_InOutConfirm_ID is mandatory.");
Set_ValueNoCheck ("M_InOutConfirm_ID", M_InOutConfirm_ID);
}
/** Get Ship/Receipt Confirmation.
@return Material Shipment or Receipt Confirmation */
public int GetM_InOutConfirm_ID() 
{
Object ii = Get_Value("M_InOutConfirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt.
@param M_InOut_ID Material Shipment Document */
public void SetM_InOut_ID (int M_InOut_ID)
{
if (M_InOut_ID < 1) throw new ArgumentException ("M_InOut_ID is mandatory.");
Set_ValueNoCheck ("M_InOut_ID", M_InOut_ID);
}
/** Get Shipment/Receipt.
@return Material Shipment Document */
public int GetM_InOut_ID() 
{
Object ii = Get_Value("M_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Phys.Inventory.
@param M_Inventory_ID Parameters for a Physical Inventory */
public void SetM_Inventory_ID (int M_Inventory_ID)
{
if (M_Inventory_ID <= 0) Set_Value ("M_Inventory_ID", null);
else
Set_Value ("M_Inventory_ID", M_Inventory_ID);
}
/** Get Phys.Inventory.
@return Parameters for a Physical Inventory */
public int GetM_Inventory_ID() 
{
Object ii = Get_Value("M_Inventory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
}

}
