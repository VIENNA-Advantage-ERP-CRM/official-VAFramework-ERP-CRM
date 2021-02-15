/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_Movement
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                  Table:              VAM_InventoryTransfer (323)
 *                  Document Types:     MMM
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
    public class Doc_Movement : Doc
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Movement(MVABAccountBook[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MMovement), idr, MVABMasterDocType.DOCBASETYPE_MATERIALMOVEMENT, trxName)
        {

        }
        public Doc_Movement(MVABAccountBook[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MMovement), dr, MVABMasterDocType.DOCBASETYPE_MATERIALMOVEMENT, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetVAB_Currency_ID(NO_CURRENCY);
            MMovement move = (MMovement)GetPO();
            SetDateDoc(move.GetMovementDate());
            SetDateAcct(move.GetMovementDate());
            //	Contained Objects
            _lines = LoadLines(move);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }

        /// <summary>
        /// Load Invoice Line
        /// </summary>
        /// <param name="move"></param>
        /// <returns>document lines (DocLine_Material)</returns>
        private DocLine[] LoadLines(MMovement move)
        {
            List<DocLine> list = new List<DocLine>();
            MMovementLine[] lines = move.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MMovementLine line = lines[i];
                DocLine docLine = new DocLine(line, this);
                docLine.SetQty(line.GetMovementQty(), false);
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
        /// <returns>balance (ZERO) - always balanced</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  MMM.
        ///  <pre>
        ///  Movement
        ///      Inventory       DR      CR
        ///      InventoryTo     DR      CR
        ///  </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>Fact</returns>
        public override List<Fact> CreateFacts(MVABAccountBook as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetVAB_Currency_ID(as1.GetVAB_Currency_ID());

            //  Line pointers
            FactLine dr = null;
            FactLine cr = null;

            for (int i = 0; i < _lines.Length; i++)
            {
                DocLine line = _lines[i];
                Decimal costs = line.GetProductCosts(as1, line.GetVAF_Org_ID(), false);

                //  ** Inventory       DR      CR
                dr = fact.CreateLine(line,
                    line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1),
                    as1.GetVAB_Currency_ID(), Decimal.Negate(costs));		//	from (-) CR
                if (dr == null)
                {
                    continue;
                }
                dr.SetVAM_Locator_ID(line.GetVAM_Locator_ID());
                dr.SetQty(Decimal.Negate(line.GetQty().Value));	//	outgoing

                //  ** InventoryTo     DR      CR
                cr = fact.CreateLine(line,
                    line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1),
                    as1.GetVAB_Currency_ID(), costs);			//	to (+) DR
                if (cr == null)
                {
                    continue;
                }
                cr.SetVAM_Locator_ID(line.GetVAM_LocatorTo_ID());
                cr.SetQty(line.GetQty());

                //	Only for between-org movements
                if (dr.GetVAF_Org_ID() != cr.GetVAF_Org_ID())
                {
                    String costingLevel = as1.GetCostingLevel();
                    MProductCategoryAcct pca = MProductCategoryAcct.Get(GetCtx(),
                        line.GetProduct().GetVAM_ProductCategory_ID(),
                        as1.GetVAB_AccountBook_ID(), GetTrx());
                    if (pca.GetCostingLevel() != null)
                    {
                        costingLevel = pca.GetCostingLevel();
                    }
                    if (!MVABAccountBook.COSTINGLEVEL_Organization.Equals(costingLevel))
                    {
                        continue;
                    }
                    //
                    String description = line.GetDescription();
                    if (description == null)
                    {
                        description = "";
                    }
                    if (!IsPosted())
                    {

                        //	Cost Detail From
                        MVAMProductCostDetail.CreateMovement(as1, dr.GetVAF_Org_ID(), 	//	locator org
                            line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                            line.Get_ID(), 0,
                            Decimal.Negate(costs), Decimal.Negate(line.GetQty().Value), true,
                            description + "(|->)", GetTrx(), GetRectifyingProcess());


                        //	Cost Detail To
                        MVAMProductCostDetail.CreateMovement(as1, cr.GetVAF_Org_ID(),	//	locator org 
                            line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                            line.Get_ID(), 0,
                            costs, line.GetQty().Value, false,
                            description + "(|<-)", GetTrx(), GetRectifyingProcess());
                    }
                }
            }

            //
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }
    }
}
