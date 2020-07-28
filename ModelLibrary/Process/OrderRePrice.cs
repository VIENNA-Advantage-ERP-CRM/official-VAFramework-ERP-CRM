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
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class OrderRePrice : ProcessEngine.SvrProcess
    {
        //	Order to re-price		
        private int _C_Order_ID = 0;
        // Invoice to re-price		
        private int _C_Invoice_ID = 0;

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
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_Invoice_ID"))
                {
                    _C_Invoice_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
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
            log.Info("C_Order_ID=" + _C_Order_ID + ", C_Invoice_ID=" + _C_Invoice_ID);
            if (_C_Order_ID == 0 && _C_Invoice_ID == 0)
            {
                throw new Exception("Nothing to do");
            }

            String retValue = "";
            if (_C_Order_ID != 0)
            {
                MOrder order = new MOrder(GetCtx(), _C_Order_ID, Get_TrxName());
                Decimal oldPrice = order.GetGrandTotal();
                MOrderLine[] lines = order.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i].SetPrice(order.GetM_PriceList_ID());
                    lines[i].Save();
                }
                order = new MOrder(GetCtx(), _C_Order_ID, Get_TrxName());
                Decimal newPrice = order.GetGrandTotal();
                retValue = order.GetDocumentNo() + ":  " + oldPrice + " -> " + newPrice;
            }
            if (_C_Invoice_ID != 0)
            {
                MInvoice invoice = new MInvoice(GetCtx(), _C_Invoice_ID, null);
                Decimal oldPrice = invoice.GetGrandTotal();
                MInvoiceLine[] lines = invoice.GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i].SetPrice(invoice.GetM_PriceList_ID(), invoice.GetC_BPartner_ID());
                    lines[i].Save();
                }
                invoice = new MInvoice(GetCtx(), _C_Invoice_ID, null);
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
