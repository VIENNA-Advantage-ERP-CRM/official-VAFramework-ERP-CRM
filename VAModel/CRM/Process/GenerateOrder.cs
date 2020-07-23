using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using ViennaAdvantage.Process;
using System.Windows.Forms;
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
        private int _C_Project_ID = 0;
        /**BPartner Customer        */
        private int C_Bpartner_id = 0;
        /**BPartner Location        */
        private int C_Bpartner_Location_id = 0;
        /**BPartner Prospect        */
        private int C_BPartnerSR_ID = 0;
        /*Order                   	*/
        //private int C_Order_ID = 0;
        /**ProjectLine       */
        private int C_ProjectLine_ID = 0;

        #endregion

        /**
	 *  Prepare - e.g., get Parameters.
	 */
        protected override void Prepare()
        {
            _C_Project_ID = GetRecord_ID();
        } //	prepare

        /** Generate Order  
        */
        protected override String DoIt()
        {

            Int32 value = 0;
            
            log.Info("C_Project_ID=" + _C_Project_ID);
            if (_C_Project_ID == 0)
            {
                throw new ArgumentException("C_Project_ID == 0");
            }
            VAdvantage.Model.MProject fromProject = new VAdvantage.Model.MProject(GetCtx(), _C_Project_ID, Get_TrxName());
            VAdvantage.Model.MOrder order = new VAdvantage.Model.MOrder(GetCtx(), 0, Get_TrxName());
            C_Bpartner_id = fromProject.GetC_BPartner_ID();
            C_Bpartner_Location_id = fromProject.GetC_BPartner_Location_ID();
            C_BPartnerSR_ID = fromProject.GetC_BPartnerSR_ID();
           // MBPartner bp = new MBPartner(GetCtx(), C_Bpartner_id, Get_TrxName());
            VAdvantage.Model.MBPartnerLocation bpartnerloc = new VAdvantage.Model.MBPartnerLocation(GetCtx(), C_Bpartner_Location_id, Get_TrxName());
            String currentdate = DateTime.Now.ToString();
            String sqlprjln = " select c_projectline_id from c_projectline where c_project_id=" + _C_Project_ID + "";
            C_ProjectLine_ID = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlprjln));
            if (C_ProjectLine_ID != 0)
            {
                order.SetDateOrdered(Convert.ToDateTime(currentdate));
                order.SetDatePromised(Convert.ToDateTime(currentdate));
                if (C_Bpartner_id != 0)
                {
                    order.SetC_BPartner_ID(fromProject.GetC_BPartner_ID());
                    if (bpartnerloc.IsShipTo() == true)
                    {
                        order.SetC_BPartner_Location_ID(fromProject.GetC_BPartner_Location_ID());
                        order.SetAD_User_ID(fromProject.GetAD_User_ID());
                    }
                    if (bpartnerloc.IsBillTo() == true)
                    {
                        order.SetBill_Location_ID(fromProject.GetC_BPartner_Location_ID());
                        order.SetBill_User_ID(fromProject.GetAD_User_ID());
                    }
                }
                if (C_BPartnerSR_ID != 0)
                {
                    String sqlcust = "update c_bpartner set iscustomer='Y', isprospect='N' where c_bpartner_id=" + C_BPartnerSR_ID + "";
                    value = DB.ExecuteQuery(sqlcust, null, Get_TrxName());
                    if (value == -1)
                    {

                    }
                    //bp.SetIsCustomer(true);
                    //bp.SetIsProspect(false);
                    order.SetC_BPartner_ID(fromProject.GetC_BPartnerSR_ID());

                    if (bpartnerloc.IsShipTo() == true)
                    {
                        order.SetC_BPartner_Location_ID(fromProject.GetC_BPartner_Location_ID());
                        order.SetAD_User_ID(fromProject.GetAD_User_ID());
                    }
                    if (bpartnerloc.IsBillTo() == true)
                    {
                        order.SetBill_Location_ID(fromProject.GetC_BPartner_Location_ID());
                        order.SetBill_User_ID(fromProject.GetAD_User_ID());
                    }
                }
               // String sql = "select c_doctype_id from c_doctype where name = 'Standard Order'";

                String sql = "select c_doctype_id from c_doctype where docbasetype = 'SOO' and docsubtypeso = 'SO' and isreturntrx = 'N' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                int Doctype_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql));
                order.SetM_PriceList_ID(Util.GetValueOfInt(fromProject.GetM_PriceList_ID()));
                //String sqlmpricelist = "select m_pricelist_id from m_pricelist where name='Export'";
                //int MPriceList_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlmpricelist));
                //if (MPriceList_id == order.GetM_PriceList_ID())
                //{
                //    String sqlconversiontype = "select c_conversiontype_id from c_conversiontype where value = 'C'";
                //    int C_ConversionType_id = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlconversiontype));
                //    order.SetC_ConversionType_ID(C_ConversionType_id);
                //}
                order.SetC_Project_ID(GetRecord_ID());
                order.SetSalesRep_ID(fromProject.GetSalesRep_ID());
                order.SetC_Currency_ID(fromProject.GetC_Currency_ID());
                if (C_Bpartner_id != 0)
                {
                    VAdvantage.Model.MBPartner bp = new VAdvantage.Model.MBPartner(GetCtx(), C_Bpartner_id, Get_TrxName());
                    bp.SetC_Campaign_ID(fromProject.GetC_Campaign_ID());
                    bp.SetAD_Client_ID(fromProject.GetAD_Client_ID());
                    bp.SetAD_Org_ID(fromProject.GetAD_Org_ID());
                    if (bp.GetC_PaymentTerm_ID() != 0)
                    {
                        order.SetPaymentMethod(bp.GetPaymentRule());
                        order.SetC_PaymentTerm_ID(bp.GetC_PaymentTerm_ID());

                    }

                    if (!bp.Save())
                    {
                        log.SaveError("CampaignIDtNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "CampaignIDtNotSaved");
                    }
                }
                else
                {
                    VAdvantage.Model.MBPartner bp = new VAdvantage.Model.MBPartner(GetCtx(), C_BPartnerSR_ID, Get_TrxName());
                    bp.SetC_Campaign_ID(fromProject.GetC_Campaign_ID());
                    bp.SetAD_Client_ID(fromProject.GetAD_Client_ID());
                    bp.SetAD_Org_ID(fromProject.GetAD_Org_ID());
                    if (bp.GetC_PaymentTerm_ID() != 0)
                    {
                        order.SetPaymentMethod(bp.GetPaymentRule());
                        order.SetC_PaymentTerm_ID(bp.GetC_PaymentTerm_ID());

                    }

                    if (!bp.Save())
                    {
                        log.SaveError("CampaignIDtNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "CampaignIDtNotSaved");
                    }
                }
                //if (bp.GetC_PaymentTerm_ID() != 0)
                //{
                //    order.SetPaymentMethod(bp.GetPaymentRule());
                //    order.SetC_PaymentTerm_ID(bp.GetC_PaymentTerm_ID());

                //}
                order.SetFreightCostRule("I");
                order.SetC_Campaign_ID(fromProject.GetC_Campaign_ID());
                order.SetDocStatus("IP");
                order.SetC_DocType_ID(Doctype_id);
                order.SetIsSOTrx(true);
                order.SetC_DocTypeTarget_ID(Doctype_id);
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
                fromProject.SetC_Order_ID(order.GetC_Order_ID());
                fromProject.SetC_BPartner_ID(fromProject.GetC_BPartnerSR_ID());
                fromProject.SetC_BPartnerSR_ID(0);
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
