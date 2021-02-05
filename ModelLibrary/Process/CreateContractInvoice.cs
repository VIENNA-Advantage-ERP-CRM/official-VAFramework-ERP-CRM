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
using VAdvantage.Model;
//using ViennaAdvantage.Model;

namespace ViennaAdvantageServer.Process
{
    public class CreateContractInvoice : SvrProcess
    {

        #region Private Variables
        StringBuilder sb = new StringBuilder("");
        StringBuilder sql = new StringBuilder();
        //int _count = 0;
        //Neha--Declare X-Class or MClass object publically for further use in class--11 Sep,2018
        VAdvantage.Model.X_VAB_Contract cont = null;
        VAdvantage.Model.X_VAB_ContractSchedule contSchedule = null;
        VAdvantage.Model.MInvoice inv = null;
        MVABBusinessPartner bp = null;
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
                cont = new VAdvantage.Model.X_VAB_Contract(GetCtx(), VAB_Contract_ID, Get_TrxName());
                bp = new MVABBusinessPartner(GetCtx(), cont.GetVAB_BusinessPartner_ID(), Get_TrxName());
                string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                int[] contSch = VAdvantage.Model.X_VAB_ContractSchedule.GetAllIDs("VAB_ContractSchedule", "VAB_Contract_ID = " + VAB_Contract_ID + " AND FROMDATE <= '" + date 
                    + "' AND NVL(VAB_INVOICE_ID,0) = 0", Get_TrxName());
                //Neha----If Not found any Invoice Schedule against the Contract then it will display the error message--11 Sep,2018
                if (contSch != null && contSch.Length > 0)
                {
                    for (int i = 0; i < contSch.Length; i++)
                    {
                        contSchedule = new VAdvantage.Model.X_VAB_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), Get_TrxName());
                        GenerateInvoice(contSchedule);
                    }
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "VIS_NotFoundInvSchedule.");
                }

                sql.Clear();
                sql.Append("SELECT COUNT(VAB_ContractSchedule_ID) FROM VAB_ContractSchedule WHERE VAB_Contract_ID = " + VAB_Contract_ID + " AND NVL(VAB_INVOICE_ID,0) > 0");
                string sql1 = "UPDATE VAB_Contract SET InvoicesGenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName())) + " WHERE VAB_Contract_ID = " + VAB_Contract_ID;
                int res = DB.ExecuteQuery(sql1, null, Get_TrxName());
            }
            else
            {
                sql.Append("SELECT VAB_Contract_ID FROM VAB_Contract WHERE IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                    while (idr.Read())
                    {
                        cont = new VAdvantage.Model.X_VAB_Contract(GetCtx(), Util.GetValueOfInt(idr[0]), Get_TrxName());
                        string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                        int[] contSch = VAdvantage.Model.X_VAB_ContractSchedule.GetAllIDs("VAB_ContractSchedule", "VAB_Contract_ID = " + cont.GetVAB_Contract_ID() + " AND FROMDATE <= '" 
                            + date + "' AND NVL(VAB_INVOICE_ID,0) = 0", Get_TrxName());
                        if (contSch != null)
                        {
                            for (int i = 0; i < contSch.Length; i++)
                            {
                                contSchedule = new VAdvantage.Model.X_VAB_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), Get_TrxName());
                                GenerateInvoice(contSchedule);
                            }
                        }
                        
                        sql.Clear();
                        sql.Append("SELECT COUNT(VAB_ContractSchedule_ID) FROM VAB_ContractSchedule WHERE VAB_Contract_ID = " + cont.GetVAB_Contract_ID() + " AND NVL(VAB_INVOICE_ID,0) > 0");
                        string sql1 = "UPDATE VAB_Contract SET InvoicesGenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()))
                            + " WHERE VAB_Contract_ID = " + cont.GetVAB_Contract_ID();
                        int res = DB.ExecuteQuery(sql1, null, Get_TrxName());
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
            //Neha--If Invoice generated sucessfully then it will return sb.Length greater then 0
            if (sb.Length > 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_InvGenerated") + sb.ToString().Substring(0, sb.Length - 2);
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "VIS_InvNotGenerated");
            }
        }

        /// <summary>
        /// Generate Contract Invoice
        /// </summary>
        /// <param name="contSchedule"></param>
        private void GenerateInvoice(VAdvantage.Model.X_VAB_ContractSchedule contSchedule)
        {
            if (contSchedule.IsActive())
            {
                int res = 0;
                sql.Clear();
                sql.Append(MVAFRole.GetDefault(GetCtx()).AddAccessSQL("SELECT MIN(VAB_DocTypes_ID) FROM VAB_DocTypes WHERE DOCBASETYPE='ARI' AND ISACTIVE ='Y'", "VAB_DocTypes", true, true));
                int VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                // sql = "select noofdays from VAB_Frequency where VAB_Frequency_id = " + cont.GetVAB_Frequency_ID();
                //  Decimal? days = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                Decimal? price = null;

                if (!cont.IsCancel())
                {
                    price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                }
                else
                {
                    sql.Clear();
                    sql.Append("UPDATE VAB_Contract SET RenewalType = NULL WHERE VAB_Contract_ID = " + cont.GetVAB_Contract_ID());
                    int res2 = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));

                    if (contSchedule.GetEndDate() <= cont.GetCancellationDate())
                    {
                        price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                    }
                    else
                    {
                        sql.Clear();
                        sql.Append("SELECT MAX(VAB_ContractSchedule_ID) FROM VAB_ContractSchedule WHERE NVL(VAB_INVOICE_ID,0) > 0 AND VAB_Contract_ID = " + cont.GetVAB_Contract_ID());
                        int VAB_Contractschedule_id = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                        if (VAB_Contractschedule_id != 0)
                        {
                            string date = cont.GetCancellationDate().Value.ToString("dd-MMM-yyyy");

                            sql.Clear();
                            sql.Append("SELECT DaysBetween('" + date + "', EndDate) FROM VAB_ContractSchedule WHERE VAB_ContractSchedule_ID = " + VAB_Contractschedule_id);
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            sql.Clear();
                            sql.Append("UPDATE VAB_ContractSchedule SET IsActive = 'N' WHERE EndDate > '" + date + "' AND VAB_Contract_ID = " + cont.GetVAB_Contract_ID());
                            res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                        }
                        else
                        {
                            sql.Clear();
                            sql.Append("SELECT DaysBetween(CancellationDate, StartDate) FROM VAB_Contract WHERE VAB_Contract_ID = " + cont.GetVAB_Contract_ID());
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            if (diffDays > 0)
                            {
                                sql.Clear();
                                sql.Append("UPDATE VAB_ContractSchedule SET IsActive = 'N' WHERE VAB_Contract_ID = " + cont.GetVAB_Contract_ID());
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                            }
                            else
                            {
                                sql.Clear();
                                sql.Append("UPDATE VAB_ContractSchedule SET IsActive = 'N' WHERE VAB_Contract_ID = " + cont.GetVAB_Contract_ID());
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                            }
                        }
                    }
                }

                price = Decimal.Round(price.Value, 2, MidpointRounding.AwayFromZero);

                inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, Get_TrxName());
                inv.SetVAF_Client_ID(cont.GetVAF_Client_ID());
                inv.SetVAF_Org_ID(cont.GetVAF_Org_ID());
                inv.SetVAB_BusinessPartner_ID(cont.GetVAB_BusinessPartner_ID());
                if (Util.GetValueOfInt(cont.GetVAB_Order_ID()) != 0)
                {
                    inv.SetVAB_Order_ID(cont.GetVAB_Order_ID());
                }
                // JID_0872: System has To pick the Payment Method defined with the Business Partner against whom the Invoice is getting generated.
                if (Env.IsModuleInstalled("VA009_"))
                {
                    if (bp.GetVA009_PaymentMethod_ID() > 0)
                    {
                        inv.SetVA009_PaymentMethod_ID(bp.GetVA009_PaymentMethod_ID());
                    }
                    else
                    {                        
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "VIS_PaymentMethodNotDefined") + " : " + bp.GetName());
                    }
                }
                inv.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                inv.SetVAB_DocTypesTarget_ID(VAB_DocTypes_ID);
                inv.SetVAB_BPart_Location_ID(cont.GetBill_Location_ID());
                inv.SetVAB_Currency_ID(cont.GetVAB_Currency_ID());
                // JID_1536_3
                inv.SetVAB_CurrencyType_ID(cont.GetVAB_CurrencyType_ID());
                inv.SetVAB_PaymentTerm_ID(cont.GetVAB_PaymentTerm_ID());
                inv.SetVAB_Promotion_ID(cont.GetVAB_Promotion_ID());
                inv.SetIsSOTrx(true);
                inv.SetVAM_PriceList_ID(cont.GetVAM_PriceList_ID());
                inv.SetSalesRep_ID(cont.GetSalesRep_ID());
                inv.SetVAB_Contract_ID(cont.GetVAB_Contract_ID());
                if (!inv.Save())
                {
                    //Neha----If Invoice not saved then will show the exception---11 Sep,2018
                    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                    if (pp != null)
                        throw new ArgumentException("Cannot save Invoice. " + pp.GetName());
                    throw new ArgumentException("Cannot save Invoice");
                }

                else
                {
                    VAdvantage.Model.MInvoiceLine invLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, Get_TrxName());
                    invLine.SetVAF_Client_ID(inv.GetVAF_Client_ID());
                    invLine.SetVAF_Org_ID(inv.GetVAF_Org_ID());
                    invLine.SetVAB_Promotion_ID(inv.GetVAB_Promotion_ID());
                    invLine.SetVAB_Invoice_ID(inv.GetVAB_Invoice_ID());
                    invLine.SetVAB_UOM_ID(cont.GetVAB_UOM_ID());
                    invLine.SetVAM_Product_ID(cont.GetVAM_Product_ID());
                    // Added by Vivek on 21/11/2017 asigned by Pradeep
                    invLine.SetVAM_PFeature_SetInstance_ID(cont.GetVAM_PFeature_SetInstance_ID());
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
                        //Neha----If Invoice Line not saved then will show the exception---11 Sep,2018
                        ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                        if (pp != null)
                            throw new ArgumentException("Cannot save Invoice Line. " + pp.GetName());
                        throw new ArgumentException("Cannot save Invoice Line");
                    }
                }

                string comp = inv.CompleteIt();
                //Neha---If Invoice Completed then Set Document Action, Document Status and Contract on Invoice Header---11 Sep,2018
                if (comp == "CO")
                {
                    inv.SetDocAction("CL");
                    inv.SetDocStatus("CO");
                    //Neha---Set VAB_Contract_ID on VAB_Invoice table using MClass object--11 Sep,2018
                    inv.SetVAB_Contract_ID(cont.GetVAB_Contract_ID());
                    if (!inv.Save())
                    {
                        //Neha----If Invoice not saved then will show the exception---11 Sep,2018
                        ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                        if (pp != null)
                            throw new ArgumentException("Cannot save Invoice. " + pp.GetName());
                        throw new ArgumentException("Cannot save Invoice");
                    }
                }
                else
                {
                    //Neha----If Invoice not completed then will show the exception---11 Sep,2018
                    if (inv.GetProcessMsg() != null)
                    {
                        throw new ArgumentException("Cannot complete the Invoice. " + inv.GetProcessMsg());
                    }
                    throw new ArgumentException("Cannot complete the Invoice");
                }

                #region Commented Code
                //Neha---Set VAB_Contract_ID on VAB_Invoice table using MClass object--11 Sep,2018

                //sql = "UPDATE VAB_Invoice SET VAB_Contract_ID = " + cont.GetVAB_Contract_ID() + " WHERE VAB_Invoice_id = " + inv.GetVAB_Invoice_ID();
                //res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));

                //Neha---taxAmt not used in this class----11 Sep,2018

                //sql = "SELECT SUM(taxamt) FROM VAB_Tax_Invoice WHERE VAB_Invoice_id = " + inv.GetVAB_Invoice_ID();
                //Decimal? taxAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                # endregion
                sql.Clear();
                sql.Append("UPDATE VAB_ContractSchedule SET VAB_Invoice_ID = " + inv.GetVAB_Invoice_ID() + ", Processed = 'Y' WHERE VAB_ContractSchedule_ID = " + contSchedule.GetVAB_ContractSchedule_ID());
                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                //Neha---Append Document No. in sb----12 Sep,2018
                sb.Append(inv.GetDocumentNo() + ", ");
                //_count++;
            }
        }
    }
}
