/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_Production
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                  Table:              M_Production (325)
 *                  Document Types:     MMP
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
    public class Doc_Production : Doc
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Production(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(X_M_Production), idr, MDocBaseType.DOCBASETYPE_MATERIALPRODUCTION, trxName)
        {

        }
        public Doc_Production(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(X_M_Production), dr, MDocBaseType.DOCBASETYPE_MATERIALPRODUCTION, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetC_Currency_ID(NO_CURRENCY);
            X_M_Production prod = (X_M_Production)GetPO();
            SetDateDoc(prod.GetMovementDate());
            SetDateAcct(prod.GetMovementDate());
            //	Contained Objects
            _lines = LoadLines(prod);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }

        /// <summary>
        /// Load Invoice Line
        /// </summary>
        /// <param name="prod">production</param>
        /// <returns> DoaLine Array</returns>
        private DocLine[] LoadLines(X_M_Production prod)
        {
            List<DocLine> list = new List<DocLine>();
            //	Production
            //	-- ProductionPlan
            //	-- -- ProductionLine	- the real level
            String sqlPP = "SELECT * FROM M_ProductionPlan pp "
                + "WHERE pp.M_Production_ID=@param1 "
                + "ORDER BY pp.Line";
            IDataReader idrPP = null;

            String sqlPL = "SELECT * FROM M_ProductionLine pl "
                + "WHERE pl.M_ProductionPlan_ID=@param2 "
                + "ORDER BY pl.Line";
            IDataReader idrPL = null;

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", Get_ID());
                idrPP = DataBase.DB.ExecuteReader(sqlPP, param, GetTrx());
                //idrPP.setInt(1, get_ID());
                //ResultSet rsPP = idrPP.executeQuery();
                while (idrPP.Read())
                {
                    int M_Product_ID = Utility.Util.GetValueOfInt(idrPP["M_Product_ID"]);
                    int M_ProductionPlan_ID = Utility.Util.GetValueOfInt(idrPP["M_ProductionPlan_ID"]);
                    //
                    try
                    {
                        param = new SqlParameter[1];
                        param[0] = new SqlParameter("@param2", M_ProductionPlan_ID);
                        idrPL = DataBase.DB.ExecuteReader(sqlPL, param, GetTrx());
                        //idrPL.setInt(1, M_ProductionPlan_ID);
                        //ResultSet rsPL = idrPL.executeQuery();
                        while (idrPL.Read())
                        {
                            X_M_ProductionLine line = new X_M_ProductionLine(GetCtx(), idrPL, GetTrx());
                            if (Env.Signum(line.GetMovementQty()) == 0)
                            {
                                log.Info("LineQty=0 - " + line);
                                continue;
                            }
                            DocLine docLine = new DocLine(line, this);
                            docLine.SetQty(line.GetMovementQty(), false);
                            //	Identify finished BOM Product
                            docLine.SetProductionBOM(line.GetM_Product_ID() == M_Product_ID);
                            //
                            log.Fine(docLine.ToString());
                            list.Add(docLine);
                        }
                        idrPL.Close();
                    }
                    catch (Exception ee)
                    {
                        if (idrPL != null)
                        {
                            idrPL.Close();
                            idrPL = null;
                        }
                        log.Log(Level.SEVERE, sqlPL, ee);
                    }
                }
                idrPP.Close();
            }
            catch (Exception e)
            {
                if (idrPP != null)
                {
                    idrPP.Close();
                    idrPP = null;
                }
                log.Log(Level.SEVERE, sqlPP, e);
            }
            //	Return Array
            DocLine[] dl = new DocLine[list.Count];
            dl = list.ToArray();
            return dl;
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
        ///  MMP.
        ///  <pre>
        ///  Production
        ///      Inventory       DR      CR
        ///  </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetC_Currency_ID(as1.GetC_Currency_ID());

            //  Line pointer
            FactLine fl = null;
            for (int i = 0; i < _lines.Length; i++)
            {
                DocLine line = _lines[i];
                //	Calculate Costs
                Decimal? costs = null;
                if (line.IsProductionBOM())
                {
                    //	Get BOM Cost - Sum of individual lines
                    Decimal bomCost = Env.ZERO;
                    for (int ii = 0; ii < _lines.Length; ii++)
                    {
                        DocLine line0 = _lines[ii];
                        if (line0.GetM_ProductionPlan_ID() != line.GetM_ProductionPlan_ID())
                        {
                            continue;
                        }
                        if (!line0.IsProductionBOM())
                        {
                            bomCost = Decimal.Add(bomCost, line0.GetProductCosts(as1, line.GetAD_Org_ID(), false));
                        }
                    }
                    costs = Decimal.Negate(bomCost);
                }
                else
                {
                    costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), false);
                }

                //  Inventory       DR      CR
                fl = fact.CreateLine(line,
                    line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1),
                    as1.GetC_Currency_ID(), costs);
                if (fl == null)
                {
                    _error = "No Costs for Line " + line.GetLine() + " - " + line;
                    return null;
                }
                fl.SetM_Locator_ID(line.GetM_Locator_ID());
                fl.SetQty(line.GetQty());

                //	Cost Detail
                String description = line.GetDescription();
                if (description == null)
                {
                    description = "";
                }
                if (line.IsProductionBOM())
                {
                    description += "(*)";
                }

                if (!IsPosted())
                {
                    MCostDetail.CreateProduction(as1, line.GetAD_Org_ID(),
                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                        line.Get_ID(), 0,
                       Utility.Util.GetValueOfInt(costs), line.GetQty().Value,
                        description, GetTrx(), GetRectifyingProcess());
                }
            }
            //
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }
    }
}
