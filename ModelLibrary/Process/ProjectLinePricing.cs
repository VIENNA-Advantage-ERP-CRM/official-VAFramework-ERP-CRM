/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ProjectLinePricing
 * Purpose        : Request Callouts
 * Class Used     : CalloutEngine
 * Chronological    Development
 * Deepak           18-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ProjectLinePricing : ProcessEngine.SvrProcess
    {
        /**	Project Line from Record			*/
        private int m_VAB_ProjectLine_ID = 0;

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
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            m_VAB_ProjectLine_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            if (m_VAB_ProjectLine_ID == 0)
            {
                throw new ArgumentException("No Project Line");
            }
            MVABProjectLine projectLine = new MVABProjectLine(GetCtx(), m_VAB_ProjectLine_ID, Get_TrxName());
            log.Info("doIt - " + projectLine);
            if (projectLine.GetVAM_Product_ID() == 0)
            {
                throw new ArgumentException("No Product");
            }
            //
            MVABProject project = new MVABProject(GetCtx(), projectLine.GetVAB_Project_ID(), Get_TrxName());
            if (project.GetVAM_PriceList_ID() == 0)
            {
                throw new ArgumentException("No PriceList");
            }
            //
            Boolean isSOTrx = true;
            MVAMProductPricing pp = new MVAMProductPricing(projectLine.GetVAF_Client_ID(), projectLine.GetVAF_Org_ID(),
                projectLine.GetVAM_Product_ID(), project.GetVAB_BusinessPartner_ID(),
                projectLine.GetPlannedQty(), isSOTrx);
            pp.SetVAM_PriceList_ID(project.GetVAM_PriceList_ID());
            //vikas  mantis Issue ( 0000517)
            pp.SetVAM_PriceListVersion_ID(project.GetVAM_PriceListVersion_ID());
            //End
            pp.SetPriceDate(project.GetDateContract());
            //
            projectLine.SetPlannedPrice(pp.GetPriceStd());
            //projectLine.SetPlannedMarginAmt(pp.GetPriceStd().subtract(pp.GetPriceLimit()));
            projectLine.SetPlannedMarginAmt(Decimal.Subtract(pp.GetPriceStd(), pp.GetPriceLimit()));
            projectLine.Save();
            //
            String retValue = Msg.GetElement(GetCtx(), "PriceList") + pp.GetPriceList() + " - "
                + Msg.GetElement(GetCtx(), "PriceStd") + pp.GetPriceStd() + " - "
                + Msg.GetElement(GetCtx(), "PriceLimit") + pp.GetPriceLimit();
            return retValue;
        }	//	doIt

    }	
}
