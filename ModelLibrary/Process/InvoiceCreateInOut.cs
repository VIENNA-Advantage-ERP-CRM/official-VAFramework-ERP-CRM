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

        private String _processMsg = String.Empty;

        //private String _processMsg = String.Empty;
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
            MInvoice invoice = new MInvoice(GetCtx(), _C_Invoice_ID, Get_Trx());
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

                MProduct product = invoiceLine.GetProduct();
                //	Nothing to Deliver

                // Get the lines of Invoice based on the setting taken on Tenant to allow non item Product         
                if (Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem")).Equals("N")
                    && ((product != null && product.GetProductType() != MProduct.PRODUCTTYPE_Item) || invoiceLine.GetC_Charge_ID() != 0))
                {
                    continue;
                }
                
                MInOutLine sLine = new MInOutLine(ship);
                //JID_1679 Generate Receipt from Invoice(Vendor) for remaining quantity 
                //decimal movementqty = 0;
                //if (invoiceLine.GetC_OrderLine_ID() != 0)
                //{
                //    decimal? res = 0;
                //    movementqty = Util.GetValueOfDecimal(DB.ExecuteScalar(@" select (QtyOrdered-sum(MovementQty))   from C_OrderLine ol Inner join M_InOutLine il on il.C_orderline_ID= ol.C_Orderline_Id "
                //            + " WHERE il.C_OrderLine_ID =" + invoiceLine.GetC_OrderLine_ID() + "group by QtyOrdered", null, Get_Trx()));
                //    // in case of partial receipt
                //    if (invoiceLine.GetQtyInvoiced() > movementqty && movementqty != 0)
                //    {
                //        if (product.GetC_UOM_ID() != invoiceLine.GetC_UOM_ID())
                //        {
                //            res = MUOMConversion.ConvertProductTo(GetCtx(), product.GetM_Product_ID(), invoiceLine.GetC_UOM_ID(), movementqty);
                //        }
                //        sLine.SetInvoiceLine(invoiceLine, 0,    //	Locator
                //            invoice.IsSOTrx() ? (movementqty) : Env.ZERO);
                //        sLine.SetQtyEntered(res == 0 ? (movementqty) : res);
                //        sLine.SetMovementQty(movementqty);
                //    }
                //    // if QtyInvoiced is less or No Material receipt is found against the order
                //    else
                //    {
                //        sLine.SetInvoiceLine(invoiceLine, 0,    //	Locator 
                //          invoice.IsSOTrx() ? invoiceLine.GetQtyInvoiced() : Env.ZERO);
                //        sLine.SetQtyEntered(invoiceLine.GetQtyEntered());
                //        sLine.SetMovementQty(invoiceLine.GetQtyInvoiced());
                //    }
                //}
                //else
                //{
                    sLine.SetInvoiceLine(invoiceLine, 0,	//	Locator 
                     invoice.IsSOTrx() ? invoiceLine.GetQtyInvoiced() : Env.ZERO);
                    sLine.SetQtyEntered(invoiceLine.GetQtyEntered());
                    sLine.SetMovementQty(invoiceLine.GetQtyInvoiced());
                //}
                if (invoice.IsCreditMemo())
                {
                    sLine.SetQtyEntered(Decimal.Negate(sLine.GetQtyEntered()));//.negate());
                    sLine.SetMovementQty(Decimal.Negate(sLine.GetMovementQty()));//.negate());
                }
                //190 - Get Print description and set
                if (sLine.Get_ColumnIndex("PrintDescription") >= 0 && invoiceLine.Get_ColumnIndex("PrintDescription") >= 0)
                    sLine.Set_Value("PrintDescription", invoiceLine.Get_Value("PrintDescription"));
                if (!sLine.Save())
                {
                    ship.Get_Trx().Rollback();
                    //if (movementqty == 0)
                    //{
                    //    _processMsg += ", LineNo: " + invoiceLine.GetLine() + Msg.GetMsg(GetCtx(), "MRIsAlreadyCreated");
                    //    return _processMsg;
                    //}
                    //else
                    //{
                        return GetRetrievedError(sLine, "@SaveError@ @M_InOutLine_ID@");
                    //}
                   // throw new ArgumentException("@SaveError@ @M_InOutLine_ID@");
                }
                invoiceLine.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
              //  _processMsg+= ", LineNo: "+invoiceLine.GetLine()+Msg.GetMsg(GetCtx(), "MRCreatedWithDocNo" + ship.GetDocumentNo());
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
