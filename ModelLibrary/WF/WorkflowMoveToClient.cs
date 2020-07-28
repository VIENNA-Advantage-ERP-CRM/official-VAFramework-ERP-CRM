/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WorkflowMoveToClient
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
    public class WorkflowMoveToClient : ProcessEngine.SvrProcess
    {
        //	The new Client			
        private int p_AD_Client_ID = 0;
        // The Workflow			
        private int p_AD_Workflow_ID = 0;

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
                else if (name.Equals("AD_Client_ID"))
                    p_AD_Client_ID = para[i].GetParameterAsInt();
                else if (name.Equals("AD_Workflow_ID"))
                    p_AD_Workflow_ID = para[i].GetParameterAsInt();
                else
                   log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
        }	//	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("doIt - AD_Client_ID=" + p_AD_Client_ID + ", AD_Workflow_ID=" + p_AD_Workflow_ID);

            int changes = 0;
            //	WF
            String sql = "UPDATE AD_Workflow SET AD_Client_ID=" + p_AD_Client_ID
                + " WHERE AD_Client_ID=0 AND EntityType NOT IN ('D','C')"
                + " AND AD_Workflow_ID=" + p_AD_Workflow_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow");
            changes += no;

            //	Node AD_WF_Node table
            sql = "UPDATE AD_WF_Node SET AD_Client_ID=" + p_AD_Client_ID
                + " WHERE AD_Client_ID=0 AND EntityType NOT IN ('D','C')"
                + " AND AD_Workflow_ID=" + p_AD_Workflow_ID;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Node");
            changes += no;

            //	Node Next from AD_WF_NodeNext table
            sql = "UPDATE AD_WF_NodeNext SET AD_Client_ID=" + p_AD_Client_ID
                + " WHERE AD_Client_ID=0 AND EntityType NOT IN ('D','C')"
                + " AND (AD_WF_Node_ID IN (SELECT AD_WF_Node_ID FROM AD_WF_Node WHERE AD_Workflow_ID=" + p_AD_Workflow_ID
                    + ") OR AD_WF_Next_ID IN (SELECT AD_WF_Node_ID FROM AD_WF_Node WHERE AD_Workflow_ID=" + p_AD_Workflow_ID
                    + "))";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Transition");
            changes += no;

            //	Node Parameters from AD_WF_Node_Para table
            sql = "UPDATE AD_WF_Node_Para SET AD_Client_ID=" + p_AD_Client_ID
                + " WHERE AD_Client_ID=0 AND EntityType NOT IN ('D','C')"
                + " AND AD_WF_Node_ID IN (SELECT AD_WF_Node_ID FROM AD_WF_Node WHERE AD_Workflow_ID=" + p_AD_Workflow_ID
                + ")";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Node Parameters");
            changes += no;

            //	Node Next Condition
            sql = "UPDATE AD_WF_NextCondition SET AD_Client_ID=" + p_AD_Client_ID
                + " WHERE AD_Client_ID=0 AND EntityType NOT IN ('D','C')"
                + " AND AD_WF_NodeNext_ID IN ("
                    + "SELECT AD_WF_NodeNext_ID FROM AD_WF_NodeNext "
                    + "WHERE AD_WF_Node_ID IN (SELECT AD_WF_Node_ID FROM AD_WF_Node WHERE AD_Workflow_ID=" + p_AD_Workflow_ID
                    + ") OR AD_WF_Next_ID IN (SELECT AD_WF_Node_ID FROM AD_WF_Node WHERE AD_Workflow_ID=" + p_AD_Workflow_ID
                    + "))";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Transition Condition");
            changes += no;

            return "@Updated@ - #" + changes;
        }	

    }
}
