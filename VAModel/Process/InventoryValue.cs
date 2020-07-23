/********************************************************
 * Module  Name   : 
 * Purpose        : Inventory Valuation. Process to fill T_InventoryValue
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Veena        20-Oct-2009
  ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Data;
using System.Data.SqlClient;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    /// <summary>
    /// Inventory Valuation.
    /// Process to fill T_InventoryValue
    /// </summary>
    public class InventoryValue : ProcessEngine.SvrProcess
    {
        /** Price List Used         */
        private int _M_PriceList_Version_ID;
        /** Valuation Date          */
        private DateTime? _DateValue;
        /** Warehouse               */
        private int _M_Warehouse_ID;
        /** Currency                */
        private int _C_Currency_ID;
        /** Optional Cost Element	*/
        private int _M_CostElement_ID;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_PriceList_Version_ID"))
                    _M_PriceList_Version_ID = para[i].GetParameterAsInt();
                else if (name.Equals("DateValue"))
                    _DateValue = (DateTime?)para[i].GetParameter();
                else if (name.Equals("M_Warehouse_ID"))
                    _M_Warehouse_ID = para[i].GetParameterAsInt();
                else if (name.Equals("C_Currency_ID"))
                    _C_Currency_ID = para[i].GetParameterAsInt();
                else if (name.Equals("M_CostElement_ID"))
                    _M_CostElement_ID = para[i].GetParameterAsInt();
            }
            if (_DateValue == null)
                _DateValue = new DateTime(CommonFunctions.CurrentTimeMillis());
        }

        /// <summary>
        /// Perform Process.
        /// - Fill Table with QtyOnHand for Warehouse and Valuation Date
        /// - Perform Price Calculations
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("M_Warehouse_ID=" + _M_Warehouse_ID
                + ",C_Currency_ID=" + _C_Currency_ID
                + ",DateValue=" + _DateValue
                + ",M_PriceList_Version_ID=" + _M_PriceList_Version_ID
                + ",M_CostElement_ID=" + _M_CostElement_ID);

            MWarehouse wh = MWarehouse.Get(GetCtx(), _M_Warehouse_ID);
            MClient c = MClient.Get(GetCtx(), wh.GetAD_Client_ID());
            MAcctSchema mas = c.GetAcctSchema();

            //  Delete (just to be sure)
            StringBuilder sql = new StringBuilder("DELETE FROM T_InventoryValue WHERE AD_PInstance_ID=");
            sql.Append(GetAD_PInstance_ID());
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            MPInstance instance = new MPInstance(GetCtx(), GetAD_PInstance_ID(), null);
            DateTime Createddate = instance.GetCreated();
            Createddate = Createddate.AddHours(-1);

            string qry = "select MAX(AD_PINSTANCE_ID) from AD_PINSTANCE WHERE AD_Process_ID=" + instance.GetAD_Process_ID() + " AND created<  TO_Date('" + Createddate.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM-DD-YYYY HH24:MI:SS')";
            int MaxInstance_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));

           int no1= DB.ExecuteQuery("DELETE FROM T_InventoryValue WHERE AD_PInstance_ID <=" + MaxInstance_ID);

            //	Insert Standard Costs
            sql = new StringBuilder("INSERT INTO T_InventoryValue "
                + "(AD_PInstance_ID, M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,"
                + " AD_Client_ID, AD_Org_ID, CostStandard) "
                + "SELECT ").Append(GetAD_PInstance_ID())
                .Append(", w.M_Warehouse_ID, c.M_Product_ID, c.M_AttributeSetInstance_ID,"
                + " w.AD_Client_ID, w.AD_Org_ID, c.CurrentCostPrice "
                + "FROM M_Warehouse w"
                + " INNER JOIN AD_ClientInfo ci ON (w.AD_Client_ID=ci.AD_Client_ID)"
                + " INNER JOIN C_AcctSchema acs ON (ci.C_AcctSchema1_ID=acs.C_AcctSchema_ID)"
                + " INNER JOIN M_Cost c ON (acs.C_AcctSchema_ID=c.C_AcctSchema_ID AND acs.M_CostType_ID=c.M_CostType_ID AND c.AD_Org_ID IN (0, w.AD_Org_ID))"
                + " INNER JOIN M_CostElement ce ON (c.M_CostElement_ID=ce.M_CostElement_ID AND ce.CostingMethod='S' AND ce.CostElementType='M') "
                + "WHERE w.M_Warehouse_ID=").Append(_M_Warehouse_ID);
            int noInsertStd = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Inserted Std=" + noInsertStd);
            if (noInsertStd == 0)
                return "No Standard Costs found";

            //	Insert addl Costs
            int noInsertCost = 0;
            if (_M_CostElement_ID != 0)
            {
                sql = new StringBuilder("INSERT INTO T_InventoryValue "
                    + "(AD_PInstance_ID, M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,"
                    + " AD_Client_ID, AD_Org_ID, CostStandard, Cost, M_CostElement_ID) "
                    + "SELECT ").Append(GetAD_PInstance_ID())
                    .Append(", w.M_Warehouse_ID, c.M_Product_ID, c.M_AttributeSetInstance_ID,"
                    + " w.AD_Client_ID, w.AD_Org_ID, 0, c.CurrentCostPrice, c.M_CostElement_ID "
                    + "FROM M_Warehouse w"
                    + " INNER JOIN AD_ClientInfo ci ON (w.AD_Client_ID=ci.AD_Client_ID)"
                    + " INNER JOIN C_AcctSchema acs ON (ci.C_AcctSchema1_ID=acs.C_AcctSchema_ID)"
                    + " INNER JOIN M_Cost c ON (acs.C_AcctSchema_ID=c.C_AcctSchema_ID AND acs.M_CostType_ID=c.M_CostType_ID AND c.AD_Org_ID IN (0, w.AD_Org_ID)) "
                    + "WHERE w.M_Warehouse_ID=").Append(_M_Warehouse_ID)
                    .Append(" AND c.M_CostElement_ID=").Append(_M_CostElement_ID)
                    .Append(" AND NOT EXISTS (SELECT * FROM T_InventoryValue iv "
                        + "WHERE iv.AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                        .Append(" AND iv.M_Warehouse_ID=w.M_Warehouse_ID"
                        + " AND iv.M_Product_ID=c.M_Product_ID"
                        + " AND iv.M_AttributeSetInstance_ID=c.M_AttributeSetInstance_ID)");
                noInsertCost = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Inserted Cost=" + noInsertCost);
                //	Update Std Cost Records
                sql = new StringBuilder("UPDATE T_InventoryValue iv "
                    + "SET (Cost, M_CostElement_ID)="
                        + "(SELECT c.CurrentCostPrice, c.M_CostElement_ID "
                        + "FROM M_Warehouse w"
                        + " INNER JOIN AD_ClientInfo ci ON (w.AD_Client_ID=ci.AD_Client_ID)"
                        + " INNER JOIN C_AcctSchema acs ON (ci.C_AcctSchema1_ID=acs.C_AcctSchema_ID)"
                        + " INNER JOIN M_Cost c ON (acs.C_AcctSchema_ID=c.C_AcctSchema_ID"
                            + " AND acs.M_CostType_ID=c.M_CostType_ID AND c.AD_Org_ID IN (0, w.AD_Org_ID)) "
                        + "WHERE c.M_CostElement_ID=" + _M_CostElement_ID
                        + " AND w.M_Warehouse_ID=iv.M_Warehouse_ID"
                        + " AND c.M_Product_ID=iv.M_Product_ID"
                        + " AND c.M_AttributeSetInstance_ID=iv.M_AttributeSetInstance_ID AND rownum=1 AND w.m_warehouse_ID=" + _M_Warehouse_ID + ") "
                    + "WHERE EXISTS (SELECT * FROM T_InventoryValue ivv "
                        + "WHERE ivv.AD_PInstance_ID=" + GetAD_PInstance_ID()
                        + " AND ivv.M_CostElement_ID IS NULL) AND iv.AD_PInstance_ID ="+GetAD_PInstance_ID());
                int noUpdatedCost = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Updated Cost=" + noUpdatedCost);
            }
            if ((noInsertStd + noInsertCost) == 0)
                return "No Costs found";

            //  Update Constants
            //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
            // String myDate = _DateValue.ToString();
            sql = new StringBuilder("UPDATE T_InventoryValue SET ")
                //.Append("DateValue=To_Date('").Append(myDate.Substring(0,10))
                //.Append("23:59:59','MM-DD-YYYY HH24:MI:SS'),")
             .Append("DateValue=").Append(GlobalVariable.TO_DATE(_DateValue, true)).Append(",")
            .Append("M_PriceList_Version_ID=").Append(_M_PriceList_Version_ID).Append(",")
            .Append("C_Currency_ID=").Append(_C_Currency_ID);
            if (_M_CostElement_ID != 0)
            {
                sql.Append(",").Append("M_CostElement_ID=").Append(_M_CostElement_ID);
            }
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Constants=" + no);

            //  Get current QtyOnHand with ASI
            sql = new StringBuilder("UPDATE T_InventoryValue iv SET QtyOnHand = "
                    + "(SELECT SUM(QtyOnHand) FROM M_Storage s"
                    + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) "
                    + "WHERE iv.M_Product_ID=s.M_Product_ID"
                    + " AND iv.M_Warehouse_ID=l.M_Warehouse_ID"
                    + " AND iv.M_AttributeSetInstance_ID=s.M_AttributeSetInstance_ID) "
                + "WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                .Append(" AND iv.M_AttributeSetInstance_ID<>0");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("QtHand with ASI=" + no);
            //  Get current QtyOnHand without ASI
            sql = new StringBuilder("UPDATE T_InventoryValue iv SET QtyOnHand = "
                    + "(SELECT SUM(QtyOnHand) FROM M_Storage s"
                    + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) "
                    + "WHERE iv.M_Product_ID=s.M_Product_ID"
                    + " AND iv.M_Warehouse_ID=l.M_Warehouse_ID) "
                + "WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                .Append(" AND iv.M_AttributeSetInstance_ID=0");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("QtHand w/o ASI=" + no);

            //  Adjust for Valuation Date
            sql = new StringBuilder("UPDATE T_InventoryValue iv "
                + "SET QtyOnHand="
                    + "(SELECT iv.QtyOnHand - NVL(SUM(t.MovementQty), 0) "
                    + "FROM M_Transaction t"
                    + " INNER JOIN M_Locator l ON (t.M_Locator_ID=l.M_Locator_ID) "
                    + "WHERE t.M_Product_ID=iv.M_Product_ID"
                    + " AND t.M_AttributeSetInstance_ID=iv.M_AttributeSetInstance_ID"
                    + " AND t.MovementDate > iv.DateValue"
                    + " AND l.M_Warehouse_ID=iv.M_Warehouse_ID) "
                + "WHERE iv.M_AttributeSetInstance_ID<>0");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Update with ASI=" + no);
            //
            sql = new StringBuilder("UPDATE T_InventoryValue iv "
                + "SET QtyOnHand="
                    + "(SELECT iv.QtyOnHand - NVL(SUM(t.MovementQty), 0) "
                    + "FROM M_Transaction t"
                    + " INNER JOIN M_Locator l ON (t.M_Locator_ID=l.M_Locator_ID) "
                    + "WHERE t.M_Product_ID=iv.M_Product_ID"
                    + " AND t.MovementDate > iv.DateValue"
                    + " AND l.M_Warehouse_ID=iv.M_Warehouse_ID) "
                + "WHERE iv.M_AttributeSetInstance_ID=0");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Update w/o ASI=" + no);

            //  Delete Records w/o OnHand Qty
            sql = new StringBuilder("DELETE FROM T_InventoryValue "
                + "WHERE (QtyOnHand=0 OR QtyOnHand IS NULL) AND AD_PInstance_ID=").Append(GetAD_PInstance_ID());
            int noQty = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("NoQty Deleted=" + noQty);

            //  Update Prices
            no = DataBase.DB.ExecuteQuery("UPDATE T_InventoryValue iv "
                + "SET PricePO = "
                    + "(SELECT MAX(currencyConvert (po.PriceList,po.C_Currency_ID,iv.C_Currency_ID,iv.DateValue,null, po.AD_Client_ID,po.AD_Org_ID))"
                    + " FROM M_Product_PO po WHERE po.M_Product_ID=iv.M_Product_ID"
                    + " AND po.IsCurrentVendor='Y'), "
                + "PriceList = "
                    + "(SELECT currencyConvert(pp.PriceList,pl.C_Currency_ID,iv.C_Currency_ID,iv.DateValue,null, pl.AD_Client_ID,pl.AD_Org_ID)"
                    + " FROM M_PriceList pl, M_PriceList_Version plv, M_ProductPrice pp"
                    + " WHERE pp.M_Product_ID=iv.M_Product_ID AND pp.M_PriceList_Version_ID=iv.M_PriceList_Version_ID"
                    + " AND pp.M_PriceList_Version_ID=plv.M_PriceList_Version_ID"
                    + " AND plv.M_PriceList_ID=pl.M_PriceList_ID), "
                + "PriceStd = "
                    + "(SELECT currencyConvert(pp.PriceStd,pl.C_Currency_ID,iv.C_Currency_ID,iv.DateValue,null, pl.AD_Client_ID,pl.AD_Org_ID)"
                    + " FROM M_PriceList pl, M_PriceList_Version plv, M_ProductPrice pp"
                    + " WHERE pp.M_Product_ID=iv.M_Product_ID AND pp.M_PriceList_Version_ID=iv.M_PriceList_Version_ID"
                    + " AND pp.M_PriceList_Version_ID=plv.M_PriceList_Version_ID"
                    + " AND plv.M_PriceList_ID=pl.M_PriceList_ID), "
                + "PriceLimit = "
                    + "(SELECT currencyConvert(pp.PriceLimit,pl.C_Currency_ID,iv.C_Currency_ID,iv.DateValue,null, pl.AD_Client_ID,pl.AD_Org_ID)"
                    + " FROM M_PriceList pl, M_PriceList_Version plv, M_ProductPrice pp"
                    + " WHERE pp.M_Product_ID=iv.M_Product_ID AND pp.M_PriceList_Version_ID=iv.M_PriceList_Version_ID"
                    + " AND pp.M_PriceList_Version_ID=plv.M_PriceList_Version_ID"
                    + " AND plv.M_PriceList_ID=pl.M_PriceList_ID)"
                    , null, Get_TrxName());
            String msg = "";
            if (no == 0)
                msg = "No Prices";

            //	Convert if different Currency
            if (mas.GetC_Currency_ID() != _C_Currency_ID)
            {
                sql = new StringBuilder("UPDATE T_InventoryValue iv "
                    + "SET CostStandard= "
                        + "(SELECT currencyConvert(iv.CostStandard,acs.C_Currency_ID,iv.C_Currency_ID,iv.DateValue,null, iv.AD_Client_ID,iv.AD_Org_ID) "
                        + "FROM C_AcctSchema acs WHERE acs.C_AcctSchema_ID=" + mas.GetC_AcctSchema_ID() + "),"
                    + "	Cost= "
                        + "(SELECT currencyConvert(iv.Cost,acs.C_Currency_ID,iv.C_Currency_ID,iv.DateValue,null, iv.AD_Client_ID,iv.AD_Org_ID) "
                        + "FROM C_AcctSchema acs WHERE acs.C_AcctSchema_ID=" + mas.GetC_AcctSchema_ID() + ") "
                    + "WHERE AD_PInstance_ID=" + GetAD_PInstance_ID());
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Convered=" + no);
            }

            //  Update Values
            no = DataBase.DB.ExecuteQuery("UPDATE T_InventoryValue SET "
                + "PricePOAmt = QtyOnHand * PricePO, "
                + "PriceListAmt = QtyOnHand * PriceList, "
                + "PriceStdAmt = QtyOnHand * PriceStd, "
                + "PriceLimitAmt = QtyOnHand * PriceLimit, "
                + "CostStandardAmt = QtyOnHand * CostStandard, "
                + "CostAmt = QtyOnHand * Cost "
                + "WHERE AD_PInstance_ID=" + GetAD_PInstance_ID(), null, Get_TrxName());
            log.Fine("Calculation=" + no);
            //
            return msg;
        }
    }
}
