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
                    return Msg.GetMsg(GetCtx(), "Failed");
                }
                string paymentbaseType = "";
                string orderDocBaseType = "";
                int invoiceScheduleCount = 0;
                int invoicePaySchedule_ID = 0;
                //int c_Order_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Order_ID From C_Order WHERE DocumentNo=" + salesOrderNo));
                MOrder order = new MOrder(GetCtx(), C_Order_ID, Get_TrxName());
                int C_Invoice_ID = order.GetC_Invoice_ID();

                string sql = @"SELECT DT.DocSubTypeSO,  PM.VA009_PaymentBaseType, MAX(C_InvoicePaySchedule_ID) AS C_InvoicePaySchedule_ID,COUNT(IPS.C_InvoicePaySchedule_ID) AS Invcount 
                FROM C_Order Ord INNER JOIN C_DocType DT ON (DT.C_DocType_ID=Ord.C_DocType_ID) INNER JOIN VA009_PaymentMethod PM ON (PM.VA009_PaymentMethod_ID = Ord.VA009_PaymentMethod_ID)
                LEFT JOIN C_Invoice Inv ON (Inv.C_Order_ID = Ord.C_Order_ID) LEFT JOIN C_InvoicePaySchedule IPS ON (IPS.C_Invoice_ID= Inv.C_Invoice_ID)
                WHERE Ord.C_Order_ID=" + C_Order_ID + " Group BY DT.DocSubTypeSO,  PM.VA009_PaymentBaseType";

                DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    paymentbaseType = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA009_PaymentBaseType"]);
                    orderDocBaseType = Util.GetValueOfString(ds.Tables[0].Rows[0]["DocSubTypeSO"]);
                    invoiceScheduleCount = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Invcount"]);
                    invoicePaySchedule_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_InvoicePaySchedule_ID"]);
                }
                if (!orderDocBaseType.Equals(MDocType.DOCSUBTYPESO_PrepayOrder) && !orderDocBaseType.Equals(MDocType.DOCSUBTYPESO_OnCreditOrder))
                {
                    //donot create payment if order is not prepay or credit
                    return Msg.GetMsg(GetCtx(), "PrepayCreditOrder");
                }
                MPayment payment = new MPayment(GetCtx(), 0, Get_TrxName());
                payment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                payment.SetAD_Org_ID(order.GetAD_Org_ID());
                //payment.SetDocumentNo(MS
                payment.SetC_BankAccount_ID(Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BankAccount_ID FROM C_BankAccount WHERE IsDefault='Y' AND IsActive='Y'" +
                    " AND AD_Org_ID IN (" + order.GetAD_Org_ID() + ",0) AND AD_Client_ID=" + GetAD_Client_ID() + " ORDER BY AD_Org_ID")));
                payment.SetDateTrx(DateTime.Now);
                payment.SetDateAcct(DateTime.Now);
                payment.SetC_BPartner_ID(order.GetC_BPartner_ID());
                //(1052) Set Business Partner Location
                payment.SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());
                payment.SetPayAmt(order.GetGrandTotal());
                payment.SetC_Currency_ID(order.GetC_Currency_ID());
                //(1052) Set Currency Type
                payment.SetC_ConversionType_ID(order.GetC_ConversionType_ID());
                payment.SetTenderType(MPayment.TENDERTYPE_Check);
                payment.SetDocStatus(MPayment.DOCSTATUS_InProgress);
                //(1052) Apply org check
                int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_DocType_ID From C_DocType WHERE isactive='Y' AND " +
                "DocBaseType = 'ARR' AND AD_Client_ID = " + order.GetAD_Client_ID() + " AND AD_Org_ID IN (0," + order.GetAD_Org_ID() + ") ORDER BY IsDefault DESC,AD_Org_ID DESC"));
                payment.SetC_DocType_ID(C_DocType_ID);
                if (orderDocBaseType.Equals(MDocType.DOCSUBTYPESO_PrepayOrder)) //prepay Order
                {
                    payment.SetC_Order_ID(order.GetC_Order_ID());
                    payment.SetIsPrepayment(true);
                }
                else if (orderDocBaseType.Equals(MDocType.DOCSUBTYPESO_OnCreditOrder))//OnCreditOrder
                {
                    if (invoiceScheduleCount == 1)
                    {
                        //if invoice has only one schedule then only set the invoice reference on header
                        payment.SetC_Invoice_ID(C_Invoice_ID);
                        payment.SetC_InvoicePaySchedule_ID(invoicePaySchedule_ID);
                    }
                }
                if (!paymentbaseType.Equals("P") && !paymentbaseType.Equals("S"))
                {
                    //for (S)'check' and (P)'oncredit' donot set payment method
                    payment.SetVA009_PaymentMethod_ID(order.GetVA009_PaymentMethod_ID());
                }

                if (!payment.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    string val = "";
                    if (pp == null)
                    {
                        val = pp.GetValue();

                        if (string.IsNullOrEmpty(val))
                        {
                            val = pp.GetName();
                        }
                    }
                    return Msg.GetMsg(GetCtx(), "PaymentNotCreated") + " " + val;
                }
                else
                {
                    if (orderDocBaseType.Equals(MDocType.DOCSUBTYPESO_OnCreditOrder) && invoiceScheduleCount > 1)
                    {
                        //credit order case 
                        ds = DB.ExecuteDataset("SELECT C_InvoicePaySchedule_ID, DueAmt FROM C_InvoicePaySchedule WHERE C_Invoice_ID = " + C_Invoice_ID);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
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

                                if (!alloc.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string val = "";
                                    if (pp != null)
                                    {
                                         val = pp.GetValue();
                                        if (string.IsNullOrEmpty(val))
                                        {
                                            val = pp.GetName();
                                        }
                                    }
                                    log.Severe("Payment Allocate not saved " + val);
                                }
                            }
                        }
                    }
                }
                return payment.GetDocumentNo().ToString();
            }
            catch (Exception ex)
            {
                log.Severe("GeneratePayment Exception:" + ex.Message);
                return Msg.GetMsg(GetCtx(), "Failed") + ":" + ex.Message;
            }
        }
    }
}
