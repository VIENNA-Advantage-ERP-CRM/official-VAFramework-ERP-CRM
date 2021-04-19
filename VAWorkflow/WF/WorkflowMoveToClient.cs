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
        private int p_VAF_Client_ID = 0;
        // The Workflow			
        private int p_VAF_Workflow_ID = 0;

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
                else if (name.Equals("VAF_Client_ID"))
                    p_VAF_Client_ID = para[i].GetParameterAsInt();
                else if (name.Equals("VAF_Workflow_ID"))
                    p_VAF_Workflow_ID = para[i].GetParameterAsInt();
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
            log.Info("doIt - VAF_Client_ID=" + p_VAF_Client_ID + ", VAF_Workflow_ID=" + p_VAF_Workflow_ID);

            int changes = 0;
            //	WF
            String sql = "UPDATE VAF_Workflow SET VAF_Client_ID=" + p_VAF_Client_ID
                + " WHERE VAF_Client_ID=0 AND RecordType NOT IN ('D','C')"
                + " AND VAF_Workflow_ID=" + p_VAF_Workflow_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow");
            changes += no;

            //	Node VAF_WFlow_Node table
            sql = "UPDATE VAF_WFlow_Node SET VAF_Client_ID=" + p_VAF_Client_ID
                + " WHERE VAF_Client_ID=0 AND RecordType NOT IN ('D','C')"
                + " AND VAF_Workflow_ID=" + p_VAF_Workflow_ID;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Node");
            changes += no;

            //	Node Next from VAF_WFlow_NextNode table
            sql = "UPDATE VAF_WFlow_NextNode SET VAF_Client_ID=" + p_VAF_Client_ID
                + " WHERE VAF_Client_ID=0 AND RecordType NOT IN ('D','C')"
                + " AND (VAF_WFlow_Node_ID IN (SELECT VAF_WFlow_Node_ID FROM VAF_WFlow_Node WHERE VAF_Workflow_ID=" + p_VAF_Workflow_ID
                    + ") OR VAF_WF_Next_ID IN (SELECT VAF_WFlow_Node_ID FROM VAF_WFlow_Node WHERE VAF_Workflow_ID=" + p_VAF_Workflow_ID
                    + "))";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Transition");
            changes += no;

            //	Node Parameters from VAF_WFlow_Node_Para table
            sql = "UPDATE VAF_WFlow_Node_Para SET VAF_Client_ID=" + p_VAF_Client_ID
                + " WHERE VAF_Client_ID=0 AND RecordType NOT IN ('D','C')"
                + " AND VAF_WFlow_Node_ID IN (SELECT VAF_WFlow_Node_ID FROM VAF_WFlow_Node WHERE VAF_Workflow_ID=" + p_VAF_Workflow_ID
                + ")";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Node Parameters");
            changes += no;

            //	Node Next Condition
            sql = "UPDATE VAF_WFlow_NextCondition SET VAF_Client_ID=" + p_VAF_Client_ID
                + " WHERE VAF_Client_ID=0 AND RecordType NOT IN ('D','C')"
                + " AND VAF_WFlow_NextNode_ID IN ("
                    + "SELECT VAF_WFlow_NextNode_ID FROM VAF_WFlow_NextNode "
                    + "WHERE VAF_WFlow_Node_ID IN (SELECT VAF_WFlow_Node_ID FROM VAF_WFlow_Node WHERE VAF_Workflow_ID=" + p_VAF_Workflow_ID
                    + ") OR VAF_WF_Next_ID IN (SELECT VAF_WFlow_Node_ID FROM VAF_WFlow_Node WHERE VAF_Workflow_ID=" + p_VAF_Workflow_ID
                    + "))";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no == -1)
                throw new SystemException("Error updating Workflow Transition Condition");
            changes += no;

            return "@Updated@ - #" + changes;
        }	

    }
}
