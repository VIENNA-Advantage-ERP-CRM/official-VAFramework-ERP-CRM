using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
//using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Classes;

using VAdvantage.Model;

namespace VAdvantage.Process
{
    public class ProjectGenPO : SvrProcess
    {
        /** Project Parameter			*/
        private int m_C_Project_ID = 0;
        /** Opt Project Line Parameter	*/
        private int m_C_ProjectLine_ID = 0;
        /** Consolidate Document		*/
        //private boolean m_ConsolidateDocument = true;
        private bool m_ConsolidateDocument = true;
        /** List of POs for Consolidation	*/
        //private ArrayList<MOrder> m_pos = new ArrayList<MOrder>();
        private List<MOrder> m_pos = new List<MOrder>();

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
                    continue;
                }

                else if (name.Equals("C_Project_ID"))
                {
                    //m_C_Project_ID = ((BigDecimal)element.getParameter()).intValue();
                    m_C_Project_ID = VAdvantage.Utility.Util.GetValueOfInt(VAdvantage.Utility.Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else if (name.Equals("C_ProjectLine_ID"))
                {
                    //m_C_ProjectLine_ID = ((BigDecimal)element.getParameter()).intValue();
                    m_C_ProjectLine_ID = VAdvantage.Utility.Util.GetValueOfInt(VAdvantage.Utility.Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    //m_ConsolidateDocument = "Y".equals(element.getParameter());
                    m_ConsolidateDocument = "Y".Equals(para[i].GetParameter()); ;
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        protected override String DoIt()
        {
            log.Info("doIt - C_Project_ID=" + m_C_Project_ID + " - C_ProjectLine_ID=" + m_C_ProjectLine_ID + " - Consolidate=" + m_ConsolidateDocument);
            if (m_C_ProjectLine_ID != 0)
            {
                MProjectLine projectLine = new MProjectLine(GetCtx(), m_C_ProjectLine_ID, Get_TrxName());
                MProject project = new MProject(GetCtx(), projectLine.GetC_Project_ID(), Get_TrxName());
                CreatePO(project, projectLine);
            }
            else
            {
                MProject project = new MProject(GetCtx(), m_C_Project_ID, Get_TrxName());
                MProjectLine[] lines = project.GetLines();
                //for (MProjectLine element : lines)
                for (int i = 0; i < lines.Length; i++)
                {
                    MProjectLine element = lines[i];
                    CreatePO(project, element);
                }
            }
            return "";
        }

        /**
	 * 	Create PO from Planned Amt/Qty
	 * 	@param projectLine project line
	 */
        private void CreatePO(MProject project, MProjectLine projectLine)
        {
            if (projectLine.GetM_Product_ID() == 0)
            {
                AddLog(projectLine.GetLine(), null, null, "Line has no Product");
                return;
            }
            if (projectLine.GetC_OrderPO_ID() != 0)
            {
                AddLog(projectLine.GetLine(), null, null, "Line was ordered previously");
                return;
            }

            //	PO Record
            MProductPO[] pos = MProductPO.GetOfProduct(GetCtx(), projectLine.GetM_Product_ID(), Get_TrxName());
            if (pos == null || pos.Length == 0)
            {
                AddLog(projectLine.GetLine(), null, null, "Product has no PO record");
                return;
            }

            //	Create to Order
            MOrder order = null;
            //	try to find PO to C_BPartner
            for (int i = 0; i < m_pos.Count; i++)
            {
                MOrder test = m_pos[i];
                if (test.GetC_BPartner_ID() == pos[0].GetC_BPartner_ID())
                {
                    order = test;
                    break;
                }
            }
            if (order == null)	//	create new Order
            {
                //	Vendor
                MBPartner bp = new MBPartner(GetCtx(), pos[0].GetC_BPartner_ID(), Get_TrxName());
                //	New Order
                order = new MOrder(project, false, null);
                int AD_Org_ID = projectLine.GetAD_Org_ID();
                if (AD_Org_ID == 0)
                {
                    log.Warning("createPOfromProjectLine - AD_Org_ID=0");
                    AD_Org_ID = GetCtx().GetAD_Org_ID();
                    if (AD_Org_ID != 0)
                        projectLine.SetAD_Org_ID(AD_Org_ID);
                }
                order.SetClientOrg(projectLine.GetAD_Client_ID(), AD_Org_ID);
                order.SetBPartner(bp);
                order.SetC_Project_ID(project.Get_ID());
                order.Save();
                //	optionally save for consolidation
                if (m_ConsolidateDocument)
                    m_pos.Add(order);
            }

            //	Create Line
            MOrderLine orderLine = new MOrderLine(order);
            orderLine.SetM_Product_ID(projectLine.GetM_Product_ID(), true);
            orderLine.SetQty(projectLine.GetPlannedQty());
            orderLine.SetDescription(projectLine.GetDescription());

            //	(Vendor) PriceList Price
            orderLine.SetPrice();
            if (Env.Signum(orderLine.GetPriceActual()) == 0)
            {
                //	Try to find purchase price
                Decimal poPrice = pos[0].GetPricePO();
                int C_Currency_ID = pos[0].GetC_Currency_ID();
                // 
                if ( Env.Signum(poPrice) == 0)
                    poPrice = pos[0].GetPriceLastPO();
                if ( Env.Signum(poPrice) == 0)
                    poPrice = pos[0].GetPriceList();
                //	We have a price
                if ( Env.Signum(poPrice) != 0)
                {
                    if (order.GetC_Currency_ID() != C_Currency_ID)
                        poPrice = VAdvantage.Model.MConversionRate.Convert(GetCtx(), poPrice,
                            C_Currency_ID, order.GetC_Currency_ID(),
                            order.GetDateAcct(), order.GetC_ConversionType_ID(),
                            order.GetAD_Client_ID(), order.GetAD_Org_ID());
                    orderLine.SetPrice(poPrice);
                }
            }

            orderLine.SetTax();
            orderLine.Save();

            // touch order to recalculate tax and totals
            order.SetIsActive(order.IsActive());
            order.Save();

            //	update ProjectLine
            projectLine.SetC_OrderPO_ID(order.GetC_Order_ID());
            projectLine.Save();
            AddLog(projectLine.GetLine(), null, projectLine.GetPlannedQty(), order.GetDocumentNo());
        }


    }  //	ProjectGenPO
}
