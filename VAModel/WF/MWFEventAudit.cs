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
    public class MWFEventAudit : X_AD_WF_EventAudit
    {
        //	Logger			
        private static VLogger _log = VLogger.GetVLogger(typeof(MWFEventAudit).FullName);

        /// <summary>
        /// Get Event Audit for node
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Process_ID">process</param>
        /// <param name="AD_WF_Node_ID">optional node</param>
        /// <returns>event audit or null</returns>
        public static MWFEventAudit[] Get(Ctx ctx, int AD_WF_Process_ID, int AD_WF_Node_ID)
        {
            List<MWFEventAudit> list = new List<MWFEventAudit>();
            String sql = "SELECT * FROM AD_WF_EventAudit "
                + "WHERE AD_WF_Process_ID=" + AD_WF_Process_ID;
            if (AD_WF_Node_ID > 0)
            {
                sql += " AND AD_WF_Node_ID=" + AD_WF_Node_ID;
            }
            sql += " ORDER BY AD_WF_EventAudit_ID";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MWFEventAudit(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "get", e);
            }
            ds = null;
            MWFEventAudit[] retValue = new MWFEventAudit[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Event Audit for node
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="AD_WF_Process_ID">process</param>
        /// <returns>event audit or null</returns>
        public static MWFEventAudit[] Get(Ctx ctx, int AD_WF_Process_ID)
        {
            return Get(ctx, AD_WF_Process_ID, 0);
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="AD_WF_EventAudit_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWFEventAudit(Ctx ctx, int AD_WF_EventAudit_ID, Trx trxName)
            : base(ctx, AD_WF_EventAudit_ID, trxName)
        {

        }

        /// <summary>
        /// Load Cosntructors
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MWFEventAudit(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Activity Constructor
        /// </summary>
        /// <param name="activity">activity</param>
        public MWFEventAudit(MWFActivity activity)
            : base(activity.GetCtx(), 0, activity.Get_TrxName())
        {
            SetAD_WF_Process_ID(activity.GetAD_WF_Process_ID());
            SetAD_WF_Node_ID(activity.GetAD_WF_Node_ID());
            SetAD_Table_ID(activity.GetAD_Table_ID());
            SetRecord_ID(activity.GetRecord_ID());
            SetAD_WF_Responsible_ID(activity.GetAD_WF_Responsible_ID());
            SetAD_User_ID(activity.GetAD_User_ID());
            SetWFState(activity.GetWFState());
            SetEventType(EVENTTYPE_ProcessCreated);
            SetElapsedTimeMS(Utility.Env.ZERO);
            MWFNode node = activity.GetNode();
            if (node != null && node.Get_ID() != 0)
            {
                String action = node.GetAction();
                if (MWFNode.ACTION_SetVariable.Equals(action)
                    || MWFNode.ACTION_UserChoice.Equals(action))
                {
                    SetAttributeName(node.GetAttributeName());
                    //SetOldValue(String.valueOf(activity.getAttributeValue()));
                    SetOldValue(Util.GetValueOfString(activity.GetAttributeValue()));
                    if (MWFNode.ACTION_SetVariable.Equals(action))
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
            MWFNode node = MWFNode.Get(GetCtx(), GetAD_WF_Node_ID());
            if (node.Get_ID() == 0)
                return "?";
            return node.GetName(true);
        }
    }
}
