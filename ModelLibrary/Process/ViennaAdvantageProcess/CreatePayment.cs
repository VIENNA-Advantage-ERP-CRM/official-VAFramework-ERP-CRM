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
using System.Data;

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
                int C_DocType_ID = order.GetC_DocTypeTarget_ID();
                string baseType = DB.ExecuteScalar("SELECT DocSubTypeSO From C_DocType WHERE isactive='Y' AND C_DocType_ID=" + C_DocType_ID).ToString();
                int C_Invoice_ID = order.GetC_Invoice_ID();
                if (!(baseType.Equals("PR") || baseType.Equals("WI")))
                {
                    return "Order Type must be Prepay Order or Credit Order.";
                }
                MPayment payment = new MPayment(GetCtx(), 0, Get_TrxName());
                payment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                payment.SetAD_Org_ID(order.GetAD_Org_ID());
                //payment.SetDocumentNo(MS
                payment.SetC_BankAccount_ID(Util.GetValueOfInt(DB.ExecuteScalar("SELECT c_bankAccount_ID FROM c_bankaccount WHERE isdefault='Y' AND isactive='Y'")));
                payment.SetDateTrx(DateTime.Now);
                payment.SetDateAcct(DateTime.Now);
                payment.SetC_BPartner_ID(order.GetC_BPartner_ID());
                //(1052) Set Business Partner Location
                payment.SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());
                payment.SetPayAmt(order.GetGrandTotal());
                payment.SetC_Currency_ID(order.GetC_Currency_ID());
                //(1052) Set Currency Type
                payment.SetC_ConversionType_ID(order.GetC_ConversionType_ID());              
                payment.SetTenderType("K");
                payment.SetDocStatus("IP");
                //(1052) Apply org check
                C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_DocType_ID From C_DocType WHERE isactive='Y' AND " +
                "DocBaseType = 'ARR' AND AD_Client_ID = " + order.GetAD_Client_ID() + " AND AD_Org_ID IN (0,"+order.GetAD_Org_ID()+ ") ORDER BY IsDefault DESC,AD_Org_ID DESC"));
                payment.SetC_DocType_ID(C_DocType_ID);
                if (baseType.Equals("PR")) //prepay Order
                {
                    payment.SetC_Order_ID(order.GetC_Order_ID());
                    payment.SetIsPrepayment(true);
                }
                else if (baseType.Equals("WI"))//OnCreditOrder
                {
                   // payment.SetC_Invoice_ID(order.GetC_Invoice_ID());                 
                }
                if (!payment.Save()) 
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    string val = pp.GetValue();
                    if (string.IsNullOrEmpty(val))
                    {
                        val = pp.GetName();
                    }
                    return Msg.GetMsg(GetCtx(), "PaymentNotCreated") + " "+ val;
                }
                else
                {
                    if (baseType.Equals("WI")) 
                    {
                        //credit order case : update invoice and Invoice schdule refrence
                        DataSet ds = null;
                        ds = DB.ExecuteDataset("SELECT C_InvoicePaySchedule_ID, DueAmt FROM C_InvoicePaySchedule WHERE C_Invoice_ID = " + C_Invoice_ID));
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count == 1)
                            {
                                //set Invoice PaySchedule and Invoice in case of single invoice schedule
                                int count =  DB.ExecuteQuery("UPDATE C_Payment SET C_InvoicePaySchedule_ID= " + Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_InvoicePaySchedule_ID"]) +
                                    ",C_Invoice_ID="+ C_Invoice_ID + " WHERE C_Payment_ID=" + payment.GetC_Payment_ID(),null,Get_TrxName());
                            }
                            else
                            {
                                //in case of multiple invoice schedule create PaymentAllocates
                                MPaymentAllocate alloc = null;
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    alloc = new MPaymentAllocate(GetCtx(), 0, Get_TrxName());
                                    alloc.SetAD_Client_ID(payment.GetAD_Client_ID());
                                    alloc.SetAD_Org_ID(payment.GetAD_Org_ID());
                                    alloc.SetC_Payment_ID(payment.GetC_Payment_ID());
                                    alloc.SetC_Invoice_ID(C_Invoice_ID);
                                    alloc.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_InvoicePaySchedule_ID"]));
                                    alloc.SetAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["DueAmt"]));
                                    alloc.SetInvoiceAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["DueAmt"]));

                                    if(!alloc.Save()){
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        string val = pp.GetValue();
                                        if (string.IsNullOrEmpty(val))
                                        {
                                            val = pp.GetName();
                                        }
                                        log.Severe("Payment Allocate not saved " + val);
                                    }                                  
                                }                              
                            }
                        } 
                    }
                }
                return payment.GetDocumentNo().ToString();
            }
            catch(Exception ex)
            {
                log.Severe("GeneratePayment Exception:"+ex.Message);
                return "Failed:"+ ex.Message;
            }
        }
    }
}
