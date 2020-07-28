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
    using System.Data;/** Generated Model for M_CostAllocation
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_CostAllocation : PO
    {
        public X_M_CostAllocation(Context ctx, int M_CostAllocation_ID, Trx trxName)
            : base(ctx, M_CostAllocation_ID, trxName)
        {/** if (M_CostAllocation_ID == 0){SetAllocationAmt (0.0);// 0.0
SetC_Currency_ID (0);SetC_Invoice_ID (0);SetC_Payment_ID (0);SetDateAcct (DateTime.Now);// sysdate
SetDocStatus (null);// DR
SetM_CostAllocation_ID (0);SetTRXDATE (DateTime.Now);// sysdate
} */
        }
        public X_M_CostAllocation(Ctx ctx, int M_CostAllocation_ID, Trx trxName)
            : base(ctx, M_CostAllocation_ID, trxName)
        {/** if (M_CostAllocation_ID == 0){SetAllocationAmt (0.0);// 0.0
SetC_Currency_ID (0);SetC_Invoice_ID (0);SetC_Payment_ID (0);SetDateAcct (DateTime.Now);// sysdate
SetDocStatus (null);// DR
SetM_CostAllocation_ID (0);SetTRXDATE (DateTime.Now);// sysdate
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_CostAllocation(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_CostAllocation(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_CostAllocation(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_CostAllocation() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27751894680062L;/** Last Updated Timestamp 7/29/2016 10:46:03 AM */
        public static long updatedMS = 1469769363273L;/** AD_Table_ID=1000748 */
        public static int Table_ID; // =1000748;
        /** TableName=M_CostAllocation */
        public static String Table_Name = "M_CostAllocation";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_CostAllocation[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Total Allocation Amount.
@param AllocationAmt Total Allocation Amount */
        public void SetAllocationAmt(Decimal? AllocationAmt) { if (AllocationAmt == null) throw new ArgumentException("AllocationAmt is mandatory."); Set_Value("AllocationAmt", (Decimal?)AllocationAmt); }/** Get Total Allocation Amount.
@return Total Allocation Amount */
        public Decimal GetAllocationAmt() { Object bd = Get_Value("AllocationAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID) { if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory."); Set_Value("C_Currency_ID", C_Currency_ID); }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Type.
@param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID <= 0) Set_Value("C_DocType_ID", null);
            else
                Set_Value("C_DocType_ID", C_DocType_ID);
        }/** Get Document Type.
@return Document type or rules */
        public int GetC_DocType_ID() { Object ii = Get_Value("C_DocType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID) { if (C_Invoice_ID < 1) throw new ArgumentException("C_Invoice_ID is mandatory."); Set_Value("C_Invoice_ID", C_Invoice_ID); }/** Get Invoice.
@return Invoice Identifier */
        public int GetC_Invoice_ID() { Object ii = Get_Value("C_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment.
@param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID) { if (C_Payment_ID < 1) throw new ArgumentException("C_Payment_ID is mandatory."); Set_Value("C_Payment_ID", C_Payment_ID); }/** Get Payment.
@return Payment identifier */
        public int GetC_Payment_ID() { Object ii = Get_Value("C_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Account Date.
@param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct) { if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory."); Set_Value("DateAcct", (DateTime?)DateAcct); }/** Get Account Date.
@return General Ledger Date */
        public DateTime? GetDateAcct() { return (DateTime?)Get_Value("DateAcct"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }
        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;/** <None> = -- */
        public static String DOCACTION_None = "--";/** Approve = AP */
        public static String DOCACTION_Approve = "AP";/** Close = CL */
        public static String DOCACTION_Close = "CL";/** Complete = CO */
        public static String DOCACTION_Complete = "CO";/** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";/** Post = PO */
        public static String DOCACTION_Post = "PO";/** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";/** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";/** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";/** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";/** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";/** Void = VO */
        public static String DOCACTION_Void = "VO";/** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";/** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocActionValid(String test) { return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL"); }/** Set Document Action.
@param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL"); if (DocAction != null && DocAction.Length > 2) { log.Warning("Length > 2 - truncated"); DocAction = DocAction.Substring(0, 2); } Set_Value("DocAction", DocAction);
        }/** Get Document Action.
@return The targeted status of the document */
        public String GetDocAction() { return (String)Get_Value("DocAction"); }
        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;/** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";/** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";/** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";/** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";/** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";/** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";/** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";/** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";/** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";/** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";/** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";/** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocStatusValid(String test) { return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP"); }/** Set Document Status.
@param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory"); if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP"); if (DocStatus.Length > 2) { log.Warning("Length > 2 - truncated"); DocStatus = DocStatus.Substring(0, 2); } Set_Value("DocStatus", DocStatus);
        }/** Get Document Status.
@return The current status of the document */
        public String GetDocStatus() { return (String)Get_Value("DocStatus"); }/** Set DocumentNo.
@param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo) { if (DocumentNo != null && DocumentNo.Length > 100) { log.Warning("Length > 100 - truncated"); DocumentNo = DocumentNo.Substring(0, 100); } Set_Value("DocumentNo", DocumentNo); }/** Get DocumentNo.
@return Document sequence number of the document */
        public String GetDocumentNo() { return (String)Get_Value("DocumentNo"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Approved.
@param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved) { Set_Value("IsApproved", IsApproved); }/** Get Approved.
@return Indicates if this document requires approval */
        public Boolean IsApproved() { Object oo = Get_Value("IsApproved"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Cost Calculated.
@param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }/** Get Cost Calculated.
@return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set IsProcessed.
@param IsProcessed IsProcessed */
        public void SetIsProcessed(Boolean IsProcessed) { Set_Value("IsProcessed", IsProcessed); }/** Get IsProcessed.
@return IsProcessed */
        public Boolean IsProcessed() { Object oo = Get_Value("IsProcessed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Reverse Cost Calculated.
@param IsReverseCostCalculated Reverse Cost Calculated */
        public void SetIsReverseCostCalculated(Boolean IsReverseCostCalculated) { Set_Value("IsReverseCostCalculated", IsReverseCostCalculated); }/** Get Reverse Cost Calculated.
@return Reverse Cost Calculated */
        public Boolean IsReverseCostCalculated() { Object oo = Get_Value("IsReverseCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Cost Allocation.
@param M_CostAllocation_ID Cost Allocation */
        public void SetM_CostAllocation_ID(int M_CostAllocation_ID) { if (M_CostAllocation_ID < 1) throw new ArgumentException("M_CostAllocation_ID is mandatory."); Set_ValueNoCheck("M_CostAllocation_ID", M_CostAllocation_ID); }/** Get Cost Allocation.
@return Cost Allocation */
        public int GetM_CostAllocation_ID() { Object ii = Get_Value("M_CostAllocation_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Posted.
@param Posted Posting status */
        public void SetPosted(Boolean Posted) { Set_Value("Posted", Posted); }/** Get Posted.
@return Posting status */
        public Boolean IsPosted() { Object oo = Get_Value("Posted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set TRXDATE.
@param TRXDATE TRXDATE */
        public void SetTRXDATE(DateTime? TRXDATE) { if (TRXDATE == null) throw new ArgumentException("TRXDATE is mandatory."); Set_Value("TRXDATE", (DateTime?)TRXDATE); }/** Get TRXDATE.
@return TRXDATE */
        public DateTime? GetTRXDATE() { return (DateTime?)Get_Value("TRXDATE"); }
    }
}