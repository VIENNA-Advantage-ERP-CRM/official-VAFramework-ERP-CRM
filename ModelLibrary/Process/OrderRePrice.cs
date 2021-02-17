/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : OrderRePrice
 * Purpose        : Re-Price Order or Invoice
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     31-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class OrderRePrice : ProcessEngine.SvrProcess
    {
        //	Order to re-price		
        private int _VAB_Order_ID = 0;
        // Invoice to re-price		
        private int _VAB_Invoice_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
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
                else if (name.Equals("VAB_Order_ID"))
                {
                    _VAB_Order_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("VAB_Invoice_ID"))
                {
                    _VAB_Invoice_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            log.Info("VAB_Order_ID=" + _VAB_Order_ID + ", VAB_Invoice_ID=" + _VAB_Invoice_ID);
            if (_VAB_Order_ID == 0 && _VAB_Invoice_ID == 0)
            {
                throw new Exception("Nothing to do");
            }

            String retValue = "";
            if (_VAB_Order_ID != 0)
            {
                MVABOrder order = new MVABOrder(GetCtx(), _VAB_Order_ID, Get_TrxName());
                Decimal oldPrice = order.GetGrandTotal();
                MVABOrderLine[] lines = order.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i].SetPrice(order.GetVAM_PriceList_ID());
                    lines[i].Save();
                }
                order = new MVABOrder(GetCtx(), _VAB_Order_ID, Get_TrxName());
                Decimal newPrice = order.GetGrandTotal();
                retValue = order.GetDocumentNo() + ":  " + oldPrice + " -> " + newPrice;
            }
            if (_VAB_Invoice_ID != 0)
            {
                MVABInvoice invoice = new MVABInvoice(GetCtx(), _VAB_Invoice_ID, null);
                Decimal oldPrice = invoice.GetGrandTotal();
                MVABInvoiceLine[] lines = invoice.GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i].SetPrice(invoice.GetVAM_PriceList_ID(), invoice.GetVAB_BusinessPartner_ID());
                    lines[i].Save();
                }
                invoice = new MVABInvoice(GetCtx(), _VAB_Invoice_ID, null);
                Decimal newPrice = invoice.GetGrandTotal();
                if (retValue.Length > 0)
                {
                    retValue += Env.NL;
                }
                retValue += invoice.GetDocumentNo() + ":  " + oldPrice + " -> " + newPrice;
            }
            //
            return retValue;
        }
    }
}
