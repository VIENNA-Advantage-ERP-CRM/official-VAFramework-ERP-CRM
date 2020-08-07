using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;

/* Process: Renew Contract 
 * Writer :Arpit Singh
 * Date   : 02/02/2012 
 */

namespace VAdvantage.Process
{
    class RenewContract : SvrProcess
    {
        DateTime today;

        protected override void Prepare()
        {

            today = DateTime.Today;

        }

        protected override String DoIt()
        {
            string Sql, newcon = "";

            String date = string.Format("{0:dd/MM/yy}", today);
            int Record_id = GetRecord_ID();
            VAdvantage.Model.X_C_Contract con = new VAdvantage.Model.X_C_Contract(GetCtx(), Record_id, null);
            if (Record_id != 0)
            {
                // Sql = "Select C_Contract_id From C_Contract where to_char(EndDate,'dd/mm/yy')='" + date + "' and C_Contract_id=" + Record_id + " and RenewContract = 'N'";
                //Sql = "Select C_Contract_id From C_Contract where to_char(EndDate,'dd/mm/yy')='" + date + "' and C_Contract_id=" + Record_id;
                Sql = "select RenewalType from c_contract where C_Contract_id=" + Record_id + " and RenewContract = 'N' and isactive = 'Y' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                string renewType = Util.GetValueOfString(DB.ExecuteScalar(Sql));
                if (renewType == "M")
                {
                    Sql = "select C_Contract_id from c_contract where C_Contract_id=" + Record_id + " and RenewContract = 'N' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                }
                else
                {
                    Sql = "select C_Contract_id from c_contract where (enddate- nvl(cancelbeforedays,0)) <= sysdate and C_Contract_id=" + Record_id + " and RenewContract = 'N' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                }
            }
            else
            {
                //Sql = "Select C_Contract_id From C_Contract where to_char(EndDate,'dd/mm/yy')='" + date + "' and RenewContract = 'N'";
                Sql = "select C_Contract_id from c_contract where (enddate- nvl(cancelbeforedays,0)) <= sysdate and RenewalType='A' and RenewContract = 'N' and ad_client_id = " + GetCtx().GetAD_Client_ID();
            }

            IDataReader dr = DB.ExecuteReader(Sql);
            int COUNT = 0;

            while (dr.Read())
            {
                VAdvantage.Model.X_C_Contract contact = new VAdvantage.Model.X_C_Contract(GetCtx(), Util.GetValueOfInt(dr[0]), null);

                string type = Util.GetValueOfString(contact.GetRenewalType());
                if (type == "M")
                {
                    DateTime? CDate = Util.GetValueOfDateTime(contact.GetCancellationDate());

                    int cycles = Util.GetValueOfInt(contact.GetCycles());

                    string RType = contact.GetRenewalType();
                    if (CDate != null)
                    {
                        continue;
                    }
                    VAdvantage.Model.X_C_Contract New = new VAdvantage.Model.X_C_Contract(GetCtx(), 0, null);
                    New.SetRefContract(contact.GetDocumentNo());
                    New.SetC_Order_ID(contact.GetC_Order_ID());
                    New.SetC_OrderLine_ID(contact.GetC_OrderLine_ID());
                    DateTime OldStart = (DateTime)(contact.GetStartDate());
                    DateTime Start = (DateTime)(contact.GetEndDate());
                    int duration = (Start - OldStart).Days;
                    DateTime End = Start.AddDays(duration);
                    New.SetStartDate(Start.AddDays(1));
                    //  New.SetEndDate(End.AddDays(1));
                    New.SetC_BPartner_ID(contact.GetC_BPartner_ID());
                    New.SetBill_Location_ID(contact.GetBill_Location_ID());
                    New.SetBill_User_ID(contact.GetBill_User_ID());
                    New.SetSalesRep_ID(contact.GetSalesRep_ID());
                    New.SetC_Currency_ID(contact.GetC_Currency_ID());
                    New.SetC_ConversionType_ID(contact.GetC_ConversionType_ID());
                    New.SetC_PaymentTerm_ID(contact.GetC_PaymentTerm_ID());

                    New.SetC_Frequency_ID(contact.GetC_Frequency_ID());
                    // invoice Count Start
                    //DateTime SDate = Start;
                    //DateTime Edate = End;
                    int frequency = Util.GetValueOfInt(contact.GetC_Frequency_ID());
                    string PSql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
                    int months = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, null));
                    int dur = months * cycles;
                    DateTime endDate = Start.AddMonths(dur);
                    endDate = endDate.AddDays(-1);
                    New.SetEndDate(endDate);
                    //int totaldays = (Edate - SDate).Days;
                    //int count = totaldays / days;
                    New.SetTotalInvoice(cycles);
                    //invoice Count end
                    if (Record_id != 0)
                    {
                        New.SetM_PriceList_ID(contact.GetRef_PriceList_ID());
                        String Query = "Select M_PriceList_Version_id from M_PriceList_Version where IsActive='Y' and M_PriceList_Id=" + contact.GetRef_PriceList_ID();
                        int Version = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                        Query = "Select PriceList from M_ProductPrice where M_PriceList_Version_id=" + Version + " and M_Product_ID=" + contact.GetM_Product_ID();
                        decimal Listprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                        Query = "Select PriceStd from M_ProductPrice where M_PriceList_Version_id=" + Version + " and M_Product_ID=" + contact.GetM_Product_ID();
                        decimal Stdprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                        if (Stdprice == 0 && Listprice == 0)
                        {
                            dr.Close();
                            return Msg.GetMsg(GetCtx(), "ProductNotINPriceList");
                        }


                        New.SetPriceEntered(Stdprice);
                        New.SetPriceActual(Stdprice);
                        New.SetPriceList(Listprice);


                    }
                    else
                    {

                        New.SetM_PriceList_ID(contact.GetM_PriceList_ID());
                        New.SetPriceEntered(contact.GetPriceEntered());
                        New.SetPriceActual(contact.GetPriceActual());
                        New.SetPriceList(contact.GetPriceList());
                    }
                    New.SetC_UOM_ID(contact.GetC_UOM_ID());
                    New.SetM_Product_ID(contact.GetM_Product_ID());
                    // New.SetLineNetAmt(contact.GetLineNetAmt());

                    New.SetQtyEntered(contact.GetQtyEntered());
                    // New.SetDiscount(contact.GetDiscount());
                    New.SetC_Tax_ID(contact.GetC_Tax_ID());
                    New.SetC_Campaign_ID(contact.GetC_Campaign_ID());
                    New.SetRef_Contract_ID(contact.GetC_Contract_ID());
                    New.SetC_Project_ID(contact.GetC_Project_ID());
                    New.SetDescription(contact.GetDescription());
                    //  New.SetLineNetAmt(contact.GetLineNetAmt());
                    //  New.SetGrandTotal(contact.GetGrandTotal());
                    New.SetTaxAmt(contact.GetTaxAmt());
                    New.SetCancelBeforeDays(contact.GetCancelBeforeDays());
                    New.SetCycles(contact.GetCycles());
                    New.SetRenewContract("N");
                    New.SetScheduleContract("N");
                    New.SetDocStatus("DR");
                    New.SetRenewalType("M");

                    String sqltax = "select rate from c_tax WHERE c_tax_id=" + contact.GetC_Tax_ID() + "";
                    Decimal? Rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sqltax, null, null));


                    New.SetLineNetAmt(Decimal.Multiply(New.GetPriceEntered(), New.GetQtyEntered()));

                    Decimal? TotalRate = Util.GetValueOfDecimal((Util.GetValueOfDecimal(New.GetLineNetAmt()) * Util.GetValueOfDecimal(Rate)) / 100);

                    TotalRate = Decimal.Round(TotalRate.Value, 2);

                    Decimal? dis = Decimal.Multiply(Decimal.Divide(Decimal.Subtract(New.GetPriceList(), New.GetPriceEntered()), New.GetPriceList()), 100);
                    //Decimal? Discount = Util.GetValueOfDecimal(((Decimal.ToDouble(PriceList.Value) - Decimal.ToDouble(PriceActual.Value)) / Decimal.ToDouble(PriceList.Value) * 100.0));

                    New.SetDiscount(dis);
                    New.SetTaxAmt(TotalRate);
                    New.SetGrandTotal(Decimal.Add(New.GetLineNetAmt(), New.GetTaxAmt()));

                    if (New.Save())
                    {
                        newcon = New.GetDocumentNo();
                        ++COUNT;
                        if (Record_id != 0)
                        {
                            contact.SetRef_Contract_ID(New.GetC_Contract_ID());
                            contact.SetRenewContract("Y");
                            if (contact.Save())
                            {
                            }
                        }
                    }
                }
                else
                {
                    DateTime? CDate = Util.GetValueOfDateTime(contact.GetCancellationDate());

                    int cycles = Util.GetValueOfInt(contact.GetCycles());

                    string RType = contact.GetRenewalType();
                    if (CDate != null)
                    {
                        continue;
                    }
                    VAdvantage.Model.X_C_Contract New = new VAdvantage.Model.X_C_Contract(GetCtx(), 0, null);
                    New.SetRefContract(contact.GetDocumentNo());
                    New.SetC_Order_ID(contact.GetC_Order_ID());
                    New.SetC_OrderLine_ID(contact.GetC_OrderLine_ID());
                    DateTime OldStart = (DateTime)(contact.GetStartDate());
                    DateTime Start = (DateTime)(contact.GetEndDate());
                    int duration = (Start - OldStart).Days;
                    DateTime End = Start.AddDays(duration);
                    New.SetStartDate(Start.AddDays(1));

                    int frequency = Util.GetValueOfInt(contact.GetC_Frequency_ID());
                    string PSql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
                    int months = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, null));
                    int total = months * cycles;

