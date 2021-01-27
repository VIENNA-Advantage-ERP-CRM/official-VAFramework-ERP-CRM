/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : CreateContractInvoice
 * Purpose        : 
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   02-Feb-2012
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Security.Policy;
using VAdvantage.ProcessEngine;
//using ViennaAdvantage.Model;

namespace VAdvantage.Process
{
    public class CreateContractInvoice : SvrProcess
    {

        #region Private Variables
        string msg = "";
        string sql = "";
        // int VAB_YearPeriod_ID = 0;
        VAdvantage.Model.X_VAB_Contract cont = null;
        #endregion

        protected override void Prepare()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            int VAB_Contract_ID = Util.GetValueOfInt(GetRecord_ID());
            if (VAB_Contract_ID != 0)
            {
                cont = new VAdvantage.Model.X_VAB_Contract(GetCtx(), VAB_Contract_ID, null);
                string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                int[] contSch = VAdvantage.Model.X_VAB_ContractSchedule.GetAllIDs("VAB_ContractSchedule", "VAB_Contract_ID = " + VAB_Contract_ID + " and FROMDATE <= '" + date + "' and VAB_Invoice_id is null", null);
                if (contSch != null)
                {
                    for (int i = 0; i < contSch.Length; i++)
                    {
                        VAdvantage.Model.X_VAB_ContractSchedule contSchedule = new VAdvantage.Model.X_VAB_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), null);
                        GenerateInvoice(contSchedule);
                    }
                }
                sql = "select count(*) from VAB_Contractschedule where VAB_Contract_id = " + VAB_Contract_ID + " and VAB_Invoice_id is not null";
                string sql1 = "update VAB_Contract set invoicesgenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) + " where VAB_Contract_id = " + VAB_Contract_ID;
                int res = DB.ExecuteQuery(sql1, null, null);
            }
            else
            {
                sql = "select VAB_Contract_id from VAB_Contract where isactive = 'Y' and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        cont = new VAdvantage.Model.X_VAB_Contract(GetCtx(), Util.GetValueOfInt(idr[0]), null);
                        string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                        int[] contSch = VAdvantage.Model.X_VAB_ContractSchedule.GetAllIDs("VAB_ContractSchedule", "VAB_Contract_ID = " + cont.GetVAB_Contract_ID() + " and FROMDATE <= '" + date + "' and VAB_Invoice_id is null", null);
                        if (contSch != null)
                        {
                            for (int i = 0; i < contSch.Length; i++)
                            {
                                VAdvantage.Model.X_VAB_ContractSchedule contSchedule = new VAdvantage.Model.X_VAB_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), null);
                                GenerateInvoice(contSchedule);
                            }
                        }
                        sql = "select count(*) from VAB_Contractschedule where VAB_Contract_id = " + cont.GetVAB_Contract_ID() + " and VAB_Invoice_id is not null";
                        string sql1 = "update VAB_Contract set invoicesgenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) + " where VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                        int res = DB.ExecuteQuery(sql1, null, null);
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                catch
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
            }
            msg = Msg.GetMsg(GetCtx(), "RecordsGenerated");
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contSchedule"></param>
        private void GenerateInvoice(VAdvantage.Model.X_VAB_ContractSchedule contSchedule)
        {
            if (contSchedule.IsActive())
            {
                int res = 0;
                sql = "select VAB_DocTypes_id from VAB_DocTypes where name = 'AR Invoice' and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
                int VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

               // sql = "select noofdays from VAB_Frequency where VAB_Frequency_id = " + cont.GetVAB_Frequency_ID();
              //  Decimal? days = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                Decimal? price = null;

                if (!cont.IsCancel())
                {
                    price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                }
                else
                {
                    sql = "update VAB_Contract set renewaltype = null where VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                    int res2 = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));

                    if (contSchedule.GetEndDate() <= cont.GetCancellationDate())
                    {
                        price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                    }
                    else
                    {
                        sql = "select max(VAB_Contractschedule_id) from VAB_Contractschedule where VAB_Invoice_id is not null and VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                        int VAB_Contractschedule_id = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (VAB_Contractschedule_id != 0)
                        {
                            string date = cont.GetCancellationDate().Value.ToString("dd-MMM-yyyy");
                            //  int contsch = Util.GetValueOfInt(contSchedule.GetVAB_ContractSchedule_ID()) - 1;
                            sql = "select daysbetween('" + date + "', enddate) from VAB_Contractschedule where VAB_Contractschedule_id = " + VAB_Contractschedule_id;
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                           // price = Decimal.Multiply(cont.GetPriceEntered(), diffDays.Value);
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            sql = "update VAB_Contractschedule set isactive = 'N' where enddate > '" + date + "' and VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                            res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                        }
                        else
                        {
                            sql = "select daysbetween(cancellationdate, startdate) from VAB_Contract where VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                            //price = Decimal.Multiply(Decimal.Divide(cont.GetPriceEntered(), days.Value), diffDays.Value);
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            if (diffDays > 0)
                            {
                                sql = "update VAB_Contractschedule set isactive = 'N' where VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                            }
                            else
                            {
                                sql = "update VAB_Contractschedule set isactive = 'N' where VAB_Contract_id = " + cont.GetVAB_Contract_ID();
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                            }
                            // sql = "update VAB_Contractschedule set isactive = 'N' where enddate > '" + System.DateTime.Now + "' and VAB_Contract_id = " + cont.GetVAB_Contract_ID();

                        }
                    }
                }

                price = Decimal.Round(price.Value, 2, MidpointRounding.AwayFromZero);

                VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, null);
                inv.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                inv.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                inv.SetVAB_BusinessPartner_ID(cont.GetVAB_BusinessPartner_ID());
                if (Util.GetValueOfInt(cont.GetVAB_Order_ID()) != 0)
                {
                    inv.SetVAB_Order_ID(cont.GetVAB_Order_ID());
                }
                inv.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                inv.SetVAB_DocTypesTarget_ID(VAB_DocTypes_ID);
                inv.SetVAB_BPart_Location_ID(cont.GetBill_Location_ID());
                inv.SetVAB_Currency_ID(cont.GetVAB_Currency_ID());
                inv.SetVAB_PaymentTerm_ID(cont.GetVAB_PaymentTerm_ID());
                inv.SetVAB_Promotion_ID(cont.GetVAB_Promotion_ID());
                inv.SetIsSOTrx(true);
                inv.SetM_PriceList_ID(cont.GetM_PriceList_ID());
                inv.SetSalesRep_ID(cont.GetSalesRep_ID());
                inv.SetVAB_Contract_ID(cont.GetVAB_Contract_ID());
                if (!inv.Save())
                {

                }

                if (inv.GetVAB_Invoice_ID() != 0)
                {
                    VAdvantage.Model.MInvoiceLine invLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, null);
                    invLine.SetVAF_Client_ID(inv.GetVAF_Client_ID());
                    invLine.SetVAF_Org_ID(inv.GetVAF_Org_ID());
                    invLine.SetVAB_Promotion_ID(inv.GetVAB_Promotion_ID());
                    invLine.SetVAB_Invoice_ID(inv.GetVAB_Invoice_ID());
                    invLine.SetVAB_UOM_ID(cont.GetVAB_UOM_ID());
                    invLine.SetM_Product_ID(cont.GetM_Product_ID());
                    if (Util.GetValueOfInt(cont.GetVAB_OrderLine_ID()) != 0)
                    {
                        invLine.SetVAB_OrderLine_ID(cont.GetVAB_OrderLine_ID());
                    }
                    invLine.SetVAB_TaxRate_ID(cont.GetVAB_TaxRate_ID());
                    invLine.SetQty(cont.GetQtyEntered());
                    invLine.SetQtyEntered(cont.GetQtyEntered());
                    // invLine.SetQtyInvoiced(1);
                    // invLine.SetPrice(price.Value);
                    invLine.SetPriceActual(cont.GetPriceEntered());
                    invLine.SetPriceEntered(cont.GetPriceEntered());
                    //  invLine.SetPriceLimit(price);
                    invLine.SetPriceList(cont.GetPriceEntered());
                    if (!invLine.Save())
                    {

                    }
                }

                string comp = inv.CompleteIt();
                inv.SetDocAction("CL");
                inv.SetDocStatus("CO");
                if (!inv.Save())
                {

                }

                sql = "update VAB_Invoice set VAB_Contract_id = " + cont.GetVAB_Contract_ID() + " where VAB_Invoice_id = " + inv.GetVAB_Invoice_ID();
                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));

                sql = "select sum(taxamt) from VAB_Tax_Invoice where VAB_Invoice_id = " + inv.GetVAB_Invoice_ID();
                Decimal? taxAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                sql = "update VAB_Contractschedule set VAB_Invoice_id = " + inv.GetVAB_Invoice_ID() + ", processed = 'Y' where VAB_Contractschedule_id = " + contSchedule.GetVAB_ContractSchedule_ID();
                // sql = "update VAB_Contractschedule set VAB_Invoice_id = " + inv.GetVAB_Invoice_ID() + ", processed = 'Y', TotalAmt = " + inv.GetTotalLines() + ", taxamt = " + taxAmt + ", grandtotal = " + inv.GetGrandTotal() + " where VAB_Contractschedule_id = " + contSchedule.GetVAB_ContractSchedule_ID();
                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
            }
        }
    }
}
