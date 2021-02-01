/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WFProcessManage
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      06-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.WF
{
    public class WFProcessManage : ProcessEngine.SvrProcess
    {
        //Abort It				
        private bool p_IsAbort = false;
        //New User				
        private int p_VAF_UserContact_ID = 0;
        // New Responsible		
        private int p_VAF_WFlow_Incharge_ID = 0;
        // Record				
        private int p_VAF_WFlow_Handler_ID = 0;
        String msg = null;
        /// <summary>
        ///Prepare - e.g., get Parameters.
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
                else if (name.Equals("IsAbort"))
                    p_IsAbort = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("VAF_UserContact_ID"))
                    p_VAF_UserContact_ID = para[i].GetParameterAsInt();
                else if (name.Equals("VAF_WFlow_Incharge_ID"))
                    p_VAF_WFlow_Incharge_ID = para[i].GetParameterAsInt();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_VAF_WFlow_Handler_ID = GetRecord_ID();
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (variables are parsed)</returns>
        protected override String DoIt()
        {
            MVAFWFlowHandler process = new MVAFWFlowHandler(GetCtx(), p_VAF_WFlow_Handler_ID, Get_Trx());
            log.Info("doIt - " + process);

            MVAFUserContact user = MVAFUserContact.Get(GetCtx(), GetVAF_UserContact_ID());
            //	Abort
            if (p_IsAbort)
            {
                msg = user.GetName() + ": Abort";
                process.SetTextMsg(msg);
                process.SetVAF_UserContact_ID(GetVAF_UserContact_ID());
                process.SetWFState(StateEngine.STATE_ABORTED);

                //JID_0278 : To mark processing checkbox false.
                // Mohit 
                // Date : 22 May 2019
                MVAFTableView table = new MVAFTableView(GetCtx(), process.GetVAF_TableView_ID(), null);
                PO po = MVAFTableView.GetPO(GetCtx(), table.GetTableName(), process.GetRecord_ID(), Get_Trx());
                if (po != null && po.Get_ColumnIndex("Processing") >= 0)
                {
                    po.Set_Value("Processing", false);
                    po.Save();
                }
                return msg;

            }

            //	Change User
            if (p_VAF_UserContact_ID != 0 && process.GetVAF_UserContact_ID() != p_VAF_UserContact_ID)
            {
                MVAFUserContact from = MVAFUserContact.Get(GetCtx(), process.GetVAF_UserContact_ID());
                MVAFUserContact to = MVAFUserContact.Get(GetCtx(), p_VAF_UserContact_ID);
                msg = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                process.SetTextMsg(msg);
                process.SetVAF_UserContact_ID(p_VAF_UserContact_ID);
            }
            //	Change Responsible
            if (p_VAF_WFlow_Incharge_ID != 0 && process.GetVAF_WFlow_Incharge_ID() != p_VAF_WFlow_Incharge_ID)
            {
                MVAFWFlowIncharge from = MVAFWFlowIncharge.Get(GetCtx(), process.GetVAF_WFlow_Incharge_ID());
                MVAFWFlowIncharge to = MVAFWFlowIncharge.Get(GetCtx(), p_VAF_WFlow_Incharge_ID);
                String msg1 = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                process.SetTextMsg(msg1);
                process.SetVAF_WFlow_Incharge_ID(p_VAF_WFlow_Incharge_ID);
                if (msg == null)
                    msg = msg1;
                else
                    msg += " - " + msg1;
            }
            process.Save();

            return "OK";
        }

    }
}
