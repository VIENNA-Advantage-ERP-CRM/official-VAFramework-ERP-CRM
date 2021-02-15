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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class InvoiceCreateInOut : ProcessEngine.SvrProcess
    {
        //	Warehouse			
        private int _VAM_Warehouse_ID = 0;
        // Document Type		
        private int _VAB_DocTypes_ID = 0;
        // Invoice			
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
                else if (name.Equals("VAM_Warehouse_ID"))
                {
                    _VAM_Warehouse_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _VAB_DocTypes_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAB_Invoice_ID = GetRecord_ID();
        }


        /// <summary>
        /// 	Create Shipment
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            //log.info("VAB_Invoice_ID=" + _VAB_Invoice_ID 
            //    + ", VAM_Warehouse_ID=" + _VAM_Warehouse_ID
            //    + ", VAB_DocTypes_ID=" + _VAB_DocTypes_ID);
            if (_VAB_Invoice_ID == 0)
            {
                throw new ArgumentException("@NotFound@ @VAB_Invoice_ID@");
            }
            if (_VAM_Warehouse_ID == 0)
            {
                throw new ArgumentException("@NotFound@ @VAM_Warehouse_ID@");
            }
            //
            MInvoice invoice = new MInvoice(GetCtx(), _VAB_Invoice_ID, Get_Trx());
            if (invoice.Get_ID() == 0)
            {
                throw new ArgumentException("@NotFound@ @VAB_Invoice_ID@");
            }
            if (!MInvoice.DOCSTATUS_Completed.Equals(invoice.GetDocStatus()))
            {
                throw new ArgumentException("@InvoiceCreateDocNotCompleted@");
            }
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), _VAB_DocTypes_ID);
            if (invoice.IsSOTrx() != dt.IsSOTrx()
                || invoice.IsReturnTrx() != dt.IsReturnTrx())
            {
                throw new ArgumentException("@VAB_DocTypes_ID@ <> @VAB_Invoice_ID@");
            }

            //*****************************Vikas  1 Dec 2015  *********************************
            //Case Msg Not Showing Proper
            MInOut ship = null;
            MVABOrder ord = new MVABOrder(GetCtx(), invoice.GetVAB_Order_ID(), null);
            if (ord.GetVAB_BusinessPartner_ID() > 0)
            {
                ship = new MInOut(invoice, _VAB_DocTypes_ID, null, _VAM_Warehouse_ID);
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
             MInOut ship = new MInOut(invoice, _VAB_DocTypes_ID, null, _VAM_Warehouse_ID);
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
                    && ((product != null && product.GetProductType() != MProduct.PRODUCTTYPE_Item) || invoiceLine.GetVAB_Charge_ID() != 0))
                {
                    continue;
                }
                
                MInOutLine sLine = new MInOutLine(ship);
                //JID_1679 Generate Receipt from Invoice(Vendor) for remaining quantity 
                 decimal movementqty = 0;
                if (invoiceLine.GetVAB_OrderLine_ID() != 0)
                {
                    decimal? res = 0;
                     movementqty = Util.GetValueOfDecimal(DB.ExecuteScalar(@" select (QtyOrdered-sum(MovementQty))   from VAB_OrderLine ol Inner join VAM_Inv_InOutLine il on il.VAB_Orderline_ID= ol.VAB_Orderline_Id "
                             + " WHERE il.VAB_OrderLine_ID =" + invoiceLine.GetVAB_OrderLine_ID() + "group by QtyOrdered", null, Get_Trx()));
                    // in case of partial receipt
                    if ( invoiceLine.GetQtyInvoiced() > movementqty && movementqty!=0)
                    {
                        if (product.GetVAB_UOM_ID() != invoiceLine.GetVAB_UOM_ID())
                        {
                            res = MUOMConversion.ConvertProductTo(GetCtx(), product.GetVAM_Product_ID(), invoiceLine.GetVAB_UOM_ID(), movementqty);
                        }
                        sLine.SetInvoiceLine(invoiceLine, 0,    //	Locator
                            invoice.IsSOTrx() ? (movementqty) : Env.ZERO);
                        sLine.SetQtyEntered(res==0?(movementqty):res);
                        sLine.SetMovementQty(movementqty);
                    }
                    // if QtyInvoiced is less or No Material receipt is found against the order
                    else
                    {
                        sLine.SetInvoiceLine(invoiceLine, 0,    //	Locator 
                          invoice.IsSOTrx() ? invoiceLine.GetQtyInvoiced() : Env.ZERO);
                        sLine.SetQtyEntered(invoiceLine.GetQtyEntered());
                        sLine.SetMovementQty(invoiceLine.GetQtyInvoiced());
                    }
                }
                else
                {
                    sLine.SetInvoiceLine(invoiceLine, 0,	//	Locator 
                     invoice.IsSOTrx() ? invoiceLine.GetQtyInvoiced() : Env.ZERO);
                    sLine.SetQtyEntered(invoiceLine.GetQtyEntered());
                    sLine.SetMovementQty(invoiceLine.GetQtyInvoiced());
                }
                if (invoice.IsCreditMemo())
                {
                    sLine.SetQtyEntered(Decimal.Negate(sLine.GetQtyEntered()));//.negate());
                    sLine.SetMovementQty(Decimal.Negate(sLine.GetMovementQty()));//.negate());
                }
                if (!sLine.Save())
                {
                    ship.Get_Trx().Rollback();
                    if (movementqty == 0)
                    {
                        return  Msg.GetMsg(GetCtx(), "MRIsAlreadyCreated");
                    }
                    else
                    {
                        return GetRetrievedError(sLine, "@SaveError@ @VAM_Inv_InOutLine_ID@");
                    }
                    //throw new ArgumentException("@SaveError@ @VAM_Inv_InOutLine_ID@");  
                }
                //
                invoiceLine.SetVAM_Inv_InOutLine_ID(sLine.GetVAM_Inv_InOutLine_ID());
                if (!invoiceLine.Save())
                {
                    return GetRetrievedError(invoiceLine, "@SaveError@ @VAB_InvoiceLine_ID@");
                    //throw new ArgumentException("@SaveError@ @VAB_InvoiceLine_ID@");
                }
            }
            return ship.GetDocumentNo();
        }
    }
}
