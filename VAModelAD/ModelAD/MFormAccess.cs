/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_Page_Rights
 * Chronological Development
 * Veena Pandey     31-Aug-09
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MFormAccess : X_VAF_Page_Rights
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">id=0</param>
        /// <param name="trxName">transaction</param>
        public MFormAccess(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
            {
                throw new ArgumentException("Multi-Key");
            }
            else
            {
                //	setVAF_Page_ID (0);
                //	setVAF_Role_ID (0);
                SetIsReadWrite(true);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MFormAccess(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="VAF_Role_ID">role id</param>
        public MFormAccess(MForm parent, int VAF_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_Page_ID(parent.GetVAF_Page_ID());
            SetVAF_Role_ID(VAF_Role_ID);
        }
    }
}
