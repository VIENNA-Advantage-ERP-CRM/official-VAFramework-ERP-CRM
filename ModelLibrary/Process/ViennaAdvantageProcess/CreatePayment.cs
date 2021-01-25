using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using ViennaAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Model;
namespace ViennaAdvantage.Process
{
    class CreatePayment : SvrProcess
    {
        int C_Order_ID = 0;
        protected override void Prepare()
        {


            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                //	log.fine("prepare - " + para[i]);
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_Order_ID"))
                {
                    C_Order_ID = para[i].GetParameterAsInt();
                }

                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }


        protected override string DoIt()
        {
            try
            {

                if (C_Order_ID < 1)
                {
                    return "Failed";
                }
                //int c_Order_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Order_ID From C_Order WHERE DocumentNo=" + salesOrderNo));
                MOrder order = new MOrder(GetCtx(), C_Order_ID, Get_TrxName());
                int VAB_DocTypes_ID = order.GetVAB_DocTypesTarget_ID();
                string baseType = DB.ExecuteScalar("SELECT DocSubTypeSO From VAB_DocTypes WHERE isactive='Y' AND VAB_DocTypes_ID=" + VAB_DocTypes_ID).ToString();

                if (!(baseType.Equals("PR") || baseType.Equals("WI")))
                {
                    return "Order Type must be Prepay Order or Credit Order.";
                }
                MPayment payment = new MPayment(GetCtx(), 0, Get_TrxName());
                payment.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                payment.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                //payment.SetDocumentNo(MS
                payment.SetVAB_Bank_Acct_ID(Util.GetValueOfInt(DB.ExecuteScalar("Select VAB_Bank_Acct_ID from VAB_Bank_Acct where isdefault='Y' and isactive='Y'")));
                payment.SetDateTrx(DateTime.Now);
                payment.SetDateAcct(DateTime.Now);
                payment.SetVAB_BusinessPartner_ID(order.GetVAB_BusinessPartner_ID());
                payment.SetPayAmt(order.GetGrandTotal());
                payment.SetVAB_Currency_ID(order.GetVAB_Currency_ID());
                payment.SetTenderType("K");
                payment.SetDocStatus("IP");
                VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_DocTypes_ID From VAB_DocTypes WHERE isactive='Y' AND DocBaseType = 'ARR' AND VAF_Client_ID = " + order.GetVAF_Client_ID() + " ORDER BY IsDefault DESC"));
                payment.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                if (baseType.Equals("PR")) //prepay Order
                {
                    payment.SetC_Order_ID(order.GetC_Order_ID());
                    payment.SetIsPrepayment(true);
                }
                else if (baseType.Equals("WI"))//OnCreditOrder
                {
                    payment.SetVAB_Invoice_ID(order.GetVAB_Invoice_ID());

                }
                payment.Save();

                return payment.GetDocumentNo().ToString();
            }
            catch
            {
                return "Failed";
            }
        }

    }
}
