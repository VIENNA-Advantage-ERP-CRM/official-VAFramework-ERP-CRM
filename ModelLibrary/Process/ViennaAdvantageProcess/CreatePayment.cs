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
                MOrder order = new MOrder(GetCtx(), C_Order_ID, null);
                int C_DocType_ID = order.GetC_DocTypeTarget_ID();
                string baseType = DB.ExecuteScalar("SELECT DocSubTypeSO From C_DocType WHERE isactive='Y' AND C_DocType_ID=" + C_DocType_ID).ToString();

                if (!(baseType.Equals("PR") || baseType.Equals("WI")))
                {
                    return "Failed";
                }
                MPayment payment = new MPayment(GetCtx(), 0, null);
                payment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                payment.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                //payment.SetDocumentNo(MS
                payment.SetC_BankAccount_ID(Util.GetValueOfInt(
                    DB.ExecuteScalar("Select c_bankAccount_ID from c_bankaccount where isdefault='Y' and isactive='Y'")));
                payment.SetDateTrx(DateTime.Now);
                payment.SetDateAcct(DateTime.Now);
                payment.SetC_BPartner_ID(order.GetC_BPartner_ID());
                payment.SetPayAmt(order.GetGrandTotal());
                payment.SetC_Currency_ID(order.GetC_Currency_ID());
                payment.SetTenderType("K");
                payment.SetDocStatus("IP");


                if (baseType.Equals("PR")) //prepay Order
                {
                    payment.SetC_Order_ID(order.GetC_Order_ID());
                    payment.SetIsPrepayment(true);
                }
                else if (baseType.Equals("WI"))//OnCreditOrder
                {
                    payment.SetC_Invoice_ID(order.GetC_Invoice_ID());

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
