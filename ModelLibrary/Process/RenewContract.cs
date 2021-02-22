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

/* Process: Renew Contract 
 * Writer :Arpit Singh
 * Date   : 02/02/2012 
 */

namespace ViennaAdvantageServer.Process
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
            StringBuilder Sql = new StringBuilder();
            string newcon = "";

            String date = string.Format("{0:dd/MM/yy}", today);
            int Record_id = GetRecord_ID();
            X_VAB_Contract con = new X_VAB_Contract(GetCtx(), Record_id, Get_TrxName());

            if (Record_id != 0)
            {
                Sql.Append("SELECT RenewalType FROM VAB_Contract WHERE VAB_Contract_ID = " + Record_id + " AND RenewContract = 'N' AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                string renewType = Util.GetValueOfString(DB.ExecuteScalar(Sql.ToString(), null, Get_TrxName()));
                if (renewType == "M")
                {
                    Sql.Clear();
                    Sql.Append("SELECT VAB_Contract_ID FROM VAB_Contract WHERE VAB_Contract_ID=" + Record_id + " AND RenewContract = 'N' AND VAF_Client_ID = " + GetVAF_Client_ID());
                }
                else
                {
                    Sql.Clear();
                    Sql.Append("SELECT VAB_Contract_ID FROM VAB_Contract WHERE (EndDate- NVL(CancelBeforeDays,0)) <= SYSDATE AND VAB_Contract_ID=" + Record_id
                        + " AND RenewContract = 'N' AND VAF_Client_ID = " + GetVAF_Client_ID());
                }
            }
            else
            {
                Sql.Clear();
                Sql.Append("SELECT VAB_Contract_ID FROM VAB_Contract WHERE (EndDate- NVL(CancelBeforeDays,0)) <= SYSDATE AND RenewalType='A' AND RenewContract = 'N' AND VAF_Client_ID = " + GetVAF_Client_ID());
            }

            IDataReader dr = DB.ExecuteReader(Sql.ToString(), null, Get_TrxName());
            int count = 0;
            decimal Listprice = 0, Stdprice = 0, TotalRate = 0;
            int cycles = 0, duration = 0, frequency = 0, months = 0;
            DateTime? CDate = null;
            DateTime OldStart, Start, endDate;

            MVAMPriceList priceList = null;
            MVABTaxRate tax = null;
            X_VAB_Contract contact = null;
            X_VAB_Contract New = null;
            ValueNamePair pp = null;
            try
            {
                while (dr.Read())
                {
                    contact = new X_VAB_Contract(GetCtx(), Util.GetValueOfInt(dr[0]), Get_TrxName());
                    if (contact.GetRenewalType() == "M")
                    {
                        // SI_0772: By Clicking on Renew Contract, System is throwing an error as 'NoContractReNewed'.
                        CDate = contact.GetCancellationDate();
                        cycles = Util.GetValueOfInt(contact.GetCycles());

                        if (CDate != null)
                        {
                            continue;
                        }

                        New = new X_VAB_Contract(GetCtx(), 0, Get_TrxName());
                        New.SetRefContract(contact.GetDocumentNo());
                        New.SetVAB_Order_ID(contact.GetVAB_Order_ID());
                        New.SetVAB_OrderLine_ID(contact.GetVAB_OrderLine_ID());
                        OldStart = (DateTime)(contact.GetStartDate());
                        Start = (DateTime)(contact.GetEndDate());
                        New.SetStartDate(Start.AddDays(1));
                        New.SetVAB_BusinessPartner_ID(contact.GetVAB_BusinessPartner_ID());
                        New.SetBill_Location_ID(contact.GetBill_Location_ID());
                        New.SetBill_User_ID(contact.GetBill_User_ID());
                        New.SetSalesRep_ID(contact.GetSalesRep_ID());
                        New.SetVAB_CurrencyType_ID(contact.GetVAB_CurrencyType_ID());
                        New.SetVAB_PaymentTerm_ID(contact.GetVAB_PaymentTerm_ID());

                        frequency = contact.GetVAB_Frequency_ID();
                        New.SetVAB_Frequency_ID(frequency);

                        // Get No Of Months from Frequency
                        months = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NoOfMonths FROM VAB_Frequency WHERE VAB_Frequency_ID=" + frequency, null, Get_TrxName()));
                        duration = months * cycles;
                        endDate = New.GetStartDate().Value.AddMonths(duration);
                        endDate = endDate.AddDays(-1);
                        New.SetEndDate(endDate);
                        New.SetTotalInvoice(cycles);
                        if (Record_id != 0)
                        {
                            // JID_1124:  System has to pick the Pricelist in Service Contract as defined in Renewal Pricelist. also need to pick Price from Latest Valid From Date Version
                            New.SetVAM_PriceList_ID(contact.GetRef_PriceList_ID());
                            priceList = new MVAMPriceList(GetCtx(), contact.GetRef_PriceList_ID(), Get_TrxName());
                            Sql.Clear();
                            Sql.Append("SELECT pp.PriceList, pp.PriceStd FROM VAM_ProductPrice pp INNER JOIN VAM_PriceListVersion plv ON pp.VAM_PriceListVersion_ID = plv.VAM_PriceListVersion_ID"
                                + " WHERE pp.VAM_Product_ID=" + contact.GetVAM_Product_ID() + " AND plv.IsActive='Y' AND plv.VAM_PriceList_ID=" + contact.GetRef_PriceList_ID()
                                + " AND plv.VALIDFROM <= SYSDATE ORDER BY plv.VALIDFROM DESC");
                            DataSet ds = DB.ExecuteDataset(Sql.ToString(), null, Get_TrxName());
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                Listprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                                Stdprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);
                            }
                            else
                            {
                                dr.Close();
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "ProductNotOnPriceList");
                            }
                            //int Version = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                            //Query = "SELECT PriceList FROM VAM_ProductPrice WHERE VAM_PriceListVersion_ID=" + Version + " AND VAM_Product_ID=" + contact.GetVAM_Product_ID();
                            //decimal Listprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                            //Query = "SELECT PriceStd FROM VAM_ProductPrice WHERE VAM_PriceListVersion_ID=" + Version + " AND VAM_Product_ID=" + contact.GetVAM_Product_ID();
                            //decimal Stdprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                            //if (Stdprice == 0 && Listprice == 0)
                            //{
                            //    dr.Close();
                            //    return Msg.GetMsg(GetCtx(), "ProductNotINPriceList");
                            //}

                            New.SetPriceEntered(Stdprice);
                            New.SetPriceActual(Stdprice);
                            New.SetPriceList(Listprice);
                        }
                        else
                        {
                            priceList = new MVAMPriceList(GetCtx(), contact.GetVAM_PriceList_ID(), Get_TrxName());
                            New.SetVAM_PriceList_ID(contact.GetVAM_PriceList_ID());
                            New.SetPriceEntered(contact.GetPriceEntered());
                            New.SetPriceActual(contact.GetPriceActual());
                            New.SetPriceList(contact.GetPriceList());
                        }
                        New.SetVAB_Currency_ID(priceList.GetVAB_Currency_ID());
                        New.SetVAB_UOM_ID(contact.GetVAB_UOM_ID());
                        New.SetVAM_Product_ID(contact.GetVAM_Product_ID());
                        New.SetVAM_PFeature_SetInstance_ID(contact.GetVAM_PFeature_SetInstance_ID());
                        New.SetQtyEntered(contact.GetQtyEntered());
                        New.SetVAB_TaxRate_ID(contact.GetVAB_TaxRate_ID());
                        New.SetVAB_Promotion_ID(contact.GetVAB_Promotion_ID());
                        New.SetRef_Contract_ID(contact.GetVAB_Contract_ID());
                        New.SetVAB_Project_ID(contact.GetVAB_Project_ID());
                        New.SetDescription(contact.GetDescription());
                        //New.SetTaxAmt(contact.GetTaxAmt());
                        New.SetCancelBeforeDays(contact.GetCancelBeforeDays());
                        New.SetCycles(contact.GetCycles());
                        New.SetRenewContract("N");
                        New.SetScheduleContract("N");
                        New.SetDocStatus("DR");
                        New.SetRenewalType("M");
                        New.SetLineNetAmt(Decimal.Multiply(New.GetPriceEntered(), New.GetQtyEntered()));

                        //String sqltax = "SELECT Rate FROM VAB_TaxRate WHERE VAB_TaxRate_ID=" + contact.GetVAB_TaxRate_ID();
                        //Decimal? Rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sqltax, null, Get_TrxName()));                       

                        //Decimal? TotalRate = Util.GetValueOfDecimal((Util.GetValueOfDecimal(New.GetLineNetAmt()) * Util.GetValueOfDecimal(Rate)) / 100);
                        //TotalRate = Decimal.Round(TotalRate.Value, 2);

                        // if Surcharge Tax is selected on Tax, then set value in Surcharge Amount
                        if (New.Get_ColumnIndex("SurchargeAmt") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                        {
                            Decimal surchargeAmt = Env.ZERO;

                            // Calculate Surcharge Amount
                            TotalRate = tax.CalculateSurcharge(New.GetLineNetAmt(), priceList.IsTaxIncluded(), priceList.GetStandardPrecision(), out surchargeAmt);
                            New.SetTaxAmt(TotalRate);
                            New.SetSurchargeAmt(surchargeAmt);
                        }
                        else
                        {
                            // Calculate Tax Amount
                            tax = MVABTaxRate.Get(GetCtx(), contact.GetVAB_TaxRate_ID());
                            TotalRate = tax.CalculateTax(New.GetLineNetAmt(), priceList.IsTaxIncluded(), priceList.GetPricePrecision());
                            New.SetTaxAmt(TotalRate);
                        }
                        // Calculate Discount %
                        Decimal? dis = Decimal.Multiply(Decimal.Divide(Decimal.Subtract(New.GetPriceList(), New.GetPriceEntered()), New.GetPriceList()), 100);
                        New.SetDiscount(dis);                        

                        // Set Grand Total Amount
                        if (priceList.IsTaxIncluded())
                        {
                            New.SetGrandTotal(New.GetLineNetAmt());
                        }
                        else
                        {
                            if (New.Get_ColumnIndex("SurchargeAmt") > 0)
                            {
                                New.SetGrandTotal(Decimal.Add(Decimal.Add(New.GetLineNetAmt(), New.GetTaxAmt()), New.GetSurchargeAmt()));
                            }
                            else
                            {
                                New.SetGrandTotal(Decimal.Add(New.GetLineNetAmt(), New.GetTaxAmt()));
                            }
                        }

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
                            count++;
                            if (Record_id != 0)
                            {
                                contact.SetRef_Contract_ID(New.GetVAB_Contract_ID());
                                contact.SetRenewContract("Y");
                                if (!contact.Save())
                                {
                                    dr.Close();
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                                }
                            }
                        }
                        else
                        {
                            dr.Close();
                            Get_TrxName().Rollback();
                            pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                return !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                            else
                            {
                                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                        }
                    }
                    else
                    {
                        // SI_0772: By Clicking on Renew Contract, System is throwing an error as 'NoContractReNewed'.
                        CDate = contact.GetCancellationDate();
                        cycles = contact.GetCycles();
                        if (CDate != null)
                        {
                            continue;
                        }

                        New = new X_VAB_Contract(GetCtx(), 0, Get_TrxName());
                        New.SetRefContract(contact.GetDocumentNo());
                        New.SetVAB_Order_ID(contact.GetVAB_Order_ID());
                        New.SetVAB_OrderLine_ID(contact.GetVAB_OrderLine_ID());
                        OldStart = (DateTime)(contact.GetStartDate());
                        Start = (DateTime)(contact.GetEndDate());
                        New.SetStartDate(Start.AddDays(1));

                        frequency = contact.GetVAB_Frequency_ID();

                        // Get No Of Months from Frequency
                        months = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NoOfMonths FROM VAB_Frequency WHERE VAB_Frequency_ID=" + frequency, null, Get_TrxName()));
                        duration = months * cycles;

                        endDate = New.GetStartDate().Value.AddMonths(duration);
                        endDate = endDate.AddDays(-1);

                        New.SetEndDate(endDate);
                        New.SetVAB_BusinessPartner_ID(contact.GetVAB_BusinessPartner_ID());
                        New.SetBill_Location_ID(contact.GetBill_Location_ID());
                        New.SetBill_User_ID(contact.GetBill_User_ID());
                        New.SetSalesRep_ID(contact.GetSalesRep_ID());
                        New.SetVAB_CurrencyType_ID(contact.GetVAB_CurrencyType_ID());
                        New.SetVAB_PaymentTerm_ID(contact.GetVAB_PaymentTerm_ID());
                        New.SetVAB_Frequency_ID(frequency);

                        // invoice Count Start                       

                        if (Record_id != 0)
                        {
                            if (contact.GetRef_PriceList_ID() == 0)
                            {
                                dr.Close();
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "FirstSelectPriceList");
                            }
                            New.SetVAM_PriceList_ID(contact.GetRef_PriceList_ID());

                            priceList = new MVAMPriceList(GetCtx(), contact.GetRef_PriceList_ID(), Get_TrxName());
                            Sql.Clear();
                            Sql.Append("SELECT pp.PriceList, pp.PriceStd FROM VAM_ProductPrice pp INNER JOIN VAM_PriceListVersion plv ON pp.VAM_PriceListVersion_ID = plv.VAM_PriceListVersion_ID"
                                + " WHERE pp.VAM_Product_ID=" + contact.GetVAM_Product_ID() + " AND plv.IsActive='Y' AND plv.VAM_PriceList_ID=" + contact.GetRef_PriceList_ID()
                                + " AND plv.VALIDFROM <= SYSDATE ORDER BY plv.VALIDFROM DESC");
                            DataSet ds = DB.ExecuteDataset(Sql.ToString(), null, Get_TrxName());
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                Listprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                                Stdprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);
                            }
                            else
                            {
                                dr.Close();
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "ProductNotOnPriceList");
                            }

                            //String Query = "Select VAM_PriceListVersion_id from VAM_PriceListVersion where IsActive='Y' and VAM_PriceList_Id=" + contact.GetRef_PriceList_ID();
                            //int Version = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                            //Query = "Select PriceList,PriceStd from VAM_ProductPrice where VAM_PriceListVersion_id=" + Version + " and VAM_Product_ID=" + contact.GetVAM_Product_ID();
                            //decimal Listprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                            //Query = "Select PriceList,PriceStd from VAM_ProductPrice where VAM_PriceListVersion_id=" + Version + " and VAM_Product_ID=" + contact.GetVAM_Product_ID();
                            //decimal Stdprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                            //if (Stdprice == 0 && Listprice == 0)
                            //{
                            //    return Msg.GetMsg(GetCtx(), "ProductNotINPriceList");
                            //}

                            New.SetPriceEntered(Stdprice);
                            New.SetPriceActual(Stdprice);
                            New.SetPriceList(Listprice);
                        }
                        else
                        {
                            priceList = new MVAMPriceList(GetCtx(), contact.GetVAM_PriceList_ID(), Get_TrxName());
                            New.SetVAM_PriceList_ID(contact.GetVAM_PriceList_ID());
                            New.SetPriceActual(contact.GetPriceActual());
                            New.SetPriceList(contact.GetPriceList());
                            New.SetPriceEntered(contact.GetPriceEntered());

                        }
                        New.SetTotalInvoice(contact.GetCycles());
                        New.SetVAB_Currency_ID(priceList.GetVAB_Currency_ID());
                        New.SetVAB_UOM_ID(contact.GetVAB_UOM_ID());
                        New.SetVAM_Product_ID(contact.GetVAM_Product_ID());
                        New.SetVAM_PFeature_SetInstance_ID(contact.GetVAM_PFeature_SetInstance_ID());
                        New.SetQtyEntered(contact.GetQtyEntered());
                        New.SetVAB_TaxRate_ID(contact.GetVAB_TaxRate_ID());
                        New.SetVAB_Promotion_ID(contact.GetVAB_Promotion_ID());
                        New.SetRef_Contract_ID(contact.GetVAB_Contract_ID());
                        New.SetVAB_Project_ID(contact.GetVAB_Project_ID());
                        New.SetDescription(contact.GetDescription());
                        New.SetCancelBeforeDays(contact.GetCancelBeforeDays());
                        New.SetCycles(contact.GetCycles());
                        New.SetRenewContract("N");
                        New.SetScheduleContract("Y");
                        New.SetRenewalType("A");
                        New.SetDocStatus("DR");
                        New.SetLineNetAmt(Decimal.Multiply(New.GetPriceEntered(), New.GetQtyEntered()));

                        // Calculate Tax Amount
                        tax = MVABTaxRate.Get(GetCtx(), contact.GetVAB_TaxRate_ID());

                        // if Surcharge Tax is selected on Tax, then set value in Surcharge Amount
                        if (New.Get_ColumnIndex("SurchargeAmt") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                        {
                            Decimal surchargeAmt = Env.ZERO;

                            // Calculate Surcharge Amount
                            TotalRate = tax.CalculateSurcharge(New.GetLineNetAmt(), priceList.IsTaxIncluded(), priceList.GetStandardPrecision(), out surchargeAmt);
                            New.SetTaxAmt(TotalRate);
                            New.SetSurchargeAmt(surchargeAmt);
                        }
                        else
                        {
                            TotalRate = tax.CalculateTax(New.GetLineNetAmt(), priceList.IsTaxIncluded(), priceList.GetPricePrecision());
                            New.SetTaxAmt(TotalRate);
                        }
                        // Calculate Discount %
                        Decimal? dis = Decimal.Multiply(Decimal.Divide(Decimal.Subtract(New.GetPriceList(), New.GetPriceEntered()), New.GetPriceList()), 100);
                        New.SetDiscount(dis);

                        // Set Grand Total Amount
                        if (priceList.IsTaxIncluded())
                        {
                            New.SetGrandTotal(New.GetLineNetAmt());
                        }
                        else
                        {
                            if (New.Get_ColumnIndex("SurchargeAmt") > 0)
                            {
                                New.SetGrandTotal(Decimal.Add(Decimal.Add(New.GetLineNetAmt(), New.GetTaxAmt()), New.GetSurchargeAmt()));
                            }
                            else
                            {
                                New.SetGrandTotal(Decimal.Add(New.GetLineNetAmt(), New.GetTaxAmt()));
                            }
                        }

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
                            count++;
                            contact.SetRef_Contract_ID(New.GetVAB_Contract_ID());
                            contact.SetRenewContract("Y");
                            if (!contact.Save())
                            {
                                dr.Close();
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                            if (!EnterSchedules(New.GetVAB_Contract_ID(), cycles))
                            {
                                dr.Close();
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                            New.SetProcessed(true);
                            if (!New.Save())
                            {
                                dr.Close();
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                        }
                        else
                        {
                            dr.Close();
                            Get_TrxName().Rollback();
                            pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                return !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                            else
                            {
                                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                            }
                        }
                    }
                }
                dr.Close();
                if (count != 0 && Record_id != 0)
                {
                    return Msg.GetMsg(GetCtx(), "ContractReNewed") + ": " + newcon;
                }
                if (count != 0)
                {
                    return Msg.GetMsg(GetCtx(), "ContractReNewed");
                }
                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }

            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            return Msg.GetMsg(GetCtx(), "ContractNotRenew");
        }

        /// <summary>
        /// Create Entry On Schedule Tab
        /// </summary>
        /// <param name="VAB_Contract_ID">Contract ID</param>
        /// <param name="cycles">No of Invoices</param>
        /// <returns>true, if Schedules created</returns>
        private bool EnterSchedules(int VAB_Contract_ID, int cycles)
        {
            X_VAB_ContractSchedule CSchedule = null;
            X_VAB_Contract contract = new X_VAB_Contract(GetCtx(), VAB_Contract_ID, Get_TrxName());
            DateTime start = (DateTime)contract.GetStartDate();
            int frequency = contract.GetVAB_Frequency_ID();

            string Sql = "SELECT NoOfMonths FROM VAB_Frequency WHERE VAB_Frequency_ID=" + frequency;
            int months = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, Get_TrxName()));
            int totalcount = months * cycles;
            DateTime end = start.AddMonths(totalcount);

            if (cycles > 0)
            {
                for (int i = 1; i <= cycles; i++)
                {
                    CSchedule = new X_VAB_ContractSchedule(GetCtx(), 0, Get_TrxName());
                    CSchedule.SetVAB_Contract_ID(VAB_Contract_ID);
                    CSchedule.SetVAB_BusinessPartner_ID(contract.GetVAB_BusinessPartner_ID());
                    CSchedule.SetFROMDATE(start);
                    CSchedule.SetUnitsDelivered(contract.GetQtyEntered());
                    if (i != cycles)
                    {
                        CSchedule.SetEndDate(start.AddMonths(months).AddDays(-1));
                        start = start.AddMonths(months);
                    }
                    else
                    {
                        CSchedule.SetEndDate(end);
                    }
                    CSchedule.SetVAM_Product_ID(contract.GetVAM_Product_ID());
                    CSchedule.SetVAM_PFeature_SetInstance_ID(contract.GetVAM_PFeature_SetInstance_ID());
                    CSchedule.SetTotalAmt(contract.GetLineNetAmt());
                    CSchedule.SetGrandTotal(contract.GetGrandTotal());
                    CSchedule.SetTaxAmt(contract.GetTaxAmt());

                    // if Surcharge Tax is selected on Tax, then set value in Surcharge Amount
                    if (CSchedule.Get_ColumnIndex("SurchargeAmt") > 0)
                    {
                        CSchedule.SetSurchargeAmt(contract.GetSurchargeAmt());
                    }

                    CSchedule.SetVAB_UOM_ID(contract.GetVAB_UOM_ID());
                    CSchedule.SetPriceEntered(contract.GetPriceEntered());
                    CSchedule.SetProcessed(true);
                    if (!CSchedule.Save())
                    {
                        log.Info("Contract Schedule not Saved for Service Contract no: " + contract.GetDocumentNo());
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
