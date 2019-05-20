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
    /** Generated Model for I_Payment
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_I_Payment : PO
    {
        public X_I_Payment(Context ctx, int I_Payment_ID, Trx trxName)
            : base(ctx, I_Payment_ID, trxName)
        {
            /** if (I_Payment_ID == 0)
            {
            SetI_IsImported (null);	// N
            SetI_Payment_ID (0);
            }
             */
        }
        public X_I_Payment(Ctx ctx, int I_Payment_ID, Trx trxName)
            : base(ctx, I_Payment_ID, trxName)
        {
            /** if (I_Payment_ID == 0)
            {
            SetI_IsImported (null);	// N
            SetI_Payment_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Payment(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Payment(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Payment(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_I_Payment()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514377438L;
        /** Last Updated Timestamp 7/29/2010 1:07:40 PM */
        public static long updatedMS = 1280389060649L;
        /** AD_Table_ID=597 */
        public static int Table_ID;
        // =597;

        /** TableName=I_Payment */
        public static String Table_Name = "I_Payment";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(2);
        /** AccessLevel
        @return 2 - Client 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
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
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_I_Payment[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Account City.
        @param A_City City or the Credit Card or Account Holder */
        public void SetA_City(String A_City)
        {
            if (A_City != null && A_City.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                A_City = A_City.Substring(0, 60);
            }
            Set_Value("A_City", A_City);
        }
        /** Get Account City.
        @return City or the Credit Card or Account Holder */
        public String GetA_City()
        {
            return (String)Get_Value("A_City");
        }
        /** Set Account Country.
        @param A_Country Country */
        public void SetA_Country(String A_Country)
        {
            if (A_Country != null && A_Country.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                A_Country = A_Country.Substring(0, 40);
            }
            Set_Value("A_Country", A_Country);
        }
        /** Get Account Country.
        @return Country */
        public String GetA_Country()
        {
            return (String)Get_Value("A_Country");
        }
        /** Set Account EMail.
        @param A_EMail Email Address */
        public void SetA_EMail(String A_EMail)
        {
            if (A_EMail != null && A_EMail.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                A_EMail = A_EMail.Substring(0, 60);
            }
            Set_Value("A_EMail", A_EMail);
        }
        /** Get Account EMail.
        @return Email Address */
        public String GetA_EMail()
        {
            return (String)Get_Value("A_EMail");
        }
        /** Set Driver License.
        @param A_Ident_DL Payment Identification - Driver License */
        public void SetA_Ident_DL(String A_Ident_DL)
        {
            if (A_Ident_DL != null && A_Ident_DL.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                A_Ident_DL = A_Ident_DL.Substring(0, 20);
            }
            Set_Value("A_Ident_DL", A_Ident_DL);
        }
        /** Get Driver License.
        @return Payment Identification - Driver License */
        public String GetA_Ident_DL()
        {
            return (String)Get_Value("A_Ident_DL");
        }
        /** Set Social Security No.
        @param A_Ident_SSN Payment Identification - Social Security No */
        public void SetA_Ident_SSN(String A_Ident_SSN)
        {
            if (A_Ident_SSN != null && A_Ident_SSN.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                A_Ident_SSN = A_Ident_SSN.Substring(0, 20);
            }
            Set_Value("A_Ident_SSN", A_Ident_SSN);
        }
        /** Get Social Security No.
        @return Payment Identification - Social Security No */
        public String GetA_Ident_SSN()
        {
            return (String)Get_Value("A_Ident_SSN");
        }
        /** Set Account Name.
        @param A_Name Name on Credit Card or Account holder */
        public void SetA_Name(String A_Name)
        {
            if (A_Name != null && A_Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                A_Name = A_Name.Substring(0, 60);
            }
            Set_Value("A_Name", A_Name);
        }
        /** Get Account Name.
        @return Name on Credit Card or Account holder */
        public String GetA_Name()
        {
            return (String)Get_Value("A_Name");
        }
        /** Set Account State.
        @param A_State State of the Credit Card or Account holder */
        public void SetA_State(String A_State)
        {
            if (A_State != null && A_State.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                A_State = A_State.Substring(0, 40);
            }
            Set_Value("A_State", A_State);
        }
        /** Get Account State.
        @return State of the Credit Card or Account holder */
        public String GetA_State()
        {
            return (String)Get_Value("A_State");
        }
        /** Set Account Street.
        @param A_Street Street address of the Credit Card or Account holder */
        public void SetA_Street(String A_Street)
        {
            if (A_Street != null && A_Street.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                A_Street = A_Street.Substring(0, 60);
            }
            Set_Value("A_Street", A_Street);
        }
        /** Get Account Street.
        @return Street address of the Credit Card or Account holder */
        public String GetA_Street()
        {
            return (String)Get_Value("A_Street");
        }
        /** Set Account Zip/Postal.
        @param A_Zip Zip Code of the Credit Card or Account Holder */
        public void SetA_Zip(String A_Zip)
        {
            if (A_Zip != null && A_Zip.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                A_Zip = A_Zip.Substring(0, 20);
            }
            Set_Value("A_Zip", A_Zip);
        }
        /** Get Account Zip/Postal.
        @return Zip Code of the Credit Card or Account Holder */
        public String GetA_Zip()
        {
            return (String)Get_Value("A_Zip");
        }
        /** Set Account No.
        @param AccountNo Account Number */
        public void SetAccountNo(String AccountNo)
        {
            if (AccountNo != null && AccountNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                AccountNo = AccountNo.Substring(0, 20);
            }
            Set_Value("AccountNo", AccountNo);
        }
        /** Get Account No.
        @return Account Number */
        public String GetAccountNo()
        {
            return (String)Get_Value("AccountNo");
        }
        /** Set Business Partner Key.
        @param BPartnerValue Key of the Business Partner */
        public void SetBPartnerValue(String BPartnerValue)
        {
            if (BPartnerValue != null && BPartnerValue.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                BPartnerValue = BPartnerValue.Substring(0, 40);
            }
            Set_Value("BPartnerValue", BPartnerValue);
        }
        /** Get Business Partner Key.
        @return Key of the Business Partner */
        public String GetBPartnerValue()
        {
            return (String)Get_Value("BPartnerValue");
        }
        /** Set Bank Account No.
        @param BankAccountNo Bank Account Number */
        public void SetBankAccountNo(String BankAccountNo)
        {
            if (BankAccountNo != null && BankAccountNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                BankAccountNo = BankAccountNo.Substring(0, 20);
            }
            Set_Value("BankAccountNo", BankAccountNo);
        }
        /** Get Bank Account No.
        @return Bank Account Number */
        public String GetBankAccountNo()
        {
            return (String)Get_Value("BankAccountNo");
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
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
        /** Set Bank Account.
        @param C_BankAccount_ID Account at the Bank */
        public void SetC_BankAccount_ID(int C_BankAccount_ID)
        {
            if (C_BankAccount_ID <= 0) Set_Value("C_BankAccount_ID", null);
            else
                Set_Value("C_BankAccount_ID", C_BankAccount_ID);
        }
        /** Get Bank Account.
        @return Account at the Bank */
        public int GetC_BankAccount_ID()
        {
            Object ii = Get_Value("C_BankAccount_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
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
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_Value("C_Invoice_ID", null);
            else
                Set_Value("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
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
        /** Set Charge Name.
        @param ChargeName Name of the Charge */
        public void SetChargeName(String ChargeName)
        {
            if (ChargeName != null && ChargeName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                ChargeName = ChargeName.Substring(0, 60);
            }
            Set_Value("ChargeName", ChargeName);
        }
        /** Get Charge Name.
        @return Name of the Charge */
        public String GetChargeName()
        {
            return (String)Get_Value("ChargeName");
        }
        /** Set Check No.
        @param CheckNo Check Number */
        public void SetCheckNo(String CheckNo)
        {
            if (CheckNo != null && CheckNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                CheckNo = CheckNo.Substring(0, 20);
            }
            Set_Value("CheckNo", CheckNo);
        }
        /** Get Check No.
        @return Check Number */
        public String GetCheckNo()
        {
            return (String)Get_Value("CheckNo");
        }
        /** Set Exp. Month.
        @param CreditCardExpMM Expiry Month */
        public void SetCreditCardExpMM(int CreditCardExpMM)
        {
            Set_Value("CreditCardExpMM", CreditCardExpMM);
        }
        /** Get Exp. Month.
        @return Expiry Month */
        public int GetCreditCardExpMM()
        {
            Object ii = Get_Value("CreditCardExpMM");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Exp. Year.
        @param CreditCardExpYY Expiry Year */
        public void SetCreditCardExpYY(int CreditCardExpYY)
        {
            Set_Value("CreditCardExpYY", CreditCardExpYY);
        }
        /** Get Exp. Year.
        @return Expiry Year */
        public int GetCreditCardExpYY()
        {
            Object ii = Get_Value("CreditCardExpYY");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Number.
        @param CreditCardNumber Credit Card Number */
        public void SetCreditCardNumber(String CreditCardNumber)
        {
            if (CreditCardNumber != null && CreditCardNumber.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                CreditCardNumber = CreditCardNumber.Substring(0, 20);
            }
            Set_Value("CreditCardNumber", CreditCardNumber);
        }
        /** Get Number.
        @return Credit Card Number */
        public String GetCreditCardNumber()
        {
            return (String)Get_Value("CreditCardNumber");
        }

        /** CreditCardType AD_Reference_ID=149 */
        public static int CREDITCARDTYPE_AD_Reference_ID = 149;
        /** Amex = A */
        public static String CREDITCARDTYPE_Amex = "A";
        /** ATM = C */
        public static String CREDITCARDTYPE_ATM = "C";
        /** Diners = D */
        public static String CREDITCARDTYPE_Diners = "D";
        /** MasterCard = M */
        public static String CREDITCARDTYPE_MasterCard = "M";
        /** Discover = N */
        public static String CREDITCARDTYPE_Discover = "N";
        /** Purchase Card = P */
        public static String CREDITCARDTYPE_PurchaseCard = "P";
        /** Visa = V */
        public static String CREDITCARDTYPE_Visa = "V";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCreditCardTypeValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("C") || test.Equals("D") || test.Equals("M") || test.Equals("N") || test.Equals("P") || test.Equals("V");
        }
        /** Set Credit Card.
        @param CreditCardType Credit Card (Visa, MC, AmEx) */
        public void SetCreditCardType(String CreditCardType)
        {
            if (!IsCreditCardTypeValid(CreditCardType))
                throw new ArgumentException("CreditCardType Invalid value - " + CreditCardType + " - Reference_ID=149 - A - C - D - M - N - P - V");
            if (CreditCardType != null && CreditCardType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreditCardType = CreditCardType.Substring(0, 1);
            }
            Set_Value("CreditCardType", CreditCardType);
        }
        /** Get Credit Card.
        @return Credit Card (Visa, MC, AmEx) */
        public String GetCreditCardType()
        {
            return (String)Get_Value("CreditCardType");
        }
        /** Set Verification Code.
        @param CreditCardVV Credit Card Verification code on credit card */
        public void SetCreditCardVV(String CreditCardVV)
        {
            if (CreditCardVV != null && CreditCardVV.Length > 4)
            {
                log.Warning("Length > 4 - truncated");
                CreditCardVV = CreditCardVV.Substring(0, 4);
            }
            Set_Value("CreditCardVV", CreditCardVV);
        }
        /** Get Verification Code.
        @return Credit Card Verification code on credit card */
        public String GetCreditCardVV()
        {
            return (String)Get_Value("CreditCardVV");
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Transaction Date.
        @param DateTrx Transaction Date */
        public void SetDateTrx(DateTime? DateTrx)
        {
            Set_Value("DateTrx", (DateTime?)DateTrx);
        }
        /** Get Transaction Date.
        @return Transaction Date */
        public DateTime? GetDateTrx()
        {
            return (DateTime?)Get_Value("DateTrx");
        }
        /** Set Discount Amount.
        @param DiscountAmt Calculated amount of discount */
        public void SetDiscountAmt(Decimal? DiscountAmt)
        {
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
        /** Set Document Type Name.
        @param DocTypeName Name of the Document Type */
        public void SetDocTypeName(String DocTypeName)
        {
            if (DocTypeName != null && DocTypeName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                DocTypeName = DocTypeName.Substring(0, 60);
            }
            Set_Value("DocTypeName", DocTypeName);
        }
        /** Get Document Type Name.
        @return Name of the Document Type */
        public String GetDocTypeName()
        {
            return (String)Get_Value("DocTypeName");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo != null && DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_Value("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Set ISO Currency Code.
        @param ISO_Code Three letter ISO 4217 Code of the Currency */
        public void SetISO_Code(String ISO_Code)
        {
            if (ISO_Code != null && ISO_Code.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                ISO_Code = ISO_Code.Substring(0, 3);
            }
            Set_Value("ISO_Code", ISO_Code);
        }
        /** Get ISO Currency Code.
        @return Three letter ISO 4217 Code of the Currency */
        public String GetISO_Code()
        {
            return (String)Get_Value("ISO_Code");
        }
        /** Set Import Error Message.
        @param I_ErrorMsg Messages generated from import process */
        public void SetI_ErrorMsg(String I_ErrorMsg)
        {
            if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                I_ErrorMsg = I_ErrorMsg.Substring(0, 2000);
            }
            Set_Value("I_ErrorMsg", I_ErrorMsg);
        }
        /** Get Import Error Message.
        @return Messages generated from import process */
        public String GetI_ErrorMsg()
        {
            return (String)Get_Value("I_ErrorMsg");
        }

        /** I_IsImported AD_Reference_ID=420 */
        public static int I_ISIMPORTED_AD_Reference_ID = 420;
        /** Error = E */
        public static String I_ISIMPORTED_Error = "E";
        /** No = N */
        public static String I_ISIMPORTED_No = "N";
        /** Yes = Y */
        public static String I_ISIMPORTED_Yes = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsI_IsImportedValid(String test)
        {
            return test.Equals("E") || test.Equals("N") || test.Equals("Y");
        }
        /** Set Imported.
        @param I_IsImported Has this import been processed */
        public void SetI_IsImported(String I_IsImported)
        {
            if (I_IsImported == null) throw new ArgumentException("I_IsImported is mandatory");
            if (!IsI_IsImportedValid(I_IsImported))
                throw new ArgumentException("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
            if (I_IsImported.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                I_IsImported = I_IsImported.Substring(0, 1);
            }
            Set_Value("I_IsImported", I_IsImported);
        }
        /** Get Imported.
        @return Has this import been processed */
        public String GetI_IsImported()
        {
            return (String)Get_Value("I_IsImported");
        }
        /** Set Import Payment.
        @param I_Payment_ID Import Payment */
        public void SetI_Payment_ID(int I_Payment_ID)
        {
            if (I_Payment_ID < 1) throw new ArgumentException("I_Payment_ID is mandatory.");
            Set_ValueNoCheck("I_Payment_ID", I_Payment_ID);
        }
        /** Get Import Payment.
        @return Import Payment */
        public int GetI_Payment_ID()
        {
            Object ii = Get_Value("I_Payment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Document No.
        @param InvoiceDocumentNo Document Number of the Invoice */
        public void SetInvoiceDocumentNo(String InvoiceDocumentNo)
        {
            if (InvoiceDocumentNo != null && InvoiceDocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                InvoiceDocumentNo = InvoiceDocumentNo.Substring(0, 30);
            }
            Set_Value("InvoiceDocumentNo", InvoiceDocumentNo);
        }
        /** Get Invoice Document No.
        @return Document Number of the Invoice */
        public String GetInvoiceDocumentNo()
        {
            return (String)Get_Value("InvoiceDocumentNo");
        }
        /** Set Approved.
        @param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved)
        {
            Set_Value("IsApproved", IsApproved);
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
        /** Set Delayed Capture.
        @param IsDelayedCapture Charge after Shipment */
        public void SetIsDelayedCapture(Boolean IsDelayedCapture)
        {
            Set_Value("IsDelayedCapture", IsDelayedCapture);
        }
        /** Get Delayed Capture.
        @return Charge after Shipment */
        public Boolean IsDelayedCapture()
        {
            Object oo = Get_Value("IsDelayedCapture");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Over/Under Payment.
        @param IsOverUnderPayment Over-Payment (unallocated) or Under-Payment (partial payment) */
        public void SetIsOverUnderPayment(Boolean IsOverUnderPayment)
        {
            Set_Value("IsOverUnderPayment", IsOverUnderPayment);
        }
        /** Get Over/Under Payment.
        @return Over-Payment (unallocated) or Under-Payment (partial payment) */
        public Boolean IsOverUnderPayment()
        {
            Object oo = Get_Value("IsOverUnderPayment");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Receipt.
        @param IsReceipt This is a sales transaction (receipt) */
        public void SetIsReceipt(Boolean IsReceipt)
        {
            Set_Value("IsReceipt", IsReceipt);
        }
        /** Get Receipt.
        @return This is a sales transaction (receipt) */
        public Boolean IsReceipt()
        {
            Object oo = Get_Value("IsReceipt");
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
        /** Set Micr.
        @param Micr Combination of routing no, account and check no */
        public void SetMicr(String Micr)
        {
            if (Micr != null && Micr.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                Micr = Micr.Substring(0, 20);
            }
            Set_Value("Micr", Micr);
        }
        /** Get Micr.
        @return Combination of routing no, account and check no */
        public String GetMicr()
        {
            return (String)Get_Value("Micr");
        }
        /** Set Original Transaction ID.
        @param Orig_TrxID Original Transaction ID */
        public void SetOrig_TrxID(String Orig_TrxID)
        {
            if (Orig_TrxID != null && Orig_TrxID.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                Orig_TrxID = Orig_TrxID.Substring(0, 20);
            }
            Set_Value("Orig_TrxID", Orig_TrxID);
        }
        /** Get Original Transaction ID.
        @return Original Transaction ID */
        public String GetOrig_TrxID()
        {
            return (String)Get_Value("Orig_TrxID");
        }
        /** Set Over/Under Payment.
        @param OverUnderAmt Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
        public void SetOverUnderAmt(Decimal? OverUnderAmt)
        {
            Set_Value("OverUnderAmt", (Decimal?)OverUnderAmt);
        }
        /** Get Over/Under Payment.
        @return Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
        public Decimal GetOverUnderAmt()
        {
            Object bd = Get_Value("OverUnderAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set PO Number.
        @param PONum Purchase Order Number */
        public void SetPONum(String PONum)
        {
            if (PONum != null && PONum.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                PONum = PONum.Substring(0, 60);
            }
            Set_Value("PONum", PONum);
        }
        /** Get PO Number.
        @return Purchase Order Number */
        public String GetPONum()
        {
            return (String)Get_Value("PONum");
        }
        /** Set Payment amount.
        @param PayAmt Amount being paid */
        public void SetPayAmt(Decimal? PayAmt)
        {
            Set_Value("PayAmt", (Decimal?)PayAmt);
        }
        /** Get Payment amount.
        @return Amount being paid */
        public Decimal GetPayAmt()
        {
            Object bd = Get_Value("PayAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Authorization Code.
        @param R_AuthCode Authorization Code returned */
        public void SetR_AuthCode(String R_AuthCode)
        {
            if (R_AuthCode != null && R_AuthCode.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                R_AuthCode = R_AuthCode.Substring(0, 20);
            }
            Set_Value("R_AuthCode", R_AuthCode);
        }
        /** Get Authorization Code.
        @return Authorization Code returned */
        public String GetR_AuthCode()
        {
            return (String)Get_Value("R_AuthCode");
        }
        /** Set Info.
        @param R_Info Response info */
        public void SetR_Info(String R_Info)
        {
            if (R_Info != null && R_Info.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                R_Info = R_Info.Substring(0, 2000);
            }
            Set_Value("R_Info", R_Info);
        }
        /** Get Info.
        @return Response info */
        public String GetR_Info()
        {
            return (String)Get_Value("R_Info");
        }
        /** Set Reference.
        @param R_PnRef Payment reference */
        public void SetR_PnRef(String R_PnRef)
        {
            if (R_PnRef != null && R_PnRef.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                R_PnRef = R_PnRef.Substring(0, 20);
            }
            Set_Value("R_PnRef", R_PnRef);
        }
        /** Get Reference.
        @return Payment reference */
        public String GetR_PnRef()
        {
            return (String)Get_Value("R_PnRef");
        }
        /** Set Response Message.
        @param R_RespMsg Response message */
        public void SetR_RespMsg(String R_RespMsg)
        {
            if (R_RespMsg != null && R_RespMsg.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                R_RespMsg = R_RespMsg.Substring(0, 60);
            }
            Set_Value("R_RespMsg", R_RespMsg);
        }
        /** Get Response Message.
        @return Response message */
        public String GetR_RespMsg()
        {
            return (String)Get_Value("R_RespMsg");
        }
        /** Set Result.
        @param R_Result Result of transmission */
        public void SetR_Result(String R_Result)
        {
            if (R_Result != null && R_Result.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                R_Result = R_Result.Substring(0, 20);
            }
            Set_Value("R_Result", R_Result);
        }
        /** Get Result.
        @return Result of transmission */
        public String GetR_Result()
        {
            return (String)Get_Value("R_Result");
        }
        /** Set Routing No.
        @param RoutingNo Bank Routing Number */
        public void SetRoutingNo(String RoutingNo)
        {
            if (RoutingNo != null && RoutingNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                RoutingNo = RoutingNo.Substring(0, 20);
            }
            Set_Value("RoutingNo", RoutingNo);
        }
        /** Get Routing No.
        @return Bank Routing Number */
        public String GetRoutingNo()
        {
            return (String)Get_Value("RoutingNo");
        }
        /** Set Swipe.
        @param Swipe Track 1 and 2 of the Credit Card */
        public void SetSwipe(String Swipe)
        {
            if (Swipe != null && Swipe.Length > 80)
            {
                log.Warning("Length > 80 - truncated");
                Swipe = Swipe.Substring(0, 80);
            }
            Set_Value("Swipe", Swipe);
        }
        /** Get Swipe.
        @return Track 1 and 2 of the Credit Card */
        public String GetSwipe()
        {
            return (String)Get_Value("Swipe");
        }
        /** Set Tax Amount.
        @param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt)
        {
            Set_Value("TaxAmt", (Decimal?)TaxAmt);
        }
        /** Get Tax Amount.
        @return Tax Amount for a document */
        public Decimal GetTaxAmt()
        {
            Object bd = Get_Value("TaxAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** TenderType AD_Reference_ID=214 */
        public static int TENDERTYPE_AD_Reference_ID = 214;
        /** Direct Deposit = A */
        public static String TENDERTYPE_DirectDeposit = "A";
        /** Credit Card = C */
        public static String TENDERTYPE_CreditCard = "C";
        /** Direct Debit = D */
        public static String TENDERTYPE_DirectDebit = "D";
        /** Check = K */
        public static String TENDERTYPE_Check = "K";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsTenderTypeValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("C") || test.Equals("D") || test.Equals("K");
        }
        /** Set Tender type.
        @param TenderType Method of Payment */
        public void SetTenderType(String TenderType)
        {
            if (!IsTenderTypeValid(TenderType))
                throw new ArgumentException("TenderType Invalid value - " + TenderType + " - Reference_ID=214 - A - C - D - K");
            if (TenderType != null && TenderType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                TenderType = TenderType.Substring(0, 1);
            }
            Set_Value("TenderType", TenderType);
        }
        /** Get Tender type.
        @return Method of Payment */
        public String GetTenderType()
        {
            return (String)Get_Value("TenderType");
        }

        /** TrxType AD_Reference_ID=215 */
        public static int TRXTYPE_AD_Reference_ID = 215;
        /** Authorization = A */
        public static String TRXTYPE_Authorization = "A";
        /** Credit (Payment) = C */
        public static String TRXTYPE_CreditPayment = "C";
        /** Delayed Capture = D */
        public static String TRXTYPE_DelayedCapture = "D";
        /** Voice Authorization = F */
        public static String TRXTYPE_VoiceAuthorization = "F";
        /** Sales = S */
        public static String TRXTYPE_Sales = "S";
        /** Void = V */
        public static String TRXTYPE_Void = "V";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsTrxTypeValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("C") || test.Equals("D") || test.Equals("F") || test.Equals("S") || test.Equals("V");
        }
        /** Set Transaction Type.
        @param TrxType Type of credit card transaction */
        public void SetTrxType(String TrxType)
        {
            if (!IsTrxTypeValid(TrxType))
                throw new ArgumentException("TrxType Invalid value - " + TrxType + " - Reference_ID=215 - A - C - D - F - S - V");
            if (TrxType != null && TrxType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                TrxType = TrxType.Substring(0, 1);
            }
            Set_Value("TrxType", TrxType);
        }
        /** Get Transaction Type.
        @return Type of credit card transaction */
        public String GetTrxType()
        {
            return (String)Get_Value("TrxType");
        }
        /** Set Voice authorization code.
        @param VoiceAuthCode Voice Authorization Code from credit card company */
        public void SetVoiceAuthCode(String VoiceAuthCode)
        {
            if (VoiceAuthCode != null && VoiceAuthCode.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                VoiceAuthCode = VoiceAuthCode.Substring(0, 20);
            }
            Set_Value("VoiceAuthCode", VoiceAuthCode);
        }
        /** Get Voice authorization code.
        @return Voice Authorization Code from credit card company */
        public String GetVoiceAuthCode()
        {
            return (String)Get_Value("VoiceAuthCode");
        }
        /** Set Write-off Amount.
        @param WriteOffAmt Amount to write-off */
        public void SetWriteOffAmt(Decimal? WriteOffAmt)
        {
            Set_Value("WriteOffAmt", (Decimal?)WriteOffAmt);
        }
        /** Get Write-off Amount.
        @return Amount to write-off */
        public Decimal GetWriteOffAmt()
        {
            Object bd = Get_Value("WriteOffAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Customer.
     @param IsCustomer Indicates if this Business Partner is a Customer */
        public void SetIsCustomer(Boolean IsCustomer)
        {
            Set_Value("IsCustomer", IsCustomer);
        }
        /** Get Customer.
        @return Indicates if this Business Partner is a Customer */
        public Boolean IsCustomer()
        {
            Object oo = Get_Value("IsCustomer");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Representative.
       @param IsSalesRep Indicates if  the business partner is a representative or company agent */
        public void SetIsSalesRep(Boolean IsSalesRep)
        {
            Set_Value("IsSalesRep", IsSalesRep);
        }
        /** Get Representative.
        @return Indicates if  the business partner is a representative or company agent */
        public Boolean IsSalesRep()
        {
            Object oo = Get_Value("IsSalesRep");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Vendor.
        @param IsVendor Indicates if this Business Partner is a Vendor */
        public void SetIsVendor(Boolean IsVendor)
        {
            Set_Value("IsVendor", IsVendor);
        }
        /** Get Vendor.
        @return Indicates if this Business Partner is a Vendor */
        public Boolean IsVendor()
        {
            Object oo = Get_Value("IsVendor");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Name.
        @param Name Name */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Name */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }

        /** Set UnAllocatedPayment.
        @param UnAllocatedPayment UnAllocatedPayment */
        public void SetUnAllocatedPayment(Decimal? UnAllocatedPayment)
        {
            Set_Value("UnAllocatedPayment", (Decimal?)UnAllocatedPayment);
        }
        /** Get UnAllocatedPayment.
        @return UnAllocatedPayment */
        public Decimal GetUnAllocatedPayment()
        {
            Object bd = Get_Value("UnAllocatedPayment");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

    }

}
