/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWFEventAudit
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      04-May-2009 
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
using VAdvantage.Utility;
namespace VAdvantage.WF
{
    public class MVAFWFlowEventLog : X_VAF_WFlow_EventLog
    {
        //	Logger			
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFWFlowEventLog).FullName);

        /// <summary>
        /// Get Event Audit for node
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Handler_ID">process</param>
        /// <param name="VAF_WFlow_Node_ID">optional node</param>
        /// <returns>event audit or null</returns>
        public static MVAFWFlowEventLog[] Get(Ctx ctx, int VAF_WFlow_Handler_ID, int VAF_WFlow_Node_ID)
        {
            List<MVAFWFlowEventLog> list = new List<MVAFWFlowEventLog>();
            String sql = "SELECT * FROM VAF_WFlow_EventLog "
                + "WHERE VAF_WFlow_Handler_ID=" + VAF_WFlow_Handler_ID;
            if (VAF_WFlow_Node_ID > 0)
            {
                sql += " AND VAF_WFlow_Node_ID=" + VAF_WFlow_Node_ID;
            }
            sql += " ORDER BY VAF_WFlow_EventLog_ID";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAFWFlowEventLog(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "get", e);
            }
            ds = null;
            MVAFWFlowEventLog[] retValue = new MVAFWFlowEventLog[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Event Audit for node
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="VAF_WFlow_Handler_ID">process</param>
        /// <returns>event audit or null</returns>
        public static MVAFWFlowEventLog[] Get(Ctx ctx, int VAF_WFlow_Handler_ID)
        {
            return Get(ctx, VAF_WFlow_Handler_ID, 0);
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="VAF_WFlow_EventLog_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowEventLog(Ctx ctx, int VAF_WFlow_EventLog_ID, Trx trxName)
            : base(ctx, VAF_WFlow_EventLog_ID, trxName)
        {

        }

        /// <summary>
        /// Load Cosntructors
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowEventLog(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Activity Constructor
        /// </summary>
        /// <param name="activity">activity</param>
        public MVAFWFlowEventLog(MVAFWFlowTask activity)
            : base(activity.GetCtx(), 0, activity.Get_TrxName())
        {
            SetVAF_WFlow_Handler_ID(activity.GetVAF_WFlow_Handler_ID());
            SetVAF_WFlow_Node_ID(activity.GetVAF_WFlow_Node_ID());
            SetVAF_TableView_ID(activity.GetVAF_TableView_ID());
            SetRecord_ID(activity.GetRecord_ID());
            SetVAF_WFlow_Incharge_ID(activity.GetVAF_WFlow_Incharge_ID());
            SetVAF_UserContact_ID(activity.GetVAF_UserContact_ID());
            SetWFState(activity.GetWFState());
            SetEventType(EVENTTYPE_ProcessCreated);
            SetElapsedTimeMS(Utility.Env.ZERO);
            MVAFWFlowNode node = activity.GetNode();
            if (node != null && node.Get_ID() != 0)
            {
                String action = node.GetAction();
                if (MVAFWFlowNode.ACTION_SetVariable.Equals(action)
                    || MVAFWFlowNode.ACTION_UserChoice.Equals(action))
                {
                    SetAttributeName(node.GetAttributeName());
                    //SetOldValue(String.valueOf(activity.getAttributeValue()));
                    SetOldValue(Util.GetValueOfString(activity.GetAttributeValue()));
                    if (MVAFWFlowNode.ACTION_SetVariable.Equals(action))
                        SetNewValue(node.GetAttributeValue());
                }
            }
        } 

        /// <summary>
        /// Get Node Name
        /// </summary>
        /// <returns>node name</returns>
        public String GetNodeName()
        {
            MVAFWFlowNode node = MVAFWFlowNode.Get(GetCtx(), GetVAF_WFlow_Node_ID());
            if (node.Get_ID() == 0)
                return "?";
            return node.GetName(true);
        }
    }
}
