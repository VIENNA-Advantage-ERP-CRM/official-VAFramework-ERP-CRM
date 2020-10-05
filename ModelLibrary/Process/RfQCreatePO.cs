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
        //Variable Declaration
        private int VA009_PaymentMethod_ID = 0;
        private string PaymentBaseType = "";
        DataSet result = new DataSet();
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
                //Added by Neha Thakur--To set Payment Method,Payment Rule and Payment Method(Button)
                if (bp.GetVA009_PO_PaymentMethod_ID() == 0)
                {
                    result = GetPaymentMethod(rfq.GetAD_Org_ID());
                    if (result != null && result.Tables[0].Rows.Count > 0)
                    {
                        order.SetVA009_PaymentMethod_ID(Util.GetValueOfInt(result.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]));
                        order.SetPaymentMethod(Util.GetValueOfString(result.Tables[0].Rows[0]["VA009_PaymentBaseType"]));
                        order.SetPaymentRule(Util.GetValueOfString(result.Tables[0].Rows[0]["VA009_PaymentBaseType"]));
                    }
                }
                else
                {
                    order.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                    order.SetPaymentMethod(GetPaymentBaseType(bp.GetVA009_PO_PaymentMethod_ID()));
                    order.SetPaymentRule(PaymentBaseType);
                }
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
            StringBuilder Orderno = new StringBuilder();
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
                        order.SetAD_Org_ID(rfq.GetAD_Org_ID());
                        //Added by Neha Thakur--To set Payment Method,Payment Rule and Payment Method(Button)
                        if (bp.GetVA009_PO_PaymentMethod_ID() == 0)
                        {
                            result = null;
                            result = GetPaymentMethod(rfq.GetAD_Org_ID());
                            if (result != null && result.Tables[0].Rows.Count > 0)
                            {
                                order.SetVA009_PaymentMethod_ID(Util.GetValueOfInt(result.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]));
                                order.SetPaymentMethod(Util.GetValueOfString(result.Tables[0].Rows[0]["VA009_PaymentBaseType"]));
                                order.SetPaymentRule(Util.GetValueOfString(result.Tables[0].Rows[0]["VA009_PaymentBaseType"]));
                            }
                        }
                        else
                        {
                            order.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                            order.SetPaymentMethod(GetPaymentBaseType(bp.GetVA009_PO_PaymentMethod_ID()));
                            order.SetPaymentRule(PaymentBaseType);
                        }
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
                    //Check Orderno. Already existing
                    if (Orderno.Length > 0)
                    {
                        Orderno.Append(",");
                    }
                    Orderno.Append(order.GetDocumentNo());
                    response.SetC_Order_ID(order.GetC_Order_ID());
                    response.Save();
                }
            }
            // Show the message in RfQ after click Create Purchase Order button
            return Msg.GetMsg(GetCtx(), "VIS_PurchaseOrder", "") + " " + Orderno;
        }
        //Added by Neha Thakur
        /// <summary>
        /// to get the payment method,Payment Type if no payment method found on the business partner
        /// </summary>
        /// <returns>returns payment meyhod ID,Payment Type</returns>
        public DataSet GetPaymentMethod(int Org_ID)
        {
            VA009_PaymentMethod_ID = 0;
            //get organisation default 
            string _sql = "SELECT VA009_PaymentMethod_ID, VA009_PAYMENTBASETYPE FROM VA009_PaymentMethod WHERE VA009_PaymentBaseType='S' AND IsActive='Y' AND AD_ORG_ID IN(@param1,0) ORDER BY AD_ORG_ID DESC, VA009_PAYMENTMETHOD_ID DESC";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param1", Org_ID);
            DataSet _ds = DB.ExecuteDataset(_sql, param, Get_TrxName());
            return _ds;
        }
        /// <summary>
        /// Get PaymentBase Type on Payment Method
        /// </summary>
        /// <param name="VA009_PaymentMethod_ID"></param>
        /// <returns>Payment Base Type</returns>
        public string GetPaymentBaseType(int VA009_PaymentMethod_ID)
        {
            PaymentBaseType = "";
            string _sql = "SELECT VA009_PAYMENTBASETYPE FROM VA009_PAYMENTMETHOD WHERE VA009_PaymentMethod_ID=" + VA009_PaymentMethod_ID;
            PaymentBaseType = Util.GetValueOfString(DB.ExecuteScalar(_sql, null, Get_TrxName()));
            return PaymentBaseType;
        }
        //-------End------
    }
}
