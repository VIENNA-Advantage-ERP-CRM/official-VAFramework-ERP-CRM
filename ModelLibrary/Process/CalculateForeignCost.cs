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
                sql = @"SELECT i.c_invoice_id ,  il.c_invoiceline_id 
                        FROM c_invoice i INNER JOIN c_invoiceline il ON i.c_invoice_id = il.c_invoice_id
                        WHERE il.isactive = 'Y' AND il.isfuturecostcalculated = 'N' AND i.isfuturecostcalculated  = 'N'
                         AND docstatus IN ('CO' , 'CL') AND i.issotrx = 'N' AND i.isreturntrx = 'N' AND NVL(il.m_inoutline_ID , 0) <> 0
                        ORDER BY i.c_invoice_id ASC";
                ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _log.Info("Foreign Cost Calculation : Average Invoice Record : " + ds.Tables[0].Rows.Count);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_invoice_id"]), Get_Trx());
                            invoiceLine = new MInvoiceLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_invoiceline_id"]), Get_Trx());
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
                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM C_InvoiceLine WHERE IsFutureCostCalculated = 'N' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID(), null, Get_Trx())) <= 0)
                                {
                                    int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE C_Invoice Set IsFutureCostCalculated = 'Y' WHERE C_Invoice_ID = " + invoice.GetC_Invoice_ID(), null, Get_Trx()));
                                }
                                Get_Trx().Commit();
                            }
                        }
                        catch (Exception ex1) { }
                    }
                }

                // Calculate Foriegn Cost for Average PO
                sql = @"SELECT i.m_inout_id ,  il.m_inoutline_id , il.c_orderline_id
                        FROM m_inout i INNER JOIN m_inoutline il ON i.m_inout_id = il.m_inout_id
                        WHERE il.isactive = 'Y' AND il.isfuturecostcalculated = 'N' AND i.isfuturecostcalculated  = 'N'
                         AND docstatus IN ('CO' , 'CL') AND i.issotrx = 'N' AND i.isreturntrx = 'N' AND NVL(il.c_orderline_ID , 0) <> 0
                        ORDER BY i.m_inout_id ASC";
                ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _log.Info("Foreign Cost Calculation : Average PO Record : " + ds.Tables[0].Rows.Count);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            orderLine = new MOrderLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_orderline_id"]), Get_Trx());
                            order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), Get_Trx());
                            inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_inoutline_id"]), Get_Trx());
                            if (!MCostForeignCurrency.InsertForeignCostAveragePO(GetCtx(), order, orderLine, inoutLine, Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for calcualting Av. PO Foreign Cost for this record ID = " + inoutLine.GetM_InOut_ID() +
                                           " Error Name is " + pp.GetName() + " And Error Value is " + pp.GetValue());
                                continue;
                            }
                            else
                            {
                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM M_InoutLine WHERE IsFutureCostCalculated = 'N' AND M_InOut_ID = " + inoutLine.GetM_InOut_ID(), null, Get_Trx())) <= 0)
                                {
                                    int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE M_Inout Set IsFutureCostCalculated = 'Y' WHERE M_Inout_ID = " + inoutLine.GetM_InOut_ID(), null, Get_Trx()));
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
