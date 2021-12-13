/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CopyOrder
 * Purpose        : Copy Order and optionally close
 * Class Used     : SvrProcess
 * Chronological    Development
 * Raghunandan     03-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
//using ViennaAdvantage.Process;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
////using System.Windows.Forms;
////using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;

namespace ViennaAdvantage.Process
{
    public class CopyOrder : SvrProcess
    {
        #region Private Variable
        //Order to Copy				
        private int _C_Order_ID = 0;
        // Document Type of new Order	
        private int _C_DocType_ID = 0;
        // New Doc Date				
        private DateTime? _DateDoc = null;
        //Close/Process Old Order		
        private bool _IsCloseDocument = false;
        int newid = 0;
        int neworg_id = 0;
        StringBuilder docNo = new StringBuilder();
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID = Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("DateDoc"))
                {
                    _DateDoc = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("IsCloseDocument"))
                {
                    _IsCloseDocument = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            log.Info("C_Order_ID=" + _C_Order_ID
                + ", C_DocType_ID=" + _C_DocType_ID
                + ", CloseDocument=" + _IsCloseDocument);
            if (_C_Order_ID == 0)
            {
                throw new ArgumentException("No Order");
            }
            VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), _C_DocType_ID);
            if (dt.Get_ID() == 0)
            {
                throw new ArgumentException("No DocType");
            }
            if (_DateDoc == null)
            {
                _DateDoc = Util.GetValueOfDateTime(DateTime.Now);
            }
            //
            VAdvantage.Model.MOrder from = new VAdvantage.Model.MOrder(GetCtx(), _C_Order_ID, Get_Trx());
            if (from.GetDocStatus() != "DR" && from.GetDocStatus() != "IP" && from.GetDocStatus() != "CO")
            {
                throw new Exception("Order Closed");
            }

            //Develop by Deekshant For check VA077 Module For spilt the Sales Order
            docNo.Clear();
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                //Check Destination Organization in c_orderline
                string str = "SELECT DISTINCT(VA077_DestinationOrg), AD_Org_Id FROM C_OrderLine WHERE C_Order_ID=" + _C_Order_ID;
                DataSet dts = DB.ExecuteDataset(str, null, Get_Trx());
                if (dts != null && dts.Tables[0].Rows.Count > 0)
                {
                    bool headerOrgCreated = false;
                    for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
                    {
                        int destinationorg = Util.GetValueOfInt(dts.Tables[0].Rows[i]["VA077_DestinationOrg"]);
                        // VAdvantage.Model.MOrder newOrder = new VAdvantage.Model.MOrder(GetCtx(), 0, Get_Trx());
                        int orgId = Util.GetValueOfInt(dts.Tables[0].Rows[i]["AD_Org_Id"]);

                        //Handel, if SO already created with header org then skip.
                        if (headerOrgCreated && (destinationorg == 0 || destinationorg == orgId))
                            continue;

                        if (i > 0)
                            docNo.Append(", ");
                        AddHeader(destinationorg, orgId);
                        Addline(destinationorg, orgId);

                        //make flag true if desination org is zero or parent org.
                        if (destinationorg == 0 || destinationorg == orgId)
                            headerOrgCreated = true;
                    }
                }

            }
            else
            {
                //JID_1799 fromCreateSo is true if DOCBASETYPE='BOO'
                VAdvantage.Model.MOrder newOrder = VAdvantage.Model.MOrder.CopyFrom(from, _DateDoc, dt.GetC_DocType_ID(), false, true, null,
                dt.GetDocBaseType().Equals(MDocBaseType.DOCBASETYPE_BLANKETSALESORDER) ? true : false);     //	copy ASI 

                newOrder.SetC_DocTypeTarget_ID(_C_DocType_ID);
                int C_Bpartner_ID = newOrder.GetC_BPartner_ID();
                newOrder.Set_Value("IsSalesQuotation", false);

                // Added by Bharat on 05 Jan 2018 to set Values on Blanket Sales Order from Sales Quotation.
                if (dt.GetDocBaseType() == "BOO")
                {
                    newOrder.Set_Value("IsBlanketTrx", true);
                }
                else   // Added by Bharat on 29 March 2018 to set Blanket Order zero in case of Sales order Creation.
                {
                    newOrder.SetC_Order_Blanket(0);
                }
                if (newOrder.Get_ColumnIndex("C_Order_Quotation") > 0)
                    newOrder.SetC_Order_Quotation(_C_Order_ID);

                //Update New Order Refrence From Sales Qutation in Sales order
                newOrder.SetPOReference(Util.GetValueOfString(from.GetDocumentNo()));

                // Added by Bharat on 31 Jan 2018 to set Inco Term from Quotation

                if (newOrder.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    newOrder.SetC_IncoTerm_ID(from.GetC_IncoTerm_ID());
                }

                String sqlbp = "update c_project set c_bpartner_id=" + C_Bpartner_ID + "  where ref_order_id=" + _C_Order_ID + "";
                int value = DB.ExecuteQuery(sqlbp, null, Get_Trx());
                bool OK = newOrder.Save();
                if (!OK)
                {
                    //return GetReterivedError( newOrder,  "Could not create new Order"); 
                    throw new Exception("Could not create new Order");
                }
                if (OK)
                {
                    string sql = "select C_Project_id from c_project where c_order_id = " + from.GetC_Order_ID();
                    int C_Project_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (C_Project_ID != 0)
                    {
                        VAdvantage.Model.X_C_Project project = new VAdvantage.Model.X_C_Project(GetCtx(), C_Project_ID, Get_Trx());
                        project.SetC_BPartner_ID(project.GetC_BPartnerSR_ID());
                        project.SetC_BPartnerSR_ID(0);
                        if (!project.Save())
                        {
                            log.SaveError("Error on " + project.Get_TableName(), "");
                        }
                    }
                    if (dt.GetDocBaseType() == "BOO")
                    {
                        from.SetC_Order_Blanket(newOrder.GetC_Order_ID());
                    }
                    else
                    {
                        from.SetRef_Order_ID(newOrder.GetC_Order_ID());
                    }
                    from.Save();
                    int bp = newOrder.GetC_BPartner_ID();
                    VAdvantage.Model.X_C_BPartner prosp = new VAdvantage.Model.X_C_BPartner(GetCtx(), bp, Get_Trx());
                    prosp.SetIsCustomer(true);
                    prosp.SetIsProspect(false);
                    if (!prosp.Save())
                    {

                        log.SaveError("Error on " + prosp.Get_TableName(), "");
                    }
                }

                //
                if (_IsCloseDocument)
                {
                    VAdvantage.Model.MOrder original = new VAdvantage.Model.MOrder(GetCtx(), _C_Order_ID, Get_Trx());
                    //Edited by Arpit Rai on 8th of Nov,2017
                    if (original.GetDocStatus() != "CO") //to check if document is already completed
                    {
                        original.SetDocAction(VAdvantage.Model.MOrder.DOCACTION_Complete);
                        original.ProcessIt(VAdvantage.Model.MOrder.DOCACTION_Complete);
                        original.Save();
                    }
                    //Arpit
                    original.SetDocAction(VAdvantage.Model.MOrder.DOCACTION_Close);
                    original.ProcessIt(VAdvantage.Model.MOrder.DOCACTION_Close);
                    original.Save();
                }
                docNo.Append(newOrder.GetDocumentNo());
            }
            //+ ": " + newOrder.GetDocumentNo()
            return Msg.GetMsg(GetCtx(), "OrderCreatedSuccessfully") + " - " + dt.GetName() + ": " + docNo.ToString();
        }
        /// <summary>Add Sales Order Header for VA077 Module</summary>
        /// <param name="destinationorg">Destination Orgnaization id</param>
        /// <param name="orgId">Organization Id</param>
        /// <returns>bool</returns>
        public bool AddHeader(int destinationorg, int orgId)
        {
            VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), _C_DocType_ID);
            MOrder newOrder = new VAdvantage.Model.MOrder(GetCtx(), 0, Get_Trx());
            VAdvantage.Model.MOrder morder = new VAdvantage.Model.MOrder(GetCtx(), _C_Order_ID, Get_Trx());
            newOrder.SetAD_Client_ID(GetAD_Client_ID());
            if (destinationorg != 0)
            {
                newOrder.SetAD_Org_ID(destinationorg);
            }
            else
            {
                newOrder.SetAD_Org_ID(orgId);
            }
            newOrder.SetC_BPartner_ID(morder.GetC_BPartner_ID());
            newOrder.SetC_BPartner_Location_ID(morder.GetC_BPartner_Location_ID());
            newOrder.SetAD_User_ID(morder.GetAD_User_ID());
            newOrder.SetBill_BPartner_ID(morder.GetBill_BPartner_ID());
            newOrder.SetAD_OrgTrx_ID(morder.GetAD_OrgTrx_ID());
            newOrder.SetBill_Location_ID(morder.GetBill_Location_ID());
            newOrder.SetBill_User_ID(morder.GetBill_User_ID());
            newOrder.SetM_PriceList_ID(morder.GetM_PriceList_ID());
            newOrder.SetC_Currency_ID(morder.GetC_Currency_ID());
            newOrder.SetC_ConversionType_ID(morder.GetC_ConversionType_ID());
            newOrder.SetSalesRep_ID(morder.GetSalesRep_ID());
            newOrder.SetDescription(morder.GetDescription());
            //newOrder.SetM_Warehouse_ID(morder.GetM_Warehouse_ID());
            newOrder.SetC_PaymentTerm_ID(morder.GetC_PaymentTerm_ID());
            newOrder.SetC_Payment_ID(morder.GetC_Payment_ID());
            newOrder.SetVA009_PaymentMethod_ID(morder.GetVA009_PaymentMethod_ID());
            newOrder.SetC_IncoTerm_ID(morder.GetC_IncoTerm_ID());
            newOrder.SetC_Campaign_ID(morder.GetC_Campaign_ID());
            newOrder.SetC_ProjectRef_ID(morder.GetC_ProjectRef_ID());
            newOrder.SetGrandTotal(morder.GetGrandTotal());
            newOrder.SetTotalLines(morder.GetTotalLines());
            newOrder.Set_Value("VA077_HistoricContractDate", morder.Get_Value("VA077_HistoricContractDate"));
            newOrder.Set_Value("VA077_ChangeStartDate", morder.Get_Value("VA077_ChangeStartDate"));
            newOrder.Set_Value("VA077_ContractCPStartDate", morder.Get_Value("VA077_ContractCPStartDate"));
            newOrder.Set_Value("VA077_ContractCPEndDate", morder.Get_Value("VA077_ContractCPEndDate"));
            newOrder.Set_Value("VA077_PartialAmtCatchUp", morder.Get_Value("VA077_PartialAmtCatchUp"));
            newOrder.Set_Value("VA077_PartialAmtCatchUp", morder.Get_Value("VA077_PartialAmtCatchUp"));
            newOrder.Set_Value("VA077_OldAnnualContractTotal", morder.Get_Value("VA077_OldAnnualContractTotal"));
            newOrder.Set_Value("VA077_AdditionalAnnualCharge", morder.Get_Value("VA077_AdditionalAnnualCharge"));
            newOrder.Set_Value("VA077_NewAnnualContractTotal", morder.Get_Value("VA077_NewAnnualContractTotal"));
            newOrder.Set_Value("VA077_OrderRef", morder.Get_Value("VA077_OrderRef"));
            newOrder.Set_Value("VA077_SalesCoWorkerPer", morder.Get_Value("VA077_SalesCoWorkerPer"));
            newOrder.Set_Value("VA077_TotalMarginAmt", morder.Get_Value("VA077_TotalMarginAmt"));
            newOrder.Set_Value("VA077_SalesCoWorker", morder.Get_Value("VA077_SalesCoWorker"));
            newOrder.Set_Value("VA077_TotalPurchaseAmt", morder.Get_Value("VA077_TotalPurchaseAmt"));
            newOrder.Set_Value("VA077_TotalSalesAmt", morder.Get_Value("VA077_TotalSalesAmt"));
            newOrder.Set_Value("VA077_MarginPercent", morder.Get_Value("VA077_MarginPercent"));
            newOrder.Set_Value("VA077_IsLegalEntity", morder.Get_Value("VA077_IsLegalEntity"));
            newOrder.Set_Value("VA077_IsContract", morder.Get_Value("VA077_IsContract"));
            newOrder.SetC_DocTypeTarget_ID(_C_DocType_ID);
            int C_Bpartner_ID = newOrder.GetC_BPartner_ID();
            newOrder.Set_Value("IsSalesQuotation", false);

            // Added by Bharat on 05 Jan 2018 to set Values on Blanket Sales Order from Sales Quotation.
            if (dt.GetDocBaseType() == "BOO")
            {
                newOrder.Set_Value("IsBlanketTrx", true);
            }
            else   // Added by Bharat on 29 March 2018 to set Blanket Order zero in case of Sales order Creation.
            {
                newOrder.SetC_Order_Blanket(0);
            }
            if (newOrder.Get_ColumnIndex("C_Order_Quotation") > 0)
                newOrder.SetC_Order_Quotation(_C_Order_ID);

            //Update New Order Refrence From Sales Qutation in Sales order
            newOrder.SetPOReference(Util.GetValueOfString(morder.GetDocumentNo()));

            // Added by Bharat on 31 Jan 2018 to set Inco Term from Quotation

            if (newOrder.Get_ColumnIndex("C_IncoTerm_ID") > 0)
            {
                newOrder.SetC_IncoTerm_ID(morder.GetC_IncoTerm_ID());
            }

            String sqlbp = "update c_project set c_bpartner_id=" + C_Bpartner_ID + "  where ref_order_id=" + _C_Order_ID + "";
            int value = DB.ExecuteQuery(sqlbp, null, Get_Trx());
            bool OK = newOrder.Save();
            if (!OK)
            {
                //return GetReterivedError( newOrder,  "Could not create new Order"); 
                throw new Exception("Could not create new Order");
            }
            if (OK)
            {
                string sql = "select C_Project_id from c_project where c_order_id = " + morder.GetC_Order_ID();
                int C_Project_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (C_Project_ID != 0)
                {
                    VAdvantage.Model.X_C_Project project = new VAdvantage.Model.X_C_Project(GetCtx(), C_Project_ID, Get_Trx());
                    project.SetC_BPartner_ID(project.GetC_BPartnerSR_ID());
                    project.SetC_BPartnerSR_ID(0);
                    if (!project.Save())
                    {
                        Get_Trx().Rollback();
                        log.SaveError("Error on " + project.Get_TableName(), "");
                    }
                }
                if (dt.GetDocBaseType() == "BOO")
                {
                    morder.SetC_Order_Blanket(newOrder.GetC_Order_ID());
                }
                else
                {
                    morder.SetRef_Order_ID(newOrder.GetC_Order_ID());
                }
                newid = newOrder.GetC_Order_ID();
                neworg_id = newOrder.GetAD_Org_ID();
                morder.Save();
                int bp = newOrder.GetC_BPartner_ID();
                VAdvantage.Model.X_C_BPartner prosp = new VAdvantage.Model.X_C_BPartner(GetCtx(), bp, Get_Trx());
                prosp.SetIsCustomer(true);
                prosp.SetIsProspect(false);
                if (!prosp.Save())
                {
                    Get_Trx().Rollback();
                    log.SaveError("Error on " + prosp.Get_TableName(), "");
                }
            }

            //
            if (_IsCloseDocument)
            {
                VAdvantage.Model.MOrder original = new VAdvantage.Model.MOrder(GetCtx(), _C_Order_ID, Get_Trx());
                //Edited by Arpit Rai on 8th of Nov,2017
                if (original.GetDocStatus() != "CO") //to check if document is already completed
                {
                    original.SetDocAction(VAdvantage.Model.MOrder.DOCACTION_Complete);
                    original.ProcessIt(VAdvantage.Model.MOrder.DOCACTION_Complete);
                    original.Save();
                }
                //Arpit
                original.SetDocAction(VAdvantage.Model.MOrder.DOCACTION_Close);
                original.ProcessIt(VAdvantage.Model.MOrder.DOCACTION_Close);
                original.Save();
            }

            docNo.Append(newOrder.GetDocumentNo());
            return true;
        }
        /// <summary>
        /// Add  Order Line for sales order
        /// </summary>
        /// <param name="destinationorg">Destination Organization id</param>
        /// <param name="org">Organization</param>
        /// <returns>bool</returns>
        public bool Addline(int destinationorg, int org)
        {
            string str = null;
            VAdvantage.Model.MOrderLine orderLine = null;
            VAdvantage.Model.MOrderLine mOrderLine = null;
            if (destinationorg != 0)
            {
                str = "SELECT VA077_DestinationOrg,C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID=" + _C_Order_ID + " AND VA077_DestinationOrg=" + destinationorg + " ORDER BY Line";
            }
            else
            {

                str = "SELECT VA077_DestinationOrg,C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID = " + _C_Order_ID + " AND ((NVL(VA077_DestinationOrg,0)=0) OR VA077_DestinationOrg=AD_Org_ID) AND AD_Org_ID=" + org + " ORDER BY Line";
            }
            DataSet st = DB.ExecuteDataset(str, null, Get_Trx());
            if (st != null && st.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < st.Tables[0].Rows.Count; i++)
                {
                    int destinationorgid = Util.GetValueOfInt(st.Tables[0].Rows[i]["VA077_DestinationOrg"]);
                    orderLine = new VAdvantage.Model.MOrderLine(GetCtx(), 0, Get_Trx());
                    mOrderLine = new VAdvantage.Model.MOrderLine(GetCtx(), Util.GetValueOfInt(st.Tables[0].Rows[i]["C_OrderLine_ID"]), Get_Trx());
                    if (destinationorg != 0)
                    {
                        orderLine.SetAD_Org_ID(destinationorg);
                    }
                    else
                    {
                        orderLine.SetAD_Org_ID(org);
                    }
                    
                    orderLine.SetAD_Client_ID(GetAD_Client_ID());
                    orderLine.SetC_Order_ID(newid);
                    orderLine.SetDescription(mOrderLine.GetDescription());
                    orderLine.SetM_Product_ID(mOrderLine.GetM_Product_ID());
                    orderLine.SetM_AttributeSetInstance_ID(mOrderLine.GetM_AttributeSetInstance_ID());
                    orderLine.SetC_Charge_ID(mOrderLine.GetC_Charge_ID());
                    orderLine.SetQtyEntered(mOrderLine.GetQtyEntered());
                    orderLine.SetC_UOM_ID(mOrderLine.GetC_UOM_ID());
                    orderLine.SetQtyOrdered(mOrderLine.GetQtyOrdered());
                    orderLine.SetPriceEntered(mOrderLine.GetPriceEntered());
                    orderLine.SetPriceActual(mOrderLine.GetPriceActual());
                    orderLine.SetPriceList(mOrderLine.GetPriceList());
                    orderLine.SetDiscount(mOrderLine.GetDiscount());
                    orderLine.SetC_Tax_ID(mOrderLine.GetC_Tax_ID());
                    if (orderLine.Get_ColumnIndex("C_TaxExemptReason_ID") > -1 && orderLine.Get_ColumnIndex("IsTaxExempt") > -1)
                    {
                        //1052-- set IsTaxExempt and Tax Exempt Reason
                        orderLine.SetC_TaxExemptReason_ID(mOrderLine.GetC_TaxExemptReason_ID());
                        orderLine.SetIsTaxExempt(mOrderLine.IsTaxExempt());
                    }
                    orderLine.SetTaxAmt(mOrderLine.GetTaxAmt());
                    orderLine.SetSurchargeAmt(mOrderLine.GetSurchargeAmt());
                    orderLine.SetC_ProjectTask_ID(mOrderLine.GetC_ProjectTask_ID());
                    orderLine.SetC_Campaign_ID(mOrderLine.GetC_Campaign_ID());
                    orderLine.SetLineNetAmt(mOrderLine.GetLineNetAmt());
                    orderLine.SetLineTotalAmt(mOrderLine.GetLineTotalAmt());
                    orderLine.Set_Value("VA077_SNErweiterbar", mOrderLine.Get_Value("VA077_SNErweiterbar"));
                    orderLine.Set_Value("VA077_SerialNo", mOrderLine.Get_Value("VA077_SerialNo"));
                    orderLine.Set_Value("VA077_ShowOldSN", mOrderLine.Get_Value("VA077_ShowOldSN"));
                    orderLine.Set_Value("VA077_OldSN", mOrderLine.Get_Value("VA077_OldSN"));
                    orderLine.Set_Value("VA077_ContractProduct", mOrderLine.Get_Value("VA077_ContractProduct"));
                    orderLine.Set_Value("VA077_UserRef_ID", mOrderLine.Get_Value("VA077_UserRef_ID"));
                    orderLine.Set_Value("VA077_StartDate", mOrderLine.Get_Value("VA077_StartDate"));
                    orderLine.Set_Value("VA077_EndDate", mOrderLine.Get_Value("VA077_EndDate"));
                    orderLine.Set_Value("VA077_Duration", mOrderLine.Get_Value("VA077_Duration"));
                    orderLine.Set_Value("VA077_ShowWISYCN", mOrderLine.Get_Value("VA077_ShowWISYCN"));
                    orderLine.Set_Value("VA077_IsOfferApproved", mOrderLine.Get_Value("VA077_IsOfferApproved"));
                    orderLine.Set_Value("VA077_OfferApprovedOn", mOrderLine.Get_Value("VA077_OfferApprovedOn"));
                    orderLine.Set_Value("AD_User_ID", mOrderLine.Get_Value("AD_User_ID"));
                    orderLine.Set_Value("VA077_ShowCNAutodesk", mOrderLine.Get_Value("VA077_ShowCNAutodesk"));
                    orderLine.Set_Value("VA077_CNAutodesk", mOrderLine.Get_Value("VA077_CNAutodesk"));
                    orderLine.Set_Value("VA077_UpdateFromVersn", mOrderLine.Get_Value("VA077_UpdateFromVersn"));
                    orderLine.Set_Value("VA077_ServiceContract_ID", mOrderLine.Get_Value("VA077_ServiceContract_ID"));
                    orderLine.Set_Value("VA077_MarginPercent", mOrderLine.Get_Value("VA077_MarginPercent"));
                    orderLine.Set_Value("VA077_MarginAmt", mOrderLine.Get_Value("VA077_MarginAmt"));
                    orderLine.Set_Value("VA077_PurchasePrice", mOrderLine.Get_Value("VA077_PurchasePrice"));
                    orderLine.Set_Value("VA077_RegEmail", mOrderLine.Get_Value("VA077_RegEmail"));
                    orderLine.Set_Value("VA077_ProductInfo", mOrderLine.Get_Value("VA077_ProductInfo"));
                    orderLine.Set_Value("VA077_IsContract", mOrderLine.Get_Value("VA077_IsContract"));
                    orderLine.Set_Value("VA077_StartDate", mOrderLine.Get_Value("VA077_StartDate"));
                    orderLine.Set_Value("VA077_EndDate", mOrderLine.Get_Value("VA077_EndDate"));

                    // Set Quotation Ref on Line
                    // Added by Bharat on 06 Jan 2018 to set Values on Sales Order from Sales Quotation.
                    if (orderLine.Get_ColumnIndex("C_Quotation_Line_ID") >= 0)
                        orderLine.Set_Value("C_Quotation_Line_ID", mOrderLine.GetC_OrderLine_ID());
                    // Added by Bharat on 06 Jan 2018 to set Values on Sales Order from Sales Quotation.
                    if (orderLine.Get_ColumnIndex("C_Order_Quotation") >= 0)
                        orderLine.Set_Value("C_Order_Quotation", mOrderLine.GetC_Order_ID());
                                        
                    orderLine.SetLine((i + 1) * 10);
                    
                    if (!orderLine.Save())
                    {
                        Get_Trx().Rollback();
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
