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
        VAdvantage.Model.X_C_Contract contact = null;

        protected override void Prepare()
        {
            orderLineID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            //int C_Contract_ID = 0;
            String Sql = "SELECT C_Order_ID FROM C_OrderLine WHERE C_OrderLine_ID=" + orderLineID;
            int orderID = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, Get_TrxName()));
            VAdvantage.Model.X_C_Order order = new VAdvantage.Model.X_C_Order(GetCtx(), orderID, Get_TrxName());
            // string DocStatus = order.GetDocStatus();
            //if (DocStatus != "CO")
            //{
            //    return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            //}


            VAdvantage.Model.X_C_OrderLine line = new VAdvantage.Model.X_C_OrderLine(GetCtx(), orderLineID, Get_TrxName());

            if (!line.IsProcessed())
            {
                return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
            }
            if (line.IsContract() && line.GetC_Contract_ID() == 0)
            {

                contact = new VAdvantage.Model.X_C_Contract(GetCtx(), 0, Get_TrxName());
                //Neha---Commented code because object created but not used in further class---17 Sep,2018

                //VAdvantage.Model.MProductPricing pp = new VAdvantage.Model.MProductPricing(GetCtx().GetAD_Client_ID(), GetCtx().GetAD_Org_ID(),
                //    line.GetM_Product_ID(), order.GetC_BPartner_ID(), line.GetQtyOrdered(), true);
                int M_PriceList_ID = Util.GetValueOfInt(order.GetM_PriceList_ID());
                //pp.SetM_PriceList_ID(M_PriceList_ID);

                string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + " ORDER BY plv.ValidFrom DESC";

                //int M_PriceList_Version_ID = 0;
                int C_Currency_ID = 0;
                DataSet ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr1 = ds.Tables[0].Rows[i];
                    //	Tax Included
                    isTaxIncluded = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsTaxIncluded"]).Equals("Y");

                    //	Currency                    
                    C_Currency_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]);

                    // Std Precision
                    StdPrecision = Util.GetValueOfInt(ds.Tables[0].Rows[i]["StdPrecision"]);
                }
                //int M_PriceList_Version_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);

                //Neha---Set Tenant,Organization from Sales Order---19 Sep,2018
                contact.SetAD_Client_ID(order.GetAD_Client_ID());
                contact.SetAD_Org_ID(order.GetAD_Org_ID());
                //---------------------End ------------------
                contact.SetDescription(order.GetDescription());
                contact.SetC_Order_ID(order.GetC_Order_ID());
                contact.SetC_OrderLine_ID(line.GetC_OrderLine_ID());
                contact.SetStartDate(line.GetStartDate());
                contact.SetEndDate(line.GetEndDate());
                contact.SetC_BPartner_ID(order.GetC_BPartner_ID());
                contact.SetBill_Location_ID(order.GetBill_Location_ID());
                contact.SetBill_User_ID(order.GetBill_User_ID());
                contact.SetSalesRep_ID(order.GetSalesRep_ID());
                contact.SetC_Currency_ID(line.GetC_Currency_ID());
                contact.SetC_ConversionType_ID(order.GetC_ConversionType_ID());
                contact.SetC_PaymentTerm_ID(order.GetC_PaymentTerm_ID());
                contact.SetM_PriceList_ID(order.GetM_PriceList_ID());
                contact.SetC_Frequency_ID(line.GetC_Frequency_ID());
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
                //int frequency = Util.GetValueOfInt(line.GetC_Frequency_ID());
                //string PSql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
                //int days = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, Get_TrxName()));
                //int totaldays = (Edate - SDate).Days;
                //int count = 1;
                //if (days > 0)
                //{
                //    count = totaldays / days;
                //}
                contact.SetTotalInvoice(line.GetNoofCycle());

                //invoice Count end
                contact.SetC_Project_ID(order.GetC_Project_ID());
                // contact.SetPriceList(line.GetPriceList());
                //contact.SetPriceActual(line.GetPriceActual());
                contact.SetC_UOM_ID(line.GetC_UOM_ID());
                contact.SetM_Product_ID(line.GetM_Product_ID());
                // Added by Vivek on 21/11/2017 asigned by Pradeep
                contact.SetM_AttributeSetInstance_ID(line.GetM_AttributeSetInstance_ID());
                // contact.SetPriceEntered(line.GetPriceEntered());
                //contact.SetQtyEntered(line.GetQtyEntered());
                // contact.SetDiscount(line.GetDiscount());
                contact.SetC_Tax_ID(line.GetC_Tax_ID());
                contact.SetC_Campaign_ID(order.GetC_Campaign_ID());

                contact.SetLineNetAmt(Decimal.Multiply(line.GetQtyPerCycle(), line.GetPriceActual()));

                // if Surcharge Tax is selected on Tax, then set value in Surcharge Amount
                MTax tax = MTax.Get(GetCtx(), line.GetC_Tax_ID());
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
                    sql = "SELECT Rate FROM C_Tax WHERE C_Tax_ID = " + line.GetC_Tax_ID();
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
                    String _qry = "UPDATE C_ORDERLINE SET C_CONTRACT_ID=" + contact.GetC_Contract_ID() + " ,CreateServiceContract='Y' WHERE C_ORDERLINE_ID=" + line.GetC_OrderLine_ID();
                    DB.ExecuteScalar(_qry, null, Get_TrxName());
                }
            }
            //Neha---Check Contarct_ID on Order line if CreateServiceContract is false then allow to create Service Contract----18 Sep,2018
            int _count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_ORDERLINE_ID) FROM C_ORDERLINE WHERE CREATESERVICECONTRACT='N' AND IsActive='Y' AND C_ORDER_ID =" + line.GetC_Order_ID(), null, Get_TrxName()));
            if (_count == 0)
            {
                String _qry = "UPDATE C_ORDER SET CreateServiceContract='Y' WHERE C_ORDER_ID=" + line.GetC_Order_ID();
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
