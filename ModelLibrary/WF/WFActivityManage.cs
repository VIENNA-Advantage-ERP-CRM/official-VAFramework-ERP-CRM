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
        private int p_AD_User_ID = 0;
        // New Responsible			
        private int p_AD_WF_Responsible_ID = 0;
        // Record					
        private int p_AD_WF_Activity_ID = 0;
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
                else if (name.Equals("AD_User_ID"))
                    p_AD_User_ID = para[i].GetParameterAsInt();
                else if (name.Equals("AD_WF_Responsible_ID"))
                    p_AD_WF_Responsible_ID = para[i].GetParameterAsInt();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_AD_WF_Activity_ID = GetRecord_ID();
        }

        /// <summary>
        ///Perform Process.
        /// </summary>
        /// <returns>Message (variables are parsed)</returns>
        protected override String DoIt()
        {
            string msg = null;
            MWFActivity activity = new MWFActivity(GetCtx(), p_AD_WF_Activity_ID, Get_Trx());
            log.Info("" + activity);

            MUser user = MUser.Get(GetCtx(), GetAD_User_ID());
            //	Abort
            if (p_IsAbort)
            {
                msg = user.GetName() + ": Abort";
                activity.SetTextMsg(msg);
                activity.SetAD_User_ID(GetAD_User_ID());
                activity.SetWFState(StateEngine.STATE_ABORTED);
                return msg;
            }

            //	Change User
            if (p_AD_User_ID != 0 && activity.GetAD_User_ID() != p_AD_User_ID)
            {
                MUser from = MUser.Get(GetCtx(), activity.GetAD_User_ID());
                MUser to = MUser.Get(GetCtx(), p_AD_User_ID);
                msg = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                activity.SetTextMsg(msg);
                activity.SetAD_User_ID(p_AD_User_ID);
            }
            //	Change Responsible
            if (p_AD_WF_Responsible_ID != 0 && activity.GetAD_WF_Responsible_ID() != p_AD_WF_Responsible_ID)
            {
                MWFResponsible from = MWFResponsible.Get(GetCtx(), activity.GetAD_WF_Responsible_ID());
                MWFResponsible to = MWFResponsible.Get(GetCtx(), p_AD_WF_Responsible_ID);
                String msg1 = user.GetName() + ": " + from.GetName() + " -> " + to.GetName();
                activity.SetTextMsg(msg1);
                activity.SetAD_WF_Responsible_ID(p_AD_WF_Responsible_ID);
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
