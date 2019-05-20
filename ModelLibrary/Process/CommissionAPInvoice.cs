/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CommissionAPInvoice
 * Purpose        : Create AP Invoices for Commission 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan      11-Dec-2009
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
    public class CommissionAPInvoice : ProcessEngine.SvrProcess
    {
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
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (variables are parsed)</returns>
        protected override String DoIt()
        {
            log.Info("doIt - C_CommissionRun_ID=" + GetRecord_ID());
            //	Load Data
            MCommissionRun comRun = new MCommissionRun(GetCtx(), GetRecord_ID(), Get_Trx());
            if (comRun.Get_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No Commission Run");
            }
            if (Env.ZERO.CompareTo(comRun.GetGrandTotal()) == 0)
            {
                throw new ArgumentException("@GrandTotal@ = 0");
            }
            MCommission com = new MCommission(GetCtx(), comRun.GetC_Commission_ID(), Get_Trx());
            if (com.Get_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No Commission");
            }
            if (com.GetC_Charge_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No Charge on Commission");
            }
            MBPartner bp = new MBPartner(GetCtx(), com.GetC_BPartner_ID(), Get_Trx());
            if (bp.Get_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No BPartner");
            }

            //	Create Invoice
            MInvoice invoice = new MInvoice(GetCtx(), 0, null);
            invoice.SetClientOrg(com.GetAD_Client_ID(), com.GetAD_Org_ID());
            invoice.SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_APINVOICE);	//	API
            invoice.SetBPartner(bp);
            //	invoice.setDocumentNo (comRun.getDocumentNo());		//	may cause unique constraint
            invoice.SetSalesRep_ID(GetAD_User_ID());	//	caller
            //
            if (com.GetC_Currency_ID() != invoice.GetC_Currency_ID())
            {
                throw new ArgumentException("CommissionAPInvoice - Currency of PO Price List not Commission Currency");
            }
            //		
            if (!invoice.Save())
            {
                throw new Exception("CommissionAPInvoice - cannot save Invoice");
            }

            //	Create Invoice Line
            MInvoiceLine iLine = new MInvoiceLine(invoice);
            iLine.SetC_Charge_ID(com.GetC_Charge_ID());
            iLine.SetQty(1);
            iLine.SetPrice(comRun.GetGrandTotal());
            iLine.SetTax();
            if (!iLine.Save())
            {
                throw new Exception("CommissionAPInvoice - cannot save Invoice Line");
            }
            //
            return "@C_Invoice_ID@ = " + invoice.GetDocumentNo();
        }

    }
}
