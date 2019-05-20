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
/** Generated Model for M_Requisition
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Requisition : PO
{
public X_M_Requisition (Context ctx, int M_Requisition_ID, Trx trxName) : base (ctx, M_Requisition_ID, trxName)
{
/** if (M_Requisition_ID == 0)
{
SetAD_User_ID (0);
SetC_DocType_ID (0);
SetDateDoc (DateTime.Now);	// @#Date@
SetDateRequired (DateTime.Now);
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);
SetM_PriceList_ID (0);
SetM_Requisition_ID (0);
SetM_Warehouse_ID (0);
SetPosted (false);
SetPriorityRule (null);	// 5
SetProcessed (false);	// N
SetTotalLines (0.0);
}
 */
}
public X_M_Requisition (Ctx ctx, int M_Requisition_ID, Trx trxName) : base (ctx, M_Requisition_ID, trxName)
{
/** if (M_Requisition_ID == 0)
{
SetAD_User_ID (0);
SetC_DocType_ID (0);
SetDateDoc (DateTime.Now);	// @#Date@
SetDateRequired (DateTime.Now);
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);
SetM_PriceList_ID (0);
SetM_Requisition_ID (0);
SetM_Warehouse_ID (0);
SetPosted (false);
SetPriorityRule (null);	// 5
SetProcessed (false);	// N
SetTotalLines (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Requisition (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Requisition (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Requisition (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Requisition()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381121L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064332L;
/** AD_Table_ID=702 */
public static int Table_ID;
 // =702;

/** TableName=M_Requisition */
public static String Table_Name="M_Requisition";

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
StringBuilder sb = new StringBuilder ("X_M_Requisition[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
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
/** Set Document Type.
@param C_DocType_ID Document type or rules */
public void SetC_DocType_ID (int C_DocType_ID)
{
if (C_DocType_ID < 0) throw new ArgumentException ("C_DocType_ID is mandatory.");
Set_Value ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document Date.
@param DateDoc Date of the Document */
public void SetDateDoc (DateTime? DateDoc)
{
if (DateDoc == null) throw new ArgumentException ("DateDoc is mandatory.");
Set_Value ("DateDoc", (DateTime?)DateDoc);
}
/** Get Document Date.
@return Date of the Document */
public DateTime? GetDateDoc() 
{
return (DateTime?)Get_Value("DateDoc");
}
/** Set Date Required.
@param DateRequired Date when required */
public void SetDateRequired (DateTime? DateRequired)
{
if (DateRequired == null) throw new ArgumentException ("DateRequired is mandatory.");
Set_Value ("DateRequired", (DateTime?)DateRequired);
}
/** Get Date Required.
@return Date when required */
public DateTime? GetDateRequired() 
{
return (DateTime?)Get_Value("DateRequired");
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
Set_ValueNoCheck ("DocumentNo", DocumentNo);
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
/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
public void SetM_PriceList_ID (int M_PriceList_ID)
{
if (M_PriceList_ID < 1) throw new ArgumentException ("M_PriceList_ID is mandatory.");
Set_Value ("M_PriceList_ID", M_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetM_PriceList_ID() 
{
Object ii = Get_Value("M_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Requisition.
@param M_Requisition_ID Material Requisition */
public void SetM_Requisition_ID (int M_Requisition_ID)
{
if (M_Requisition_ID < 1) throw new ArgumentException ("M_Requisition_ID is mandatory.");
Set_ValueNoCheck ("M_Requisition_ID", M_Requisition_ID);
}
/** Get Requisition.
@return Material Requisition */
public int GetM_Requisition_ID() 
{
Object ii = Get_Value("M_Requisition_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Posted.
@param Posted Posting status */
public void SetPosted (Boolean Posted)
{
Set_Value ("Posted", Posted);
}
/** Get Posted.
@return Posting status */
public Boolean IsPosted() 
{
Object oo = Get_Value("Posted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** PriorityRule AD_Reference_ID=154 */
public static int PRIORITYRULE_AD_Reference_ID=154;
/** Urgent = 1 */
public static String PRIORITYRULE_Urgent = "1";
/** High = 3 */
public static String PRIORITYRULE_High = "3";
/** Medium = 5 */
public static String PRIORITYRULE_Medium = "5";
/** Low = 7 */
public static String PRIORITYRULE_Low = "7";
/** Minor = 9 */
public static String PRIORITYRULE_Minor = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPriorityRuleValid (String test)
{
return test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
}
/** Set Priority.
@param PriorityRule Priority of a document */
public void SetPriorityRule (String PriorityRule)
{
if (PriorityRule == null) throw new ArgumentException ("PriorityRule is mandatory");
if (!IsPriorityRuleValid(PriorityRule))
throw new ArgumentException ("PriorityRule Invalid value - " + PriorityRule + " - Reference_ID=154 - 1 - 3 - 5 - 7 - 9");
if (PriorityRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
PriorityRule = PriorityRule.Substring(0,1);
}
Set_Value ("PriorityRule", PriorityRule);
}
/** Get Priority.
@return Priority of a document */
public String GetPriorityRule() 
{
return (String)Get_Value("PriorityRule");
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
/** Set SubTotal.
@param TotalLines Total of all document lines (excluding Tax) */
public void SetTotalLines (Decimal? TotalLines)
{
if (TotalLines == null) throw new ArgumentException ("TotalLines is mandatory.");
Set_Value ("TotalLines", (Decimal?)TotalLines);
}
/** Get SubTotal.
@return Total of all document lines (excluding Tax) */
public Decimal GetTotalLines() 
{
Object bd =Get_Value("TotalLines");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** DTD001_MWarehouseSource_ID AD_Reference_ID=1000313 */
public static int DTD001_MWAREHOUSESOURCE_ID_AD_Reference_ID = 1000313;
/** Set Source Warehouse.
@param DTD001_MWarehouseSource_ID Source Warehouse */
public void SetDTD001_MWarehouseSource_ID(int DTD001_MWarehouseSource_ID)
{
    if (DTD001_MWarehouseSource_ID <= 0) Set_Value("DTD001_MWarehouseSource_ID", null);
    else
        Set_Value("DTD001_MWarehouseSource_ID", DTD001_MWarehouseSource_ID);
}
/** Get Source Warehouse.
@return Source Warehouse */
public int GetDTD001_MWarehouseSource_ID()
{
    Object ii = Get_Value("DTD001_MWarehouseSource_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
    
}

}
