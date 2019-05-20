/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_GLJournal
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                  Table:              GL_Journal (224)
 *                  Document Types:     GLJ
 *                  </pre>
 *                  * Class Used     : Doc
 * Chronological    Development
 * Raghunandan      21-Jan-2010
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
    public class Doc_GLJournal : Doc
    {
        //Posting Type				
        private String _PostingType = null;
        private int _C_AcctSchema_ID = 0;

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_GLJournal(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MJournal), idr, null, trxName)
        {

        }
        public Doc_GLJournal(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MJournal), dr, null, trxName)
        {

        }
        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MJournal journal = (MJournal)GetPO();
            _PostingType = journal.GetPostingType();
            _C_AcctSchema_ID = journal.GetC_AcctSchema_ID();
            SetDateAcct(journal.GetDateAcct());

            //	Contained Objects
            _lines = LoadLines(journal);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }

        /// <summary>
        /// Load Invoice Line
        /// </summary>
        /// <param name="journal"></param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MJournal journal)
        {
            List<DocLine> list = new List<DocLine>();
            MJournalLine[] lines = journal.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MJournalLine line = lines[i];

                if (line.GetElementType() == null)
                {
                    DocLine docLine = new DocLine(line, this);
                    //  --  Source Amounts
                    docLine.SetAmount(line.GetAmtAcctDr(), line.GetAmtAcctCr());
                    //  --  Converted Amounts
                    // no need to update converted amount here
                    //docLine.SetConvertedAmt(_C_AcctSchema_ID, line.GetAmtAcctDr(), line.GetAmtAcctCr());
                    //  --  Account
                    MAccount account = line.GetAccount();
                    docLine.SetAccount(account);
                    //  -- Quantity
                    docLine.SetQty(line.GetQty(), false);
                    // -- Date
                    docLine.SetDateAcct(journal.GetDateAcct());
                    //	--	Organization of Line was set to Org of Account
                    list.Add(docLine);
                }
                else
                {
                    string sql = "SELECT * FROM GL_LineDimension WHERE GL_JournalLine_ID=" + line.Get_ID();
                    DataSet ds = DB.ExecuteDataset(sql);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                        {
                            DataRow dr = ds.Tables[0].Rows[m];
                            X_GL_LineDimension lDim = new X_GL_LineDimension(GetCtx(), dr, null);

                            DocLine docLine = new DocLine(lDim, this);
                            //  --  Source Amounts


                            decimal cRate = line.GetCurrencyRate();
                            if (cRate == 0 || cRate == null)
                            {
                                cRate = 1;
                            }
                            decimal amtAcctCr = 0;
                            decimal amtAcctDr = 0;

                            MAcctSchema mSc = new MAcctSchema(GetCtx(), _C_AcctSchema_ID, null);


                            if (line.GetAmtSourceDr() > 0)
                            {
                                amtAcctDr = lDim.GetAmount() * cRate;
                                docLine.SetAmount(amtAcctDr, 0);
                                amtAcctDr = Decimal.Round(amtAcctDr, mSc.GetStdPrecision());

                            }
                            else
                            {
                                amtAcctCr = lDim.GetAmount() * cRate;
                                docLine.SetAmount(0, amtAcctCr);
                                amtAcctCr = Decimal.Round(amtAcctCr, mSc.GetStdPrecision());
                            }
                            //  --  Converted Amounts
                            // no need to update converted amount here
                            //docLine.SetConvertedAmt(_C_AcctSchema_ID, amtAcctDr, amtAcctCr);
                            //  --  Account
                            MAccount account = line.GetAccount();


                            docLine.SetAccount(account);
                            //  -- Quantity
                            docLine.SetQty(lDim.GetQty(), false);
                            // -- Date
                            docLine.SetDateAcct(journal.GetDateAcct());

                            //	--	Organization of Line was set to Org of Account
                            list.Add(docLine);
                        }
                    }

                }




            }
            //	Return Array
            int size = list.Count;
            DocLine[] dls = new DocLine[size];
            dls = list.ToArray();
            return dls;
        }


        /// <summary>
        ///  Get Source Currency Balance - subtracts line and tax amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total invoice is bigger than lines</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            StringBuilder sb = new StringBuilder(" [");
            //  Lines
            for (int i = 0; i < _lines.Length; i++)
            {
                retValue = Decimal.Add(retValue, _lines[i].GetAmtSource());
                sb.Append("+").Append(_lines[i].GetAmtSource());
            }
            sb.Append("]");
            //
            log.Fine(ToString() + " Balance=" + retValue + sb.ToString());
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  GLJ.
        ///  (only for the accounting scheme, it was created)
        ///  <pre>
        ///     account     DR          CR
        ///  </pre>
        /// </summary>
        /// <param name="?"></param>
        /// <returns>fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            List<Fact> facts = new List<Fact>();
            //	Other Acct Schema
            // need to Post GL Journal for Multiple Accounting Schema that's why commented this section
            //if (as1.GetC_AcctSchema_ID() != _C_AcctSchema_ID)
            //{
            //    return facts;
            //}

            //  create Fact Header
            Fact fact = new Fact(this, as1, _PostingType);

            //  GLJ
            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_GLJOURNAL))
            {
                //  account     DR      CR
                for (int i = 0; i < _lines.Length; i++)
                {
                    // need to Post GL Journal for Multiple Accounting Schema that's why commented this condition
                    //if (_lines[i].GetC_AcctSchema_ID() == as1.GetC_AcctSchema_ID())
                    //{
                    fact.CreateLine(_lines[i],
                                    _lines[i].GetAccount(),
                                    GetC_Currency_ID(),
                                    _lines[i].GetAmtSourceDr(),
                                    _lines[i].GetAmtSourceCr());
                    //}
                }	//	for all lines
            }
            else
            {
                _error = "DocumentType unknown: " + GetDocumentType();
                log.Log(Level.SEVERE, _error);
                fact = null;
            }
            //
            facts.Add(fact);
            return facts;
        }
    }
}
