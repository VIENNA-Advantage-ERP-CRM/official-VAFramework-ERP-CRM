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
        VAdvantage.Model.X_C_Contract contact = null;
        VAdvantage.Model.X_C_OrderLine line = null;
        string DocumntNo;

        protected override void Prepare()
        {
            orderID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            //int C_Contract_ID = 0;
            String Sql = "SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID=" + orderID + " AND CreateServiceContract='N' AND IsContract='Y' AND IsActive='Y'";
            dr = DB.ExecuteReader(Sql);
            try
            {
                VAdvantage.Model.X_C_Order order = new VAdvantage.Model.X_C_Order(GetCtx(), orderID, Get_TrxName());
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
                    line = new VAdvantage.Model.X_C_OrderLine(GetCtx(), Util.GetValueOfInt(dr[0]), Get_TrxName());
                    if (line.IsContract() && line.GetC_Contract_ID() == 0)
                    {

                        contact = new VAdvantage.Model.X_C_Contract(GetCtx(), 0, Get_TrxName());
                        int M_PriceList_ID = Util.GetValueOfInt(order.GetM_PriceList_ID());

                        //Neha---Commented code because object created but not used in further class---04 Sep,2018

                        //VAdvantage.Model.MProductPricing pp = new VAdvantage.Model.MProductPricing(GetCtx().GetAD_Client_ID(), GetCtx().GetAD_Org_ID(),
                        //    line.GetM_Product_ID(), order.GetC_BPartner_ID(), line.GetQtyOrdered(), true);                        
                        //pp.SetM_PriceList_ID(M_PriceList_ID);
                        //VAdvantage.Model.MProduct prd = new VAdvantage.Model.MProduct(GetCtx(), line.GetM_Product_ID(), null);
                        //pp.SetC_UOM_ID(prd.GetC_UOM_ID());

                        string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                        + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                        + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                        + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                        + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                        + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                        + "ORDER BY plv.ValidFrom DESC";

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
                            // int prislst = Util.GetValueOfInt(dr[4].ToString());
                            //	PriceList Version
                            //M_PriceList_Version_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_PriceList_Version_ID"]);
                        }
                        //int M_PriceList_Version_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
                        //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                        //Neha---Set Tenant,Organization from Sales Order---11 Sep,2018
                        contact.SetAD_Client_ID(order.GetAD_Client_ID());
                        contact.SetAD_Org_ID(order.GetAD_Org_ID());
                        //---------------------End ------------------
                        // Set Contract type accounts receivable done by rakesh kunar 17/June/2021
                        contact.SetContractType(X_C_Contract.CONTRACTTYPE_AccountsReceivable);
                        contact.SetDescription(order.GetDescription());
                        contact.SetC_Order_ID(order.GetC_Order_ID());
                        contact.SetC_OrderLine_ID(line.GetC_OrderLine_ID());
                        contact.SetStartDate(line.GetStartDate());
                        contact.SetBillStartDate(line.GetStartDate());
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
                        //int frequency = Util.GetValueOfInt(line.GetC_Frequency_ID());
                        //string PSql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
                        //int days = Util.GetValueOfInt(DB.ExecuteScalar(PSql, null, null));
                        //int totaldays = (Edate - SDate).Days;
                        //int count = totaldays / days;
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
                            sql = "SELECT Rate FROM C_Tax WHERE C_Tax_ID = " + line.GetC_Tax_ID();
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
                            dr.Close();
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
                                dr.Close();
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
                        //C_Contract_ID = contact.GetC_Contract_ID();
                        DocumntNo += contact.GetDocumentNo() + ",";

                    }

                }
                dr.Close();
                order.SetCreateServiceContract("Y");
                if (!order.Save())
                {

                }
                // Added Document No on Message
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
          int C_BPartner_ID = ctx.GetContextAsInt(WindowNo, "C_BPartner_ID");
                Decimal Qty = Util.GetValueOfDecimal(mTab.GetValue("QtyOrdered"));
                bool isSOTrx = ctx.GetContext(WindowNo, "IsSOTrx").Equals("Y");
                MProductPricing pp = new MProductPricing(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(),
                        M_Product_ID, C_BPartner_ID, Qty, isSOTrx);
                int M_PriceList_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_ID");
                pp.SetM_PriceList_ID(M_PriceList_ID);
                /** PLV is only accurate if PL selected in header
                int M_PriceList_Version_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
                pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
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
                mTab.SetValue("C_Currency_ID", System.Convert.ToInt32(pp.GetC_Currency_ID()));
                mTab.SetValue("Discount", pp.GetDiscount());
                mTab.SetValue("C_UOM_ID", System.Convert.ToInt32(pp.GetC_UOM_ID()));
                mTab.SetValue("QtyOrdered", mTab.GetValue("QtyEntered"));
                ctx.SetContext(WindowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
                ctx.SetContext(WindowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");
        **********************/



    }
}
