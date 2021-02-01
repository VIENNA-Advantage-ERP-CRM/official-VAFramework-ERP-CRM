/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WFActivityManage
 * Purpose        : 
 * Class Used     : WFActivityManage inherits SvrProcess
 * Chronological    Development
 * Raghunandan      05-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using VAdvantage.Model;
using VAdvantage.Process;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.WF;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.WF
{
    public class WFActivityManage : ProcessEngine.SvrProcess
    {
        #region Private variables
        //	Abort It				
        private bool p_IsAbort = false;
        // New User				
        private int p_VAF_UserContact_ID = 0;
        // New Responsible			
        private int p_VAF_WFlow_Incharge_ID = 0;
        // Record					
        private int p_VAF_WFlow_Task_ID = 0;
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
                else if (name.Equals("IsAbort"))
                    p_IsAbort = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("VAF_UserContact_ID"))
                    p_VAF_UserContact_ID = para[i].GetParameterAsInt();
                else if (name.Equals("VAF_WFlow_Incharge_ID"))
                    p_VAF_WFlow_Incharge_ID = para[i].GetParameterAsInt();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_VAF_WFlow_Task_ID = GetRecord_ID();
        }

        /// <summary>
        ///Perform Process.
        /// </summary>
        /// <returns>Message (variables are parsed)</returns>
        protected override String DoIt()
        {
            string msg = null;
            MVAFWFlowTask activity = new MVAFWFlowTask(GetCtx(), p_VAF_WFlow_Task_ID, Get_Trx());
            log.Info("" + activity);

            MVAFUserContact user = MVAFUserContact.Get(GetCtx(), GetVAF_UserContact_ID());
            //	Abort
            if (p_IsAbort)
            {
                msg = user.GetName() + ": Abort";
                activity.SetTextMsg(msg);
                activity.SetVAF_UserContact_ID(GetVAF_UserContact_ID());
                activity.SetWFState(StateEngine.STATE_ABORTED);
                //JID_0278 : To mark processing checkbox false.
                // Mohit 
                // Date : 22 May 2019
                MVAFTableView table = new MVAFTableView(GetCtx(), activity.GetVAF_TableView_ID(), null);
                PO po = MVAFTableView.GetPO(GetCtx(), table.GetTableName(), activity.GetRecord_ID(), Get_Trx());
                if (po != null && po.Get_ColumnIndex("Processing") >= 0)
                {
                    po.Set_Value("Processing", false);
                    po.Save();
                }
                return msg;
            }

            //	Change User
            if (p_VAF_UserContact_ID != 0 && activity.GetVAF_UserContact_ID() != p_VAF_UserContact_ID)
            {
                MVAFUserContact from = MVAFUserContact.Get(GetCtx(), activity.GetVAF_UserContact_ID());
                MVAFUserContact to = MVAFUserContact.Get(GetCtx(), p_VAF_UserContact_ID);
                msg = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                activity.SetTextMsg(msg);
                activity.SetVAF_UserContact_ID(p_VAF_UserContact_ID);
            }
            //	Change Responsible
            if (p_VAF_WFlow_Incharge_ID != 0 && activity.GetVAF_WFlow_Incharge_ID() != p_VAF_WFlow_Incharge_ID)
            {
                MVAFWFlowIncharge from = MVAFWFlowIncharge.Get(GetCtx(), activity.GetVAF_WFlow_Incharge_ID());
                MVAFWFlowIncharge to = MVAFWFlowIncharge.Get(GetCtx(), p_VAF_WFlow_Incharge_ID);
                String msg1 = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                activity.SetTextMsg(msg1);
                activity.SetVAF_WFlow_Incharge_ID(p_VAF_WFlow_Incharge_ID);
                if (msg == null)
                    msg = msg1;
                else
                    msg += " - " + msg1;
            }
            activity.Save();
            return msg;
        }
    }
}
