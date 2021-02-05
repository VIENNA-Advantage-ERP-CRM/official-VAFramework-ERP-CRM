/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : Calculate Foreign Cost
 * Chronological Development
 * Amit Bansal     12-Dec-2016
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;

namespace VAdvantage.Process
{
    public class CalculateForeignCost : SvrProcess
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(CalculateForeignCost).FullName);

        private string sql = string.Empty;
        private DataSet ds = null;
        MInvoice invoice = null;
        MInvoiceLine invoiceLine = null;
        MOrder order = null;
        MOrderLine orderLine = null;
        MInOutLine inoutLine = null;

        protected override void Prepare()
        {
            ;
        }

        protected override string DoIt()
        {
            try
            {
                _log.Info("Foreign Cost Calculation start at : " + DateTime.Now);

                // Calculate Foreign Cost for Average Invoice
                sql = @"SELECT i.VAB_Invoice_id ,  il.VAB_InvoiceLine_id 
                        FROM VAB_Invoice i INNER JOIN VAB_InvoiceLine il ON i.VAB_Invoice_id = il.VAB_Invoice_id
                        WHERE il.isactive = 'Y' AND il.isfuturecostcalculated = 'N' AND i.isfuturecostcalculated  = 'N'
                         AND docstatus IN ('CO' , 'CL') AND i.issotrx = 'N' AND i.isreturntrx = 'N' AND NVL(il.VAM_Inv_InOutLine_ID , 0) <> 0
                        ORDER BY i.VAB_Invoice_id ASC";
                ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _log.Info("Foreign Cost Calculation : Average Invoice Record : " + ds.Tables[0].Rows.Count);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Invoice_id"]), Get_Trx());
                            invoiceLine = new MInvoiceLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_InvoiceLine_id"]), Get_Trx());
                            if (!MCostForeignCurrency.InsertForeignCostAverageInvoice(GetCtx(), invoice, invoiceLine, Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for calcualting Av. Invoice Foreign Cost for this record ID = " + invoice.GetDocumentNo() +
                                           " Error Name is " + pp.GetName() + " And Error Value is " + pp.GetValue());
                                continue;
                            }
                            else
                            {
                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAB_InvoiceLine WHERE IsFutureCostCalculated = 'N' AND VAB_Invoice_ID = " + invoice.GetVAB_Invoice_ID(), null, Get_Trx())) <= 0)
                                {
                                    int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE VAB_Invoice Set IsFutureCostCalculated = 'Y' WHERE VAB_Invoice_ID = " + invoice.GetVAB_Invoice_ID(), null, Get_Trx()));
                                }
                                Get_Trx().Commit();
                            }
                        }
                        catch (Exception ex1) { }
                    }
                }

                // Calculate Foriegn Cost for Average PO
                sql = @"SELECT i.VAM_Inv_InOut_id ,  il.VAM_Inv_InOutLine_id , il.VAB_Orderline_id
                        FROM VAM_Inv_InOut i INNER JOIN VAM_Inv_InOutLine il ON i.VAM_Inv_InOut_id = il.VAM_Inv_InOut_id
                        WHERE il.isactive = 'Y' AND il.isfuturecostcalculated = 'N' AND i.isfuturecostcalculated  = 'N'
                         AND docstatus IN ('CO' , 'CL') AND i.issotrx = 'N' AND i.isreturntrx = 'N' AND NVL(il.VAB_Orderline_ID , 0) <> 0
                        ORDER BY i.VAM_Inv_InOut_id ASC";
                ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _log.Info("Foreign Cost Calculation : Average PO Record : " + ds.Tables[0].Rows.Count);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            orderLine = new MOrderLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Orderline_id"]), Get_Trx());
                            order = new MOrder(GetCtx(), orderLine.GetVAB_Order_ID(), Get_Trx());
                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_InOutLine_id"]), Get_Trx());
                            if (!MCostForeignCurrency.InsertForeignCostAveragePO(GetCtx(), order, orderLine, inoutLine, Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for calcualting Av. PO Foreign Cost for this record ID = " + inoutLine.GetVAM_Inv_InOut_ID() +
                                           " Error Name is " + pp.GetName() + " And Error Value is " + pp.GetValue());
                                continue;
                            }
                            else
                            {
                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_InOutLine WHERE IsFutureCostCalculated = 'N' AND VAM_Inv_InOut_ID = " + inoutLine.GetVAM_Inv_InOut_ID(), null, Get_Trx())) <= 0)
                                {
                                    int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE VAM_Inv_InOut Set IsFutureCostCalculated = 'Y' WHERE VAM_Inv_InOut_ID = " + inoutLine.GetVAM_Inv_InOut_ID(), null, Get_Trx()));
                                }
                                Get_Trx().Commit();
                            }
                        }
                        catch (Exception ex2) { }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            _log.Info("Foreign Cost Calculation End : " + DateTime.Now);
            return Msg.GetMsg(GetCtx(), "SuccessFullyCompleted");
        }
    }
}
