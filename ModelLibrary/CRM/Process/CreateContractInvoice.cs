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
        // int C_Period_ID = 0;
        VAdvantage.Model.X_C_Contract cont = null;
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
            int C_Contract_ID = Util.GetValueOfInt(GetRecord_ID());
            if (C_Contract_ID != 0)
            {
                cont = new VAdvantage.Model.X_C_Contract(GetCtx(), C_Contract_ID, null);
                string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                int[] contSch = VAdvantage.Model.X_C_ContractSchedule.GetAllIDs("C_ContractSchedule", "C_Contract_ID = " + C_Contract_ID + " and FROMDATE <= '" + date + "' and c_invoice_id is null", null);
                if (contSch != null)
                {
                    for (int i = 0; i < contSch.Length; i++)
                    {
                        VAdvantage.Model.X_C_ContractSchedule contSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), null);
                        GenerateInvoice(contSchedule);
                    }
                }
                sql = "select count(*) from c_contractschedule where c_contract_id = " + C_Contract_ID + " and c_invoice_id is not null";
                string sql1 = "update c_contract set invoicesgenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) + " where c_contract_id = " + C_Contract_ID;
                int res = DB.ExecuteQuery(sql1, null, null);
            }
            else
            {
                sql = "select C_Contract_id from c_contract where isactive = 'Y' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        cont = new VAdvantage.Model.X_C_Contract(GetCtx(), Util.GetValueOfInt(idr[0]), null);
                        string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                        int[] contSch = VAdvantage.Model.X_C_ContractSchedule.GetAllIDs("C_ContractSchedule", "C_Contract_ID = " + cont.GetC_Contract_ID() + " and FROMDATE <= '" + date + "' and c_invoice_id is null", null);
                        if (contSch != null)
                        {
                            for (int i = 0; i < contSch.Length; i++)
                            {
                                VAdvantage.Model.X_C_ContractSchedule contSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), null);
                                GenerateInvoice(contSchedule);
                            }
                        }
                        sql = "select count(*) from c_contractschedule where c_contract_id = " + cont.GetC_Contract_ID() + " and c_invoice_id is not null";
                        string sql1 = "update c_contract set invoicesgenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) + " where c_contract_id = " + cont.GetC_Contract_ID();
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
        private void GenerateInvoice(VAdvantage.Model.X_C_ContractSchedule contSchedule)
        {
            if (contSchedule.IsActive())
            {
                int res = 0;
                sql = "select c_doctype_id from c_doctype where name = 'AR Invoice' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

               // sql = "select noofdays from c_frequency where c_frequency_id = " + cont.GetC_Frequency_ID();
              //  Decimal? days = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                Decimal? price = null;

                if (!cont.IsCancel())
                {
                    price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                }
                else
                {
                    sql = "update c_contract set renewaltype = null where c_contract_id = " + cont.GetC_Contract_ID();
                    int res2 = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));

                    if (contSchedule.GetEndDate() <= cont.GetCancellationDate())
                    {
                        price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                    }
                    else
                    {
                        sql = "select max(c_contractschedule_id) from c_contractschedule where c_invoice_id is not null and c_contract_id = " + cont.GetC_Contract_ID();
                        int c_contractschedule_id = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (c_contractschedule_id != 0)
                        {
                            string date = cont.GetCancellationDate().Value.ToString("dd-MMM-yyyy");
                            //  int contsch = Util.GetValueOfInt(contSchedule.GetC_ContractSchedule_ID()) - 1;
                            sql = "select daysbetween('" + date + "', enddate) from c_contractschedule where c_contractschedule_id = " + c_contractschedule_id;
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                           // price = Decimal.Multiply(cont.GetPriceEntered(), diffDays.Value);
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            sql = "update c_contractschedule set isactive = 'N' where enddate > '" + date + "' and c_contract_id = " + cont.GetC_Contract_ID();
                            res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                        }
                        else
                        {
                            sql = "select daysbetween(cancellationdate, startdate) from c_contract where c_contract_id = " + cont.GetC_Contract_ID();
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                            //price = Decimal.Multiply(Decimal.Divide(cont.GetPriceEntered(), days.Value), diffDays.Value);
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            if (diffDays > 0)
                            {
                                sql = "update c_contractschedule set isactive = 'N' where c_contract_id = " + cont.GetC_Contract_ID();
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                            }
                            else
                            {
                                sql = "update c_contractschedule set isactive = 'N' where c_contract_id = " + cont.GetC_Contract_ID();
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                            }
                            // sql = "update c_contractschedule set isactive = 'N' where enddate > '" + System.DateTime.Now + "' and c_contract_id = " + cont.GetC_Contract_ID();

                        }
                    }
                }

                price = Decimal.Round(price.Value, 2, MidpointRounding.AwayFromZero);

                VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, null);
                inv.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                inv.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                inv.SetC_BPartner_ID(cont.GetC_BPartner_ID());
                if (Util.GetValueOfInt(cont.GetC_Order_ID()) != 0)
                {
                    inv.SetC_Order_ID(cont.GetC_Order_ID());
                }
                inv.SetC_DocType_ID(C_DocType_ID);
                inv.SetC_DocTypeTarget_ID(C_DocType_ID);
                inv.SetC_BPartner_Location_ID(cont.GetBill_Location_ID());
                inv.SetC_Currency_ID(cont.GetC_Currency_ID());
                inv.SetC_PaymentTerm_ID(cont.GetC_PaymentTerm_ID());
                inv.SetC_Campaign_ID(cont.GetC_Campaign_ID());
                inv.SetIsSOTrx(true);
                inv.SetM_PriceList_ID(cont.GetM_PriceList_ID());
                inv.SetSalesRep_ID(cont.GetSalesRep_ID());
                inv.SetC_Contract_ID(cont.GetC_Contract_ID());
                if (!inv.Save())
                {

                }

                if (inv.GetC_Invoice_ID() != 0)
                {
                    VAdvantage.Model.MInvoiceLine invLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, null);
                    invLine.SetAD_Client_ID(inv.GetAD_Client_ID());
                    invLine.SetAD_Org_ID(inv.GetAD_Org_ID());
                    invLine.SetC_Campaign_ID(inv.GetC_Campaign_ID());
                    invLine.SetC_Invoice_ID(inv.GetC_Invoice_ID());
                    invLine.SetC_UOM_ID(cont.GetC_UOM_ID());
                    invLine.SetM_Product_ID(cont.GetM_Product_ID());
                    if (Util.GetValueOfInt(cont.GetC_OrderLine_ID()) != 0)
                    {
                        invLine.SetC_OrderLine_ID(cont.GetC_OrderLine_ID());
                    }
                    invLine.SetC_Tax_ID(cont.GetC_Tax_ID());
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

                sql = "update c_invoice set c_contract_id = " + cont.GetC_Contract_ID() + " where c_invoice_id = " + inv.GetC_Invoice_ID();
                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));

                sql = "select sum(taxamt) from c_invoicetax where c_invoice_id = " + inv.GetC_Invoice_ID();
                Decimal? taxAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                sql = "update c_contractschedule set c_invoice_id = " + inv.GetC_Invoice_ID() + ", processed = 'Y' where c_contractschedule_id = " + contSchedule.GetC_ContractSchedule_ID();
                // sql = "update c_contractschedule set c_invoice_id = " + inv.GetC_Invoice_ID() + ", processed = 'Y', TotalAmt = " + inv.GetTotalLines() + ", taxamt = " + taxAmt + ", grandtotal = " + inv.GetGrandTotal() + " where c_contractschedule_id = " + contSchedule.GetC_ContractSchedule_ID();
                res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
            }
        }
    }
}
