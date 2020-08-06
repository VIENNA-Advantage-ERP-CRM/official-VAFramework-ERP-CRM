namespace ViennaAdvantage.Model
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
/** Generated Model for VAMFG_M_WrkOdrTransaction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAMFG_M_WrkOdrTransaction : PO
{
public X_VAMFG_M_WrkOdrTransaction (Context ctx, int VAMFG_M_WrkOdrTransaction_ID, Trx trxName) : base (ctx, VAMFG_M_WrkOdrTransaction_ID, trxName)
{
/** if (VAMFG_M_WrkOdrTransaction_ID == 0)
{
SetC_DocType_ID (0);
SetC_UOM_ID (0);	// @#C_UOM_ID@
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);	// @IsApproved@
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetPosted (false);	// N
SetProcessed (false);	// N
SetVAMFG_DateAcct (DateTime.Now);	// @#Date@
SetVAMFG_IsOptionalFrom (false);	// N
SetVAMFG_IsOptionalTo (false);	// N
SetVAMFG_M_WorkOrder_ID (0);
SetVAMFG_M_WrkOdrTransaction_ID (0);
SetVAMFG_NewWOBarcode (null);	// CO
SetVAMFG_QtyEntered (0.0);	// 1
SetVAMFG_WOComplete (false);	// N
SetVAMFG_WOTxnSource (null);	// M
SetVAMFG_WorkOrderTxnType (null);
}
 */
}
public X_VAMFG_M_WrkOdrTransaction (Ctx ctx, int VAMFG_M_WrkOdrTransaction_ID, Trx trxName) : base (ctx, VAMFG_M_WrkOdrTransaction_ID, trxName)
{
/** if (VAMFG_M_WrkOdrTransaction_ID == 0)
{
SetC_DocType_ID (0);
SetC_UOM_ID (0);	// @#C_UOM_ID@
SetDocAction (null);	// CO
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);	// @IsApproved@
SetM_Product_ID (0);
SetM_Warehouse_ID (0);
SetPosted (false);	// N
SetProcessed (false);	// N
SetVAMFG_DateAcct (DateTime.Now);	// @#Date@
SetVAMFG_IsOptionalFrom (false);	// N
SetVAMFG_IsOptionalTo (false);	// N
SetVAMFG_M_WorkOrder_ID (0);
SetVAMFG_M_WrkOdrTransaction_ID (0);
SetVAMFG_NewWOBarcode (null);	// CO
SetVAMFG_QtyEntered (0.0);	// 1
SetVAMFG_WOComplete (false);	// N
SetVAMFG_WOTxnSource (null);	// M
SetVAMFG_WorkOrderTxnType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAMFG_M_WrkOdrTransaction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAMFG_M_WrkOdrTransaction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAMFG_M_WrkOdrTransaction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAMFG_M_WrkOdrTransaction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27638802505310L;
/** Last Updated Timestamp 12/28/2012 12:16:28 PM */
public static long updatedMS = 1356677188521L;
/** AD_Table_ID=1000424 */
public static int Table_ID;
 // =1000424;

/** TableName=VAMFG_M_WrkOdrTransaction */
public static String Table_Name="VAMFG_M_WrkOdrTransaction";

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
StringBuilder sb = new StringBuilder ("X_VAMFG_M_WrkOdrTransaction[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_OrgTrx_ID AD_Reference_ID=1000147 */
public static int AD_ORGTRX_ID_AD_Reference_ID=1000147;
/** Set Trx Organization.
@param AD_OrgTrx_ID Performing or initiating organization */
public void SetAD_OrgTrx_ID (int AD_OrgTrx_ID)
{
if (AD_OrgTrx_ID <= 0) Set_Value ("AD_OrgTrx_ID", null);
else
Set_Value ("AD_OrgTrx_ID", AD_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetAD_OrgTrx_ID() 
{
Object ii = Get_Value("AD_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);
else
Set_Value ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Location.
@param C_BPartner_Location_ID Identifies the address for this Account/Prospect. */
public void SetC_BPartner_Location_ID (int C_BPartner_Location_ID)
{
if (C_BPartner_Location_ID <= 0) Set_Value ("C_BPartner_Location_ID", null);
else
Set_Value ("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Location.
@return Identifies the address for this Account/Prospect. */
public int GetC_BPartner_Location_ID() 
{
Object ii = Get_Value("C_BPartner_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);
else
Set_Value ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_DocType_ID AD_Reference_ID=1000135 */
public static int C_DOCTYPE_ID_AD_Reference_ID=1000135;
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
/** Set Opportunity.
@param C_Project_ID Business Opportunity */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Opportunity.
@return Business Opportunity */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID < 1) throw new ArgumentException ("C_UOM_ID is mandatory.");
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set DocumentNo.
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
/** Get DocumentNo.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
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

/** M_Locator_ID AD_Reference_ID=1000134 */
public static int M_LOCATOR_ID_AD_Reference_ID=1000134;
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID <= 0) Set_Value ("M_Locator_ID", null);
else
Set_Value ("M_Locator_ID", M_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() 
{
Object ii = Get_Value("M_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
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

/** OperationFrom_ID AD_Reference_ID=1000152 */
public static int OPERATIONFROM_ID_AD_Reference_ID=1000152;
/** Set Operation From.
@param OperationFrom_ID Process the operations in a work order transaction starting at this one. */
public void SetOperationFrom_ID (int OperationFrom_ID)
{
if (OperationFrom_ID <= 0) Set_Value ("OperationFrom_ID", null);
else
Set_Value ("OperationFrom_ID", OperationFrom_ID);
}
/** Get Operation From.
@return Process the operations in a work order transaction starting at this one. */
public int GetOperationFrom_ID() 
{
Object ii = Get_Value("OperationFrom_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** OperationTo_ID AD_Reference_ID=1000152 */
public static int OPERATIONTO_ID_AD_Reference_ID=1000152;
/** Set Operation To.
@param OperationTo_ID Process the operations in a work order transaction ending at this one (inclusive). */
public void SetOperationTo_ID (int OperationTo_ID)
{
if (OperationTo_ID <= 0) Set_Value ("OperationTo_ID", null);
else
Set_Value ("OperationTo_ID", OperationTo_ID);
}
/** Get Operation To.
@return Process the operations in a work order transaction ending at this one (inclusive). */
public int GetOperationTo_ID() 
{
Object ii = Get_Value("OperationTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ParentWorkOrderTxn_ID AD_Reference_ID=1000154 */
public static int PARENTWORKORDERTXN_ID_AD_Reference_ID=1000154;
/** Set Parent Work Order Transaction.
@param ParentWorkOrderTxn_ID Work Order Transaction that created this Work Order Transaction */
public void SetParentWorkOrderTxn_ID (int ParentWorkOrderTxn_ID)
{
if (ParentWorkOrderTxn_ID <= 0) Set_Value ("ParentWorkOrderTxn_ID", null);
else
Set_Value ("ParentWorkOrderTxn_ID", ParentWorkOrderTxn_ID);
}
/** Get Parent Work Order Transaction.
@return Work Order Transaction that created this Work Order Transaction */
public int GetParentWorkOrderTxn_ID() 
{
Object ii = Get_Value("ParentWorkOrderTxn_ID");
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

/** User1_ID AD_Reference_ID=1000143 */
public static int USER1_ID_AD_Reference_ID=1000143;
/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID)
{
if (User1_ID <= 0) Set_Value ("User1_ID", null);
else
Set_Value ("User1_ID", User1_ID);
}
/** Get User List 1.
@return User defined list element #1 */
public int GetUser1_ID() 
{
Object ii = Get_Value("User1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User2_ID AD_Reference_ID=1000144 */
public static int USER2_ID_AD_Reference_ID=1000144;
/** Set User List 2.
@param User2_ID User defined list element #2 */
public void SetUser2_ID (int User2_ID)
{
if (User2_ID <= 0) Set_Value ("User2_ID", null);
else
Set_Value ("User2_ID", User2_ID);
}
/** Get User List 2.
@return User defined list element #2 */
public int GetUser2_ID() 
{
Object ii = Get_Value("User2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute.
@param VAMFG_Attribute Attribute */
public void SetVAMFG_Attribute (String VAMFG_Attribute)
{
if (VAMFG_Attribute != null && VAMFG_Attribute.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_Attribute = VAMFG_Attribute.Substring(0,1);
}
Set_Value ("VAMFG_Attribute", VAMFG_Attribute);
}
/** Get Attribute.
@return Attribute */
public String GetVAMFG_Attribute() 
{
return (String)Get_Value("VAMFG_Attribute");
}
/** Set BarcodeFashion.
@param VAMFG_BarcodeFashion Barcode for Fashion */
public void SetVAMFG_BarcodeFashion (String VAMFG_BarcodeFashion)
{
if (VAMFG_BarcodeFashion != null && VAMFG_BarcodeFashion.Length > 22)
{
log.Warning("Length > 22 - truncated");
VAMFG_BarcodeFashion = VAMFG_BarcodeFashion.Substring(0,22);
}
Set_Value ("VAMFG_BarcodeFashion", VAMFG_BarcodeFashion);
}
/** Get BarcodeFashion.
@return Barcode for Fashion */
public String GetVAMFG_BarcodeFashion() 
{
return (String)Get_Value("VAMFG_BarcodeFashion");
}
/** Set Account Date.
@param VAMFG_DateAcct General Ledger Date */
public void SetVAMFG_DateAcct (DateTime? VAMFG_DateAcct)
{
if (VAMFG_DateAcct == null) throw new ArgumentException ("VAMFG_DateAcct is mandatory.");
Set_Value ("VAMFG_DateAcct", (DateTime?)VAMFG_DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetVAMFG_DateAcct() 
{
return (DateTime?)Get_Value("VAMFG_DateAcct");
}
/** Set Transaction Date.
@param VAMFG_DateTrx Transaction Date */
public void SetVAMFG_DateTrx (DateTime? VAMFG_DateTrx)
{
Set_Value ("VAMFG_DateTrx", (DateTime?)VAMFG_DateTrx);
}
/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetVAMFG_DateTrx() 
{
return (DateTime?)Get_Value("VAMFG_DateTrx");
}
/** Set Description.
@param VAMFG_Description Optional short description of the record */
public void SetVAMFG_Description (String VAMFG_Description)
{
if (VAMFG_Description != null && VAMFG_Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
VAMFG_Description = VAMFG_Description.Substring(0,255);
}
Set_Value ("VAMFG_Description", VAMFG_Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetVAMFG_Description() 
{
return (String)Get_Value("VAMFG_Description");
}
/** Set Create Component Txn Lines.
@param VAMFG_GenerateLines Generate component lines for Push supply type components  */
public void SetVAMFG_GenerateLines (String VAMFG_GenerateLines)
{
if (VAMFG_GenerateLines != null && VAMFG_GenerateLines.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_GenerateLines = VAMFG_GenerateLines.Substring(0,1);
}
Set_Value ("VAMFG_GenerateLines", VAMFG_GenerateLines);
}
/** Get Create Component Txn Lines.
@return Generate component lines for Push supply type components  */
public String GetVAMFG_GenerateLines() 
{
return (String)Get_Value("VAMFG_GenerateLines");
}
/** Set Generate Resource Usage Lines.
@param VAMFG_GenerateResourceLines Generate resource usage lines for manually charged resources */
public void SetVAMFG_GenerateResourceLines (String VAMFG_GenerateResourceLines)
{
if (VAMFG_GenerateResourceLines != null && VAMFG_GenerateResourceLines.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_GenerateResourceLines = VAMFG_GenerateResourceLines.Substring(0,1);
}
Set_Value ("VAMFG_GenerateResourceLines", VAMFG_GenerateResourceLines);
}
/** Get Generate Resource Usage Lines.
@return Generate resource usage lines for manually charged resources */
public String GetVAMFG_GenerateResourceLines() 
{
return (String)Get_Value("VAMFG_GenerateResourceLines");
}
/** Set GetLocator.
@param VAMFG_GetLocator GetLocator */
public void SetVAMFG_GetLocator (String VAMFG_GetLocator)
{
if (VAMFG_GetLocator != null && VAMFG_GetLocator.Length > 22)
{
log.Warning("Length > 22 - truncated");
VAMFG_GetLocator = VAMFG_GetLocator.Substring(0,22);
}
Set_Value ("VAMFG_GetLocator", VAMFG_GetLocator);
}
/** Get GetLocator.
@return GetLocator */
public String GetVAMFG_GetLocator() 
{
return (String)Get_Value("VAMFG_GetLocator");
}
/** Set Comment.
@param VAMFG_Help Comment Help for hint */
public void SetVAMFG_Help (String VAMFG_Help)
{
if (VAMFG_Help != null && VAMFG_Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
VAMFG_Help = VAMFG_Help.Substring(0,2000);
}
Set_Value ("VAMFG_Help", VAMFG_Help);
}
/** Get Comment.
@return Comment Help for hint */
public String GetVAMFG_Help() 
{
return (String)Get_Value("VAMFG_Help");
}
/** Set Optional.
@param VAMFG_IsOptionalFrom Indicates if the Operation From in the Work Order Move Transaction is an optional operation */
public void SetVAMFG_IsOptionalFrom (Boolean VAMFG_IsOptionalFrom)
{
Set_Value ("VAMFG_IsOptionalFrom", VAMFG_IsOptionalFrom);
}
/** Get Optional.
@return Indicates if the Operation From in the Work Order Move Transaction is an optional operation */
public Boolean IsVAMFG_IsOptionalFrom() 
{
Object oo = Get_Value("VAMFG_IsOptionalFrom");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Optional To.
@param VAMFG_IsOptionalTo Indicates if the Operation To in Work Order Move Transaction is an optional operation */
public void SetVAMFG_IsOptionalTo (Boolean VAMFG_IsOptionalTo)
{
Set_Value ("VAMFG_IsOptionalTo", VAMFG_IsOptionalTo);
}
/** Get Optional To.
@return Indicates if the Operation To in Work Order Move Transaction is an optional operation */
public Boolean IsVAMFG_IsOptionalTo() 
{
Object oo = Get_Value("VAMFG_IsOptionalTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Selected.
@param VAMFG_IsSelected Selected */
public void SetVAMFG_IsSelected (Boolean VAMFG_IsSelected)
{
Set_Value ("VAMFG_IsSelected", VAMFG_IsSelected);
}
/** Get Selected.
@return Selected */
public Boolean IsVAMFG_IsSelected() 
{
Object oo = Get_Value("VAMFG_IsSelected");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Work Order.
@param VAMFG_M_WorkOrder_ID Work Order */
public void SetVAMFG_M_WorkOrder_ID (int VAMFG_M_WorkOrder_ID)
{
if (VAMFG_M_WorkOrder_ID < 1) throw new ArgumentException ("VAMFG_M_WorkOrder_ID is mandatory.");
Set_Value ("VAMFG_M_WorkOrder_ID", VAMFG_M_WorkOrder_ID);
}
/** Get Work Order.
@return Work Order */
public int GetVAMFG_M_WorkOrder_ID() 
{
Object ii = Get_Value("VAMFG_M_WorkOrder_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAMFG_M_WrkOdrTransaction_ID.
@param VAMFG_M_WrkOdrTransaction_ID VAMFG_M_WrkOdrTransaction_ID */
public void SetVAMFG_M_WrkOdrTransaction_ID (int VAMFG_M_WrkOdrTransaction_ID)
{
if (VAMFG_M_WrkOdrTransaction_ID < 1) throw new ArgumentException ("VAMFG_M_WrkOdrTransaction_ID is mandatory.");
Set_ValueNoCheck ("VAMFG_M_WrkOdrTransaction_ID", VAMFG_M_WrkOdrTransaction_ID);
}
/** Get VAMFG_M_WrkOdrTransaction_ID.
@return VAMFG_M_WrkOdrTransaction_ID */
public int GetVAMFG_M_WrkOdrTransaction_ID() 
{
Object ii = Get_Value("VAMFG_M_WrkOdrTransaction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set New Barcode With Expiry.
@param VAMFG_NewBarcode New Barcode With Expiry */
public void SetVAMFG_NewBarcode (String VAMFG_NewBarcode)
{
if (VAMFG_NewBarcode != null && VAMFG_NewBarcode.Length > 22)
{
log.Warning("Length > 22 - truncated");
VAMFG_NewBarcode = VAMFG_NewBarcode.Substring(0,22);
}
Set_Value ("VAMFG_NewBarcode", VAMFG_NewBarcode);
}
/** Get New Barcode With Expiry.
@return New Barcode With Expiry */
public String GetVAMFG_NewBarcode() 
{
return (String)Get_Value("VAMFG_NewBarcode");
}

/** VAMFG_NewWOBarcode AD_Reference_ID=1000138 */
public static int VAMFG_NEWWOBARCODE_AD_Reference_ID=1000138;
/** <None> = -- */
public static String VAMFG_NEWWOBARCODE_None = "--";
/** Approve = AP */
public static String VAMFG_NEWWOBARCODE_Approve = "AP";
/** Close = CL */
public static String VAMFG_NEWWOBARCODE_Close = "CL";
/** Complete = CO */
public static String VAMFG_NEWWOBARCODE_Complete = "CO";
/** Invalidate = IN */
public static String VAMFG_NEWWOBARCODE_Invalidate = "IN";
/** Post = PO */
public static String VAMFG_NEWWOBARCODE_Post = "PO";
/** Prepare = PR */
public static String VAMFG_NEWWOBARCODE_Prepare = "PR";
/** Reverse - Accrual = RA */
public static String VAMFG_NEWWOBARCODE_Reverse_Accrual = "RA";
/** Reverse - Correct = RC */
public static String VAMFG_NEWWOBARCODE_Reverse_Correct = "RC";
/** Re-activate = RE */
public static String VAMFG_NEWWOBARCODE_Re_Activate = "RE";
/** Reject = RJ */
public static String VAMFG_NEWWOBARCODE_Reject = "RJ";
/** Void = VO */
public static String VAMFG_NEWWOBARCODE_Void = "VO";
/** Wait Complete = WC */
public static String VAMFG_NEWWOBARCODE_WaitComplete = "WC";
/** Unlock = XL */
public static String VAMFG_NEWWOBARCODE_Unlock = "XL";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_NewWOBarcodeValid (String test)
{
return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
}
/** Set New Barcode WithOut Expiry.
@param VAMFG_NewWOBarcode New Barcode WithOut Expiry */
public void SetVAMFG_NewWOBarcode (String VAMFG_NewWOBarcode)
{
if (VAMFG_NewWOBarcode == null) throw new ArgumentException ("VAMFG_NewWOBarcode is mandatory");
if (!IsVAMFG_NewWOBarcodeValid(VAMFG_NewWOBarcode))
throw new ArgumentException ("VAMFG_NewWOBarcode Invalid value - " + VAMFG_NewWOBarcode + " - Reference_ID=1000138 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
if (VAMFG_NewWOBarcode.Length > 2)
{
log.Warning("Length > 2 - truncated");
VAMFG_NewWOBarcode = VAMFG_NewWOBarcode.Substring(0,2);
}
Set_Value ("VAMFG_NewWOBarcode", VAMFG_NewWOBarcode);
}
/** Get New Barcode WithOut Expiry.
@return New Barcode WithOut Expiry */
public String GetVAMFG_NewWOBarcode() 
{
return (String)Get_Value("VAMFG_NewWOBarcode");
}
/** Set Quantity.
@param VAMFG_QtyEntered The Quantity Entered is based on the selected UoM */
public void SetVAMFG_QtyEntered (Decimal? VAMFG_QtyEntered)
{
if (VAMFG_QtyEntered == null) throw new ArgumentException ("VAMFG_QtyEntered is mandatory.");
Set_Value ("VAMFG_QtyEntered", (Decimal?)VAMFG_QtyEntered);
}
/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
public Decimal GetVAMFG_QtyEntered() 
{
Object bd =Get_Value("VAMFG_QtyEntered");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** VAMFG_StepFrom AD_Reference_ID=1000155 */
public static int VAMFG_STEPFROM_AD_Reference_ID=1000155;
/** Waiting = Q */
public static String VAMFG_STEPFROM_Waiting = "Q";
/** Process = R */
public static String VAMFG_STEPFROM_Process = "R";
/** Finish = T */
public static String VAMFG_STEPFROM_Finish = "T";
/** Scrap = X */
public static String VAMFG_STEPFROM_Scrap = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_StepFromValid (String test)
{
return test == null || test.Equals("Q") || test.Equals("R") || test.Equals("T") || test.Equals("X");
}
/** Set Step From.
@param VAMFG_StepFrom The source intra-operation step from which the work order movement is being made. */
public void SetVAMFG_StepFrom (String VAMFG_StepFrom)
{
if (!IsVAMFG_StepFromValid(VAMFG_StepFrom))
throw new ArgumentException ("VAMFG_StepFrom Invalid value - " + VAMFG_StepFrom + " - Reference_ID=1000155 - Q - R - T - X");
if (VAMFG_StepFrom != null && VAMFG_StepFrom.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_StepFrom = VAMFG_StepFrom.Substring(0,1);
}
Set_Value ("VAMFG_StepFrom", VAMFG_StepFrom);
}
/** Get Step From.
@return The source intra-operation step from which the work order movement is being made. */
public String GetVAMFG_StepFrom() 
{
return (String)Get_Value("VAMFG_StepFrom");
}

/** VAMFG_StepTo AD_Reference_ID=1000155 */
public static int VAMFG_STEPTO_AD_Reference_ID=1000155;
/** Waiting = Q */
public static String VAMFG_STEPTO_Waiting = "Q";
/** Process = R */
public static String VAMFG_STEPTO_Process = "R";
/** Finish = T */
public static String VAMFG_STEPTO_Finish = "T";
/** Scrap = X */
public static String VAMFG_STEPTO_Scrap = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_StepToValid (String test)
{
return test == null || test.Equals("Q") || test.Equals("R") || test.Equals("T") || test.Equals("X");
}
/** Set StepTo.
@param VAMFG_StepTo The destination intra-operation step to which the work order movement is being done. */
public void SetVAMFG_StepTo (String VAMFG_StepTo)
{
if (!IsVAMFG_StepToValid(VAMFG_StepTo))
throw new ArgumentException ("VAMFG_StepTo Invalid value - " + VAMFG_StepTo + " - Reference_ID=1000155 - Q - R - T - X");
if (VAMFG_StepTo != null && VAMFG_StepTo.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_StepTo = VAMFG_StepTo.Substring(0,1);
}
Set_Value ("VAMFG_StepTo", VAMFG_StepTo);
}
/** Get StepTo.
@return The destination intra-operation step to which the work order movement is being done. */
public String GetVAMFG_StepTo() 
{
return (String)Get_Value("VAMFG_StepTo");
}
/** Set Complete this Assembly.
@param VAMFG_WOComplete Indicates that a move transaction should include a completion for the assembly. */
public void SetVAMFG_WOComplete (Boolean VAMFG_WOComplete)
{
Set_Value ("VAMFG_WOComplete", VAMFG_WOComplete);
}
/** Get Complete this Assembly.
@return Indicates that a move transaction should include a completion for the assembly. */
public Boolean IsVAMFG_WOComplete() 
{
Object oo = Get_Value("VAMFG_WOComplete");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** VAMFG_WOTxnSource AD_Reference_ID=1000156 */
public static int VAMFG_WOTXNSOURCE_AD_Reference_ID=1000156;
/** Generated = G */
public static String VAMFG_WOTXNSOURCE_Generated = "G";
/** Manually Entered = M */
public static String VAMFG_WOTXNSOURCE_ManuallyEntered = "M";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_WOTxnSourceValid (String test)
{
return test.Equals("G") || test.Equals("M");
}
/** Set Transaction Source.
@param VAMFG_WOTxnSource Indicates where the work order transaction originated. */
public void SetVAMFG_WOTxnSource (String VAMFG_WOTxnSource)
{
if (VAMFG_WOTxnSource == null) throw new ArgumentException ("VAMFG_WOTxnSource is mandatory");
if (!IsVAMFG_WOTxnSourceValid(VAMFG_WOTxnSource))
throw new ArgumentException ("VAMFG_WOTxnSource Invalid value - " + VAMFG_WOTxnSource + " - Reference_ID=1000156 - G - M");
if (VAMFG_WOTxnSource.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_WOTxnSource = VAMFG_WOTxnSource.Substring(0,1);
}
Set_Value ("VAMFG_WOTxnSource", VAMFG_WOTxnSource);
}
/** Get Transaction Source.
@return Indicates where the work order transaction originated. */
public String GetVAMFG_WOTxnSource() 
{
return (String)Get_Value("VAMFG_WOTxnSource");
}

/** VAMFG_WorkOrderTxnType AD_Reference_ID=1000151 */
public static int VAMFG_WORKORDERTXNTYPE_AD_Reference_ID=1000151;
/** 3_Transfer Assembly To Store = AI */
public static String VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore = "AI";
/** Assembly Return from Inventory = AR */
public static String VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory = "AR";
/** 1_Component Issue to Work Order = CI */
public static String VAMFG_WORKORDERTXNTYPE_1_ComponentIssueToWorkOrder = "CI";
/** Component Return from Work Order = CR */
public static String VAMFG_WORKORDERTXNTYPE_ComponentReturnFromWorkOrder = "CR";
/** Resource Usage = RU */
public static String VAMFG_WORKORDERTXNTYPE_ResourceUsage = "RU";
/** 2_Work Order In Progress = WM */
public static String VAMFG_WORKORDERTXNTYPE_2_WorkOrderInProgress = "WM";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_WorkOrderTxnTypeValid (String test)
{
return test.Equals("AI") || test.Equals("AR") || test.Equals("CI") || test.Equals("CR") || test.Equals("RU") || test.Equals("WM");
}
/** Set Transaction Type.
@param VAMFG_WorkOrderTxnType Transaction Type */
public void SetVAMFG_WorkOrderTxnType (String VAMFG_WorkOrderTxnType)
{
if (VAMFG_WorkOrderTxnType == null) throw new ArgumentException ("VAMFG_WorkOrderTxnType is mandatory");
if (!IsVAMFG_WorkOrderTxnTypeValid(VAMFG_WorkOrderTxnType))
throw new ArgumentException ("VAMFG_WorkOrderTxnType Invalid value - " + VAMFG_WorkOrderTxnType + " - Reference_ID=1000151 - AI - AR - CI - CR - RU - WM");
if (VAMFG_WorkOrderTxnType.Length > 2)
{
log.Warning("Length > 2 - truncated");
VAMFG_WorkOrderTxnType = VAMFG_WorkOrderTxnType.Substring(0,2);
}
Set_Value ("VAMFG_WorkOrderTxnType", VAMFG_WorkOrderTxnType);
}
/** Get Transaction Type.
@return Transaction Type */
public String GetVAMFG_WorkOrderTxnType() 
{
return (String)Get_Value("VAMFG_WorkOrderTxnType");
}
}

}
