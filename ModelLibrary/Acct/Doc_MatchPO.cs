/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_MatchPO
 * Purpose        : Post MatchPO Documents.
 *                  <pre>
 *                  Table:              C_MatchPO (473)
 *                  Document Types:     MXP
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
    public class Doc_MatchPO : Doc
    {
        private int _VAB_OrderLine_ID = 0;
        private MVABOrderLine _oLine = null;
        //
        private int _VAM_Inv_InOutLine_ID = 0;
        private ProductCost _pc;
        private int _VAM_PFeature_SetInstance_ID = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_MatchPO(MVABAccountBook[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MVAMMatchPO), idr, MVABMasterDocType.DOCBASETYPE_MATCHPO, trxName)
        {

        }
        public Doc_MatchPO(MVABAccountBook[] ass,DataRow dr, Trx trxName)
            : base(ass, typeof(MVAMMatchPO), dr, MVABMasterDocType.DOCBASETYPE_MATCHPO, trxName)
        {

        }


        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetVAB_Currency_ID(Doc.NO_CURRENCY);
            MVAMMatchPO matchPO = (MVAMMatchPO)GetPO();
            SetDateDoc(matchPO.GetDateTrx());
            //
            _VAM_PFeature_SetInstance_ID = matchPO.GetVAM_PFeature_SetInstance_ID();
            SetQty(matchPO.GetQty());
            //
            _VAB_OrderLine_ID = matchPO.GetVAB_OrderLine_ID();
            _oLine = new MVABOrderLine(GetCtx(), _VAB_OrderLine_ID, GetTrxName());
            //
            _VAM_Inv_InOutLine_ID = matchPO.GetVAM_Inv_InOutLine_ID();
            //	m_VAB_InvoiceLine_ID = matchPO.getVAB_InvoiceLine_ID();
            //
            _pc = new ProductCost(GetCtx(),
                GetVAM_Product_ID(), _VAM_PFeature_SetInstance_ID, GetTrxName());
            _pc.SetQty(GetQty());
            return null;
        }


        /// <summary>
        /// Get Source Currency Balance - subtracts line and tax amounts from total - no rounding
        /// </summary>
        /// <returns>Zero - always balanced</returns>
        public override Decimal GetBalance()
        {
            return Env.ZERO;
        }


        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  MXP.
        ///  <pre>
        ///      Product PPV     <difference>
        ///      PPV_Offset                  <difference>
        ///  </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns></returns>
        public override List<Fact> CreateFacts(MVABAccountBook as1)
        {
            List<Fact> facts = new List<Fact>();
            //
            if (GetVAM_Product_ID() == 0		//  Nothing to do if no Product
                || Env.Signum(Utility.Util.GetValueOfDecimal(GetQty())) == 0
                || _VAM_Inv_InOutLine_ID == 0)	//  No posting if not matched to Shipment
            {
                log.Fine("No Product/Qty - VAM_Product_ID=" + GetVAM_Product_ID()
                    + ",Qty=" + GetQty());
                return facts;
            }

            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetVAB_Currency_ID(as1.GetVAB_Currency_ID());

            //	Purchase Order Line
            Decimal poCost = _oLine.GetPriceCost();
            if ( Env.Signum(poCost) == 0)
            {
                poCost = _oLine.GetPriceActual();
            }
            poCost = Decimal.Multiply(poCost,Utility.Util.GetValueOfDecimal(GetQty()));			//	Delivered so far
            //	Different currency
            if (_oLine.GetVAB_Currency_ID() != as1.GetVAB_Currency_ID())
            {
                MVABOrder order = _oLine.GetParent();
                Decimal rate = MVABExchangeRate.GetRate(
                    order.GetVAB_Currency_ID(), as1.GetVAB_Currency_ID(),
                    order.GetDateAcct(), order.GetVAB_CurrencyType_ID(),
                    _oLine.GetVAF_Client_ID(), _oLine.GetVAF_Org_ID());
                if (rate.ToString() == null)
                {
                    _error = "Purchase Order not convertible - " + as1.GetName();
                    return null;
                }
                poCost = Decimal.Multiply(poCost, rate);
                if (Env.Scale(poCost) > as1.GetCostingPrecision())
                {
                    poCost = Decimal.Round(poCost, as1.GetCostingPrecision(), MidpointRounding.AwayFromZero);
                }
            }

            MVABOrder order1 = _oLine.GetParent();
            bool isReturnTrx = order1.IsReturnTrx();
            log.Fine("Temp");

            if (!IsPosted())
            {
                //	Create PO Cost Detail Record firs
                MVAMVAMProductCostDetail.CreateOrder(as1, _oLine.GetVAF_Org_ID(),
                        GetVAM_Product_ID(), _VAM_PFeature_SetInstance_ID,
                        _VAB_OrderLine_ID, 0,		//	no cost element
                        isReturnTrx ? Decimal.Negate(poCost) : poCost, isReturnTrx ? Decimal.Negate(Utility.Util.GetValueOfDecimal(GetQty())) : Utility.Util.GetValueOfDecimal(GetQty()),			//	Delivered
                        _oLine.GetDescription(), GetTrx(), GetRectifyingProcess());
            }


            //	Current Costs
            String costingMethod = as1.GetCostingMethod();
            MVAMProduct product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());
            MVAMProductCategoryAcct pca = MVAMProductCategoryAcct.Get(GetCtx(),
                product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), GetTrx());
            if (pca.GetCostingMethod() != null)
            {
                costingMethod = pca.GetCostingMethod();
            }
            Decimal? costs = _pc.GetProductCosts(as1, GetVAF_Org_ID(), costingMethod, _VAB_OrderLine_ID, false);	//	non-zero costs

            //	No Costs yet - no PPV
            if (costs == null || Env.Signum(Utility.Util.GetValueOfDecimal(costs)) == 0)
            {
                _error = "Resubmit - No Costs for " + product.GetName();
                log.Log(Level.SEVERE, _error);
                return null;
            }

            //	Difference
            Decimal difference = Decimal.Subtract(poCost,Utility.Util.GetValueOfDecimal(costs));


            /***********************************************************************************/
            //05,Sep,2011
            //Special Check to restic Price varience posting in case of
            //AvarageInvoice Selected on product Category.Then Neglact the AverageInvoice Cost

            try
            {
                if (as1.IsNotPostPOVariance())
                {
                    difference = 0;
                }
            }
            catch (Exception ex)
            {
                log.SaveError("AccountSchemaColumnError", ex);
            }
            /***********************************************************************************/
            //	Nothing to post
            if (Env.Signum(difference) == 0)
            {
                log.Log(Level.FINE, "No Cost Difference for VAM_Product_ID=" + GetVAM_Product_ID());
                facts.Add(fact);
                return facts;
            }

            //  Product PPV
            FactLine cr = fact.CreateLine(null,
                _pc.GetAccount(ProductCost.ACCTTYPE_P_PPV, as1),
                as1.GetVAB_Currency_ID(), difference);
            if (cr != null)
            {
                cr.SetQty(GetQty());
                cr.SetVAB_BusinessPartner_ID(_oLine.GetVAB_BusinessPartner_ID());
                cr.SetVAB_BillingCode_ID(_oLine.GetVAB_BillingCode_ID());
                cr.SetVAB_Promotion_ID(_oLine.GetVAB_Promotion_ID());
                cr.SetVAB_Project_ID(_oLine.GetVAB_Project_ID());
                cr.SetVAB_UOM_ID(_oLine.GetVAB_UOM_ID());
                cr.SetUser1_ID(_oLine.GetUser1_ID());
                cr.SetUser2_ID(_oLine.GetUser2_ID());
            }

            //  PPV Offset
            FactLine dr = fact.CreateLine(null,
                GetAccount(Doc.ACCTTYPE_PPVOffset, as1),
                as1.GetVAB_Currency_ID(), Decimal.Negate(difference));
            if (dr != null)
            {
                dr.SetQty((Decimal?) Decimal.Negate(Utility.Util.GetValueOfDecimal(GetQty())));

                dr.SetVAB_BusinessPartner_ID(_oLine.GetVAB_BusinessPartner_ID());
                dr.SetVAB_BillingCode_ID(_oLine.GetVAB_BillingCode_ID());
                dr.SetVAB_Promotion_ID(_oLine.GetVAB_Promotion_ID());
                dr.SetVAB_Project_ID(_oLine.GetVAB_Project_ID());
                dr.SetVAB_UOM_ID(_oLine.GetVAB_UOM_ID());
                dr.SetUser1_ID(_oLine.GetUser1_ID());
                dr.SetUser2_ID(_oLine.GetUser2_ID());
            }
            //
            facts.Add(fact);
            return facts;
        }
    }
}