                    DateTime? endDate = New.GetStartDate().Value.AddMonths(total);
                    endDate = endDate.Value.AddDays(-1);
                    //int totaldays = (Edate - SDate).Days;


                    New.SetEndDate(endDate);
                    New.SetC_BPartner_ID(contact.GetC_BPartner_ID());
                    New.SetBill_Location_ID(contact.GetBill_Location_ID());
                    New.SetBill_User_ID(contact.GetBill_User_ID());
                    New.SetSalesRep_ID(contact.GetSalesRep_ID());
                    New.SetC_Currency_ID(contact.GetC_Currency_ID());
                    New.SetC_ConversionType_ID(contact.GetC_ConversionType_ID());
                    New.SetC_PaymentTerm_ID(contact.GetC_PaymentTerm_ID());

                    New.SetC_Frequency_ID(contact.GetC_Frequency_ID());
                    // invoice Count Start

                    DateTime SDate = Start;
                    DateTime Edate = End;
                    //int frequency = Util.GetValueOfInt(contact.GetC_Frequency_ID());
                    //string PSql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
                    //int days = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, null));
                    //int totaldays = (Edate - SDate).Days;
                    //int count = totaldays / days;

                    if (Record_id != 0)
                    {
                        if (contact.GetRef_PriceList_ID() == 0)
                        {
                            return Msg.GetMsg(GetCtx(), "FirstSelectPriceList");
                        }
                        New.SetM_PriceList_ID(contact.GetRef_PriceList_ID());
                        String Query = "Select M_PriceList_Version_id from M_PriceList_Version where IsActive='Y' and M_PriceList_Id=" + contact.GetRef_PriceList_ID();
                        int Version = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                        Query = "Select PriceList,PriceStd from M_ProductPrice where M_PriceList_Version_id=" + Version + " and M_Product_ID=" + contact.GetM_Product_ID();
                        decimal Listprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                        Query = "Select PriceList,PriceStd from M_ProductPrice where M_PriceList_Version_id=" + Version + " and M_Product_ID=" + contact.GetM_Product_ID();
                        decimal Stdprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                        if (Stdprice == 0 && Listprice == 0)
                        {
                            dr.Close();
                            return Msg.GetMsg(GetCtx(), "ProductNotINPriceList");
                        }


                        New.SetPriceEntered(Stdprice);
                        New.SetPriceActual(Stdprice);
                        New.SetPriceList(Listprice);



                    }
                    else
                    {
                        New.SetPriceActual(contact.GetPriceActual());
                        New.SetPriceList(contact.GetPriceList());
                        New.SetPriceEntered(contact.GetPriceEntered());
                        New.SetM_PriceList_ID(contact.GetM_PriceList_ID());
                    }
                    New.SetTotalInvoice(contact.GetCycles());
                    //invoice Count end

