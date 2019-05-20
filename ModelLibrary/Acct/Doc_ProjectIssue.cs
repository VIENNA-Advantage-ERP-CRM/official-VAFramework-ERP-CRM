/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_ProjectIssue
 * Purpose        : Project Issue.
 *	                Note:
 *		            Will load the default GL Category. 
 *		            Set up a document type to set the GL Category. 
 * Class Used     : Doc
 * * Chronological    Development
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
    public class Doc_ProjectIssue : Doc
    {
        //	Pseudo Line							
        private DocLine _line = null;
        // Issue									
        private MProjectIssue _issue = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_ProjectIssue(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MProjectIssue), idr, MDocBaseType.DOCBASETYPE_PROJECTISSUE, trxName)
        {

        }
        public Doc_ProjectIssue(MAcctSchema[] ass,DataRow dr, Trx trxName)
            : base(ass, typeof(MProjectIssue), dr, MDocBaseType.DOCBASETYPE_PROJECTISSUE, trxName)
        {

        }

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            SetC_Currency_ID(NO_CURRENCY);
            _issue = (MProjectIssue)GetPO();
            SetDateDoc(_issue.GetMovementDate());
            SetDateAcct(_issue.GetMovementDate());

            //	Pseudo Line
            _line = new DocLine(_issue, this);
            _line.SetQty(_issue.GetMovementQty(), true);    //  sets Trx and Storage Qty

            //	Pseudo Line Check
            if (_line.GetM_Product_ID() == 0)
            {
                log.Warning(_line.ToString() + " - No Product");
            }
            log.Fine(_line.ToString());
            return null;
        }

        /// <summary>
        /// Get DocumentNo
        /// </summary>
        /// <returns>document no</returns>
        public new String GetDocumentNo()
        {
            MProject p = _issue.GetParent();
            if (p != null)
            {
                return p.GetValue() + " #" + _issue.GetLine();
            }
            return "(" + _issue.Get_ID() + ")";
        }

        /// <summary>
        ///  Get Balance
        /// </summary>
        /// <returns>Zero (always balanced)</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  PJI
        ///  <pre>
        ///  Issue
        ///      ProjectWIP      DR
        ///      Inventory               CR
        ///  </pre>
        ///  Project Account is either Asset or WIP depending on Project Type
        /// </summary>
        /// <param name="?"></param>
        /// <returns>fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            SetC_Currency_ID(as1.GetC_Currency_ID());

            MProject project = new MProject(GetCtx(), _issue.GetC_Project_ID(), null);
            String ProjectCategory = project.GetProjectCategory();
            MProduct product = MProduct.Get(GetCtx(), _issue.GetM_Product_ID());

            //  Line pointers
            FactLine dr = null;
            FactLine cr = null;

            //  Issue Cost
            Decimal? cost = null;
            if (_issue.GetM_InOutLine_ID() != 0)
            {
                cost = GetPOCost(as1);
            }
            else if (_issue.GetS_TimeExpenseLine_ID() != 0)
            {
                cost = GetLaborCost(as1);
            }
            if (cost == null)	//	standard Product Costs
                cost = _line.GetProductCosts(as1, GetAD_Org_ID(), false);

            //  Project         DR
            int acctType = ACCTTYPE_ProjectWIP;
            if (MProject.PROJECTCATEGORY_AssetProject.Equals(ProjectCategory))
            {
                acctType = ACCTTYPE_ProjectAsset;
            }
            dr = fact.CreateLine(_line,
                GetAccount(acctType, as1), as1.GetC_Currency_ID(), cost, null);
            dr.SetQty((Decimal?)Decimal.Negate(Utility.Util.GetValueOfDecimal(_line.GetQty())));

            //  Inventory               CR
            acctType = ProductCost.ACCTTYPE_P_Asset;
            if (product.IsService())
            {
                acctType = ProductCost.ACCTTYPE_P_Expense;
            }
            cr = fact.CreateLine(_line,
                _line.GetAccount(acctType, as1),
                as1.GetC_Currency_ID(), null, cost);
            cr.SetM_Locator_ID(_line.GetM_Locator_ID());
            cr.SetLocationFromLocator(_line.GetM_Locator_ID(), true);	// from Loc
            //
            List<Fact> facts = new List<Fact>();
            facts.Add(fact);
            return facts;
        }

        /// <summary>
        /// Get PO Costs in Currency of AcctSchema
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>Unit PO Cost</returns>
        private Decimal? GetPOCost(MAcctSchema as1)
        {
            Decimal? retValue = null;
            //	Uses PO Date
            String sql = "SELECT currencyConvert(ol.PriceActual, o.C_Currency_ID, @param1, o.DateOrdered, o.C_ConversionType_ID, @param2, @param3) "
                + "FROM C_OrderLine ol"
                + " INNER JOIN M_InOutLine iol ON (iol.C_OrderLine_ID=ol.C_OrderLine_ID)"
                + " INNER JOIN C_Order o ON (o.C_Order_ID=ol.C_Order_ID) "
                + "WHERE iol.M_InOutLine_ID=@param4";
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@param1", as1.GetC_Currency_ID());
                param[1] = new SqlParameter("@param2", GetAD_Client_ID());
                param[2] = new SqlParameter("@param3", GetAD_Org_ID());
                param[3] = new SqlParameter("@param4", _issue.GetM_InOutLine_ID());

                idr = DataBase.DB.ExecuteReader(sql, param, null);

                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);///.getBigDecimal(1);
                    log.Fine("POCost = " + retValue);
                }
                else
                {
                    log.Warning("Not found for M_InOutLine_ID=" + _issue.GetM_InOutLine_ID());
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

            return retValue;
        }

        /// <summary>
        /// Get Labor Cost from Expense Report
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>Unit Labor Cost</returns>
        private Decimal? GetLaborCost(MAcctSchema as1)
        {
            Decimal? retValue = null;
            /** TODO Labor Cost	*/
            return retValue;
        }
    }
}
