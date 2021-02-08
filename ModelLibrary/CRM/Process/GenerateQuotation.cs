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
    public class GenerateQuotation  : SvrProcess
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

        protected override void Prepare()
        {
            _VAB_Project_ID = GetRecord_ID();
        }

        protected override string DoIt()
        {
            //Int32 value = 0;
            string msg = "";
            log.Info("VAB_Project_ID=" + _VAB_Project_ID);
            if (_VAB_Project_ID == 0)
            {
                throw new ArgumentException("VAB_Project_ID == 0");
            }

            VAdvantage.Model.MProject fromProject = new VAdvantage.Model.MProject(GetCtx(), _VAB_Project_ID, null);
            VAdvantage.Model.MVABOrder order = new VAdvantage.Model.MVABOrder(GetCtx(), 0, null);
            VAB_BusinessPartner_id = fromProject.GetVAB_BusinessPartner_ID();
            VAB_BPart_Location_id = fromProject.GetVAB_BPart_Location_ID();
            VAB_BusinessPartnerSR_ID = fromProject.GetVAB_BusinessPartnerSR_ID();
            //MBPartner bp = new MBPartner(GetCtx(), VAB_BusinessPartner_id, null);
            VAdvantage.Model.MVABBPartLocation bpartnerloc = new VAdvantage.Model.MVABBPartLocation(GetCtx(), VAB_BPart_Location_id, null);
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
                    //String sqlcust = "update VAB_BusinessPartner set iscustomer='Y', isprospect='N' where VAB_BusinessPartner_id=" + VAB_BusinessPartnerSR_ID + "";
                    //value = DB.ExecuteQuery(sqlcust, null, null);
                    //if (value == -1)
                    //{

                    //}
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
               // String sql = "select VAB_DocTypes_id from VAB_DocTypes where docbasetype= 'SOO' and  = 'Sales Quotation'";
                String sql = "select VAB_DocTypes_id from VAB_DocTypes where docbasetype = 'SOO' and docsubtypeso = 'ON' and isreturntrx = 'N' and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
                int Doctype_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql));
                int MPriceList_id = Util.GetValueOfInt(fromProject.GetVAM_PriceList_ID());
                order.SetVAM_PriceList_ID(MPriceList_id);
                ////String sqlmpricelist = "select VAM_PriceList_id from VAM_PriceList where name='Export'";
                ////int MPriceList_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlmpricelist));
                //if (MPriceList_id == order.GetVAM_PriceList_ID())
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
                    VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), VAB_BusinessPartner_id, null);
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
                        log.SaveError("CampaignIDNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "CampaignIDtNotSaved");
                    }
                }
                else
                {
                    VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), VAB_BusinessPartnerSR_ID, null);
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
                order.SetVAB_DocTypesTarget_ID(Doctype_id);
                order.SetIsSOTrx(true);
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
                    VAdvantage.Model.MVABOrderLine ol = new VAdvantage.Model.MVABOrderLine(order);
                    ol.SetLine(lines[i].GetLine());
                    ol.SetDescription(lines[i].GetDescription());
                    ol.SetVAM_Product_ID(lines[i].GetVAM_Product_ID(), true);
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

                fromProject.SetRef_Order_ID(order.GetVAB_Order_ID());
                fromProject.SetGenerate_Quotation("Y");
                if (!fromProject.Save())
                {
                    log.SaveError("ProjectNotSaved", "");
                    return Msg.GetMsg(GetCtx(), "ProjectNotSaved"); ;
                }
            }
            msg = Msg.GetMsg(GetCtx(), "QuotationGenerated");
            return msg;
        }
    }
}
