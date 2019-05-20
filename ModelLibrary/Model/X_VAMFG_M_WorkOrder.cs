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
/** Generated Model for VAMFG_M_WorkOrder
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAMFG_M_WorkOrder : PO
{
public X_VAMFG_M_WorkOrder (Context ctx, int VAMFG_M_WorkOrder_ID, Trx trxName) : base (ctx, VAMFG_M_WorkOrder_ID, trxName)
{
/** if (VAMFG_M_WorkOrder_ID == 0)
{
SetC_DocType_ID (0);
SetC_UOM_ID (0);
SetDocAction (null);	// PR
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);	// @IsApproved@
SetIsSOTrx (true);	// Y
SetM_Product_ID (0);
SetM_Warehouse_ID (0);	// @#M_Warehouse_ID@
SetProcessed (false);	// N
SetSupervisor_ID (0);
SetVAMFG_DateAcct (DateTime.Now);	// @#Date@
SetVAMFG_M_WorkOrderClass_ID (0);
SetVAMFG_M_WorkOrder_ID (0);
SetVAMFG_PriorityRule (null);	// 5
SetVAMFG_QtyAssembled (0.0);	// 0
SetVAMFG_QtyAvailable (0.0);	// 0
SetVAMFG_QtyEntered (0.0);	// 1
SetVAMFG_QtyScrapped (0.0);	// 0
SetVAMFG_SendEMail (false);	// N
SetVAMFG_WOType (null);	// S
}
 */
}
public X_VAMFG_M_WorkOrder (Ctx ctx, int VAMFG_M_WorkOrder_ID, Trx trxName) : base (ctx, VAMFG_M_WorkOrder_ID, trxName)
{
/** if (VAMFG_M_WorkOrder_ID == 0)
{
SetC_DocType_ID (0);
SetC_UOM_ID (0);
SetDocAction (null);	// PR
SetDocStatus (null);	// DR
SetDocumentNo (null);
SetIsApproved (false);	// @IsApproved@
SetIsSOTrx (true);	// Y
SetM_Product_ID (0);
SetM_Warehouse_ID (0);	// @#M_Warehouse_ID@
SetProcessed (false);	// N
SetSupervisor_ID (0);
SetVAMFG_DateAcct (DateTime.Now);	// @#Date@
SetVAMFG_M_WorkOrderClass_ID (0);
SetVAMFG_M_WorkOrder_ID (0);
SetVAMFG_PriorityRule (null);	// 5
SetVAMFG_QtyAssembled (0.0);	// 0
SetVAMFG_QtyAvailable (0.0);	// 0
SetVAMFG_QtyEntered (0.0);	// 1
SetVAMFG_QtyScrapped (0.0);	// 0
SetVAMFG_SendEMail (false);	// N
SetVAMFG_WOType (null);	// S
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAMFG_M_WorkOrder (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAMFG_M_WorkOrder (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAMFG_M_WorkOrder (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAMFG_M_WorkOrder()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27638802652062L;
/** Last Updated Timestamp 12/28/2012 12:18:55 PM */
public static long updatedMS = 1356677335273L;
/** AD_Table_ID=1000404 */
public static int Table_ID;
 // =1000404;

/** TableName=VAMFG_M_WorkOrder */
public static String Table_Name="VAMFG_M_WorkOrder";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAMFG_M_WorkOrder[").Append(Get_ID()).Append("]");
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
throw new ArgumentException ("C_Campaign_ID Is virtual column");
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
Set_ValueNoCheck ("C_DocType_ID", C_DocType_ID);
}
/** Get Document Type.
@return Document type or rules */
public int GetC_DocType_ID() 
{
Object ii = Get_Value("C_DocType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order Line.
@param C_OrderLine_ID Order Line */
public void SetC_OrderLine_ID (int C_OrderLine_ID)
{
if (C_OrderLine_ID <= 0) Set_Value ("C_OrderLine_ID", null);
else
Set_Value ("C_OrderLine_ID", C_OrderLine_ID);
}
/** Get Order Line.
@return Order Line */
public int GetC_OrderLine_ID() 
{
Object ii = Get_Value("C_OrderLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param C_Order_ID Sales Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_Value ("C_Order_ID", null);
else
Set_Value ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Sales Order */
public int GetC_Order_ID() 
{
Object ii = Get_Value("C_Order_ID");
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
/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
public void SetIsSOTrx (Boolean IsSOTrx)
{
Set_Value ("IsSOTrx", IsSOTrx);
}
/** Get Sales Transaction.
@return This is a Sales Transaction */
public Boolean IsSOTrx() 
{
Object oo = Get_Value("IsSOTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Plan Run.
@param MRP_PlanRun_ID An execution of the plan */
public void SetMRP_PlanRun_ID (int MRP_PlanRun_ID)
{
if (MRP_PlanRun_ID <= 0) Set_ValueNoCheck ("MRP_PlanRun_ID", null);
else
Set_ValueNoCheck ("MRP_PlanRun_ID", MRP_PlanRun_ID);
}
/** Get Plan Run.
@return An execution of the plan */
public int GetMRP_PlanRun_ID() 
{
Object ii = Get_Value("MRP_PlanRun_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID <= 0) Set_Value ("M_AttributeSetInstance_ID", null);
else
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BOM.
@param M_BOM_ID Bill of Material */
public void SetM_BOM_ID (int M_BOM_ID)
{
if (M_BOM_ID <= 0) Set_Value ("M_BOM_ID", null);
else
Set_Value ("M_BOM_ID", M_BOM_ID);
}
/** Get BOM.
@return Bill of Material */
public int GetM_BOM_ID() 
{
Object ii = Get_Value("M_BOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
Set_ValueNoCheck ("Processed", Processed);
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

/** SalesRep_ID AD_Reference_ID=1000142 */
public static int SALESREP_ID_AD_Reference_ID=1000142;
/** Set Sales Rep.
@param SalesRep_ID Company Agent like Sales Representitive, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID <= 0) Set_Value ("SalesRep_ID", null);
else
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Sales Rep.
@return Company Agent like Sales Representitive, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Supervisor_ID AD_Reference_ID=1000142 */
public static int SUPERVISOR_ID_AD_Reference_ID=1000142;
/** Set Supervisor.
@param Supervisor_ID Supervisor for this user/organization - used for escalation and approval */
public void SetSupervisor_ID (int Supervisor_ID)
{
if (Supervisor_ID < 1) throw new ArgumentException ("Supervisor_ID is mandatory.");
Set_Value ("Supervisor_ID", Supervisor_ID);
}
/** Get Supervisor.
@return Supervisor for this user/organization - used for escalation and approval */
public int GetSupervisor_ID() 
{
Object ii = Get_Value("Supervisor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Copy From.
@param VAMFG_CopyFrom Copy From Record */
public void SetVAMFG_CopyFrom (String VAMFG_CopyFrom)
{
if (VAMFG_CopyFrom != null && VAMFG_CopyFrom.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_CopyFrom = VAMFG_CopyFrom.Substring(0,1);
}
Set_Value ("VAMFG_CopyFrom", VAMFG_CopyFrom);
}
/** Get Copy From.
@return Copy From Record */
public String GetVAMFG_CopyFrom() 
{
return (String)Get_Value("VAMFG_CopyFrom");
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
/** Set Actual Date From.
@param VAMFG_DateActualFrom Actual date an activity started */
public void SetVAMFG_DateActualFrom (DateTime? VAMFG_DateActualFrom)
{
Set_Value ("VAMFG_DateActualFrom", (DateTime?)VAMFG_DateActualFrom);
}
/** Get Actual Date From.
@return Actual date an activity started */
public DateTime? GetVAMFG_DateActualFrom() 
{
return (DateTime?)Get_Value("VAMFG_DateActualFrom");
}
/** Set Actual Date To.
@param VAMFG_DateActualTo Actual date an activity ended */
public void SetVAMFG_DateActualTo (DateTime? VAMFG_DateActualTo)
{
Set_Value ("VAMFG_DateActualTo", (DateTime?)VAMFG_DateActualTo);
}
/** Get Actual Date To.
@return Actual date an activity ended */
public DateTime? GetVAMFG_DateActualTo() 
{
return (DateTime?)Get_Value("VAMFG_DateActualTo");
}
/** Set Scheduled Date From.
@param VAMFG_DateScheduleFrom Date an activity is scheduled to start */
public void SetVAMFG_DateScheduleFrom (DateTime? VAMFG_DateScheduleFrom)
{
Set_Value ("VAMFG_DateScheduleFrom", (DateTime?)VAMFG_DateScheduleFrom);
}
/** Get Scheduled Date From.
@return Date an activity is scheduled to start */
public DateTime? GetVAMFG_DateScheduleFrom() 
{
return (DateTime?)Get_Value("VAMFG_DateScheduleFrom");
}
/** Set Scheduled Date To.
@param VAMFG_DateScheduleTo Date an activity is scheduled to end */
public void SetVAMFG_DateScheduleTo (DateTime? VAMFG_DateScheduleTo)
{
Set_Value ("VAMFG_DateScheduleTo", (DateTime?)VAMFG_DateScheduleTo);
}
/** Get Scheduled Date To.
@return Date an activity is scheduled to end */
public DateTime? GetVAMFG_DateScheduleTo() 
{
return (DateTime?)Get_Value("VAMFG_DateScheduleTo");
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
/** Set Routing.
@param VAMFG_M_Routing_ID Routing for an assembly */
public void SetVAMFG_M_Routing_ID (int VAMFG_M_Routing_ID)
{
if (VAMFG_M_Routing_ID <= 0) Set_Value ("VAMFG_M_Routing_ID", null);
else
Set_Value ("VAMFG_M_Routing_ID", VAMFG_M_Routing_ID);
}
/** Get Routing.
@return Routing for an assembly */
public int GetVAMFG_M_Routing_ID() 
{
Object ii = Get_Value("VAMFG_M_Routing_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Work Order Class.
@param VAMFG_M_WorkOrderClass_ID Indicates the document types and accounts to be used for a work order. */
public void SetVAMFG_M_WorkOrderClass_ID (int VAMFG_M_WorkOrderClass_ID)
{
if (VAMFG_M_WorkOrderClass_ID < 1) throw new ArgumentException ("VAMFG_M_WorkOrderClass_ID is mandatory.");
Set_Value ("VAMFG_M_WorkOrderClass_ID", VAMFG_M_WorkOrderClass_ID);
}
/** Get Work Order Class.
@return Indicates the document types and accounts to be used for a work order. */
public int GetVAMFG_M_WorkOrderClass_ID() 
{
Object ii = Get_Value("VAMFG_M_WorkOrderClass_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Work Order.
@param VAMFG_M_WorkOrder_ID Work Order */
public void SetVAMFG_M_WorkOrder_ID (int VAMFG_M_WorkOrder_ID)
{
if (VAMFG_M_WorkOrder_ID < 1) throw new ArgumentException ("VAMFG_M_WorkOrder_ID is mandatory.");
Set_ValueNoCheck ("VAMFG_M_WorkOrder_ID", VAMFG_M_WorkOrder_ID);
}
/** Get Work Order.
@return Work Order */
public int GetVAMFG_M_WorkOrder_ID() 
{
Object ii = Get_Value("VAMFG_M_WorkOrder_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAMFG_PriorityRule AD_Reference_ID=1000141 */
public static int VAMFG_PRIORITYRULE_AD_Reference_ID=1000141;
/** Urgent = 1 */
public static String VAMFG_PRIORITYRULE_Urgent = "1";
/** High = 3 */
public static String VAMFG_PRIORITYRULE_High = "3";
/** Medium = 5 */
public static String VAMFG_PRIORITYRULE_Medium = "5";
/** Low = 7 */
public static String VAMFG_PRIORITYRULE_Low = "7";
/** Minor = 9 */
public static String VAMFG_PRIORITYRULE_Minor = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_PriorityRuleValid (String test)
{
return test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
}
/** Set Priority.
@param VAMFG_PriorityRule Priority of a document */
public void SetVAMFG_PriorityRule (String VAMFG_PriorityRule)
{
if (VAMFG_PriorityRule == null) throw new ArgumentException ("VAMFG_PriorityRule is mandatory");
if (!IsVAMFG_PriorityRuleValid(VAMFG_PriorityRule))
throw new ArgumentException ("VAMFG_PriorityRule Invalid value - " + VAMFG_PriorityRule + " - Reference_ID=1000141 - 1 - 3 - 5 - 7 - 9");
if (VAMFG_PriorityRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_PriorityRule = VAMFG_PriorityRule.Substring(0,1);
}
Set_Value ("VAMFG_PriorityRule", VAMFG_PriorityRule);
}
/** Get Priority.
@return Priority of a document */
public String GetVAMFG_PriorityRule() 
{
return (String)Get_Value("VAMFG_PriorityRule");
}
/** Set Quantity Assembled.
@param VAMFG_QtyAssembled Quantity finished at a production routing step */
public void SetVAMFG_QtyAssembled (Decimal? VAMFG_QtyAssembled)
{
if (VAMFG_QtyAssembled == null) throw new ArgumentException ("VAMFG_QtyAssembled is mandatory.");
Set_Value ("VAMFG_QtyAssembled", (Decimal?)VAMFG_QtyAssembled);
}
/** Get Quantity Assembled.
@return Quantity finished at a production routing step */
public Decimal GetVAMFG_QtyAssembled() 
{
Object bd =Get_Value("VAMFG_QtyAssembled");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Available Quantity.
@param VAMFG_QtyAvailable Available Quantity (On Hand - Reserved) */
public void SetVAMFG_QtyAvailable (Decimal? VAMFG_QtyAvailable)
{
if (VAMFG_QtyAvailable == null) throw new ArgumentException ("VAMFG_QtyAvailable is mandatory.");
Set_Value ("VAMFG_QtyAvailable", (Decimal?)VAMFG_QtyAvailable);
}
/** Get Available Quantity.
@return Available Quantity (On Hand - Reserved) */
public Decimal GetVAMFG_QtyAvailable() 
{
Object bd =Get_Value("VAMFG_QtyAvailable");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Quantity Scrapped.
@param VAMFG_QtyScrapped This is the number of sub-assemblies in the Scrap step of an operation in Work Order. */
public void SetVAMFG_QtyScrapped (Decimal? VAMFG_QtyScrapped)
{
if (VAMFG_QtyScrapped == null) throw new ArgumentException ("VAMFG_QtyScrapped is mandatory.");
Set_Value ("VAMFG_QtyScrapped", (Decimal?)VAMFG_QtyScrapped);
}
/** Get Quantity Scrapped.
@return This is the number of sub-assemblies in the Scrap step of an operation in Work Order. */
public Decimal GetVAMFG_QtyScrapped() 
{
Object bd =Get_Value("VAMFG_QtyScrapped");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Send EMail.
@param VAMFG_SendEMail Enable sending Document EMail */
public void SetVAMFG_SendEMail (Boolean VAMFG_SendEMail)
{
Set_Value ("VAMFG_SendEMail", VAMFG_SendEMail);
}
/** Get Send EMail.
@return Enable sending Document EMail */
public Boolean IsVAMFG_SendEMail() 
{
Object oo = Get_Value("VAMFG_SendEMail");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** VAMFG_User1_ID AD_Reference_ID=1000143 */
public static int VAMFG_USER1_ID_AD_Reference_ID=1000143;
/** Set User List 1.
@param VAMFG_User1_ID User defined list element #1 */
public void SetVAMFG_User1_ID (int VAMFG_User1_ID)
{
if (VAMFG_User1_ID <= 0) Set_Value ("VAMFG_User1_ID", null);
else
Set_Value ("VAMFG_User1_ID", VAMFG_User1_ID);
}
/** Get User List 1.
@return User defined list element #1 */
public int GetVAMFG_User1_ID() 
{
Object ii = Get_Value("VAMFG_User1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAMFG_User2_ID AD_Reference_ID=1000144 */
public static int VAMFG_USER2_ID_AD_Reference_ID=1000144;
/** Set User List 2.
@param VAMFG_User2_ID User defined list element #2 */
public void SetVAMFG_User2_ID (int VAMFG_User2_ID)
{
if (VAMFG_User2_ID <= 0) Set_Value ("VAMFG_User2_ID", null);
else
Set_Value ("VAMFG_User2_ID", VAMFG_User2_ID);
}
/** Get User List 2.
@return User defined list element #2 */
public int GetVAMFG_User2_ID() 
{
Object ii = Get_Value("VAMFG_User2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAMFG_WOSource AD_Reference_ID=1000145 */
public static int VAMFG_WOSOURCE_AD_Reference_ID=1000145;
/** Sales Order = O */
public static String VAMFG_WOSOURCE_SalesOrder = "O";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_WOSourceValid (String test)
{
return test == null || test.Equals("O");
}
/** Set Source.
@param VAMFG_WOSource Work Order source */
public void SetVAMFG_WOSource (String VAMFG_WOSource)
{
if (!IsVAMFG_WOSourceValid(VAMFG_WOSource))
throw new ArgumentException ("VAMFG_WOSource Invalid value - " + VAMFG_WOSource + " - Reference_ID=1000145 - O");
if (VAMFG_WOSource != null && VAMFG_WOSource.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_WOSource = VAMFG_WOSource.Substring(0,1);
}
Set_Value ("VAMFG_WOSource", VAMFG_WOSource);
}
/** Get Source.
@return Work Order source */
public String GetVAMFG_WOSource() 
{
return (String)Get_Value("VAMFG_WOSource");
}

/** VAMFG_WOType AD_Reference_ID=1000146 */
public static int VAMFG_WOTYPE_AD_Reference_ID=1000146;
/** Refurbish = F */
public static String VAMFG_WOTYPE_Refurbish = "F";
/** Repair = R */
public static String VAMFG_WOTYPE_Repair = "R";
/** Standard = S */
public static String VAMFG_WOTYPE_Standard = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsVAMFG_WOTypeValid (String test)
{
return test.Equals("F") || test.Equals("R") || test.Equals("S");
}
/** Set Work Order Type.
@param VAMFG_WOType Work Order Type */
public void SetVAMFG_WOType (String VAMFG_WOType)
{
if (VAMFG_WOType == null) throw new ArgumentException ("VAMFG_WOType is mandatory");
if (!IsVAMFG_WOTypeValid(VAMFG_WOType))
throw new ArgumentException ("VAMFG_WOType Invalid value - " + VAMFG_WOType + " - Reference_ID=1000146 - F - R - S");
if (VAMFG_WOType.Length > 1)
{
log.Warning("Length > 1 - truncated");
VAMFG_WOType = VAMFG_WOType.Substring(0,1);
}
Set_Value ("VAMFG_WOType", VAMFG_WOType);
}
/** Get Work Order Type.
@return Work Order Type */
public String GetVAMFG_WOType() 
{
return (String)Get_Value("VAMFG_WOType");
}
}

}
