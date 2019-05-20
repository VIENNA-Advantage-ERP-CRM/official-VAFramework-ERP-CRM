/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_InOut
 * Purpose        : Post Shipment/Receipt Documents.
 *                  <pre>
 *                  Table:              M_InOut (319)
 *                  Document Types:     MMS, MMR
 *                  </pre>
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
    public class Doc_InOut : Doc
    {

        //Match Requirement		
        private String _MatchRequirementR = null;
        //Match Problem Info	
        private String _matchProblem = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass">accounting schemata</param>
        /// <param name="idr">record</param>
        /// <param name="trxName">trx</param>
        public Doc_InOut(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MInOut), idr, null, trxName)
        {

        }
        public Doc_InOut(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MInOut), dr, null, trxName)
        {

        }

        /// <summary>
        ///  Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetC_Currency_ID(NO_CURRENCY);
            MInOut inout = (MInOut)GetPO();
            SetDateDoc(inout.GetMovementDate());
            //
            _MatchRequirementR = X_AD_ClientInfo.MATCHREQUIREMENTR_None;
            if (!inout.IsSOTrx())
            {
                _MatchRequirementR = MClientInfo.Get(GetCtx(), inout.GetAD_Client_ID())
                    .GetMatchRequirementR();
                String mr = inout.GetMatchRequirementR();
                if (mr == null)
                {
                    inout.SetMatchRequirementR(_MatchRequirementR);
                }
                else
                {
                    _MatchRequirementR = mr;
                }
            }

            //	Contained Objects
            _lines = LoadLines(inout);
            log.Fine("Lines=" + _lines.Length);
            if (_matchProblem == null || _matchProblem.Length == 0)
            {
                return null;
            }
            return _matchProblem.Substring(1).Trim();
        }

        /// <summary>
        /// Load Invoice Line
        /// </summary>
        /// <param name="inout">shipment/receipt</param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MInOut inout)
        {
            List<DocLine> list = new List<DocLine>();
            MInOutLine[] lines = inout.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInOutLine line = lines[i];
                if (line.IsDescription()
                    || line.GetM_Product_ID() == 0
                    || Env.Signum(line.GetMovementQty()) == 0)
                {
                    log.Finer("Ignored: " + line);
                    continue;
                }
                //	PO Matching
                if (_MatchRequirementR.Equals(X_M_InOut.MATCHREQUIREMENTR_PurchaseOrder)
                    || _MatchRequirementR.Equals(X_M_InOut.MATCHREQUIREMENTR_PurchaseOrderAndInvoice))
                {
                    Decimal poDiff = line.GetMatchPODifference();
                    if (Env.Signum(poDiff) != 0)
                    {
                        _matchProblem += "; Line=" + line.GetLine()
                            + " PO Match diff=" + poDiff;
                    }
                    else if (!line.IsMatchPOPosted())
                    {
                        _matchProblem += "; PO Match not posted for Line=" + line.GetLine();
                    }
                }
                //	Inv Matching
                else if (_MatchRequirementR.Equals(X_M_InOut.MATCHREQUIREMENTR_Invoice)
                    || _MatchRequirementR.Equals(X_M_InOut.MATCHREQUIREMENTR_PurchaseOrderAndInvoice))
                {
                    Decimal invDiff = line.GetMatchInvDifference();
                    if (Env.Signum(invDiff) != 0)
                    {
                        _matchProblem += "; Line=" + line.GetLine()
                            + " PO Match diff=" + invDiff;
                    }
                    else if (!line.IsMatchInvPosted())
                    {
                        _matchProblem += "; Inv Match not posted for Line=" + line.GetLine();
                    }
                }

                DocLine docLine = new DocLine(line, this);
                Decimal Qty = line.GetMovementQty();
                docLine.SetQty(Qty, GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_MATERIALDELIVERY));    //  sets Trx and Storage Qty
                //
                log.Fine(docLine.ToString());
                list.Add(docLine);
            }

            //	Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
        }

        /// <summary>
        /// Get Balance
        /// </summary>
        /// <returns>Zero (always balanced)</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        /// MMS, MMR.
        /// <pre>
        /// Shipment
        /// CoGS (RevOrg)   DR
        /// Inventory               CR
        /// Shipment of Project Issue
        /// CoGS            DR
        /// Project                 CR
        /// Receipt
        /// Inventory       DR
        /// NotInvoicedReceipt      CR
        /// </pre>
        /// </summary>
        /// <param name="as1">accounting schema</param>
        /// <returns>Fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetC_Currency_ID(as1.GetC_Currency_ID());

            //  Line pointers
            FactLine dr = null;
            FactLine cr = null;

            //  *** Sales - Shipment
            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_MATERIALDELIVERY))
            {
                for (int i = 0; i < _lines.Length; i++)
                {
                    DocLine line = _lines[i];
                    MInOutLine sLine = new MInOutLine(GetCtx(), line.Get_ID(), null);
                    Decimal costs = 0;
                    if (sLine.GetA_Asset_ID() > 0)
                    {
                        costs = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT cost.CUrrentcostPrice
                                                                            FROM m_cost cost
                                                                            INNER JOIN A_Asset ass
                                                                            ON(ass.a_asset_ID=cost.a_asset_ID)
                                                                            INNER JOIN M_InOutLine IOL
                                                                            ON(IOL.A_Asset_ID       =ass.A_Asset_ID)
                                                                            WHERE IOL.M_InOutLine_ID=" + sLine.GetM_InOutLine_ID() + @"
                                                                              ORDER By cost.created desc"));
                        // Change if Cost not found against Asset then get Product Cost
                        if (Env.Signum(costs) == 0)	//	zero costs OK
                        {
                            costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), true);
                        }
                    }
                    else
                    {
                        costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), true);
                    }

                    if (Env.Signum(costs) == 0)	//	zero costs OK
                    {
                        MProduct product = line.GetProduct();
                        if (product.IsStocked())
                        {
                            _error = "No Costs for " + line.GetProduct().GetName();
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        else	//	ignore service
                        {
                            continue;
                        }
                    }

                    if (!IsReturnTrx())
                    {
                        //  CoGS            DR
                        dr = fact.CreateLine(line,
                                line.GetAccount(ProductCost.ACCTTYPE_P_Cogs, as1),
                                as1.GetC_Currency_ID(), costs, null);
                        if (dr == null)
                        {
                            _error = "FactLine DR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        dr.SetM_Locator_ID(line.GetM_Locator_ID());
                        dr.SetLocationFromLocator(line.GetM_Locator_ID(), true);    //  from Loc
                        dr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), false);  //  to Loc
                        dr.SetAD_Org_ID(line.GetOrder_Org_ID());		//	Revenue X-Org
                        dr.SetQty(Decimal.Negate(line.GetQty().Value));

                        //  Inventory               CR
                        cr = fact.CreateLine(line, line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1),
                            as1.GetC_Currency_ID(), null, costs);
                        if (cr == null)
                        {
                            _error = "FactLine CR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        cr.SetM_Locator_ID(line.GetM_Locator_ID());
                        cr.SetLocationFromLocator(line.GetM_Locator_ID(), true);    // from Loc
                        cr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), false);  // to Loc
                    }
                    else // Reverse accounting entries for returns
                    {
                        //				  CoGS            CR
                        cr = fact.CreateLine(line,
                                line.GetAccount(ProductCost.ACCTTYPE_P_Cogs, as1),
                                as1.GetC_Currency_ID(), null, costs);
                        if (cr == null)
                        {
                            _error = "FactLine CR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        cr.SetM_Locator_ID(line.GetM_Locator_ID());
                        cr.SetLocationFromLocator(line.GetM_Locator_ID(), true);    //  from Loc
                        cr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), false);  //  to Loc
                        cr.SetAD_Org_ID(line.GetOrder_Org_ID());		//	Revenue X-Org
                        cr.SetQty(Decimal.Negate(line.GetQty().Value));

                        //  Inventory               DR
                        dr = fact.CreateLine(line,
                            line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1),
                            as1.GetC_Currency_ID(), costs, null);
                        if (dr == null)
                        {
                            _error = "FactLine DR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        dr.SetM_Locator_ID(line.GetM_Locator_ID());
                        dr.SetLocationFromLocator(line.GetM_Locator_ID(), true);    // from Loc
                        dr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), false);  // to Loc
                    }
                    //
                    if (line.GetM_Product_ID() != 0)
                    {
                        if (!IsPosted())
                        {
                            MCostDetail.CreateShipment(as1, line.GetAD_Org_ID(),
                                line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                line.Get_ID(), 0,
                                costs, IsReturnTrx() ? Decimal.Negate(line.GetQty().Value) : line.GetQty().Value,
                                line.GetDescription(), true, GetTrx(), GetRectifyingProcess());
                        }
                    }
                }	//	for all lines

                if (!IsPosted())
                {
                    UpdateProductInfo(as1.GetC_AcctSchema_ID());     //  only for SO!
                }
            }	//	Shipment

            //  *** Purchasing - Receipt
            else if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_MATERIALRECEIPT))
            {
                for (int i = 0; i < _lines.Length; i++)
                {
                    Decimal costs = 0;
                    DocLine line = _lines[i];
                    MProduct product = line.GetProduct();
                    /***********************************************************/
                    //05,Sep,2011
                    //Special Check to restic Price varience posting in case of
                    //AvarageInvoice Selected on product Category.Then Neglact the AverageInvoice Cost
                    MProductCategoryAcct pca = MProductCategoryAcct.Get(product.GetCtx(),
                product.GetM_Product_Category_ID(), as1.GetC_AcctSchema_ID(), null);
                    try
                    {
                        if (as1.IsNotPostPOVariance() && line.GetC_OrderLine_ID() > 0)
                        {
                            MOrderLine oLine = new MOrderLine(product.GetCtx(), line.GetC_OrderLine_ID(), null);
                            MOrder order = new MOrder(product.GetCtx(), oLine.GetC_Order_ID(), null);
                            Decimal convertedCost = MConversionRate.Convert(product.GetCtx(),
                                oLine.GetPriceEntered(), order.GetC_Currency_ID(), as1.GetC_Currency_ID(),
                                line.GetDateAcct(), order.GetC_ConversionType_ID(),
                                oLine.GetAD_Client_ID(), line.GetAD_Org_ID());

                            costs = Decimal.Multiply(convertedCost, oLine.GetQtyEntered());

                        }
                        else
                        {
                            costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), false);	//	non-zero costs
                        }
                    }
                    catch (Exception ex)
                    {
                        log.SaveError("AccountSchemaColumnError", ex);
                        costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), false);	//	non-zero costs
                    }
                    /***********************************************************/

                    //Decimal costs = costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), false);	//	non-zero costs

                    if ( Env.Signum(costs) == 0)
                    {
                        _error = "Resubmit - No Costs for " + product.GetName();
                        log.Log(Level.WARNING, _error);
                        return null;
                    }
                    //  Inventory/Asset			DR
                    MAccount assets = line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1);
                    if (product.IsService())
                        assets = line.GetAccount(ProductCost.ACCTTYPE_P_Expense, as1);

                    if (!IsReturnTrx())
                    {
                        //  Inventory/Asset			DR
                        dr = fact.CreateLine(line, assets,
                            as1.GetC_Currency_ID(), costs, null);
                        if (dr == null)
                        {
                            _error = "DR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        dr.SetM_Locator_ID(line.GetM_Locator_ID());
                        dr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), true);   // from Loc
                        dr.SetLocationFromLocator(line.GetM_Locator_ID(), false);   // to Loc
                        //  NotInvoicedReceipt				CR
                        cr = fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_NotInvoicedReceipts, as1),
                            as1.GetC_Currency_ID(), null, costs);
                        if (cr == null)
                        {
                            _error = "CR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        cr.SetM_Locator_ID(line.GetM_Locator_ID());
                        cr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), true);   //  from Loc
                        cr.SetLocationFromLocator(line.GetM_Locator_ID(), false);   //  to Loc
                        cr.SetQty(Decimal.Negate(line.GetQty().Value));
                    }
                    else // reverse accounting entries for returns
                    {
                        //  Inventory/Asset			CR
                        cr = fact.CreateLine(line, assets,
                                as1.GetC_Currency_ID(), null, costs);
                        if (cr == null)
                        {
                            _error = "CR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        cr.SetM_Locator_ID(line.GetM_Locator_ID());
                        cr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), true);   // from Loc
                        cr.SetLocationFromLocator(line.GetM_Locator_ID(), false);   // to Loc
                        //  NotInvoicedReceipt				DR
                        dr = fact.CreateLine(line,
                            GetAccount(Doc.ACCTTYPE_NotInvoicedReceipts, as1),
                            as1.GetC_Currency_ID(), costs, null);
                        if (dr == null)
                        {
                            _error = "DR not created: " + line;
                            log.Log(Level.WARNING, _error);
                            return null;
                        }
                        dr.SetM_Locator_ID(line.GetM_Locator_ID());
                        dr.SetLocationFromBPartner(GetC_BPartner_Location_ID(), true);   //  from Loc
                        dr.SetLocationFromLocator(line.GetM_Locator_ID(), false);   //  to Loc
                        dr.SetQty(Decimal.Negate(line.GetQty().Value));

                    }
                }
            }	//	Receipt
            else
            {
                _error = "DocumentType unknown: " + GetDocumentType();
                log.Log(Level.SEVERE, _error);
                return null;
            }
            //
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }


        /// <summary>
        /// Update Sales Order Costing Product Info (old).
        /// Purchase side handeled in Invoice Matching.
        /// <br>
        /// decrease average cumulatives
        /// @deprecated old costing
        /// </summary>
        /// <param name="C_AcctSchema_ID">accounting schema</param>
        private void UpdateProductInfo(int C_AcctSchema_ID)
        {
            log.Fine("M_InOut_ID=" + Get_ID());
            //	Old Model
            StringBuilder sql = new StringBuilder(
                "UPDATE M_Product_Costing pc "
                + "SET (CostAverageCumQty, CostAverageCumAmt)="
                + "(SELECT CostAverageCumQty - SUM(il.MovementQty),"
                + " CostAverageCumAmt - SUM(il.MovementQty*CurrentCostPrice) "
                + "FROM M_InOutLine il "
                + "WHERE pc.M_Product_ID=il.M_Product_ID"
                + " AND il.M_InOut_ID=").Append(Get_ID()).Append(") ")
                .Append("WHERE EXISTS (SELECT * "
                + "FROM M_InOutLine il "
                + "WHERE pc.M_Product_ID=il.M_Product_ID"
                + " AND il.M_InOut_ID=").Append(Get_ID()).Append(")");

            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
            log.Fine("M_Product_Costing - Updated=" + no);
            //
        }
    }
}
