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

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class CommissionAPInvoice : ProcessEngine.SvrProcess
    {
        //Document Type 
        private int _VAB_DocTypes_ID = 0;
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
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _VAB_DocTypes_ID = para[i].GetParameterAsInt(); ;
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
            log.Info("doIt - VAB_WorkCommission_Calc_ID=" + GetRecord_ID());
            //	Load Data
            MVABWorkCommissionCalc comRun = new MVABWorkCommissionCalc(GetCtx(), GetRecord_ID(), Get_Trx());
            if (comRun.Get_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No Commission Run");
            }
            if (Env.ZERO.CompareTo(comRun.GetGrandTotal()) == 0)
            {
                throw new ArgumentException("@GrandTotal@ = 0");
            }
            MVABWorkCommission com = new MVABWorkCommission(GetCtx(), comRun.GetVAB_WorkCommission_ID(), Get_Trx());
            if (com.Get_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No Commission");
            }
            if (com.GetVAB_Charge_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No Charge on Commission");
            }
            MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), com.GetVAB_BusinessPartner_ID(), Get_Trx());
            if (bp.Get_ID() == 0)
            {
                throw new ArgumentException("CommissionAPInvoice - No BPartner");
            }

            //	Create Expense Invoice 
            MVABInvoice invoice = new MVABInvoice(GetCtx(), 0, null);
            invoice.SetClientOrg(com.GetVAF_Client_ID(), com.GetVAF_Org_ID());
            invoice.SetVAB_DocTypesTarget_ID(_VAB_DocTypes_ID);   //	API
            invoice.SetIsExpenseInvoice(true);
            invoice.SetIsSOTrx(false);
            invoice.SetBPartner(bp);

            // JID_0101: When we generate the AP invoice from Commission run window, its giving price list error.
            if (invoice.GetVAM_PriceList_ID() == 0)
            {
                string sql = "SELECT VAM_PriceList_ID FROM VAM_PriceList WHERE IsActive = 'Y' AND VAF_Client_ID = " + com.GetVAF_Client_ID() + " AND VAF_Org_ID = " + com.GetVAF_Org_ID()
                            + " AND IsDefault='Y' AND IsSOPriceList='N'";
                int pricelist = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (pricelist == 0)
                {
                    try
                    {
                        pricelist = MVAMPriceList.GetDefault(GetCtx(), false).Get_ID();
                    }
                    catch
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "CR_NoDefaultPurchasePL"));
                    }
                }
                if (pricelist > 0)
                {
                    invoice.SetVAM_PriceList_ID(pricelist);
                }
            }
            //	invoice.setDocumentNo (comRun.getDocumentNo());		//	may cause unique constraint
            invoice.SetSalesRep_ID(GetVAF_UserContact_ID());	//	caller
            //
            if (com.GetVAB_Currency_ID() != invoice.GetVAB_Currency_ID())
            {
                throw new ArgumentException("CommissionAPInvoice - Currency of PO Price List not Commission Currency");
            }
            //		
            if (!invoice.Save())
            {
                //return GetReterivedError(invoice, "CommissionAPInvoice - cannot save Invoice");
                throw new Exception("CommissionAPInvoice - cannot save Invoice");
            }

            //	Create Invoice Line
            MVABInvoiceLine iLine = new MVABInvoiceLine(invoice);
            iLine.SetVAB_Charge_ID(com.GetVAB_Charge_ID());
            iLine.SetQty(1);
            iLine.SetPrice(comRun.GetGrandTotal());
            iLine.SetTax();
            if (!iLine.Save())
            {
                //return GetReterivedError(iLine, "CommissionAPInvoice - cannot save Invoice Line");
                throw new Exception("CommissionAPInvoice - cannot save Invoice Line");
            }
            //
            return "@VAB_Invoice_ID@ = " + invoice.GetDocumentNo();
        }

    }
}
