/********************************************************
 * Project Name   : VIS
 * Class Name     : CalloutPaymentModel
 * Purpose        : Used for payment callout.......
 * Chronological    Development
 * Mohit            27/04/2017
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class CalloutPaymentModel
    {
        Ctx ctx = null;
        public CalloutPaymentModel(Ctx _ctx)
        {
            ctx = _ctx;
        }

        // Payment callout- Invoice selection
        public bool CheckedModuleInfo(string Prefix)
        {
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='" + Prefix + "'  AND IsActive = 'Y'", null, null)) > 0)
            {
                return true;
            }
            return false;
        }

        public LcDetails GetLcDetail(int Invoice_ID, Ctx ctx)
        {
            LcDetails lc = new LcDetails();
            MInvoice inv = new MInvoice(ctx, Invoice_ID, null);
            if (inv.GetC_Order_ID() > 0)
            {
                MOrder order = new MOrder(ctx, inv.GetC_Order_ID(), null);
                try
                {
                    lc.paymethod = order.GetVA009_PaymentMethod_ID();
                    if (Util.GetValueOfString(DB.ExecuteScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod  WHERE VA009_PaymentMethod_ID=" + order.GetVA009_PaymentMethod_ID(), null, null)) == "L")
                    {
                        lc.lcno = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(VA026_LCDetail_ID)  FROM VA026_LCDetail 
                                                            WHERE IsActive = 'Y' AND DocStatus IN ('CO' , 'CL')  AND
                                                            c_order_id =" + order.GetC_Order_ID(), null, null));
                        // Check PO Detail tab of Letter of Credit
                        if (lc.lcno == 0)
                        {
                            lc.lcno = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(lc.VA026_LCDetail_ID)  FROM VA026_LCDetail lc
                                                        INNER JOIN VA026_PODetail sod ON sod.VA026_LCDetail_ID = lc.VA026_LCDetail_ID 
                                                            WHERE sod.IsActive = 'Y' AND lc.IsActive = 'Y' AND lc.DocStatus IN ('CO' , 'CL')  AND
                                                            sod.C_Order_ID =" + order.GetC_Order_ID(), null, null));
                            if (lc.lcno != 0)
                            {
                                if (Util.GetValueOfString(DB.ExecuteScalar("SELECT VA026_IsLoanRequired FROM VA026_LCDetail WHERE VA026_LCDetail_ID=" + lc.lcno, null, null)) == "Y")
                                {
                                    lc.trloan_id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VA026_TRLoanApplication_ID FROM VA026_TRLoanApplication WHERE IsActive='Y' AND DOCSTATUS='CO'  AND  VA026_LCDetail_ID= " + lc.lcno, null, null));
                                }
                                

                            }
                        }
                        else
                        {
                            if (Util.GetValueOfString(DB.ExecuteScalar("SELECT VA026_IsLoanRequired FROM VA026_LCDetail WHERE VA026_LCDetail_ID=" + lc.lcno, null, null)) == "Y")
                            {
                                lc.trloan_id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VA026_TRLoanApplication_ID FROM VA026_TRLoanApplication WHERE IsActive='Y' AND DOCSTATUS='CO'  AND VA026_LCDetail_ID= " + lc.lcno, null, null));
                            }
                        }
                    }
                    else
                    {
                        lc.lcno = 0;
                        lc.trloan_id = 0;
                    }
                }
                catch (Exception e)
                {

                }
            }


            return lc;
        }
        //
    }
    public class LcDetails
    {
        public int lcno { get; set; }
        public int paymethod { get; set; }
        public int trloan_id { get; set; }
    }
}