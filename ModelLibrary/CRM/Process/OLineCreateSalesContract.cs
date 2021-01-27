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

/* Process: Create Sales Order Contract by order line
 * Writer :Arpit Singh
 * Date   : 9/2/12 
 */

namespace VAdvantage.Process
{
    class OLineCreateSalesContract : SvrProcess
    {
        int orderLineID;
       

        protected override void Prepare()
        {
            orderLineID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            int VAB_Contract_ID = 0;
            String Sql = "Select VAB_Order_ID from VAB_OrderLine where VAB_OrderLine_ID=" + orderLineID;
            int orderID =Util.GetValueOfInt(DB.ExecuteScalar(Sql,null,null));
            VAdvantage.Model.X_VAB_Order order = new VAdvantage.Model.X_VAB_Order(GetCtx(), orderID, null);
           // string DocStatus = order.GetDocStatus();
            //if (DocStatus != "CO")
            //{
            //    return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            //}


            VAdvantage.Model.X_VAB_OrderLine line = new VAdvantage.Model.X_VAB_OrderLine(GetCtx(), orderLineID, null);

            if (!line.IsProcessed())
            {
                return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            }
                if (line.IsContract() && line.GetVAB_Contract_ID() == 0)
                {

                    VAdvantage.Model.X_VAB_Contract contact = new VAdvantage.Model.X_VAB_Contract(GetCtx(), 0, null);
                    VAdvantage.Model.MProductPricing pp = new VAdvantage.Model.MProductPricing(GetCtx().GetVAF_Client_ID(), GetCtx().GetVAF_Org_ID(),
                        line.GetM_Product_ID(), order.GetVAB_BusinessPartner_ID(), line.GetQtyOrdered(), true);
                    int M_PriceList_ID = Util.GetValueOfInt(order.GetM_PriceList_ID());
                    pp.SetM_PriceList_ID(M_PriceList_ID);

                    string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.VAB_Currency_ID,c.StdPrecision,"
                    + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                    + "FROM M_PriceList pl,VAB_Currency c,M_PriceList_Version plv "
                    + "WHERE pl.VAB_Currency_ID=c.VAB_Currency_ID"
                    + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                    + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                    + "ORDER BY plv.ValidFrom DESC";

                    int M_PriceList_Version_ID = 0;
                    int VAB_Currency_ID = 0;
                    DataSet ds = DB.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr1 = ds.Tables[0].Rows[i];
                        //	Tax Included
                        //  bool isTaxIncluded = Util.GetValueOfBool(ds.Tables[0].Rows[i]["IsTaxIncluded"]);
                        //	Price Limit Enforce
                        //  bool isTaxIncluded = Util.GetValueOfBool(ds.Tables[0].Rows[i]["IsTaxIncluded"]);
                        //	Currency
                        //  int ii = Util.GetValueOfInt(dr[2].ToString());
                        VAB_Currency_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Currency_ID"]);
                        // int prislst = Util.GetValueOfInt(dr[4].ToString());
                        //	PriceList Version
                        M_PriceList_Version_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_PriceList_Version_ID"]);
                    }
                    //int M_PriceList_Version_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
                    pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);


                    contact.SetDescription(order.GetDescription());
                    contact.SetVAB_Order_ID(order.GetVAB_Order_ID());
                    contact.SetVAB_OrderLine_ID(line.GetVAB_OrderLine_ID());
                    contact.SetStartDate(line.GetStartDate());
                    contact.SetEndDate(line.GetEndDate());

                    contact.SetVAB_BusinessPartner_ID(order.GetVAB_BusinessPartner_ID());
                    contact.SetBill_Location_ID(order.GetBill_Location_ID());
                    contact.SetBill_User_ID(order.GetBill_User_ID());
                    contact.SetSalesRep_ID(order.GetSalesRep_ID());

                    contact.SetVAB_Currency_ID(line.GetVAB_Currency_ID());
                    contact.SetVAB_CurrencyType_ID(order.GetVAB_CurrencyType_ID());
                    contact.SetVAB_PaymentTerm_ID(order.GetVAB_PaymentTerm_ID());
                    contact.SetM_PriceList_ID(order.GetM_PriceList_ID());
                    contact.SetVAB_Frequency_ID(line.GetVAB_Frequency_ID());

                    contact.SetPriceList(pp.GetPriceList());
                    contact.SetPriceActual(pp.GetPriceStd());
                    contact.SetPriceEntered(pp.GetPriceStd());
                    contact.SetQtyEntered(line.GetQtyPerCycle());

                    Decimal discount = Decimal.Round(Decimal.Divide(Decimal.Multiply(Decimal.Subtract(pp.GetPriceList(), pp.GetPriceStd()), new Decimal(100)), pp.GetPriceList()), 2);
                    contact.SetDiscount(discount);

                    //contact.SetGrandTotal(
                    // invoice Count Start

                    DateTime SDate = (DateTime)(line.GetStartDate());
                    DateTime Edate = (DateTime)(line.GetEndDate());
                    int frequency = Util.GetValueOfInt(line.GetVAB_Frequency_ID());
                    string PSql = "Select NoOfDays from VAB_Frequency where VAB_Frequency_ID=" + frequency;
                    int days = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, null));
                    int totaldays = (Edate - SDate).Days;
                    int count = 1;
                    if (days > 0)
                    {
                        count = totaldays / days;
                    }

                    contact.SetTotalInvoice(count);

                    //invoice Count end
                    contact.SetVAB_Project_ID(order.GetVAB_Project_ID());
                    // contact.SetPriceList(line.GetPriceList());
                    //contact.SetPriceActual(line.GetPriceActual());
                    contact.SetVAB_UOM_ID(line.GetVAB_UOM_ID());
                    contact.SetM_Product_ID(line.GetM_Product_ID());
                    // contact.SetPriceEntered(line.GetPriceEntered());
                    //contact.SetQtyEntered(line.GetQtyEntered());
                    // contact.SetDiscount(line.GetDiscount());
                    contact.SetVAB_TaxRate_ID(line.GetVAB_TaxRate_ID());
                    contact.SetVAB_Promotion_ID(order.GetVAB_Promotion_ID());

                    sql = "select rate from VAB_TaxRate where VAB_TaxRate_id = " + line.GetVAB_TaxRate_ID();
                    Decimal? rate = Util.GetNullableDecimal(DB.ExecuteScalar(sql, null, null));

                    Decimal? amt = Decimal.Multiply(pp.GetPriceStd(), (Decimal.Divide(rate.Value, 100)));


                    amt = Decimal.Round(amt.Value, 2, MidpointRounding.AwayFromZero);

                    Decimal? taxAmt = Decimal.Multiply(amt.Value, line.GetQtyPerCycle());

                    contact.SetTaxAmt(taxAmt);
                    contact.SetGrandTotal(Decimal.Add(Decimal.Multiply(line.GetQtyPerCycle(), pp.GetPriceStd()), taxAmt.Value));
                    contact.SetLineNetAmt(Decimal.Multiply(line.GetQtyPerCycle(), pp.GetPriceStd()));

                    contact.SetDocStatus("DR");
                    contact.SetRenewContract("N");
                    if (contact.Save())
                    {
                        line.SetVAB_Contract_ID(contact.GetVAB_Contract_ID());
                        if (line.Save())
                        {

                        }

                    }
                    VAB_Contract_ID = contact.GetVAB_Contract_ID();

                }

            

            //order.SetCreateServiceContract("Y");
            //if (!order.Save())
            //{

            //}


            return Msg.GetMsg(GetCtx(), "ServiceContractGenerationDone");
        }


        



    }
}
