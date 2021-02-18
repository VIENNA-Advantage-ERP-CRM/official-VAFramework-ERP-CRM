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

/* Process: Create Sales Order Contract 
 * Writer :Arpit Singh
 * Date   : 31/1/12 
 */

namespace ViennaAdvantageServer.Process
{
    class CreateSalesContract : SvrProcess
    {
        int orderID;
        bool isTaxIncluded = false;
        int StdPrecision = 0;
        IDataReader dr;
        VAdvantage.Model.X_VAB_Contract contact = null;
        VAdvantage.Model.X_VAB_OrderLine line = null;
        string DocumntNo;

        protected override void Prepare()
        {
            orderID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            //int VAB_Contract_ID = 0;
            String Sql = "SELECT VAB_OrderLine_ID FROM VAB_OrderLine WHERE VAB_Order_ID=" + orderID + " AND CreateServiceContract='N' AND IsContract='Y' AND IsActive='Y'";
            dr = DB.ExecuteReader(Sql);
            try
            {
                VAdvantage.Model.X_VAB_Order order = new VAdvantage.Model.X_VAB_Order(GetCtx(), orderID, Get_TrxName());
                string DocStatus = order.GetDocStatus();
                if (DocStatus != "CO")
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }

                    return Msg.GetMsg(GetCtx(), "FirstCompleteOrder");
                }


                while (dr.Read())
                {
                    line = new VAdvantage.Model.X_VAB_OrderLine(GetCtx(), Util.GetValueOfInt(dr[0]), Get_TrxName());
                    if (line.IsContract() && line.GetVAB_Contract_ID() == 0)
                    {

                        contact = new VAdvantage.Model.X_VAB_Contract(GetCtx(), 0, Get_TrxName());
                        int VAM_PriceList_ID = Util.GetValueOfInt(order.GetVAM_PriceList_ID());

                        //Neha---Commented code because object created but not used in further class---04 Sep,2018

                        //VAdvantage.Model.MProductPricing pp = new VAdvantage.Model.MProductPricing(GetCtx().GetVAF_Client_ID(), GetCtx().GetVAF_Org_ID(),
                        //    line.GetVAM_Product_ID(), order.GetVAB_BusinessPartner_ID(), line.GetQtyOrdered(), true);                        
                        //pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
                        //VAdvantage.Model.MProduct prd = new VAdvantage.Model.MProduct(GetCtx(), line.GetVAM_Product_ID(), null);
                        //pp.SetVAB_UOM_ID(prd.GetVAB_UOM_ID());

                        string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.VAB_Currency_ID,c.StdPrecision,"
                        + "plv.VAM_PriceListVersion_ID,plv.ValidFrom "
                        + "FROM VAM_PriceList pl,VAB_Currency c,VAM_PriceListVersion plv "
                        + "WHERE pl.VAB_Currency_ID=c.VAB_Currency_ID"
                        + " AND pl.VAM_PriceList_ID=plv.VAM_PriceList_ID"
                        + " AND pl.VAM_PriceList_ID=" + VAM_PriceList_ID						//	1
                        + "ORDER BY plv.ValidFrom DESC";

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
                            // int prislst = Util.GetValueOfInt(dr[4].ToString());
                            //	PriceList Version
                            //VAM_PriceListVersion_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PriceListVersion_ID"]);
                        }
                        //int VAM_PriceListVersion_ID = GetCtx().GetContextAsInt(WindowNo, "VAM_PriceListVersion_ID");
                        //pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);
                        //Neha---Set Tenant,Organization from Sales Order---11 Sep,2018
                        contact.SetVAF_Client_ID(order.GetVAF_Client_ID());
                        contact.SetVAF_Org_ID(order.GetVAF_Org_ID());
                        //---------------------End ------------------
                        contact.SetDescription(order.GetDescription());
                        contact.SetVAB_Order_ID(order.GetVAB_Order_ID());
                        contact.SetVAB_OrderLine_ID(line.GetVAB_OrderLine_ID());
                        contact.SetStartDate(line.GetStartDate());
                        contact.SetBillStartDate(line.GetStartDate());
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

                        //Neha--Set List Price,Price,Unit Price,Discount from Order Line--4 Sep,2018

                        //contact.SetPriceList(pp.GetPriceList());
                        //contact.SetPriceActual(pp.GetPriceStd());
                        //contact.SetPriceEntered(pp.GetPriceStd());
                        contact.SetPriceList(line.GetPriceList());
                        contact.SetPriceActual(line.GetPriceActual());
                        contact.SetPriceEntered(line.GetPriceEntered());
                        contact.SetQtyEntered(line.GetQtyPerCycle());
                        //Decimal discount = Decimal.Round(Decimal.Divide(Decimal.Multiply(Decimal.Subtract(pp.GetPriceList(), pp.GetPriceStd()), new Decimal(100)), pp.GetPriceList()), 2);
                        contact.SetDiscount(line.GetDiscount());
                        //------------------End----------------------------

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
                            sql = "SELECT Rate FROM VAB_TaxRate WHERE VAB_TaxRate_ID = " + line.GetVAB_TaxRate_ID();
                            Decimal? rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            //Decimal? amt = Decimal.Multiply(pp.GetPriceStd(), (Decimal.Divide(rate.Value, 100)));
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

                        //-------------------------End--------------------------
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
                        //VAB_Contract_ID = contact.GetVAB_Contract_ID();
                        DocumntNo += contact.GetDocumentNo() + ",";
                    }

                }
                dr.Close();
                order.SetCreateServiceContract("Y");
                if (!order.Save())
                {

                }
                return Msg.GetMsg(GetCtx(), "ServiceContractGenerationDone") + DocumntNo.TrimEnd(',');
            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                return Msg.GetMsg(GetCtx(), ex.Message);
            }
            // return "";            
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
