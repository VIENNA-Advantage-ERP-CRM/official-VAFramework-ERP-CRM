using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace ModelLibrary.Acct
{
    class Doc_ProfitLoss : Doc
    {

        // private int C_AcctSchema = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_ProfitLoss(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MProfitLoss), idr, null, trxName)
        {

        }
        public Doc_ProfitLoss(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MProfitLoss), dr, null, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MProfitLoss pay = (MProfitLoss)GetPO();
            SetDateDoc(pay.GetDateTrx());
            _lines = LoadLines(pay);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }
        private DocLine[] LoadLines(MProfitLoss pay)
        {
            List<DocLine> list = new List<DocLine>();
            MProfitLossLines[] lines = pay.GetLines(false);
            //C_AcctSchema = Util.GetValueOfInt(DB.ExecuteScalar("SELECT c_acctschema1_id FROM AD_ClientInfo WHERE AD_Client_ID=" + GetAD_Client_ID()));
            for (int i = 0; i < lines.Length; i++)
            {
                MProfitLossLines line = lines[i];
                DocLine docLine = new DocLine(line, this);
                docLine.SetAmount(line.GetC_ProfitAndLoss_ID() != 0 ? line.GetAccountDebit() : Math.Abs(line.GetAccountDebit()),
                                  line.GetC_ProfitAndLoss_ID() != 0 ? line.GetAccountCredit():Math.Abs(line.GetAccountCredit()));
                //docLine.SetConvertedAmt(line.GetC_AcctSchema_ID(), line.GetAccountDebit(), line.GetAccountCredit());

                // set primary key value 
                docLine.SetPrimaryKeyValue(line.GetC_ProfitLossLines_ID());
                // set GL journal line table ID
                docLine.SetLineTable_ID(line.Get_Table_ID());
                //
                list.Add(docLine);
            }

            //	Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
        }

        private int GetCurrency(int c_acct_Schema_id)
        {
            if (c_acct_Schema_id > 0)
            {
                int Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_AcctSchema WHERE C_AcctSchema_ID=" + c_acct_Schema_id));
                return Currency_ID;
            }
            return 0;
        }


        /// <summary>
        /// Get Source Currency Balance - subtracts line amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total invoice is bigger than lines</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            StringBuilder sb = new StringBuilder(" [");
            //  Total
            retValue = Decimal.Add(retValue, GetAmount(Doc.AMTTYPE_Gross).Value);
            sb.Append(GetAmount(Doc.AMTTYPE_Gross));
            //  - Lines
            for (int i = 0; i < _lines.Length; i++)
            {
                retValue = Decimal.Subtract(retValue, _lines[i].GetAmtSource());
                sb.Append("-").Append(_lines[i].GetAmtSource());
            }
            sb.Append("]");
            //
            log.Fine(ToString() + " Balance=" + retValue + sb.ToString());
            //	return retValue;
            return Env.ZERO;    //  Lines are balanced
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        /// CMC.
        /// <pre>
        /// Expense
        /// CashExpense     DR
        ///        CashAsset               CR
        ///Receipt
        ///        CashAsset       DR
        ///        CashReceipt             CR
        ///  Charge
        ///        Charge          DR
        ///          CashAsset               CR
        ///  Difference
        ///          CashDifference  DR
        ///          CashAsset               CR
        ///  Invoice
        ///          CashAsset       DR
        ///          CashTransfer            CR
        ///  Transfer
        ///          BankInTransit   DR
        ///          CashAsset               CR
        ///  </pre>
        /// </summary>
        /// <param name="?"></param>
        /// <returns>Fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {


            //  create Fact Header
            List<Fact> facts = new List<Fact>();

            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_PROFITLOSS))
            {
                //Change By mohit 
                // Get Assigned Accounting Schemas based on organization                
                MProfitLossLines PLossline = new MProfitLossLines(GetCtx(), _lines[0].Get_ID(), null);
                MProfitLoss PLoss = new MProfitLoss(GetCtx(), PLossline.GetC_ProfitLoss_ID(), null);
                MAcctSchema HeaderAcctSchema = new MAcctSchema(GetCtx(), Util.GetValueOfInt(PLoss.Get_Value("C_AcctSchema_ID")), null);
                List<int> _ListAcctSch = new List<int>();
                // Profit & Loss account shall be posted only in accounting schema selected on header (By Ashish - discussed with Mukesh sir)
                //_ListAcctSch = GetAcctSchemas(PLoss.GetAD_Org_ID());
                _ListAcctSch.Add(HeaderAcctSchema.GetC_AcctSchema_ID());



                if (_ListAcctSch.Count > 0)
                {

                    int CurrencyType_ID = GetDefaultConversionType(GetAD_Client_ID(), GetAD_Org_ID());
                    for (int asch = 0; asch < _ListAcctSch.Count; asch++)
                    {
                        MAcctSchema AccountingSchema = new MAcctSchema(GetCtx(), _ListAcctSch[asch], null);
                        //	Decimal grossAmt = getAmount(Doc.AMTTYPE_Gross);
                        SetC_Currency_ID(GetCurrency(AccountingSchema.GetC_AcctSchema_ID()));
                        //  Commitment
                        Fact fact = new Fact(this, AccountingSchema, Fact.POST_Actual);
                        Decimal total = Env.ZERO, totalCredit = Env.ZERO, totalDebit = Env.ZERO;
                        Decimal credit = Env.ZERO, debit = Env.ZERO;

                        for (int i = 0; i < _lines.Length; i++)
                        {
                            DocLine dline = _lines[i];
                            MProfitLossLines line = new MProfitLossLines(GetCtx(), dline.Get_ID(), null);

                            if (Util.GetValueOfInt(HeaderAcctSchema.GetC_Currency_ID()) == Util.GetValueOfInt(AccountingSchema.GetC_Currency_ID()))
                            {
                                credit = Util.GetValueOfDecimal(dline.GetAmtSourceCr());
                                debit = Util.GetValueOfDecimal(dline.GetAmtSourceDr());
                            }
                            else
                            {
                                credit = MConversionRate.Convert(GetCtx(), Util.GetValueOfDecimal(dline.GetAmtSourceCr()), HeaderAcctSchema.GetC_Currency_ID(), AccountingSchema.GetC_Currency_ID(),
                                                                     PLoss.GetDateAcct(), CurrencyType_ID, GetAD_Client_ID(), GetAD_Org_ID());

                                debit = MConversionRate.Convert(GetCtx(), Util.GetValueOfDecimal(dline.GetAmtSourceDr()), HeaderAcctSchema.GetC_Currency_ID(), AccountingSchema.GetC_Currency_ID(),
                                                                     PLoss.GetDateAcct(), CurrencyType_ID, GetAD_Client_ID(), GetAD_Org_ID());

                                Util.GetValueOfDecimal(dline.GetAmtAcctDr());

                            }
                            if (credit > 0)
                            {
                                totalCredit = Decimal.Add(totalCredit, credit);
                            }
                            if (debit > 0)
                            {
                                totalDebit = Decimal.Add(totalDebit, debit);
                            }

                            //	Account
                            MAccount expense = MAccount.Get(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), AccountingSchema.GetC_AcctSchema_ID(), line.GetAccount_ID(), line.GetC_SubAcct_ID(), line.GetM_Product_ID(), line.GetC_BPartner_ID(), line.GetAD_OrgTrx_ID(),
                                line.GetC_LocFrom_ID(), line.GetC_LocTo_ID(), line.GetC_SalesRegion_ID(), line.GetC_Project_ID(), line.GetC_Campaign_ID(), line.GetC_Activity_ID(), line.GetUser1_ID(), line.GetUser2_ID(), line.GetUserElement1_ID(), line.GetUserElement2_ID(),
                                line.GetUserElement3_ID(), line.GetUserElement4_ID(), line.GetUserElement5_ID(), line.GetUserElement6_ID(), line.GetUserElement7_ID(), line.GetUserElement8_ID(), line.GetUserElement9_ID());

                            fact.CreateLine(dline, expense, GetCurrency(AccountingSchema.GetC_AcctSchema_ID()), debit, credit);
                        }
                        total = totalCredit - totalDebit;
                        if (total != Env.ZERO)
                        {
                            int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT IncomeSummary_Acct FROM C_AcctSchema_GL WHERE C_AcctSchema_ID=" + AccountingSchema.GetC_AcctSchema_ID() + " AND AD_Client_ID = " + GetAD_Client_ID()));
                            MAccount acct = MAccount.Get(GetCtx(), validComID);
                            fact.CreateLine(null, acct, GetC_Currency_ID(), total);
                        }
                        //if (TotalCurrLoss != Env.ZERO)
                        //{
                        //    int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ValidCombination_ID FROM C_ValidCombination WHERE Account_ID= ( SELECT C_ElementValue_ID FROM C_ElementValue WHERE Value='82540' AND AD_Client_ID = " + GetAD_Client_ID() + " )"));
                        //    MAccount acct = MAccount.Get(GetCtx(), validComID);
                        //    TotalCurrLoss = MConversionRate.Convert(GetCtx(), TotalCurrLoss, childCashCurrency, headerCashCurrency, GetAD_Client_ID(), GetAD_Org_ID());
                        //    fact.CreateLine(null, acct,
                        //     GetC_Currency_ID(), (TotalCurrLoss));
                        //}

                        facts.Add(fact);
                    }
                }
            }
            return facts;
        }
        private List<int> GetAcctSchemas(int AD_Org_ID)
        {
            List<int> AcctSch = new List<int>();
            string Sql = @"SELECT asch.C_AcctSchema_ID FROM FRPT_AssignedOrg aorg
                           INNER JOIN C_AcctSchema asch ON (aorg.C_AcctSchema_ID=asch.C_AcctSchema_ID) WHERE asch.IsActive='Y'
                            AND aorg.IsActive='Y' AND aorg.AD_Org_ID=" + AD_Org_ID;
            DataSet _ds = DB.ExecuteDataset(Sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                {
                    AcctSch.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                }
                _ds.Dispose();
                _ds = null;
            }
            return AcctSch;
        }
        private int GetDefaultConversionType(int AD_Client_ID, int AD_Org_ID)
        {
            int C_ConversionType_ID = 0;

            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT (*) from C_ConversionType where ISDEFAULT='Y'")) > 1)
            {
                if (C_ConversionType_ID == 0)
                {
                    C_ConversionType_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ConversionType_ID from 
                    C_ConversionType where ISDEFAULT='Y' AND AD_Client_ID=" + AD_Client_ID + @" AND AD_Org_ID=" + AD_Org_ID));
                }
                if (C_ConversionType_ID == 0)
                {
                    C_ConversionType_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ConversionType_ID from 
                    C_ConversionType where ISDEFAULT='Y' AND AD_Client_ID=" + AD_Client_ID + @" AND AD_Org_ID=0"));
                }
                if (C_ConversionType_ID == 0)
                {
                    C_ConversionType_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ConversionType_ID from 
                    C_ConversionType where ISDEFAULT='Y' AND AD_Client_ID=0 AND AD_Org_ID=0"));
                }
            }
            else
            {
                C_ConversionType_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ConversionType_ID from 
                    C_ConversionType where ISDEFAULT='Y' "));
            }

            return C_ConversionType_ID;
        }
    }

}
