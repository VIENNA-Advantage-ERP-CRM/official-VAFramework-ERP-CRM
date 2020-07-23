/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_Form_Access
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
    public class MFormAccess : X_AD_Form_Access
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
                //	setAD_Form_ID (0);
                //	setAD_Role_ID (0);
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
        /// <param name="AD_Role_ID">role id</param>
        public MFormAccess(MForm parent, int AD_Role_ID)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_Form_ID(parent.GetAD_Form_ID());
            SetAD_Role_ID(AD_Role_ID);
        }
    }
}
