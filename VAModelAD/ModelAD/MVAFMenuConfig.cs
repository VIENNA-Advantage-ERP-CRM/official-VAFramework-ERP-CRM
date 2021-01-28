/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : 
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan      27-April-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAFMenuConfig : X_VAF_MenuConfig
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFMenuConfig).FullName);

        /// <summary>
        /// Get menus with where clause
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="whereClause">whereClause where clause w/o the actual WHERE</param>
        /// <returns>MMenu</returns>
        public static MVAFMenuConfig[] Get(Ctx ctx, string whereClause)
        {
            string sql = "SELECT * FROM VAF_MenuConfig";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE " + whereClause;
            List<MVAFMenuConfig> list = new List<MVAFMenuConfig>();
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    //list.Add(new MColumn(GetCtx(), dr, Get_TrxName()));
                    list.Add(new MVAFMenuConfig(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                if (ds != null)
                {
                    ds = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            MVAFMenuConfig[] retValue = new MVAFMenuConfig[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_MenuConfig_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFMenuConfig(Ctx ctx, int VAF_MenuConfig_ID, Trx trxName)
            : base(ctx, VAF_MenuConfig_ID, trxName)
        {
            if (VAF_MenuConfig_ID == 0)
            {
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsReadOnly(false);	// N
                SetIsSummary(false);
                //	setName (null);
            }
        }

        /// <summary>
        /// Load Contrusctor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFMenuConfig(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Reset info
            if (IsSummary() && GetAction() != null)
            {
                SetAction(null);
            }
            string action = GetAction();
            if (action == null)
            {
                action = "";
            }
            //	Clean up references
            if (GetVAF_Screen_ID() != 0 && !action.Equals(ACTION_Window))
                SetVAF_Screen_ID(0);
            if (GetVAF_Page_ID() != 0 && !action.Equals(ACTION_Form))
                SetVAF_Page_ID(0);
            if (GetVAF_Workflow_ID() != 0 && !action.Equals(ACTION_WorkFlow))
                SetVAF_Workflow_ID(0);
            if (GetAD_Workbench_ID() != 0 && !action.Equals(ACTION_Workbench))
                SetAD_Workbench_ID(0);
            if (GetVAF_Task_ID() != 0 && !action.Equals(ACTION_Task))
                SetVAF_Task_ID(0);
            if (GetVAF_Job_ID() != 0
                && !(action.Equals(ACTION_Process) || action.Equals(ACTION_Report)))
                SetVAF_Job_ID(0);
            return true;
        }

        /// <summary>
        ///String Info
        /// </summary>
        /// <returns>info</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MMenu[");
            sb.Append(Get_ID())
                .Append("-").Append(GetAction())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }
}
