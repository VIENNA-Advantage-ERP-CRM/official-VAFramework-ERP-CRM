/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : CostingCalculation
    * Purpose        : Calculate Cost for Products whose costing already calculated
                       for specified accouting schema
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     25-Oct-2016
******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class CostCalculationWithAcctSchema : SvrProcess
    {
        private StringBuilder sql = new StringBuilder();
        private static VLogger _log = VLogger.GetVLogger(typeof(CostCalculationWithAcctSchema).FullName);

        DateTime? currentDate = DateTime.Now;
        DateTime? minDateRecord;
        String _TrxDate = null;

        DataSet dsInOut = null;
        DataSet dsInvoice = null;
        DataSet dsInventory = null;
        DataSet dsMovement = null;
        DataSet dsChildRecord = null;
        DataRow[] dataRow = null;

        Decimal quantity = 0;

        MInventory inventory = null;
        MInventoryLine inventoryLine = null;

        MMovement movement = null;
        MMovementLine movementLine = null;
        MWarehouse warehouse = null;

        MInOut inout = null;
        MInOutLine inoutLine = null;
        MOrderLine orderLine = null;

        MInvoice invoice = null;
        MInvoiceLine invoiceLine = null;

        MProduct product = null;

        MMatchPO match = null;

        string acctRecord = null;
        int[] acctSchemaRecord;

        string conversionNotFoundInvoice = "";
        string conversionNotFoundInOut = "";
        string conversionNotFoundInventory = "";
        string conversionNotFoundMovement = "";
        string conversionNotFoundInvoice1 = "";
        string conversionNotFoundInOut1 = "";
        string conversionNotFoundInventory1 = "";
        string conversionNotFoundMovement1 = "";
        string conversionNotFound = "";

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
                else if (name.Equals("C_AcctSchema_ID"))
                {
                    acctRecord = (string)para[i].GetParameter();
                    acctSchemaRecord = Array.ConvertAll(acctRecord.Split(','), int.Parse);
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            try
            {
                _log.Info("Cost Calculation Start on " + DateTime.Now);

                // min date record from the transaction window
                minDateRecord = SerachMinDate();

                int diff = (DateTime.Now - minDateRecord.Value).Days;

                for (int days = 0; days <= diff; days++)
                {
                    if (days != 0)
                    {
                        minDateRecord = minDateRecord.Value.AddDays(1);
                    }

                    _log.Info("Cost Calculation Start for " + minDateRecord); 

                    sql.Clear();
                    sql.Append("SELECT * FROM M_InOut WHERE dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + " AND  isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%')) ORDER BY dateacct");
                    dsInOut = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());

                    sql.Clear();
                    sql.Append("SELECT * FROM C_Invoice WHERE dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + " AND isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%')) ORDER BY dateacct");
                    dsInvoice = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());

                    sql.Clear();
                    sql.Append("SELECT * FROM m_Inventory WHERE movementdate = " + GlobalVariable.TO_DATE(minDateRecord, true) + " AND isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%')) ORDER BY movementdate");
                    dsInventory = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());

                    sql.Clear();
                    sql.Append("SELECT * FROM M_Movement WHERE movementdate = " + GlobalVariable.TO_DATE(minDateRecord, true) + " AND isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%')) ORDER BY movementdate");
                    dsMovement = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());

                    // Complete Record

                    #region Cost Calculation For Material Receipt
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'N' AND IsReturnTrx = 'N' AND DocStatus IN ('CO' , 'CL') AND  DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'Y' " +
                                                " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region Material Receipt
                                                if (!inout.IsSOTrx() && !inout.IsReturnTrx())
                                                {
                                                    if (orderLine == null || orderLine.GetC_OrderLine_ID() == 0) //MR Without PO
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                       "Material Receipt", null, inoutLine, null, null, 0, inoutLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //MR With PO
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                           "Material Receipt", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()), inoutLine.GetMovementQty()), inoutLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for SO / PO / CRMA / VRMA
                    try
                    {
                        dataRow = dsInvoice.Tables[0].Select(" DocStatus IN ('CO' , 'CL') AND DateAcct = '" + minDateRecord + "'", "C_Invoice_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(dataRow[i]["C_Invoice_ID"]), Get_Trx());

                                sql.Clear();
                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'Y' " +
                                                 " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            invoiceLine = new MInvoiceLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["C_InvoiceLine_ID"]), Get_Trx());
                                            if (invoiceLine != null && invoiceLine.GetC_Invoice_ID() > 0 && invoiceLine.GetQtyInvoiced() == 0)
                                                continue;
                                            if (invoiceLine.GetC_OrderLine_ID() > 0)
                                            {
                                                if (invoiceLine.GetC_Charge_ID() > 0)
                                                {
                                                    #region Landed Cost Allocation
                                                    if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0, "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region for Expense type product
                                                    if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                             "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region  for Item Type product
                                                    else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                    {
                                                        MOrder order1 = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());
                                                        if (order1.GetC_Order_ID() == 0)
                                                        {
                                                            MOrderLine ol1 = new MOrderLine(GetCtx(), invoiceLine.GetC_OrderLine_ID(), Get_Trx());
                                                            order1 = new MOrder(GetCtx(), ol1.GetC_Order_ID(), Get_Trx());
                                                        }

                                                        #region  Sales Order
                                                        if (order1.IsSOTrx() && !order1.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Customer)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()),
                                                                  Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                        #endregion

                                                        #region Purchase Order
                                                        else if (!order1.IsSOTrx() && !order1.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(),
                                                                  invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                        #endregion

                                                        #region CRMA
                                                        else if (order1.IsSOTrx() && order1.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Customer)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(),
                                                              invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                        #endregion

                                                        #region VRMA
                                                        else if (!order1.IsSOTrx() && order1.IsReturnTrx())
                                                        {
                                                            //change 12-5-2016
                                                            // when Ap Credit memo is alone then we will do a impact on costing.
                                                            // this is bcz of giving discount for particular product
                                                            MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                            if (docType.GetDocBaseType() == "APC" && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()),
                                                                  Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                #region for Landed Cost Allocation
                                                if (invoiceLine.GetC_Charge_ID() > 0)
                                                {
                                                    if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0,
                                                            "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_TrxName(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region for Expense type product
                                                if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                {
                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                        "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_TrxName(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                    {
                                                        if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                        {
                                                            conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                        }
                                                        _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                    }
                                                }
                                                #endregion

                                                #region  for Item Type product
                                                else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                {
                                                    #region Sales Order
                                                    if (invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Customer)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()),
                                                              Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region Purchase Order
                                                    else if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Vendor)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(),
                                                              invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region CRMA
                                                    else if (invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                          "Invoice(Customer)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(),
                                                          invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region VRMA
                                                    else if (!invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                    {
                                                        // when Ap Credit memo is alone then we will do a impact on costing.
                                                        // this is bcz of giving discount for particular product
                                                        MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                        if (docType.GetDocBaseType() == "APC" && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Vendor)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()),
                                                              Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for Physical Inventory
                    try
                    {
                        dataRow = dsInventory.Tables[0].Select(" DocStatus IN ('CO' , 'CL') AND IsInternalUse = 'N' AND MovementDate = '" + minDateRecord + "'", "M_Inventory_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Inventory_ID"]), Get_Trx());
                                sql.Clear();
                                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                        inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InventoryLine_ID"]), Get_Trx());
                                        if (product.GetProductType() == "I") // for Item Type product
                                        {
                                            quantity = 0;
                                            if (inventory.IsInternalUse())
                                            {
                                                #region for Internal use inventory
                                                quantity = Decimal.Negate(inventoryLine.GetQtyInternalUse());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Internal Use Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region for Physical Inventory
                                                quantity = Decimal.Subtract(inventoryLine.GetQtyCount(), inventoryLine.GetQtyBook());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Physical Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Physical Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for  Internal use inventory
                    try
                    {
                        dataRow = dsInventory.Tables[0].Select(" DocStatus IN ('CO' , 'CL') AND IsInternalUse = 'Y'  AND MovementDate = '" + minDateRecord + "'", "M_Inventory_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Inventory_ID"]), Get_Trx());
                                sql.Clear();
                                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                        inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InventoryLine_ID"]), Get_Trx());
                                        if (product.GetProductType() == "I") // for Item Type product
                                        {
                                            quantity = 0;
                                            if (inventory.IsInternalUse())
                                            {
                                                #region for Internal use inventory
                                                quantity = Decimal.Negate(inventoryLine.GetQtyInternalUse());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Internal Use Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region for Physical Inventory
                                                quantity = Decimal.Subtract(inventoryLine.GetQtyCount(), inventoryLine.GetQtyBook());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Physical Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Physical Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for Inventory Move
                    try
                    {
                        dataRow = dsMovement.Tables[0].Select(" DocStatus IN ('CO' , 'CL') AND MovementDate = '" + minDateRecord + "'", "M_Movement_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                movement = new MMovement(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Movement_ID"]), Get_Trx());
                                warehouse = new MWarehouse(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Warehouse_ID"]), Get_Trx());

                                sql.Clear();
                                if (movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_MovementLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Movement_ID = " + movement.GetM_Movement_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_MovementLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Movement_ID = " + movement.GetM_Movement_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                        movementLine = new MMovementLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_MovementLine_ID"]), Get_Trx());
                                        if (product.GetProductType() == "I" && movement.GetAD_Org_ID() != warehouse.GetAD_Org_ID()) // for Item Type product
                                        {
                                            #region for inventory move
                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), movement.GetAD_Client_ID(), movement.GetAD_Org_ID(), product, movementLine.GetM_AttributeSetInstance_ID(),
                                                "Inventory Move", null, null, movementLine, null, 0, movementLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundMovement))
                                            {
                                                if (!conversionNotFoundMovement1.Contains(conversionNotFoundMovement))
                                                {
                                                    conversionNotFoundMovement1 += conversionNotFoundMovement + " , ";
                                                }
                                                _log.Info("Cost not Calculated for Inventory Move for this Line ID = " + movementLine.GetM_MovementLine_ID());
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation For  Return to Vendor
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'N' AND IsReturnTrx = 'Y' AND DocStatus IN ('CO' , 'CL') AND DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());

                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);

                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region  Return To Vendor
                                                if (!inout.IsSOTrx() && inout.IsReturnTrx())
                                                {
                                                    if (inout.GetOrig_Order_ID() == 0 || orderLine == null || orderLine.GetC_OrderLine_ID() == 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                       "Return To Vendor", null, inoutLine, null, null, 0, Decimal.Negate(inoutLine.GetMovementQty()),
                                                       Get_TrxName(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                            "Return To Vendor", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()),
                                                            Decimal.Negate(inoutLine.GetMovementQty())), Decimal.Negate(inoutLine.GetMovementQty()), Get_TrxName(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation For shipment
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'Y' AND IsReturnTrx = 'N' AND DocStatus IN ('CO' , 'CL') AND DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());

                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);

                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region shipment
                                                if (inout.IsSOTrx() && !inout.IsReturnTrx())
                                                {
                                                    if (inout.GetC_Order_ID() <= 0)
                                                    {
                                                        break;
                                                    }
                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                         "Shipment", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()),
                                                         Decimal.Negate(inoutLine.GetMovementQty())), Decimal.Negate(inoutLine.GetMovementQty()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                    {
                                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                        {
                                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                        }
                                                        _log.Info("Cost not Calculated for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation For Customer Return
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'Y' AND IsReturnTrx = 'Y' AND DocStatus IN ('CO' , 'CL') AND DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region Customer Return
                                                if (inout.IsSOTrx() && inout.IsReturnTrx())
                                                {
                                                    if (inout.GetOrig_Order_ID() <= 0)
                                                    {
                                                        break;
                                                    }
                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                          "Customer Return", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()),
                                                          inoutLine.GetMovementQty()), inoutLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                    {
                                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                        {
                                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                        }
                                                        _log.Info("Cost not Calculated for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    //Reverse Record

                    #region Cost Calculation For Customer Return
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'Y' AND IsReturnTrx = 'Y' AND DocStatus = 'RE' AND DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region Customer Return
                                                if (inout.IsSOTrx() && inout.IsReturnTrx())
                                                {
                                                    if (inout.GetOrig_Order_ID() <= 0)
                                                    {
                                                        break;
                                                    }
                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                          "Customer Return", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()), inoutLine.GetMovementQty()), inoutLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                    {
                                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                        {
                                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                        }
                                                        _log.Info("Cost not Calculated for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation For shipment
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'Y' AND IsReturnTrx = 'N' AND DocStatus = 'RE' AND DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region shipment
                                                if (inout.IsSOTrx() && !inout.IsReturnTrx())
                                                {
                                                    if (inout.GetC_Order_ID() <= 0)
                                                    {
                                                        break;
                                                    }
                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                         "Shipment", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()), Decimal.Negate(inoutLine.GetMovementQty())), Decimal.Negate(inoutLine.GetMovementQty()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                    {
                                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                        {
                                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                        }
                                                        _log.Info("Cost not Calculated for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation For  Return to Vendor
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'N' AND IsReturnTrx = 'Y' AND DocStatus = 'RE' AND DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region  Return To Vendor
                                                if (!inout.IsSOTrx() && inout.IsReturnTrx())
                                                {
                                                    if (inout.GetOrig_Order_ID() == 0 || orderLine == null || orderLine.GetC_OrderLine_ID() == 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                       "Return To Vendor", null, inoutLine, null, null, 0, Decimal.Negate(inoutLine.GetMovementQty()), Get_TrxName(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                            "Return To Vendor", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()), Decimal.Negate(inoutLine.GetMovementQty())), Decimal.Negate(inoutLine.GetMovementQty()), Get_TrxName(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for Inventory Move
                    try
                    {
                        dataRow = dsMovement.Tables[0].Select(" DocStatus = 'RE' AND MovementDate = '" + minDateRecord + "'", "M_Movement_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                movement = new MMovement(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Movement_ID"]), Get_Trx());
                                warehouse = new MWarehouse(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Warehouse_ID"]), Get_Trx());

                                sql.Clear();
                                if (movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_MovementLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Movement_ID = " + movement.GetM_Movement_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_MovementLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Movement_ID = " + movement.GetM_Movement_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                        movementLine = new MMovementLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_MovementLine_ID"]), Get_Trx());
                                        if (product.GetProductType() == "I" && movement.GetAD_Org_ID() != warehouse.GetAD_Org_ID()) // for Item Type product
                                        {
                                            #region for inventory move
                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), movement.GetAD_Client_ID(), movement.GetAD_Org_ID(), product, movementLine.GetM_AttributeSetInstance_ID(),
                                                "Inventory Move", null, null, movementLine, null, 0, movementLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundMovement))
                                            {
                                                if (!conversionNotFoundMovement1.Contains(conversionNotFoundMovement))
                                                {
                                                    conversionNotFoundMovement1 += conversionNotFoundMovement + " , ";
                                                }
                                                _log.Info("Cost not Calculated for Inventory Move for this Line ID = " + movementLine.GetM_MovementLine_ID());
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for  Internal use inventory
                    try
                    {
                        dataRow = dsInventory.Tables[0].Select(" DocStatus = 'RE' AND IsInternalUse = 'Y' AND MovementDate = '" + minDateRecord + "'", "M_Inventory_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Inventory_ID"]), Get_Trx());
                                sql.Clear();
                                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                        inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InventoryLine_ID"]), Get_Trx());
                                        if (product.GetProductType() == "I") // for Item Type product
                                        {
                                            quantity = 0;
                                            if (inventory.IsInternalUse())
                                            {
                                                #region for Internal use inventory
                                                quantity = Decimal.Negate(inventoryLine.GetQtyInternalUse());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Internal Use Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region for Physical Inventory
                                                quantity = Decimal.Subtract(inventoryLine.GetQtyCount(), inventoryLine.GetQtyBook());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Physical Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Physical Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for Physical Inventory
                    try
                    {
                        dataRow = dsInventory.Tables[0].Select(" DocStatus = 'RE' AND IsInternalUse = 'N' AND MovementDate = '" + minDateRecord + "'", "M_Inventory_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_Inventory_ID"]), Get_Trx());
                                sql.Clear();
                                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'Y' " +
                                              " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                        inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InventoryLine_ID"]), Get_Trx());
                                        if (product.GetProductType() == "I") // for Item Type product
                                        {
                                            quantity = 0;
                                            if (inventory.IsInternalUse())
                                            {
                                                #region for Internal use inventory
                                                quantity = Decimal.Negate(inventoryLine.GetQtyInternalUse());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Internal Use Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region for Physical Inventory
                                                quantity = Decimal.Subtract(inventoryLine.GetQtyCount(), inventoryLine.GetQtyBook());
                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                                               "Physical Inventory", inventoryLine, null, null, null, 0, quantity, Get_Trx(), acctSchemaRecord, out conversionNotFoundInventory))
                                                {
                                                    if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                                    {
                                                        conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                                    }
                                                    _log.Info("Cost not Calculated for Physical Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation for SO / PO / CRMA / VRMA
                    try
                    {
                        dataRow = dsInvoice.Tables[0].Select(" DocStatus = 'RE' AND DateAcct = '" + minDateRecord + "'", "C_Invoice_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(dataRow[i]["C_Invoice_ID"]), Get_Trx());

                                sql.Clear();
                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'Y' " +
                                                 " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            invoiceLine = new MInvoiceLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["C_InvoiceLine_ID"]), Get_Trx());
                                            if (invoiceLine != null && invoiceLine.GetC_Invoice_ID() > 0 && invoiceLine.GetQtyInvoiced() == 0)
                                                continue;
                                            if (invoiceLine.GetC_OrderLine_ID() > 0)
                                            {
                                                if (invoiceLine.GetC_Charge_ID() > 0)
                                                {
                                                    #region Landed Cost Allocation
                                                    if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0, "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region for Expense type product
                                                    if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                             "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region  for Item Type product
                                                    else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                    {
                                                        MOrder order1 = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());
                                                        if (order1.GetC_Order_ID() == 0)
                                                        {
                                                            MOrderLine ol1 = new MOrderLine(GetCtx(), invoiceLine.GetC_OrderLine_ID(), Get_Trx());
                                                            order1 = new MOrder(GetCtx(), ol1.GetC_Order_ID(), Get_Trx());
                                                        }

                                                        #region  Sales Order
                                                        if (order1.IsSOTrx() && !order1.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Customer)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()), Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                        #endregion

                                                        #region Purchase Order
                                                        else if (!order1.IsSOTrx() && !order1.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(), invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                        #endregion

                                                        #region CRMA
                                                        else if (order1.IsSOTrx() && order1.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Customer)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(), invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                        #endregion

                                                        #region VRMA
                                                        else if (!order1.IsSOTrx() && order1.IsReturnTrx())
                                                        {
                                                            //change 12-5-2016
                                                            // when Ap Credit memo is alone then we will do a impact on costing.
                                                            // this is bcz of giving discount for particular product
                                                            MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                            if (docType.GetDocBaseType() == "APC" && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()), Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                #region for Landed Cost Allocation
                                                if (invoiceLine.GetC_Charge_ID() > 0)
                                                {
                                                    if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0,
                                                            "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_TrxName(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region for Expense type product
                                                if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                {
                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                        "Invoice(Vendor)", null, null, null, invoiceLine, 0, 0, Get_TrxName(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                    {
                                                        if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                        {
                                                            conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                        }
                                                        _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                    }
                                                }
                                                #endregion

                                                #region  for Item Type product
                                                else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                {
                                                    #region Sales Order
                                                    if (invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Customer)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()), Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region Purchase Order
                                                    else if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Vendor)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(), invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region CRMA
                                                    else if (invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                          "Invoice(Customer)", null, null, null, invoiceLine, invoiceLine.GetLineNetAmt(), invoiceLine.GetQtyInvoiced(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                    }
                                                    #endregion

                                                    #region VRMA
                                                    else if (!invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                    {
                                                        // when Ap Credit memo is alone then we will do a impact on costing.
                                                        // this is bcz of giving discount for particular product
                                                        MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                        if (docType.GetDocBaseType() == "APC" && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Vendor)", null, null, null, invoiceLine, Decimal.Negate(invoiceLine.GetLineNetAmt()), Decimal.Negate(invoiceLine.GetQtyInvoiced()), Get_Trx(), acctSchemaRecord, out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region Cost Calculation For Material Receipt
                    try
                    {
                        dataRow = dsInOut.Tables[0].Select("IsSoTrx = 'N' AND IsReturnTrx = 'N' AND DocStatus = 'RE' AND  DateAcct = '" + minDateRecord + "'", "M_InOut_ID");
                        if (dataRow != null && dataRow.Length > 0)
                        {
                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dataRow[i]["M_inout_ID"]), Get_Trx());

                                sql.Clear();
                                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'Y' " +
                                                " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                else
                                {
                                    sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' " +
                                                 " AND M_Inout_ID = " + inout.GetM_InOut_ID() + " ORDER BY Line ");
                                }
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        try
                                        {
                                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                                            orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                                continue;
                                            product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                            if (product.GetProductType() == "I") // for Item Type product
                                            {
                                                #region Material Receipt
                                                if (!inout.IsSOTrx() && !inout.IsReturnTrx())
                                                {
                                                    if (orderLine == null || orderLine.GetC_OrderLine_ID() == 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                       "Material Receipt", null, inoutLine, null, null, 0, inoutLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                           "Material Receipt", null, inoutLine, null, null, Decimal.Multiply(Decimal.Divide(orderLine.GetLineNetAmt(), orderLine.GetQtyOrdered()), inoutLine.GetMovementQty()), inoutLine.GetMovementQty(), Get_Trx(), acctSchemaRecord, out conversionNotFoundInOut))
                                                        {
                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                            {
                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _log.Info("Error Occured during costing " + ex.ToString());
                if (dsInOut != null)
                    dsInOut.Dispose();
                if (dsInventory != null)
                    dsInventory.Dispose();
                if (dsInvoice != null)
                    dsInvoice.Dispose();
                if (dsMovement != null)
                    dsMovement.Dispose();
                if (dsChildRecord != null)
                    dsChildRecord.Dispose();
            }
            finally
            {
                if (!string.IsNullOrEmpty(conversionNotFoundInOut1))
                {
                    conversionNotFoundInOut = "Conversion Rate Not Found for M_Inout : " + conversionNotFoundInOut1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundInvoice1))
                {
                    conversionNotFoundInvoice = "Conversion Rate Not Found for C_Invoice : " + conversionNotFoundInvoice1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundInventory1))
                {
                    conversionNotFoundInventory = "Conversion Rate Not Found for Phy. Inventory / Internal Use Inventory : " + conversionNotFoundInventory1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundMovement1))
                {
                    conversionNotFoundMovement = "Conversion Rate Not Found for Movement : " + conversionNotFoundMovement1;
                }

                conversionNotFound = conversionNotFoundInOut + "\n" + conversionNotFoundInvoice + "\n" + conversionNotFoundInventory + "\n" + conversionNotFoundMovement;
               
                if (dsInOut != null)
                    dsInOut.Dispose();
                if (dsInventory != null)
                    dsInventory.Dispose();
                if (dsInvoice != null)
                    dsInvoice.Dispose();
                if (dsMovement != null)
                    dsMovement.Dispose();
                if (dsChildRecord != null)
                    dsChildRecord.Dispose();
                _log.Info("Successfully Ended Cost Calculation ");
            }
            return conversionNotFound;
        }

        public DateTime? SerachMinDate()
        {
            DateTime? minDate;
            DateTime? tempDate;
            try
            {
                sql.Clear();
                sql.Append("SELECT Min(To_char(MovementDate, 'DD-MON-YY')) FROM m_Inventory WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%'))");
                minDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                sql.Clear();
                sql.Append("SELECT Min(To_char(MovementDate, 'DD-MON-YY')) FROM m_movement WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%'))");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                sql.Clear();
                sql.Append("SELECT Min(To_char(DateAcct, 'DD-MON-YY')) FROM C_Invoice WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%'))");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                sql.Clear();
                sql.Append("SELECT Min(To_char(DateAcct, 'DD-MON-YY')) FROM m_inout WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'Y') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'Y' AND description like '%{->%'))");
                tempDate = Util.GetValueOfDateTime
                    (DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return minDate;
        }
    }
}
