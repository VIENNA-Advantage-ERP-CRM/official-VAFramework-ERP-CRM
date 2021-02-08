/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ProjectGenOrder
 * Purpose        : Generate Sales Order from Project.
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           07-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class ProjectGenOrder : ProcessEngine.SvrProcess
    {
        /**	Project ID from project directly		*/
        private int _VAB_Project_ID = 0;
        /**StingBuilder                    */
        private StringBuilder _msg = new StringBuilder();

        /**
         *  Prepare - e.g., get Parameters.
         */
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
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAB_Project_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            log.Info("VAB_Project_ID=" + _VAB_Project_ID);
            if (_VAB_Project_ID == 0)
            {
                throw new ArgumentException("VAB_Project_ID == 0");
            }
            MProject fromProject = GetProject(GetCtx(), _VAB_Project_ID, Get_TrxName());
            GetCtx().SetIsSOTrx(true);	//	Set SO context

            /** @todo duplicate invoice prevention */
            //Added by Vivek for Credit Limit on 24/08/2016
            if (fromProject.GetVAB_BusinessPartner_ID() != 0)
            {
                VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), fromProject.GetVAB_BusinessPartner_ID(), Get_TrxName());
                if (bp.GetCreditStatusSettingOn() == "CH")
                {
                    if (bp.GetCreditValidation() == "A" || bp.GetCreditValidation() == "D" || bp.GetCreditValidation() == "E")
                    {
                        log.SaveError("StopOrder", "");
                        return Msg.GetMsg(GetCtx(), "StopOrder");
                    }
                    else if (bp.GetCreditValidation() == "G" || bp.GetCreditValidation() == "J" || bp.GetCreditValidation() == "K")
                    {
                        if (_msg != null)
                        {
                            _msg.Clear();
                        }
                        log.SaveError("WarningOrder", "");
                        _msg.Append(Msg.GetMsg(GetCtx(), "WarningOrder"));
                    }
                }
                else
                {
                    VAdvantage.Model.MVABBPartLocation loc = new VAdvantage.Model.MVABBPartLocation(GetCtx(), fromProject.GetVAB_BPart_Location_ID(), Get_TrxName());
                    if (loc.GetCreditValidation() == "A" || loc.GetCreditValidation() == "D" || loc.GetCreditValidation() == "E")
                    {
                        log.SaveError("StopOrder", "");
                        return Msg.GetMsg(GetCtx(), "StopOrder");
                    }
                    else if (loc.GetCreditValidation() == "G" || loc.GetCreditValidation() == "J" || loc.GetCreditValidation() == "K")
                    {
                        if (_msg != null)
                        {
                            _msg.Clear();
                        }
                        log.SaveError("WarningOrder", "");
                        _msg.Append(Msg.GetMsg(GetCtx(), "WarningOrder"));
                    }
                }
            }
            //Credit Limit
            MVABOrder order = new MVABOrder(fromProject, true, MVABOrder.DocSubTypeSO_OnCredit);
            if (!order.Save())
            {
                return GetRetrievedError(order, "Could not create Order");
                //throw new Exception("Could not create Order");
            }

            //	***	Lines ***
            int count = 0;

            //	Service Project	
            if (MProject.PROJECTCATEGORY_ServiceChargeProject.Equals(fromProject.GetProjectCategory()))
            {
                /** @todo service project invoicing */
                throw new Exception("Service Charge Projects are on the TODO List");
            }	//	Service Lines

            else	//	Order Lines
            {
                MProjectLine[] lines = fromProject.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    MVABOrderLine ol = new MVABOrderLine(order);
                    ol.SetLine(lines[i].GetLine());
                    ol.SetDescription(lines[i].GetDescription());
                    //
                    ol.SetVAM_Product_ID(lines[i].GetVAM_Product_ID(), true);

                    //ol.SetQty(lines[i].GetPlannedQty().subtract(lines[i].GetInvoicedQty()));
                    ol.SetQty(Decimal.Subtract(lines[i].GetPlannedQty(), lines[i].GetInvoicedQty()));
                    ol.SetPrice();
                    if (lines[i].GetPlannedPrice().CompareTo(Env.ZERO) != 0)
                    {
                        ol.SetPrice(lines[i].GetPlannedPrice());
                    }
                    ol.SetDiscount();
                    ol.SetTax();
                    if (ol.Save())
                    {
                        count++;
                    }
                }	//	for all lines
                if (lines.Length != count)
                {
                    log.Log(Level.SEVERE, "Lines difference - ProjectLines=" + lines.Length + " <> Saved=" + count);
                }
            }	//	Order Lines
            if (_msg != null)
            {
                return "@VAB_Order_ID@ " + order.GetDocumentNo() + " (" + count + ")" + "  " + _msg;
            }
            else
            {
                return "@VAB_Order_ID@ " + order.GetDocumentNo() + " (" + count + ")";
            }
        }	//	doIt

        /// <summary>
        /// Get and validate Project
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Project_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>valid project</returns>
        static internal MProject GetProject(Ctx ctx, int VAB_Project_ID, Trx trxName)
        {
            MProject fromProject = new MProject(ctx, VAB_Project_ID, trxName);
            if (fromProject.GetVAB_Project_ID() == 0)
            {
                throw new ArgumentException("Project not found VAB_Project_ID=" + VAB_Project_ID);
            }
            if (fromProject.GetVAM_PriceListVersion_ID() == 0)
            {
                throw new ArgumentException("Project has no Price List");
            }
            if (fromProject.GetVAM_Warehouse_ID() == 0)
            {
                throw new ArgumentException("Project has no Warehouse");
            }
            if (fromProject.GetVAB_BusinessPartner_ID() == 0 || fromProject.GetVAB_BPart_Location_ID() == 0)
            {
                throw new ArgumentException("Project has no Business Partner/Location");
            }
            return fromProject;
        }	//	getProject

    }	//	ProjectGenOrder

}
