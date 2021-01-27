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
    /** Generated Model for VAB_CashJRNLLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_CashJRNLLine : PO
    {
        public X_VAB_CashJRNLLine(Context ctx, int VAB_CashJRNLLine_ID, Trx trxName)
            : base(ctx, VAB_CashJRNLLine_ID, trxName)
        {
            /** if (VAB_CashJRNLLine_ID == 0)
            {
            SetAmount (0.0);
            SetVAB_CashJRNLLine_ID (0);
            SetVAB_CashJRNL_ID (0);
            SetCashType (null);	// E
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAB_CashJRNLLine WHERE VAB_CashJRNL_ID=@VAB_CashJRNL_ID@
            SetProcessed (false);	// N
            }
             */
        }
        public X_VAB_CashJRNLLine(Ctx ctx, int VAB_CashJRNLLine_ID, Trx trxName)
            : base(ctx, VAB_CashJRNLLine_ID, trxName)
        {
            /** if (VAB_CashJRNLLine_ID == 0)
            {
            SetAmount (0.0);
            SetVAB_CashJRNLLine_ID (0);
            SetVAB_CashJRNL_ID (0);
            SetCashType (null);	// E
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAB_CashJRNLLine WHERE VAB_CashJRNL_ID=@VAB_CashJRNL_ID@
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_CashJRNLLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_CashJRNLLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_CashJRNLLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_CashJRNLLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27633016154099L;
        /** Last Updated Timestamp 10/22/2012 12:57:17 PM */
        public static long updatedMS = 1350890837310L;
        /** VAF_TableView_ID=410 */
        public static int Table_ID;
        // =410;

        /** TableName=VAB_CashJRNLLine */
        public static String Table_Name = "VAB_CashJRNLLine";

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
            StringBuilder sb = new StringBuilder("X_VAB_CashJRNLLine[").Append(Get_ID()).Append("]");
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
        @param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
            else
                Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Customer/Prospect */
        public int GetVAB_BusinessPartner_ID()
        {
            Object ii = Get_Value("VAB_BusinessPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Account.
        @param VAB_Bank_Acct_ID Account at the Bank */
        public void SetVAB_Bank_Acct_ID(int VAB_Bank_Acct_ID)
        {
            if (VAB_Bank_Acct_ID <= 0) Set_Value("VAB_Bank_Acct_ID", null);
            else
                Set_Value("VAB_Bank_Acct_ID", VAB_Bank_Acct_ID);
        }
        /** Get Bank Account.
        @return Account at the Bank */
        public int GetVAB_Bank_Acct_ID()
        {
            Object ii = Get_Value("VAB_Bank_Acct_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Journal Line.
        @param VAB_CashJRNLLine_ID Cash Journal Line */
        public void SetVAB_CashJRNLLine_ID(int VAB_CashJRNLLine_ID)
        {
            if (VAB_CashJRNLLine_ID < 1) throw new ArgumentException("VAB_CashJRNLLine_ID is mandatory.");
            Set_ValueNoCheck("VAB_CashJRNLLine_ID", VAB_CashJRNLLine_ID);
        }
        /** Get Cash Journal Line.
        @return Cash Journal Line */
        public int GetVAB_CashJRNLLine_ID()
        {
            Object ii = Get_Value("VAB_CashJRNLLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Journal.
        @param VAB_CashJRNL_ID Cash Journal */
        public void SetVAB_CashJRNL_ID(int VAB_CashJRNL_ID)
        {
            if (VAB_CashJRNL_ID < 1) throw new ArgumentException("VAB_CashJRNL_ID is mandatory.");
            Set_ValueNoCheck("VAB_CashJRNL_ID", VAB_CashJRNL_ID);
        }
        /** Get Cash Journal.
        @return Cash Journal */
        public int GetVAB_CashJRNL_ID()
        {
            Object ii = Get_Value("VAB_CashJRNL_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetVAB_CashJRNL_ID().ToString());
        }
        /** Set Charge.
        @param VAB_Charge_ID Additional document charges */
        public void SetVAB_Charge_ID(int VAB_Charge_ID)
        {
            if (VAB_Charge_ID <= 0) Set_Value("VAB_Charge_ID", null);
            else
                Set_Value("VAB_Charge_ID", VAB_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetVAB_Charge_ID()
        {
            Object ii = Get_Value("VAB_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param VAB_Currency_ID The Currency for this record */
        public void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            if (VAB_Currency_ID <= 0) Set_ValueNoCheck("VAB_Currency_ID", null);
            else
                Set_ValueNoCheck("VAB_Currency_ID", VAB_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetVAB_Currency_ID()
        {
            Object ii = Get_Value("VAB_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param VAB_Invoice_ID Invoice Identifier */
        public void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            if (VAB_Invoice_ID <= 0) Set_ValueNoCheck("VAB_Invoice_ID", null);
            else
                Set_ValueNoCheck("VAB_Invoice_ID", VAB_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetVAB_Invoice_ID()
        {
            Object ii = Get_Value("VAB_Invoice_ID");
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

        /** CashType VAF_Control_Ref_ID=217 */
        public static int CASHTYPE_VAF_Control_Ref_ID = 217;
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
        /** General Receipts = R */
        public static String CASHTYPE_GeneralReceipts = "R";
        /** Bank Account Transfer = T */
        public static String CASHTYPE_BankAccountTransfer = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCashTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("E") || test.Equals("F") || test.Equals("I") || test.Equals("R") || test.Equals("T");
        }
        /** Set Cash Type.
        @param CashType Source of Cash */
        public void SetCashType(String CashType)
        {
            if (CashType == null) throw new ArgumentException("CashType is mandatory");
            if (!IsCashTypeValid(CashType))
                throw new ArgumentException("CashType Invalid value - " + CashType + " - Reference_ID=217 - A - B - C - D - E - F - I - R - T");
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

        /** VSS_PAYMENTTYPE VAF_Control_Ref_ID=1000108 */
        public static int VSS_PAYMENTTYPE_VAF_Control_Ref_ID = 1000108;
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


        /** Set VAB_CashBook_ID.
       @param VAB_CashBook_ID for this record */
        public void SetVAB_CashBook_ID(int VAB_CashBook_ID)
        {
            if (VAB_CashBook_ID <= 0) Set_ValueNoCheck("VAB_CashBook_ID", null);
            else
                Set_ValueNoCheck("VAB_CashBook_ID", VAB_CashBook_ID);
        }
        /** Get VAB_CashBook_ID.
        @return The VAB_CashBook_ID for this record */
        public int GetVAB_CashBook_ID()
        {
            Object ii = Get_Value("VAB_CashBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }



        /** VAB_CashJRNLLine_ID_1 VAF_Control_Ref_ID=1000168 */
        public static int VAB_CASHJRNLLINE_ID_1_VAF_Control_Ref_ID = 1000168;
        /** Set Cash Journal Transection.
        @param VAB_CashJRNLLine_ID_1 Cash Journal Transection */
        public void SetVAB_CashJRNLLine_ID_1(int VAB_CashJRNLLine_ID_1)
        {
            Set_Value("VAB_CashJRNLLine_ID_1", VAB_CashJRNLLine_ID_1);
        }
        /** Get Cash Journal Transection.
        @return Cash Journal Transection */
        public int GetVAB_CashJRNLLine_ID_1()
        {
            Object ii = Get_Value("VAB_CashJRNLLine_ID_1");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Payment Schedule.
@param VAB_sched_InvoicePayment_ID Invoice Payment Schedule */
        public void SetVAB_sched_InvoicePayment_ID(int VAB_sched_InvoicePayment_ID)
        {
            if (VAB_sched_InvoicePayment_ID <= 0) Set_Value("VAB_sched_InvoicePayment_ID", null);
            else
                Set_Value("VAB_sched_InvoicePayment_ID", VAB_sched_InvoicePayment_ID);
        }
        /** Get Invoice Payment Schedule.
        @return Invoice Payment Schedule */
        public int GetVAB_sched_InvoicePayment_ID()
        {
            Object ii = Get_Value("VAB_sched_InvoicePayment_ID");
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
        /** TransferType VAF_Control_Ref_ID=1000295 */
        public static int TRANSFERTYPE_VAF_Control_Ref_ID = 1000295;
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
        @param VAB_TaxRate_ID Tax identifier */
        public void SetVAB_TaxRate_ID(int VAB_TaxRate_ID)
        {
            if (VAB_TaxRate_ID <= 0) Set_Value("VAB_TaxRate_ID", null);
            else
                Set_Value("VAB_TaxRate_ID", VAB_TaxRate_ID);
        }
        /** Get Tax Rate.
        @return Tax identifier */
        public int GetVAB_TaxRate_ID()
        {
            Object ii = Get_Value("VAB_TaxRate_ID");
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
        @param VAB_CurrencyType_ID Currency Conversion Rate Type */
        public void SetVAB_CurrencyType_ID(int VAB_CurrencyType_ID)
        {
            if (VAB_CurrencyType_ID <= 0) Set_Value("VAB_CurrencyType_ID", null);
            else
                Set_Value("VAB_CurrencyType_ID", VAB_CurrencyType_ID);
        }
        /** Get Currency Type.
        @return Currency Conversion Rate Type */
        public int GetVAB_CurrencyType_ID()
        {
            Object ii = Get_Value("VAB_CurrencyType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        // END
        //Arpit -For cash Journal Account setting on Location tab of Customer/Vendor
        /** Set Location.
@param VAB_BPart_Location_ID Identifies the address for this Account/Prospect. */
        public void SetVAB_BPart_Location_ID(int VAB_BPart_Location_ID)
        {
            if (VAB_BPart_Location_ID <= 0) Set_Value("VAB_BPart_Location_ID", null);
            else
                Set_Value("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
        }/** Get Location.
@return Identifies the address for this Account/Prospect. */
        public int GetVAB_BPart_Location_ID() { Object ii = Get_Value("VAB_BPart_Location_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
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

    }

}
