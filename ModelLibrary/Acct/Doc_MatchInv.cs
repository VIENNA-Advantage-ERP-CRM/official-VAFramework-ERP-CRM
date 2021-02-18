/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_MatchInv
 * Purpose        : Post MatchInv Documents.
 *                  <pre>
 *                  Table:              VAM_MatchInvoice (472)
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
    public class Doc_MatchInv : Doc
    {
        //Invoice Line			
        private MVABInvoiceLine _invoiceLine = null;
        // Material Receipt		
        private MVAMInvInOutLine _receiptLine = null;

        private ProductCost _pc = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_MatchInv(MVABAccountBook[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MVAMMatchInvoice), idr, MVABMasterDocType.DOCBASETYPE_MATCHINVOICE, trxName)
        {

        }
        public Doc_MatchInv(MVABAccountBook[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MVAMMatchInvoice), dr, MVABMasterDocType.DOCBASETYPE_MATCHINVOICE, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetVAB_Currency_ID(Doc.NO_CURRENCY);
            MVAMMatchInvoice matchInv = (MVAMMatchInvoice)GetPO();
            SetDateDoc(matchInv.GetDateTrx());
            SetQty(matchInv.GetQty());
            //	Invoice Info
            int VAB_InvoiceLine_ID = matchInv.GetVAB_InvoiceLine_ID();
            _invoiceLine = new MVABInvoiceLine(GetCtx(), VAB_InvoiceLine_ID, null);
            //		BP for NotInvoicedReceipts
            int VAB_BusinessPartner_ID = _invoiceLine.GetParent().GetVAB_BusinessPartner_ID();
            SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
            //
            int VAM_Inv_InOutLine_ID = matchInv.GetVAM_Inv_InOutLine_ID();
            _receiptLine = new MVAMInvInOutLine(GetCtx(), VAM_Inv_InOutLine_ID, null);
            //
            _pc = new ProductCost(GetCtx(),
                GetVAM_Product_ID(), matchInv.GetVAM_PFeature_SetInstance_ID(), null);
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
        public override List<Fact> CreateFacts(MVABAccountBook as1)
        {
            List<Fact> facts = new List<Fact>();
            //  Nothing to do
            if (GetVAM_Product_ID() == 0								//	no Product
                || Env.Signum(GetQty().Value) == 0
                || Env.Signum(_receiptLine.GetMovementQty()) == 0)	//	Qty = 0
            {
                log.Fine("No Product/Qty - VAM_Product_ID=" + GetVAM_Product_ID()
                    + ",Qty=" + GetQty() + ",InOutQty=" + _receiptLine.GetMovementQty());
                return facts;
            }
            MVAMMatchInvoice matchInv = (MVAMMatchInvoice)GetPO();

            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetVAB_Currency_ID(as1.GetVAB_Currency_ID());

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
                as1.GetVAB_Currency_ID(), Env.ONE, null);			// updated below
            if (dr == null)
            {
                _error = "No Product Costs";
                return null;
            }
            dr.SetQty(GetQty());
            //	dr.setVAM_Locator_ID(_receiptLine.getVAM_Locator_ID());
            //	MVAMInvInOut receipt = _receiptLine.getParent();
            //	dr.setLocationFromBPartner(receipt.getVAB_BPart_Location_ID(), true);	//  from Loc
            //	dr.setLocationFroMVAMLocator(_receiptLine.getVAM_Locator_ID(), false);		//  to Loc
            Decimal temp = dr.GetAcctBalance();
            //	Set AmtAcctCr/Dr from Receipt (sets also Project)
            if (!dr.UpdateReverseLine(MVAMInvInOut.Table_ID, 		//	Amt updated
                _receiptLine.GetVAM_Inv_InOut_ID(), _receiptLine.GetVAM_Inv_InOutLine_ID(),
                multiplier))
            {
                _error = "Mat.Receipt not posted yet";
                return null;
            }
            log.Fine("CR - Amt(" + temp + "->" + dr.GetAcctBalance()
                + ") - " + dr.ToString());

            //  InventoryClearing               CR
            //  From Invoice
            MVABAccount expense = _pc.GetAccount(ProductCost.ACCTTYPE_P_InventoryClearing, as1);
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
                    as1.GetVAB_Currency_ID(), null, LineNetAmt);		//	updated below
                if (cr == null)
                {
                    log.Fine("Line Net Amt=0 - VAM_Product_ID=" + GetVAM_Product_ID()
                        + ",Qty=" + GetQty() + ",InOutQty=" + _receiptLine.GetMovementQty());
                    facts.Add(fact);
                    return facts;
                }
                cr.SetQty(Decimal.Negate(GetQty().Value));
                temp = cr.GetAcctBalance();
                //	Set AmtAcctCr/Dr from Invoice (sets also Project)
                if (as1.IsAccrual() && !cr.UpdateReverseLine(MVABInvoice.Table_ID, 		//	Amt updated
                    _invoiceLine.GetVAB_Invoice_ID(), _invoiceLine.GetVAB_InvoiceLine_ID(), multiplier))
                {
                    _error = "Invoice not posted yet";
                    return null;
                }
                log.Fine("DR - Amt(" + temp + "->" + cr.GetAcctBalance()
                    + ") - " + cr.ToString());
            }
            else	//	Cash Acct
            {
                MVABInvoice invoice = _invoiceLine.GetParent();
                if (as1.GetVAB_Currency_ID() == invoice.GetVAB_Currency_ID())
                {
                    LineNetAmt = MVABExchangeRate.Convert(GetCtx(), LineNetAmt,
                        invoice.GetVAB_Currency_ID(), as1.GetVAB_Currency_ID(),
                        invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(),
                        invoice.GetVAF_Client_ID(), invoice.GetVAF_Org_ID());
                }
                cr = fact.CreateLine(null, expense,
                    as1.GetVAB_Currency_ID(), null, LineNetAmt);

                cr.SetQty(Decimal.Negate(Decimal.Multiply(GetQty().Value, multiplier)));
            }
            cr.SetVAB_BillingCode_ID(_invoiceLine.GetVAB_BillingCode_ID());
            cr.SetVAB_Promotion_ID(_invoiceLine.GetVAB_Promotion_ID());
            cr.SetVAB_Project_ID(_invoiceLine.GetVAB_Project_ID());
            cr.SetVAB_UOM_ID(_invoiceLine.GetVAB_UOM_ID());
            cr.SetUser1_ID(_invoiceLine.GetUser1_ID());
            cr.SetUser2_ID(_invoiceLine.GetUser2_ID());


            //  Invoice Price Variance 	difference
            Decimal ipv = Decimal.Negate(Decimal.Add(cr.GetAcctBalance(), dr.GetAcctBalance()));
            if (Env.Signum(ipv) != 0)
            {
                FactLine pv = fact.CreateLine(null,
                    _pc.GetAccount(ProductCost.ACCTTYPE_P_IPV, as1),
                    as1.GetVAB_Currency_ID(), ipv);
                pv.SetVAB_BillingCode_ID(_invoiceLine.GetVAB_BillingCode_ID());
                pv.SetVAB_Promotion_ID(_invoiceLine.GetVAB_Promotion_ID());
                pv.SetVAB_Project_ID(_invoiceLine.GetVAB_Project_ID());
                pv.SetVAB_UOM_ID(_invoiceLine.GetVAB_UOM_ID());
                pv.SetUser1_ID(_invoiceLine.GetUser1_ID());
                pv.SetUser2_ID(_invoiceLine.GetUser2_ID());
            }
            log.Fine("IPV=" + ipv + "; Balance=" + fact.GetSourceBalance());

            MVAMInvInOut inOut = _receiptLine.GetParent();
            bool isReturnTrx = inOut.IsReturnTrx();

            if (!IsPosted())
            {
                //	Cost Detail Record - data from Expense/IncClearing (CR) record
                MVAMProductCostDetail.CreateInvoice(as1, GetVAF_Org_ID(),
                    GetVAM_Product_ID(), matchInv.GetVAM_PFeature_SetInstance_ID(),
                    _invoiceLine.GetVAB_InvoiceLine_ID(), 0,		//	No cost element
                    Decimal.Negate(cr.GetAcctBalance()), isReturnTrx ? Decimal.Negate(Utility.Util.GetValueOfDecimal(GetQty())) : Utility.Util.GetValueOfDecimal(GetQty()),		//	correcting
                    GetDescription(), GetTrx(), GetRectifyingProcess());

                //  Update Costing
                UpdateProductInfo(as1.GetVAB_AccountBook_ID(),
                    MVABAccountBook.COSTINGMETHOD_StandardCosting.Equals(as1.GetCostingMethod()));
            }
            //
            facts.Add(fact);

            /** Commitment release										****/
            if (as1.IsAccrual() && as1.IsCreateCommitment())
            {
                fact = Doc_Order.GetCommitmentRelease(as1, this,
                   Utility.Util.GetValueOfDecimal(GetQty()), _invoiceLine.GetVAB_InvoiceLine_ID(), Env.ONE);
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
        /// <param name="VAB_AccountBook_ID">accounting schema</param>
        /// <param name="standardCosting">true if std costing</param>
        /// <returns>true if updated</returns>
        /// @deprecated old costing
        private bool UpdateProductInfo(int VAB_AccountBook_ID, bool standardCosting)
        {
            log.Fine("VAM_MatchInvoice_ID=" + Get_ID());

            //  update Product Costing Qty/Amt
            //  requires existence of currency conversion !!
            StringBuilder sql = new StringBuilder(
                "UPDATE VAM_ProductCosting pc "
                + "SET (CostStandardCumQty,CostStandardCumAmt, CostAverageCumQty,CostAverageCumAmt) = "
                + "(SELECT pc.CostStandardCumQty + m.Qty, " //jz ", "
                + "pc.CostStandardCumAmt + currencyConvert(il.PriceActual,i.VAB_Currency_ID,a.VAB_Currency_ID,i.DateInvoiced,i.VAB_CurrencyType_ID,i.VAF_Client_ID,i.VAF_Org_ID)*m.Qty, "
                + "pc.CostAverageCumQty + m.Qty, "
                + "pc.CostAverageCumAmt + currencyConvert(il.PriceActual,i.VAB_Currency_ID,a.VAB_Currency_ID,i.DateInvoiced,i.VAB_CurrencyType_ID,i.VAF_Client_ID,i.VAF_Org_ID)*m.Qty "
                + "FROM VAM_MatchInvoice m"
                + " INNER JOIN VAB_InvoiceLine il ON (m.VAB_InvoiceLine_ID=il.VAB_InvoiceLine_ID)"
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID),"
                + " VAB_AccountBook a "
                + "WHERE pc.VAB_AccountBook_ID=a.VAB_AccountBook_ID"
                + " AND pc.VAM_Product_ID=m.VAM_Product_ID"
                + " AND m.VAM_MatchInvoice_ID=").Append(Get_ID()).Append(")"
                //
                + "WHERE pc.VAB_AccountBook_ID=").Append(VAB_AccountBook_ID).Append(
                  " AND EXISTS (SELECT * FROM VAM_MatchInvoice m "
                    + "WHERE pc.VAM_Product_ID=m.VAM_Product_ID"
                    + " AND m.VAM_MatchInvoice_ID=").Append(Get_ID()).Append(")");

            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
            log.Fine("VAM_ProductCosting - Qty/Amt Updated #=" + no);

            //  Update Average Cost
            sql = new StringBuilder(
                "UPDATE VAM_ProductCosting "
                + "SET CostAverage = CostAverageCumAmt/DECODE(CostAverageCumQty, 0,1, CostAverageCumQty) "
                + "WHERE VAB_AccountBook_ID=").Append(VAB_AccountBook_ID)
                .Append(" AND VAM_Product_ID=").Append(GetVAM_Product_ID());

            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
            log.Fine("VAM_ProductCosting - AvgCost Updated #=" + no);


            //  Update Current Cost
            if (!standardCosting)
            {
                sql = new StringBuilder(
                    "UPDATE VAM_ProductCosting "
                    + "SET CurrentCostPrice = CostAverage "
                    + "WHERE VAB_AccountBook_ID=").Append(VAB_AccountBook_ID)
                    .Append(" AND VAM_Product_ID=").Append(GetVAM_Product_ID());

                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
                log.Fine("VAM_ProductCosting - CurrentCost Updated=" + no);
            }
            return true;
        }
    }
}
