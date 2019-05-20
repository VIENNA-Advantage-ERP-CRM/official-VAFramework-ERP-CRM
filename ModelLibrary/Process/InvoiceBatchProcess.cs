/********************************************************
 * Module Name    : Process
 * Purpose        : Process the Invoice Batch
 * Author         : Jagmohan Bhatt
 * Date           : 04-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    /// <summary>
    /// Process Invoice Batch	
    /// </summary>
    public class InvoiceBatchProcess : ProcessEngine.SvrProcess
    {
        /**	Batch to process		*/
        private int _C_InvoiceBatch_ID = 0;
        /** Action					*/
        private String _DocAction = null;

        /** Invoice					*/
        private MInvoice _invoice = null;
        /** Old DocumentNo			*/
        private String _oldDocumentNo = null;
        /** Old BPartner			*/
        private int _oldC_BPartner_ID = 0;
        /** Old BPartner Loc		*/
        private int _oldC_BPartner_Location_ID = 0;

        /** Counter					*/
        private int _count = 0;


        /// <summary>
        /// Prepare - Get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                { }
                else if (name.Equals("DocAction"))
                    _DocAction = (String)para[i].GetParameter();
            }
            _C_InvoiceBatch_ID = GetRecord_ID();
        }   //  prepare



        /// <summary>
        /// Returns the signum function of this Decimal.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private int Signum(Decimal d)
        {
            if (d < 0)
                return -1;
            else if (d == 0)
                return 0;
            else if (d > 0)
                return 1;

            return 0;
        }

        /// <summary>
        /// Process Invoice Batch
        /// </summary>
        /// <returns></returns>
        protected override String DoIt()
        {
            log.Info("C_InvoiceBatch_ID=" + _C_InvoiceBatch_ID + ", DocAction=" + _DocAction);
            if (_C_InvoiceBatch_ID == 0)
                throw new Exception("C_InvoiceBatch_ID = 0");
            MInvoiceBatch batch = new MInvoiceBatch(GetCtx(), _C_InvoiceBatch_ID, Get_TrxName());
            if (batch.Get_ID() == 0)
                throw new Exception("@NotFound@: @C_InvoiceBatch_ID@ - " + _C_InvoiceBatch_ID);
            if (batch.IsProcessed())
                throw new Exception("@Processed@");
            //
            if (Signum(batch.GetControlAmt()) != 0 && batch.GetControlAmt().CompareTo(batch.GetDocumentAmt()) != 0)
                throw new Exception("@ControlAmt@ <> @DocumentAmt@");
            //
            MInvoiceBatchLine[] lines = batch.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInvoiceBatchLine line = lines[i];
                if (line.GetC_Invoice_ID() != 0 || line.GetC_InvoiceLine_ID() != 0)
                    continue;

                if ((_oldDocumentNo != null
                        && !_oldDocumentNo.Equals(line.GetDocumentNo()))
                    || _oldC_BPartner_ID != line.GetC_BPartner_ID()
                    || _oldC_BPartner_Location_ID != line.GetC_BPartner_Location_ID())
                    CompleteInvoice();
                //	New Invoice
                if (_invoice == null)
                {
                    _invoice = new MInvoice(batch, line);
                    if (!_invoice.Save())
                        throw new Exception("Cannot save Invoice");
                    //
                    _oldDocumentNo = line.GetDocumentNo();
                    _oldC_BPartner_ID = line.GetC_BPartner_ID();
                    _oldC_BPartner_Location_ID = line.GetC_BPartner_Location_ID();
                }

                if (line.IsTaxIncluded() != _invoice.IsTaxIncluded())
                {
                    //	rollback
                    throw new Exception("Line " + line.GetLine() + " TaxIncluded inconsistent");
                }

                //	Add Line
                MInvoiceLine invoiceLine = new MInvoiceLine(_invoice);
                invoiceLine.SetDescription(line.GetDescription());
                invoiceLine.SetC_Charge_ID(line.GetC_Charge_ID());
                invoiceLine.SetQty(line.GetQtyEntered());	// Entered/Invoiced
                invoiceLine.SetPrice(line.GetPriceEntered());
                invoiceLine.SetC_Tax_ID(line.GetC_Tax_ID());
                invoiceLine.SetTaxAmt(line.GetTaxAmt());
                invoiceLine.SetLineNetAmt(line.GetLineNetAmt());
                invoiceLine.SetLineTotalAmt(line.GetLineTotalAmt());
                if (!invoiceLine.Save())
                {
                    //	rollback
                    throw new Exception("Cannot save Invoice Line");
                }

                //	Update Batch Line
                line.SetC_Invoice_ID(_invoice.GetC_Invoice_ID());
                line.SetC_InvoiceLine_ID(invoiceLine.GetC_InvoiceLine_ID());
                line.Save();
            }	//	for all lines

            CompleteInvoice();
            //
            batch.SetProcessed(true);
            batch.Save();

            return "#" + _count;
        }	//	doIt


        /// <summary>
        /// Complete Invoice
        /// </summary>
        private void CompleteInvoice()
        {
            if (_invoice == null)
                return;

            _invoice.SetDocAction(_DocAction);
            _invoice.ProcessIt(_DocAction);
            _invoice.Save();

            AddLog(0, _invoice.GetDateInvoiced(), _invoice.GetGrandTotal(), _invoice.GetDocumentNo());
            _count++;

            _invoice = null;
        }	//	completeInvoice
    }
}
