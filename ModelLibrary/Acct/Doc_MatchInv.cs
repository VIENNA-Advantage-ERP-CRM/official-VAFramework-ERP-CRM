/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_MatchInv
 * Purpose        : Post MatchInv Documents.
 *                  <pre>
 *                  Table:              M_MatchInv (472)
 *                  Document Types:     MXI
 *                  </pre>
 *                  *                  Update Costing Records
 * Class Used     : Doc
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
    public class Doc_MatchInv : Doc
    {
        //Invoice Line			
        private MInvoiceLine _invoiceLine = null;
        // Material Receipt		
        private MInOutLine _receiptLine = null;

        private ProductCost _pc = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_MatchInv(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MMatchInv), idr, MDocBaseType.DOCBASETYPE_MATCHINVOICE, trxName)
        {

        }
        public Doc_MatchInv(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MMatchInv), dr, MDocBaseType.DOCBASETYPE_MATCHINVOICE, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetC_Currency_ID(Doc.NO_CURRENCY);
            MMatchInv matchInv = (MMatchInv)GetPO();
            SetDateDoc(matchInv.GetDateTrx());
            SetQty(matchInv.GetQty());
            //	Invoice Info
            int C_InvoiceLine_ID = matchInv.GetC_InvoiceLine_ID();
            _invoiceLine = new MInvoiceLine(GetCtx(), C_InvoiceLine_ID, null);
            //		BP for NotInvoicedReceipts
            int C_BPartner_ID = _invoiceLine.GetParent().GetC_BPartner_ID();
            SetC_BPartner_ID(C_BPartner_ID);
            //
            int M_InOutLine_ID = matchInv.GetM_InOutLine_ID();
            _receiptLine = new MInOutLine(GetCtx(), M_InOutLine_ID, null);
            //
            _pc = new ProductCost(GetCtx(),
                GetM_Product_ID(), matchInv.GetM_AttributeSetInstance_ID(), null);
            _pc.SetQty(GetQty());

            return null;
        }

        /// <summary>
        /// Get Source Currency Balance - subtracts line and tax amounts from total - no rounding
        /// </summary>
        /// <returns>Zero (always balanced)</returns>
        public override Decimal GetBalance()
        {
            return Env.ZERO;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  MXI.
        /// 	(single line)
        ///  <pre>
        ///      NotInvoicedReceipts     DR			(Receipt Org)
        ///      InventoryClearing               CR
        ///      InvoicePV               DR      CR  (difference)
        ///  Commitment
        /// 		Expense							CR
        /// 		Offset					DR
        ///  </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns></returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            List<Fact> facts = new List<Fact>();
            //  Nothing to do
            if (GetM_Product_ID() == 0								//	no Product
                || Env.Signum(GetQty().Value) == 0
                || Env.Signum(_receiptLine.GetMovementQty()) == 0)	//	Qty = 0
            {
                log.Fine("No Product/Qty - M_Product_ID=" + GetM_Product_ID()
                    + ",Qty=" + GetQty() + ",InOutQty=" + _receiptLine.GetMovementQty());
                return facts;
            }
            MMatchInv matchInv = (MMatchInv)GetPO();

            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetC_Currency_ID(as1.GetC_Currency_ID());

            /**	Needs to be handeled in PO Matching as1 no Receipt info
            if (_pc.isService())
            {
                log.Fine("Service - skipped");
                return fact;
            }
            **/


            //  NotInvoicedReceipt      DR
            //  From Receipt
            Decimal multiplier = Math.Abs(Decimal.Round(Decimal.Divide(GetQty().Value, _receiptLine.GetMovementQty()), 12, MidpointRounding.AwayFromZero));
            FactLine dr = fact.CreateLine(null,
                GetAccount(Doc.ACCTTYPE_NotInvoicedReceipts, as1),
                as1.GetC_Currency_ID(), Env.ONE, null);			// updated below
            if (dr == null)
            {
                _error = "No Product Costs";
                return null;
            }
            dr.SetQty(GetQty());
            //	dr.setM_Locator_ID(_receiptLine.getM_Locator_ID());
            //	MInOut receipt = _receiptLine.getParent();
            //	dr.setLocationFromBPartner(receipt.getC_BPartner_Location_ID(), true);	//  from Loc
            //	dr.setLocationFromLocator(_receiptLine.getM_Locator_ID(), false);		//  to Loc
            Decimal temp = dr.GetAcctBalance();
            //	Set AmtAcctCr/Dr from Receipt (sets also Project)
            if (!dr.UpdateReverseLine(MInOut.Table_ID, 		//	Amt updated
                _receiptLine.GetM_InOut_ID(), _receiptLine.GetM_InOutLine_ID(),
                multiplier))
            {
                _error = "Mat.Receipt not posted yet";
                return null;
            }
            log.Fine("CR - Amt(" + temp + "->" + dr.GetAcctBalance()
                + ") - " + dr.ToString());

            //  InventoryClearing               CR
            //  From Invoice
            MAccount expense = _pc.GetAccount(ProductCost.ACCTTYPE_P_InventoryClearing, as1);
            if (_pc.IsService())
            {
                expense = _pc.GetAccount(ProductCost.ACCTTYPE_P_Expense, as1);
            }
            Decimal LineNetAmt = _invoiceLine.GetLineNetAmt();
            multiplier = Math.Abs(Decimal.Round(Decimal.Divide(GetQty().Value, _invoiceLine.GetQtyInvoiced()), 12, MidpointRounding.AwayFromZero));
            if (multiplier.CompareTo(Env.ONE) != 0)
            {
                LineNetAmt = Decimal.Multiply(LineNetAmt, multiplier);
            }
            if (_pc.IsService())
            {
                LineNetAmt = dr.GetAcctBalance();	//	book out exact receipt amt
            }
            FactLine cr = null;
            if (as1.IsAccrual())
            {
                cr = fact.CreateLine(null, expense,
                    as1.GetC_Currency_ID(), null, LineNetAmt);		//	updated below
                if (cr == null)
                {
                    log.Fine("Line Net Amt=0 - M_Product_ID=" + GetM_Product_ID()
                        + ",Qty=" + GetQty() + ",InOutQty=" + _receiptLine.GetMovementQty());
                    facts.Add(fact);
                    return facts;
                }
                cr.SetQty(Decimal.Negate(GetQty().Value));
                temp = cr.GetAcctBalance();
                //	Set AmtAcctCr/Dr from Invoice (sets also Project)
                if (as1.IsAccrual() && !cr.UpdateReverseLine(MInvoice.Table_ID, 		//	Amt updated
                    _invoiceLine.GetC_Invoice_ID(), _invoiceLine.GetC_InvoiceLine_ID(), multiplier))
                {
                    _error = "Invoice not posted yet";
                    return null;
                }
                log.Fine("DR - Amt(" + temp + "->" + cr.GetAcctBalance()
                    + ") - " + cr.ToString());
            }
            else	//	Cash Acct
            {
                MInvoice invoice = _invoiceLine.GetParent();
                if (as1.GetC_Currency_ID() == invoice.GetC_Currency_ID())
                {
                    LineNetAmt = MConversionRate.Convert(GetCtx(), LineNetAmt,
                        invoice.GetC_Currency_ID(), as1.GetC_Currency_ID(),
                        invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(),
                        invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                }
                cr = fact.CreateLine(null, expense,
                    as1.GetC_Currency_ID(), null, LineNetAmt);

                cr.SetQty(Decimal.Negate(Decimal.Multiply(GetQty().Value, multiplier)));
            }
            cr.SetC_Activity_ID(_invoiceLine.GetC_Activity_ID());
            cr.SetC_Campaign_ID(_invoiceLine.GetC_Campaign_ID());
            cr.SetC_Project_ID(_invoiceLine.GetC_Project_ID());
            cr.SetC_UOM_ID(_invoiceLine.GetC_UOM_ID());
            cr.SetUser1_ID(_invoiceLine.GetUser1_ID());
            cr.SetUser2_ID(_invoiceLine.GetUser2_ID());


            //  Invoice Price Variance 	difference
            Decimal ipv = Decimal.Negate(Decimal.Add(cr.GetAcctBalance(), dr.GetAcctBalance()));
            if (Env.Signum(ipv) != 0)
            {
                FactLine pv = fact.CreateLine(null,
                    _pc.GetAccount(ProductCost.ACCTTYPE_P_IPV, as1),
                    as1.GetC_Currency_ID(), ipv);
                pv.SetC_Activity_ID(_invoiceLine.GetC_Activity_ID());
                pv.SetC_Campaign_ID(_invoiceLine.GetC_Campaign_ID());
                pv.SetC_Project_ID(_invoiceLine.GetC_Project_ID());
                pv.SetC_UOM_ID(_invoiceLine.GetC_UOM_ID());
                pv.SetUser1_ID(_invoiceLine.GetUser1_ID());
                pv.SetUser2_ID(_invoiceLine.GetUser2_ID());
            }
            log.Fine("IPV=" + ipv + "; Balance=" + fact.GetSourceBalance());

            MInOut inOut = _receiptLine.GetParent();
            bool isReturnTrx = inOut.IsReturnTrx();

            if (!IsPosted())
            {
                //	Cost Detail Record - data from Expense/IncClearing (CR) record
                MCostDetail.CreateInvoice(as1, GetAD_Org_ID(),
                    GetM_Product_ID(), matchInv.GetM_AttributeSetInstance_ID(),
                    _invoiceLine.GetC_InvoiceLine_ID(), 0,		//	No cost element
                    Decimal.Negate(cr.GetAcctBalance()), isReturnTrx ? Decimal.Negate(Utility.Util.GetValueOfDecimal(GetQty())) : Utility.Util.GetValueOfDecimal(GetQty()),		//	correcting
                    GetDescription(), GetTrx(), GetRectifyingProcess());

                //  Update Costing
                UpdateProductInfo(as1.GetC_AcctSchema_ID(),
                    MAcctSchema.COSTINGMETHOD_StandardCosting.Equals(as1.GetCostingMethod()));
            }
            //
            facts.Add(fact);

            /** Commitment release										****/
            if (as1.IsAccrual() && as1.IsCreateCommitment())
            {
                fact = Doc_Order.GetCommitmentRelease(as1, this,
                   Utility.Util.GetValueOfDecimal(GetQty()), _invoiceLine.GetC_InvoiceLine_ID(), Env.ONE);
                if (fact == null)
                {
                    return null;
                }
                facts.Add(fact);
            }	//	Commitment

            return facts;
        }

        /// <summary>
        /// Update Product Info (old).
        /// - Costing (CostStandardCumQty, CostStandardCumAmt, CostAverageCumQty, CostAverageCumAmt)
        /// </summary>
        /// <param name="C_AcctSchema_ID">accounting schema</param>
        /// <param name="standardCosting">true if std costing</param>
        /// <returns>true if updated</returns>
        /// @deprecated old costing
        private bool UpdateProductInfo(int C_AcctSchema_ID, bool standardCosting)
        {
            log.Fine("M_MatchInv_ID=" + Get_ID());

            //  update Product Costing Qty/Amt
            //  requires existence of currency conversion !!
            StringBuilder sql = new StringBuilder(
                "UPDATE M_Product_Costing pc "
                + "SET (CostStandardCumQty,CostStandardCumAmt, CostAverageCumQty,CostAverageCumAmt) = "
                + "(SELECT pc.CostStandardCumQty + m.Qty, " //jz ", "
                + "pc.CostStandardCumAmt + currencyConvert(il.PriceActual,i.C_Currency_ID,a.C_Currency_ID,i.DateInvoiced,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID)*m.Qty, "
                + "pc.CostAverageCumQty + m.Qty, "
                + "pc.CostAverageCumAmt + currencyConvert(il.PriceActual,i.C_Currency_ID,a.C_Currency_ID,i.DateInvoiced,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID)*m.Qty "
                + "FROM M_MatchInv m"
                + " INNER JOIN C_InvoiceLine il ON (m.C_InvoiceLine_ID=il.C_InvoiceLine_ID)"
                + " INNER JOIN C_Invoice i ON (il.C_Invoice_ID=i.C_Invoice_ID),"
                + " C_AcctSchema a "
                + "WHERE pc.C_AcctSchema_ID=a.C_AcctSchema_ID"
                + " AND pc.M_Product_ID=m.M_Product_ID"
                + " AND m.M_MatchInv_ID=").Append(Get_ID()).Append(")"
                //
                + "WHERE pc.C_AcctSchema_ID=").Append(C_AcctSchema_ID).Append(
                  " AND EXISTS (SELECT * FROM M_MatchInv m "
                    + "WHERE pc.M_Product_ID=m.M_Product_ID"
                    + " AND m.M_MatchInv_ID=").Append(Get_ID()).Append(")");

            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
            log.Fine("M_Product_Costing - Qty/Amt Updated #=" + no);

            //  Update Average Cost
            sql = new StringBuilder(
                "UPDATE M_Product_Costing "
                + "SET CostAverage = CostAverageCumAmt/DECODE(CostAverageCumQty, 0,1, CostAverageCumQty) "
                + "WHERE C_AcctSchema_ID=").Append(C_AcctSchema_ID)
                .Append(" AND M_Product_ID=").Append(GetM_Product_ID());

            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
            log.Fine("M_Product_Costing - AvgCost Updated #=" + no);


            //  Update Current Cost
            if (!standardCosting)
            {
                sql = new StringBuilder(
                    "UPDATE M_Product_Costing "
                    + "SET CurrentCostPrice = CostAverage "
                    + "WHERE C_AcctSchema_ID=").Append(C_AcctSchema_ID)
                    .Append(" AND M_Product_ID=").Append(GetM_Product_ID());

                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
                log.Fine("M_Product_Costing - CurrentCost Updated=" + no);
            }
            return true;
        }
    }
}
