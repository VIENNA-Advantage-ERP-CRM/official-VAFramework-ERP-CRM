/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_InOut
 * Purpose        : Post Invoice Documents.
                    <pre>
                    Table: VAB_BankingJRNL (392)
                    Document Types:     CMB
                    </pre>
 * Class Used     : Doc
 * Chronological    Development
 * Raghunandan      19-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
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
    /// <summary>
    /// 
    /// </summary>
    public class Doc_Bank : Doc
    {
        //Bank Account			
        private int _VAB_Bank_Acct_ID = 0;

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Bank(MVABAccountBook[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MVABBankingJRNL), idr, MVABMasterDocType.DOCBASETYPE_BANKSTATEMENT, trxName)
        {

        }
        public Doc_Bank(MVABAccountBook[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MVABBankingJRNL), dr, MVABMasterDocType.DOCBASETYPE_BANKSTATEMENT, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MVABBankingJRNL bs = (MVABBankingJRNL)GetPO();
            SetDateDoc(bs.GetStatementDate());
            SetDateAcct(bs.GetStatementDate());	//	Overwritten on Line Level

            _VAB_Bank_Acct_ID = bs.GetVAB_Bank_Acct_ID();
            //	Amounts
            SetAmount(AMTTYPE_Gross, bs.GetStatementDifference());

            //  Set Bank Account Info (Currency)
            MVABBankAcct ba = MVABBankAcct.Get(GetCtx(), _VAB_Bank_Acct_ID);
            SetVAB_Currency_ID(ba.GetVAB_Currency_ID());

            //	Contained Objects
            _lines = LoadLines(bs);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }

        /// <summary>
        /// Load Invoice Line.
        /// 4 amounts
        /// AMTTYPE_Payment
        /// AMTTYPE_Statement2
        /// AMTTYPE_Charge
        /// AMTTYPE_Interest
        /// </summary>
        /// <param name="bs">bank statement</param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MVABBankingJRNL bs)
        {
            List<DocLine> list = new List<DocLine>();
            MVABBankingJRNLLine[] lines = bs.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MVABBankingJRNLLine line = lines[i];
                DocLine_Bank docLine = new DocLine_Bank(line, this);
                //	Set Date Acct
                if (i == 0)
                {
                    SetDateAcct(line.GetDateAcct());
                }
                MVABYearPeriod period = MVABYearPeriod.Get(GetCtx(), line.GetDateAcct());
                if (period != null && period.IsOpen(MVABMasterDocType.DOCBASETYPE_BANKSTATEMENT))
                {
                    docLine.SetVAB_YearPeriod_ID(period.GetVAB_YearPeriod_ID());
                }
                //
                list.Add(docLine);
            }

            //	Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
        }

        /// <summary>
        ///  Get Source Currency Balance - subtracts line amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total invoice is bigger than lines</returns>
        public override Decimal GetBalance()
        {
            Decimal? retValue = Env.ZERO;
            StringBuilder sb = new StringBuilder(" [");
            //  Total
            retValue = Decimal.Add(retValue.Value, GetAmount(Doc.AMTTYPE_Gross).Value);
            sb.Append(GetAmount(Doc.AMTTYPE_Gross));
            //  - Lines
            for (int i = 0; i < _lines.Length; i++)
            {
                Decimal lineBalance = ((DocLine_Bank)_lines[i]).GetStmtAmt();
                retValue = Decimal.Subtract(retValue.Value, lineBalance);
                sb.Append("-").Append(lineBalance);
            }
            sb.Append("]");
            //
            log.Fine(ToString() + " Balance=" + retValue + sb.ToString());
            return retValue.Value;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        /// CMB.
        /// <pre>
        /// BankAsset       DR      CR  (Statement)
        /// BankInTransit   DR      CR              (Payment)
        /// Charge          DR          (Charge)
        /// Interest        DR      CR  (Interest)
        /// </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns></returns>
        public override List<Fact> CreateFacts(MVABAccountBook as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);

            //  Header -- there may be different currency amounts

            FactLine fl = null;
            int VAF_Org_ID = GetBank_Org_ID();	//	Bank Account Org
            bool addPost = false;
            //Check For Module
            Tuple<String, String, String> aInfo = null;
            if (Env.HasModulePrefix("ED008_", out aInfo))
            {
                addPost = true;
            }
            else
            {
                addPost = false;
            }
            // Posting Work Done For ED008 Module
            if (addPost == true)
            {
                for (int i = 0; i < _lines.Length; i++)
                {
                    DocLine_Bank line = (DocLine_Bank)_lines[i];
                    int VAB_BusinessPartner_ID = line.GetVAB_BusinessPartner_ID();
                    int VAB_Payment_ID = line.GetVAB_Payment_ID();
                    //  BankAsset       DR      CR  (Statement)
                    fl = fact.CreateLine(line,
                        GetAccount(Doc.ACCTTYPE_BankAsset, as1), line.GetVAB_Currency_ID(), line.GetStmtAmt());

                    if (fl != null && VAF_Org_ID != 0)
                    {
                        fl.SetVAF_Org_ID(VAF_Org_ID);
                    }
                    if (fl != null && VAB_BusinessPartner_ID != 0)
                    {
                        fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    }

                    //  BankInTransit   DR      CR              (Payment)
                    MVABAccount acct = null;

                    string tenderType = Util.GetValueOfString(DB.ExecuteScalar("SELECT tendertype FROM VAB_Payment WHERE VAB_Payment_ID=" + VAB_Payment_ID + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                    // Tender Type RIBA
                    if ("R".Equals(tenderType))
                    {
                        int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RIBA_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=" + GetVAB_Bank_Acct_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (validComID > 0)
                        {
                            acct = MVABAccount.Get(Env.GetCtx(), validComID);
                        }

                        if (acct == null)
                        {
                            validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RIBA_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            acct = MVABAccount.Get(Env.GetCtx(), validComID);
                        }
                    }
                    // Tender Type MAV
                    else if ("M".Equals(tenderType))
                    {
                        int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_MAV_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=" + GetVAB_Bank_Acct_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (validComID > 0)
                        {
                            acct = MVABAccount.Get(Env.GetCtx(), validComID);
                        }

                        if (acct == null)
                        {
                            validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_MAV_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            acct = MVABAccount.Get(Env.GetCtx(), validComID);
                        }
                    }
                    // Tender Type RID
                    else if ("I".Equals(tenderType))
                    {
                        int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RID_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=" + GetVAB_Bank_Acct_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (validComID > 0)
                        {
                            acct = MVABAccount.Get(Env.GetCtx(), validComID);
                        }

                        if (acct == null)
                        {
                            validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RID_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            acct = MVABAccount.Get(Env.GetCtx(), validComID);
                        }
                    }
                    else
                    {
                        acct = GetAccount(Doc.ACCTTYPE_BankInTransit, as1);
                    }

                    fl = fact.CreateLine(line, acct, line.GetVAB_Currency_ID(), Decimal.Negate(line.GetTrxAmt()));
                    if (fl != null)
                    {
                        if (VAB_BusinessPartner_ID != 0)
                        {
                            fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                        }
                        if (VAF_Org_ID != 0)
                        {
                            fl.SetVAF_Org_ID(VAF_Org_ID);
                        }
                        else
                        {
                            fl.SetVAF_Org_ID(line.GetVAF_Org_ID(true)); // from payment
                        }
                    }
                    //  Charge          DR          (Charge)
                    fl = fact.CreateLine(line, line.GetChargeAccount(as1, Decimal.Negate(line.GetChargeAmt())), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetChargeAmt()), null);
                    if (fl != null && VAB_BusinessPartner_ID != 0)
                    {
                        fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    }

                    //  Interest        DR      CR  (Interest)
                    if (Env.Signum(line.GetInterestAmt()) < 0)
                    {
                        fl = fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_InterestExp, as1), GetAccount(Doc.ACCTTYPE_InterestExp, as1), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetInterestAmt()));
                    }
                    else
                    {
                        fl = fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_InterestRev, as1), GetAccount(Doc.ACCTTYPE_InterestRev, as1), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetInterestAmt()));
                    }
                    if (fl != null && VAB_BusinessPartner_ID != 0)
                    {
                        fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    }
                }
            }
            // Default Posting Logic
            else
            {
                //  Lines
                for (int i = 0; i < _lines.Length; i++)
                {
                    DocLine_Bank line = (DocLine_Bank)_lines[i];
                    int VAB_BusinessPartner_ID = line.GetVAB_BusinessPartner_ID();

                    //  BankAsset       DR      CR  (Statement)
                    fl = fact.CreateLine(line,
                        GetAccount(Doc.ACCTTYPE_BankAsset, as1), line.GetVAB_Currency_ID(), line.GetStmtAmt());

                    if (fl != null && VAF_Org_ID != 0)
                    {
                        fl.SetVAF_Org_ID(VAF_Org_ID);
                    }
                    if (fl != null && VAB_BusinessPartner_ID != 0)
                    {
                        fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    }

                    //  BankInTransit   DR      CR              (Payment)
                    fl = fact.CreateLine(line,
                        GetAccount(Doc.ACCTTYPE_BankInTransit, as1), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetTrxAmt()));
                    if (fl != null)
                    {
                        if (VAB_BusinessPartner_ID != 0)
                        {
                            fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                        }
                        if (VAF_Org_ID != 0)
                        {
                            fl.SetVAF_Org_ID(VAF_Org_ID);
                        }
                        else
                        {
                            fl.SetVAF_Org_ID(line.GetVAF_Org_ID(true)); // from payment
                        }
                    }
                    //  Charge          DR          (Charge)
                    fl = fact.CreateLine(line, line.GetChargeAccount(as1, Decimal.Negate(line.GetChargeAmt())), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetChargeAmt()), null);
                    if (fl != null && VAB_BusinessPartner_ID != 0)
                    {
                        fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    }

                    //  Interest        DR      CR  (Interest)
                    if (Env.Signum(line.GetInterestAmt()) < 0)
                    {
                        fl = fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_InterestExp, as1), GetAccount(Doc.ACCTTYPE_InterestExp, as1), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetInterestAmt()));
                    }
                    else
                    {
                        fl = fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_InterestRev, as1), GetAccount(Doc.ACCTTYPE_InterestRev, as1), line.GetVAB_Currency_ID(), Decimal.Negate(line.GetInterestAmt()));
                    }
                    if (fl != null && VAB_BusinessPartner_ID != 0)
                    {
                        fl.SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    }
                    //
                    //	fact.createTaxCorrection();
                }
            }
            //
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }

        /// <summary>
        /// Get VAF_Org_ID from Bank Account
        /// </summary>
        /// <returns>VAF_Org_ID or 0</returns>
        private int GetBank_Org_ID()
        {
            if (_VAB_Bank_Acct_ID == 0)
            {
                return 0;
            }
            //
            MVABBankAcct ba = MVABBankAcct.Get(GetCtx(), _VAB_Bank_Acct_ID);
            return ba.GetVAF_Org_ID();
        }

    }
}
