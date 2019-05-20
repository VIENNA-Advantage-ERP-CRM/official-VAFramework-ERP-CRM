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
        private int p_AD_User_ID = 0;
        // New Responsible		
        private int p_AD_WF_Responsible_ID = 0;
        // Record				
        private int p_AD_WF_Process_ID = 0;
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
                else if (name.Equals("AD_User_ID"))
                    p_AD_User_ID = para[i].GetParameterAsInt();
                else if (name.Equals("AD_WF_Responsible_ID"))
                    p_AD_WF_Responsible_ID = para[i].GetParameterAsInt();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_AD_WF_Process_ID = GetRecord_ID();
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (variables are parsed)</returns>
        protected override String DoIt()
        {
            MWFProcess process = new MWFProcess(GetCtx(), p_AD_WF_Process_ID, Get_Trx());
            log.Info("doIt - " + process);

            MUser user = MUser.Get(GetCtx(), GetAD_User_ID());
            //	Abort
            if (p_IsAbort)
            {
                msg = user.GetName() + ": Abort";
                process.SetTextMsg(msg);
                process.SetAD_User_ID(GetAD_User_ID());
                process.SetWFState(StateEngine.STATE_ABORTED);
                return msg;
            }

            //	Change User
            if (p_AD_User_ID != 0 && process.GetAD_User_ID() != p_AD_User_ID)
            {
                MUser from = MUser.Get(GetCtx(), process.GetAD_User_ID());
                MUser to = MUser.Get(GetCtx(), p_AD_User_ID);
                msg = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                process.SetTextMsg(msg);
                process.SetAD_User_ID(p_AD_User_ID);
            }
            //	Change Responsible
            if (p_AD_WF_Responsible_ID != 0 && process.GetAD_WF_Responsible_ID() != p_AD_WF_Responsible_ID)
            {
                MWFResponsible from = MWFResponsible.Get(GetCtx(), process.GetAD_WF_Responsible_ID());
                MWFResponsible to = MWFResponsible.Get(GetCtx(), p_AD_WF_Responsible_ID);
                String msg1 = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                process.SetTextMsg(msg1);
                process.SetAD_WF_Responsible_ID(p_AD_WF_Responsible_ID);
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
