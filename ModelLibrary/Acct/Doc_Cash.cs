/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_Cash
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                  Table:              C_Cash (407)
 *                  Document Types:     CMC
 *                  </pre>
 * Class Used      : Doc
 * Chronological    Development
 * Raghunandan      20-Jan-2010
  ******************************************************/
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

namespace VAdvantage.Acct
{
    public class Doc_Cash : Doc
    {

        //Beginning balance of cash Journal
        private Decimal BeginningBalance = Env.ZERO;
        //Header CashBook
        private int HeaderCasbookID = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Cash(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MCash), idr, MDocBaseType.DOCBASETYPE_CASHJOURNAL, trxName)
        {

        }
        public Doc_Cash(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MCash), dr, MDocBaseType.DOCBASETYPE_CASHJOURNAL, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MCash cash = (MCash)GetPO();
            SetDateDoc(cash.GetStatementDate());
            BeginningBalance = cash.GetBeginningBalance();
            HeaderCasbookID = cash.GetC_CashBook_ID();

            //	Amounts
            SetAmount(Doc.AMTTYPE_Gross, cash.GetStatementDifference());

            //  Set CashBook Org & Currency
            MCashBook cb = MCashBook.Get(GetCtx(), cash.GetC_CashBook_ID());
            SetC_CashBook_ID(cb.GetC_CashBook_ID());
            SetC_Currency_ID(cb.GetC_Currency_ID());

