/********************************************************
 * Module Name    : Process
 * Purpose        : 
 * Class Used     : X_VAF_Job_Rights
 * Chronological Development
 * Veena Pandey     07-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Classes;
using VAdvantage.Utility;

using VAdvantage.DataBase;
namespace VAdvantage.Model
{
   public class MVAFJobRights : X_VAF_Job_Rights
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MVAFJobRights(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            else
            {
                //	setVAF_Job_ID (0);
                //	setVAF_Role_ID (0);
                SetIsReadWrite(true);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFJobRights(Ctx ctx, System.Data.DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="VAF_Role_ID">role id</param>
        public MVAFJobRights(X_VAF_Job parent, int VAF_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_Job_ID(parent.GetVAF_Job_ID());
            SetVAF_Role_ID(VAF_Role_ID);
        }

    }
}
