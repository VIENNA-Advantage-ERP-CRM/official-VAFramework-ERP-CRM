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
    /** Generated Model for C_CashLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_CashLine : PO
    {
        public X_C_CashLine(Context ctx, int C_CashLine_ID, Trx trxName)
            : base(ctx, C_CashLine_ID, trxName)
        {
            /** if (C_CashLine_ID == 0)
            {
            SetAmount (0.0);
            SetC_CashLine_ID (0);
            SetC_Cash_ID (0);
            SetCashType (null);	// E
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_CashLine WHERE C_Cash_ID=@C_Cash_ID@
            SetProcessed (false);	// N
            }
             */
        }
        public X_C_CashLine(Ctx ctx, int C_CashLine_ID, Trx trxName)
            : base(ctx, C_CashLine_ID, trxName)
        {
            /** if (C_CashLine_ID == 0)
            {
            SetAmount (0.0);
            SetC_CashLine_ID (0);
            SetC_Cash_ID (0);
            SetCashType (null);	// E
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_CashLine WHERE C_Cash_ID=@C_Cash_ID@
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_CashLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_CashLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_CashLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_CashLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27633016154099L;
        /** Last Updated Timestamp 10/22/2012 12:57:17 PM */
        public static long updatedMS = 1350890837310L;
        /** AD_Table_ID=410 */
        public static int Table_ID;
        // =410;

        /** TableName=C_CashLine */
        public static String Table_Name = "C_CashLine";

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
            StringBuilder sb = new StringBuilder("X_C_CashLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Amount.
        @param Amount Amount in a defined currency */
        public void SetAmount(Decimal? Amount)
        {
            if (Amount == null) throw new ArgumentException("Amount is mandatory.");
            Set_Value("Amount", (Decimal?)Amount);
        }
        /** Get Amount.
        @return Amount in a defined currency */
        public Decimal GetAmount()
        {
            Object bd = Get_Value("Amount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
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
        /** Set Cash Journal Line.
        @param C_CashLine_ID Cash Journal Line */
        public void SetC_CashLine_ID(int C_CashLine_ID)
        {
            if (C_CashLine_ID < 1) throw new ArgumentException("C_CashLine_ID is mandatory.");
            Set_ValueNoCheck("C_CashLine_ID", C_CashLine_ID);
        }
        /** Get Cash Journal Line.
        @return Cash Journal Line */
        public int GetC_CashLine_ID()
        {
            Object ii = Get_Value("C_CashLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Journal.
        @param C_Cash_ID Cash Journal */
        public void SetC_Cash_ID(int C_Cash_ID)
        {
            if (C_Cash_ID < 1) throw new ArgumentException("C_Cash_ID is mandatory.");
            Set_ValueNoCheck("C_Cash_ID", C_Cash_ID);
        }
        /** Get Cash Journal.
        @return Cash Journal */
        public int GetC_Cash_ID()
        {
            Object ii = Get_Value("C_Cash_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_Cash_ID().ToString());
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
            if (C_Currency_ID <= 0) Set_ValueNoCheck("C_Currency_ID", null);
            else
                Set_ValueNoCheck("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_ValueNoCheck("C_Invoice_ID", null);
            else
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
        /** Set Cancel.
        @param Cancel Cancel */
        public void SetCancel(String Cancel)
        {
            if (Cancel != null && Cancel.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Cancel = Cancel.Substring(0, 50);
            }
            Set_Value("Cancel", Cancel);
        }
        /** Get Cancel.
        @return Cancel */
        public String GetCancel()
        {
            return (String)Get_Value("Cancel");
        }

        /** CashType AD_Reference_ID=217 */
        public static int CASHTYPE_AD_Reference_ID = 217;
        /** Cash Book Transfer = A */
        public static String CASHTYPE_CashBookTransfer = "A";
        /** Business Partner = B */
        public static String CASHTYPE_BusinessPartner = "B";
        /** Charge = C */
        public static String CASHTYPE_Charge = "C";
        /** Difference = D */
        public static String CASHTYPE_Difference = "D";
        /** General Expense = E */
        public static String CASHTYPE_GeneralExpense = "E";
        /** Cash Recieved From = F */
        public static String CASHTYPE_CashRecievedFrom = "F";
        /** Invoice = I */
        public static String CASHTYPE_Invoice = "I";
        /** Order = O */
        public static String CASHTYPE_Order = "O";
        /** General Receipts = R */
        public static String CASHTYPE_GeneralReceipts = "R";
        /** Bank Account Transfer = T */
        public static String CASHTYPE_BankAccountTransfer = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCashTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("E") || test.Equals("F") || test.Equals("I") || test.Equals("O") || test.Equals("R") || test.Equals("T");
        }
        /** Set Cash Type.
        @param CashType Source of Cash */
        public void SetCashType(String CashType)
        {
            if (CashType == null) throw new ArgumentException("CashType is mandatory");
            if (!IsCashTypeValid(CashType))
                throw new ArgumentException("CashType Invalid value - " + CashType + " - Reference_ID=217 - A - B - C - D - E - F - I - O - R - T");
            if (CashType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CashType = CashType.Substring(0, 1);
            }
            Set_ValueNoCheck("CashType", CashType);
        }
        /** Get Cash Type.
        @return Source of Cash */
        public String GetCashType()
        {
            return (String)Get_Value("CashType");
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
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set Allocated.
        @param IsAllocated Indicates if the payment has been allocated */
        public void SetIsAllocated(Boolean IsAllocated)
        {
            Set_Value("IsAllocated", IsAllocated);
        }
        /** Get Allocated.
        @return Indicates if the payment has been allocated */
        public Boolean IsAllocated()
        {
            Object oo = Get_Value("IsAllocated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Generated.
        @param IsGenerated This Line is generated */
        public void SetIsGenerated(Boolean IsGenerated)
        {
            Set_ValueNoCheck("IsGenerated", IsGenerated);
        }
        /** Get Generated.
        @return This Line is generated */
        public Boolean IsGenerated()
        {
            Object oo = Get_Value("IsGenerated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line No.
        @param Line Unique line for this document */
        public void SetLine(int Line)
        {
            Set_Value("Line", Line);
        }
        /** Get Line No.
        @return Unique line for this document */
        public int GetLine()
        {
            Object ii = Get_Value("Line");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Name = Name.Substring(0, 50);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
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

        /** VSS_PAYMENTTYPE AD_Reference_ID=1000108 */
        public static int VSS_PAYMENTTYPE_AD_Reference_ID = 1000108;
        /** Payment = P */
        public static String VSS_PAYMENTTYPE_Payment = "P";
        /** Receipt = R */
        public static String VSS_PAYMENTTYPE_Receipt = "R";

        /* Payment Return = A */
         public static String VSS_PAYMENTTYPE_PaymentReturn = "A";
         /* Receipt Return = E */
         public static String VSS_PAYMENTTYPE_ReceiptReturn = "E";																  
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVSS_PAYMENTTYPEValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("E") || test.Equals("P") || test.Equals("R");
        }
        /** Set Payment Type.
        @param VSS_PAYMENTTYPE Payment Type */
        public void SetVSS_PAYMENTTYPE(String VSS_PAYMENTTYPE)
        {
            if (!IsVSS_PAYMENTTYPEValid(VSS_PAYMENTTYPE))
                throw new ArgumentException("VSS_PAYMENTTYPE Invalid value - " + VSS_PAYMENTTYPE + " - Reference_ID=1000108 - A - E - P - R");
            if (VSS_PAYMENTTYPE != null && VSS_PAYMENTTYPE.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VSS_PAYMENTTYPE = VSS_PAYMENTTYPE.Substring(0, 1);
            }
            Set_Value("VSS_PAYMENTTYPE", VSS_PAYMENTTYPE);
        }
        /** Get Payment Type.
        @return Payment Type */
        public String GetVSS_PAYMENTTYPE()
        {
            return (String)Get_Value("VSS_PAYMENTTYPE");
        }
        /** Set Receipt No..
        @param VSS_RECEIPTNO Receipt No. */
        public void SetVSS_RECEIPTNO(String VSS_RECEIPTNO)
        {
            if (VSS_RECEIPTNO != null && VSS_RECEIPTNO.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                VSS_RECEIPTNO = VSS_RECEIPTNO.Substring(0, 20);
            }
            Set_Value("VSS_RECEIPTNO", VSS_RECEIPTNO);
        }
        /** Get Receipt No..
        @return Receipt No. */
        public String GetVSS_RECEIPTNO()
        {
            return (String)Get_Value("VSS_RECEIPTNO");
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
        /** Set vss_Paymentvoucher.
        @param vss_Paymentvoucher vss_Paymentvoucher */
        public void Setvss_Paymentvoucher(String vss_Paymentvoucher)
        {
            if (vss_Paymentvoucher != null && vss_Paymentvoucher.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                vss_Paymentvoucher = vss_Paymentvoucher.Substring(0, 50);
            }
            Set_Value("vss_Paymentvoucher", vss_Paymentvoucher);
        }
        /** Get vss_Paymentvoucher.
        @return vss_Paymentvoucher */
        public String Getvss_Paymentvoucher()
        {
            return (String)Get_Value("vss_Paymentvoucher");
        }


        /** Set C_CashBook_ID.
       @param C_CashBook_ID for this record */
        public void SetC_CashBook_ID(int C_CashBook_ID)
        {
            if (C_CashBook_ID <= 0) Set_ValueNoCheck("C_CashBook_ID", null);
            else
                Set_ValueNoCheck("C_CashBook_ID", C_CashBook_ID);
        }
        /** Get C_CashBook_ID.
        @return The C_CashBook_ID for this record */
        public int GetC_CashBook_ID()
        {
            Object ii = Get_Value("C_CashBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }



        /** C_CashLine_ID_1 AD_Reference_ID=1000168 */
        public static int C_CASHLINE_ID_1_AD_Reference_ID = 1000168;
        /** Set Cash Journal Transection.
        @param C_CashLine_ID_1 Cash Journal Transection */
        public void SetC_CashLine_ID_1(int C_CashLine_ID_1)
        {
            Set_Value("C_CashLine_ID_1", C_CashLine_ID_1);
        }
        /** Get Cash Journal Transection.
        @return Cash Journal Transection */
        public int GetC_CashLine_ID_1()
        {
            Object ii = Get_Value("C_CashLine_ID_1");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
        public void SetC_InvoicePaySchedule_ID(int C_InvoicePaySchedule_ID)
        {
            if (C_InvoicePaySchedule_ID <= 0) Set_Value("C_InvoicePaySchedule_ID", null);
            else
                Set_Value("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
        }
        /** Get Invoice Payment Schedule.
        @return Invoice Payment Schedule */
        public int GetC_InvoicePaySchedule_ID()
        {
            Object ii = Get_Value("C_InvoicePaySchedule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Converted Amount.
       @param ConvertedAmt Converted Amount */
        public void SetConvertedAmt(String ConvertedAmt)
        {
            if (ConvertedAmt != null && ConvertedAmt.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ConvertedAmt = ConvertedAmt.Substring(0, 50);
            }
            Set_Value("ConvertedAmt", ConvertedAmt);
        }
        /** Get Converted Amount.
        @return Converted Amount */
        public String GetConvertedAmt()
        {
            return (String)Get_Value("ConvertedAmt");
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

        // Added By Amit 31-7-2015 VAMRP

        /** Set ReturnLoan amount.
        @param ReturnLoanAmount  */
        public void SetRETURNLOANAMOUNT(Decimal? RETURNLOANAMOUNT)
        {
            if (RETURNLOANAMOUNT == null) throw new ArgumentException("ReturnAmountLoan is mandatory.");
            Set_Value("RETURNLOANAMOUNT", (Decimal?)RETURNLOANAMOUNT);
        }
        /** Get ReturnLoan amount.
        @return Amount being paid */
        public Decimal GetRETURNLOANAMOUNT()
        {
            Object bd = Get_Value("RETURNLOANAMOUNT");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set LOAN    amount.
        @param LOAN  */
        public void SetLOAN(bool LOAN)
        {
            if (LOAN == null) throw new ArgumentException("LOAN is mandatory.");
            Set_Value("LOAN", (bool)LOAN);
        }
        /** Get LOAN amount.
        @return LOAN */
        public Decimal GetLOAN()
        {
            Object bd = Get_Value("LOAN");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }



        //pratap
        /** TransferType AD_Reference_ID=1000295 */
        public static int TRANSFERTYPE_AD_Reference_ID = 1000295;
        /** Cash = CH */
        public static String TRANSFERTYPE_Cash = "CH";
        /** Check = CK */
        public static String TRANSFERTYPE_Check = "CK";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsTransferTypeValid(String test)
        {
            return test == null || test.Equals("CH") || test.Equals("CK");
        }
        /** Set Transfer Type.
        @param TransferType Transfer Type */
        public void SetTransferType(String TransferType)
        {
            if (!IsTransferTypeValid(TransferType))
                throw new ArgumentException("TransferType Invalid value - " + TransferType + " - Reference_ID=1000295 - CH - CK");
            if (TransferType != null && TransferType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                TransferType = TransferType.Substring(0, 2);
            }
            Set_Value("TransferType", TransferType);
        }
        /** Get Transfer Type.
        @return Transfer Type */
        public String GetTransferType()
        {
            return (String)Get_Value("TransferType");
        }

        /** Set Valid Months.
        @param ValidMonths Valid Months */
        public void SetValidMonths(int ValidMonths)
        {
            Set_Value("ValidMonths", ValidMonths);
        }
        /** Get Valid Months.
        @return Valid Months */
        public int GetValidMonths()
        {
            Object ii = Get_Value("ValidMonths");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value != null && Value.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Value = Value.Substring(0, 50);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }

        //Pratap

        /** Set Tax Rate.
        @param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
                Set_Value("C_Tax_ID", C_Tax_ID);
        }
        /** Get Tax Rate.
        @return Tax identifier */
        public int GetC_Tax_ID()
        {
            Object ii = Get_Value("C_Tax_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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


        /** Set Check Date.
        @param CheckDate Check Date */
        public void SetCheckDate(DateTime? CheckDate)
        {
            Set_Value("CheckDate", (DateTime?)CheckDate);
        }
        /** Get Check Date.
        @return Check Date */
        public DateTime? GetCheckDate()
        {
            return (DateTime?)Get_Value("CheckDate");
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

        /** Set Advance Request.
        @param MMFA_AdvanceRequest_ID Advance Request */
        public void SetMMFA_AdvanceRequest_ID(int MMFA_AdvanceRequest_ID)
        {
            if (MMFA_AdvanceRequest_ID <= 0) Set_Value("MMFA_AdvanceRequest_ID", null);
            else
                Set_Value("MMFA_AdvanceRequest_ID", MMFA_AdvanceRequest_ID);
        }
        /** Get Advance Request.
        @return Advance Request */
        public int GetMMFA_AdvanceRequest_ID()
        {
            Object ii = Get_Value("MMFA_AdvanceRequest_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Details.
        @param MMFA_CashDetails Cash Details */
        public void SetMMFA_CashDetails(String MMFA_CashDetails)
        {
            if (MMFA_CashDetails != null && MMFA_CashDetails.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MMFA_CashDetails = MMFA_CashDetails.Substring(0, 1);
            }
            Set_Value("MMFA_CashDetails", MMFA_CashDetails);
        }
        /** Get Cash Details.
        @return Cash Details */
        public String GetMMFA_CashDetails()
        {
            return (String)Get_Value("MMFA_CashDetails");
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

        //Added By Pratap 1/30/16
        /** Set Contra.
        @param VA012_IsContra Contra */
        public void SetVA012_IsContra(Boolean VA012_IsContra)
        {
            Set_Value("VA012_IsContra", VA012_IsContra);
        }
        /** Get Contra.
        @return Contra */
        public Boolean IsVA012_IsContra()
        {
            Object oo = Get_Value("VA012_IsContra");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Reconciled.
        @param VA012_IsReconciled Reconciled */
        public void SetVA012_IsReconciled(Boolean VA012_IsReconciled)
        {
            Set_Value("VA012_IsReconciled", VA012_IsReconciled);
        }
        /** Get Reconciled.
        @return Reconciled */
        public Boolean IsVA012_IsReconciled()
        {
            Object oo = Get_Value("VA012_IsReconciled");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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

        // END
        //Arpit -For cash Journal Account setting on Location tab of Customer/Vendor
        /** Set Location.
@param C_BPartner_Location_ID Identifies the address for this Account/Prospect. */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }/** Get Location.
@return Identifies the address for this Account/Prospect. */
        public int GetC_BPartner_Location_ID() { Object ii = Get_Value("C_BPartner_Location_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        //End Here
        //Added by Manjot suggested by Mukesh sir. reason for adding is need to lock this record while allocation from allocation form
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

        /** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Payment Schedule.
@param VA009_OrderPaySchedule_ID Order Payment Schedule */
        public void SetVA009_OrderPaySchedule_ID(int VA009_OrderPaySchedule_ID)
        {
            if (VA009_OrderPaySchedule_ID <= 0) Set_Value("VA009_OrderPaySchedule_ID", null);
            else
                Set_Value("VA009_OrderPaySchedule_ID", VA009_OrderPaySchedule_ID);
        }/** Get Order Payment Schedule.
@return Order Payment Schedule */
        public int GetVA009_OrderPaySchedule_ID() { Object ii = Get_Value("VA009_OrderPaySchedule_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Prepayment.
@param IsPrepayment The Payment/Receipt is a Prepayment */
        public void SetIsPrepayment(Boolean IsPrepayment) { Set_Value("IsPrepayment", IsPrepayment); }/** Get Prepayment.
@return The Payment/Receipt is a Prepayment */
        public Boolean IsPrepayment() { Object oo = Get_Value("IsPrepayment"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }


    }

}
