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
    /** Generated Model for C_InvoicePaySchedule
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_InvoicePaySchedule : PO
    {
        public X_C_InvoicePaySchedule(Context ctx, int C_InvoicePaySchedule_ID, Trx trxName)
            : base(ctx, C_InvoicePaySchedule_ID, trxName)
        {
            /** if (C_InvoicePaySchedule_ID == 0)
            {
            SetC_InvoicePaySchedule_ID (0);
            SetC_Invoice_ID (0);
            SetDiscountAmt (0.0);
            SetDiscountDate (DateTime.Now);
            SetDueAmt (0.0);
            SetDueDate (DateTime.Now);
            SetIsValid (false);
            SetProcessed (false);	// N
            }
             */
        }
        public X_C_InvoicePaySchedule(Ctx ctx, int C_InvoicePaySchedule_ID, Trx trxName)
            : base(ctx, C_InvoicePaySchedule_ID, trxName)
        {
            /** if (C_InvoicePaySchedule_ID == 0)
            {
            SetC_InvoicePaySchedule_ID (0);
            SetC_Invoice_ID (0);
            SetDiscountAmt (0.0);
            SetDiscountDate (DateTime.Now);
            SetDueAmt (0.0);
            SetDueDate (DateTime.Now);
            SetIsValid (false);
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_InvoicePaySchedule(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_InvoicePaySchedule(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_InvoicePaySchedule(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_InvoicePaySchedule()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514372548L;
        /** Last Updated Timestamp 7/29/2010 1:07:35 PM */
        public static long updatedMS = 1280389055759L;
        /** AD_Table_ID=551 */
        public static int Table_ID;
        // =551;

        /** TableName=C_InvoicePaySchedule */
        public static String Table_Name = "C_InvoicePaySchedule";

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
            StringBuilder sb = new StringBuilder("X_C_InvoicePaySchedule[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice Payment Schedule.
        @param C_InvoicePaySchedule_ID Invoice Payment Schedule */
        public void SetC_InvoicePaySchedule_ID(int C_InvoicePaySchedule_ID)
        {
            if (C_InvoicePaySchedule_ID < 1) throw new ArgumentException("C_InvoicePaySchedule_ID is mandatory.");
            Set_ValueNoCheck("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
        }
        /** Get Invoice Payment Schedule.
        @return Invoice Payment Schedule */
        public int GetC_InvoicePaySchedule_ID()
        {
            Object ii = Get_Value("C_InvoicePaySchedule_ID");
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
        /** Set Payment Schedule.
        @param C_PaySchedule_ID Payment Schedule Template */
        public void SetC_PaySchedule_ID(int C_PaySchedule_ID)
        {
            if (C_PaySchedule_ID <= 0) Set_ValueNoCheck("C_PaySchedule_ID", null);
            else
                Set_ValueNoCheck("C_PaySchedule_ID", C_PaySchedule_ID);
        }
        /** Get Payment Schedule.
        @return Payment Schedule Template */
        public int GetC_PaySchedule_ID()
        {
            Object ii = Get_Value("C_PaySchedule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Discount Amount.
        @param DiscountAmt Calculated amount of discount */
        public void SetDiscountAmt(Decimal? DiscountAmt)
        {
            if (DiscountAmt == null) throw new ArgumentException("DiscountAmt is mandatory.");
            Set_Value("DiscountAmt", (Decimal?)DiscountAmt);
        }
        /** Get Discount Amount.
        @return Calculated amount of discount */
        public Decimal GetDiscountAmt()
        {
            Object bd = Get_Value("DiscountAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Discount Date.
        @param DiscountDate Last Date for payments with discount */
        public void SetDiscountDate(DateTime? DiscountDate)
        {
            if (DiscountDate == null) throw new ArgumentException("DiscountDate is mandatory.");
            Set_Value("DiscountDate", (DateTime?)DiscountDate);
        }
        /** Get Discount Date.
        @return Last Date for payments with discount */
        public DateTime? GetDiscountDate()
        {
            return (DateTime?)Get_Value("DiscountDate");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetDiscountDate().ToString());
        }
        /** Set Amount due.
        @param DueAmt Amount of the payment due */
        public void SetDueAmt(Decimal? DueAmt)
        {
            if (DueAmt == null) throw new ArgumentException("DueAmt is mandatory.");
            Set_Value("DueAmt", (Decimal?)DueAmt);
        }
        /** Get Amount due.
        @return Amount of the payment due */
        public Decimal GetDueAmt()
        {
            Object bd = Get_Value("DueAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Due Date.
        @param DueDate Date when the payment is due */
        public void SetDueDate(DateTime? DueDate)
        {
            if (DueDate == null) throw new ArgumentException("DueDate is mandatory.");
            Set_Value("DueDate", (DateTime?)DueDate);
        }
        /** Get Due Date.
        @return Date when the payment is due */
        public DateTime? GetDueDate()
        {
            return (DateTime?)Get_Value("DueDate");
        }
        /** Set Valid.
        @param IsValid Element is valid */
        public void SetIsValid(Boolean IsValid)
        {
            Set_Value("IsValid", IsValid);
        }
        /** Get Valid.
        @return Element is valid */
        public Boolean IsValid()
        {
            Object oo = Get_Value("IsValid");
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
            Set_Value("Processed", Processed);
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


        //------------------COLUMN ADDED BY ANUJ-----------------------
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID <= 0) Set_Value("C_DocType_ID", null);
            else
                Set_Value("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** Set Payment Term.
        @param C_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetC_PaymentTerm_ID(int C_PaymentTerm_ID)
        {
            if (C_PaymentTerm_ID <= 0) Set_Value("C_PaymentTerm_ID", null);
            else
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
        /** VA009_ExecutionStatus AD_Reference_ID=1000403 */
        public static int VA009_EXECUTIONSTATUS_AD_Reference_ID = 1000403;
        /** Awaited = A */
        public static String VA009_EXECUTIONSTATUS_Awaited = "A";
        /** Bounced = B */
        public static String VA009_EXECUTIONSTATUS_Bounced = "B";
        /** Rejected = C */
        public static String VA009_EXECUTIONSTATUS_Rejected = "C";
        /** Dunning = D */
        public static String VA009_EXECUTIONSTATUS_Dunning = "D";
        /** In-Progress = I */
        public static String VA009_EXECUTIONSTATUS_In_Progress = "I";
        /** Assigned To Journal = J */
        public static String VA009_EXECUTIONSTATUS_AssignedToJournal = "J";
        /** Overdue = O */
        public static String VA009_EXECUTIONSTATUS_Overdue = "O";
        /** Received = R */
        public static String VA009_EXECUTIONSTATUS_Received = "R";
        /** Stopped = S */
        public static String VA009_EXECUTIONSTATUS_Stopped = "S";
        /** Write-Off = W */
        public static String VA009_EXECUTIONSTATUS_Write_Off = "W";
        /** Assigned To Batch = Y */
        public static String VA009_EXECUTIONSTATUS_AssignedToBatch = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA009_ExecutionStatusValid(String test)
        {
            return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("I") || test.Equals("J") || test.Equals("O") || test.Equals("R") || test.Equals("S") || test.Equals("W") || test.Equals("Y");
        }
        /** Set Payment Execution Status.
        @param VA009_ExecutionStatus Payment Execution Status */
        public void SetVA009_ExecutionStatus(String VA009_ExecutionStatus)
        {
            if (VA009_ExecutionStatus == null) throw new ArgumentException("VA009_ExecutionStatus is mandatory");
            if (!IsVA009_ExecutionStatusValid(VA009_ExecutionStatus))
                throw new ArgumentException("VA009_ExecutionStatus Invalid value - " + VA009_ExecutionStatus + " - Reference_ID=1000403 - A - B - C - D - I - J - O - R - S - W - Y");
            if (VA009_ExecutionStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VA009_ExecutionStatus = VA009_ExecutionStatus.Substring(0, 1);
            }
            Set_Value("VA009_ExecutionStatus", VA009_ExecutionStatus);
        }
        /** Get Payment Execution Status.
        @return Payment Execution Status */
        public String GetVA009_ExecutionStatus()
        {
            return (String)Get_Value("VA009_ExecutionStatus");
        }




        /** Set Follow Up Date.
        @param VA009_FollowupDate Follow Up Date */
        public void SetVA009_FollowupDate(DateTime? VA009_FollowupDate)
        {
            Set_Value("VA009_FollowupDate", (DateTime?)VA009_FollowupDate);
        }
        /** Get Follow Up Date.
        @return Follow Up Date */
        public DateTime? GetVA009_FollowupDate()
        {
            return (DateTime?)Get_Value("VA009_FollowupDate");
        }


        /** Set Grand Total.
        @param VA009_GrandTotal Grand Total */
        public void SetVA009_GrandTotal(Decimal? VA009_GrandTotal)
        {
            Set_Value("VA009_GrandTotal", (Decimal?)VA009_GrandTotal);
        }
        /** Get Grand Total.
        @return Grand Total */
        public Decimal GetVA009_GrandTotal()
        {
            Object bd = Get_Value("VA009_GrandTotal");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }


        /** Set IsPaid.
        @param VA009_IsPaid IsPaid */
        public void SetVA009_IsPaid(Boolean VA009_IsPaid)
        {
            Set_Value("VA009_IsPaid", VA009_IsPaid);
        }
        /** Get IsPaid.
        @return IsPaid */
        public Boolean IsVA009_IsPaid()
        {
            Object oo = Get_Value("VA009_IsPaid");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        //..............Added by Amit for VA009.................

        /** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
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

        /** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetVA009_TransCurrency(int VA009_TransCurrency)
        {
            if (VA009_TransCurrency <= 0) Set_Value("VA009_TransCurrency", null);
            else
                Set_Value("VA009_TransCurrency", VA009_TransCurrency);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetVA009_TransCurrency()
        {
            Object ii = Get_Value("VA009_TransCurrency");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** VA009_BseCurrncy AD_Reference_ID=1000446 */
        public static int VA009_BSECURRNCY_AD_Reference_ID = 1000446;
        /** Set Base Currency.
        @param VA009_BseCurrncy Base Currency */
        public void SetVA009_BseCurrncy(int VA009_BseCurrncy)
        {
            Set_Value("VA009_BseCurrncy", VA009_BseCurrncy);
        }
        /** Get Base Currency.
        @return Base Currency */
        public int GetVA009_BseCurrncy()
        {
            Object ii = Get_Value("VA009_BseCurrncy");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Open Amount (Base).
@param VA009_OpenAmnt Open Amount (Base) */
        public void SetVA009_OpenAmnt(Decimal? VA009_OpenAmnt)
        {
            Set_Value("VA009_OpenAmnt", (Decimal?)VA009_OpenAmnt);
        }
        /** Get Open Amount (Base).
        @return Open Amount (Base) */
        public Decimal GetVA009_OpenAmnt()
        {
            Object bd = Get_Value("VA009_OpenAmnt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Open Amount (Invoice).
        @param VA009_OpnAmntInvce Open Amount (Invoice) */
        public void SetVA009_OpnAmntInvce(Decimal? VA009_OpnAmntInvce)
        {
            Set_Value("VA009_OpnAmntInvce", (Decimal?)VA009_OpnAmntInvce);
        }
        /** Get Open Amount (Invoice).
        @return Open Amount (Invoice) */
        public Decimal GetVA009_OpnAmntInvce()
        {
            Object bd = Get_Value("VA009_OpnAmntInvce");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Paid Amount (Base).
        @param VA009_PaidAmnt Paid Amount (Base) */
        public void SetVA009_PaidAmnt(Decimal? VA009_PaidAmnt)
        {
            Set_Value("VA009_PaidAmnt", (Decimal?)VA009_PaidAmnt);
        }
        /** Get Paid Amount (Base).
        @return Paid Amount (Base) */
        public Decimal GetVA009_PaidAmnt()
        {
            Object bd = Get_Value("VA009_PaidAmnt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Paid Amount(Invoice).
        @param VA009_PaidAmntInvce Paid Amount(Invoice) */
        public void SetVA009_PaidAmntInvce(Decimal? VA009_PaidAmntInvce)
        {
            Set_Value("VA009_PaidAmntInvce", (Decimal?)VA009_PaidAmntInvce);
        }
        /** Get Paid Amount(Invoice).
        @return Paid Amount(Invoice) */
        public Decimal GetVA009_PaidAmntInvce()
        {
            Object bd = Get_Value("VA009_PaidAmntInvce");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Currency Variance.
        @param VA009_Variance Currency Variance */
        public void SetVA009_Variance(Decimal? VA009_Variance)
        {
            Set_Value("VA009_Variance", (Decimal?)VA009_Variance);
        }
        /** Get Currency Variance.
        @return Currency Variance */
        public Decimal GetVA009_Variance()
        {
            Object bd = Get_Value("VA009_Variance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //.....................................................

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


        /** VA009_PaymentMode AD_Reference_ID=1000368 */
        public static int VA009_PAYMENTMODE_AD_Reference_ID = 1000368;
        /** Bank = B */
        public static String VA009_PAYMENTMODE_Bank = "B";
        /** Cash = C */
        public static String VA009_PAYMENTMODE_Cash = "C";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA009_PaymentModeValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("C");
        }
        /** Set Payment Mode.
        @param VA009_PaymentMode Payment Mode */
        public void SetVA009_PaymentMode(String VA009_PaymentMode)
        {
            if (!IsVA009_PaymentModeValid(VA009_PaymentMode))
                throw new ArgumentException("VA009_PaymentMode Invalid value - " + VA009_PaymentMode + " - Reference_ID=1000368 - B - C");
            if (VA009_PaymentMode != null && VA009_PaymentMode.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VA009_PaymentMode = VA009_PaymentMode.Substring(0, 1);
            }
            Set_Value("VA009_PaymentMode", VA009_PaymentMode);
        }
        /** Get Payment Mode.
        @return Payment Mode */
        public String GetVA009_PaymentMode()
        {
            return (String)Get_Value("VA009_PaymentMode");
        }


        /** VA009_PaymentTrigger AD_Reference_ID=1000370 */
        public static int VA009_PAYMENTTRIGGER_AD_Reference_ID = 1000370;
        /** Pull By Recipient = R */
        public static String VA009_PAYMENTTRIGGER_PullByRecipient = "R";
        /** Push By Sender = S */
        public static String VA009_PAYMENTTRIGGER_PushBySender = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA009_PaymentTriggerValid(String test)
        {
            return test == null || test.Equals("R") || test.Equals("S");
        }
        /** Set Payment Trigger By.
        @param VA009_PaymentTrigger Payment Trigger By */
        public void SetVA009_PaymentTrigger(String VA009_PaymentTrigger)
        {
            if (!IsVA009_PaymentTriggerValid(VA009_PaymentTrigger))
                throw new ArgumentException("VA009_PaymentTrigger Invalid value - " + VA009_PaymentTrigger + " - Reference_ID=1000370 - R - S");
            if (VA009_PaymentTrigger != null && VA009_PaymentTrigger.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VA009_PaymentTrigger = VA009_PaymentTrigger.Substring(0, 1);
            }
            Set_Value("VA009_PaymentTrigger", VA009_PaymentTrigger);
        }
        /** Get Payment Trigger By.
        @return Payment Trigger By */
        public String GetVA009_PaymentTrigger()
        {
            return (String)Get_Value("VA009_PaymentTrigger");
        }


        /** VA009_PaymentType AD_Reference_ID=1000369 */
        public static int VA009_PAYMENTTYPE_AD_Reference_ID = 1000369;
        /** Batch = B */
        public static String VA009_PAYMENTTYPE_Batch = "B";
        /** Single = S */
        public static String VA009_PAYMENTTYPE_Single = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA009_PaymentTypeValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("S");
        }
        /** Set Payment Type.
        @param VA009_PaymentType Payment Type */
        public void SetVA009_PaymentType(String VA009_PaymentType)
        {
            if (!IsVA009_PaymentTypeValid(VA009_PaymentType))
                throw new ArgumentException("VA009_PaymentType Invalid value - " + VA009_PaymentType + " - Reference_ID=1000369 - B - S");
            if (VA009_PaymentType != null && VA009_PaymentType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VA009_PaymentType = VA009_PaymentType.Substring(0, 1);
            }
            Set_Value("VA009_PaymentType", VA009_PaymentType);
        }
        /** Get Payment Type.
        @return Payment Type */
        public String GetVA009_PaymentType()
        {
            return (String)Get_Value("VA009_PaymentType");
        }


        /** Set Planned Due Date.
        @param VA009_PlannedDueDate Planned Due Date */
        public void SetVA009_PlannedDueDate(DateTime? VA009_PlannedDueDate)
        {
            Set_Value("VA009_PlannedDueDate", (DateTime?)VA009_PlannedDueDate);
        }
        /** Get Planned Due Date.
        @return Planned Due Date */
        public DateTime? GetVA009_PlannedDueDate()
        {
            return (DateTime?)Get_Value("VA009_PlannedDueDate");
        }


        /** VA009_ReconciliationStatus AD_Reference_ID=1000376 */
        public static int VA009_RECONCILIATIONSTATUS_AD_Reference_ID = 1000376;
        /** Bounced = B */
        public static String VA009_RECONCILIATIONSTATUS_Bounced = "B";
        /** Dunning = D */
        public static String VA009_RECONCILIATIONSTATUS_Dunning = "D";
        /** Overdue = O */
        public static String VA009_RECONCILIATIONSTATUS_Overdue = "O";
        /** Rejected = R */
        public static String VA009_RECONCILIATIONSTATUS_Rejected = "R";
        /** Stopped = S */
        public static String VA009_RECONCILIATIONSTATUS_Stopped = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA009_ReconciliationStatusValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("D") || test.Equals("O") || test.Equals("R") || test.Equals("S");
        }
        /** Set Reconciliation Status.
        @param VA009_ReconciliationStatus Reconciliation Status */
        public void SetVA009_ReconciliationStatus(String VA009_ReconciliationStatus)
        {
            if (!IsVA009_ReconciliationStatusValid(VA009_ReconciliationStatus))
                throw new ArgumentException("VA009_ReconciliationStatus Invalid value - " + VA009_ReconciliationStatus + " - Reference_ID=1000376 - B - D - O - R - S");
            if (VA009_ReconciliationStatus != null && VA009_ReconciliationStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VA009_ReconciliationStatus = VA009_ReconciliationStatus.Substring(0, 1);
            }
            Set_Value("VA009_ReconciliationStatus", VA009_ReconciliationStatus);
        }
        /** Get Reconciliation Status.
        @return Reconciliation Status */
        public String GetVA009_ReconciliationStatus()
        {
            return (String)Get_Value("VA009_ReconciliationStatus");
        }

        /** Set BP Remarks.
        @param VA009_Remarks BP Remarks */
        public void SetVA009_Remarks(String VA009_Remarks)
        {
            if (VA009_Remarks != null && VA009_Remarks.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                VA009_Remarks = VA009_Remarks.Substring(0, 500);
            }
            Set_Value("VA009_Remarks", VA009_Remarks);
        }
        /** Get BP Remarks.
        @return BP Remarks */
        public String GetVA009_Remarks()
        {
            return (String)Get_Value("VA009_Remarks");
        }

        /** Set Discount 2 %.
        @param Discount2 Discount in percent */
        public void SetDiscount2(Decimal? Discount2)
        {
            Set_Value("Discount2", (Decimal?)Discount2);
        }
        /** Get Discount 2 %.
        @return Discount in percent */
        public Decimal GetDiscount2()
        {
            Object bd = Get_Value("Discount2");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }


        /** Set Discount Days 2.
        @param DiscountDays2 Number of days from invoice date to be eligible for discount */
        public void SetDiscountDays2(DateTime? DiscountDays2)
        {
            Set_Value("DiscountDays2", (DateTime?)DiscountDays2);
        }
        /** Get Discount Days 2.
        @return Number of days from invoice date to be eligible for discount */
        public DateTime? GetDiscountDays2()
        {
            return (DateTime?)Get_Value("DiscountDays2");
        }

        /** Set Order Payment Schedule.
        @param VA009_OrderPaySchedule_ID Order Payment Schedule */
        public void SetVA009_OrderPaySchedule_ID(int VA009_OrderPaySchedule_ID)
        {
            if (VA009_OrderPaySchedule_ID <= 0) Set_Value("VA009_OrderPaySchedule_ID", null);
            else
                Set_Value("VA009_OrderPaySchedule_ID", VA009_OrderPaySchedule_ID);
        }
        
        /** Get Order Payment Schedule.
        @return Order Payment Schedule */
        public int GetVA009_OrderPaySchedule_ID()
        {
            Object ii = Get_Value("VA009_OrderPaySchedule_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
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

        /** Set Backup Withholding Amount.
@param BackupWithholdingAmount Backup Withholding Amount */
        public void SetBackupWithholdingAmount(Decimal? BackupWithholdingAmount) { Set_Value("BackupWithholdingAmount", (Decimal?)BackupWithholdingAmount); }/** Get Backup Withholding Amount.
@return Backup Withholding Amount */
        public Decimal GetBackupWithholdingAmount() { Object bd = Get_Value("BackupWithholdingAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Withholding Amount.
@param WithholdingAmt This field represents the calculated withholding amount */
        public void SetWithholdingAmt(Decimal? WithholdingAmt) { Set_Value("WithholdingAmt", (Decimal?)WithholdingAmt); }/** Get Withholding Amount.
@return This field represents the calculated withholding amount */
        public Decimal GetWithholdingAmt() { Object bd = Get_Value("WithholdingAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }


    }

}
