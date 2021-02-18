/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_GLJournal
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                  Table:              VAGL_JRNL (224)
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
    public class Doc_GLJournal : Doc
    {
        //Posting Type				
        private String _PostingType = null;
        private int _VAB_AccountBook_ID = 0;
        private int record_Id = 0;

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_GLJournal(MVABAccountBook[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MVAGLJRNL), idr, null, trxName)
        {

        }
        public Doc_GLJournal(MVABAccountBook[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MVAGLJRNL), dr, null, trxName)
        {

        }
        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MVAGLJRNL journal = (MVAGLJRNL)GetPO();
            _PostingType = journal.GetPostingType();
            _VAB_AccountBook_ID = journal.GetVAB_AccountBook_ID();
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
        private DocLine[] LoadLines(MVAGLJRNL journal)
        {
            MVABAccountBook mSc = new MVABAccountBook(GetCtx(), _VAB_AccountBook_ID, null);
            List<DocLine> list = new List<DocLine>();
            MVAGLJRNLLine[] lines = journal.GetLines(false);
            record_Id = lines[0].GetVAGL_JRNL_ID();

            for (int i = 0; i < lines.Length; i++)
            {
                MVAGLJRNLLine line = lines[i];

                if (line.GetElementType() == null)
                {
                    DocLine docLine = new DocLine(line, this);
                    //  --  Source Amounts
                    docLine.SetAmount(line.GetAmtAcctDr(), line.GetAmtAcctCr());
                    //  --  Converted Amounts
                    // no need to update converted amount here
                    //docLine.SetConvertedAmt(_VAB_AccountBook_ID, line.GetAmtAcctDr(), line.GetAmtAcctCr());
                    //  --  Account
                    MVABAccount account = line.GetAccount();
                    docLine.SetAccount(account);
                    //  -- Quantity
                    docLine.SetQty(line.GetQty(), false);
                    // -- Date
                    docLine.SetDateAcct(journal.GetDateAcct());
                    //	--	Organization of Line was set to Org of Account

                    // Set Description
                    docLine.SetDescription(line.GetDescription());
                    // set primary key value 
                    docLine.SetPrimaryKeyValue(line.GetVAGL_JRNLLine_ID());
                    // set GL journal line table ID
                    docLine.SetLineTable_ID(line.Get_Table_ID());

                    list.Add(docLine);
                }
                else
                {
                    string sql = "SELECT * FROM VAGL_LineDimension WHERE VAGL_JRNLLine_ID=" + line.Get_ID();
                    DataSet ds = DB.ExecuteDataset(sql);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                        {
                            DataRow dr = ds.Tables[0].Rows[m];
                            X_VAGL_LineDimension lDim = new X_VAGL_LineDimension(GetCtx(), dr, null);

                            DocLine docLine = new DocLine(lDim, this);
                            //  --  Source Amounts


                            decimal cRate = line.GetCurrencyRate();
                            if (cRate == 0)
                            {
                                cRate = 1;
                            }
                            decimal amtAcctCr = 0;
                            decimal amtAcctDr = 0;

                            //MAcctSchema mSc = new MAcctSchema(GetCtx(), _VAB_AccountBook_ID, null);


                            if (line.GetAmtSourceDr() != 0)
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
                            //docLine.SetConvertedAmt(_VAB_AccountBook_ID, amtAcctDr, amtAcctCr);
                            //  --  Account
                            MVABAccount account = line.GetAccount();


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
                            docLine.SetPrimaryKeyValue(line.GetVAGL_JRNLLine_ID());
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
        private DocLine SetUserDimension(X_VAGL_LineDimension journalLineDimension, DocLine docLine)
        {
            if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement1 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement1(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement2 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement2(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement3 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement3(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement4 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement4(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement5 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement5(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement6 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement6(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement7 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement7(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement8 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement8(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType() == MVAGLJRNLLine.ELEMENTTYPE_UserElement9 && !String.IsNullOrEmpty(journalLineDimension.GetDimensionValue()))
            {
                docLine.SetUserElement9(Convert.ToInt32(journalLineDimension.GetDimensionValue()));
            }
            else if (journalLineDimension.GetLineType().Equals(MVAGLJRNLLine.ELEMENTTYPE_OrgTrx) && journalLineDimension.GetOrg_ID() > 0)
            {
                docLine.SetVAF_OrgTrx_ID(Convert.ToInt32(journalLineDimension.GetOrg_ID()));
            }
            else if (journalLineDimension.GetLineType().Equals(MVAGLJRNLLine.ELEMENTTYPE_Organization) && journalLineDimension.GetOrg_ID() > 0)
            {
                docLine.SetVAF_Org_ID(Convert.ToInt32(journalLineDimension.GetOrg_ID()));
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
        public override List<Fact> CreateFacts(MVABAccountBook as1)
        {
            List<Fact> facts = new List<Fact>();
            //	Other Acct Schema
            // need to Post GL Journal for Multiple Accounting Schema that's why commented this section
            //if (as1.GetVAB_AccountBook_ID() != _VAB_AccountBook_ID)
            //{
            //    return facts;
            //}

            //  create Fact Header
            Fact fact = new Fact(this, as1, _PostingType);

            // get conversion rate from Assigned accounting schema tab - 
            Decimal conversionRate = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT CurrencyRate FROM VAGL_AssignAcctSchema WHERE 
                                     VAB_AccountBook_ID = " + as1.GetVAB_AccountBook_ID() + " AND VAGL_JRNL_ID = " + record_Id, null, null));

            //  GLJ
            if (GetDocumentType().Equals(MVABMasterDocType.DOCBASETYPE_GLJOURNAL))
            {
                //  account     DR      CR
                for (int i = 0; i < _lines.Length; i++)
                {

                    // need to Post GL Journal for Multiple Accounting Schema that's why commented this condition
                    //if (_lines[i].GetVAB_AccountBook_ID() == as1.GetVAB_AccountBook_ID())
                    //{
                    // set conversion rate on line, so that amount to be converted based on that multiply rate 
                    _lines[i].SetConversionRate(conversionRate);
                    fact.CreateLine(_lines[i],
                                    _lines[i].GetAccount(),
                                    GetVAB_Currency_ID(),
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
