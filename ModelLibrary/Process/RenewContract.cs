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
            StringBuilder newcon = new StringBuilder();

            String date = string.Format("{0:dd/MM/yy}", today);
            int Record_id = GetRecord_ID();
            X_C_Contract con = new X_C_Contract(GetCtx(), Record_id, Get_TrxName());

            if (Record_id != 0)
            {
                Sql.Append("SELECT RenewalType FROM C_Contract WHERE C_Contract_ID = " + Record_id + " AND RenewContract = 'N' AND IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                string renewType = Util.GetValueOfString(DB.ExecuteScalar(Sql.ToString(), null, Get_TrxName()));
                if (renewType.Equals(X_C_Contract.RENEWALTYPE_Manual))
                {
                    Sql.Clear();
                    Sql.Append("SELECT * FROM C_Contract WHERE C_Contract_ID=" + Record_id + " AND RenewContract = 'N' AND AD_Client_ID = " + GetAD_Client_ID());
                }
                else
                {
                    // Done by Rakesh Kumar(228) to exectue query in postgresql db
                    Sql.Clear();
                    Sql.Append(DBFunctionCollection.GetContract(Record_id, GetAD_Client_ID()));
                }
            }
            else
            {
                Sql.Clear();
                //Get contracts based on client
                Sql.Append(DBFunctionCollection.GetContract(0, GetAD_Client_ID()));
            }

            // Changed datareader to dataset to resolve issue in postgresql done by Rakesh Kumar(228) on 31/May/2021
            DataSet dsCont = DB.ExecuteDataset(Sql.ToString(), null, Get_TrxName());
            int count = 0;
            decimal Listprice = 0, Stdprice = 0, TotalRate = 0;
            int cycles = 0, duration = 0, frequency = 0, months = 0;
            DateTime? CDate = null;
            DateTime OldStart, Start, endDate;

            MPriceList priceList = null;
            MTax tax = null;
            X_C_Contract contact = null;
            X_C_Contract New = null;
            ValueNamePair pp = null;
            try
            {
                if (dsCont != null && dsCont.Tables.Count > 0 && dsCont.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCont.Tables[0].Rows.Count; i++)
                    {
                        // Get Contract Detail using current datarow
                        contact = new X_C_Contract(GetCtx(), dsCont.Tables[0].Rows[i], Get_TrxName());
                        if (contact.GetRenewalType().Equals(X_C_Contract.RENEWALTYPE_Manual))
                        {
                            // SI_0772: By Clicking on Renew Contract, System is throwing an error as 'NoContractReNewed'.
                            CDate = contact.GetCancellationDate();
                            cycles = Util.GetValueOfInt(contact.GetCycles());

                            if (CDate != null)
                            {
                                continue;
                            }

                            New = new X_C_Contract(GetCtx(), 0, Get_TrxName());
                            New.SetAD_Org_ID(contact.GetAD_Org_ID());
                            New.SetRefContract(contact.GetDocumentNo());
                            New.SetC_Order_ID(contact.GetC_Order_ID());
                            New.SetC_OrderLine_ID(contact.GetC_OrderLine_ID());
                            OldStart = (DateTime)(contact.GetStartDate());
                            Start = (DateTime)(contact.GetEndDate());
                            New.SetStartDate(Start.AddDays(1));
                            New.SetC_BPartner_ID(contact.GetC_BPartner_ID());
                            New.SetBill_Location_ID(contact.GetBill_Location_ID());
                            New.SetBill_User_ID(contact.GetBill_User_ID());
                            New.SetSalesRep_ID(contact.GetSalesRep_ID());
                            New.SetC_ConversionType_ID(contact.GetC_ConversionType_ID());
                            New.SetC_PaymentTerm_ID(contact.GetC_PaymentTerm_ID());

                            frequency = contact.GetC_Frequency_ID();
                            New.SetC_Frequency_ID(frequency);

                            // Get No Of Months from Frequency
                            months = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NoOfMonths FROM C_Frequency WHERE C_Frequency_ID=" + frequency, null, Get_TrxName()));
                            duration = months * cycles;
                            endDate = New.GetStartDate().Value.AddMonths(duration);
                            endDate = endDate.AddDays(-1);
                            New.SetEndDate(endDate);
                            New.SetTotalInvoice(cycles);
                            if (Record_id != 0)
                            {
                                // JID_1124:  System has to pick the Pricelist in Service Contract as defined in Renewal Pricelist. also need to pick Price from Latest Valid From Date Version
                                New.SetM_PriceList_ID(contact.GetRef_PriceList_ID());
                                priceList = new MPriceList(GetCtx(), contact.GetRef_PriceList_ID(), Get_TrxName());
                                Sql.Clear();
                                Sql.Append("SELECT pp.PriceList, pp.PriceStd FROM M_ProductPrice pp INNER JOIN M_PriceList_Version plv ON pp.M_PriceList_Version_ID = plv.M_PriceList_Version_ID"
                                    + " WHERE pp.M_Product_ID=" + contact.GetM_Product_ID() + " AND plv.IsActive='Y' AND plv.M_PriceList_ID=" + contact.GetRef_PriceList_ID()
                                    + " AND plv.VALIDFROM <= " + GlobalVariable.TO_DATE(DateTime.Now, true) + " ORDER BY plv.VALIDFROM DESC");
                                DataSet ds = DB.ExecuteDataset(Sql.ToString(), null, Get_TrxName());
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    Listprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                                    Stdprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);
                                }
                                else
                                {
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "ProductNotOnPriceList");
                                }
                                //int Version = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                                //Query = "SELECT PriceList FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + Version + " AND M_Product_ID=" + contact.GetM_Product_ID();
                                //decimal Listprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                                //Query = "SELECT PriceStd FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + Version + " AND M_Product_ID=" + contact.GetM_Product_ID();
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
                                priceList = new MPriceList(GetCtx(), contact.GetM_PriceList_ID(), Get_TrxName());
                                New.SetM_PriceList_ID(contact.GetM_PriceList_ID());
                                New.SetPriceEntered(contact.GetPriceEntered());
                                New.SetPriceActual(contact.GetPriceActual());
                                New.SetPriceList(contact.GetPriceList());
                            }
                            // Set Contract Type done by Rakesh Kumar(228) date 19/May/2021
                            New.SetContractType(contact.GetContractType());
                            New.SetC_Currency_ID(priceList.GetC_Currency_ID());
                            New.SetC_UOM_ID(contact.GetC_UOM_ID());
                            New.SetM_Product_ID(contact.GetM_Product_ID());
                            New.SetM_AttributeSetInstance_ID(contact.GetM_AttributeSetInstance_ID());
                            New.SetQtyEntered(contact.GetQtyEntered());
                            New.SetC_Tax_ID(contact.GetC_Tax_ID());
                            New.SetC_Campaign_ID(contact.GetC_Campaign_ID());
                            New.SetRef_Contract_ID(contact.GetC_Contract_ID());
                            New.SetC_Project_ID(contact.GetC_Project_ID());
                            New.SetDescription(contact.GetDescription());
                            //New.SetTaxAmt(contact.GetTaxAmt());
                            New.SetCancelBeforeDays(contact.GetCancelBeforeDays());
                            New.SetCycles(contact.GetCycles());
                            New.SetRenewContract("N");
                            New.SetScheduleContract("N");
                            New.SetDocStatus("DR");
                            New.SetRenewalType("M");
                            New.SetLineNetAmt(Decimal.Multiply(New.GetPriceEntered(), New.GetQtyEntered()));

                            //String sqltax = "SELECT Rate FROM C_Tax WHERE C_Tax_ID=" + contact.GetC_Tax_ID();
                            //Decimal? Rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sqltax, null, Get_TrxName()));                       

                            //Decimal? TotalRate = Util.GetValueOfDecimal((Util.GetValueOfDecimal(New.GetLineNetAmt()) * Util.GetValueOfDecimal(Rate)) / 100);
                            //TotalRate = Decimal.Round(TotalRate.Value, 2);

                            // Calculate tax done by Rakesh Kumar(228) date 19/May/2021
                            tax = MTax.Get(GetCtx(), contact.GetC_Tax_ID());

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
                                tax = MTax.Get(GetCtx(), contact.GetC_Tax_ID());
                                TotalRate = tax.CalculateTax(New.GetLineNetAmt(), priceList.IsTaxIncluded(), priceList.GetPricePrecision());
                                New.SetTaxAmt(TotalRate);
                            }
                            // Calculate Discount %
                            Decimal? dis = Decimal.Multiply(Decimal.Divide(Decimal.Subtract(New.GetPriceList(), New.GetPriceEntered()), New.GetPriceList()), 100);
                            // Round off based on Price Precision decimal points done by Rakesh Kumar(228) on 21/May/2021
                            New.SetDiscount(Math.Round(dis ?? 0, priceList.GetPricePrecision(), MidpointRounding.AwayFromZero));

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
                                newcon.Append(New.GetDocumentNo());
                                count++;
                                if (Record_id != 0)
                                {
                                    contact.SetRef_Contract_ID(New.GetC_Contract_ID());
                                    contact.SetRenewContract("Y");
                                    if (!contact.Save())
                                    {
                                        dsCont.Dispose();
                                        Get_TrxName().Rollback();
                                        return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                                    }
                                }
                            }
                            else
                            {
                                dsCont.Dispose();
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

                            New = new X_C_Contract(GetCtx(), 0, Get_TrxName());
                            New.SetAD_Org_ID(contact.GetAD_Org_ID());
                            New.SetRefContract(contact.GetDocumentNo());
                            New.SetC_Order_ID(contact.GetC_Order_ID());
                            New.SetC_OrderLine_ID(contact.GetC_OrderLine_ID());
                            OldStart = (DateTime)(contact.GetStartDate());
                            Start = (DateTime)(contact.GetEndDate());
                            New.SetStartDate(Start.AddDays(1));

                            frequency = contact.GetC_Frequency_ID();

                            // Get No Of Months from Frequency
                            months = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NoOfMonths FROM C_Frequency WHERE C_Frequency_ID=" + frequency, null, Get_TrxName()));
                            duration = months * cycles;

                            endDate = New.GetStartDate().Value.AddMonths(duration);
                            endDate = endDate.AddDays(-1);

                            New.SetEndDate(endDate);
                            New.SetC_BPartner_ID(contact.GetC_BPartner_ID());
                            New.SetBill_Location_ID(contact.GetBill_Location_ID());
                            New.SetBill_User_ID(contact.GetBill_User_ID());
                            New.SetSalesRep_ID(contact.GetSalesRep_ID());
                            New.SetC_ConversionType_ID(contact.GetC_ConversionType_ID());
                            New.SetC_PaymentTerm_ID(contact.GetC_PaymentTerm_ID());
                            New.SetC_Frequency_ID(frequency);

                            // invoice Count Start                       

                            if (Record_id != 0)
                            {
                                if (contact.GetRef_PriceList_ID() == 0)
                                {
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "FirstSelectPriceList");
                                }
                                New.SetM_PriceList_ID(contact.GetRef_PriceList_ID());

                                priceList = new MPriceList(GetCtx(), contact.GetRef_PriceList_ID(), Get_TrxName());
                                Sql.Clear();
                                Sql.Append("SELECT pp.PriceList, pp.PriceStd FROM M_ProductPrice pp INNER JOIN M_PriceList_Version plv ON pp.M_PriceList_Version_ID = plv.M_PriceList_Version_ID"
                                    + " WHERE pp.M_Product_ID=" + contact.GetM_Product_ID() + " AND plv.IsActive='Y' AND plv.M_PriceList_ID=" + contact.GetRef_PriceList_ID()
                                    + " AND plv.VALIDFROM <= " + GlobalVariable.TO_DATE(DateTime.Now, true) + " ORDER BY plv.VALIDFROM DESC");
                                DataSet ds = DB.ExecuteDataset(Sql.ToString(), null, Get_TrxName());
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    Listprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                                    Stdprice = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);
                                }
                                else
                                {
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "ProductNotOnPriceList");
                                }

                                //String Query = "Select M_PriceList_Version_id from M_PriceList_Version where IsActive='Y' and M_PriceList_Id=" + contact.GetRef_PriceList_ID();
                                //int Version = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                                //Query = "Select PriceList,PriceStd from M_ProductPrice where M_PriceList_Version_id=" + Version + " and M_Product_ID=" + contact.GetM_Product_ID();
                                //decimal Listprice = Util.GetValueOfInt(DB.ExecuteScalar(Query));
                                //Query = "Select PriceList,PriceStd from M_ProductPrice where M_PriceList_Version_id=" + Version + " and M_Product_ID=" + contact.GetM_Product_ID();
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
                                priceList = new MPriceList(GetCtx(), contact.GetM_PriceList_ID(), Get_TrxName());
                                New.SetM_PriceList_ID(contact.GetM_PriceList_ID());
                                New.SetPriceActual(contact.GetPriceActual());
                                New.SetPriceList(contact.GetPriceList());
                                New.SetPriceEntered(contact.GetPriceEntered());

                            }
                            // Set Contract Type done by Rakesh Kumar(228) date 19/May/2021
                            New.SetContractType(contact.GetContractType());
                            New.SetTotalInvoice(contact.GetCycles());
                            New.SetC_Currency_ID(priceList.GetC_Currency_ID());
                            New.SetC_UOM_ID(contact.GetC_UOM_ID());
                            New.SetM_Product_ID(contact.GetM_Product_ID());
                            New.SetM_AttributeSetInstance_ID(contact.GetM_AttributeSetInstance_ID());
                            New.SetQtyEntered(contact.GetQtyEntered());
                            New.SetC_Tax_ID(contact.GetC_Tax_ID());
                            New.SetC_Campaign_ID(contact.GetC_Campaign_ID());
                            New.SetRef_Contract_ID(contact.GetC_Contract_ID());
                            New.SetC_Project_ID(contact.GetC_Project_ID());
                            New.SetDescription(contact.GetDescription());
                            New.SetCancelBeforeDays(contact.GetCancelBeforeDays());
                            New.SetCycles(contact.GetCycles());
                            New.SetRenewContract("N");
                            New.SetScheduleContract("Y");
                            New.SetRenewalType("A");
                            New.SetDocStatus("DR");
                            New.SetLineNetAmt(Decimal.Multiply(New.GetPriceEntered(), New.GetQtyEntered()));

                            // Calculate Tax Amount
                            tax = MTax.Get(GetCtx(), contact.GetC_Tax_ID());

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
                            // Round off based on Price Precision done by Rakesh Kumar(228) on 21/May/2021
                            New.SetDiscount(Math.Round(dis ?? 0, priceList.GetPricePrecision(), MidpointRounding.AwayFromZero));

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
                                newcon.Append(New.GetDocumentNo() + ",");
                                count++;
                                contact.SetRef_Contract_ID(New.GetC_Contract_ID());
                                contact.SetRenewContract("Y");
                                if (!contact.Save())
                                {
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                                }
                                if (!EnterSchedules(New.GetC_Contract_ID(), cycles))
                                {
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                                }
                                New.SetProcessed(true);
                                if (!New.Save())
                                {
                                    Get_TrxName().Rollback();
                                    return Msg.GetMsg(GetCtx(), "ContractNotRenew");
                                }
                            }
                            else
                            {
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
                    if (count != 0 && Record_id != 0)
                    {
                        return Msg.GetMsg(GetCtx(), "ContractReNewed") + ": " + newcon.ToString();
                    }
                    if (count != 0)
                    {
                        return Msg.GetMsg(GetCtx(), "ContractReNewed") + ": " + newcon.ToString();
                    }
                }
                return Msg.GetMsg(GetCtx(), "ContractNotRenew");
            }
            catch (Exception ex)
            {
                if (dsCont != null)
                {
                    dsCont.Dispose();
                }
            }
            finally
            {
                if (dsCont != null)
                {
                    dsCont.Dispose();
                }
            }
            return Msg.GetMsg(GetCtx(), "ContractNotRenew");
        }

        /// <summary>
        /// Create Entry On Schedule Tab
        /// </summary>
        /// <param name="C_Contract_ID">Contract ID</param>
        /// <param name="cycles">No of Invoices</param>
        /// <returns>true, if Schedules created</returns>
        private bool EnterSchedules(int C_Contract_ID, int cycles)
        {
            X_C_ContractSchedule CSchedule = null;
            X_C_Contract contract = new X_C_Contract(GetCtx(), C_Contract_ID, Get_TrxName());
            DateTime start = (DateTime)contract.GetStartDate();
            int frequency = contract.GetC_Frequency_ID();

            string Sql = "SELECT NoOfMonths FROM C_Frequency WHERE C_Frequency_ID=" + frequency;
            int months = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, Get_TrxName()));
            int totalcount = months * cycles;
            DateTime end = start.AddMonths(totalcount);

            if (cycles > 0)
            {
                for (int i = 1; i <= cycles; i++)
                {
                    CSchedule = new X_C_ContractSchedule(GetCtx(), 0, Get_TrxName());
                    CSchedule.SetAD_Org_ID(contract.GetAD_Org_ID());
                    CSchedule.SetC_Contract_ID(C_Contract_ID);
                    CSchedule.SetC_BPartner_ID(contract.GetC_BPartner_ID());
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
                    CSchedule.SetM_Product_ID(contract.GetM_Product_ID());
                    CSchedule.SetM_AttributeSetInstance_ID(contract.GetM_AttributeSetInstance_ID());
                    CSchedule.SetTotalAmt(contract.GetLineNetAmt());
                    CSchedule.SetGrandTotal(contract.GetGrandTotal());
                    CSchedule.SetTaxAmt(contract.GetTaxAmt());

                    // if Surcharge Tax is selected on Tax, then set value in Surcharge Amount
                    if (CSchedule.Get_ColumnIndex("SurchargeAmt") > 0)
                    {
                        CSchedule.SetSurchargeAmt(contract.GetSurchargeAmt());
                    }

                    CSchedule.SetC_UOM_ID(contract.GetC_UOM_ID());
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