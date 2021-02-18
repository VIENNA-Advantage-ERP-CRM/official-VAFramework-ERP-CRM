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
using VAdvantage.Model;

/* Process: Create Sales Order Contract by order line
 * Writer :Arpit Singh
 * Date   : 9/2/12 
 */

namespace ViennaAdvantageServer.Process
{
    class OLineCreateSalesContract : SvrProcess
    {
        int orderLineID;
        bool isTaxIncluded = false;
        int StdPrecision = 0;
        VAdvantage.Model.X_VAB_Contract contact = null;

        protected override void Prepare()
        {
            orderLineID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            //int VAB_Contract_ID = 0;
            String Sql = "SELECT VAB_Order_ID FROM VAB_OrderLine WHERE VAB_OrderLine_ID=" + orderLineID;
            int orderID = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, Get_TrxName()));
            VAdvantage.Model.X_VAB_Order order = new VAdvantage.Model.X_VAB_Order(GetCtx(), orderID, Get_TrxName());
            // string DocStatus = order.GetDocStatus();
            //if (DocStatus != "CO")
            //{
            //    return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            //}


            VAdvantage.Model.X_VAB_OrderLine line = new VAdvantage.Model.X_VAB_OrderLine(GetCtx(), orderLineID, Get_TrxName());

            if (!line.IsProcessed())
            {
                return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            }
            if (line.IsContract() && line.GetVAB_Contract_ID() == 0)
            {

                contact = new VAdvantage.Model.X_VAB_Contract(GetCtx(), 0, Get_TrxName());
                //Neha---Commented code because object created but not used in further class---17 Sep,2018

                //VAdvantage.Model.MProductPricing pp = new VAdvantage.Model.MProductPricing(GetCtx().GetVAF_Client_ID(), GetCtx().GetVAF_Org_ID(),
                //    line.GetVAM_Product_ID(), order.GetVAB_BusinessPartner_ID(), line.GetQtyOrdered(), true);
                int VAM_PriceList_ID = Util.GetValueOfInt(order.GetVAM_PriceList_ID());
                //pp.SetVAM_PriceList_ID(VAM_PriceList_ID);

                string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.VAB_Currency_ID,c.StdPrecision,"
                + "plv.VAM_PriceListVersion_ID,plv.ValidFrom "
                + "FROM VAM_PriceList pl,VAB_Currency c,VAM_PriceListVersion plv "
                + "WHERE pl.VAB_Currency_ID=c.VAB_Currency_ID"
                + " AND pl.VAM_PriceList_ID=plv.VAM_PriceList_ID"
                + " AND pl.VAM_PriceList_ID=" + VAM_PriceList_ID						//	1
                + " ORDER BY plv.ValidFrom DESC";

                //int VAM_PriceListVersion_ID = 0;
                int VAB_Currency_ID = 0;
                DataSet ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr1 = ds.Tables[0].Rows[i];
                    //	Tax Included
                    isTaxIncluded = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsTaxIncluded"]).Equals("Y");

                    //	Currency                    
                    VAB_Currency_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Currency_ID"]);

                    // Std Precision
                    StdPrecision = Util.GetValueOfInt(ds.Tables[0].Rows[i]["StdPrecision"]);
                }
                //int VAM_PriceListVersion_ID = GetCtx().GetContextAsInt(WindowNo, "VAM_PriceListVersion_ID");
                //pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);

                //Neha---Set Tenant,Organization from Sales Order---19 Sep,2018
                contact.SetVAF_Client_ID(order.GetVAF_Client_ID());
                contact.SetVAF_Org_ID(order.GetVAF_Org_ID());
                //---------------------End ------------------
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
                //contact.SetPriceList(pp.GetPriceList());
                //contact.SetPriceActual(pp.GetPriceStd());
                //contact.SetPriceEntered(pp.GetPriceStd());
                contact.SetQtyEntered(line.GetQtyPerCycle());

                //Neha--Set List Price,Price,Unit Price,Discount,Total Invoice from Order Line--17 Sep,2018
                contact.SetPriceList(line.GetPriceList());
                contact.SetPriceActual(line.GetPriceActual());
                contact.SetPriceEntered(line.GetPriceEntered());
                //Decimal discount = Decimal.Round(Decimal.Divide(Decimal.Multiply(Decimal.Subtract(pp.GetPriceList(), pp.GetPriceStd()), new Decimal(100)), pp.GetPriceList()), 2);
                //contact.SetDiscount(discount);
                contact.SetDiscount(line.GetDiscount());
                //contact.SetGrandTotal(
                // invoice Count Start
                //DateTime SDate = (DateTime)(line.GetStartDate());
                //DateTime Edate = (DateTime)(line.GetEndDate());
                //int frequency = Util.GetValueOfInt(line.GetVAB_Frequency_ID());
                //string PSql = "Select NoOfDays from VAB_Frequency where VAB_Frequency_ID=" + frequency;
                //int days = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, Get_TrxName()));
                //int totaldays = (Edate - SDate).Days;
                //int count = 1;
                //if (days > 0)
                //{
                //    count = totaldays / days;
                //}
                contact.SetTotalInvoice(line.GetNoofCycle());

