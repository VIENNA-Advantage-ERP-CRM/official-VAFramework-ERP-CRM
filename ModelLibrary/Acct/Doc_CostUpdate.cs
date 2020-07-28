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
    public class Doc_CostUpdate : Doc
    {
        #region Private Variables
        private MProductCategory mpc = null;
        private MCostElement m_ce;
        private String costingMethod;
        private String costingLevel;
        MCostUpdate costupdate;

        #endregion

        public Doc_CostUpdate(MAcctSchema[] ass, DataRow dr, Trx trx)
            : base(ass, typeof(MCostUpdate), dr, MDocBaseType.DOCBASETYPE_STANDARDCOSTUPDATE, trx)
        {
        }

        public Doc_CostUpdate(MAcctSchema[] ass, IDataReader idr, Trx trx)
            : base(ass, typeof(MCostUpdate), idr, MDocBaseType.DOCBASETYPE_STANDARDCOSTUPDATE, trx)
        {
        }

        public override String LoadDocumentDetails()
        {
            costupdate = (MCostUpdate)GetPO();
            if (costupdate.GetM_Product_Category_ID() != 0)
                mpc = MProductCategory.Get(GetCtx(), costupdate.GetM_Product_Category_ID());

            _lines = LoadLines(costupdate);
            m_ce = MCostElement.GetMaterialCostElement(MClient.Get(GetCtx()), X_C_AcctSchema.COSTINGMETHOD_StandardCosting);
            SetDateAcct(costupdate.GetDateAcct());
            SetDateDoc(costupdate.GetDateAcct());
            return null;
        }

        private DocLine[] LoadLines(MCostUpdate costupdate)
        {
            List<DocLine> list = new List<DocLine>();
            MCostUpdateLine[] lines = costupdate.GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                MCostUpdateLine line = lines[i];
                DocLine docLine = new DocLine(line, this);
                list.Add(docLine);
            }

            DocLine[] dl = new DocLine[list.Count];
            dl = list.ToArray();
            return dl;
        }

        public override Decimal GetBalance()
        {
            return Env.ZERO;
        }

        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            List<Fact> facts = new List<Fact>();
            MProductCategoryAcct pca = null;
            String costingMethodOfSchema = as1.GetCostingMethod();
            String costingLevelOfSchema = as1.GetCostingLevel();

            // Get the costing method and the costing level of the product Category for the current accounting schema.
            if (mpc != null)
            {
                pca = MProductCategoryAcct.Get(GetCtx(), mpc.GetM_Product_Category_ID(), as1.GetC_AcctSchema_ID(), null);
                if (pca.GetCostingMethod() != null)
                    costingMethod = pca.GetCostingMethod();
                else
                    costingMethod = costingMethodOfSchema;

                if (pca.GetCostingLevel() != null)
                    costingLevel = pca.GetCostingLevel();
                else
                    costingLevel = costingLevelOfSchema;

                // proceed only if the costing method is standard
                if (!costingMethod.Equals(X_C_AcctSchema.COSTINGMETHOD_StandardCosting))
                    return facts;
            }

            Fact fact = new Fact(this, as1, Fact.POST_Actual);

            FactLine dr = null;
            FactLine cr = null;

            for (int i = 0; i < _lines.Length; i++)
            {
                DocLine line = _lines[i];

                if (mpc == null)
                {
                    pca = MProductCategoryAcct.Get(GetCtx(), MProduct.Get(GetCtx(), line.GetM_Product_ID()).
                                                   GetM_Product_Category_ID(),
                                                   as1.GetC_AcctSchema_ID(), null);
                    if (pca.GetCostingMethod() != null)
                        costingMethod = pca.GetCostingMethod();
                    else
                        costingMethod = costingMethodOfSchema;

                    if (pca.GetCostingLevel() != null)
                        costingLevel = pca.GetCostingLevel();
                    else
                        costingLevel = costingLevelOfSchema;

                    // proceed only if the costing method is standard
                    if (!costingMethod.Equals(X_C_AcctSchema.COSTINGMETHOD_StandardCosting))
                        return facts;

                }
                Decimal currentCost = Env.ZERO;
                Decimal oldCost = Env.ZERO;
                Decimal Qty = Env.ZERO;
                String sql = " SELECT Sum(CurrentCostPrice), Sum(LastCostPrice), Sum(CurrentQty)"
                           + " FROM M_Cost WHERE M_Product_ID = " + line.GetM_Product_ID()
                           + " AND C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID()
                           + " AND M_CostElement_ID = " + m_ce.GetM_CostElement_ID()
                           + " AND M_CostType_ID = " + as1.GetM_CostType_ID()
                           + " AND AD_Client_ID = " + GetAD_Client_ID();
                if (costingLevel.Equals(X_C_AcctSchema.COSTINGLEVEL_Client))
                {
                    sql += " AND AD_Org_ID = 0"
                        + " AND M_AttributeSetInstance_ID  = 0";
                }
                else if (costingLevel.Equals(X_C_AcctSchema.COSTINGLEVEL_Organization))
                {
                    sql += " AND M_AttributeSetInstance_ID  = 0";
                }
                else if (costingLevel.Equals(X_C_AcctSchema.COSTINGLEVEL_BatchLot))
                {
                    sql += " AND AD_Org_ID = 0";
                }

                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null,GetTrx());
                    while (idr.Read())
                    {
                        currentCost = Util.GetValueOfDecimal(idr[0]);
                        oldCost = Util.GetValueOfDecimal(idr[1]);
                        Qty = Util.GetValueOfDecimal(idr[2]);
                        line.SetQty(Qty, costupdate.IsSOTrx());

                        Decimal PriceCost = Decimal.Subtract(currentCost, oldCost);
                        Decimal amt = Env.ZERO;
                        if (Env.Signum(PriceCost) != 0)
                        {
                            amt = Decimal.Multiply(Qty, PriceCost);
                        }

                        line.SetAmount(amt);

                        if (Env.Signum(amt) == 0)
                        {
                            continue;
                        }
                        MAccount db_acct, cr_acct;

                        /* Decide the Credit and Debit Accounts */
                        if (Env.Signum(amt) == 1)
                        {
                            db_acct = line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1);
                            cr_acct = line.GetAccount(ProductCost.ACCTTYPE_P_CostAdjustment, as1);
                        }
                        else
                        {
                            cr_acct = line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1);
                            db_acct = line.GetAccount(ProductCost.ACCTTYPE_P_CostAdjustment, as1);
                        }

                        /* Create Credit and Debit lines*/
                        dr = fact.CreateLine(line, db_acct, as1.GetC_Currency_ID(), Math.Abs(amt), null);
                        if (dr == null)
                        {
                            log.SaveError("NoProductCosts", "No Product Costs");
                            return null;
                        }

                        cr = fact.CreateLine(line, cr_acct, as1.GetC_Currency_ID(), null, Math.Abs(amt));
                        if (cr == null)
                        {
                            log.SaveError("NoProductCosts", "No Product Costs");
                            return null;
                        }
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
            }
            facts.Add(fact);
            return facts;
        }
    }
}
