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
        private int record_Id = 0;

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
            MAcctSchema mSc = new MAcctSchema(GetCtx(), _C_AcctSchema_ID, null);
            List<DocLine> list = new List<DocLine>();
            MJournalLine[] lines = journal.GetLines(false);
            record_Id = lines[0].GetGL_Journal_ID();

            for (int i = 0; i < lines.Length; i++)
            {
                MJournalLine line = lines[i];

                if (line.GetElementType() == null)
                {
                    DocLine docLine = new DocLine(line, this);
                    //  --  Source Amounts
                    docLine.SetAmount(line.GetAmtSourceDr(), line.GetAmtSourceCr());
                    docLine.SetC_Currency_ID(line.GetC_Currency_ID());
                    docLine.SetConversionRate(line.GetCurrencyRate() == 0 ? 1 : line.GetCurrencyRate());
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

                    // Set Description
                    docLine.SetDescription(line.GetDescription());
                    // set primary key value 
                    docLine.SetPrimaryKeyValue(line.GetGL_JournalLine_ID());
                    // set GL journal line table ID
                    docLine.SetLineTable_ID(line.Get_Table_ID());

                    list.Add(docLine);
                }
                else
                {
                    string sql = "SELECT * FROM GL_LineDimension WHERE GL_JournalLine_ID=" + line.Get_ID();
                    DataSet ds = DB.ExecuteDataset(sql);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = null;
                        X_GL_LineDimension lDim = null;
                        DocLine docLine = null;
                        MAccount account = null;
                        for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                        {
                            dr = ds.Tables[0].Rows[m];
                            lDim = new X_GL_LineDimension(GetCtx(), dr, null);

                            docLine = new DocLine(lDim, this);
                            //  --  Source Amounts


                            //decimal cRate = line.GetCurrencyRate();
                            //if (cRate == 0)
                            //{
                            //    cRate = 1;
                            //}
                            //decimal amtAcctCr = 0;
                            //decimal amtAcctDr = 0;

                            //MAcctSchema mSc = new MAcctSchema(GetCtx(), _C_AcctSchema_ID, null);


                            if (line.GetAmtSourceDr() != 0)
                            {
                                //amtAcctDr = lDim.GetAmount() * cRate;
                                docLine.SetAmount(lDim.GetAmount(), 0);
                                //amtAcctDr = Decimal.Round(amtAcctDr, mSc.GetStdPrecision());

                            }
                            else
                            {
                                //amtAcctCr = lDim.GetAmount() * cRate;
                                docLine.SetAmount(0, lDim.GetAmount());
                                //amtAcctCr = Decimal.Round(lDim.GetAmount(), mSc.GetStdPrecision());
                            }

                            docLine.SetC_Currency_ID(line.GetC_Currency_ID());
                            docLine.SetConversionRate(line.GetCurrencyRate() == 0 ? 1 : line.GetCurrencyRate());

                            //  --  Converted Amounts
                            // no need to update converted amount here
                            //docLine.SetConvertedAmt(_C_AcctSchema_ID, amtAcctDr, amtAcctCr);
                            //  --  Account
                            account = line.GetAccount();


                            docLine.SetAccount(account);
                            //  -- Quantity
                            docLine.SetQty(lDim.GetQty(), false);
                            // -- Date
                            docLine.SetDateAcct(journal.GetDateAcct());

                            // -- User Dimension
                            docLine = SetUserDimension(lDim, docLine);

                            // Set Description
                            docLine.SetDescription(line.GetDescription());
                            // set primary key value 
                            docLine.SetPrimaryKeyValue(line.GetGL_JournalLine_ID());
                            // set GL journal line table ID
                            docLine.SetLineTable_ID(line.Get_Table_ID());

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
        /// is used to set User Element Dimension
        /// </summary>
        /// <param name="journalLineDimension">journal line dimension object</param>
        /// <param name="docLine">document line object</param>
        private DocLine SetUserDimension(X_GL_LineDimension journalLineDimension, DocLine docLine)
        {
            if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement1 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement1(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement2 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement2(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement3 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement3(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement4 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement4(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement5 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement5(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement6 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement6(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement7 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement7(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement8 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement8(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MJournalLine.ELEMENTTYPE_UserElement9 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement9(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType().Equals(MJournalLine.ELEMENTTYPE_OrgTrx) && journalLineDimension.GetOrg_ID() > 0)
            {
                docLine.SetAD_OrgTrx_ID(Convert.ToInt32(journalLineDimension.GetOrg_ID()));
            }
            else if (journalLineDimension.GetLineType().Equals(MJournalLine.ELEMENTTYPE_Organization) && journalLineDimension.GetOrg_ID() > 0)
            {
                docLine.SetAD_Org_ID(Convert.ToInt32(journalLineDimension.GetOrg_ID()));
            }
            return docLine;
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

            // get conversion rate from Assigned accounting schema tab - 
            Decimal conversionRate = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT CurrencyRate FROM GL_AssignAcctSchema WHERE 
                                     C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID() + " AND GL_Journal_ID = " + record_Id, null, null));

            //  GLJ
            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_GLJOURNAL))
            {
                //  account     DR      CR
                for (int i = 0; i < _lines.Length; i++)
                {

                    // need to Post GL Journal for Multiple Accounting Schema that's why commented this condition
                    //if (_lines[i].GetC_AcctSchema_ID() == as1.GetC_AcctSchema_ID())
                    //{
                    // set conversion rate on line, so that amount to be converted based on that multiply rate 
                    if (as1.GetC_AcctSchema_ID() != _C_AcctSchema_ID && _lines[i].GetC_Currency_ID() != as1.GetC_Currency_ID())
                    {
                        conversionRate = MConversionRate.GetRate(_lines[i].GetC_Currency_ID(), as1.GetC_Currency_ID(),
                            _lines[i].GetDateAcct(), _lines[i].GetC_ConversionType_ID(),
                            as1.GetAD_Client_ID(), _lines[i].GetAD_Org_ID());
                        _lines[i].SetConversionRate(conversionRate);
                    }
                    else if (as1.GetC_AcctSchema_ID() != _C_AcctSchema_ID)
                    {
                        _lines[i].SetConversionRate(conversionRate);
                    }

                    fact.CreateLine(_lines[i],
                                    _lines[i].GetAccount(),
                                    _lines[i].GetC_Currency_ID(),
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
