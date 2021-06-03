/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAFWFlowRights
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan      04-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
namespace VAdvantage.WF
{
    public class MVAFWFlowRights : X_VAF_WFlow_Rights
    {
        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored -</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowRights(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            else
            {
                //	setVAF_Role_ID (0);
                //	setVAF_Workflow_ID (0);
                SetIsReadWrite(true);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowRights(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="VAF_Role_ID">role id</param>
        public MVAFWFlowRights(MVAFWorkflow parent, int VAF_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_Workflow_ID(parent.GetVAF_Workflow_ID());
            SetVAF_Role_ID(VAF_Role_ID);
        }

    }
}
