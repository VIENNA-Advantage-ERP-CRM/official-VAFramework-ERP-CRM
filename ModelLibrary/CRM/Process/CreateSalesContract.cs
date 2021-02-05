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

/* Process: Create Sales Order Contract 
 * Writer :Arpit Singh
 * Date   : 31/1/12 
 */

namespace VAdvantage.Process
{
    class CreateSalesContract : SvrProcess
    {
        int orderID;
        IDataReader dr;

        protected override void Prepare()
        {
            orderID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            int VAB_Contract_ID = 0;
            String Sql = "Select VAB_OrderLine_ID from VAB_OrderLine where VAB_Order_ID=" + orderID;
            dr = DB.ExecuteReader(Sql);
            VAdvantage.Model.X_VAB_Order order = new VAdvantage.Model.X_VAB_Order(GetCtx(), orderID, null);
            string DocStatus = order.GetDocStatus();
            if (DocStatus != "CO")
            {
                return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            }

            while (dr.Read())
            {
                VAdvantage.Model.X_VAB_OrderLine line = new VAdvantage.Model.X_VAB_OrderLine(GetCtx(), Util.GetValueOfInt(dr[0]), null);
                if (line.IsContract() && line.GetVAB_Contract_ID()==0)
                {

                    VAdvantage.Model.X_VAB_Contract contact = new VAdvantage.Model.X_VAB_Contract(GetCtx(), 0, null);
                    VAdvantage.Model.MProductPricing pp = new VAdvantage.Model.MProductPricing(GetCtx().GetVAF_Client_ID(), GetCtx().GetVAF_Org_ID(),
                        line.GetVAM_Product_ID(), order.GetVAB_BusinessPartner_ID(), line.GetQtyOrdered(), true);
                    int VAM_PriceList_ID = Util.GetValueOfInt(order.GetVAM_PriceList_ID());
                    pp.SetVAM_PriceList_ID(VAM_PriceList_ID);

                    string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.VAB_Currency_ID,c.StdPrecision,"
                    + "plv.VAM_PriceListVersion_ID,plv.ValidFrom "
                    + "FROM VAM_PriceList pl,VAB_Currency c,VAM_PriceListVersion plv "
                    + "WHERE pl.VAB_Currency_ID=c.VAB_Currency_ID"
                    + " AND pl.VAM_PriceList_ID=plv.VAM_PriceList_ID"
                    + " AND pl.VAM_PriceList_ID=" + VAM_PriceList_ID						//	1
                    + "ORDER BY plv.ValidFrom DESC";

                    int VAM_PriceListVersion_ID = 0;
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
                        VAM_PriceListVersion_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PriceListVersion_ID"]);
                    }
                    //int VAM_PriceListVersion_ID = GetCtx().GetContextAsInt(WindowNo, "VAM_PriceListVersion_ID");
                    pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);


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
                    contact.SetVAM_PriceList_ID(order.GetVAM_PriceList_ID());
                    contact.SetVAB_Frequency_ID(line.GetVAB_Frequency_ID());

                    contact.SetPriceList(pp.GetPriceList());
                    contact.SetPriceActual(pp.GetPriceStd());
                    contact.SetPriceEntered(pp.GetPriceStd());
                    contact.SetQtyEntered(line.GetQtyPerCycle());

                    Decimal discount = Decimal.Round(Decimal.Divide(Decimal.Multiply(Decimal.Subtract(pp.GetPriceList(), pp.GetPriceStd()), new Decimal(100)), pp.GetPriceList()), 2);
                    contact.SetDiscount(discount);
                    //contact.SetGrandTotal(
                    // invoice Count Start

                    //DateTime SDate = (DateTime)(line.GetStartDate());
                    //DateTime Edate = (DateTime)(line.GetEndDate());
                    //int frequency = Util.GetValueOfInt(line.GetVAB_Frequency_ID());
                    //string PSql = "Select NoOfDays from VAB_Frequency where VAB_Frequency_ID=" + frequency;
                    //int days = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, null));
                    //int totaldays = (Edate - SDate).Days;
                    //int count = totaldays / days;

                    contact.SetTotalInvoice(line.GetNoofCycle());

                    //invoice Count end
                    contact.SetVAB_Project_ID(order.GetVAB_Project_ID());
                    // contact.SetPriceList(line.GetPriceList());
                    //contact.SetPriceActual(line.GetPriceActual());
                    contact.SetVAB_UOM_ID(line.GetVAB_UOM_ID());
                    contact.SetVAM_Product_ID(line.GetVAM_Product_ID());
                    // contact.SetPriceEntered(line.GetPriceEntered());
                    //contact.SetQtyEntered(line.GetQtyEntered());
                    // contact.SetDiscount(line.GetDiscount());
                    contact.SetVAB_TaxRate_ID(line.GetVAB_TaxRate_ID());
                    contact.SetVAB_Promotion_ID(order.GetVAB_Promotion_ID());

                    sql = "select rate from VAB_TaxRate where VAB_TaxRate_id = " + line.GetVAB_TaxRate_ID();
                    Decimal? rate = Util.GetNullableDecimal(DB.ExecuteScalar(sql, null, null));

                    Decimal? amt = Decimal.Multiply(pp.GetPriceStd(),(Decimal.Divide(rate.Value, 100)));


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

            }
            dr.Close();

            order.SetCreateServiceContract("Y");
            if (!order.Save())
            {

            }
            

            return Msg.GetMsg(GetCtx(), "ServiceContractGenerationDone");
        }


        /*****
          int VAB_BusinessPartner_ID = ctx.GetContextAsInt(WindowNo, "VAB_BusinessPartner_ID");
                Decimal Qty = Util.GetValueOfDecimal(mTab.GetValue("QtyOrdered"));
                bool isSOTrx = ctx.GetContext(WindowNo, "IsSOTrx").Equals("Y");
                MProductPricing pp = new MProductPricing(ctx.GetVAF_Client_ID(), ctx.GetVAF_Org_ID(),
                        VAM_Product_ID, VAB_BusinessPartner_ID, Qty, isSOTrx);
                int VAM_PriceList_ID = ctx.GetContextAsInt(WindowNo, "VAM_PriceList_ID");
                pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
                /** PLV is only accurate if PL selected in header
                int VAM_PriceListVersion_ID = ctx.GetContextAsInt(WindowNo, "VAM_PriceListVersion_ID");
                pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);
                DateTime orderDate = System.Convert.ToDateTime(mTab.GetValue("DateOrdered"));
                pp.SetPriceDate(orderDate);
                //		

                if (!Util.GetValueOfBool(mTab.GetValue("IsContract")))
                {
                    mTab.SetValue("PriceList", pp.GetPriceList());
                    mTab.SetValue("PriceLimit", pp.GetPriceLimit());
                    mTab.SetValue("PriceActual", pp.GetPriceStd());
                    mTab.SetValue("PriceEntered", pp.GetPriceStd());
                }
                else
                {
                    mTab.SetValue("PriceList", 0);
                    mTab.SetValue("PriceLimit", 0);
                    mTab.SetValue("PriceActual", 0);
                    mTab.SetValue("PriceEntered", 0);
                }
                mTab.SetValue("VAB_Currency_ID", System.Convert.ToInt32(pp.GetVAB_Currency_ID()));
                mTab.SetValue("Discount", pp.GetDiscount());
                mTab.SetValue("VAB_UOM_ID", System.Convert.ToInt32(pp.GetVAB_UOM_ID()));
                mTab.SetValue("QtyOrdered", mTab.GetValue("QtyEntered"));
                ctx.SetContext(WindowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
                ctx.SetContext(WindowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");
        **********************/



    }
}
