/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RfQCreatePO
 * Purpose        : Create RfQ PO.
 *	                Create purchase order(s) for the resonse(s) and lines marked as 
 *              	Selected Winner using the selected Purchase Quantity (in RfQ Line Quantity)
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     11-Aug.-2009
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
using System.Data.SqlClient;
using VAdvantage.Logging;


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class RfQCreatePO : ProcessEngine.SvrProcess
    {
        //	RfQ 			
        private int _C_RfQ_ID = 0;
        private int _C_DocType_ID = 0;

        /// <summary>
        ///  	Prepare
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
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_RfQ_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process.
        /// Create purchase order(s) for the resonse(s) and lines marked as 
        /// Selected Winner using the selected Purchase Quantity (in RfQ Line Quantity) . 
        /// If a Response is marked as Selected Winner, all lines are created 
        /// (and Selected Winner of other responses ignored).  
        /// If there is no response marked as Selected Winner, the lines are used.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MRfQ rfq = new MRfQ(GetCtx(), _C_RfQ_ID, Get_TrxName());
            if (rfq.Get_ID() == 0)
            {
                throw new ArgumentException("No RfQ found");
            }
            log.Info(rfq.ToString());

            //	Complete 
            MRfQResponse[] responses = rfq.GetResponses(true, true);
            log.Config("#Responses=" + responses.Length);
            if (responses.Length == 0)
            {
                throw new ArgumentException("No completed RfQ Responses found");
            }

            //	Winner for entire RfQ
            for (int i = 0; i < responses.Length; i++)
            {
                MRfQResponse response = responses[i];
                if (!response.IsSelectedWinner())
                {
                    continue;
                }
                //
                MBPartner bp = new MBPartner(GetCtx(), response.GetC_BPartner_ID(), Get_TrxName());
                log.Config("Winner=" + bp);
                MOrder order = new MOrder(GetCtx(), 0, Get_TrxName());
                order.SetIsSOTrx(false);
                if (_C_DocType_ID != 0)
                {
                    order.SetC_DocTypeTarget_ID(_C_DocType_ID);
                }
                else
                {
                    order.SetC_DocTypeTarget_ID();
                }
                order.SetBPartner(bp);
                order.SetC_BPartner_Location_ID(response.GetC_BPartner_Location_ID());
                order.SetSalesRep_ID(rfq.GetSalesRep_ID());
                if (response.GetDateWorkComplete() != null)
                {
                    order.SetDatePromised(response.GetDateWorkComplete());
                }
                else if (rfq.GetDateWorkComplete() != null)
                {
                    order.SetDatePromised(rfq.GetDateWorkComplete());
                }
                order.Save();
                //
                MRfQResponseLine[] lines = response.GetLines(false);
                for (int j = 0; j < lines.Length; j++)
                {
                    //	Respones Line
                    MRfQResponseLine line = lines[j];
                    if (!line.IsActive())
                    {
                        continue;
                    }
                    MRfQResponseLineQty[] qtys = line.GetQtys(false);
                    //	Response Line Qty
                    for (int k = 0; k < qtys.Length; k++)
                    {
                        MRfQResponseLineQty qty = qtys[k];
                        //	Create PO Lline for all Purchase Line Qtys
                        if (qty.GetRfQLineQty().IsActive() && qty.GetRfQLineQty().IsPurchaseQty())
                        {
                            MOrderLine ol = new MOrderLine(order);
                            ol.SetM_Product_ID(line.GetRfQLine().GetM_Product_ID(),
                                qty.GetRfQLineQty().GetC_UOM_ID());
                            ol.SetDescription(line.GetDescription());
                            ol.SetQty(qty.GetRfQLineQty().GetQty());
                            Decimal? price = qty.GetNetAmt();
                            ol.SetPrice(price == null ? Env.ZERO : price.Value);
                            // Work done to set prices on purchase order and attributesetinstance from rfq line. Done by mohit asked by pradeep- 11 January 2019
                            MRfQLine Rfqline = new MRfQLine(GetCtx(), line.GetC_RfQLine_ID(), null);
                            ol.SetM_AttributeSetInstance_ID(Rfqline.GetM_AttributeSetInstance_ID());
                            ol.SetPriceActual(price);
                            ol.SetPriceEntered(price);
                            ol.SetPriceList(price);
                            ol.Save();
                        }
                    }
                }
                response.SetC_Order_ID(order.GetC_Order_ID());
                response.Save();
                return order.GetDocumentNo();
            }


            //	Selected Winner on Line Level
            int noOrders = 0;
            for (int i = 0; i < responses.Length; i++)
            {
                MRfQResponse response = responses[i];
                MBPartner bp = null;
                MOrder order = null;
                //	For all Response Lines
                MRfQResponseLine[] lines = response.GetLines(false);
                for (int j = 0; j < lines.Length; j++)
                {
                    MRfQResponseLine line = lines[j];
                    if (!line.IsActive() || !line.IsSelectedWinner())
                        continue;
                    //	New/different BP
                    if (bp == null)
                    {
                        bp = new MBPartner(GetCtx(), response.GetC_BPartner_ID(), Get_TrxName());
                        order = null;
                    }
                    log.Config("Line=" + line + ", Winner=" + bp);
                    //	New Order
                    if (order == null)
                    {
                        order = new MOrder(GetCtx(), 0, Get_TrxName());
                        order.SetIsSOTrx(false);
                        // Adde by mohit to set selected document type on purchase order.- 11 january 2019
                        if (_C_DocType_ID != 0)
                        {
                            order.SetC_DocTypeTarget_ID(_C_DocType_ID);
                        }
                        else
                        {
                            order.SetC_DocTypeTarget_ID();
                        }
                        order.SetBPartner(bp);
                        order.SetC_BPartner_Location_ID(response.GetC_BPartner_Location_ID());
                        order.SetSalesRep_ID(rfq.GetSalesRep_ID());
                        order.Save();
                        noOrders++;
                        //AddLog(0, null, null, order.GetDocumentNo());
                        AddLog(0, DateTime.Now, null, order.GetDocumentNo());
                    }
                    //	For all Qtys
                    MRfQResponseLineQty[] qtys = line.GetQtys(false);
                    for (int k = 0; k < qtys.Length; k++)
                    {
                        MRfQResponseLineQty qty = qtys[k];
                        if (qty.GetRfQLineQty().IsActive() && qty.GetRfQLineQty().IsPurchaseQty())
                        {
                            MOrderLine ol = new MOrderLine(order);
                            ol.SetM_Product_ID(line.GetRfQLine().GetM_Product_ID(),
                                qty.GetRfQLineQty().GetC_UOM_ID());
                            ol.SetDescription(line.GetDescription());
                            ol.SetQty(qty.GetRfQLineQty().GetQty());
                            Decimal? price = qty.GetNetAmt();
                            ol.SetPriceActual(price);
                            // Work done to set prices on purchase order and attributesetinstance from rfq line. Done by mohit asked by pradeep- 11 January 2019
                            MRfQLine Rfqline = new MRfQLine(GetCtx(), line.GetC_RfQLine_ID(), null);
                            ol.SetM_AttributeSetInstance_ID(Rfqline.GetM_AttributeSetInstance_ID());
                            ol.SetPriceActual(price);
                            ol.SetPriceEntered(price);
                            ol.SetPriceList(price);
                            ol.Save();
                        }
                    }	//	for all Qtys
                }	//	for all Response Lines
                if (order != null)
                {
                    response.SetC_Order_ID(order.GetC_Order_ID());
                    response.Save();
                }
            }

            return "#" + noOrders;
        }
    }
}
