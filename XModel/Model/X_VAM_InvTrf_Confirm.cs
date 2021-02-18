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
/** Generated Model for VAM_InvTrf_Confirm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_InvTrf_Confirm : PO
{
public X_VAM_InvTrf_Confirm (Context ctx, int VAM_InvTrf_Confirm_ID, Trx trxName) : base (ctx, VAM_InvTrf_Confirm_ID, trxName)
{
/** if (VAM_InvTrf_Confirm_ID == 0)
{
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);	// N
SetVAM_InvTrf_Confirm_ID (0);
SetVAM_InventoryTransfer_ID (0);
SetProcessed (false);	// N
}
 */
}
public X_VAM_InvTrf_Confirm (Ctx ctx, int VAM_InvTrf_Confirm_ID, Trx trxName) : base (ctx, VAM_InvTrf_Confirm_ID, trxName)
{
/** if (VAM_InvTrf_Confirm_ID == 0)
{
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);	// N
SetVAM_InvTrf_Confirm_ID (0);
SetVAM_InventoryTransfer_ID (0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_Confirm (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_Confirm (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_InvTrf_Confirm (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_InvTrf_Confirm()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380086L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063297L;
/** VAF_TableView_ID=738 */
public static int Table_ID;
 // =738;

/** TableName=VAM_InvTrf_Confirm */
public static String Table_Name="VAM_InvTrf_Confirm";

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
StringBuilder sb = new StringBuilder ("X_VAM_InvTrf_Confirm[").Append(Get_ID()).Append("]");
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

/** DocAction VAF_Control_Ref_ID=135 */
public static int DOCACTION_VAF_Control_Ref_ID=135;
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

/** DocStatus VAF_Control_Ref_ID=131 */
public static int DOCSTATUS_VAF_Control_Ref_ID=131;
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
/** Set Phys.Inventory.
@param VAM_Inventory_ID Parameters for a Physical Inventory */
public void SetVAM_Inventory_ID (int VAM_Inventory_ID)
{
if (VAM_Inventory_ID <= 0) Set_Value ("VAM_Inventory_ID", null);
else
Set_Value ("VAM_Inventory_ID", VAM_Inventory_ID);
}
/** Get Phys.Inventory.
@return Parameters for a Physical Inventory */
public int GetVAM_Inventory_ID() 
{
Object ii = Get_Value("VAM_Inventory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Move Confirm.
@param VAM_InvTrf_Confirm_ID Inventory Move Confirmation */
public void SetVAM_InvTrf_Confirm_ID (int VAM_InvTrf_Confirm_ID)
{
if (VAM_InvTrf_Confirm_ID < 1) throw new ArgumentException ("VAM_InvTrf_Confirm_ID is mandatory.");
Set_ValueNoCheck ("VAM_InvTrf_Confirm_ID", VAM_InvTrf_Confirm_ID);
}
/** Get Move Confirm.
@return Inventory Move Confirmation */
public int GetVAM_InvTrf_Confirm_ID() 
{
Object ii = Get_Value("VAM_InvTrf_Confirm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Move.
@param VAM_InventoryTransfer_ID Movement of Inventory */
public void SetVAM_InventoryTransfer_ID (int VAM_InventoryTransfer_ID)
{
if (VAM_InventoryTransfer_ID < 1) throw new ArgumentException ("VAM_InventoryTransfer_ID is mandatory.");
Set_Value ("VAM_InventoryTransfer_ID", VAM_InventoryTransfer_ID);
}
/** Get Inventory Move.
@return Movement of Inventory */
public int GetVAM_InventoryTransfer_ID() 
{
Object ii = Get_Value("VAM_InventoryTransfer_ID");
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