                //invoice Count end
                contact.SetVAB_Project_ID(order.GetVAB_Project_ID());
                // contact.SetPriceList(line.GetPriceList());
                //contact.SetPriceActual(line.GetPriceActual());
                contact.SetVAB_UOM_ID(line.GetVAB_UOM_ID());
                contact.SetVAM_Product_ID(line.GetVAM_Product_ID());
                // Added by Vivek on 21/11/2017 asigned by Pradeep
                contact.SetVAM_PFeature_SetInstance_ID(line.GetVAM_PFeature_SetInstance_ID());
                // contact.SetPriceEntered(line.GetPriceEntered());
                //contact.SetQtyEntered(line.GetQtyEntered());
                // contact.SetDiscount(line.GetDiscount());
                contact.SetVAB_TaxRate_ID(line.GetVAB_TaxRate_ID());
                contact.SetVAB_Promotion_ID(order.GetVAB_Promotion_ID());

                contact.SetLineNetAmt(Decimal.Multiply(line.GetQtyPerCycle(), line.GetPriceActual()));

                // if Surcharge Tax is selected on Tax, then set value in Surcharge Amount
                MVABTaxRate tax = MVABTaxRate.Get(GetCtx(), line.GetVAB_TaxRate_ID());
                if (contact.Get_ColumnIndex("SurchargeAmt") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                {
                    Decimal surchargeAmt = Env.ZERO;

                    // Calculate Surcharge Amount
                    Decimal TotalRate = tax.CalculateSurcharge(contact.GetLineNetAmt(), isTaxIncluded, StdPrecision, out surchargeAmt);
                    contact.SetTaxAmt(TotalRate);
                    contact.SetSurchargeAmt(surchargeAmt);
                }
                else
                {
                    //Neha---Calculate TaxAmt,GrandTotal,Line Amount,Bill Start Date on the basis of Actual Price(Sales Order Line)--17 Sep,2018
                    sql = "SELECT Rate FROM VAB_TaxRate WHERE VAB_TaxRate_ID = " + line.GetVAB_TaxRate_ID();
                    Decimal? rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
                    Decimal? amt = Decimal.Multiply(line.GetPriceActual(), (Decimal.Divide(rate.Value, 100)));
                    amt = Decimal.Round(amt.Value, 2, MidpointRounding.AwayFromZero);
                    Decimal? taxAmt = Decimal.Multiply(amt.Value, line.GetQtyPerCycle());
                    contact.SetTaxAmt(taxAmt);
                }

                // Set Grand Total Amount
                if (isTaxIncluded)
                {
                    contact.SetGrandTotal(contact.GetLineNetAmt());
                }
                else
                {
                    if (contact.Get_ColumnIndex("SurchargeAmt") > 0)
                    {
                        contact.SetGrandTotal(Decimal.Add(Decimal.Add(contact.GetLineNetAmt(), contact.GetTaxAmt()), contact.GetSurchargeAmt()));
                    }
                    else
                    {
                        contact.SetGrandTotal(Decimal.Add(contact.GetLineNetAmt(), contact.GetTaxAmt()));
                    }
                }

                contact.SetBillStartDate(line.GetStartDate());

                contact.SetDocStatus("DR");
                contact.SetRenewContract("N");
                if (!contact.Save())
                {
                    //Neha----If Service Contract not saved then will show the exception---17 Sep,2018
                    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                    if (pp != null)
                        throw new ArgumentException("Cannot save Service Contract. " + pp.GetName());
                    throw new ArgumentException("Cannot save Service Contract");
                }
                else
                {
                    if (!line.Save())
                    {
                        //Neha----If Order Line not saved then will show the exception---17 Sep,2018
                        ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                        if (pp != null)
                            throw new ArgumentException("Cannot save Order Line. " + pp.GetName());
                        throw new ArgumentException("Cannot save Order Line");
                    }
                    //Neha---Set CreateServiceContract,Order Line ID on Order Line tab---17 Sep,2018
                    String _qry = "UPDATE VAB_ORDERLINE SET VAB_Contract_ID=" + contact.GetVAB_Contract_ID() + " ,CreateServiceContract='Y' WHERE VAB_ORDERLINE_ID=" + line.GetVAB_OrderLine_ID();
                    DB.ExecuteScalar(_qry, null, Get_TrxName());
                }
            }
            //Neha---Check Contarct_ID on Order line if CreateServiceContract is false then allow to create Service Contract----18 Sep,2018
            int _count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAB_ORDERLINE_ID) FROM VAB_ORDERLINE WHERE CREATESERVICECONTRACT='N' AND IsActive='Y' AND VAB_ORDER_ID =" + line.GetVAB_Order_ID(), null, Get_TrxName()));
            if (_count == 0)
            {
                String _qry = "UPDATE VAB_ORDER SET CreateServiceContract='Y' WHERE VAB_ORDER_ID=" + line.GetVAB_Order_ID();
                DB.ExecuteScalar(_qry, null, Get_TrxName());
            }
            //order.SetCreateServiceContract("Y");
            //if (!order.Save())
            //{

            //}
            return Msg.GetMsg(GetCtx(), "ServiceContractGenerationDone") + contact.GetDocumentNo();
        }

    }
}
