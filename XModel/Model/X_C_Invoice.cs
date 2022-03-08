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
    /** Generated Model for C_Invoice
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Invoice : PO
    {
        public X_C_Invoice(Context ctx, int C_Invoice_ID, Trx trxName)
            : base(ctx, C_Invoice_ID, trxName)
        {
            /** if (C_Invoice_ID == 0)
            {
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_DocTypeTarget_ID (0);
            SetC_DocType_ID (0);	// 0
            SetC_Invoice_ID (0);
            SetC_PaymentTerm_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDateInvoiced (DateTime.Now);	// @#Date@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetGrandTotal (0.0);
            SetIsApproved (false);	// @IsApproved@
            SetIsDiscountPrinted (false);
            SetIsInDispute (false);	// N
            SetIsPaid (false);
            SetIsPayScheduleValid (false);
            SetIsPrinted (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);	// @IsSOTrx@
            SetIsSelfService (false);
            SetIsTaxIncluded (false);
            SetIsTransferred (false);
            SetM_PriceList_ID (0);
            SetPaymentRule (null);	// P
            SetPosted (false);	// N
            SetProcessed (false);	// N
            SetSendEMail (false);
            SetTotalLines (0.0);
            }
             */
        }
        public X_C_Invoice(Ctx ctx, int C_Invoice_ID, Trx trxName)
            : base(ctx, C_Invoice_ID, trxName)
        {
            /** if (C_Invoice_ID == 0)
            {
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_DocTypeTarget_ID (0);
            SetC_DocType_ID (0);	// 0
            SetC_Invoice_ID (0);
            SetC_PaymentTerm_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDateInvoiced (DateTime.Now);	// @#Date@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetGrandTotal (0.0);
            SetIsApproved (false);	// @IsApproved@
            SetIsDiscountPrinted (false);
            SetIsInDispute (false);	// N
            SetIsPaid (false);
            SetIsPayScheduleValid (false);
            SetIsPrinted (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);	// @IsSOTrx@
            SetIsSelfService (false);
            SetIsTaxIncluded (false);
            SetIsTransferred (false);
            SetM_PriceList_ID (0);
            SetPaymentRule (null);	// P
            SetPosted (false);	// N
            SetProcessed (false);	// N
            SetSendEMail (false);
            SetTotalLines (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Invoice(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Invoice(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Invoice(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_Invoice()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514372266L;
        /** Last Updated Timestamp 7/29/2010 1:07:35 PM */
        public static long updatedMS = 1280389055477L;
        /** AD_Table_ID=318 */
        public static int Table_ID;
        // =318;

        /** TableName=C_Invoice */
        public static String Table_Name = "C_Invoice";

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
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_C_Invoice[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
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
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
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
        public void SetC_Activity_ID(int C_Activity_ID)
        {
            if (C_Activity_ID <= 0) Set_Value("C_Activity_ID", null);
            else
                Set_Value("C_Activity_ID", C_Activity_ID);
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
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory.");
            Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Location.
        @param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID < 1) throw new ArgumentException("C_BPartner_Location_ID is mandatory.");
            Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetC_Campaign_ID()
        {
            Object ii = Get_Value("C_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Journal Line.
        @param C_CashLine_ID Cash Journal Line */
        public void SetC_CashLine_ID(int C_CashLine_ID)
        {
            if (C_CashLine_ID <= 0) Set_Value("C_CashLine_ID", null);
            else
                Set_Value("C_CashLine_ID", C_CashLine_ID);
        }
        /** Get Cash Journal Line.
        @return Cash Journal Line */
        public int GetC_CashLine_ID()
        {
            Object ii = Get_Value("C_CashLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_Charge_ID AD_Reference_ID=200 */
        public static int C_CHARGE_ID_AD_Reference_ID = 200;
        /** Set Charge.
        @param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetC_Charge_ID()
        {
            Object ii = Get_Value("C_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency Type.
        @param C_ConversionType_ID Currency Conversion Rate Type */
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            if (C_ConversionType_ID <= 0) Set_Value("C_ConversionType_ID", null);
            else
                Set_Value("C_ConversionType_ID", C_ConversionType_ID);
        }
        /** Get Currency Type.
        @return Currency Conversion Rate Type */
        public int GetC_ConversionType_ID()
        {
            Object ii = Get_Value("C_ConversionType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_DocTypeTarget_ID AD_Reference_ID=170 */
        public static int C_DOCTYPETARGET_ID_AD_Reference_ID = 170;
        /** Set Target Doc Type.
        @param C_DocTypeTarget_ID Target document type for documents */
        public void SetC_DocTypeTarget_ID(int C_DocTypeTarget_ID)
        {
            if (C_DocTypeTarget_ID < 1) throw new ArgumentException("C_DocTypeTarget_ID is mandatory.");
            Set_Value("C_DocTypeTarget_ID", C_DocTypeTarget_ID);
        }
        /** Get Target Doc Type.
        @return Target document type for documents */
        public int GetC_DocTypeTarget_ID()
        {
            Object ii = Get_Value("C_DocTypeTarget_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
            Set_ValueNoCheck("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID < 1) throw new ArgumentException("C_Invoice_ID is mandatory.");
            Set_ValueNoCheck("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param C_Order_ID Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_ValueNoCheck("C_Order_ID", null);
            else
                Set_ValueNoCheck("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Term.
        @param C_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetC_PaymentTerm_ID(int C_PaymentTerm_ID)
        {
            if (C_PaymentTerm_ID < 1) throw new ArgumentException("C_PaymentTerm_ID is mandatory.");
            Set_Value("C_PaymentTerm_ID", C_PaymentTerm_ID);
        }
        /** Get Payment Term.
        @return The terms of Payment (timing, discount) */
        public int GetC_PaymentTerm_ID()
        {
            Object ii = Get_Value("C_PaymentTerm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment.
        @param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            if (C_Payment_ID <= 0) Set_Value("C_Payment_ID", null);
            else
                Set_Value("C_Payment_ID", C_Payment_ID);
        }
        /** Get Payment.
        @return Payment identifier */
        public int GetC_Payment_ID()
        {
            Object ii = Get_Value("C_Payment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project.
        @param C_Project_ID Financial Project */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge amount.
        @param ChargeAmt Charge Amount */
        public void SetChargeAmt(Decimal? ChargeAmt)
        {
            Set_Value("ChargeAmt", (Decimal?)ChargeAmt);
        }
        /** Get Charge amount.
        @return Charge Amount */
        public Decimal GetChargeAmt()
        {
            Object bd = Get_Value("ChargeAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Copy From.
        @param CopyFrom Copy From Record */
        public void SetCopyFrom(String CopyFrom)
        {
            if (CopyFrom != null && CopyFrom.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CopyFrom = CopyFrom.Substring(0, 1);
            }
            Set_Value("CopyFrom", CopyFrom);
        }
        /** Get Copy From.
        @return Copy From Record */
        public String GetCopyFrom()
        {
            return (String)Get_Value("CopyFrom");
        }
        /** Set Create lines from.
        @param CreateFrom Process which will generate a new document lines based on an existing document */
        public void SetCreateFrom(String CreateFrom)
        {
            if (CreateFrom != null && CreateFrom.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreateFrom = CreateFrom.Substring(0, 1);
            }
            Set_Value("CreateFrom", CreateFrom);
        }
        /** Get Create lines from.
        @return Process which will generate a new document lines based on an existing document */
        public String GetCreateFrom()
        {
            return (String)Get_Value("CreateFrom");
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory.");
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Date Invoiced.
        @param DateInvoiced Date printed on Invoice */
        public void SetDateInvoiced(DateTime? DateInvoiced)
        {
            if (DateInvoiced == null) throw new ArgumentException("DateInvoiced is mandatory.");
            Set_Value("DateInvoiced", (DateTime?)DateInvoiced);
        }
        /** Get Date Invoiced.
        @return Date printed on Invoice */
        public DateTime? GetDateInvoiced()
        {
            return (DateTime?)Get_Value("DateInvoiced");
        }
        /** Set Date Ordered.
        @param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered)
        {
            Set_ValueNoCheck("DateOrdered", (DateTime?)DateOrdered);
        }
        /** Get Date Ordered.
        @return Date of Order */
        public DateTime? GetDateOrdered()
        {
            return (DateTime?)Get_Value("DateOrdered");
        }
        /** Set Date printed.
        @param DatePrinted Date the document was printed. */
        public void SetDatePrinted(DateTime? DatePrinted)
        {
            Set_Value("DatePrinted", (DateTime?)DatePrinted);
        }
        /** Get Date printed.
        @return Date the document was printed. */
        public DateTime? GetDatePrinted()
        {
            return (DateTime?)Get_Value("DatePrinted");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }

        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;
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
        public bool IsDocActionValid(String test)
        {
            return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
        }
        /** Set Document Action.
        @param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (DocAction == null) throw new ArgumentException("DocAction is mandatory");
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
            if (DocAction.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocAction = DocAction.Substring(0, 2);
            }
            Set_Value("DocAction", DocAction);
        }
        /** Get Document Action.
        @return The targeted status of the document */
        public String GetDocAction()
        {
            return (String)Get_Value("DocAction");
        }

        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;
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
        public bool IsDocStatusValid(String test)
        {
            return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory");
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocStatus = DocStatus.Substring(0, 2);
            }
            Set_Value("DocStatus", DocStatus);
        }
        /** Get Document Status.
        @return The current status of the document */
        public String GetDocStatus()
        {
            return (String)Get_Value("DocStatus");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo == null) throw new ArgumentException("DocumentNo is mandatory.");
            if (DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_ValueNoCheck("DocumentNo", DocumentNo);
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

        /** Adhoc Payment - Adding a new column (DueDate) for Payment Term ** Dt: 18/01/2021 ** Modified By: Kumar **/
        /** Set Due Date.
        @param DueDate Date when the payment is due */
        public void SetDueDate(DateTime? DueDate)
        {
            Set_Value("DueDate", (DateTime?)DueDate);
        }
        /** Get Due Date.
        @return Date when the payment is due */
        public DateTime? GetDueDate()
        {
            return (DateTime?)Get_Value("DueDate");
        }
        /** Set Generate To.
        @param GenerateTo Generate To */
        public void SetGenerateTo(String GenerateTo)
        {
            if (GenerateTo != null && GenerateTo.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                GenerateTo = GenerateTo.Substring(0, 1);
            }
            Set_Value("GenerateTo", GenerateTo);
        }
        /** Get Generate To.
        @return Generate To */
        public String GetGenerateTo()
        {
            return (String)Get_Value("GenerateTo");
        }
        /** Set Grand Total.
        @param GrandTotal Total amount of document */
        public void SetGrandTotal(Decimal? GrandTotal)
        {
            if (GrandTotal == null) throw new ArgumentException("GrandTotal is mandatory.");
            Set_ValueNoCheck("GrandTotal", (Decimal?)GrandTotal);
        }
        /** Get Grand Total.
        @return Total amount of document */
        public Decimal GetGrandTotal()
        {
            Object bd = Get_Value("GrandTotal");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Get ED007_DiscountPercent.
                @return ED007_DiscountPercent */
        public Decimal GetED007_DiscountPercent()
        {
            Object bd = Get_Value("ED007_DiscountPercent");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** InvoiceCollectionType AD_Reference_ID=394 */
        public static int INVOICECOLLECTIONTYPE_AD_Reference_ID = 394;
        /** Collection Agency = C */
        public static String INVOICECOLLECTIONTYPE_CollectionAgency = "C";
        /** Dunning = D */
        public static String INVOICECOLLECTIONTYPE_Dunning = "D";
        /** Legal Procedure = L */
        public static String INVOICECOLLECTIONTYPE_LegalProcedure = "L";
        /** Uncollectable = U */
        public static String INVOICECOLLECTIONTYPE_Uncollectable = "U";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsInvoiceCollectionTypeValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("D") || test.Equals("L") || test.Equals("U");
        }
        /** Set Collection Status.
        @param InvoiceCollectionType Invoice Collection Status */
        public void SetInvoiceCollectionType(String InvoiceCollectionType)
        {
            if (!IsInvoiceCollectionTypeValid(InvoiceCollectionType))
                throw new ArgumentException("InvoiceCollectionType Invalid value - " + InvoiceCollectionType + " - Reference_ID=394 - C - D - L - U");
            if (InvoiceCollectionType != null && InvoiceCollectionType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                InvoiceCollectionType = InvoiceCollectionType.Substring(0, 1);
            }
            Set_Value("InvoiceCollectionType", InvoiceCollectionType);
        }
        /** Get Collection Status.
        @return Invoice Collection Status */
        public String GetInvoiceCollectionType()
        {
            return (String)Get_Value("InvoiceCollectionType");
        }
        /** Set Approved.
        @param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved)
        {
            Set_ValueNoCheck("IsApproved", IsApproved);
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
        /** Set Discount Printed.
        @param IsDiscountPrinted Print Discount on Invoice and Order */
        public void SetIsDiscountPrinted(Boolean IsDiscountPrinted)
        {
            Set_Value("IsDiscountPrinted", IsDiscountPrinted);
        }
        /** Get Discount Printed.
        @return Print Discount on Invoice and Order */
        public Boolean IsDiscountPrinted()
        {
            Object oo = Get_Value("IsDiscountPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set In Dispute.
        @param IsInDispute Document is in dispute */
        public void SetIsInDispute(Boolean IsInDispute)
        {
            Set_Value("IsInDispute", IsInDispute);
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
        /** Set Paid.
        @param IsPaid The document is paid */
        public void SetIsPaid(Boolean IsPaid)
        {
            Set_Value("IsPaid", IsPaid);
        }
        /** Get Paid.
        @return The document is paid */
        public Boolean IsPaid()
        {
            Object oo = Get_Value("IsPaid");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Pay Schedule valid.
        @param IsPayScheduleValid Is the Payment Schedule is valid */
        public void SetIsPayScheduleValid(Boolean IsPayScheduleValid)
        {
            Set_ValueNoCheck("IsPayScheduleValid", IsPayScheduleValid);
        }
        /** Get Pay Schedule valid.
        @return Is the Payment Schedule is valid */
        public Boolean IsPayScheduleValid()
        {
            Object oo = Get_Value("IsPayScheduleValid");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Printed.
        @param IsPrinted Indicates if this document / line is printed */
        public void SetIsPrinted(Boolean IsPrinted)
        {
            Set_ValueNoCheck("IsPrinted", IsPrinted);
        }
        /** Get Printed.
        @return Indicates if this document / line is printed */
        public Boolean IsPrinted()
        {
            Object oo = Get_Value("IsPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Return Transaction.
        @param IsReturnTrx This is a return transaction */
        public void SetIsReturnTrx(Boolean IsReturnTrx)
        {
            Set_Value("IsReturnTrx", IsReturnTrx);
        }
        /** Get Return Transaction.
        @return This is a return transaction */
        public Boolean IsReturnTrx()
        {
            Object oo = Get_Value("IsReturnTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sales Transaction.
        @param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx)
        {
            Set_ValueNoCheck("IsSOTrx", IsSOTrx);
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
        /** Set Self-Service.
        @param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
        public void SetIsSelfService(Boolean IsSelfService)
        {
            Set_Value("IsSelfService", IsSelfService);
        }
        /** Get Self-Service.
        @return This is a Self-Service entry or this entry can be changed via Self-Service */
        public Boolean IsSelfService()
        {
            Object oo = Get_Value("IsSelfService");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Price includes Tax.
        @param IsTaxIncluded Tax is included in the price */
        public void SetIsTaxIncluded(Boolean IsTaxIncluded)
        {
            Set_Value("IsTaxIncluded", IsTaxIncluded);
        }
        /** Get Price includes Tax.
        @return Tax is included in the price */
        public Boolean IsTaxIncluded()
        {
            Object oo = Get_Value("IsTaxIncluded");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Transferred.
        @param IsTransferred Transferred to General Ledger (i.e. accounted) */
        public void SetIsTransferred(Boolean IsTransferred)
        {
            Set_ValueNoCheck("IsTransferred", IsTransferred);
        }
        /** Get Transferred.
        @return Transferred to General Ledger (i.e. accounted) */
        public Boolean IsTransferred()
        {
            Object oo = Get_Value("IsTransferred");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Price List.
        @param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID < 1) throw new ArgumentException("M_PriceList_ID is mandatory.");
            Set_Value("M_PriceList_ID", M_PriceList_ID);
        }
        /** Get Price List.
        @return Unique identifier of a Price List */
        public int GetM_PriceList_ID()
        {
            Object ii = Get_Value("M_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** MatchRequirementI AD_Reference_ID=360 */
        public static int MATCHREQUIREMENTI_AD_Reference_ID = 360;
        /** Purchase Order and Receipt = B */
        public static String MATCHREQUIREMENTI_PurchaseOrderAndReceipt = "B";
        /** None = N */
        public static String MATCHREQUIREMENTI_None = "N";
        /** Purchase Order = P */
        public static String MATCHREQUIREMENTI_PurchaseOrder = "P";
        /** Receipt = R */
        public static String MATCHREQUIREMENTI_Receipt = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMatchRequirementIValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("N") || test.Equals("P") || test.Equals("R");
        }
        /** Set Invoice Match Requirement.
        @param MatchRequirementI Matching Requirement for Invoice */
        public void SetMatchRequirementI(String MatchRequirementI)
        {
            if (!IsMatchRequirementIValid(MatchRequirementI))
                throw new ArgumentException("MatchRequirementI Invalid value - " + MatchRequirementI + " - Reference_ID=360 - B - N - P - R");
            if (MatchRequirementI != null && MatchRequirementI.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MatchRequirementI = MatchRequirementI.Substring(0, 1);
            }
            Set_Value("MatchRequirementI", MatchRequirementI);
        }
        /** Get Invoice Match Requirement.
        @return Matching Requirement for Invoice */
        public String GetMatchRequirementI()
        {
            return (String)Get_Value("MatchRequirementI");
        }
        /** Set Order Reference.
        @param POReference Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public void SetPOReference(String POReference)
        {
            if (POReference != null && POReference.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                POReference = POReference.Substring(0, 20);
            }
            Set_Value("POReference", POReference);
        }
        /** Get Order Reference.
        @return Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public String GetPOReference()
        {
            return (String)Get_Value("POReference");
        }

        /** PaymentRule AD_Reference_ID=195 */
        public static int PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        public static String PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        public static String PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        public static String PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        public static String PAYMENTRULE_DirectDeposit = "T";
        /** Letter of Credit = L */
        public static String PAYMENTRULE_LetterOfCredit = "L";
        /** Cash + Credit */
        public static String PAYMENTRULE_CashAndCredit = "C";
        /** Wire Transfer = W */
        public static String PAYMENTRULE_WireTransfer = "W";

        public static String PAYMENTRULE_ThirdPartyPayment = "O"; // Third Party Payment = O

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPaymentRuleValid(String test)
        {
            return test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("L") || test.Equals("P") || test.Equals("S") || test.Equals("T") || test.Equals("C")
                || test.Equals("W") || test.Equals("O");
        }
        /** Set Payment Method.
        @param PaymentRule How you pay the invoice */
        public void SetPaymentRule(String PaymentRule)
        {
            if (PaymentRule == null) throw new ArgumentException("PaymentRule is mandatory");
            if (!IsPaymentRuleValid(PaymentRule))
                throw new ArgumentException("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - L - P - S - T - W -O");
            if (PaymentRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PaymentRule = PaymentRule.Substring(0, 1);
            }
            Set_Value("PaymentRule", PaymentRule);
        }
        /** Get Payment Method.
        @return How you pay the invoice */
        public String GetPaymentRule()
        {
            return (String)Get_Value("PaymentRule");
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_Value("Posted", Posted);
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
        public void SetProcessed(Boolean Processed)
        {
            Set_ValueNoCheck("Processed", Processed);
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
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
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
        /** Set Referenced Invoice.
        @param Ref_Invoice_ID Referenced Invoice */
        public void SetRef_Invoice_ID(int Ref_Invoice_ID)
        {
            if (Ref_Invoice_ID <= 0) Set_Value("Ref_Invoice_ID", null);
            else
                Set_Value("Ref_Invoice_ID", Ref_Invoice_ID);
        }
        /** Get Referenced Invoice.
        @return Referenced Invoice */
        public int GetRef_Invoice_ID()
        {
            Object ii = Get_Value("Ref_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
        /** Set Representative.
        @param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Representative.
        @return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Send EMail.
        @param SendEMail Enable sending Document EMail */
        public void SetSendEMail(Boolean SendEMail)
        {
            Set_Value("SendEMail", SendEMail);
        }
        /** Get Send EMail.
        @return Enable sending Document EMail */
        public Boolean IsSendEMail()
        {
            Object oo = Get_Value("SendEMail");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set SubTotal.
        @param TotalLines Total of all document lines (excluding Tax) */
        public void SetTotalLines(Decimal? TotalLines)
        {
            if (TotalLines == null) throw new ArgumentException("TotalLines is mandatory.");
            Set_ValueNoCheck("TotalLines", (Decimal?)TotalLines);
        }
        /** Get SubTotal.
        @return Total of all document lines (excluding Tax) */
        public Decimal GetTotalLines()
        {
            Object bd = Get_Value("TotalLines");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** User1_ID AD_Reference_ID=134 */
        public static int USER1_ID_AD_Reference_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
        }
        /** Get User List 1.
        @return User defined list element #1 */
        public int GetUser1_ID()
        {
            Object ii = Get_Value("User1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User2_ID AD_Reference_ID=137 */
        public static int USER2_ID_AD_Reference_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Contract.
        @param C_Contract_ID identifier */
        public void SetC_Contract_ID(int C_Contract_ID)
        {
            if (C_Contract_ID <= 0) Set_Value("C_Contract_ID", null);
            else
                Set_Value("C_Contract_ID", C_Contract_ID);
        }
        /** Get Contract.
        @return Contract identifier */
        public int GetC_Contract_ID()
        {
            Object ii = Get_Value("C_Contract_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Paid From POS.
        @param VAPOS_IsPaid Paid From POS */
        public void SetVAPOS_IsPaid(Boolean VAPOS_IsPaid)
        {
            Set_Value("VAPOS_IsPaid", VAPOS_IsPaid);
        }
        /** Get Paid From POS.
        @return Paid From POS */
        public Boolean IsVAPOS_IsPaid()
        {
            Object oo = Get_Value("VAPOS_IsPaid");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Payment Method.
@param VA009_PaymentMethod_ID Payment Method */
        public void SetVA009_PaymentMethod_ID(int VA009_PaymentMethod_ID)
        {
            if (VA009_PaymentMethod_ID <= 0) Set_Value("VA009_PaymentMethod_ID", null);
            else
                Set_Value("VA009_PaymentMethod_ID", VA009_PaymentMethod_ID);
        }
        /** Get Payment Method.
        @return Payment Method */
        public int GetVA009_PaymentMethod_ID()
        {
            Object ii = Get_Value("VA009_PaymentMethod_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PaymentMethod AD_Reference_ID=195 */
        public static int PAYMENTMETHOD_AD_Reference_ID = 195;/** Cash = B */
        public static String PAYMENTMETHOD_Cash = "B";/** Cash+Card = C */
        public static String PAYMENTMETHOD_CashPlusCard = "C";/** Direct Debit = D */
        public static String PAYMENTMETHOD_DirectDebit = "D";/** Credit Card = K */
        public static String PAYMENTMETHOD_CreditCard = "K";/** Letter of Credit = L */
        public static String PAYMENTMETHOD_LetterOfCredit = "L";/** On Credit = P */
        public static String PAYMENTMETHOD_OnCredit = "P";/** Check = S */
        public static String PAYMENTMETHOD_Check = "S";/** Direct Deposit = T */
        public static String PAYMENTMETHOD_DirectDeposit = "T";/** Wire Transfer = W */
        public static String PAYMENTMETHOD_WireTransfer = "W";/** Is test a valid value. */
        public static String PAYMENTMETHOD_ThirdPartyPayment = "O";/** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPaymentMethodValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("K") || test.Equals("L") || test.Equals("P") || test.Equals("S") || test.Equals("T")
                || test.Equals("W") || test.Equals("O");
        }
        /** Set Payment Method.
        @param PaymentMethod Payment Method */
        public void SetPaymentMethod(String PaymentMethod)
        {
            if (!IsPaymentMethodValid(PaymentMethod))
                throw new ArgumentException("PaymentMethod Invalid value - " + PaymentMethod + " - Reference_ID=195 - B - C - D - K - L - P - S - T - W -O"); if (PaymentMethod != null && PaymentMethod.Length > 1) { log.Warning("Length > 1 - truncated"); PaymentMethod = PaymentMethod.Substring(0, 1); }
            Set_Value("PaymentMethod", PaymentMethod);
        }
        /** Get Payment Method.
        @return Payment Method */
        public String GetPaymentMethod()
        {
            return (String)Get_Value("PaymentMethod");
        }


        //Added By Amit for VA009
        /** Set Open Amount .
        @param VA009_OpenAmount Open Amount */
        public void SetVA009_OpenAmount(Decimal? VA009_OpenAmount)
        {
            Set_Value("VA009_OpenAmount", (Decimal?)VA009_OpenAmount);
        }
        /** Get Open Amount (Base).
        @return Open Amount (Base) */
        public Decimal GetVA009_OpenAmount()
        {
            Object bd = Get_Value("VA009_OpenAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Paid Amount .
       @param VA009_PaidAmount Paid Amount */
        public void SetVA009_PaidAmount(Decimal? VA009_PaidAmount)
        {
            Set_Value("VA009_PaidAmount", (Decimal?)VA009_PaidAmount);
        }
        /** Get Paid Amount .
        @return Paid Amount */
        public Decimal GetVA009_PaidAmount()
        {
            Object bd = Get_Value("VA009_PaidAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set/Get Expense Invoice. Added by arpit asked by surya Sir**************29-12-2015
        @param IsExpenseInvoice Expense Invoice */
        public void SetIsExpenseInvoice(Boolean IsExpenseInvoice)
        {
            Set_Value("IsExpenseInvoice", IsExpenseInvoice);
        }
        /** Get Expense Invoice.
        @return Expense Invoice */
        public Boolean IsExpenseInvoice()
        {
            Object oo = Get_Value("IsExpenseInvoice");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        //***************end here**************
        //end

        /** Set Cost Calculated.@param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }
        /** Get Cost Calculated. @return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Reversed Cost Calculated.@param IsReversedCostCalculated Reversed Cost Calculated */
        public void SetIsReversedCostCalculated(Boolean IsReversedCostCalculated) { Set_Value("IsReversedCostCalculated", IsReversedCostCalculated); }
        /** Get Reversed Cost Calculated. @return Reversed Cost Calculated */
        public Boolean IsReversedCostCalculated() { Object oo = Get_Value("IsReversedCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set LC Detail.@param VA026_LCDetail_ID LC Detail  */
        public void SetVA026_LCDetail_ID(int VA026_LCDetail_ID) { if (VA026_LCDetail_ID <= 0) Set_Value("VA026_LCDetail_ID", null); else Set_Value("VA026_LCDetail_ID", VA026_LCDetail_ID); }
        /** Get LC Detail.@return LC Detail  */
        public int GetVA026_LCDetail_ID() { Object ii = Get_Value("VA026_LCDetail_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Future Cost Calculated.@param IsFutureCostCalculated Future Cost Calculated */
        public void SetIsFutureCostCalculated(Boolean IsFutureCostCalculated) { Set_Value("IsFutureCostCalculated", IsFutureCostCalculated); }
        /** Get Future Cost Calculated.@return Future Cost Calculated */
        public Boolean IsFutureCostCalculated() { Object oo = Get_Value("IsFutureCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }


        //Added by Bharat
        /** Set Discrepancy Amount .
        @param Discrepancy Amount */
        public void SetDiscrepancyAmt(Decimal? DiscrepancyAmt)
        {
            Set_Value("DiscrepancyAmt", (Decimal?)DiscrepancyAmt);
        }
        /** Get Discrepancy Amount (Base).
        @return Discrepancy Amount (Base) */
        public Decimal GetDiscrepancyAmt()
        {
            Object bd = Get_Value("DiscrepancyAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Discrepancy Adjusted .
        @param Discrepancy Adjusted */
        public void SetDiscrepancyAdjusted(Decimal? DiscrepancyAdjusted)
        {
            Set_Value("DiscrepancyAdjusted", (Decimal?)DiscrepancyAdjusted);
        }
        /** Get Discrepancy Adjusted (Base).
        @return Discrepancy Adjusted (Base) */
        public Decimal GetDiscrepancyAdjusted()
        {
            Object bd = Get_Value("DiscrepancyAdjusted");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Ref_C_Invoice_ID AD_Reference_ID=1000238 */
        public static int REF_C_INVOICE_ID_AD_Reference_ID = 1000238;
        /** Set Invoice Ref.
        @param Ref_C_Invoice_ID Original reference of reversed invoice document */
        public void SetRef_C_Invoice_ID(int Ref_C_Invoice_ID)
        {
            if (Ref_C_Invoice_ID <= 0) Set_Value("Ref_C_Invoice_ID", null);
            else
                Set_Value("Ref_C_Invoice_ID", Ref_C_Invoice_ID);
        }
        /** Get Invoice Ref.
        @return Original reference of reversed invoice document */
        public int GetRef_C_Invoice_ID()
        {
            Object ii = Get_Value("Ref_C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip)
        {
            Set_Value("IsDropShip", IsDropShip);
        }
        /** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip()
        {
            Object oo = Get_Value("IsDropShip");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool))
                    return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Inco Term.
        @param C_IncoTerm_ID Inco term will be used to create or define the Inco term based on client requirement */
        public void SetC_IncoTerm_ID(int C_IncoTerm_ID)
        {
            if (C_IncoTerm_ID <= 0)
                Set_Value("C_IncoTerm_ID", null);
            else
                Set_Value("C_IncoTerm_ID", C_IncoTerm_ID);
        }
        /** Get Inco Term.
        @return Inco term will be used to create or define the Inco term based on client requirement */
        public int GetC_IncoTerm_ID()
        {
            Object ii = Get_Value("C_IncoTerm_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }

        /// <summary>
        /// Set Hold Payment.
        /// </summary>
        /// <param name="IsHoldPayment">This checkbox used to hold the payments. If this checkbox true on the Order, it will set hold payment checkbox true at Invoice window while creating invoice against this order and user can’t do the payment for that invoice.</param>
        public void SetIsHoldPayment(Boolean IsHoldPayment) { Set_Value("IsHoldPayment", IsHoldPayment); }

        /// <summary>
        /// Get Hold Payment.
        /// </summary>
        /// <returns>This checkbox used to hold the payments. If this checkbox true on the Order, it will set hold payment checkbox true at Invoice window while creating invoice against this order and user can't do the payment for that invoice.</returns>
        public Boolean IsHoldPayment() { Object oo = Get_Value("IsHoldPayment"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /// <summary>
        /// Set Treat As Discount.
        /// </summary>
        /// <param name="TreatAsDiscount">This checkbox indicates if an invoice is considered as discount invoice while calculating product costing. Also the system will not allow to create return to vendor invoice.</param>
        public void SetTreatAsDiscount(Boolean TreatAsDiscount) { Set_Value("TreatAsDiscount", TreatAsDiscount); }
        /// <summary>
        ///  Get Treat As Discount.
        /// </summary>
        /// <returns>This checkbox indicates if an invoice is considered as discount invoice while calculating product costing. Also the system will not allow to create return to vendor invoice.</returns>
        public Boolean IsTreatAsDiscount() { Object oo = Get_Value("TreatAsDiscount"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Temp Document No.
        @param TempDocumentNo Temp Document No for this Document */
        public void SetTempDocumentNo(String TempDocumentNo)
        {
            if (TempDocumentNo != null && TempDocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                TempDocumentNo = TempDocumentNo.Substring(0, 30);
            }
            Set_Value("TempDocumentNo", TempDocumentNo);
        }

        /** Get Temp Document No.
        @return Temp Document No for this Document */
        public String GetTempDocumentNo()
        {
            return (String)Get_Value("TempDocumentNo");
        }

        /// <summary>
        /// Set Withholding Tax.
        /// </summary>
        /// <param name="C_Withholding_ID">Withholding type defined</param>
        public void SetC_Withholding_ID(int C_Withholding_ID)
        {
            if (C_Withholding_ID <= 0) Set_Value("C_Withholding_ID", null);
            else
                Set_Value("C_Withholding_ID", C_Withholding_ID);
        }
        /// <summary>
        /// Get Withholding Tax.
        /// </summary>
        /// <returns>Withholding type defined</returns>
        public int GetC_Withholding_ID() { Object ii = Get_Value("C_Withholding_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /// <summary>
        /// Set Withholding Amt.
        /// </summary>
        /// <param name="WithholdingAmt">Withholding Amt </param>
        public void SetWithholdingAmt(Decimal? WithholdingAmt) { Set_Value("WithholdingAmt", (Decimal?)WithholdingAmt); }

        /// <summary>
        /// Get Withholding Amt.
        /// </summary>
        /// <returns>Withholding Amt</returns>
        public Decimal GetWithholdingAmt() { Object bd = Get_Value("WithholdingAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Grand Total after Withholding.
@param GrandTotalAfterWithholding This field represents the total amount after deduction of withholding amount. */
        public void SetGrandTotalAfterWithholding(Decimal? GrandTotalAfterWithholding) { Set_Value("GrandTotalAfterWithholding", (Decimal?)GrandTotalAfterWithholding); }/** Get Grand Total after Withholding.
@return This field represents the total amount after deduction of withholding amount. */
        public Decimal GetGrandTotalAfterWithholding() { Object bd = Get_Value("GrandTotalAfterWithholding"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Backup Withholding Amount.
@param BackupWithholdingAmount Backup Withholding Amount */
        public void SetBackupWithholdingAmount(Decimal? BackupWithholdingAmount) { Set_Value("BackupWithholdingAmount", (Decimal?)BackupWithholdingAmount); }/** Get Backup Withholding Amount.
@return Backup Withholding Amount */
        public Decimal GetBackupWithholdingAmount() { Object bd = Get_Value("BackupWithholdingAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Service Contract.
@param ServiceContract This Checkbox is true when the Invoice is generated through the Service Contract. */
        public void SetServiceContract(Boolean ServiceContract) { Set_Value("ServiceContract", ServiceContract); }/** Get Service Contract.
@return This Checkbox is true when the Invoice is generated through the Service Contract. */
        public Boolean IsServiceContract() { Object oo = Get_Value("ServiceContract"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** ConditionalFlag AD_Reference_ID=1000246 */
        public static int CONDITIONALFLAG_AD_Reference_ID = 1000246;/** Reversal = 00 */
        public static String CONDITIONALFLAG_Reversal = "00";/** PrepareIt = 01 */
        public static String CONDITIONALFLAG_PrepareIt = "01";/** CompleteIt = 02 */
        public static String CONDITIONALFLAG_CompleteIt = "02";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsConditionalFlagValid(String test) { return test == null || test.Equals("00") || test.Equals("01") || test.Equals("02"); }/** Set Conditional Flag.
@param ConditionalFlag This Field is used to bypass constraint on different action */
        public void SetConditionalFlag(String ConditionalFlag)
        {
            if (!IsConditionalFlagValid(ConditionalFlag))
                throw new ArgumentException("ConditionalFlag Invalid value - " + ConditionalFlag + " - Reference_ID=1000246 - 00 - 01 - 02"); if (ConditionalFlag != null && ConditionalFlag.Length > 2) { log.Warning("Length > 2 - truncated"); ConditionalFlag = ConditionalFlag.Substring(0, 2); }
            Set_Value("ConditionalFlag", ConditionalFlag);
        }/** Get Conditional Flag.
@return This Field is used to bypass constraint on different action */
        public String GetConditionalFlag() { return (String)Get_Value("ConditionalFlag"); }

        /** ReversalDoc_ID AD_Reference_ID=336 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 336;/** Set Reversal Document.
@param ReversalDoc_ID Reference of its original document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }/** Get Reversal Document.
@return Reference of its original document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Warehouse.@param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID <= 0) Set_Value("M_Warehouse_ID", null);
            else
                Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.@return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID() { Object ii = Get_Value("M_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

    }

}
