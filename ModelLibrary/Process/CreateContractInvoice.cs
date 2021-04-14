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
        VAdvantage.Model.X_C_Contract cont = null;
        VAdvantage.Model.X_C_ContractSchedule contSchedule = null;
        VAdvantage.Model.MInvoice inv = null;
        MBPartner bp = null;
        int _C_DocType_ID = 0;
        #endregion

        protected override void Prepare()
        {

        }

        /// <summary>
        /// Create AR and AP Invoices
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            int C_Contract_ID = Util.GetValueOfInt(GetRecord_ID());
            if (C_Contract_ID != 0)
            {
                cont = new VAdvantage.Model.X_C_Contract(GetCtx(), C_Contract_ID, Get_TrxName());
                bp = new MBPartner(GetCtx(), cont.GetC_BPartner_ID(), Get_TrxName());
                string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                int[] contSch = VAdvantage.Model.X_C_ContractSchedule.GetAllIDs("C_ContractSchedule", "C_Contract_ID = " + C_Contract_ID + " AND FROMDATE <= '" + date
                    + "' AND NVL(C_INVOICE_ID,0) = 0", Get_TrxName());
                //Neha----If Not found any Invoice Schedule against the Contract then it will display the error message--11 Sep,2018
                if (contSch != null && contSch.Length > 0)
                {
                    // Done by Rakesh Kumar on 01/Apr/2021
                    // Set DocTypeId
                    SetDocType();
                    for (int i = 0; i < contSch.Length; i++)
                    {
                        contSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), Get_TrxName());
                        GenerateInvoice(contSchedule);
                    }
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "VIS_NotFoundInvSchedule.");
                }

                sql.Clear();
                sql.Append("SELECT COUNT(C_ContractSchedule_ID) FROM C_ContractSchedule WHERE C_Contract_ID = " + C_Contract_ID + " AND NVL(C_INVOICE_ID,0) > 0");
                string sql1 = "UPDATE C_Contract SET InvoicesGenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName())) + " WHERE C_Contract_ID = " + C_Contract_ID;
                int res = DB.ExecuteQuery(sql1, null, Get_TrxName());
            }
            else
            {
                sql.Append("SELECT C_Contract_ID FROM C_Contract WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID());
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                    while (idr.Read())
                    {
                        cont = new VAdvantage.Model.X_C_Contract(GetCtx(), Util.GetValueOfInt(idr[0]), Get_TrxName());
                        string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
                        int[] contSch = VAdvantage.Model.X_C_ContractSchedule.GetAllIDs("C_ContractSchedule", "C_Contract_ID = " + cont.GetC_Contract_ID() + " AND FROMDATE <= '"
                            + date + "' AND NVL(C_INVOICE_ID,0) = 0", Get_TrxName());
                        if (contSch != null)
                        {
                            // Done by Rakesh Kumar on 01/Apr/2021
                            // Set DocTypeId
                            SetDocType();
                            for (int i = 0; i < contSch.Length; i++)
                            {
                                contSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), Util.GetValueOfInt(contSch[i]), Get_TrxName());
                                GenerateInvoice(contSchedule);
                            }
                        }

                        sql.Clear();
                        sql.Append("SELECT COUNT(C_ContractSchedule_ID) FROM C_ContractSchedule WHERE C_Contract_ID = " + cont.GetC_Contract_ID() + " AND NVL(C_INVOICE_ID,0) > 0");
                        string sql1 = "UPDATE C_Contract SET InvoicesGenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()))
                            + " WHERE C_Contract_ID = " + cont.GetC_Contract_ID();
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
        private void GenerateInvoice(VAdvantage.Model.X_C_ContractSchedule contSchedule)
        {
            if (contSchedule.IsActive())
            {
                int res = 0;
                // sql = "select noofdays from c_frequency where c_frequency_id = " + cont.GetC_Frequency_ID();
                //  Decimal? days = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                Decimal? price = null;
                if (!cont.IsCancel())
                {
                    price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                }
                else
                {
                    sql.Clear();
                    sql.Append("UPDATE C_Contract SET RenewalType = NULL WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                    int res2 = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));

                    if (contSchedule.GetEndDate() <= cont.GetCancellationDate())
                    {
                        price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                    }
                    else
                    {
                        sql.Clear();
                        sql.Append("SELECT MAX(C_ContractSchedule_ID) FROM C_ContractSchedule WHERE NVL(C_INVOICE_ID,0) > 0 AND C_Contract_ID = " + cont.GetC_Contract_ID());
                        int c_contractschedule_id = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                        if (c_contractschedule_id != 0)
                        {
                            string date = cont.GetCancellationDate().Value.ToString("dd-MMM-yyyy");

                            sql.Clear();
                            sql.Append("SELECT DaysBetween('" + date + "', EndDate) FROM C_ContractSchedule WHERE C_ContractSchedule_ID = " + c_contractschedule_id);
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            sql.Clear();
                            sql.Append("UPDATE C_ContractSchedule SET IsActive = 'N' WHERE EndDate > '" + date + "' AND C_Contract_ID = " + cont.GetC_Contract_ID());
                            res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                        }
                        else
                        {
                            sql.Clear();
                            sql.Append("SELECT DaysBetween(CancellationDate, StartDate) FROM C_Contract WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                            Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                            if (diffDays > 0)
                            {
                                sql.Clear();
                                sql.Append("UPDATE C_ContractSchedule SET IsActive = 'N' WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                            }
                            else
                            {
                                sql.Clear();
                                sql.Append("UPDATE C_ContractSchedule SET IsActive = 'N' WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                            }
                        }
                    }
                }

                price = Decimal.Round(price.Value, 2, MidpointRounding.AwayFromZero);

                inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, Get_TrxName());
                inv.SetAD_Client_ID(cont.GetAD_Client_ID());
                inv.SetAD_Org_ID(cont.GetAD_Org_ID());
                inv.SetC_BPartner_ID(cont.GetC_BPartner_ID());
                if (Util.GetValueOfInt(cont.GetC_Order_ID()) != 0)
                {
                    inv.SetC_Order_ID(cont.GetC_Order_ID());
                }
                // JID_0872: System has To pick the Payment Method defined with the Business Partner against whom the Invoice is getting generated.
                if (Env.IsModuleInstalled("VA009_"))
                {
                    // Done by Rakesh Kumar on 01/Apr/2021
                    // When ContractType is Accounts Receivable
                    if (bp.GetVA009_PaymentMethod_ID() > 0 && (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsReceivable)))
                    {
                        inv.SetVA009_PaymentMethod_ID(bp.GetVA009_PaymentMethod_ID());
                        inv.SetIsSOTrx(true);
                    }
                    else if (bp.GetVA009_PO_PaymentMethod_ID() > 0 && (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsPayable)))
                    {
                        // When ContractType is Accounts Payable
                        inv.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                        inv.SetIsSOTrx(false);
                    }
                    else
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "VIS_PaymentMethodNotDefined") + " : " + bp.GetName());
                    }
                }
                inv.SetC_DocType_ID(_C_DocType_ID);
                inv.SetC_DocTypeTarget_ID(_C_DocType_ID);
                inv.SetC_BPartner_Location_ID(cont.GetBill_Location_ID());
                inv.SetC_Currency_ID(cont.GetC_Currency_ID());
                // JID_1536_3
                inv.SetC_ConversionType_ID(cont.GetC_ConversionType_ID());
                inv.SetC_PaymentTerm_ID(cont.GetC_PaymentTerm_ID());
                inv.SetC_Campaign_ID(cont.GetC_Campaign_ID());

                inv.SetM_PriceList_ID(cont.GetM_PriceList_ID());
                inv.SetSalesRep_ID(cont.GetSalesRep_ID());
                inv.SetC_Contract_ID(cont.GetC_Contract_ID());
                // Done by rakesh kumar on 31/Mar/2021
                // When invoice created from service contract set ServiceContract always true
                inv.SetServiceContract(true);
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
                    invLine.SetAD_Client_ID(inv.GetAD_Client_ID());
                    invLine.SetAD_Org_ID(inv.GetAD_Org_ID());
                    invLine.SetC_Campaign_ID(inv.GetC_Campaign_ID());
                    invLine.SetC_Invoice_ID(inv.GetC_Invoice_ID());
                    invLine.SetC_UOM_ID(cont.GetC_UOM_ID());
                    invLine.SetM_Product_ID(cont.GetM_Product_ID());
                    // Added by Vivek on 21/11/2017 asigned by Pradeep
                    invLine.SetM_AttributeSetInstance_ID(cont.GetM_AttributeSetInstance_ID());
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
                    //Neha---Set C_Contract_ID on C_invoice table using MClass object--11 Sep,2018
                    inv.SetC_Contract_ID(cont.GetC_Contract_ID());
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
                //Neha---Set C_Contract_ID on C_invoice table using MClass object--11 Sep,2018

                //sql = "UPDATE c_invoice SET C_Contract_ID = " + cont.GetC_Contract_ID() + " WHERE c_invoice_id = " + inv.GetC_Invoice_ID();
                //res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));

                //Neha---taxAmt not used in this class----11 Sep,2018

                //sql = "SELECT SUM(taxamt) FROM c_invoicetax WHERE c_invoice_id = " + inv.GetC_Invoice_ID();
                //Decimal? taxAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                # endregion
                sql.Clear();
                sql.Append("UPDATE C_ContractSchedule SET C_Invoice_ID = " + inv.GetC_Invoice_ID() + ", Processed = 'Y' WHERE C_ContractSchedule_ID = " + contSchedule.GetC_ContractSchedule_ID());
                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                //Neha---Append Document No. in sb----12 Sep,2018
                sb.Append(inv.GetDocumentNo() + ", ");
                //_count++;
            }
        }
        /// <summary>
        /// Created By: Rakesh Kumar
        /// Created Date: 01/Apr/2021
        /// Purpose: Get DocType
        /// </summary>
        private void SetDocType()
        {
            sql.Clear();
            _C_DocType_ID = 0;
            sql.Append("SELECT C_DocType_ID FROM C_DocType "
                      + "WHERE AD_Client_ID=" + cont.GetAD_Client_ID() + " AND IsActive='Y' AND AD_Org_ID IN(0," + cont.GetAD_Org_ID() + ") ");

            // When ContractType is Accounts Receivable
            if (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsReceivable))
            {
                sql.Append(@" AND DocBaseType = '" + MDocBaseType.DOCBASETYPE_ARINVOICE + @"'");
            }
            else if (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsPayable))
            {
                sql.Append(@" AND DocBaseType = '" + MDocBaseType.DOCBASETYPE_APINVOICE + @"'");
            }
            sql.Append(" ORDER BY AD_Org_ID Desc");
            _C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(MRole.GetDefault(GetCtx()).AddAccessSQL(sql.ToString(), "C_DocType", true, true), null, Get_TrxName()));
           
            // If ContractType not defined on Service Contract Window
            if (_C_DocType_ID == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "ContractTypeNotDefined"));
            }
        }
    }
}
