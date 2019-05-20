/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_Requisition
 * Purpose        : Post Order Documents.
 *                  <pre>
 *                  Table:              M_Requisition
 *                  Document Types:     POR (Requisition)
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
    public class Doc_Requisition : Doc
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Requisition(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MRequisition), idr, MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION, trxName)
        {

        }
        public Doc_Requisition(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MRequisition), dr, MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetC_Currency_ID(NO_CURRENCY);
            MRequisition req = (MRequisition)GetPO();
            SetDateDoc(req.GetDateDoc());
            SetDateAcct(req.GetDateDoc());
            // Amounts
            SetAmount(AMTTYPE_Gross, req.GetTotalLines());
            SetAmount(AMTTYPE_Net, req.GetTotalLines());
            // Contained Objects
            _lines = LoadLines(req);
            // log.fine( "Lines=" + _lines.length + ", Taxes=" + m_taxes.length);
            return null;
        }

        /// <summary>
        /// Load Requisition Lines
        /// </summary>
        /// <param name="req">requisition</param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MRequisition req)
        {
            List<DocLine> list = new List<DocLine>();
            MRequisitionLine[] lines = req.GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                MRequisitionLine line = lines[i];
                DocLine docLine = new DocLine(line, this);
                Decimal Qty = line.GetQty();
                docLine.SetQty(Qty, false);
                //	Decimal PriceActual = 
                line.GetPriceActual();
                Decimal LineNetAmt = line.GetLineNetAmt();
                docLine.SetAmount(LineNetAmt);	 // DR
                list.Add(docLine);
            }
            // Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
        }

        /// <summary>
        /// Get Source Currency Balance - subtracts line and tax amounts from total -
        /// no rounding
        /// </summary>
        /// <returns>positive amount, if total invoice is bigger than lines</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = new Decimal(0.0);
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for POR.
        /// <pre>
        /// Reservation
        /// 	Expense		CR
        /// 	Offset			DR
        /// </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            List<Fact> facts = new List<Fact>();
            Fact fact = new Fact(this, as1, Fact.POST_Reservation);
            SetC_Currency_ID(as1.GetC_Currency_ID());
            //
            //	Decimal grossAmt = 
            GetAmount(Doc.AMTTYPE_Gross);
            // Commitment
            if (as1.IsCreateReservation())
            {
                Decimal total = Env.ZERO;
                for (int i = 0; i < _lines.Length; i++)
                {
                    DocLine line = _lines[i];
                    Decimal cost = line.GetAmtSource();
                    total = Decimal.Add(total, cost);
                    // Account
                    MAccount expense = line.GetAccount(ProductCost.ACCTTYPE_P_Expense, as1);
                    //
                    fact.CreateLine(line, expense, as1.GetC_Currency_ID(), cost, null);
                }
                // Offset
                MAccount offset = GetAccount(ACCTTYPE_CommitmentOffset, as1);
                if (offset == null)
                {
                    _error = "@NotFound@ @CommitmentOffset_Acct@";
                    log.Log(Level.SEVERE, _error);
                    return null;
                }
                fact.CreateLine(null, offset, GetC_Currency_ID(), null, total);
                facts.Add(fact);
            }

            return facts;
        }
    }
}
