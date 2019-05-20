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
    /** Generated Model for C_BankStatementLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_BankStatementLine : PO
    {
        public X_C_BankStatementLine(Context ctx, int C_BankStatementLine_ID, Trx trxName)
            : base(ctx, C_BankStatementLine_ID, trxName)
        {
            /** if (C_BankStatementLine_ID == 0)
            {
            SetC_BankStatementLine_ID (0);
            SetC_BankStatement_ID (0);
            SetC_Currency_ID (0);	// @SQL=SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=@C_BankAccount_ID@
            SetChargeAmt (0.0);
            SetDateAcct (DateTime.Now);	// @StatementDate@
            SetInterestAmt (0.0);
            SetIsManual (true);	// Y
            SetIsReversal (false);
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 FROM C_BankStatementLine WHERE C_BankStatement_ID=@C_BankStatement_ID@
            SetProcessed (false);	// N
            SetStatementLineDate (DateTime.Now);	// @StatementLineDate@
            SetStmtAmt (0.0);
            SetTrxAmt (0.0);
            SetValutaDate (DateTime.Now);	// @StatementDate@
            }
             */
        }
        public X_C_BankStatementLine(Ctx ctx, int C_BankStatementLine_ID, Trx trxName)
            : base(ctx, C_BankStatementLine_ID, trxName)
        {
            /** if (C_BankStatementLine_ID == 0)
            {
            SetC_BankStatementLine_ID (0);
            SetC_BankStatement_ID (0);
            SetC_Currency_ID (0);	// @SQL=SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=@C_BankAccount_ID@
            SetChargeAmt (0.0);
            SetDateAcct (DateTime.Now);	// @StatementDate@
            SetInterestAmt (0.0);
            SetIsManual (true);	// Y
            SetIsReversal (false);
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 FROM C_BankStatementLine WHERE C_BankStatement_ID=@C_BankStatement_ID@
            SetProcessed (false);	// N
            SetStatementLineDate (DateTime.Now);	// @StatementLineDate@
            SetStmtAmt (0.0);
            SetTrxAmt (0.0);
            SetValutaDate (DateTime.Now);	// @StatementDate@
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BankStatementLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BankStatementLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BankStatementLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_BankStatementLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514370871L;
        /** Last Updated Timestamp 7/29/2010 1:07:34 PM */
        public static long updatedMS = 1280389054082L;
        /** AD_Table_ID=393 */
        public static int Table_ID;
        // =393;

        /** TableName=C_BankStatementLine */
        public static String Table_Name = "C_BankStatementLine";

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
            StringBuilder sb = new StringBuilder("X_C_BankStatementLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set Bank statement line.
        @param C_BankStatementLine_ID Line on a statement from this Bank */
        public void SetC_BankStatementLine_ID(int C_BankStatementLine_ID)
        {
            if (C_BankStatementLine_ID < 1) throw new ArgumentException("C_BankStatementLine_ID is mandatory.");
            Set_ValueNoCheck("C_BankStatementLine_ID", C_BankStatementLine_ID);
        }
        /** Get Bank statement line.
        @return Line on a statement from this Bank */
        public int GetC_BankStatementLine_ID()
        {
            Object ii = Get_Value("C_BankStatementLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Statement.
        @param C_BankStatement_ID Bank Statement of account */
        public void SetC_BankStatement_ID(int C_BankStatement_ID)
        {
            if (C_BankStatement_ID < 1) throw new ArgumentException("C_BankStatement_ID is mandatory.");
            Set_ValueNoCheck("C_BankStatement_ID", C_BankStatement_ID);
        }
        /** Get Bank Statement.
        @return Bank Statement of account */
        public int GetC_BankStatement_ID()
        {
            Object ii = Get_Value("C_BankStatement_ID");
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
            if (ChargeAmt == null) throw new ArgumentException("ChargeAmt is mandatory.");
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
        /** Set Create Payment.
        @param CreatePayment Create Payment */
        public void SetCreatePayment(String CreatePayment)
        {
            if (CreatePayment != null && CreatePayment.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreatePayment = CreatePayment.Substring(0, 1);
            }
            Set_Value("CreatePayment", CreatePayment);
        }
        /** Get Create Payment.
        @return Create Payment */
        public String GetCreatePayment()
        {
            return (String)Get_Value("CreatePayment");
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
        /** Set EFT Amount.
        @param EftAmt Electronic Funds Transfer Amount */
        public void SetEftAmt(Decimal? EftAmt)
        {
            Set_Value("EftAmt", (Decimal?)EftAmt);
        }
        /** Get EFT Amount.
        @return Electronic Funds Transfer Amount */
        public Decimal GetEftAmt()
        {
            Object bd = Get_Value("EftAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set EFT Check No.
        @param EftCheckNo Electronic Funds Transfer Check No */
        public void SetEftCheckNo(String EftCheckNo)
        {
            if (EftCheckNo != null && EftCheckNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                EftCheckNo = EftCheckNo.Substring(0, 20);
            }
            Set_Value("EftCheckNo", EftCheckNo);
        }
        /** Get EFT Check No.
        @return Electronic Funds Transfer Check No */
        public String GetEftCheckNo()
        {
            return (String)Get_Value("EftCheckNo");
        }
        /** Set EFT Currency.
        @param EftCurrency Electronic Funds Transfer Currency */
        public void SetEftCurrency(String EftCurrency)
        {
            if (EftCurrency != null && EftCurrency.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                EftCurrency = EftCurrency.Substring(0, 20);
            }
            Set_Value("EftCurrency", EftCurrency);
        }
        /** Get EFT Currency.
        @return Electronic Funds Transfer Currency */
        public String GetEftCurrency()
        {
            return (String)Get_Value("EftCurrency");
        }
        /** Set EFT Memo.
        @param EftMemo Electronic Funds Transfer Memo */
        public void SetEftMemo(String EftMemo)
        {
            if (EftMemo != null && EftMemo.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                EftMemo = EftMemo.Substring(0, 2000);
            }
            Set_Value("EftMemo", EftMemo);
        }
        /** Get EFT Memo.
        @return Electronic Funds Transfer Memo */
        public String GetEftMemo()
        {
            return (String)Get_Value("EftMemo");
        }
        /** Set EFT Payee.
        @param EftPayee Electronic Funds Transfer Payee information */
        public void SetEftPayee(String EftPayee)
        {
            if (EftPayee != null && EftPayee.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                EftPayee = EftPayee.Substring(0, 255);
            }
            Set_Value("EftPayee", EftPayee);
        }
        /** Get EFT Payee.
        @return Electronic Funds Transfer Payee information */
        public String GetEftPayee()
        {
            return (String)Get_Value("EftPayee");
        }
        /** Set EFT Payee Account.
        @param EftPayeeAccount Electronic Funds Transfer Payyee Account Information */
        public void SetEftPayeeAccount(String EftPayeeAccount)
        {
            if (EftPayeeAccount != null && EftPayeeAccount.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                EftPayeeAccount = EftPayeeAccount.Substring(0, 40);
            }
            Set_Value("EftPayeeAccount", EftPayeeAccount);
        }
        /** Get EFT Payee Account.
        @return Electronic Funds Transfer Payyee Account Information */
        public String GetEftPayeeAccount()
        {
            return (String)Get_Value("EftPayeeAccount");
        }
        /** Set EFT Reference.
        @param EftReference Electronic Funds Transfer Reference */
        public void SetEftReference(String EftReference)
        {
            if (EftReference != null && EftReference.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                EftReference = EftReference.Substring(0, 60);
            }
            Set_Value("EftReference", EftReference);
        }
        /** Get EFT Reference.
        @return Electronic Funds Transfer Reference */
        public String GetEftReference()
        {
            return (String)Get_Value("EftReference");
        }
        /** Set EFT Statement Line Date.
        @param EftStatementLineDate Electronic Funds Transfer Statement Line Date */
        public void SetEftStatementLineDate(DateTime? EftStatementLineDate)
        {
            Set_Value("EftStatementLineDate", (DateTime?)EftStatementLineDate);
        }
        /** Get EFT Statement Line Date.
        @return Electronic Funds Transfer Statement Line Date */
        public DateTime? GetEftStatementLineDate()
        {
            return (DateTime?)Get_Value("EftStatementLineDate");
        }
        /** Set EFT Trx ID.
        @param EftTrxID Electronic Funds Transfer Transaction ID */
        public void SetEftTrxID(String EftTrxID)
        {
            if (EftTrxID != null && EftTrxID.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                EftTrxID = EftTrxID.Substring(0, 40);
            }
            Set_Value("EftTrxID", EftTrxID);
        }
        /** Get EFT Trx ID.
        @return Electronic Funds Transfer Transaction ID */
        public String GetEftTrxID()
        {
            return (String)Get_Value("EftTrxID");
        }
        /** Set EFT Trx Type.
        @param EftTrxType Electronic Funds Transfer Transaction Type */
        public void SetEftTrxType(String EftTrxType)
        {
            if (EftTrxType != null && EftTrxType.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                EftTrxType = EftTrxType.Substring(0, 20);
            }
            Set_Value("EftTrxType", EftTrxType);
        }
        /** Get EFT Trx Type.
        @return Electronic Funds Transfer Transaction Type */
        public String GetEftTrxType()
        {
            return (String)Get_Value("EftTrxType");
        }
        /** Set EFT Effective Date.
        @param EftValutaDate Electronic Funds Transfer Valuta (effective) Date */
        public void SetEftValutaDate(DateTime? EftValutaDate)
        {
            Set_Value("EftValutaDate", (DateTime?)EftValutaDate);
        }
        /** Get EFT Effective Date.
        @return Electronic Funds Transfer Valuta (effective) Date */
        public DateTime? GetEftValutaDate()
        {
            return (DateTime?)Get_Value("EftValutaDate");
        }
        /** Set Interest Amount.
        @param InterestAmt Interest Amount */
        public void SetInterestAmt(Decimal? InterestAmt)
        {
            if (InterestAmt == null) throw new ArgumentException("InterestAmt is mandatory.");
            Set_Value("InterestAmt", (Decimal?)InterestAmt);
        }
        /** Get Interest Amount.
        @return Interest Amount */
        public Decimal GetInterestAmt()
        {
            Object bd = Get_Value("InterestAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Manual.
        @param IsManual This is a manual process */
        public void SetIsManual(Boolean IsManual)
        {
            Set_Value("IsManual", IsManual);
        }
        /** Get Manual.
        @return This is a manual process */
        public Boolean IsManual()
        {
            Object oo = Get_Value("IsManual");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Reversal.
        @param IsReversal This is a reversing transaction */
        public void SetIsReversal(Boolean IsReversal)
        {
            Set_Value("IsReversal", IsReversal);
        }
        /** Get Reversal.
        @return This is a reversing transaction */
        public Boolean IsReversal()
        {
            Object oo = Get_Value("IsReversal");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetLine().ToString());
        }
        /** Set Match Statement.
        @param MatchStatement Match Statement */
        public void SetMatchStatement(String MatchStatement)
        {
            if (MatchStatement != null && MatchStatement.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MatchStatement = MatchStatement.Substring(0, 1);
            }
            Set_Value("MatchStatement", MatchStatement);
        }
        /** Get Match Statement.
        @return Match Statement */
        public String GetMatchStatement()
        {
            return (String)Get_Value("MatchStatement");
        }
        /** Set Memo.
        @param Memo Memo Text */
        public void SetMemo(String Memo)
        {
            if (Memo != null && Memo.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Memo = Memo.Substring(0, 255);
            }
            Set_Value("Memo", Memo);
        }
        /** Get Memo.
        @return Memo Text */
        public String GetMemo()
        {
            return (String)Get_Value("Memo");
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
        /** Set Reference No.
        @param ReferenceNo Your customer or vendor number at the Business Partner's site */
        public void SetReferenceNo(String ReferenceNo)
        {
            if (ReferenceNo != null && ReferenceNo.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                ReferenceNo = ReferenceNo.Substring(0, 40);
            }
            Set_Value("ReferenceNo", ReferenceNo);
        }
        /** Get Reference No.
        @return Your customer or vendor number at the Business Partner's site */
        public String GetReferenceNo()
        {
            return (String)Get_Value("ReferenceNo");
        }
        /** Set Statement Line Date.
        @param StatementLineDate Date of the Statement Line */
        public void SetStatementLineDate(DateTime? StatementLineDate)
        {
            if (StatementLineDate == null) throw new ArgumentException("StatementLineDate is mandatory.");
            Set_Value("StatementLineDate", (DateTime?)StatementLineDate);
        }
        /** Get Statement Line Date.
        @return Date of the Statement Line */
        public DateTime? GetStatementLineDate()
        {
            return (DateTime?)Get_Value("StatementLineDate");
        }
        /** Set Statement amount.
        @param StmtAmt Statement Amount */
        public void SetStmtAmt(Decimal? StmtAmt)
        {
            if (StmtAmt == null) throw new ArgumentException("StmtAmt is mandatory.");
            Set_Value("StmtAmt", (Decimal?)StmtAmt);
        }
        /** Get Statement amount.
        @return Statement Amount */
        public Decimal GetStmtAmt()
        {
            Object bd = Get_Value("StmtAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Transaction Amount.
        @param TrxAmt Amount of a transaction */
        public void SetTrxAmt(Decimal? TrxAmt)
        {
            if (TrxAmt == null) throw new ArgumentException("TrxAmt is mandatory.");
            Set_Value("TrxAmt", (Decimal?)TrxAmt);
        }
        /** Get Transaction Amount.
        @return Amount of a transaction */
        public Decimal GetTrxAmt()
        {
            Object bd = Get_Value("TrxAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Effective date.
        @param ValutaDate Date when money is available */
        public void SetValutaDate(DateTime? ValutaDate)
        {
            if (ValutaDate == null) throw new ArgumentException("ValutaDate is mandatory.");
            Set_Value("ValutaDate", (DateTime?)ValutaDate);
        }
        /** Get Effective date.
        @return Date when money is available */
        public DateTime? GetValutaDate()
        {
            return (DateTime?)Get_Value("ValutaDate");
        }

        //added by pratap 16-nov-2015


        /** Set Cash Book.
        @param C_CashBook_ID Cash Book for recording petty cash transactions */
        public void SetC_CashBook_ID(int C_CashBook_ID)
        {
            if (C_CashBook_ID <= 0) Set_Value("C_CashBook_ID", null);
            else
                Set_Value("C_CashBook_ID", C_CashBook_ID);
        }
        /** Get Cash Book.
        @return Cash Book for recording petty cash transactions */
        public int GetC_CashBook_ID()
        {
            Object ii = Get_Value("C_CashBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }



        /** VA012_ContraType AD_Reference_ID=1000447 */
        public static int VA012_CONTRATYPE_AD_Reference_ID = 1000447;
        /** Bank To Bank = BB */
        public static String VA012_CONTRATYPE_BankToBank = "BB";
        /** Cash To Bank = CB */
        public static String VA012_CONTRATYPE_CashToBank = "CB";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA012_ContraTypeValid(String test)
        {
            return test == null || test.Equals("BB") || test.Equals("CB");
        }
        /** Set Contra Type.
        @param VA012_ContraType Contra Type */
        public void SetVA012_ContraType(String VA012_ContraType)
        {
            if (!IsVA012_ContraTypeValid(VA012_ContraType))
                throw new ArgumentException("VA012_ContraType Invalid value - " + VA012_ContraType + " - Reference_ID=1000447 - BB - CB");
            if (VA012_ContraType != null && VA012_ContraType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VA012_ContraType = VA012_ContraType.Substring(0, 2);
            }
            Set_Value("VA012_ContraType", VA012_ContraType);
        }
        /** Get Contra Type.
        @return Contra Type */
        public String GetVA012_ContraType()
        {
            return (String)Get_Value("VA012_ContraType");
        }


        /** VA012_DifferenceType AD_Reference_ID=1000448 */
        public static int VA012_DIFFERENCETYPE_AD_Reference_ID = 1000448;
        /** Charge = CH */
        public static String VA012_DIFFERENCETYPE_Charge = "CH";
        /** Discount Amount = DA */
        public static String VA012_DIFFERENCETYPE_DiscountAmount = "DA";
        /** Over Under Payment = OU */
        public static String VA012_DIFFERENCETYPE_OverUnderPayment = "OU";
        /** Write-off Amount = WO */
        public static String VA012_DIFFERENCETYPE_Write_OffAmount = "WO";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA012_DifferenceTypeValid(String test)
        {
            return test == null || test.Equals("CH") || test.Equals("DA") || test.Equals("OU") || test.Equals("WO");
        }
        /** Set Difference Type.
        @param VA012_DifferenceType Difference Type */
        public void SetVA012_DifferenceType(String VA012_DifferenceType)
        {
            if (!IsVA012_DifferenceTypeValid(VA012_DifferenceType))
                throw new ArgumentException("VA012_DifferenceType Invalid value - " + VA012_DifferenceType + " - Reference_ID=1000448 - CH - DA - OU - WO");
            if (VA012_DifferenceType != null && VA012_DifferenceType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VA012_DifferenceType = VA012_DifferenceType.Substring(0, 2);
            }
            Set_Value("VA012_DifferenceType", VA012_DifferenceType);
        }
        /** Get Difference Type.
        @return Difference Type */
        public String GetVA012_DifferenceType()
        {
            return (String)Get_Value("VA012_DifferenceType");
        }

        /** Set Voucher No.
        @param VA012_VoucherNo Voucher No */
        public void SetVA012_VoucherNo(String VA012_VoucherNo)
        {
            if (VA012_VoucherNo != null && VA012_VoucherNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VA012_VoucherNo = VA012_VoucherNo.Substring(0, 50);
            }
            Set_Value("VA012_VoucherNo", VA012_VoucherNo);
        }
        /** Get Voucher No.
        @return Voucher No */
        public String GetVA012_VoucherNo()
        {
            return (String)Get_Value("VA012_VoucherNo");
        }


        //added by pratap 1-oct-2015

        /** Set Page.
        @param VA012_Page Page */
        public void SetVA012_Page(int VA012_Page)
        {
            Set_Value("VA012_Page", VA012_Page);
        }
        /** Get Page.
        @return Page */
        public int GetVA012_Page()
        {
            Object ii = Get_Value("VA012_Page");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
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
        /** Set Use Next Time.
            @param VA012_IsUseNextTime Use Next Time */
        public void SetVA012_IsUseNextTime(Boolean VA012_IsUseNextTime)
        {
            Set_Value("VA012_IsUseNextTime", VA012_IsUseNextTime);
        }
        /** Get Use Next Time.
        @return Use Next Time */
        public Boolean IsVA012_IsUseNextTime()
        {
            Object oo = Get_Value("VA012_IsUseNextTime");
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
        //

        /** Set Matching Confirmed.
           @param VA012_IsMatchingConfirmed Matching Confirmed */
        public void SetVA012_IsMatchingConfirmed(Boolean VA012_IsMatchingConfirmed)
        {
            Set_Value("VA012_IsMatchingConfirmed", VA012_IsMatchingConfirmed);
        }
        /** Get Matching Confirmed.
        @return Matching Confirmed */
        public Boolean IsVA012_IsMatchingConfirmed()
        {
            Object oo = Get_Value("VA012_IsMatchingConfirmed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** VA012_VoucherType AD_Reference_ID=1000421 */
        public static int VA012_VOUCHERTYPE_AD_Reference_ID = 1000421;
        /** Match = M */
        public static String VA012_VOUCHERTYPE_Match = "M";
        /** Voucher = V */
        public static String VA012_VOUCHERTYPE_Voucher = "V";
        /** Voucher = C */
        public static String VA012_VOUCHERTYPE_Contra = "C";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA012_VoucherTypeValid(String test)
        {
            return test == null || test.Equals("M") || test.Equals("V") || test.Equals("C");
        }
        /** Set Voucher Type.
            @param VA012_VoucherType Voucher Type */
        public void SetVA012_VoucherType(String VA012_VoucherType)
        {
            if (!IsVA012_VoucherTypeValid(VA012_VoucherType))
                throw new ArgumentException("VA012_VoucherType Invalid value - " + VA012_VoucherType + " - Reference_ID=1000421 - M - V - C");
            if (VA012_VoucherType != null && VA012_VoucherType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VA012_VoucherType = VA012_VoucherType.Substring(0, 1);
            }
            Set_Value("VA012_VoucherType", VA012_VoucherType);
        }
        /** Get Voucher Type.
        @return Voucher Type */
        public String GetVA012_VoucherType()
        {
            return (String)Get_Value("VA012_VoucherType");
        }
        /** Set Order.
        @param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Sales Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        //Added By Pratap 1/30/16
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



    }

}
