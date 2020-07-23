/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_InOut
 * Purpose        : Post Inventory Documents.
 *                  <pre>
 *                  Table:  M_Inventory (321)
 *                  Document Types:     MMI
 *                  </pre>
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
    public class Doc_Inventory : Doc
    {
        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Inventory(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MInventory), idr, MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY, trxName)
        {

        }
        public Doc_Inventory(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MInventory), dr, MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetC_Currency_ID(NO_CURRENCY);
            MInventory inventory = (MInventory)GetPO();
            SetDateDoc(inventory.GetMovementDate());
            SetDateAcct(inventory.GetMovementDate());
            //	Contained Objects
            _lines = LoadLines(inventory);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }

        /// <summary>
        /// Load Invoice Line
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MInventory inventory)
        {
            List<DocLine> list = new List<DocLine>();
            MInventoryLine[] lines = inventory.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInventoryLine line = lines[i];
                //	nothing to post
                if (line.GetQtyBook().CompareTo(line.GetQtyCount()) == 0
                    && Env.Signum(line.GetQtyInternalUse()) == 0)
                {
                    continue;
                }
                //
                DocLine docLine = new DocLine(line, this);
                Decimal Qty = line.GetQtyInternalUse();
                if (Env.Signum(Qty) != 0)
                {
                    Qty = Decimal.Negate(Qty);		//	Internal Use entered positive
                }
                else
                {
                    Decimal QtyBook = line.GetQtyBook();
                    Decimal QtyCount = line.GetQtyCount();
                    Qty = Decimal.Subtract(QtyCount, QtyBook);
                }
                docLine.SetQty(Qty, false);		// -5 => -5
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
        ///  MMI.
        ///  <pre>
        ///  Inventory
        ///      Inventory       DR      CR
        ///      InventoryDiff   DR      CR   (or Charge)
        ///  </pre>
        /// </summary>
        /// <param name="?"></param>
        /// <returns> Fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetC_Currency_ID(as1.GetC_Currency_ID());

            //  Line pointers
            FactLine dr = null;
            FactLine cr = null;

            for (int i = 0; i < _lines.Length; i++)
            {
                DocLine line = _lines[i];
                Decimal costs = line.GetProductCosts(as1, line.GetAD_Org_ID(), false);
                if ( Env.Signum(costs) == 0)
                {
                    _error = "No Costs for " + line.GetProduct().GetName();
                    return null;
                }
                //  Inventory       DR      CR
                dr = fact.CreateLine(line,
                    line.GetAccount(ProductCost.ACCTTYPE_P_Asset, as1),
                    as1.GetC_Currency_ID(), costs);
                //  may be zero difference - no line created.
                if (dr == null)
                {
                    continue;
                }
                dr.SetM_Locator_ID(line.GetM_Locator_ID());

                //  InventoryDiff   DR      CR
                //	or Charge
                MAccount invDiff = line.GetChargeAccount(as1, Decimal.Negate(costs));
                if (invDiff == null)
                {
                    invDiff = GetAccount(Doc.ACCTTYPE_InvDifferences, as1);
                }
                cr = fact.CreateLine(line, invDiff,
                    as1.GetC_Currency_ID(), Decimal.Negate(costs));
                if (cr == null)
                {
                    continue;
                }
                cr.SetM_Locator_ID(line.GetM_Locator_ID());
                cr.SetQty(Decimal.Negate(line.GetQty().Value));
                if (line.GetC_Charge_ID() != 0)	//	explicit overwrite for charge
                {
                    cr.SetAD_Org_ID(line.GetAD_Org_ID());
                }
                if (!IsPosted())
                {
                    //	Cost Detail
                    MCostDetail.CreateInventory(as1, line.GetAD_Org_ID(),
                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                        line.Get_ID(), 0,
                        costs, line.GetQty().Value,
                        line.GetDescription(), GetTrx(), GetRectifyingProcess());
                }
            }
            //
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }
    }
}
