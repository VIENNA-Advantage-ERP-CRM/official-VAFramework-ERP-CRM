/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InvoiceCreateInOut  
 * Purpose        : Create (Generate) Receipt from Invoice
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     20-Aug-2009
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class InvoiceCreateInOut : ProcessEngine.SvrProcess
    {
        //	Warehouse			
        private int _M_Warehouse_ID = 0;
        // Document Type		
        private int _C_DocType_ID = 0;
        // Invoice			
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
                else if (name.Equals("M_Warehouse_ID"))
                {
                    _M_Warehouse_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_Invoice_ID = GetRecord_ID();
        }


        /// <summary>
        /// 	Create Shipment
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            //log.info("C_Invoice_ID=" + _C_Invoice_ID 
            //    + ", M_Warehouse_ID=" + _M_Warehouse_ID
            //    + ", C_DocType_ID=" + _C_DocType_ID);
            if (_C_Invoice_ID == 0)
            {
                throw new ArgumentException("@NotFound@ @C_Invoice_ID@");
            }
            if (_M_Warehouse_ID == 0)
            {
                throw new ArgumentException("@NotFound@ @M_Warehouse_ID@");
            }
            //
            MInvoice invoice = new MInvoice(GetCtx(), _C_Invoice_ID, null);
            if (invoice.Get_ID() == 0)
            {
                throw new ArgumentException("@NotFound@ @C_Invoice_ID@");
            }
            if (!MInvoice.DOCSTATUS_Completed.Equals(invoice.GetDocStatus()))
            {
                throw new ArgumentException("@InvoiceCreateDocNotCompleted@");
            }
            MDocType dt = MDocType.Get(GetCtx(), _C_DocType_ID);
            if (invoice.IsSOTrx() != dt.IsSOTrx()
                || invoice.IsReturnTrx() != dt.IsReturnTrx())
            {
                throw new ArgumentException("@C_DocType_ID@ <> @C_Invoice_ID@");
            }

            //*****************************Vikas  1 Dec 2015  *********************************
            //Case Msg Not Showing Proper
            MInOut ship = null;
            MOrder ord = new MOrder(GetCtx(), invoice.GetC_Order_ID(), null);
            if (ord.GetC_BPartner_ID() > 0)
            {
                ship = new MInOut(invoice, _C_DocType_ID, null, _M_Warehouse_ID);
                // Change by Mohit Asked by Amardeep sir 02/03/2016
                ship.SetPOReference(invoice.GetPOReference());
                // End
                if (!ship.Save())
                {
                    return GetRetrievedError(ship, "@SaveError@ Receipt");
                   // throw new ArgumentException("@SaveError@ Receipt");
                }
            }
            else
            {
                return GetRetrievedError(ship, "InvoiceNotLinkedWithPO");
                //throw new ArgumentException("@InvoiceNotLinkedWithPO@");
            }
            /*
             MInOut ship = new MInOut(invoice, _C_DocType_ID, null, _M_Warehouse_ID);
               if (!ship.Save())
               {
                   throw new ArgumentException("@SaveError@ Receipt");
               }
            */
            //************************END*****************************************
            MInvoiceLine[] invoiceLines = invoice.GetLines(false);
            for (int i = 0; i < invoiceLines.Length; i++)
            {
                MInvoiceLine invoiceLine = invoiceLines[i];
                MInOutLine sLine = new MInOutLine(ship);
                sLine.SetInvoiceLine(invoiceLine, 0,	//	Locator 
                    invoice.IsSOTrx() ? invoiceLine.GetQtyInvoiced() : Env.ZERO);
                sLine.SetQtyEntered(invoiceLine.GetQtyEntered());
                sLine.SetMovementQty(invoiceLine.GetQtyInvoiced());
                if (invoice.IsCreditMemo())
                {
                    sLine.SetQtyEntered(Decimal.Negate(sLine.GetQtyEntered()));//.negate());
                    sLine.SetMovementQty(Decimal.Negate(sLine.GetMovementQty()));//.negate());
                }
                if (!sLine.Save())
                {
                    return GetRetrievedError(sLine, "@SaveError@ @M_InOutLine_ID@");
                    //throw new ArgumentException("@SaveError@ @M_InOutLine_ID@");
                }
                //
                invoiceLine.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                if (!invoiceLine.Save())
                {
                    return GetRetrievedError(invoiceLine, "@SaveError@ @C_InvoiceLine_ID@");
                    //throw new ArgumentException("@SaveError@ @C_InvoiceLine_ID@");
                }
            }
            return ship.GetDocumentNo();
        }
    }
}
