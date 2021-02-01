using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using ViennaAdvantage.Process;
//////using System.Windows.Forms;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class GenerateOrder : SvrProcess
    {
        #region Private Variable
        /**	Project         		*/
        private int _VAB_Project_ID = 0;
        /**BPartner Customer        */
        private int VAB_BusinessPartner_id = 0;
        /**BPartner Location        */
        private int VAB_BPart_Location_id = 0;
        /**BPartner Prospect        */
        private int VAB_BusinessPartnerSR_ID = 0;
        /*Order                   	*/
        //private int VAB_Order_ID = 0;
        /**ProjectLine       */
        private int VAB_ProjectLine_ID = 0;

        #endregion

        /**
	 *  Prepare - e.g., get Parameters.
	 */
        protected override void Prepare()
        {
            _VAB_Project_ID = GetRecord_ID();
        } //	prepare

        /** Generate Order  
        */
        protected override String DoIt()
        {

            Int32 value = 0;
            
            log.Info("VAB_Project_ID=" + _VAB_Project_ID);
            if (_VAB_Project_ID == 0)
            {
                throw new ArgumentException("VAB_Project_ID == 0");
            }
            VAdvantage.Model.MProject fromProject = new VAdvantage.Model.MProject(GetCtx(), _VAB_Project_ID, Get_TrxName());
            VAdvantage.Model.MOrder order = new VAdvantage.Model.MOrder(GetCtx(), 0, Get_TrxName());
            VAB_BusinessPartner_id = fromProject.GetVAB_BusinessPartner_ID();
            VAB_BPart_Location_id = fromProject.GetVAB_BPart_Location_ID();
            VAB_BusinessPartnerSR_ID = fromProject.GetVAB_BusinessPartnerSR_ID();
           // MBPartner bp = new MBPartner(GetCtx(), VAB_BusinessPartner_id, Get_TrxName());
            VAdvantage.Model.MVABBPartLocation bpartnerloc = new VAdvantage.Model.MVABBPartLocation(GetCtx(), VAB_BPart_Location_id, Get_TrxName());
            String currentdate = DateTime.Now.ToString();
            String sqlprjln = " select VAB_ProjectLine_ID from VAB_ProjectLine where VAB_Project_ID=" + _VAB_Project_ID + "";
            VAB_ProjectLine_ID = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlprjln));
            if (VAB_ProjectLine_ID != 0)
            {
                order.SetDateOrdered(Convert.ToDateTime(currentdate));
                order.SetDatePromised(Convert.ToDateTime(currentdate));
                if (VAB_BusinessPartner_id != 0)
                {
                    order.SetVAB_BusinessPartner_ID(fromProject.GetVAB_BusinessPartner_ID());
                    if (bpartnerloc.IsShipTo() == true)
                    {
                        order.SetVAB_BPart_Location_ID(fromProject.GetVAB_BPart_Location_ID());
                        order.SetVAF_UserContact_ID(fromProject.GetVAF_UserContact_ID());
                    }
                    if (bpartnerloc.IsBillTo() == true)
                    {
                        order.SetBill_Location_ID(fromProject.GetVAB_BPart_Location_ID());
                        order.SetBill_User_ID(fromProject.GetVAF_UserContact_ID());
                    }
                }
                if (VAB_BusinessPartnerSR_ID != 0)
                {
                    String sqlcust = "update VAB_BusinessPartner set iscustomer='Y', isprospect='N' where VAB_BusinessPartner_id=" + VAB_BusinessPartnerSR_ID + "";
                    value = DB.ExecuteQuery(sqlcust, null, Get_TrxName());
                    if (value == -1)
                    {

                    }
                    //bp.SetIsCustomer(true);
                    //bp.SetIsProspect(false);
                    order.SetVAB_BusinessPartner_ID(fromProject.GetVAB_BusinessPartnerSR_ID());

                    if (bpartnerloc.IsShipTo() == true)
                    {
                        order.SetVAB_BPart_Location_ID(fromProject.GetVAB_BPart_Location_ID());
                        order.SetVAF_UserContact_ID(fromProject.GetVAF_UserContact_ID());
                    }
                    if (bpartnerloc.IsBillTo() == true)
                    {
                        order.SetBill_Location_ID(fromProject.GetVAB_BPart_Location_ID());
                        order.SetBill_User_ID(fromProject.GetVAF_UserContact_ID());
                    }
                }
               // String sql = "select VAB_DocTypes_id from VAB_DocTypes where name = 'Standard Order'";

                String sql = "select VAB_DocTypes_id from VAB_DocTypes where docbasetype = 'SOO' and docsubtypeso = 'SO' and isreturntrx = 'N' and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
                int Doctype_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql));
                order.SetM_PriceList_ID(Util.GetValueOfInt(fromProject.GetM_PriceList_ID()));
                //String sqlmpricelist = "select m_pricelist_id from m_pricelist where name='Export'";
                //int MPriceList_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlmpricelist));
                //if (MPriceList_id == order.GetM_PriceList_ID())
                //{
                //    String sqlconversiontype = "select VAB_CurrencyType_id from VAB_CurrencyType where value = 'C'";
                //    int VAB_CurrencyType_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlconversiontype));
                //    order.SetVAB_CurrencyType_ID(VAB_CurrencyType_id);
                //}
                order.SetVAB_Project_ID(GetRecord_ID());
                order.SetSalesRep_ID(fromProject.GetSalesRep_ID());
                order.SetVAB_Currency_ID(fromProject.GetVAB_Currency_ID());
                if (VAB_BusinessPartner_id != 0)
                {
                    VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), VAB_BusinessPartner_id, Get_TrxName());
                    bp.SetVAB_Promotion_ID(fromProject.GetVAB_Promotion_ID());
                    bp.SetVAF_Client_ID(fromProject.GetVAF_Client_ID());
                    bp.SetVAF_Org_ID(fromProject.GetVAF_Org_ID());
                    if (bp.GetVAB_PaymentTerm_ID() != 0)
                    {
                        order.SetPaymentMethod(bp.GetPaymentRule());
                        order.SetVAB_PaymentTerm_ID(bp.GetVAB_PaymentTerm_ID());

                    }

                    if (!bp.Save())
                    {
                        log.SaveError("CampaignIDtNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "CampaignIDtNotSaved");
                    }
                }
                else
                {
                    VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), VAB_BusinessPartnerSR_ID, Get_TrxName());
                    bp.SetVAB_Promotion_ID(fromProject.GetVAB_Promotion_ID());
                    bp.SetVAF_Client_ID(fromProject.GetVAF_Client_ID());
                    bp.SetVAF_Org_ID(fromProject.GetVAF_Org_ID());
                    if (bp.GetVAB_PaymentTerm_ID() != 0)
                    {
                        order.SetPaymentMethod(bp.GetPaymentRule());
                        order.SetVAB_PaymentTerm_ID(bp.GetVAB_PaymentTerm_ID());

                    }

                    if (!bp.Save())
                    {
                        log.SaveError("CampaignIDtNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "CampaignIDtNotSaved");
                    }
                }
                //if (bp.GetVAB_PaymentTerm_ID() != 0)
                //{
                //    order.SetPaymentMethod(bp.GetPaymentRule());
                //    order.SetVAB_PaymentTerm_ID(bp.GetVAB_PaymentTerm_ID());

                //}
                order.SetFreightCostRule("I");
                order.SetVAB_Promotion_ID(fromProject.GetVAB_Promotion_ID());
                order.SetDocStatus("IP");
                order.SetVAB_DocTypes_ID(Doctype_id);
                order.SetIsSOTrx(true);
                order.SetVAB_DocTypesTarget_ID(Doctype_id);
                order.SetPriorityRule("5");
                order.SetFreightCostRule("I");
                if (!order.Save())
                {
                    log.SaveError("SaleOrdertNotSaved", "");
                    return Msg.GetMsg(GetCtx(), "SaleOrdertNotSaved");
                }
                //Order Lines
                int count = 0;
                VAdvantage.Model.MProjectLine[] lines = fromProject.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    VAdvantage.Model.MOrderLine ol = new VAdvantage.Model.MOrderLine(order);
                    ol.SetLine(lines[i].GetLine());
                    ol.SetDescription(lines[i].GetDescription());
                    ol.SetM_Product_ID(lines[i].GetM_Product_ID(), true);
                    ol.SetQtyEntered(lines[i].GetPlannedQty());
                    ol.SetQtyOrdered(lines[i].GetPlannedQty());
                    ol.SetPriceEntered(lines[i].GetPlannedPrice());
                    ol.SetPriceActual(lines[i].GetPlannedPrice());
                    ol.SetPriceList(lines[i].GetPriceList());
                    if (ol.Save())
                    {
                        count++;
                    }
                }
                fromProject.SetVAB_Order_ID(order.GetVAB_Order_ID());
                fromProject.SetVAB_BusinessPartner_ID(fromProject.GetVAB_BusinessPartnerSR_ID());
                fromProject.SetVAB_BusinessPartnerSR_ID(0);
                fromProject.SetGenerate_Order("Y");

                if (!fromProject.Save())
                {
                    log.SaveError("ProjectNotSaved", "");
                    return Msg.GetMsg(GetCtx(), "ProjectNotSaved");

                }
            }
            return Msg.GetMsg(GetCtx(),"OrderGenerationDone");
              
        }


        
    }
}
