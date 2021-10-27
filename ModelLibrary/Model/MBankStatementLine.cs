/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MBankStatementLine
    * Purpose        : Bank Statement Line Model open on button click(it is Form)
    * Class Used     : X_C_BankStatementLine
    * Chronological    Development
    * Raghunandan     01-Aug-2009
******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MBankStatementLine : X_C_BankStatementLine
    {

        private decimal old_ebAmt;
        private decimal old_sdAmt;
        private decimal new_ebAmt;
        private decimal new_sdAmt;
        private MBankStatement _parent;

        private bool resetAmtDim = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_BankStatementLine_ID">Id</param>
        /// <param name="trxName">Transaction</param>
        public MBankStatementLine(Ctx ctx, int C_BankStatementLine_ID, Trx trxName)
            : base(ctx, C_BankStatementLine_ID, trxName)
        {

            if (C_BankStatementLine_ID == 0)
            {
                /*/	setC_BankStatement_ID (0);		//	Parent
                //	setC_Charge_ID (0);
                //	setC_Currency_ID (0);	//	Bank Acct Currency
                //	setLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_BankStatementLine WHERE C_BankStatement_ID=@C_BankStatement_ID@*/
                SetStmtAmt(Env.ZERO);
                SetTrxAmt(Env.ZERO);
                SetInterestAmt(Env.ZERO);
                SetChargeAmt(Env.ZERO);
                SetIsReversal(false);
                /*/	setValutaDate (new DateTime(System.currentTimeMillis()));	// @StatementDate@
                //	setDateAcct (new DateTime(System.currentTimeMillis()));	// @StatementDate@*/
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="dr">Datarow</param>
        /// <param name="trxName">Transaction</param>
        public MBankStatementLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="statement">Bank Statement that the line is part of</param>
        public MBankStatementLine(MBankStatement statement)
            : this(statement.GetCtx(), 0, statement.Get_TrxName())
        {
            SetClientOrg(statement);
            SetC_BankStatement_ID(statement.GetC_BankStatement_ID());
            SetStatementLineDate(statement.GetStatementDate());
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="statement">Bank Statement that the line is part of</param>
        /// <param name="lineNo">position of the line within the statement</param>
        public MBankStatementLine(MBankStatement statement, int lineNo)
            : this(statement)
        {
            SetLine(lineNo);
        }

        /// <summary>
        /// Set Payment
        /// </summary>
        /// <param name="payment">Payment</param>
        public void SetPayment(MPayment payment)
        {
            SetC_Payment_ID(payment.GetC_Payment_ID());
            SetC_Currency_ID(payment.GetC_Currency_ID());
            Decimal amt = payment.GetPayAmt(true);
            SetTrxAmt(amt);
            SetStmtAmt(amt);
            SetDescription(payment.GetDescription());
        }

        /// <summary>
        /// Set Statement Line 
        /// </summary>
        /// <param name="payment">Payment</param>
        /// <returns>Message</returns>
        public string SetPayments(MPayment payment)
        {
            SetC_Payment_ID(payment.GetC_Payment_ID());
            //SetC_Currency_ID(payment.GetC_Currency_ID());
            Decimal amt = payment.GetPayAmt(true);
            //Currency Conversion to BankStatementLine Currency
            if (GetC_Currency_ID() != payment.GetC_Currency_ID())
            {
                int conversionType = Get_ColumnIndex("C_ConversionType_ID") > 0 ? Get_ValueAsInt("C_ConversionType_ID") : payment.GetC_ConversionType_ID();
                amt = MConversionRate.Convert(GetCtx(), amt, payment.GetC_Currency_ID(), GetC_Currency_ID(), GetValutaDate(), conversionType, GetAD_Client_ID(), GetAD_Org_ID());
            }
            if (amt != 0)
            {
                SetTrxAmt(amt);
                if (GetStmtAmt() == 0)
                {
                    SetStmtAmt(amt);
                }
                else // Incase of Uncociled StatementLine have existing Amount
                {
                    decimal _diffAmt = GetStmtAmt() - GetTrxAmt();
                    SetChargeAmt(_diffAmt - GetInterestAmt());
                }
                SetDescription(payment.GetDescription());
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "NoCurrencyConversion");
            }
            //Set VA012_VoucherType
            if (Env.IsModuleInstalled("VA012_"))
            {
                bool _IsPaymentAllocate = false;
                int _InvCount;
                //When Payment have Multiple InvoicePaySchedules
                if (payment.Get_ColumnIndex("IsPaymentAllocate") > 0)
                {
                    _IsPaymentAllocate = Util.GetValueOfBool(payment.Get_Value("IsPaymentAllocate"));
                }
                if (_IsPaymentAllocate)
                {
                    _InvCount = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_PaymentAllocate_ID) FROM C_PaymentAllocate WHERE IsActive='Y' AND C_Payment_ID=" + payment.GetC_Payment_ID(), null, null));
                }
                else
                {
                    _InvCount = payment.GetC_Invoice_ID();
                }
                if (_InvCount > 0 || payment.GetC_Order_ID() > 0)
                {
                    SetVA012_VoucherType("M"); // 'M' indicates VA012_VoucherType as Payment
                }
                else if (payment.GetC_Charge_ID() > 0)
                {
                    SetVA012_VoucherType("V"); // 'V' indicates VA012_VoucherType as Voucher
                }
            }
            return "";
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }

        /// <summary>
        /// Set Statement Line Date and all other dates (Valuta, Acct)
        /// </summary>
        /// <param name="StatementLineDate">Date</param>
        public new void SetStatementLineDate(DateTime? StatementLineDate)
        {
            base.SetStatementLineDate(StatementLineDate);
            SetValutaDate(StatementLineDate);
            SetDateAcct(StatementLineDate);
        }

        /// <summary>
        /// Set StatementLineDate - Callout
        /// </summary>
        /// <param name="oldStatementLineDate">old</param>
        /// <param name="newStatementLineDate">new</param>
        /// <param name="windowNo"window no></param>
        /// @UICallout
        public void SetStatementLineDate(String oldStatementLineDate, String newStatementLineDate, int windowNo)
        {
            if (newStatementLineDate == null || newStatementLineDate.Length == 0)
            {
                return;
            }
            DateTime statementLineDate = Convert.ToDateTime(PO.ConvertToTimestamp(newStatementLineDate));
            if (statementLineDate == null)
            {
                return;
            }
            SetStatementLineDate(statementLineDate);
        }

        /// <summary>
        /// Set ChargeAmt - Callout
        /// </summary>
        /// <param name="oldChargeAmt">old</param>
        /// <param name="newChargeAmt">new</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetChargeAmt(String oldChargeAmt, String newChargeAmt, int windowNo)
        {
            if (newChargeAmt == null || newChargeAmt.Length == 0)
            {
                return;
            }
            Decimal ChargeAmt = Convert.ToDecimal(newChargeAmt);
            base.SetChargeAmt(ChargeAmt);
            SetAmt(windowNo, "ChargeAmt");
        }

        /// <summary>
        /// 	Set InterestAmt - Callout
        /// </summary>
        /// <param name="oldInterestAmt">old</param>
        /// <param name="newInterestAmt">new</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetInterestAmt(String oldInterestAmt, String newInterestAmt, int windowNo)
        {
            if (newInterestAmt == null || newInterestAmt.Length == 0)
            {
                return;
            }
            Decimal InterestAmt = Convert.ToDecimal(newInterestAmt);
            base.SetInterestAmt(InterestAmt);
            SetAmt(windowNo, "InterestAmt");
        }

        /// <summary>
        /// Set StmtAmt - Callout
        /// </summary>
        /// <param name="oldStmtAmt">old</param>
        /// <param name="newStmtAmt">new</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetStmtAmt(String oldStmtAmt, String newStmtAmt, int windowNo)
        {
            if (newStmtAmt == null || newStmtAmt.Length == 0)
            {
                return;
            }
            Decimal StmtAmt = Convert.ToDecimal(newStmtAmt);
            base.SetStmtAmt(StmtAmt);
            SetAmt(windowNo, "StmtAmt");
        }

        /// <summary>
        /// Set Charge/Interest Amount
        /// </summary>
        /// <param name="windowNo">window no</param>
        /// <param name="columnName">callout source</param>
        private void SetAmt(int windowNo, String columnName)
        {
            Decimal stmt = GetStmtAmt();
            //if (stmt == null)
            //{
            //    stmt = Env.ZERO;
            //}
            Decimal trx = GetTrxAmt();
            //if (trx == null)
            //{
            //    trx = Env.ZERO;
            //}
            Decimal bd = Decimal.Subtract(stmt, trx);

            //  Charge - calculate Interest
            if (columnName.Equals("ChargeAmt"))
            {
                Decimal charge = GetChargeAmt();
                //if (charge == null)
                //{
                //    charge = Env.ZERO;
                //}
                bd = Decimal.Subtract(bd, charge);
                //log.trace(log.l5_DData, "Interest (" + bd + ") = Stmt(" + stmt + ") - Trx(" + trx + ") - Charge(" + charge + ")");
                SetInterestAmt(bd);
            }
            //  Calculate Charge
            else
            {
                Decimal interest = GetInterestAmt();
                //if (interest == null)
                //{
                //    interest = Env.ZERO;
                //}
                bd = Decimal.Subtract(bd, interest);
                //log.trace(log.l5_DData, "Charge (" + bd + ") = Stmt(" + stmt + ") - Trx(" + trx + ") - Interest(" + interest + ")");
                SetChargeAmt(bd);
            }
        }


        /// <summary>
        /// Set Payment - Callout.
        /// </summary>
        /// <param name="oldC_Payment_ID">old ID</param>
        /// <param name="newC_Payment_ID">new ID</param>
        /// <param name="windowNo">window</param>
        /// @UICallout
        public void SetC_Payment_ID(String oldC_Payment_ID, String newC_Payment_ID, int windowNo)
        {
            if (newC_Payment_ID == null || newC_Payment_ID.Length == 0)
            {
                return;
            }
            int C_Payment_ID = int.Parse(newC_Payment_ID);
            if (C_Payment_ID == 0)
            {
                return;
            }
            SetC_Payment_ID(C_Payment_ID);
            Decimal stmt = GetStmtAmt();
            if (stmt == null)
            {
                stmt = Env.ZERO;
            }
            String sql = "SELECT PayAmt FROM C_Payment_v WHERE C_Payment_ID=@C_Payment_ID";		//	1
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_Payment_ID", C_Payment_ID);
                idr = DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// if (dr.next())
                {
                    Decimal bd = Util.GetValueOfDecimal(dr[0]);//.getBigDecimal(1);
                    SetTrxAmt(bd);
                    if (Env.Signum(stmt) == 0)
                    {
                        SetStmtAmt(bd);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            //  Recalculate Amounts
            SetAmt(windowNo, "C_Payment_ID");
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (Env.Signum(GetChargeAmt()) != 0 && GetC_Charge_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "C_Charge_ID"));
                // ErrorLog.FillErrorLog("FillMandatory", GetMessage.Translate(GetCtx(), "C_Charge_ID"), "", VAdvantage.Framework.Message.MessageType.ERROR);
                return false;
            }

            // VIS0060: if no period found for statement date, then give message.
            MPeriod period = MPeriod.Get(GetCtx(), GetStatementLineDate(), GetAD_Org_ID());
            if (period == null)
            {
                log.SaveError("PeriodNotValid", "");
                return false;
            }

            //	Set Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_BankStatementLine WHERE C_BankStatement_ID=@param1";
                int ii = Convert.ToInt32(DB.GetSQLValue(Get_TrxName(), sql, GetC_BankStatement_ID()));
                SetLine(ii);
            }

            //	Set References
            if (GetC_Payment_ID() != 0 && GetC_BPartner_ID() == 0)
            {
                MPayment payment = new MPayment(GetCtx(), GetC_Payment_ID(), Get_TrxName());
                SetC_BPartner_ID(payment.GetC_BPartner_ID());
                if (payment.GetC_Invoice_ID() != 0)
                {
                    SetC_Invoice_ID(payment.GetC_Invoice_ID());
                }
            }
            if (GetC_Invoice_ID() != 0 && GetC_BPartner_ID() == 0)
            {
                MInvoice invoice = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
                SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            }
            //Set BPartner_ID when having Order_ID
            if (Env.IsModuleInstalled("VA012_"))
            {
                if (GetC_Order_ID() != 0 && GetC_BPartner_ID() == 0)
                {
                    int _bpartner_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BPartner_ID FROM C_Order WHERE IsActive='Y' AND C_Order_ID=" + GetC_Order_ID(), null, Get_Trx()));
                    SetC_BPartner_ID(_bpartner_ID);
                }
            }
            //	Calculate Charge = Statement - trx - Interest  
            Decimal amt = GetStmtAmt();
            amt = Decimal.Subtract(amt, GetTrxAmt());
            //amt = Decimal.Subtract(amt, GetInterestAmt());     //Commented By Pratap to Exclude Interest 18-11-2015
            if (amt.CompareTo(GetChargeAmt()) != 0)
            {
                SetChargeAmt(amt);
            }

            // Reset Amount Dimension if Trx Amount is different
            if (Util.GetValueOfInt(Get_Value("AmtDimTrxAmount")) > 0)
            {
                string qry = "SELECT Amount FROM C_DimAmt WHERE C_DimAmt_ID=" + Util.GetValueOfInt(Get_Value("AmtDimTrxAmount"));
                decimal amtdimAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(qry, null, Get_TrxName()));

                if (amtdimAmt != GetTrxAmt())
                {
                    resetAmtDim = true;
                    Set_Value("AmtDimTrxAmount", null);
                }
            }

            // Reset Amount Dimension if charge Amount is different
            if (Util.GetValueOfInt(Get_Value("AmtDimChargeAmount")) > 0)
            {
                string qry = "SELECT Amount FROM C_DimAmt WHERE C_DimAmt_ID=" + Util.GetValueOfInt(Get_Value("AmtDimChargeAmount"));
                decimal amtdimAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(qry, null, Get_TrxName()));

                if (amtdimAmt != GetChargeAmt())
                {
                    resetAmtDim = true;
                    Set_Value("AmtDimChargeAmount", null);
                }
            }
            //Rakesh(VA228):Check tendertype column
            if (Get_ColumnIndex("TenderType") >= 0)
            {
                if (!string.IsNullOrEmpty(Util.GetValueOfString(Get_Value("TenderType"))) && Get_Value("TenderType").Equals("K"))
                {
                    //Rakesh:Get autocheckcontrol on 28/Sep/2021 assigned by Amit
                    string autoCheckControl = Util.GetValueOfString(DB.ExecuteScalar("SELECT B.ChkNoAutoControl FROM C_BankStatement S " +
                        "INNER JOIN C_BankAccount B ON B.C_BankAccount_ID=S.C_BankAccount_ID WHERE S.C_BankStatement_ID=" + GetC_BankStatement_ID(), null, Get_TrxName()));
                    //When check number not autocontrolled for selected bank account then make it manadatory
                    if (autoCheckControl.Equals("N"))
                    {
                        if (String.IsNullOrEmpty(GetEftCheckNo()))
                        {
                            log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "EftCheckNo"));
                            return false;
                        }
                        if (!GetEftValutaDate().HasValue)
                        {
                            log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "EftValutaDate"));
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            bool updateBS = UpdateBSAndLine();

            //JID_1325: On save on bank statment system should give warning message "Payment and charge reference not found. Either create payment or the system will not complete bank statement."
            if (GetC_Payment_ID() == 0 && GetC_Charge_ID() == 0)
            {
                log.SaveWarning("VIS_NoPaymentorChargeStatement", "");
            }

            return updateBS;
        }

        //private bool UpdateBSAndLine()
        public bool UpdateBSAndLine(bool DeletingLinesFromHeader = false)
        {
            // Update Bank Statement
            //bool DeletingLinesFromHeader_ = DeletingLinesFromHeader;
            UpdateHeader(DeletingLinesFromHeader);

            // Update BankAccount and BankAccountLine
            MBankStatement parent = GetParent();
            MBankAccount bankAccount = new MBankAccount(GetCtx(), parent.GetC_BankAccount_ID(), Get_TrxName());

            // bankAccount.SetUnMatchedBalance(Decimal.Add(Decimal.Subtract(bankAccount.GetUnMatchedBalance(), old_ebAmt), new_ebAmt)); //Commented Arpit..To updated only if document gets completed..asked by Ashish
            if (!bankAccount.Save(Get_TrxName()))
            {
                log.Warning("Cannot update unmatched balance.");
                return false;
            }

            DataTable dtBankAccountLine;
            int C_BANKACCOUNTLINE_ID = 0;

            string sql = "SELECT C_BANKACCOUNTLINE_ID FROM C_BANKACCOUNTLINE WHERE C_BANKACCOUNT_ID="
                            + bankAccount.GetC_BankAccount_ID() + " AND STATEMENTDATE="
                            + DB.TO_DATE(GetStatementLineDate()) + " AND AD_ORG_ID=" + GetAD_Org_ID();

            dtBankAccountLine = DB.ExecuteDataset(sql, null, Get_TrxName()).Tables[0];

            if (dtBankAccountLine.Rows.Count > 0)
            {
                C_BANKACCOUNTLINE_ID = Util.GetValueOfInt(dtBankAccountLine.Rows[0][0]);
            }

            MBankAccountLine bankAccountLine = new MBankAccountLine(GetCtx(), C_BANKACCOUNTLINE_ID, Get_TrxName());
            if (C_BANKACCOUNTLINE_ID == 0)
            {
                bankAccountLine.SetC_BankAccount_ID(bankAccount.GetC_BankAccount_ID());
                //Arpit To set same orgnization as Bank Statement on Account Line
                //bankAccountLine.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                //bankAccountLine.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                bankAccountLine.SetAD_Org_ID(parent.GetAD_Org_ID());
                bankAccountLine.SetAD_Client_ID(parent.GetAD_Client_ID());
                bankAccountLine.SetEndingBalance(
                        Decimal.Add(Decimal.Add(Decimal.Subtract(bankAccountLine.GetEndingBalance(), old_ebAmt), new_ebAmt), bankAccount.GetCurrentBalance()));

            }
            else
            {
                bankAccountLine.SetEndingBalance(Decimal.Add(Decimal.Subtract(bankAccountLine.GetEndingBalance(), old_ebAmt), new_ebAmt));
            }
            bankAccountLine.SetStatementDate(GetStatementLineDate());
            bankAccountLine.SetStatementDifference(Decimal.Add(Decimal.Subtract(bankAccountLine.GetStatementDifference(), old_sdAmt), new_sdAmt));
            //bankAccountLine.SetEndingBalance(Decimal.Add(Decimal.Subtract(bankAccountLine.GetEndingBalance(), old_ebAmt), new_ebAmt)); //Arpit commented because Ending Balance Already updated in above Lines

            if (!bankAccountLine.Save(Get_TrxName()))
            {
                log.Warning("Cannot create/update bank account line.");
                return false;
            }

            return true;
        }

        /**
        * 	Get BankAccount (parent)
        *	@return BankAccount
        */
        public MBankStatement GetParent()
        {
            if (_parent == null)
                _parent = new MBankStatement(GetCtx(), GetC_BankStatement_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            //UpdateHeader();
            //return success;
            //Arpit
            if (!success)
                return success;
            return UpdateBSAndLine(false);
            //End Here
        }

        /// <summary>
        /// Update Header
        /// </summary>
        private void UpdateHeader(bool DeletingLinesFromHeader = false)
        {
            // Statement difference and ending balance before update in bank statement.
            DataTable dtOldValues = GetCurrentAmounts();

            if (dtOldValues.Rows.Count > 0)
            {
                old_ebAmt = Util.GetValueOfDecimal(dtOldValues.Rows[0][0]);
                old_sdAmt = Util.GetValueOfDecimal(dtOldValues.Rows[0][1]);
            }
            try
            {
                StringBuilder _Sql = new StringBuilder();
                if (DeletingLinesFromHeader)
                {
                    //Commented the below query
                    // _Sql.Append("UPDATE C_BankStatement bs"
                    //+ " SET StatementDifference=(SELECT COALESCE( SUM(StmtAmt), 0 ) FROM C_BankStatementLine bsl "
                    //    + "WHERE bsl.C_BankStatement_ID=bs.C_BankStatement_ID AND bsl.IsActive='Y' AND bsl.C_BankStatementLine_ID=" + GetC_BankStatementLine_ID()+ ") "
                    //+ "WHERE C_BankStatement_ID=" + GetC_BankStatement_ID());
                    //as per requirement not updating StatementDate as per DateAcct
                    _Sql.Append("UPDATE C_BankStatement bs"
                  + " SET StatementDifference=(StatementDifference-(SELECT COALESCE( SUM(StmtAmt), 0 ) FROM C_BankStatementLine bsl "
                  + " WHERE bsl.C_BankStatement_ID=bs.C_BankStatement_ID AND bsl.IsActive='Y' AND bsl.C_BankStatementLine_ID=" + GetC_BankStatementLine_ID() + ")) "
                  //+ ", StatementDate=(SELECT MAX(DateAcct) FROM C_BankStatementLine WHERE IsActive = 'Y' AND C_BankStatement_ID = " + GetC_BankStatement_ID()+")"
                  + " WHERE C_BankStatement_ID=" + GetC_BankStatement_ID());

                }
                else
                {
                    _Sql.Append("UPDATE C_BankStatement bs"
                   + " SET StatementDifference=(SELECT COALESCE( SUM(StmtAmt), 0 ) FROM C_BankStatementLine bsl "
                       + "WHERE bsl.C_BankStatement_ID=bs.C_BankStatement_ID AND bsl.IsActive='Y') "
                   //+ ", StatementDate=(SELECT MAX(DateAcct) FROM C_BankStatementLine WHERE IsActive = 'Y' AND C_BankStatement_ID = " + GetC_BankStatement_ID() + ")"
                   + "WHERE C_BankStatement_ID=" + GetC_BankStatement_ID());
                }

                DB.ExecuteQuery(_Sql.ToString(), null, Get_TrxName());

                _Sql.Clear();

                _Sql.Append("UPDATE C_BankStatement bs"
                          + " SET EndingBalance=BeginningBalance+StatementDifference "
                          + "WHERE C_BankStatement_ID=" + GetC_BankStatement_ID());

                //Commented the below query
                //sql = "UPDATE C_BankStatement bs"
                //    + " SET EndingBalance=BeginningBalance+StatementDifference "
                //    + "WHERE C_BankStatement_ID=" + GetC_BankStatement_ID();
                //DB.executeUpdate(sql, get_TrxName());
                //DB.ExecuteQuery(sql, null, Get_TrxName());

                DB.ExecuteQuery(_Sql.ToString(), null, Get_TrxName());
                // Statement difference and ending balance after update in bank statement.
                DataTable dtNewValues = GetCurrentAmounts();

                if (dtOldValues.Rows.Count > 0)
                {
                    new_ebAmt = Util.GetValueOfDecimal(dtNewValues.Rows[0][0]);
                    new_sdAmt = Util.GetValueOfDecimal(dtNewValues.Rows[0][1]);
                }
            }
            catch (Exception e)
            {

            }
        }

        private DataTable GetCurrentAmounts()
        {
            string sql = "SELECT ENDINGBALANCE,STATEMENTDIFFERENCE FROM C_BANKSTATEMENT "
                    + "WHERE C_BANKSTATEMENT_ID=" + GetC_BankStatement_ID();

            return DB.ExecuteDataset(sql, null, Get_TrxName()).Tables[0];
        }
    }
}
