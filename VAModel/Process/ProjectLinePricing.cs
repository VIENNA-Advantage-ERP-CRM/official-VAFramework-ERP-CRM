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
        private int m_C_ProjectLine_ID = 0;

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
            m_C_ProjectLine_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            if (m_C_ProjectLine_ID == 0)
            {
                throw new ArgumentException("No Project Line");
            }
            MProjectLine projectLine = new MProjectLine(GetCtx(), m_C_ProjectLine_ID, Get_TrxName());
            log.Info("doIt - " + projectLine);
            if (projectLine.GetM_Product_ID() == 0)
            {
                throw new ArgumentException("No Product");
            }
            //
            MProject project = new MProject(GetCtx(), projectLine.GetC_Project_ID(), Get_TrxName());
            if (project.GetM_PriceList_ID() == 0)
            {
                throw new ArgumentException("No PriceList");
            }
            //
            Boolean isSOTrx = true;
            MProductPricing pp = new MProductPricing(projectLine.GetAD_Client_ID(), projectLine.GetAD_Org_ID(),
                projectLine.GetM_Product_ID(), project.GetC_BPartner_ID(),
                projectLine.GetPlannedQty(), isSOTrx);
            pp.SetM_PriceList_ID(project.GetM_PriceList_ID());
            //vikas  mantis Issue ( 0000517)
            pp.SetM_PriceList_Version_ID(project.GetM_PriceList_Version_ID());
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