                    New.SetC_UOM_ID(contact.GetC_UOM_ID());
                    New.SetM_Product_ID(contact.GetM_Product_ID());
                    // New.SetLineNetAmt(contact.GetLineNetAmt());

                    New.SetQtyEntered(contact.GetQtyEntered());
                    New.SetDiscount(contact.GetDiscount());
                    New.SetC_Tax_ID(contact.GetC_Tax_ID());
                    New.SetC_Campaign_ID(contact.GetC_Campaign_ID());
                    New.SetRef_Contract_ID(contact.GetC_Contract_ID());
                    New.SetC_Project_ID(contact.GetC_Project_ID());
                    New.SetDescription(contact.GetDescription());
                    New.SetLineNetAmt(contact.GetLineNetAmt());
                    New.SetGrandTotal(contact.GetGrandTotal());
                    New.SetTaxAmt(contact.GetTaxAmt());
                    New.SetCancelBeforeDays(contact.GetCancelBeforeDays());
                    New.SetCycles(contact.GetCycles());
                    New.SetRenewContract("N");
                    New.SetScheduleContract("Y");
                    New.SetRenewalType("A");
                    New.SetDocStatus("DR");
                    if (contact.GetBillStartDate() != null)
                    {
                        New.SetBillStartDate(contact.GetBillStartDate().Value.AddMonths(contact.GetTotalInvoice()));
                    }
                    else
                    {
                        New.SetBillStartDate(New.GetStartDate());
                    }

