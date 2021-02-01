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
      //  private int VAB_Order_ID = 0;
        /**ProjectLine       */
        private int VAB_ProjectLine_ID = 0;

        #endregion

        protected override void Prepare()
        {
            _VAB_Project_ID = GetRecord_ID();
        }

        /// <summary>
        /// Generate Quotation
        /// </summary>
        /// <returns>Process Message</returns>
        protected override string DoIt()
        {
           // Int32 value = 0;
            string msg = "";
            ValueNamePair vp = null;
            MVABBusinessPartner bp = null;
            MOrderLine ol = null;

            log.Info("VAB_Project_ID=" + _VAB_Project_ID);
            if (_VAB_Project_ID == 0)
            {
                throw new ArgumentException("VAB_Project_ID == 0");
            }

           MProject fromProject = new MProject(GetCtx(), _VAB_Project_ID, Get_TrxName());

            if (fromProject.GetGenerate_Quotation() == null)
            {
                throw new ArgumentException("No Generate Quotation found on Project.");
            }
            if (fromProject.GetGenerate_Quotation().Trim() == "Y")
            {
                throw new ArgumentException("Sales Quotation already generated");
            }

            // if Business Partner or Prospect is not selected then gives error
            if (fromProject.GetVAB_BusinessPartner_ID() == 0 && fromProject.GetVAB_BusinessPartnerSR_ID() == 0)
            {
                return Msg.GetMsg(GetCtx(), "SelectBP/Prospect");
            }

            //JID_1200: if Business Partner/Prospect Location is not selected then gives error
            if (fromProject.GetVAB_BPart_Location_ID() == 0)
            {
                return Msg.GetMsg(GetCtx(), "SelectBPLocation");
            }

           MOrder order = new MOrder(GetCtx(), 0, Get_TrxName());
            order.SetVAF_Client_ID(fromProject.GetVAF_Client_ID());
            order.SetVAF_Org_ID(fromProject.GetVAF_Org_ID());
            VAB_BusinessPartner_id = fromProject.GetVAB_BusinessPartner_ID();
            VAB_BPart_Location_id = fromProject.GetVAB_BPart_Location_ID();
            VAB_BusinessPartnerSR_ID = fromProject.GetVAB_BusinessPartnerSR_ID();
            
            MVABBPartLocation bpartnerloc = new MVABBPartLocation(GetCtx(), VAB_BPart_Location_id, Get_TrxName());
            //String currentdate = DateTime.Now.ToString();
            String sqlprjln = "SELECT COUNT(VAB_ProjectLine_ID) FROM VAB_ProjectLine WHERE VAB_Project_ID=" + _VAB_Project_ID;
            VAB_ProjectLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlprjln, null, Get_TrxName()));
            if (VAB_ProjectLine_ID != 0)
            {
                order.SetDateOrdered(DateTime.Now.ToLocalTime());
                order.SetDatePromised(DateTime.Now.ToLocalTime());
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

                String sql = "SELECT VAB_DocTypes_ID FROM VAB_DocTypes WHERE DocBaseType = 'SOO' AND DocSubTypeSO = 'ON' AND IsReturnTrx = 'N' AND IsActive = 'Y' AND VAF_Client_ID = "
                            + GetCtx().GetVAF_Client_ID() + " AND VAF_Org_ID IN (0, " + GetVAF_Org_ID() + ") ORDER BY  VAF_Org_ID DESC";
                int Doctype_id = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                int MPriceList_id = fromProject.GetM_PriceList_ID();
                order.SetM_PriceList_ID(MPriceList_id);
               
                order.SetVAB_Project_ID(GetRecord_ID());
                if (fromProject.GetSalesRep_ID() > 0)
                    order.SetSalesRep_ID(fromProject.GetSalesRep_ID());
                order.SetVAB_Currency_ID(fromProject.GetVAB_Currency_ID());
                if (VAB_BusinessPartner_id != 0)
                {
                    bp = new MVABBusinessPartner(GetCtx(), VAB_BusinessPartner_id, Get_TrxName());
                    if (bp.GetVAB_Promotion_ID() == 0 && fromProject.GetVAB_Promotion_ID() > 0)
                        bp.SetVAB_Promotion_ID(fromProject.GetVAB_Promotion_ID());
                    //bp.SetVAF_Client_ID(fromProject.GetVAF_Client_ID());
                    //bp.SetVAF_Org_ID(fromProject.GetVAF_Org_ID());
                    if (bp.GetVAB_PaymentTerm_ID() != 0)
                    {
                        order.SetPaymentMethod(bp.GetPaymentRule());
                        order.SetVAB_PaymentTerm_ID(bp.GetVAB_PaymentTerm_ID());
                    }

                    if (!bp.Save())
                    {
                        log.SaveError("BPartnerNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "BPartnerNotSaved");
                    }
                }
                else
                {
                    bp = new MVABBusinessPartner(GetCtx(), VAB_BusinessPartnerSR_ID, Get_TrxName());
                    if (bp.GetVAB_Promotion_ID() == 0 && fromProject.GetVAB_Promotion_ID() > 0)
                        bp.SetVAB_Promotion_ID(fromProject.GetVAB_Promotion_ID());
                    //bp.SetVAF_Client_ID(fromProject.GetVAF_Client_ID());
                    //bp.SetVAF_Org_ID(fromProject.GetVAF_Org_ID());
                    if (bp.GetVAB_PaymentTerm_ID() != 0)
                    {
                        order.SetPaymentMethod(bp.GetPaymentRule());
                        order.SetVAB_PaymentTerm_ID(bp.GetVAB_PaymentTerm_ID());
                    }

                    if (!bp.Save())
                    {
                        log.SaveError("BPartnerNotSaved", "");
                        return Msg.GetMsg(GetCtx(), "BPartnerNotSaved");
                    }
                }
                
                order.SetFreightCostRule("I");
                if (order.GetVAB_Promotion_ID() == 0 && fromProject.GetVAB_Promotion_ID() > 0)
                    order.SetVAB_Promotion_ID(fromProject.GetVAB_Promotion_ID());
                order.SetDocStatus("IP");
                order.SetVAB_DocTypes_ID(Doctype_id);
                order.SetVAB_DocTypesTarget_ID(Doctype_id);
                order.SetIsSOTrx(true);
                order.Set_Value("IsSalesQuotation", true);
                if (!order.Save())
                {
                    Get_TrxName().Rollback();
                    vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        msg = vp.GetName();
                    }
                    else
                    {
                        msg = Msg.GetMsg(GetCtx(), "QuotationNotSaved");
                    }
                    log.SaveError("QuotationNotSaved", "");
                    return msg;
                }

                //Order Lines
                int count = 0;
                MProjectLine[] lines = fromProject.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    ol = new MOrderLine(order);
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
                    else
                    {
                        Get_TrxName().Rollback();
                        vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            msg = vp.GetName();
                        }
                        else
                        {
                            msg = Msg.GetMsg(GetCtx(), "QuoteLineNotSaved");
                        }
                        log.SaveError("QuoteLineNotSaved", "");
                        return msg;
                    }
                }

                fromProject.SetRef_Order_ID(order.GetVAB_Order_ID());
                fromProject.SetGenerate_Quotation("Y");
                if (!fromProject.Save())
                {
                    Get_TrxName().Rollback();
                    log.SaveError("ProjectNotSaved", "");
                    return Msg.GetMsg(GetCtx(), "ProjectNotSaved");
                }
                msg = Msg.GetMsg(GetCtx(), "QuotationGenerated");
            }
            else
                msg = Msg.GetMsg(GetCtx(), "NoLines");
            return msg;
        }
    }
}
