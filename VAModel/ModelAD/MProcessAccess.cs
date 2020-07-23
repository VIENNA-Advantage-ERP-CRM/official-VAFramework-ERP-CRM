/********************************************************
 * Module Name    : Process
 * Purpose        : 
 * Class Used     : X_AD_Process_Access
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
using VAdvantage.ProcessEngine;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
   public class MProcessAccess : X_AD_Process_Access
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MProcessAccess(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            else
            {
                //	setAD_Process_ID (0);
                //	setAD_Role_ID (0);
                SetIsReadWrite(true);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProcessAccess(Ctx ctx, System.Data.DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="AD_Role_ID">role id</param>
        public MProcessAccess(MProcess parent, int AD_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_Process_ID(parent.GetAD_Process_ID());
            SetAD_Role_ID(AD_Role_ID);
        }

    }
}