            //	Contained Objects
            _lines = LoadLines(cash, cb);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }


        /// <summary>
        /// Load Cash Line
        /// </summary>
        /// <param name="cash">journal</param>
        /// <param name="cb">cash book</param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MCash cash, MCashBook cb)
        {
            List<DocLine> list = new List<DocLine>();
            MCashLine[] lines = cash.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MCashLine line = lines[i];
                DocLine_Cash docLine = new DocLine_Cash(line, this);
                //
                list.Add(docLine);
            }

            //	Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
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
            //  Need to have CashBook
            if (GetC_CashBook_ID() == 0)
            {
                _error = "C_CashBook_ID not set";
                log.Log(Level.SEVERE, _error);
                return null;
            }

            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);

            //  Header posting amt as1 Invoices and Transfer could be differenet currency
            //  CashAsset Total
            Decimal assetAmt = Env.ZERO;
            Decimal TotalCurrLoss = Env.ZERO;
            Decimal TotalCurrGain = Env.ZERO;
            int headerCashCurrency = 0;
            int childCashCurrency = 0;
            int headerCashOrg = 0;
            int childCashOrg = 0;



            //  Lines
            for (int i = 0; i < _lines.Length; i++)
            {
                DocLine_Cash line = (DocLine_Cash)_lines[i];
                String CashType = line.GetCashType();

                if (CashType.Equals(DocLine_Cash.CASHTYPE_EXPENSE))
                {   //  amount is negative
                    //  CashExpense     DR
                    //  CashAsset               CR
                    fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashExpense, as1),
                        GetC_Currency_ID(), Decimal.Negate(line.GetAmount()), null);
                    //	fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                    //		p_vo.C_Currency_ID, null, line.GetAmount().negate());
                    assetAmt = Decimal.Subtract(assetAmt, Decimal.Negate(line.GetAmount()));

                    CreateFactLine(fact, line, as1, line.GetAmount());
                }
                else if (CashType.Equals(DocLine_Cash.CASHTYPE_RECEIPT))
                {   //  amount is positive
                    //  CashAsset       DR
                    //  CashReceipt             CR
                    //	fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                    //		p_vo.C_Currency_ID, line.GetAmount(), null);
                    assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                    fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashReceipt, as1),
                        GetC_Currency_ID(), null, line.GetAmount());

                    CreateFactLine(fact, line, as1, line.GetAmount());
                }
                else if (CashType.Equals(DocLine_Cash.CASHTYPE_CHARGE))
                {   //  amount is negative
                    //  Charge          DR
                    //  CashAsset               CR
                    fact.CreateLine(line, line.GetChargeAccount(as1, GetAmount()),
                        GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                    //fact.CreateLine(line, line.GetChargeAccount(as1, GetAmount()),
                    //    GetC_Currency_ID(), Decimal.Negate(line.GetAmount()), null);
                    //	fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                    //		p_vo.C_Currency_ID, null, line.GetAmount().negate());
                    assetAmt = Decimal.Subtract(assetAmt, Decimal.Negate(line.GetAmount()));

                    CreateFactLine(fact, line, as1, line.GetAmount());
                }
                else if (CashType.Equals(DocLine_Cash.CASHTYPE_DIFFERENCE))
                {   //  amount is pos/neg
                    //  CashDifference  DR
                    //  CashAsset               CR
                    fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashDifference, as1),
                        GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                    //	fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                    //		p_vo.C_Currency_ID, line.GetAmount());
                    assetAmt = Decimal.Add(assetAmt, line.GetAmount());

                    CreateFactLine(fact, line, as1, line.GetAmount());

                }
                else if (CashType.Equals(DocLine_Cash.CASHTYPE_INVOICE))
                {   //  amount is pos/neg
                    //  CashAsset       DR      dr      --   Invoice is in Invoice Currency !
                    //  CashTransfer    cr      CR
                    if (line.GetC_Currency_ID() == GetC_Currency_ID())
                    {
                        assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                    }
                    else
                    {
                        fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                            line.GetC_Currency_ID(), line.GetAmount());
                    }
                    fact.CreateLine(line,
                        GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                        line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));

                    CreateFactLine(fact, line, as1, line.GetAmount());

                }
                else if (CashType.Equals(DocLine_Cash.CASHTYPE_TRANSFER))
                {   //  amount is pos/neg
                    //  BankInTransit   DR      dr      --  Transfer is in Bank Account Currency
                    //  CashAsset       dr      CR
                    int temp = GetC_BankAccount_ID();
                    SetC_BankAccount_ID(line.GetC_BankAccount_ID());
                    fact.CreateLine(line,
                        GetAccount(Doc.ACCTTYPE_BankInTransit, as1),
                        line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                    SetC_BankAccount_ID(temp);
                    if (line.GetC_Currency_ID() == GetC_Currency_ID())
                    {
                        assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                    }
                    else
                    {
                        fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                            line.GetC_Currency_ID(), line.GetAmount());
                    }

                    CreateFactLine(fact, line, as1, line.GetAmount());
                }
                // Change to Apply Posting Logic against BusinessPartner
                else if (CashType.Equals(DocLine_Cash.CASHTYPE_BUSINESSPARTNER))
                {   //  amount is pos/neg
                    //  CashAsset       DR      dr      --   Invoice is in Invoice Currency !
                    //  CashTransfer    cr      CR

                    MBPartner bPartner = new MBPartner(Env.GetCtx(), line.GetC_BPartner_ID(), null);
                    if (bPartner != null)
                    {
                        if (bPartner.IsEmployee())
                        {
                            if (line.GetC_Currency_ID() == GetC_Currency_ID())
                            {
                                assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                            }
                            else
                            {
                                fact.CreateLine(line,
                                    GetAccount(Doc.ACCTTYPE_E_Prepayment, as1, line.GetC_BPartner_ID()),
                                    line.GetC_Currency_ID(), line.GetAmount());
                            }
                            fact.CreateLine(line,
                                GetAccount(Doc.ACCTTYPE_E_Prepayment, as1, line.GetC_BPartner_ID()),
                                line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        }
                        else if (bPartner.IsVendor())
                        {
                            if (line.GetC_Currency_ID() == GetC_Currency_ID())
                            {
                                assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                            }
                            else
                            {
                                fact.CreateLine(line,
                                    GetAccount(Doc.ACCTTYPE_V_Prepayment, as1, line.GetC_BPartner_ID()),
                                    line.GetC_Currency_ID(), line.GetAmount());
                            }
                            fact.CreateLine(line,
                                GetAccount(Doc.ACCTTYPE_V_Prepayment, as1, line.GetC_BPartner_ID()),
                                line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        }
                        else if (bPartner.IsCustomer())
                        {
                            if (line.GetC_Currency_ID() == GetC_Currency_ID())
                            {
                                assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                            }
                            else
                            {
                                fact.CreateLine(line,
                                    GetAccount(Doc.ACCTTYPE_C_Prepayment, as1, line.GetC_BPartner_ID()),
                                    line.GetC_Currency_ID(), line.GetAmount());
                            }
                            fact.CreateLine(line,
                                GetAccount(Doc.ACCTTYPE_C_Prepayment, as1, line.GetC_BPartner_ID()),
                                line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        }
                        CreateFactLine(fact, line, as1, line.GetAmount());
                    }
                    else
                    {
                        if (line.GetC_Currency_ID() == GetC_Currency_ID())
                        {
                            assetAmt = Decimal.Add(assetAmt, line.GetAmount());
                        }
                        else
                        {
                            fact.CreateLine(line,
                                GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                                line.GetC_Currency_ID(), line.GetAmount());
                        }
                        fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                            line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));

                        CreateFactLine(fact, line, as1, line.GetAmount());
                    }
                }


                //Code Added to CashBookTransfer

                else if (CashType.Equals(DocLine_Cash.CASHTYPE_CASHBOOKTRANSFER))
                {


                    ////  amount is negative
                    ////  Charge          DR
                    ////  CashAsset               CR
                    //fact.CreateLine(line, line.GetChargeAccount(as1, GetAmount()),
                    //    GetC_Currency_ID(), Decimal.Negate(line.GetAmount()), null);
                    ////	fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                    ////		p_vo.C_Currency_ID, null, line.GetAmount().negate());
                    //assetAmt = Decimal.Subtract(assetAmt, Decimal.Negate(line.GetAmount()));
                    int temp = GetC_CashBook_ID();
                    SetC_CashBook_ID(line.Get_C_CashBook_ID());

                    if (BeginningBalance > 0)
                    {

                        fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                            line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        SetC_CashBook_ID(temp);
                        //if (line.GetC_Currency_ID() == (new MCashBook(Env.GetCtx(), GetC_CashBook_ID(), null)).GetC_Currency_ID())// GetC_Currency_ID())
                        //{
                        //if (BeginningBalance > assetAmt)
                        //if (BeginningBalance >= Math.Abs(line.GetAmount()))
                        //if (assetAmt <= Math.Abs(line.GetAmount()))
                        //{
                        assetAmt = (Decimal.Subtract(assetAmt, Decimal.Negate(line.GetAmount())));
                        //}
                        //else
                        //{
                        //    assetAmt = (Decimal.Subtract(assetAmt, Decimal.Negate(line.GetAmount())));
                        //}
                        //}
                        //else
                        //{
                        //    fact.CreateLine(line,
                        //        GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                        //        line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        //}
                    }
                    else
                    {


                        //fact.CreateLine(line,
                        //    GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                        //    line.GetC_Currency_ID(), decimal.Negate(line.GetAmount()));
                        fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                            (new MCashBook(Env.GetCtx(), HeaderCasbookID, null)).GetC_Currency_ID(), decimal.Negate(line.GetAmount()));
                        SetC_CashBook_ID(temp);
                        //if (line.GetC_Currency_ID() == (new MCashBook(Env.GetCtx(), GetC_CashBook_ID(), null)).GetC_Currency_ID())// GetC_Currency_ID())
                        //{
                        assetAmt = (Decimal.Add(assetAmt, line.GetAmount()));
                        //}
                        //else
                        //{
                        //    fact.CreateLine(line,
                        //        GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                        //        line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        //}
                    }

                    CreateFactLine(fact, line, as1, line.GetAmount());
                }

                else if (CashType.Equals(DocLine_Cash.CASHTYPE_CASHRECIEVEDFROM))
                {

                    int temp = GetC_CashBook_ID();
                    //SetC_CashBook_ID(line.Get_C_CashBook_ID());
                    //fact.CreateLine(line,
                    //    GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                    //    line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                    //SetC_CashBook_ID(temp);
                    //if (line.GetC_Currency_ID() == GetC_Currency_ID())
                    //{
                    //assetAmt = (Decimal.Add(assetAmt, line.GetAmount()));
                    //}
                    //else
                    //{
                    //    fact.CreateLine(line,
                    //        GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                    //        line.GetC_Currency_ID(), line.GetAmount());
                    //}
                    headerCashCurrency = (new MCashBook(Env.GetCtx(), HeaderCasbookID, null).GetC_Currency_ID());
                    childCashCurrency = (new MCashBook(Env.GetCtx(), line.Get_C_CashBook_ID(), null).GetC_Currency_ID());
                    headerCashOrg = (new MCashBook(Env.GetCtx(), HeaderCasbookID, null).GetAD_Org_ID());
                    childCashOrg = (new MCashBook(Env.GetCtx(), line.Get_C_CashBook_ID(), null).GetAD_Org_ID());

                    //else
                    //{
                    if (headerCashCurrency != childCashCurrency)
                    {
                        Decimal transferdAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT Amount FROM C_CashLine WHERE C_CashLine_ID =" + line.Get_C_CashLine_Ref_ID()));
                        Decimal recievedAmt = MConversionRate.Convert(Env.GetCtx(), line.GetAmount(), headerCashCurrency, childCashCurrency, GetAD_Client_ID(), GetAD_Org_ID());
                        Decimal res = Decimal.Subtract(recievedAmt, Math.Abs(transferdAmt));
                        if (res > 0)
                        {
                            TotalCurrGain = Decimal.Add(TotalCurrGain, res);
                        }
                        else if (res < 0)
                        {
                            TotalCurrLoss = Decimal.Add(TotalCurrLoss, Decimal.Negate(res));
                        }

                        SetC_CashBook_ID(line.Get_C_CashBook_ID());

                        int OrgID = line.GetAD_Org_ID();
                        if (headerCashOrg != childCashOrg)
                        {
                            OrgID = childCashOrg;

                        }
                        //else
                        //{
                        transferdAmt = MConversionRate.Convert(Env.GetCtx(), transferdAmt, childCashCurrency, headerCashCurrency, GetAD_Client_ID(), GetAD_Org_ID());
                        //}
                        fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                            headerCashCurrency, transferdAmt, OrgID);
                        SetC_CashBook_ID(temp);
                    }
                    else
                    {
                        SetC_CashBook_ID(line.Get_C_CashBook_ID());
                        if (headerCashOrg != childCashOrg)
                        {

                            fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                            line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()), childCashOrg);
                        }
                        else
                        {
                            fact.CreateLine(line,
                                GetAccount(Doc.ACCTTYPE_CashTransfer, as1),
                                line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                        }
                        SetC_CashBook_ID(temp);
                    }


                    //if (headerCashOrg != childCashOrg)
                    //{
                    //    DataSet ds = DB.ExecuteDataset("SELECT INTERCOMPANYDUETO_ACCT,INTERCOMPANYDUEFROM_ACCT FROM C_AcctSchema_GL WHERE AD_Client_ID=" + GetAD_Client_ID());
                    //    int dueFrom = 0;
                    //    int dueTo = 0;
                    //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    //    {
                    //        dueTo = Util.GetValueOfInt(ds.Tables[0].Rows[0]["INTERCOMPANYDUETO_ACCT"]);
                    //        dueFrom = Util.GetValueOfInt(ds.Tables[0].Rows[0]["INTERCOMPANYDUEFROM_ACCT"]);
                    //        if (dueFrom > 0 && dueTo > 0)
                    //        {
                    //            fact.CreateLine(line, MAccount.Get(Env.GetCtx(), dueFrom),line.GetC_Currency_ID(), line.GetAmount());
                    //            fact.CreateLine(line, MAccount.Get(Env.GetCtx(), dueTo), line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));
                    //        }
                    //    }

                    //}
                    //}
                    assetAmt = (Decimal.Add(assetAmt, line.GetAmount()));

                    CreateFactLine(fact, line, as1, line.GetAmount());

                }


            }	//  lines

            if (TotalCurrGain != Env.ZERO)
            {
                int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ValidCombination_ID FROM C_ValidCombination WHERE Account_ID= ( SELECT C_ElementValue_ID FROM C_ElementValue WHERE Value='80540' AND AD_Client_ID = " + GetAD_Client_ID() + " )"));
                MAccount acct = MAccount.Get(Env.GetCtx(), validComID);
                TotalCurrGain = MConversionRate.Convert(Env.GetCtx(), TotalCurrGain, childCashCurrency, headerCashCurrency, GetAD_Client_ID(), GetAD_Org_ID());


                fact.CreateLine(null, acct,
                GetC_Currency_ID(), Decimal.Negate(TotalCurrGain));


            }
            if (TotalCurrLoss != Env.ZERO)
            {
                int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ValidCombination_ID FROM C_ValidCombination WHERE Account_ID= ( SELECT C_ElementValue_ID FROM C_ElementValue WHERE Value='82540' AND AD_Client_ID = " + GetAD_Client_ID() + " )"));
                MAccount acct = MAccount.Get(Env.GetCtx(), validComID);
                TotalCurrLoss = MConversionRate.Convert(Env.GetCtx(), TotalCurrLoss, childCashCurrency, headerCashCurrency, GetAD_Client_ID(), GetAD_Org_ID());


                fact.CreateLine(null, acct,
                 GetC_Currency_ID(), (TotalCurrLoss));

            }
            //

            ////  Cash Asset
            //fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
            //    GetC_Currency_ID(), assetAmt);

            //  Cash Asset
            //fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
            //    GetC_Currency_ID(), assetAmt);
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }

        private void CreateFactLine(Fact fact, DocLine_Cash line, MAcctSchema as1, decimal assetAmt)
        {
            fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_CashAsset, as1),
                   GetC_Currency_ID(), assetAmt);
        }
    }
}
