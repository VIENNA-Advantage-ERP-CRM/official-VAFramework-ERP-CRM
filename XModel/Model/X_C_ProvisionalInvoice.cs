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
    using System.Data;/** Generated Model for C_ProvisionalInvoice
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ProvisionalInvoice : PO
    {
        public X_C_ProvisionalInvoice(Context ctx, int C_ProvisionalInvoice_ID, Trx trxName) : base(ctx, C_ProvisionalInvoice_ID, trxName)
        {/** if (C_ProvisionalInvoice_ID == 0){SetC_BPartner_ID (0);SetC_Currency_ID (0);// @C_Currency_ID@
SetC_DocType_ID (0);// -1
SetC_PaymentTerm_ID (0);SetC_ProvisionalInvoice_ID (0);SetDocStatus (null);// DR
SetM_PriceList_ID (0);// -1
SetPosted (false);// N
SetProcessed (false);// N
} */
        }
        public X_C_ProvisionalInvoice(Ctx ctx, int C_ProvisionalInvoice_ID, Trx trxName) : base(ctx, C_ProvisionalInvoice_ID, trxName)
        {/** if (C_ProvisionalInvoice_ID == 0){SetC_BPartner_ID (0);SetC_Currency_ID (0);// @C_Currency_ID@
SetC_DocType_ID (0);// -1
SetC_PaymentTerm_ID (0);SetC_ProvisionalInvoice_ID (0);SetDocStatus (null);// DR
SetM_PriceList_ID (0);// -1
SetPosted (false);// N
SetProcessed (false);// N
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoice(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoice(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoice(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_ProvisionalInvoice() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27909665790665L;/** Last Updated Timestamp 7/29/2021 6:34:33 AM */
        public static long updatedMS = 1627540473876L;/** AD_Table_ID=1000551 */
        public static int Table_ID; // =1000551;
        /** TableName=C_ProvisionalInvoice */
        public static String Table_Name = "C_ProvisionalInvoice";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_ProvisionalInvoice[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID() { Object ii = Get_Value("AD_User_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID) { if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory."); Set_Value("C_BPartner_ID", C_BPartner_ID); }/** Get Business Partner.
@return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID() { Object ii = Get_Value("C_BPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Location.
@param C_BPartner_Location_ID Identifies the address for this Account/Prospect. */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }/** Get Location.
@return Identifies the address for this Account/Prospect. */
        public int GetC_BPartner_Location_ID() { Object ii = Get_Value("C_BPartner_Location_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }/** Get Campaign.
@return Marketing Campaign */
        public int GetC_Campaign_ID() { Object ii = Get_Value("C_Campaign_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency Rate Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            if (C_ConversionType_ID <= 0) Set_Value("C_ConversionType_ID", null);
            else
                Set_Value("C_ConversionType_ID", C_ConversionType_ID);
        }/** Get Currency Rate Type.
@return Currency Conversion Rate Type */
        public int GetC_ConversionType_ID() { Object ii = Get_Value("C_ConversionType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID) { if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory."); Set_Value("C_Currency_ID", C_Currency_ID); }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Type.
@param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID) { if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory."); Set_Value("C_DocType_ID", C_DocType_ID); }/** Get Document Type.
@return Document type or rules */
        public int GetC_DocType_ID() { Object ii = Get_Value("C_DocType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Inco Term.
@param C_IncoTerm_ID Inco term will be used to create or define the Inco term based on client requirement */
        public void SetC_IncoTerm_ID(int C_IncoTerm_ID)
        {
            if (C_IncoTerm_ID <= 0) Set_Value("C_IncoTerm_ID", null);
            else
                Set_Value("C_IncoTerm_ID", C_IncoTerm_ID);
        }/** Get Inco Term.
@return Inco term will be used to create or define the Inco term based on client requirement */
        public int GetC_IncoTerm_ID() { Object ii = Get_Value("C_IncoTerm_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetC_PaymentTerm_ID(int C_PaymentTerm_ID) { if (C_PaymentTerm_ID < 1) throw new ArgumentException("C_PaymentTerm_ID is mandatory."); Set_Value("C_PaymentTerm_ID", C_PaymentTerm_ID); }/** Get Payment Term.
@return The terms of Payment (timing, discount) */
        public int GetC_PaymentTerm_ID() { Object ii = Get_Value("C_PaymentTerm_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Project.
@param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }/** Get Project.
@return Business Opportunity */
        public int GetC_Project_ID() { Object ii = Get_Value("C_Project_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Provisional Invoice.
@param C_ProvisionalInvoice_ID Provisional Invoice */
        public void SetC_ProvisionalInvoice_ID(int C_ProvisionalInvoice_ID) { if (C_ProvisionalInvoice_ID < 1) throw new ArgumentException("C_ProvisionalInvoice_ID is mandatory."); Set_ValueNoCheck("C_ProvisionalInvoice_ID", C_ProvisionalInvoice_ID); }/** Get Provisional Invoice.
@return Provisional Invoice */
        public int GetC_ProvisionalInvoice_ID() { Object ii = Get_Value("C_ProvisionalInvoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Copy From.
@param CopyFrom Copy From Record */
        public void SetCopyFrom(String CopyFrom) { if (CopyFrom != null && CopyFrom.Length > 1) { log.Warning("Length > 1 - truncated"); CopyFrom = CopyFrom.Substring(0, 1); } Set_Value("CopyFrom", CopyFrom); }/** Get Copy From.
@return Copy From Record */
        public String GetCopyFrom() { return (String)Get_Value("CopyFrom"); }/** Set Create lines from.
@param CreateFrom Process which will generate a new document lines based on an existing document */
        public void SetCreateFrom(String CreateFrom) { if (CreateFrom != null && CreateFrom.Length > 1) { log.Warning("Length > 1 - truncated"); CreateFrom = CreateFrom.Substring(0, 1); } Set_Value("CreateFrom", CreateFrom); }/** Get Create lines from.
@return Process which will generate a new document lines based on an existing document */
        public String GetCreateFrom() { return (String)Get_Value("CreateFrom"); }/** Set Account Date.
@param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct) { Set_Value("DateAcct", (DateTime?)DateAcct); }/** Get Account Date.
@return General Ledger Date */
        public DateTime? GetDateAcct() { return (DateTime?)Get_Value("DateAcct"); }/** Set Date Invoiced.
@param DateInvoiced Date printed on Invoice */
        public void SetDateInvoiced(DateTime? DateInvoiced) { Set_Value("DateInvoiced", (DateTime?)DateInvoiced); }/** Get Date Invoiced.
@return Date printed on Invoice */
        public DateTime? GetDateInvoiced() { return (DateTime?)Get_Value("DateInvoiced"); }/** Set Date Ordered.
@param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered) { Set_Value("DateOrdered", (DateTime?)DateOrdered); }/** Get Date Ordered.
@return Date of Order */
        public DateTime? GetDateOrdered() { return (DateTime?)Get_Value("DateOrdered"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 500) { log.Warning("Length > 500 - truncated"); Description = Description.Substring(0, 500); } Set_Value("Description", Description); }/** Get Description.
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
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL"); if (DocAction != null && DocAction.Length > 2) { log.Warning("Length > 2 - truncated"); DocAction = DocAction.Substring(0, 2); }
            Set_Value("DocAction", DocAction);
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
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP"); if (DocStatus.Length > 2) { log.Warning("Length > 2 - truncated"); DocStatus = DocStatus.Substring(0, 2); }
            Set_Value("DocStatus", DocStatus);
        }/** Get Document Status.
@return The current status of the document */
        public String GetDocStatus() { return (String)Get_Value("DocStatus"); }/** Set Document No..
@param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo) { if (DocumentNo != null && DocumentNo.Length > 100) { log.Warning("Length > 100 - truncated"); DocumentNo = DocumentNo.Substring(0, 100); } Set_Value("DocumentNo", DocumentNo); }/** Get Document No..
@return Document sequence number of the document */
        public String GetDocumentNo() { return (String)Get_Value("DocumentNo"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Grand Total.
@param GrandTotal Total amount of document */
        public void SetGrandTotal(Decimal? GrandTotal) { Set_Value("GrandTotal", (Decimal?)GrandTotal); }/** Get Grand Total.
@return Total amount of document */
        public Decimal GetGrandTotal() { Object bd = Get_Value("GrandTotal"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Cost Calculated.
@param IsCostCalculated This checkbox will auto set "True", when the cost is calculated for the document. */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }/** Get Cost Calculated.
@return This checkbox will auto set "True", when the cost is calculated for the document. */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Paid.
@param IsPaid The document is paid */
        public void SetIsPaid(Boolean IsPaid) { Set_Value("IsPaid", IsPaid); }/** Get Paid.
@return The document is paid */
        public Boolean IsPaid() { Object oo = Get_Value("IsPaid"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Return Transaction.
@param IsReturnTrx This is a return transaction */
        public void SetIsReturnTrx(Boolean IsReturnTrx) { Set_Value("IsReturnTrx", IsReturnTrx); }/** Get Return Transaction.
@return This is a return transaction */
        public Boolean IsReturnTrx() { Object oo = Get_Value("IsReturnTrx"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Reversal.
@param IsReversal This is a reversing transaction */
        public void SetIsReversal(Boolean IsReversal) { Set_Value("IsReversal", IsReversal); }/** Get Reversal.
@return This is a reversing transaction */
        public Boolean IsReversal() { Object oo = Get_Value("IsReversal"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Reversed Cost Calculated.
@param IsReversedCostCalculated This checkbox will auto set "True", when the impact of cost is reversed for the document if any. */
        public void SetIsReversedCostCalculated(Boolean IsReversedCostCalculated) { Set_Value("IsReversedCostCalculated", IsReversedCostCalculated); }/** Get Reversed Cost Calculated.
@return This checkbox will auto set "True", when the impact of cost is reversed for the document if any. */
        public Boolean IsReversedCostCalculated() { Object oo = Get_Value("IsReversedCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx) { Set_Value("IsSOTrx", IsSOTrx); }/** Get Sales Transaction.
@return This is a Sales Transaction */
        public Boolean IsSOTrx() { Object oo = Get_Value("IsSOTrx"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID) { if (M_PriceList_ID < 1) throw new ArgumentException("M_PriceList_ID is mandatory."); Set_Value("M_PriceList_ID", M_PriceList_ID); }/** Get Price List.
@return Unique identifier of a Price List */
        public int GetM_PriceList_ID() { Object ii = Get_Value("M_PriceList_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Reference.
@param POReference Transaction Reference Number  of your Customer/Prospect */
        public void SetPOReference(String POReference) { if (POReference != null && POReference.Length > 100) { log.Warning("Length > 100 - truncated"); POReference = POReference.Substring(0, 100); } Set_Value("POReference", POReference); }/** Get Order Reference.
@return Transaction Reference Number  of your Customer/Prospect */
        public String GetPOReference() { return (String)Get_Value("POReference"); }/** Set Posted.
@param Posted Posting status */
        public void SetPosted(Boolean Posted) { Set_Value("Posted", Posted); }/** Get Posted.
@return Posting status */
        public Boolean IsPosted() { Object oo = Get_Value("Posted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing) { Set_Value("Processing", Processing); }/** Get Process Now.
@return Process Now */
        public Boolean IsProcessing() { Object oo = Get_Value("Processing"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** ReversalDoc_ID AD_Reference_ID=1000247 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 1000247;/** Set Reversal Document.
@param ReversalDoc_ID Reference of its original document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }/** Get Reversal Document.
@return Reference of its original document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;/** Set Sales Rep.
@param SalesRep_ID Company Agent like Sales Representative, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }/** Get Sales Rep.
@return Company Agent like Sales Representative, Customer Service Representative, ... */
        public int GetSalesRep_ID() { Object ii = Get_Value("SalesRep_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set SubTotal.
@param TotalLines Total of all document lines (excluding Tax) */
        public void SetTotalLines(Decimal? TotalLines) { Set_Value("TotalLines", (Decimal?)TotalLines); }/** Get SubTotal.
@return Total of all document lines (excluding Tax) */
        public Decimal GetTotalLines() { Object bd = Get_Value("TotalLines"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}