                    if (New.Save())
                    {
                        newcon = New.GetDocumentNo();
                        ++COUNT;
                        //if (Record_id != 0)
                        //{
                        contact.SetRef_Contract_ID(New.GetC_Contract_ID());
                        contact.SetRenewContract("Y");
                        if (contact.Save())
                        {
                        }
                        //}
                        EnterSchedules(New.GetC_Contract_ID(), cycles);
                        New.SetProcessed(true);
                        New.Save();
                    }
                }
            }
            dr.Close();
            // X_C_Contract contract = new X_C_Contract(GetCtx(),id, null);
            if (COUNT != 0 && Record_id != 0)
            {
                return Msg.GetMsg(GetCtx(), "ContractReNewed :" + newcon);
            }
            if (COUNT != 0)
            {
                return Msg.GetMsg(GetCtx(), "ContractReNewed");
            }
            return Msg.GetMsg(GetCtx(), "NoContractReNewed");
        }

        private void EnterSchedules(int C_Contract_ID, int cycles)
        {
            VAdvantage.Model.X_C_Contract contract = new VAdvantage.Model.X_C_Contract(GetCtx(), C_Contract_ID, null);
            //DateTime start = (DateTime)contract.GetStartDate();
            DateTime start = (DateTime)contract.GetStartDate();
            //DateTime end = (DateTime)contract.GetEndDate();
            int frequency = contract.GetC_Frequency_ID();

            string Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
            int months = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            int totalcount = months * cycles;
            DateTime end = start.AddMonths(totalcount);
            //int totaldays = (end - start).Days;
            //int count = totaldays / days;

            if (cycles > 0)
            {
                // decimal Units = Math.Round((contract.GetQtyEntered() / count), 2);

                for (int i = 1; i <= cycles; i++)
                {

                    VAdvantage.Model.X_C_ContractSchedule CSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), 0, null);
                    CSchedule.SetC_Contract_ID(C_Contract_ID);
                    CSchedule.SetC_BPartner_ID(contract.GetC_BPartner_ID());

                    CSchedule.SetFROMDATE(start);

                    CSchedule.SetUnitsDelivered(contract.GetQtyEntered());
                    if (i != cycles)
                    {
                        CSchedule.SetEndDate(start.AddMonths(months).AddDays(-1));
                        start = start.AddMonths(months);
                        //start = start.AddDays(days);
                        // CSchedule.SetNoOfDays(days);
                    }
                    else
                    {
                        CSchedule.SetEndDate(end);
                        // CSchedule.SetNoOfDays((end - start).Days+1);
                    }
                    CSchedule.SetM_Product_ID(contract.GetM_Product_ID());
                    CSchedule.SetM_Product_ID(contract.GetM_Product_ID());
                    CSchedule.SetTotalAmt(contract.GetLineNetAmt());
                    CSchedule.SetGrandTotal(contract.GetGrandTotal());
                    CSchedule.SetTaxAmt(contract.GetTaxAmt());
                    CSchedule.SetC_UOM_ID(contract.GetC_UOM_ID());
                    CSchedule.SetPriceEntered(contract.GetPriceEntered());
                    CSchedule.SetProcessed(true);
                    if (CSchedule.Save())
                    {
                        //contract.SetProcessed(true);
                        //contract.SetScheduleContract("Y");
                        //contract.Save();
                    }

                }
            }
        }
    }
}
