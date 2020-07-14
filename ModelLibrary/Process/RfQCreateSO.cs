/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RfQCreateSO
 * Purpose        : Create SO for RfQ.
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RfQCreateSO : ProcessEngine.SvrProcess
    {
        //	RfQ 			
        private int _C_RfQ_ID = 0;
        private int _C_DocType_ID = 0;
        //Variable Declaration
        private int VA009_PaymentMethod_ID = 0;
        private string PaymentBaseType = "";

        //100
        private static Decimal ONEHUNDRED = 100;

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
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            _C_RfQ_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process.
        /// A Sales Order is created for the entered Business Partner.  
        /// A sales order line is created for each RfQ line quantity, 
        /// where "Offer Quantity" is selected.  
        /// If on the RfQ Line Quantity, an offer amount is entered (not 0), 
        /// that price is used. 
        /// If a magin is entered on RfQ Line Quantity, it overwrites the 
        /// general margin.  The margin is the percentage added to the 
        /// Best Response Amount.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MRfQ rfq = new MRfQ(GetCtx(), _C_RfQ_ID, Get_TrxName());
            if (rfq.Get_ID() == 0)
            {
                throw new ArgumentException("No RfQ found");
            }
            log.Info("doIt - " + rfq);

            if (rfq.GetC_BPartner_ID() == 0 || rfq.GetC_BPartner_Location_ID() == 0)
            {
                throw new Exception("No Business Partner/Location");
            }
            MBPartner bp = new MBPartner(GetCtx(), rfq.GetC_BPartner_ID(), Get_TrxName());

            MOrder order = new MOrder(GetCtx(), 0, Get_TrxName());
            order.SetIsSOTrx(true);
            if (_C_DocType_ID != 0)
            {
                order.SetC_DocTypeTarget_ID(_C_DocType_ID);
            }
            else
            {
                order.SetC_DocTypeTarget_ID();
            }
            order.SetAD_Org_ID(rfq.GetAD_Org_ID());
            order.SetBPartner(bp);
            order.SetC_BPartner_Location_ID(rfq.GetC_BPartner_Location_ID());
            order.SetSalesRep_ID(rfq.GetSalesRep_ID());
            //Added by Neha Thakur--To set Payment Method,Payment Rule and Payment Method(Button)
            if (bp.GetVA009_PaymentMethod_ID() == 0)
            {
                DataSet result = GetPaymentMethod(rfq.GetAD_Org_ID());
                if (result != null && result.Tables[0].Rows.Count > 0)
                {
                    order.SetVA009_PaymentMethod_ID(Util.GetValueOfInt(result.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]));
                    order.SetPaymentMethod(Util.GetValueOfString(result.Tables[0].Rows[0]["VA009_PaymentBaseType"]));
                    order.SetPaymentRule(Util.GetValueOfString(result.Tables[0].Rows[0]["VA009_PaymentBaseType"]));
                }
            }
            else
            {
                order.SetVA009_PaymentMethod_ID(bp.GetVA009_PaymentMethod_ID());
                order.SetPaymentMethod(GetPaymentBaseType(bp.GetVA009_PaymentMethod_ID()));
                order.SetPaymentRule(PaymentBaseType);
            }
            if (rfq.GetDateWorkComplete() != null)
            {
                order.SetDatePromised(rfq.GetDateWorkComplete());
            }
            order.Save();

            MRfQLine[] lines = rfq.GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                MRfQLine line = lines[i];
                MRfQLineQty[] qtys = line.GetQtys();
                for (int j = 0; j < qtys.Length; j++)
                {
                    MRfQLineQty qty = qtys[j];
                    if (qty.IsActive() && qty.IsOfferQty())
                    {
                        MOrderLine ol = new MOrderLine(order);
                        ol.SetM_Product_ID(line.GetM_Product_ID(),
                            qty.GetC_UOM_ID());
                        ol.SetDescription(line.GetDescription());
                        ol.SetQty(qty.GetQty());
                        //
                        Decimal price = qty.GetOfferAmt();
                        if ( Env.Signum(price) == 0)
                        {
                            price = qty.GetBestResponseAmt();
                            if (Env.Signum(price) == 0)
                            {
                                price = Env.ZERO;
                                log.Warning(" - BestResponse=0 - " + qty);
                            }
                            else
                            {
                                Decimal margin = qty.GetMargin();
                                if ( Env.Signum(margin) == 0)
                                {
                                    margin = rfq.GetMargin();
                                }
                                if ( Env.Signum(margin) != 0)
                                {
                                    margin = Decimal.Add(margin, ONEHUNDRED);
                                    price = Decimal.Round(Decimal.Divide(Decimal.Multiply(price, margin),
                                        ONEHUNDRED), 2, MidpointRounding.AwayFromZero);
                                }
                            }
                        }	//	price
                        ol.SetPrice(price);
                        ol.Save();
                    }	//	Offer Qty
                }	//	All Qtys
            }	//	All Lines
            rfq.SetC_Order_ID(order.GetC_Order_ID());
            rfq.Save();
            return order.GetDocumentNo();
        }
        //Added by Neha Thakur
        /// <summary>
        /// to get the payment method if no payment method found on the business partner
        /// </summary>
        /// <returns>returns payment meyhod ID, Payment Type</returns>
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
            PaymentBaseType= Util.GetValueOfString(DB.ExecuteScalar(_sql, null, Get_TrxName()));
            return PaymentBaseType;
        }
        //-------End------
    }
}
